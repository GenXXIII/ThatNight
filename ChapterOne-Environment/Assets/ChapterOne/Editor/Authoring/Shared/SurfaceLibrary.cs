using UnityEditor;
using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    public static class SurfaceLibrary
    {
        const string Root = "Assets/ChapterOne/Materials/";
        public static Material Wood => AssetDatabase.LoadAssetAtPath<Material>(Root + "Weathered teak.mat");

        public static Material Plain(string name, Color color, float smooth = .12f, float metal = 0)
        {
            string path = Root + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material) return material; // Preserve authored colors and shader settings.
            material = new Material(Shader.Find("ThatNight/WeatheredSurface"));
            material.color = color;
            material.SetFloat("_Glossiness", smooth);
            material.SetFloat("_Metallic", metal);
            material.enableInstancing = true;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }
    }
}
