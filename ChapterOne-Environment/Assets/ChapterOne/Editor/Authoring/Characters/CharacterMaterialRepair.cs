using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    // The imported characters have separate body, head and alpha-card submeshes.
    // An opaque face material on the third slot breaks hair and eyelashes.
    public static class CharacterMaterialRepair
    {
        public static Material OpacityMaterial(bool female)
        {
            string folder = female ? "Mother" : "Groom";
            string prefix = female ? "f001" : "m002";
            string path = "Assets/ChapterOne/Gameplay/Materials/" + folder + "Hair.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!material)
            {
                material = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/ChapterOne/Characters/" + folder + "/" + prefix + "_opacity_color.tga");
            material.color = Color.white;
            material.SetFloat("_Mode", 1);
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetFloat("_Cutoff", 0.45f);
            material.SetFloat("_Glossiness", 0.18f);
            material.EnableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            EditorUtility.SetDirty(material);
            return material;
        }

        [MenuItem("THAT NIGHT/Chapter 1/Repair character material slots")]
        public static void Apply()
        {
            if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Stop Play first.");
            int repaired = 0;
            foreach (var renderer in Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (!renderer.sharedMesh || renderer.sharedMesh.subMeshCount != 3) continue;
                bool female = renderer.sharedMesh.name.StartsWith("f001");
                if (!female && !renderer.sharedMesh.name.StartsWith("m002")) continue;
                var materials = renderer.sharedMaterials;
                if (materials.Length != 3) continue;
                Undo.RecordObject(renderer, "Repair hair material");
                materials[2] = OpacityMaterial(female);
                renderer.sharedMaterials = materials;
                EditorUtility.SetDirty(renderer);
                repaired++;
            }
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log("Character material slots repaired: " + repaired);
        }
    }
}
