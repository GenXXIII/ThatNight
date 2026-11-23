using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    /// <summary>Builds authored scenes without regenerating any scene content.</summary>
    public static class EnvironmentBuild
    {
        public const string OutputPath = "Builds/ChapterOne/THAT NIGHT.exe";

        [MenuItem("THAT NIGHT/Build current Chapter 1 preview")]
        public static void Build()
        {
            string scenePath = PrepareScene();
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { scenePath },
                target = BuildTarget.StandaloneWindows64,
                locationPathName = OutputPath,
                options = BuildOptions.None
            });
            Debug.Log($"CHAPTER_ONE_BUILD {report.summary.result} scene={scenePath} bytes={report.summary.totalSize}");
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Chapter 1 build failed. See the Console.");
        }

        // Existing automation fails clearly instead of resetting the authored scene.
        public static void Generate() => throw new NotSupportedException(
            "Legacy regeneration was removed. Edit the saved scene and use Build current Chapter 1 preview.");

        public static string PrepareScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Stop Play mode before building.");
            if (Application.isBatchMode)
            {
                var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).ToArray();
                if (scenes.Length != 1)
                    throw new InvalidOperationException("Enable exactly one authored Chapter 1 scene in Build Profiles.");
                ValidateScenePath(scenes[0].path);
                return scenes[0].path;
            }
            if (EditorSceneManager.GetSceneManagerSetup().Length != 1)
                throw new InvalidOperationException("Open only the Chapter 1 scene to build; close additive scenes first.");
            var scene = SceneManager.GetActiveScene();
            ValidateScenePath(scene.path);
            if (!scene.IsValid() || !scene.isLoaded)
                throw new InvalidOperationException("Open your saved Chapter 1 scene first.");
            if (scene.isDirty && !EditorSceneManager.SaveScene(scene))
                throw new InvalidOperationException("Scene could not be saved. Build cancelled.");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scene.path, true) };
            AssetDatabase.SaveAssets();
            return scene.path;
        }

        static void ValidateScenePath(string path)
        {
            if (string.IsNullOrEmpty(path) ||
                !path.StartsWith("Assets/ChapterOne/Scenes/", StringComparison.Ordinal) ||
                !path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) ||
                AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
                throw new InvalidOperationException("Save and open your scene under Assets/ChapterOne/Scenes before building.");
        }
    }
}
