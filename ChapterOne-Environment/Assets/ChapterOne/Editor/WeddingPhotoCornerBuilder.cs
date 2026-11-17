using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class WeddingPhotoCornerBuilder
    {
        public static void Build()
        {
            var old=Parent;Parent=Group("Entrance guest photo corner",new Vector3(7,0,SiteLayout.EntranceSideFront-.9f));
            Grid("Photo corner carpet",8,8,(u,v)=>new Vector3((u-.5f)*6.4f,-.04f,-1.3f+v*2.3f),S.Pink,new Vector2(4,2),true);
            // Backdrop is on the existing fabric frontage, beside the smaller entrance.
            for(int i=0;i<17;i++)
            {
                float x=-2.8f+i*.35f,y=2.9f+Mathf.Sin(i/16f*Mathf.PI)*.6f;
                WeddingYardBuilder.Rosette(new Vector3(x,y,.65f),.17f);
                Ball("Ivory photo garland bloom",new Vector3(x,y+.2f,.65f),new Vector3(.18f,.22f,.17f),S.Cream);
            }
            Chair(new Vector3(-.7f,0,0));Chair(new Vector3(.7f,0,0));
            for(int s=-1;s<=1;s+=2)
            {
                var p=new Vector3(s*2.3f,0,0);
                Cylinder("Photo flower pedestal",p,.14f,1.0f,S.Brass);
                for(int j=0;j<9;j++)
                {
                    float a=j*2.4f;var tip=p+new Vector3(Mathf.Sin(a)*.25f,1.3f+(j%3)*.09f,Mathf.Cos(a)*.2f);
                    Beam("Cut flower stem",p+Vector3.up*.9f,tip,.009f,S.Leaf);
                    Ball("Photo arrangement flower",tip,Vector3.one*.18f,j%2==0?S.Pink:S.Cream);
                }
            }
            LightingBuilder.Bulb(new Vector3(0,3.4f,-.5f),5,2.3f,false);
            Parent=old;
        }
        static void Chair(Vector3 p)
        {
            var old=Parent;Parent=Group("Ivory covered photo chair",p);
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)
                Box("Photo chair leg",new Vector3(x*.26f,.23f,z*.25f),new Vector3(.06f,.46f,.06f),S.Wood);
            Box("Photo seat cushion",new Vector3(0,.5f,0),new Vector3(.65f,.14f,.62f),S.Cream);
            Box("Photo chair back",new Vector3(0,.96f,.25f),new Vector3(.65f,.9f,.09f),S.Cream);
            Grid("Photo chair gathered skirt",28,10,(u,v)=>{
                float a=u*Mathf.PI*2,r=.36f+Mathf.Sin(a*12)*.025f;
                return new Vector3(Mathf.Cos(a)*r,.48f-v*.44f,Mathf.Sin(a)*r);
            },S.Cream,new Vector2(2,1),true);
            Box("Photo chair pink sash",new Vector3(0,1.0f,.19f),new Vector3(.67f,.15f,.025f),S.Pink);
            WeddingYardBuilder.Rosette(new Vector3(0,1.0f,.14f),.13f);Parent=old;
        }
    }
}
