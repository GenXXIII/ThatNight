using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    [InitializeOnLoad]
    internal static class EnvironmentBuildMailbox
    {
        const string RequestPath = "Tools/editor-request.txt";
        const string ResultPath = "Logs/editor-result.txt";
        const string RefreshKey = "EnvironmentBuildRefresh";
        static double nextPoll;

        static EnvironmentBuildMailbox() => EditorApplication.update += Tick;

        static void Tick()
        {
            if (Application.isBatchMode || EditorApplication.isCompiling ||
                EditorApplication.isUpdating || EditorApplication.timeSinceStartup < nextPoll)
                return;
            nextPoll = EditorApplication.timeSinceStartup + 1;
            if (!File.Exists(RequestPath)) return;
            try
            {
                string command = File.ReadAllText(RequestPath).Replace("\0", "").Trim().ToLowerInvariant();
                if (command == "build" && !SessionState.GetBool(RefreshKey, false))
                {
                    SessionState.SetBool(RefreshKey, true);
                    nextPoll = EditorApplication.timeSinceStartup + 3;
                    AssetDatabase.Refresh();
                    return;
                }
                SessionState.SetBool(RefreshKey, false);
                File.Delete(RequestPath);
                Execute(command);
            }
            catch (Exception error)
            {
                Write("ERROR " + error.Message);
                Debug.LogException(error);
            }
        }

        static void Execute(string command)
        {
            switch (command)
            {
                case "status":
                    Write($"STATUS dirty={SceneManager.GetActiveScene().isDirty} playing={EditorApplication.isPlaying}");
                    break;
                case "save-scene":
                    if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveOpenScenes())
                        throw new InvalidOperationException("Stop Play and save your authored scene first.");
                    Write("OK scene saved");
                    break;
                case "build":
                    EnvironmentBuild.Build();
                    Write("OK build " + EnvironmentBuild.OutputPath);
                    break;
                case "refresh":
                    AssetDatabase.Refresh();
                    Write("OK refresh");
                    break;
                case "setup-chapter1":
                    ChapterOneDirectorBuilder.Setup();
                    Write("OK chapter1 setup");
                    break;
                case "navigation":
                    SceneNavigation.Enable();
                    SceneNavigation.FrameHouse();
                    Write("OK navigation " + SceneNavigation.Status());
                    break;
                case "navigation-status":
                    Write("STATUS " + SceneNavigation.Status());
                    break;
                case "navigation-check":
                    Write("OK " + SceneNavigation.VerifyNavigation());
                    break;
                case "cattle-view":
                    SceneNavigation.FrameCattle();
                    Write("OK cattle view " + SceneNavigation.Status());
                    break;
                case "artwork":
                    SceneNavigation.Artwork();
                    SceneNavigation.FrameHouse();
                    Write("OK artwork " + SceneNavigation.Status());
                    break;
                default:
                    throw new InvalidOperationException("Unknown environment request: " + command);
            }
        }

        static void Write(string message)
        {
            Directory.CreateDirectory("Logs");
            File.WriteAllText(ResultPath, message);
        }
    }
}
