using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    internal static class SurfaceLibrary
    {
        public static Material Wood,DarkWood,Floor,Roof,Soil,Grass,Bark,Pink,Cream,Metal,Rust,Black,Leaf,Foliage,Clay,Porcelain,Brass,Water,Carving,Bulb;
        static readonly Dictionary<string,Material> library=new Dictionary<string,Material>();
        const string Root="Assets/ChapterOne/";
        public static void Prepare()
        {
            library.Clear();
            foreach(string guid in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"Textures"}))
            {
                string path=AssetDatabase.GUIDToAssetPath(guid);var imp=AssetImporter.GetAtPath(path) as TextureImporter;
                if(!imp)continue;
                imp.maxTextureSize=1024;imp.mipmapEnabled=true;imp.anisoLevel=4;
                if(path.Contains("NormalGL"))imp.textureType=TextureImporterType.NormalMap;
                if(path.Contains("AmbientOcclusion")||path.Contains("Roughness"))imp.sRGBTexture=false;
                if(path.Contains("Foliage")){imp.alphaIsTransparency=true;imp.mipMapsPreserveCoverage=true;imp.alphaTestReferenceValue=.42f;}
                imp.SaveAndReimport();
            }
            Wood=Pbr("Weathered teak","Planks037A",new Color(.70f,.65f,.57f));
            DarkWood=Pbr("Dark hardwood","Planks037A",new Color(.38f,.33f,.26f));
            Floor=Pbr("Worn floorboards","WoodFloor051",new Color(.71f,.65f,.54f),.25f);
            Roof=Pbr("Old clay tiles","RoofingTiles014B",new Color(.47f,.41f,.34f));
            Soil=Terrain("Packed brown soil");Grass=Terrain("Garden earth");
            Bark=Pbr("Rough bark","Wood066",new Color(.60f,.55f,.43f));
            Pink=Pbr("Dusty rose wedding cloth","Fabric035",new Color(.82f,.47f,.49f));
            Cream=Pbr("Ivory wedding cloth","Fabric035",new Color(.96f,.89f,.72f));
            Pink.SetFloat("_Desaturate",1);Cream.SetFloat("_Desaturate",1);
            Metal=Plain("Worn aluminum",new Color(.44f,.47f,.44f),.36f,.7f);
            Rust=Pbr("Oxidized metal","Metal033",new Color(.58f,.35f,.23f),.25f);Rust.SetFloat("_Metallic",.45f);
            Black=Plain("Aged rubber",new Color(.025f,.028f,.025f),.18f);
            Leaf=Plain("Banana green",new Color(.12f,.27f,.07f),.14f);
            Clay=Plain("Earthenware",new Color(.34f,.20f,.12f),.18f);
            Porcelain=Plain("Ivory porcelain",new Color(.8f,.8f,.7f),.6f);
            Brass=Plain("Old ceremonial brass",new Color(.65f,.43f,.13f),.48f,.64f);
            Water=Plain("Dark water",new Color(.06f,.1f,.085f),.85f,.18f);
            Carving=Plain("Carved cabinet",new Color(.65f,.5f,.36f),.3f);Carving.mainTexture=Texture("CarvedTeak.png");
            Foliage=Plain("Mango leaves",new Color(.8f,.88f,.73f),.16f);
            Foliage.shader=Shader.Find("Standard");Foliage.mainTexture=Texture("MangoFoliage.png");
            Foliage.SetFloat("_Mode",1);Foliage.SetFloat("_Cutoff",.42f);Foliage.EnableKeyword("_ALPHATEST_ON");Foliage.SetOverrideTag("RenderType","TransparentCutout");Foliage.renderQueue=2450;
            Bulb=Plain("Warm incandescent glass",new Color(1,.79f,.42f),.4f);
            Bulb.shader=Shader.Find("Standard");Bulb.EnableKeyword("_EMISSION");Bulb.SetColor("_EmissionColor",new Color(1,.62f,.22f)*3.5f);
            foreach(var mat in library.Values)EditorUtility.SetDirty(mat);
        }
        public static Texture2D Texture(string path)=>AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Textures/"+path);
        static Material Terrain(string name)
        {
            var mat=Plain(name,new Color(.72f,.65f,.54f));mat.shader=Shader.Find("ThatNight/VillageGround");
            mat.SetTexture("_MainTex",Texture("Ground054/Ground054_1K-JPG_Color.jpg"));
            mat.SetTexture("_BumpMap",Texture("Ground054/Ground054_1K-JPG_NormalGL.jpg"));
            mat.SetTexture("_GrassTex",Texture("Ground037/Ground037_1K-JPG_Color.jpg"));
            mat.SetTexture("_GrassBump",Texture("Ground037/Ground037_1K-JPG_NormalGL.jpg"));mat.SetFloat("_TileScale",.4f);return mat;
        }
        public static Material Plain(string name,Color color,float smooth=.12f,float metal=0)
        {
            if(library.TryGetValue(name,out var cached))return cached;
            string path=Root+"Materials/"+name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(!mat){mat=new Material(Shader.Find("ThatNight/WeatheredSurface"));AssetDatabase.CreateAsset(mat,path);}
            mat.color=color;mat.SetFloat("_Glossiness",smooth);mat.SetFloat("_Metallic",metal);mat.enableInstancing=true;
            library.Add(name,mat);EditorUtility.SetDirty(mat);return mat;
        }
        public static Material Pbr(string name,string asset,Color color,float smooth=.12f)
        {
            var m=Plain(name,color,smooth);m.mainTexture=Texture(asset+"/"+asset+"_1K-JPG_Color.jpg");
            m.SetTexture("_BumpMap",Texture(asset+"/"+asset+"_1K-JPG_NormalGL.jpg"));
            m.SetTexture("_OcclusionMap",Texture(asset+"/"+asset+"_1K-JPG_AmbientOcclusion.jpg"));
            return m;
        }
    }
}
