using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class LightingBuilder
    {
        public static readonly List<Light> Practicals = new List<Light>();

        public static void Bulb(Vector3 position, float range, float intensity, bool castsShadows)
        {
            var root = Group("Tungsten bulb and light", position);
            var light = root.gameObject.AddComponent<Light>();
            light.type = castsShadows ? LightType.Spot : LightType.Point;
            light.color = new Color(1f, .82f, .64f);
            light.range = range;
            light.intensity = intensity;

            if (castsShadows)
            {
                light.spotAngle = 145f;
                light.innerSpotAngle = 85f;
                root.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }

            light.shadows = castsShadows ? LightShadows.Soft : LightShadows.None;
            light.shadowResolution = LightShadowResolution.Low;
            light.shadowStrength = .7f;
            light.shadowBias = .035f;
            light.renderMode = castsShadows
                ? LightRenderMode.ForcePixel
                : LightRenderMode.ForceVertex;
            Practicals.Add(light);

            Ball("Incandescent bulb glass", position, new Vector3(.09f, .13f, .09f), S.Bulb);
            Cylinder("Ceramic bulb socket", position + Vector3.up * .08f, .025f, .07f, S.Porcelain);
            Beam("Bulb pendant wire", position + Vector3.up * .13f, position + Vector3.up * .4f, .01f, S.Black);
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
