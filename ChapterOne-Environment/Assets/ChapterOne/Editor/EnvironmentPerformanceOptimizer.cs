using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    /// <summary>
    /// Applies rendering policy without changing authored transforms, meshes,
    /// materials, or object placement.
    /// </summary>
    internal static class EnvironmentPerformanceOptimizer
    {
        const string MenuPath = "THAT NIGHT/Optimize current environment";

        [MenuItem(MenuPath)]
        public static void OptimizeCurrentScene()
        {
            var camera = UnityEngine.Object.FindAnyObjectByType<Camera>();
            var result = ApplyToScene(camera);

            var scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log(result);
        }

        public static string ApplyToScene(Camera camera)
        {
            // SpatialRenderBatcher already produces the final render geometry.
            // Unity static batching would duplicate it again in the player data.
            PlayerSettings.SetStaticBatchingForPlatform(BuildTarget.StandaloneWindows64, false);
            PlayerSettings.SetDynamicBatchingForPlatform(BuildTarget.StandaloneWindows64, true);

            if (camera != null)
                EnvironmentPerformance.Apply(camera);

            int distantRenderers = 0;
            int optimizedLights = 0;
            var clearingCenter = new Vector3(0f, SiteLayout.HillElevation, 15f);

            foreach (var renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                if (renderer == null) continue;

                renderer.allowOcclusionWhenDynamic = true;

                if (!IsDistantEnvironment(renderer, clearingCenter)) continue;

                renderer.gameObject.layer = EnvironmentPerformance.DistantEnvironmentLayer;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.lightProbeUsage = LightProbeUsage.Off;
                renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                EditorUtility.SetDirty(renderer);
                distantRenderers++;
            }

            foreach (var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (light == null || light.type == LightType.Directional) continue;

                // Non-shadowed decorative bulbs do not need an extra per-pixel pass.
                light.renderMode = light.shadows == LightShadows.None
                    ? LightRenderMode.ForceVertex
                    : LightRenderMode.ForcePixel;
                EditorUtility.SetDirty(light);
                optimizedLights++;
            }

            string batching = SpatialRenderBatcher.Rebuild();
            if (camera != null) EditorUtility.SetDirty(camera);

            return $"ENVIRONMENT_OPTIMIZED distantRenderers={distantRenderers} " +
                   $"practicalLights={optimizedLights} {batching} cullDistance=" +
                   EnvironmentPerformance.DistantEnvironmentCullDistance;
        }

        static bool IsDistantEnvironment(Renderer renderer, Vector3 clearingCenter)
        {
            var name = renderer.gameObject.name;

            if (name.StartsWith("Outer Forest ", StringComparison.Ordinal) ||
                name.StartsWith("Mountain Forest ", StringComparison.Ordinal) ||
                name.StartsWith("Colossal 20x Sacred World Tree", StringComparison.Ordinal))
                return true;

            return name == "Deep Forest Jungle Tree" &&
                   Vector3.Distance(renderer.bounds.center, clearingCenter) > 220f;
        }
    }
}
