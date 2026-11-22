using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne.Editor
{
    public static class EnvironmentBuild
    {
        const string Scene = "Assets/ChapterOne/Scenes/ChapterOneEnvironment.unity";

        [MenuItem("THAT NIGHT/Build environment preview")]
        public static void Build()
        {
            Generate();
            Directory.CreateDirectory("Builds");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[]{ Scene },
                target = BuildTarget.StandaloneWindows64,
                locationPathName = "Builds/ChapterOneEnvironment.exe",
                options = BuildOptions.None
            });
            Debug.Log("ENVIRONMENT_BUILD " + report.summary.result + " bytes=" + report.summary.totalSize);
            if (report.summary.result != BuildResult.Succeeded) throw new Exception("Environment build failed");
        }

        [MenuItem("THAT NIGHT/Regenerate environment scene")]
        public static void Generate()
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isDirty)
                    EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));

            SurfaceLibrary.Prepare();
            LightingBuilder.Practicals.Clear();

            PlayerSettings.companyName = "That Night Project";
            PlayerSettings.productName = "THAT NIGHT - Chapter One Environment";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new[]{ GraphicsDeviceType.Direct3D11 });

            // Explicit spatial render batches replace Unity's memory-heavy static batching.
            PlayerSettings.SetStaticBatchingForPlatform(BuildTarget.StandaloneWindows64, false);
            PlayerSettings.SetDynamicBatchingForPlatform(BuildTarget.StandaloneWindows64, true);

            QualitySettings.SetQualityLevel(2, false);
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 2;
            QualitySettings.pixelLightCount = 3;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            QualitySettings.shadowDistance = 34;
            QualitySettings.shadowCascades = 2;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Build all 5 environment sectors
            var house = StiltHouseBuilder.Build();
            var wedding = WeddingYardBuilder.Build();
            var kitchen = KitchenBuilder.Build();
            var ground = LandscapeBuilder.BuildGround();
            var village = LandscapeBuilder.BuildVillage();
            var cattle = CattleShelterBuilder.Build();
            var lighting = LightingBuilder.Build();

            // Consolidate every model in the hierarchy so each tree, chair, bed, or building
            // is an individually selectable object with its own collider, mesh, and materials.
            ModelCombiner.CombineAllInHierarchy(house);
            ModelCombiner.CombineAllInHierarchy(wedding);
            ModelCombiner.CombineAllInHierarchy(kitchen);
            ModelCombiner.CombineAllInHierarchy(village);
            ModelCombiner.CombineAllInHierarchy(cattle);

            // Review camera
            var go = new GameObject("07 REVIEW CAMERA - environment inspection only");
            var camera = go.AddComponent<Camera>();
            camera.nearClipPlane = .05f;
            camera.farClipPlane = 1200;
            camera.fieldOfView = 62;
            camera.allowHDR = true;
            camera.allowMSAA = true;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.renderingPath = RenderingPath.Forward;
            go.AddComponent<AudioListener>();
            var review = go.AddComponent<EnvironmentCamera>();
            review.lighting = lighting;
            var grade = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Camera grade.mat");
            if (!grade)
            {
                grade = new Material(Shader.Find("ThatNight/CameraGrade"));
                AssetDatabase.CreateAsset(grade, "Assets/ChapterOne/Materials/Camera grade.mat");
            }
            review.grade = grade;
            review.SetView(0);

            // Apply performance policy after all authored geometry and lights exist.
            EnvironmentPerformanceOptimizer.ApplyToScene(camera);

            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), Scene);
            EditorBuildSettings.scenes = new[]{ new EditorBuildSettingsScene(Scene, true) };
            AssetDatabase.SaveAssets();

            if (!Application.isBatchMode && SceneNavigation.Enabled)
            {
                SceneNavigation.Enable();
                SceneNavigation.FrameHouse();
            }

            int triangles = 0;
            foreach (var mesh in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))
                triangles += mesh.sharedMesh.triangles.Length / 3;

            int rendererCount = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Length;
            Debug.Log($"ENVIRONMENT_SCENE complete! Total renderers={rendererCount}, triangles={triangles}");
        }


        [MenuItem("THAT NIGHT/Atmosphere/1. 4-00 AM Dawn (Scene 1 Quiet)")]
        public static void SetAtmosphereDawn()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.Dawn4AM); Debug.Log("ATMOSPHERE: 4:00 AM Dawn (Quiet)"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/2. Morning Peaceful (Scene 2-4 Preparations)")]
        public static void SetAtmosphereMorning()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.MorningPeaceful); Debug.Log("ATMOSPHERE: Morning Peaceful"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/3. Afternoon Celebration (Scene 5 Ceremony)")]
        public static void SetAtmosphereAfternoon()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.AfternoonCelebration); Debug.Log("ATMOSPHERE: Afternoon Celebration"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/4. Evening Warm (Scene 6 Celebration)")]
        public static void SetAtmosphereEvening()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.EveningWarm); Debug.Log("ATMOSPHERE: Evening Warm"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/5. Night Horror (Scene 7-9 Climax)")]
        public static void SetAtmosphereNightHorror()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.NightHorror); Debug.Log("ATMOSPHERE: Night Horror"); }
        }
    }
}

