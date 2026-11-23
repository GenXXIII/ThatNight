using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    /// <summary>
    /// Removes editor-retained source render components from the temporary scene
    /// copy used by a player build. Colliders and authored hierarchy remain.
    /// The saved Editor scene is never modified by this processor.
    /// </summary>
    internal sealed class EnvironmentBuildSceneProcessor : IProcessSceneWithReport
    {
        const string BatchRootName = "09 RENDER BATCHES - generated performance geometry";

        public int callbackOrder => 100;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (report == null) return;

            bool hasBatches = false;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == BatchRootName)
                {
                    hasBatches = true;
                    break;
                }
            }
            if (!hasBatches) return;

            var retainedMeshes = new HashSet<Mesh>();
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var filter in root.GetComponentsInChildren<MeshFilter>(true))
                {
                    var renderer = filter.GetComponent<MeshRenderer>();
                    if (renderer != null && renderer.enabled && filter.sharedMesh != null)
                        retainedMeshes.Add(filter.sharedMesh);
                }

                foreach (var collider in root.GetComponentsInChildren<MeshCollider>(true))
                {
                    if (collider.sharedMesh != null)
                        retainedMeshes.Add(collider.sharedMesh);
                }
            }

            int stripped = 0;
            int strippedMeshes = 0;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == BatchRootName) continue;

                foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (renderer == null || renderer.enabled || !renderer.GetComponent<RenderBatchSource>()) continue;

                    var filter = renderer.GetComponent<MeshFilter>();
                    var sourceMesh = filter != null ? filter.sharedMesh : null;
                    Object.DestroyImmediate(renderer);
                    if (filter != null) Object.DestroyImmediate(filter);

                    if (sourceMesh != null &&
                        !retainedMeshes.Contains(sourceMesh) &&
                        !AssetDatabase.Contains(sourceMesh))
                    {
                        Object.DestroyImmediate(sourceMesh);
                        strippedMeshes++;
                    }
                    stripped++;
                }
            }

            Debug.Log($"ENVIRONMENT_BUILD_STRIP sourceRenderers={stripped} " +
                      $"proceduralMeshes={strippedMeshes}");
        }
    }
}
