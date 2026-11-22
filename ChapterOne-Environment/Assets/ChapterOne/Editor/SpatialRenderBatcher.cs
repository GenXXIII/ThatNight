using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne.Editor
{
    /// <summary>
    /// Builds render-only spatial batches while retaining the authored hierarchy,
    /// colliders, transforms, and source meshes for editing and regeneration.
    /// </summary>
    internal static class SpatialRenderBatcher
    {
        const string BatchRootName = "09 RENDER BATCHES - generated performance geometry";
        const float HorizontalCellSize = 40f;
        const float VerticalCellSize = 20f;

        sealed class Batch
        {
            public Material material;
            public int layer;
            public ShadowCastingMode shadowCastingMode;
            public bool receiveShadows;
            public readonly List<CombineInstance> meshes = new List<CombineInstance>();
        }

        public static string Rebuild()
        {
            RestoreAuthoredRenderers();

            var batchRoot = new GameObject(BatchRootName);
            batchRoot.isStatic = true;
            var batches = new Dictionary<string, Batch>();
            var sources = new List<MeshRenderer>();
            int sourceSubmeshes = 0;

            foreach (var renderer in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
            {
                if (!CanBatch(renderer)) continue;

                var filter = renderer.GetComponent<MeshFilter>();
                var mesh = filter.sharedMesh;
                var materials = renderer.sharedMaterials;
                if (materials.Length == 0) continue;

                bool valid = true;
                for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                {
                    if (materials[Mathf.Min(submesh, materials.Length - 1)] == null)
                    {
                        valid = false;
                        break;
                    }
                }
                if (!valid) continue;

                var topLevel = TopLevel(renderer.transform);
                var center = renderer.bounds.center;

                for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                {
                    var material = materials[Mathf.Min(submesh, materials.Length - 1)];
                    bool transparent = material.renderQueue >= (int)RenderQueue.Transparent;
                    string spatialKey = transparent
                        ? renderer.GetInstanceID().ToString()
                        : CellKey(center);
                    string key = $"{topLevel.name}|{spatialKey}|{renderer.gameObject.layer}|" +
                                 $"{material.GetInstanceID()}";

                    if (!batches.TryGetValue(key, out var batch))
                    {
                        batch = new Batch
                        {
                            material = material,
                            layer = renderer.gameObject.layer,
                            shadowCastingMode = renderer.shadowCastingMode,
                            receiveShadows = renderer.receiveShadows
                        };
                        batches.Add(key, batch);
                    }
                    else
                    {
                        if (renderer.shadowCastingMode != ShadowCastingMode.Off)
                            batch.shadowCastingMode = ShadowCastingMode.On;
                        batch.receiveShadows |= renderer.receiveShadows;
                    }

                    batch.meshes.Add(new CombineInstance
                    {
                        mesh = mesh,
                        subMeshIndex = submesh,
                        transform = batchRoot.transform.worldToLocalMatrix *
                                    renderer.transform.localToWorldMatrix
                    });
                    sourceSubmeshes++;
                }

                sources.Add(renderer);
            }

            int batchIndex = 0;
            foreach (var pair in batches)
            {
                var batch = pair.Value;
                var mesh = new Mesh
                {
                    name = $"Environment render batch {batchIndex}",
                    indexFormat = IndexFormat.UInt32
                };
                mesh.CombineMeshes(batch.meshes.ToArray(), true, true);
                mesh.RecalculateBounds();
                const string meshFolder="Assets/ChapterOne/Gameplay/RenderBatches";
                System.IO.Directory.CreateDirectory(meshFolder);
                AssetDatabase.CreateAsset(mesh,AssetDatabase.GenerateUniqueAssetPath(meshFolder+"/Batch "+batchIndex+".asset"));

                var batchObject = new GameObject($"Batch {batchIndex:0000} - {batch.material.name}");
                batchObject.transform.SetParent(batchRoot.transform, false);
                batchObject.layer = batch.layer;
                batchObject.isStatic = true;
                batchObject.AddComponent<MeshFilter>().sharedMesh = mesh;

                var batchRenderer = batchObject.AddComponent<MeshRenderer>();
                batchRenderer.sharedMaterial = batch.material;
                batchRenderer.shadowCastingMode = batch.shadowCastingMode;
                batchRenderer.receiveShadows = batch.receiveShadows;
                batchRenderer.allowOcclusionWhenDynamic = true;
                batchIndex++;
            }

            foreach (var source in sources)
            {
                if(!source.GetComponent<RenderBatchSource>())source.gameObject.AddComponent<RenderBatchSource>();
                source.enabled = false;
                EditorUtility.SetDirty(source);
            }

            return $"renderBatches={batchIndex} sourceRenderers={sources.Count} " +
                   $"sourceSubmeshes={sourceSubmeshes}";
        }

        [MenuItem("THAT NIGHT/Restore authored renderers")]
        public static void RestoreAuthoredRenderers()
        {
            var existing = GameObject.Find(BatchRootName);
            if (existing != null)
                UnityEngine.Object.DestroyImmediate(existing);

            foreach (var marker in UnityEngine.Object.FindObjectsByType<RenderBatchSource>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var renderer=marker.GetComponent<MeshRenderer>();
                if(renderer)renderer.enabled=true;
                UnityEngine.Object.DestroyImmediate(marker);
            }
        }

        static bool CanBatch(MeshRenderer renderer)
        {
            if (renderer == null || !renderer.enabled) return false;
            if(renderer.GetComponentInParent<WorldInteractable>()||renderer.GetComponentInParent<FirstPersonBody>())return false;
            if (renderer.gameObject.layer == EnvironmentPerformance.DistantEnvironmentLayer) return false;

            var filter = renderer.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) return false;

            // Never batch house interior items, bedrooms, or furniture so they remain individually selectable
            var cur = renderer.transform;
            while (cur != null)
            {
                if (cur.name.IndexOf("bedroom", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    cur.name.IndexOf("bed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    cur.name.IndexOf("01 house", StringComparison.OrdinalIgnoreCase) >= 0)
                    return false;
                cur = cur.parent;
            }

            var top = TopLevel(renderer.transform).name;
            return top.StartsWith("02 ", StringComparison.Ordinal) ||
                   top.StartsWith("03 ", StringComparison.Ordinal) ||
                   top.StartsWith("04 ", StringComparison.Ordinal) ||
                   top.StartsWith("05 ", StringComparison.Ordinal) ||
                   top.StartsWith("08 ", StringComparison.Ordinal);
        }

        static Transform TopLevel(Transform transform)
        {
            while (transform.parent != null)
                transform = transform.parent;
            return transform;
        }

        static string CellKey(Vector3 center)
        {
            int x = Mathf.FloorToInt(center.x / HorizontalCellSize);
            int y = Mathf.FloorToInt(center.y / VerticalCellSize);
            int z = Mathf.FloorToInt(center.z / HorizontalCellSize);
            return $"{x}:{y}:{z}";
        }
    }
}
