using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class LightingBuilder
    {
        public static readonly List<Light> Practicals=new List<Light>();
        public static void Bulb(Vector3 p,float range,float intensity,bool shadow)
        {
            var root=Group("Tungsten bulb and light",p);var light=root.gameObject.AddComponent<Light>();light.type=shadow?LightType.Spot:LightType.Point;light.color=new Color(1,.82f,.64f);light.range=range;light.intensity=intensity;
            if(shadow){light.spotAngle=145;light.innerSpotAngle=85;root.localRotation=Quaternion.Euler(90,0,0);}
            light.shadows=shadow?LightShadows.Soft:LightShadows.None;light.shadowResolution=UnityEngine.Rendering.LightShadowResolution.Low;light.shadowStrength=.7f;light.shadowBias=.035f;
            light.renderMode=shadow?LightRenderMode.ForcePixel:LightRenderMode.Auto;Practicals.Add(light);
            Ball("Incandescent bulb glass",p,new Vector3(.09f,.13f,.09f),S.Bulb);
            Cylinder("Ceramic bulb socket",p+Vector3.up*.08f,.025f,.07f,S.Porcelain);
            Beam("Bulb pendant wire",p+Vector3.up*.13f,p+Vector3.up*.4f,.01f,S.Black);
        }
        public static EnvironmentLighting Build()
        {
            Parent=null;var root=Group("06 LIGHTING - evening reference and daylight review",Vector3.zero);var lighting=root.gameObject.AddComponent<EnvironmentLighting>();
            var moon=new GameObject("Moon and sun key");moon.transform.SetParent(root);lighting.key=moon.AddComponent<Light>();lighting.key.type=LightType.Directional;lighting.key.shadows=LightShadows.Soft;lighting.key.shadowStrength=.65f;lighting.key.shadowBias=.05f;
            var sky=AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Village sky.mat");if(!sky){sky=new Material(Shader.Find("ThatNight/VillageSky"));AssetDatabase.CreateAsset(sky,"Assets/ChapterOne/Materials/Village sky.mat");}
            lighting.sky=sky;lighting.practicals=Practicals.ToArray();lighting.Apply(false);
            return lighting;
        }
    }
}
