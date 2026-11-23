using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    // Opt-in rendered verification. Never runs during a normal player session.
    public sealed class GameplayReview : MonoBehaviour
    {
        public static bool IsRunning { get; private set; }
        void OnEnable(){IsRunning=true;}
        void OnDestroy(){IsRunning=false;}
        [Serializable] public class Sample { public string location; public int frames; public float averageFps,p95Ms,focusedFraction; }
        [Serializable] public class Result
        {
            public string gpu,resolution,method="Standalone rendered stationary samples, 6 seconds each after 2 seconds warmup; 30 FPS cap. Not a whole-map traversal benchmark.";
            public List<string> passed=new List<string>(),failed=new List<string>();
            public List<Sample> samples=new List<Sample>();
        }
        readonly Result result=new Result();
        string folder;
        PlayerInteraction player;
        ForestExploration explorer;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if(Array.Exists(Environment.GetCommandLineArgs(),x=>x=="-gameplayReview"))
                new GameObject("Opt-in gameplay verification").AddComponent<GameplayReview>();
        }
        void Check(bool condition,string name){(condition?result.passed:result.failed).Add(name);}
        IEnumerator Start()
        {
            Application.runInBackground=true;
            folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../Review/Gameplay"));Directory.CreateDirectory(folder);
            Application.logMessageReceived+=OnLog;
            yield return new WaitForSeconds(3);
            player=FindFirstObjectByType<PlayerInteraction>();explorer=player.explorer;
            result.gpu=SystemInfo.graphicsDeviceName;result.resolution=Screen.width+"x"+Screen.height;
            Check(explorer.Walking&&explorer.Body.isGrounded,"Arrival walking and grounded");
            Check(player.body.lowerBody.gameObject.activeInHierarchy&&player.body.leftHand.gameObject.activeInHierarchy,"Visible first person body and hands");
            yield return Capture("arrival-native");
            foreach(var guest in FindObjectsByType<WeddingGuest>(FindObjectsSortMode.None))
            {
                guest.Use(player);Check(player.Speaker==guest&&explorer.ControlsLocked,"Conversation "+guest.displayName);
                player.Choose(0);player.Choose(1);player.CloseDialogue();
                Check(!player.Speaker&&!explorer.ControlsLocked,"Conversation release "+guest.displayName);
            }
            foreach(var item in FindObjectsByType<PickupItem>(FindObjectsSortMode.None))
            {
                item.Use(player);yield return new WaitForSeconds(.85f);
                Check(item.Held&&player.HeldItem==item&&item.transform.parent==player.body.holdSocket,"Reach and hold "+item.displayName);
                if(item.displayName.Contains("cup"))yield return Capture("held-cup-native");
                player.ReturnHeldItem();yield return new WaitForSeconds(.85f);
                Check(!item.Held&&!player.HeldItem&&!player.Busy&&!explorer.ControlsLocked,"Put back and unlock "+item.displayName);
            }
            foreach(var door in FindObjectsByType<HingedOpening>(FindObjectsSortMode.None))
            {
                bool previous=door.isOpen;door.Use(player);yield return new WaitForSeconds(1.2f);
                Check(door.isOpen!=previous&&!door.Moving,"Animate "+door.transform.parent.name+" / "+door.name);
                door.Use(player);yield return new WaitForSeconds(1.2f);
                Check(door.isOpen==previous&&!door.Moving,"Reverse "+door.name);
            }
            player.EnterGameplay();yield return SampleFrames("Wedding arrival afternoon");
            explorer.Visit(0);player.GetComponent<EnvironmentCamera>().lighting.ApplyStage(AtmosphereStage.NightHorror);
            explorer.flashlight.enabled=true;yield return SampleFrames("Forest edge night flashlight");
            yield return Capture("night-flashlight-native");
            explorer.flashlight.enabled=false;yield return Capture("night-no-light-native");
            explorer.flashlight.enabled=true;explorer.Visit(2);yield return SampleFrames("Deep forest night flashlight");
            Check(RenderSettings.fogDensity>=.05f&&explorer.flashlight.range<=18.1f,"Dark short visibility night");
            Check(explorer.Walking&&explorer.Body.isGrounded,"Deep forest grounded");
            File.WriteAllText(Path.Combine(folder,"gameplay-review.json"),JsonUtility.ToJson(result,true));
            Debug.Log("GAMEPLAY_REVIEW passed="+result.passed.Count+" failed="+result.failed.Count);
            Application.logMessageReceived-=OnLog;
            Application.Quit(result.failed.Count==0?0:1);
        }
        IEnumerator SampleFrames(string name)
        {
            yield return new WaitForSeconds(2);
            var frames=new List<float>(200);int focus=0;float end=Time.realtimeSinceStartup+6;
            while(Time.realtimeSinceStartup<end){yield return null;frames.Add(Time.unscaledDeltaTime*1000);if(Application.isFocused)focus++;}
            float sum=0;foreach(float dt in frames)sum+=dt;frames.Sort();
            result.samples.Add(new Sample{location=name,frames=frames.Count,averageFps=frames.Count*1000f/sum,p95Ms=frames[Mathf.Min(frames.Count-1,Mathf.FloorToInt(frames.Count*.95f))],focusedFraction=(float)focus/frames.Count});
        }
        IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame();
            var texture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,name+".png"),texture.EncodeToPNG());Destroy(texture);
        }
        void OnLog(string message,string trace,LogType type)
        {
            if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)result.failed.Add(type+": "+message);
        }
    }
}
