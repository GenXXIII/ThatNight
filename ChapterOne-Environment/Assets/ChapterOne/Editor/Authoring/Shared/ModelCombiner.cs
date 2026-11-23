using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThatNight.ChapterOne.Editor
{
    public static class ModelCombiner
    {
        /// <summary>
        /// Combines all child MeshFilters of the given root into the root itself,
        /// grouping by material into submeshes. Removes child geometry objects.
        /// Preserves special children (lights, cameras, or child ModelRoots).
        /// Adds an appropriate Collider matching the combined bounds.
        /// </summary>
        public static GameObject CombineModel(Transform modelRoot, bool addCollider = true, bool useCapsuleCollider = false)
        {
            if (modelRoot == null) return null;

            // Find all child MeshRenderers that are NOT part of a nested ModelRoot
            var childRenderers = new List<MeshRenderer>();
            CollectRenderers(modelRoot, modelRoot, childRenderers);

            if (childRenderers.Count == 0) return modelRoot.gameObject;

            // Group CombineInstances by Material
            var matGroups = new Dictionary<Material, List<CombineInstance>>();
            var defaultMat = SurfaceLibrary.Wood;

            foreach (var r in childRenderers)
            {
                var mf = r.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;

                var mats = r.sharedMaterials;
                for (int sub = 0; sub < mats.Length; sub++)
                {
                    var mat = (sub < mats.Length && mats[sub] != null) ? mats[sub] : defaultMat;
                    if (mat == null)
                    {
                        if (SurfaceLibrary.Wood != null) mat = SurfaceLibrary.Wood;
                        else mat = new Material(Shader.Find("Standard") ?? Shader.Find("Diffuse"));
                    }
                    if (!matGroups.TryGetValue(mat, out var materialMeshes))
                    {
                        materialMeshes = new List<CombineInstance>();
                        matGroups.Add(mat, materialMeshes);
                    }

                    var ci = new CombineInstance();
                    ci.mesh = mf.sharedMesh;
                    ci.subMeshIndex = Mathf.Clamp(sub, 0, mf.sharedMesh.subMeshCount - 1);
                    ci.transform = modelRoot.worldToLocalMatrix * r.transform.localToWorldMatrix;
                    materialMeshes.Add(ci);
                }
            }

            if (matGroups.Count == 0) return modelRoot.gameObject;

            // Create combined submeshes
            var submeshCombines = new List<CombineInstance>();
            var materialList = new List<Material>();

            foreach (var kvp in matGroups)
            {
                var subMesh = new Mesh();
                subMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                subMesh.CombineMeshes(kvp.Value.ToArray(), true, true);

                var ci = new CombineInstance();
                ci.mesh = subMesh;
                ci.transform = Matrix4x4.identity;
                submeshCombines.Add(ci);
                materialList.Add(kvp.Key);
            }

            var finalMesh = new Mesh();
            finalMesh.name = "Mesh_" + modelRoot.name;
            finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            finalMesh.CombineMeshes(submeshCombines.ToArray(), false, false);
            finalMesh.RecalculateBounds();

            // CombineMeshes copied the source normals and tangents. Recalculating them
            // here was expensive during every regeneration and could soften hard edges.
            foreach (var submeshCombine in submeshCombines)
                UnityEngine.Object.DestroyImmediate(submeshCombine.mesh);

            // Destroy child geometry objects (keep any nested ModelRoot or functional children)
            var toDestroy = new List<GameObject>();
            for (int i = 0; i < modelRoot.childCount; i++)
            {
                var child = modelRoot.GetChild(i).gameObject;
                if (child.GetComponent<ThatNight.ChapterOne.ModelRoot>() != null) continue;
                if (child.GetComponent<Light>() != null || child.GetComponent<Camera>() != null) continue;
                toDestroy.Add(child);
            }
            foreach (var go in toDestroy)
            {
                UnityEngine.Object.DestroyImmediate(go);
            }

            // Assign combined mesh and materials to modelRoot
            var rootMf = modelRoot.GetComponent<MeshFilter>();
            if (rootMf == null) rootMf = modelRoot.gameObject.AddComponent<MeshFilter>();
            rootMf.sharedMesh = finalMesh;

            var rootMr = modelRoot.GetComponent<MeshRenderer>();
            if (rootMr == null) rootMr = modelRoot.gameObject.AddComponent<MeshRenderer>();
            rootMr.sharedMaterials = materialList.ToArray();
            rootMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            rootMr.receiveShadows = true;

            // Ensure ModelRoot component exists
            var rootTag = modelRoot.GetComponent<ThatNight.ChapterOne.ModelRoot>();
            if (rootTag == null)
            {
                rootTag = modelRoot.gameObject.AddComponent<ThatNight.ChapterOne.ModelRoot>();
                rootTag.modelName = modelRoot.name;
            }

            // Add suitable Collider
            if (addCollider)
            {
                var bounds = finalMesh.bounds;
                if (useCapsuleCollider)
                {
                    var cap = modelRoot.GetComponent<CapsuleCollider>();
                    if (cap == null) cap = modelRoot.gameObject.AddComponent<CapsuleCollider>();
                    cap.center = bounds.center;
                    cap.height = Mathf.Max(bounds.size.y, 0.5f);
                    cap.radius = Mathf.Max(Mathf.Min(bounds.size.x, bounds.size.z) * 0.35f, 0.2f);
                }
                else
                {
                    var box = modelRoot.GetComponent<BoxCollider>();
                    if (box == null) box = modelRoot.gameObject.AddComponent<BoxCollider>();
                    box.center = bounds.center;
                    box.size = Vector3.Max(bounds.size, new Vector3(0.1f, 0.1f, 0.1f));
                }
            }

            modelRoot.gameObject.isStatic = false;
            return modelRoot.gameObject;
        }

        private static void CollectRenderers(Transform current, Transform root, List<MeshRenderer> results)
        {
            for (int i = 0; i < current.childCount; i++)
            {
                var child = current.GetChild(i);
                // Do not descend into a nested ModelRoot
                if (child.GetComponent<ThatNight.ChapterOne.ModelRoot>() != null) continue;

                var mr = child.GetComponent<MeshRenderer>();
                if (mr != null) results.Add(mr);

                CollectRenderers(child, root, results);
            }
        }

    }
}
