using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    [InitializeOnLoad]
    public static class SceneNavigation
    {
        const string Key="ThatNight.Environment.EasyNavigation";
        const string HiddenKey="ThatNight.Environment.HiddenBackground";
        static int panningView;
        static SceneNavigation()
        {
            SceneView.duringSceneGui+=Draw;
            EditorApplication.delayCall+=()=>
            {
                if(Application.isBatchMode||!SceneManager.GetActiveScene().path.Contains("ChapterOne"))return;
                if(!SessionState.GetBool(Key+"Initialized",false)){SessionState.SetBool(Key+"Initialized",true);Enable();FrameHouse();}
            };
        }
        public static bool Enabled=>EditorPrefs.GetBool(Key,true);
        [MenuItem("THAT NIGHT/Scene navigation/Easy editing view")]
        public static void Enable()
        {
            EditorPrefs.SetBool(Key,true);
            // Ultra-lightweight graphics to keep Intel UHD 630 CPU/GPU cool and smooth:
            QualitySettings.SetQualityLevel(0, true); // Very Low quality preset
            QualitySettings.shadows = ShadowQuality.Disable; // Disable expensive shadow cascades
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60; // Cap at 60 FPS to stop CPU/GPU spinning at 100%
            foreach(SceneView view in SceneView.sceneViews)Apply(view,true);
            SetBackgroundHidden(true);SceneView.RepaintAll();
        }
        [MenuItem("THAT NIGHT/Scene navigation/Artwork lighting and village")]
        public static void Artwork()
        {
            EditorPrefs.SetBool(Key,false);
            // Full visual fidelity for artwork inspection:
            QualitySettings.SetQualityLevel(2, true); // Medium quality preset
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
            foreach(SceneView view in SceneView.sceneViews)Apply(view,false);
            SetBackgroundHidden(false);SceneView.RepaintAll();
        }
        static void Apply(SceneView view,bool easy)
        {
            var settings=view.cameraSettings;
            settings.speedMin=.1f;settings.speedMax=60;settings.speed=easy?4:12;settings.accelerationEnabled=true;settings.easingEnabled=false;
            settings.dynamicClip=false;settings.nearClip=.03f;settings.farClip=easy?1400:4500;view.cameraSettings=settings;
            view.sceneLighting=!easy;view.sceneViewState.showFog=!easy;view.sceneViewState.showImageEffects=!easy;
            view.sceneViewState.showSkybox=!easy; // Disables procedural skybox compute in Easy mode
            view.sceneViewState.alwaysRefresh=false;view.drawGizmos=false;view.showGrid=false;view.in2DMode=false;view.orthographic=false;
        }
        static void SetBackgroundHidden(bool hidden)
        {
            var ids=new List<int>();foreach(string value in SessionState.GetString(HiddenKey,"").Split(','))if(int.TryParse(value,out int id))ids.Add(id);
            if(!hidden)
            {
#if UNITY_6000_0_OR_NEWER
                foreach(int id in ids)if(EditorUtility.EntityIdToObject(id) is GameObject g)SceneVisibilityManager.instance.Show(g,true);
#else
                foreach(int id in ids)if(EditorUtility.InstanceIDToObject(id) is GameObject g)SceneVisibilityManager.instance.Show(g,true);
#endif
                SessionState.SetString(HiddenKey,"");return;
            }
            foreach(var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if(root.name.StartsWith("05 VILLAGE"))Hide(root,ids);
                if(root.name.StartsWith("04 GROUND"))foreach(var r in root.GetComponentsInChildren<MeshRenderer>())
                {
                    if(r.name.Contains("World Tree") || r.name.Contains("Sacred")) continue;
                    var p=r.bounds.center;
                    if(Mathf.Abs(p.x)>SiteLayout.HalfWidth+6||p.z>SiteLayout.Back+6||p.z<SiteLayout.Front-8)Hide(r.gameObject,ids);
                }
            }
            SessionState.SetString(HiddenKey,string.Join(",",ids));
        }
        static void Hide(GameObject g,List<int> ids)
        {if(!SceneVisibilityManager.instance.IsHidden(g)){SceneVisibilityManager.instance.Hide(g,true);ids.Add(g.GetInstanceID());}}
        [MenuItem("THAT NIGHT/Scene navigation/Frame house")]
        public static void FrameHouse()=>Frame(SiteLayout.HousePosition+Vector3.up*3.4f,Quaternion.Euler(17,-15,0),13.5f);
        [MenuItem("THAT NIGHT/Scene navigation/Frame cattle shelter")]
        public static void FrameCattle()=>Frame(SiteLayout.CattleShelterPosition+Vector3.up*1.6f,Quaternion.Euler(21,40,0),8.5f);
        [MenuItem("THAT NIGHT/Scene navigation/Frame giant tree")]
        public static void FrameGiantTree()
        {
            Vector3 camPos = new Vector3(-740f, 40f, 860f);
            Vector3 target = new Vector3(-880f, 180f, 1020f);
            Quaternion rot = Quaternion.LookRotation((target - camPos).normalized);
            Frame(target, rot, 75f);
        }
        static void Frame(Vector3 pivot,Quaternion angle,float size)
        {
            var view=SceneView.lastActiveSceneView;if(!view)view=EditorWindow.GetWindow<SceneView>();
            Apply(view,Enabled);view.LookAtDirect(pivot,angle,size);view.Focus();view.Repaint();
        }
        static void Draw(SceneView view)
        {
            if(!SceneManager.GetActiveScene().path.Contains("ChapterOne"))return;
            var panel=new Rect(62,10,695,88);
            var e=Event.current;
            int panControl=GUIUtility.GetControlID("ChapterOneScenePan".GetHashCode(),FocusType.Passive);
            if(Enabled&&!e.alt&&e.type==EventType.MouseDown&&e.button==2&&!panel.Contains(e.mousePosition))
            {panningView=view.GetInstanceID();GUIUtility.hotControl=panControl;e.Use();}
            if(Enabled&&panningView==view.GetInstanceID()&&e.type==EventType.MouseDrag&&e.button==2)
            {
                float unitsPerPixel=Mathf.Max(.0025f,2*view.cameraDistance*Mathf.Tan(view.camera.fieldOfView*Mathf.Deg2Rad*.5f)/Mathf.Max(1,view.camera.pixelHeight));
                view.pivot+=(view.camera.transform.right*-e.delta.x+view.camera.transform.up*e.delta.y)*unitsPerPixel;
                e.Use();view.Repaint();
            }
            if(panningView==view.GetInstanceID()&&(e.type==EventType.MouseUp||e.type==EventType.Ignore))
            {panningView=0;GUIUtility.hotControl=0;if(e.type==EventType.MouseUp)e.Use();}
            // Near the orbit pivot, Unity's normal wheel zoom can approach zero movement.
            // Keep a usable minimum orbit distance, then dolly through it instead of stalling.
            if(Enabled&&e.type==EventType.ScrollWheel&&!e.alt&&!e.control&&!panel.Contains(e.mousePosition))
            {
                float size=view.size*Mathf.Exp(e.delta.y*.075f);
                if(size<.35f){view.pivot+=view.camera.transform.forward*(-e.delta.y*.14f);size=.35f;}
                view.size=Mathf.Clamp(size,.35f,350);e.Use();view.Repaint();
            }
            Handles.BeginGUI();GUI.Box(panel,GUIContent.none);
            GUILayout.BeginArea(new Rect(72,16,675,77));
            GUILayout.BeginHorizontal();GUILayout.Label("CHAPTER ONE",EditorStyles.boldLabel,GUILayout.Width(115));
            if(GUILayout.Button(Enabled?"Easy editing: ON":"Use easy editing"))Enable();
            if(GUILayout.Button("Artwork view"))Artwork();GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("House"))FrameHouse();
            if(GUILayout.Button("Courtyard"))Frame(new Vector3(0,SiteLayout.HillElevation+2,(SiteLayout.Front+SiteLayout.Back)/2),Quaternion.Euler(42,-12,0),26);
            if(GUILayout.Button("Kitchen"))Frame(SiteLayout.KitchenPosition+Vector3.up*1.6f,Quaternion.Euler(15,-20,0),6.5f);
            if(GUILayout.Button("Cattle shed"))FrameCattle();
            if(GUILayout.Button("Forest road"))Frame(new Vector3(50,5.2f,-102),Quaternion.Euler(17,178,0),20);
            if(GUILayout.Button("Reveal turn"))Frame(new Vector3(15,SiteLayout.HillElevation+.4f,-21),Quaternion.Euler(22,120,0),12);
            if(GUILayout.Button("Giant tree"))FrameGiantTree();
            GUILayout.EndHorizontal();GUILayout.Label("Wheel zoom · Middle drag pan · Alt + left drag orbit · F focus selection",EditorStyles.miniLabel);
            GUILayout.EndArea();Handles.EndGUI();
        }
        public static string Status()
        {var v=SceneView.lastActiveSceneView;return v?"easy="+Enabled+" sceneLighting="+v.sceneLighting+" speed="+v.cameraSettings.speed+" size="+v.size+" pivot="+v.pivot:"No Scene view";}
        public static string VerifyNavigation()
        {
            Enable();FrameHouse();var view=SceneView.lastActiveSceneView;
            var oldTool=Tools.current;Tools.current=Tool.View;
            Vector2 pointer=new Vector2(Mathf.Max(580,view.position.width*.6f),view.position.height*.55f);
            float before=view.size;
            view.SendEvent(new Event{type=EventType.ScrollWheel,mousePosition=pointer,delta=new Vector2(0,-3)});
            bool zoom=view.size<before&&view.size>0;
            var panBefore=view.pivot;
            view.SendEvent(new Event{type=EventType.MouseDown,button=2,mousePosition=pointer});
            view.SendEvent(new Event{type=EventType.MouseDrag,button=2,mousePosition=pointer+new Vector2(70,35),delta=new Vector2(70,35)});
            view.SendEvent(new Event{type=EventType.MouseUp,button=2,mousePosition=pointer+new Vector2(70,35)});
            bool pan=Vector3.Distance(panBefore,view.pivot)>.001f;
            view.size=.351f;var closeBefore=view.pivot;
            view.SendEvent(new Event{type=EventType.ScrollWheel,mousePosition=pointer,delta=new Vector2(0,-3)});
            bool close=Vector3.Distance(closeBefore,view.pivot)>.1f&&view.size>=.35f;
            Tools.current=oldTool;FrameHouse();
            string result="wheelZoom="+zoom+" middlePan="+pan+" closeZoom="+close+" "+Status();
            if(!zoom||!pan||!close)throw new InvalidOperationException("Navigation input check: "+result);
            return result;
        }
    }
}
