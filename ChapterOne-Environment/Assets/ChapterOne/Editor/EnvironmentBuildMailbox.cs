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
        static double next;
        static EnvironmentBuildMailbox(){EditorApplication.update+=Tick;}
        static void Tick()
        {
            if(Application.isBatchMode||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.timeSinceStartup<next)return;
            next=EditorApplication.timeSinceStartup+1;
            const string request="Tools/editor-request.txt";
            if(!File.Exists(request))return;
            string command=File.ReadAllText(request).Trim();
            if(command=="build"&&!SessionState.GetBool("EnvironmentBuildRefresh",false))
            {SessionState.SetBool("EnvironmentBuildRefresh",true);AssetDatabase.Refresh();next=EditorApplication.timeSinceStartup+3;return;}
            SessionState.SetBool("EnvironmentBuildRefresh",false);File.Delete(request);
            try
            {
                if(command=="status")Write("STATUS dirty="+SceneManager.GetActiveScene().isDirty+" playing="+EditorApplication.isPlaying);
                else if(command=="save-scene"){EditorSceneManager.SaveOpenScenes();Write("OK scene saved");}
                else if(command=="navigation"){SceneNavigation.Enable();SceneNavigation.FrameHouse();Write("OK navigation "+SceneNavigation.Status());}
                else if(command=="navigation-status")Write("STATUS "+SceneNavigation.Status());
                else if(command=="navigation-check")Write("OK "+SceneNavigation.VerifyNavigation());
                else if(command=="cattle-view"){SceneNavigation.FrameCattle();Write("OK cattle view "+SceneNavigation.Status());}
                else if(command=="build")
                {
                    if(EditorApplication.isPlaying)throw new Exception("Stop Play mode before building.");
                    EnvironmentBuild.Build();Write("OK build");
                }
                else throw new Exception("Unknown environment build request");
            }
            catch(Exception e){Write("ERROR "+e.Message);Debug.LogException(e);}
        }
        static void Write(string message){Directory.CreateDirectory("Logs");File.WriteAllText("Logs/editor-result.txt",message);}
    }
}
