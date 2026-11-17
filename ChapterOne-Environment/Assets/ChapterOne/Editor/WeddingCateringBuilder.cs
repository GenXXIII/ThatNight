using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Staff cooking area for preparing the wedding banquet.
    internal static class WeddingCateringBuilder
    {
        public static void Build()
        {
            var old=Parent;Parent=Group("Rear right cooks kitchen under roof extension",Vector3.zero);
            float left=SiteLayout.CateringLeft,right=SiteLayout.CateringRight,front=SiteLayout.WeddingRear,back=SiteLayout.CateringBack;
            WeddingRoofBuilder.Section("Cooks pink and ivory wedding canopy",left,right,1.5f,front-.15f,back);
            // Open-sided cooking shelter with high roof ventilation.
            var parent=Parent;Parent=Group("Cooks preparation washing and storage",new Vector3(17,0,21));
            KitchenPropsBuilder.Build();Parent=parent;
            for(int i=0;i<3;i++)
            {
                var p=new Vector3(14+i*2.3f,0,25.5f);
                Cylinder("Banquet stove base",p,.5f,.55f,S.Clay);
                Box("Banquet stove firebox",p+new Vector3(0,.22f,-.49f),new Vector3(.3f,.22f,.035f),S.Black);
                PropsBuilder.Pot(p+Vector3.up*.55f,.54f);
            }
            PropsBuilder.Table(new Vector3(17,0,12.9f),new Vector2(7,1.1f),.86f,S.Wood);
            for(int i=0;i<5;i++)PropsBuilder.Pot(new Vector3(14.5f+i*1.2f,.90f,12.9f),.27f);
            for(int i=0;i<2;i++)LightingBuilder.Bulb(new Vector3(17,3.8f,15+i*8),9,3,false);
            Parent=old;
        }
    }
}
