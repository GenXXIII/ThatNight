using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class KitchenBuilder
    {
        public static Transform Build()
        {
            Parent=null;var root=Group("03 KITCHEN AND YARD PROPS",Vector3.zero);Parent=root;
            Parent=Group("Rural Khmer kitchen - rear cooking and washing yard",SiteLayout.KitchenPosition);
            float halfWidth=SiteLayout.KitchenWidth/2,halfDepth=SiteLayout.KitchenDepth/2;
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z++)
            {
                var p=new Vector3(x*(halfWidth-.3f),0,z*(halfDepth-.35f));
                Box("Kitchen post stone footing",p+Vector3.up*.1f,new Vector3(.32f,.2f,.32f),S.Clay);
                Box("Kitchen timber post",p+Vector3.up*1.48f,new Vector3(.16f,2.96f,.16f),S.Bark,true);
                Beam("Kitchen corner brace",p+Vector3.up*2.3f,p+new Vector3(-x*.6f,2.96f,0),.075f,S.DarkWood);
            }
            Grid("Weathered corrugated kitchen roof",140,14,(u,v)=>new Vector3((u-.5f)*SiteLayout.KitchenWidth,3.65f-v*.8f+Mathf.Sin(u*Mathf.PI*120)*.035f,(v-.5f)*SiteLayout.KitchenDepth),S.Rust,new Vector2(6,4),true);
            for(int z=-2;z<=2;z++)
            {
                float pz=z*(halfDepth-.3f)/2;
                Beam("Exposed kitchen roof purlin",new Vector3(-halfWidth,3.22f-pz*.8f/SiteLayout.KitchenDepth,pz),new Vector3(halfWidth,3.22f-pz*.8f/SiteLayout.KitchenDepth,pz),.10f,S.DarkWood);
            }
            // Open front, low side walls and ventilated woven rear panels.
            for(int side=-1;side<=1;side+=2)
            {
                for(int i=0;i<30;i++)Box("Kitchen low weathered wall",new Vector3(side*(halfWidth-.3f),.68f,-halfDepth+.4f+i*(SiteLayout.KitchenDepth-.8f)/30),new Vector3(.055f,1.3f,.23f),S.Wood);
                Beam("Kitchen side wall rail",new Vector3(side*(halfWidth-.3f),1.38f,-halfDepth+.3f),new Vector3(side*(halfWidth-.3f),1.38f,halfDepth-.3f),.075f,S.DarkWood);
            }
            for(int col=0;col<50;col++)Box("Rear woven bamboo upright",new Vector3(Mathf.Lerp(-halfWidth+.3f,halfWidth-.3f,col/49f),1.4f,halfDepth-.35f),new Vector3(.075f,2.7f,.022f),S.Wood);
            for(int row=0;row<26;row++)Box("Rear woven bamboo cross strip",new Vector3(0,.12f+row*.10f,halfDepth-.37f+(row%2)*.024f),new Vector3(SiteLayout.KitchenWidth-.6f,.055f,.02f),S.Bark);
            Box("Smoke darkened timber behind hearth",new Vector3(-1.15f,1.55f,halfDepth-.4f),new Vector3(3.1f,2.6f,.035f),S.DarkWood);
            KitchenPropsBuilder.Build();
            LightingBuilder.Bulb(new Vector3(.5f,2.85f,-.5f),7.5f,3.5f,true);
            Parent=null;return root;
        }
    }
}
