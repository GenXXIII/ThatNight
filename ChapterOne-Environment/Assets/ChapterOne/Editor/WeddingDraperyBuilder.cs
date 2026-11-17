using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Temporary wedding textile enclosure: front and sides only, open toward the house.
    internal static class WeddingDraperyBuilder
    {
        const float Height=SiteLayout.WeddingEave,Opening=SiteLayout.EntranceHalfWidth,Rear=SiteLayout.WeddingRear;
        public static void Build(float front)
        {
            front=SiteLayout.WeddingFront;
            var old=Parent;
            Parent=Group("Wedding fabric enclosure - front and sides - open rear",Vector3.up*SiteLayout.HillElevation);
            float half=SiteLayout.HalfWidth;
            float sideFront=SiteLayout.EntranceSideFront;
            Run(new Vector3(-half,0,sideFront),new Vector3(-Opening,0,sideFront),5);
            Run(new Vector3(Opening,0,sideFront),new Vector3(half,0,sideFront),5);
            Run(new Vector3(-half,0,sideFront),new Vector3(-half,0,Rear),8);
            Run(new Vector3(half,0,sideFront),new Vector3(half,0,Rear),8);
            // Close the marked porch-side gaps beneath the middle roof.
            var enclosure=Parent;
            Parent=Group("Entrance porch left fabric cover",Vector3.zero);
            Run(new Vector3(-Opening,0,SiteLayout.RoofFront),new Vector3(-Opening,0,sideFront),1);
            Parent=enclosure;
            Parent=Group("Entrance porch right fabric cover",Vector3.zero);
            Run(new Vector3(Opening,0,SiteLayout.RoofFront),new Vector3(Opening,0,sideFront),1);
            Parent=enclosure;
            // Full-width passage: no inner curtain panels, returns or doorway posts.
            Grid("Fabric entrance runner",6,90,(u,v)=>{
                float x=Mathf.Lerp(-1.0f,1.0f,u),z=Mathf.Lerp(front-2,Rear,v);
                return new Vector3(x,GroundDetailBuilder.Height(x,z)-SiteLayout.HillElevation+.012f,z);
            },S.Pink,new Vector2(2,12),true);
            LightingBuilder.Bulb(new Vector3(0,3.25f,front+.35f),6,2.5f,false);
            Parent=old;
        }
        static void Run(Vector3 a,Vector3 b,int bays)
        {
            Vector3 dir=(b-a).normalized;float width=Vector3.Distance(a,b)/bays;
            for(int i=0;i<bays;i++)
            {
                var old=Parent;Parent=Group("Hanging fabric bay",Vector3.Lerp(a,b,i/(float)bays));
                Parent.localRotation=Quaternion.FromToRotation(Vector3.right,dir);
                Cylinder("Slim temporary wedding pole",Vector3.zero,.035f,Height+.08f,S.DarkWood);
                if(i==bays-1)Cylinder("Slim temporary wedding pole",Vector3.right*width,.035f,Height+.08f,S.DarkWood);
                Beam("Fabric suspension rail",Vector3.up*Height,new Vector3(width,Height,0),.04f,S.DarkWood);
                Grid("Loose pleated ivory curtain",64,24,(u,v)=>{
                    float sag=Mathf.Sin(u*Mathf.PI)*.09f;
                    float hem=.08f+Mathf.Sin(u*Mathf.PI*18)*.035f;
                    float y=Mathf.Lerp(Height-sag,hem,v);
                    float fold=Mathf.Sin(u*Mathf.PI*28)*(.055f+v*.045f);
                    float billow=Mathf.Sin(u*Mathf.PI)*Mathf.Sin(v*Mathf.PI)*.19f;
                    return new Vector3(u*width,y,fold+billow+.05f);
                },S.Cream,new Vector2(3,3),true);
                Swags(width);
                WeddingYardBuilder.Rosette(new Vector3(0,Height,-.15f),.22f);
                Parent=old;
            }
        }
        static void Swags(float width)
        {
            var old=Parent;Parent=Group("Dusty pink gathered valance",new Vector3(0,Height,-.16f));
            WeddingYardBuilder.Swag(width,.65f,S.Pink);
            Parent=Group("Ivory draped trim",new Vector3(0,-.13f,-.04f));
            WeddingYardBuilder.Swag(width,.65f,S.Cream);
            Parent=old;
        }
    }
}
