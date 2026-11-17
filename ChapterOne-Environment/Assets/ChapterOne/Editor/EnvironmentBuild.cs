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
        const string Scene="Assets/ChapterOne/Scenes/ChapterOneEnvironment.unity";
        [MenuItem("THAT NIGHT/Build environment preview")]
        public static void Build()
        {
            Generate();
            Directory.CreateDirectory("Builds");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{Scene},target=BuildTarget.StandaloneWindows64,locationPathName="Builds/ChapterOneEnvironment.exe",options=BuildOptions.None});
            Debug.Log("ENVIRONMENT_BUILD "+report.summary.result+" bytes="+report.summary.totalSize);
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Environment build failed");
        }
        [MenuItem("THAT NIGHT/Regenerate environment scene")]
        public static void Generate()
        {
            for(int i=0;i<UnityEngine.SceneManagement.SceneManager.sceneCount;i++)
                if(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isDirty)throw new Exception("Save your scene edits before regenerating. Duplicate the scene to preserve manual changes.");
            SurfaceLibrary.Prepare();LightingBuilder.Practicals.Clear();
            PlayerSettings.companyName="That Night Project";PlayerSettings.productName="THAT NIGHT - Chapter One Environment";
            PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=720;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.resizableWindow=true;PlayerSettings.runInBackground=true;PlayerSettings.colorSpace=ColorSpace.Linear;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{GraphicsDeviceType.Direct3D11});
            PlayerSettings.SetStaticBatchingForPlatform(BuildTarget.StandaloneWindows64,false);PlayerSettings.SetDynamicBatchingForPlatform(BuildTarget.StandaloneWindows64,true);
            QualitySettings.SetQualityLevel(2,false);QualitySettings.vSyncCount=0;QualitySettings.antiAliasing=2;QualitySettings.pixelLightCount=3;
            QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowResolution=ShadowResolution.Medium;QualitySettings.shadowDistance=34;QualitySettings.shadowCascades=2;
            QualitySettings.anisotropicFiltering=AnisotropicFiltering.ForceEnable;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var house=StiltHouseBuilder.Build();var wedding=WeddingYardBuilder.Build();var kitchen=KitchenBuilder.Build();var ground=LandscapeBuilder.BuildGround();var village=LandscapeBuilder.BuildVillage();
            var cattle=CattleShelterBuilder.Build();
            var lighting=LightingBuilder.Build();
            MeshFactory.BakeCategory(house,"House");MeshFactory.BakeCategory(wedding,"Wedding");MeshFactory.BakeCategory(kitchen,"KitchenAndProps");MeshFactory.BakeCategory(ground,"Ground");MeshFactory.BakeCategory(village,"Village");
            MeshFactory.BakeCategory(cattle,"CattleShelter");
            var go=new GameObject("07 REVIEW CAMERA - environment inspection only");var camera=go.AddComponent<Camera>();camera.nearClipPlane=.05f;camera.farClipPlane=700;camera.fieldOfView=62;camera.allowHDR=true;camera.allowMSAA=true;camera.clearFlags=CameraClearFlags.Skybox;camera.renderingPath=RenderingPath.Forward;
            go.AddComponent<AudioListener>();var review=go.AddComponent<EnvironmentCamera>();review.lighting=lighting;
            var grade=AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Camera grade.mat");if(!grade){grade=new Material(Shader.Find("ThatNight/CameraGrade"));AssetDatabase.CreateAsset(grade,"Assets/ChapterOne/Materials/Camera grade.mat");}review.grade=grade;review.SetView(0);
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};AssetDatabase.SaveAssets();
            if(!Application.isBatchMode&&SceneNavigation.Enabled){SceneNavigation.Enable();SceneNavigation.FrameHouse();}
            int triangles=0;foreach(var mesh in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))triangles+=mesh.sharedMesh.triangles.Length/3;
            Debug.Log("ENVIRONMENT_SCENE renderers="+UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Length+" triangles="+triangles);
        }
    }
}
