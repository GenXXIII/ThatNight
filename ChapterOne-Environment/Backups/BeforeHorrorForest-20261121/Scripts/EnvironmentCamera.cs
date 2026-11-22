using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    // A scene inspection tool, not a player controller. No tasks, story state or progression.
    public sealed class EnvironmentCamera:MonoBehaviour
    {
        public EnvironmentLighting lighting;public Material grade;
        public static readonly Vector3[] Positions={
            new Vector3(0,SiteLayout.HillElevation+1.72f,SiteLayout.WeddingFront-4.5f),
            SiteLayout.HousePoint(new Vector3(SiteLayout.HouseWidth/2+4,3.7f,-SiteLayout.HouseDepth/2-9)),
            new Vector3(-11,SiteLayout.HillElevation+1.65f,-9.5f),
            SiteLayout.KitchenPosition+new Vector3(2.2f,1.9f,-6.2f),
            SiteLayout.HousePoint(new Vector3(0,3.95f,-SiteLayout.HouseDepth/2+.7f)),new Vector3(36,38,-40),
            SiteLayout.HousePoint(new Vector3(-SiteLayout.HouseWidth/4+.5f,3.95f,2.4f)),
            SiteLayout.HousePoint(new Vector3(SiteLayout.HouseWidth/4+.5f,3.95f,2.4f)),
            SiteLayout.CattleShelterPosition+new Vector3(-7.5f,2.1f,-8),
            new Vector3(48,5.25f,-109),
            new Vector3(18,SiteLayout.HillElevation+1.35f,-23),
            new Vector3(8,SiteLayout.HillElevation+1.85f,-15.5f),
            new Vector3(15,19.1f,-23),
            new Vector3(7,SiteLayout.HillElevation+1.65f,SiteLayout.WeddingFront-4.5f),
            new Vector3(4.5f,SiteLayout.HillElevation+1.65f,-8.5f),
            new Vector3(13,SiteLayout.HillElevation+1.7f,14),
            SiteLayout.HousePoint(new Vector3(-2.0f,1.5f,-6.0f)),
            SiteLayout.HousePoint(new Vector3(6.8f,4.1f,-4.9f)),
            new Vector3(12.5f,SiteLayout.HillElevation+3.7f,-19),
            SiteLayout.HousePoint(new Vector3(-.9f,3.9f,-4.6f))
        };
        public static readonly Vector3[] Targets={
            new Vector3(-2,SiteLayout.HillElevation+3.7f,13),SiteLayout.HousePoint(new Vector3(0,4.2f,0)),
            new Vector3(-11,SiteLayout.HillElevation+1.6f,7),SiteLayout.KitchenPosition+new Vector3(0,1.3f,1.4f),
            SiteLayout.HousePoint(new Vector3(0,3.55f,1.5f)),new Vector3(0,SiteLayout.HillElevation+1,13),
            SiteLayout.HousePoint(new Vector3(-SiteLayout.HouseWidth/4,3.2f,SiteLayout.HouseDepth/2-.8f)),
            SiteLayout.HousePoint(new Vector3(SiteLayout.HouseWidth/4,3.2f,SiteLayout.HouseDepth/2-.8f)),
            SiteLayout.CattleShelterPosition+new Vector3(0,1.5f,0),
            new Vector3(52,6.2f,-99),
            new Vector3(0,SiteLayout.HillElevation+1.6f,-15),
            new Vector3(175,SiteLayout.HillElevation+45,-560),
            SiteLayout.HousePoint(new Vector3(0,8,0)),
            new Vector3(7,SiteLayout.HillElevation+1.4f,SiteLayout.EntranceSideFront),
            new Vector3(13,SiteLayout.HillElevation+1.4f,5.5f),
            new Vector3(17,SiteLayout.HillElevation+1.4f,23),
            SiteLayout.HousePoint(new Vector3(-4,.6f,-3.8f)),
            SiteLayout.HousePoint(new Vector3(.5f,2.75f,-2.1f)),
            new Vector3(12.5f,SiteLayout.HillElevation+4.9f,SiteLayout.EntranceSideFront),
            SiteLayout.HousePoint(new Vector3(-3.4f,2.8f,-2.3f))
        };
        public static readonly string[] Names={"Entrance","House","Wedding canopy","Outdoor kitchen","Living room","Site overview","Bedroom 1","Bedroom 2","Cattle shelter","Hidden forest road","Wedding reveal turn","Mountain view from wedding","House landmark on mountain","Entrance photo corner","Dance and ceremony","Cooks banquet kitchen","Under house sitting platform","Wrist tying ceremony","Front fabric covers","Processional fruit trays"};
        bool showHelp=true,review;int view;float yaw,pitch;
        void Start()
        {
            QualitySettings.vSyncCount=0;
            Application.targetFrameRate=30;
            EnvironmentPerformance.Apply(GetComponent<Camera>());
            SetView(0);
            review=Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-environmentReview");
            if(review)StartCoroutine(Review());
        }
        public void SetView(int index)
        {
            view=index;transform.position=Positions[index];transform.LookAt(Targets[index]);yaw=transform.eulerAngles.y;pitch=transform.eulerAngles.x;
        }
        void Update()
        {
            if(review)return;
            if(Input.GetKeyDown(KeyCode.Escape)){Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
            if(Input.GetKeyDown(KeyCode.H))showHelp=!showHelp;
            if(Input.GetKeyDown(KeyCode.N))lighting.Apply(!lighting.daylight);
            if(Input.GetKeyDown(KeyCode.R))SetView(10);
            if(Input.GetKeyDown(KeyCode.M))SetView(11);
            if(Input.GetKeyDown(KeyCode.T))SetView(12);
            if(Input.GetKeyDown(KeyCode.P))SetView(13);
            if(Input.GetKeyDown(KeyCode.V))SetView(14);
            if(Input.GetKeyDown(KeyCode.C))SetView(15);
            if(Input.GetKeyDown(KeyCode.B))SetView(16);
            if(Input.GetKeyDown(KeyCode.J))SetView(17);
            if(Input.GetKeyDown(KeyCode.G))SetView(18);
            if(Input.GetKeyDown(KeyCode.F))SetView(19);
            for(int i=0;i<9;i++)if(Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1+i)))SetView(i);
            if(Input.GetKeyDown(KeyCode.Alpha0))SetView(9);
            if(Input.GetKeyDown(KeyCode.F12))Capture("manual-"+DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            if(Input.GetMouseButton(1))
            {
                Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;
                yaw+=Input.GetAxisRaw("Mouse X")*1.6f;pitch=Mathf.Clamp(pitch-Input.GetAxisRaw("Mouse Y")*1.6f,-85,85);transform.rotation=Quaternion.Euler(pitch,yaw,0);
                Vector3 direction=new Vector3((Input.GetKey(KeyCode.D)?1:0)-(Input.GetKey(KeyCode.A)?1:0),(Input.GetKey(KeyCode.E)?1:0)-(Input.GetKey(KeyCode.Q)?1:0),(Input.GetKey(KeyCode.W)?1:0)-(Input.GetKey(KeyCode.S)?1:0));
                transform.position+=transform.TransformDirection(direction)*Time.unscaledDeltaTime*(Input.GetKey(KeyCode.LeftShift)?7:1.65f);
            }
            else {Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
        }
        void OnRenderImage(RenderTexture source,RenderTexture target)
        {if(grade)Graphics.Blit(source,target,grade);else Graphics.Blit(source,target);}
        void OnGUI()
        {
            if(review){GUI.Label(new Rect(18,18,650,30),"Automatic environment review — please leave this window open; it closes when finished.");return;}
            if(!showHelp)return;
            GUI.color=new Color(.035f,.04f,.04f,.88f);GUI.DrawTexture(new Rect(18,18,480,89),Texture2D.whiteTexture);GUI.color=Color.white;
            GUI.Label(new Rect(32,27,450,24),"THAT NIGHT  /  CHAPTER ONE  /  ENVIRONMENT STUDY");
            GUI.Label(new Rect(32,51,450,22),"1 Entrance  3 Tables  6 Overview  P Photo  V Dance  C Cooks  N Day");
            GUI.Label(new Rect(32,74,450,22),"Hold right mouse + WASD move · Q/E down/up · Shift faster");
        }
        string OutputFolder()
        {string root=Application.isEditor?Path.GetDirectoryName(Application.dataPath):Path.GetFullPath(Path.Combine(Application.dataPath,"../.."));string folder=Path.Combine(root,"Review");Directory.CreateDirectory(folder);return folder;}
        void Capture(string filename)
        {
            var cam=GetComponent<Camera>();var rt=new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var old=cam.targetTexture;var active=RenderTexture.active;
            cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;
            var image=new Texture2D(1280,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();
            File.WriteAllBytes(Path.Combine(OutputFolder(),filename+".png"),image.EncodeToPNG());cam.targetTexture=old;RenderTexture.active=active;Destroy(image);rt.Release();Destroy(rt);
        }
        [Serializable] class ReviewResult {public string gpu,resolution,method;public bool visualReviewComplete,timingComplete;public string[] captures;public float averageFps,p95Ms,focusedFraction;public int sceneRenderers;}
        IEnumerator Review()
        {
            showHelp=false;yield return new WaitForSeconds(3);var captures=new List<string>();
            for(int day=0;day<2;day++)
            {
                lighting.Apply(day==1);
                for(int i=0;i<Names.Length;i++){SetView(i);yield return new WaitForSeconds(.7f);string name=(day==0?"night-":"day-")+Names[i].Replace(" ","-");Capture(name);captures.Add(name+".png");}
            }
            var pending=new ReviewResult{gpu=SystemInfo.graphicsDeviceName,resolution=Screen.width+"x"+Screen.height,method=captures.Count+" scene captures completed; "+(Application.isBatchMode?"batch capture only, no interactive frame timing.":"timing not yet completed."),visualReviewComplete=true,captures=captures.ToArray(),sceneRenderers=FindObjectsByType<Renderer>(FindObjectsSortMode.None).Length};
            File.WriteAllText(Path.Combine(OutputFolder(),"result.json"),JsonUtility.ToJson(pending,true));
            if(Application.isBatchMode){Debug.Log("ENVIRONMENT_REVIEW "+JsonUtility.ToJson(pending));Application.Quit();yield break;}
            lighting.Apply(false);SetView(0);yield return new WaitForSeconds(2);
            // Normal rendered frames, with no capture/readback during the timing interval.
            Application.targetFrameRate=60;var frames=new List<float>();int focused=0;float until=Time.realtimeSinceStartup+8;
            while(Time.realtimeSinceStartup<until){yield return null;frames.Add(Time.unscaledDeltaTime*1000);if(Application.isFocused)focused++;}
            frames.Sort();float sum=0;foreach(float ms in frames)sum+=ms;
            var result=new ReviewResult{gpu=SystemInfo.graphicsDeviceName,resolution=Screen.width+"x"+Screen.height,method="Entrance night view; visible normal rendering; 60 FPS cap; no GPU readback during timing. Not an exhaustive benchmark.",visualReviewComplete=true,timingComplete=true,captures=captures.ToArray(),averageFps=frames.Count*1000/sum,p95Ms=frames[(int)(frames.Count*.95f)],focusedFraction=focused/(float)frames.Count,sceneRenderers=FindObjectsByType<Renderer>(FindObjectsSortMode.None).Length};
            File.WriteAllText(Path.Combine(OutputFolder(),"result.json"),JsonUtility.ToJson(result,true));Debug.Log("ENVIRONMENT_REVIEW "+JsonUtility.ToJson(result));Application.Quit();
        }
    }
}
