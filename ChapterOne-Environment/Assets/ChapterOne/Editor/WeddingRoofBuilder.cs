using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class WeddingRoofBuilder
    {
        public static void Build()
        {
            Section("Left roof over guest seating",-22,-3,1.5f,SiteLayout.EntranceSideFront,SiteLayout.WeddingRear,true,true);
            Section("Middle roof over entrance aisle",-3,3,1.9f,SiteLayout.RoofFront,SiteLayout.WeddingRear,false);
            Section("Right roof over dance and ceremonies",3,22,1.5f,SiteLayout.EntranceSideFront,SiteLayout.WeddingRear,true,true);
        }
        public static void Section(string name,float left,float right,float rise,float front,float rear,bool frontValance=true,bool frontGable=false)
        {
            var old=Parent;Parent=Group(name,Vector3.zero);
            float eave=SiteLayout.WeddingEave;
            if(frontGable)
                GableCurtain(left,right,rise,front,eave);
            int bands=right-left>10?8:4;
            // Joined canvas roofs with a separate raised ridge for each section.
            for(int b=0;b<bands;b++)
            {
                int band=b;
                Grid("Pitched wedding canvas panel",12,28,(u,v)=>{
                    float t=(band+u)/bands;
                    float ridge=1-Mathf.Abs(t*2-1);
                    return new Vector3(Mathf.Lerp(left,right,t),eave+ridge*rise-
                        Mathf.Sin(u*Mathf.PI)*.045f-Mathf.Sin(v*Mathf.PI*4)*.035f,Mathf.Lerp(front,rear,v));
                },band%2==0?S.Cream:S.Pink,new Vector2(1,8),true);
            }
            for(int bay=0;bay<=4;bay++)
            {
                float z=Mathf.Lerp(front,rear,bay/4f),mid=(left+right)/2;
                foreach(float x in new[]{left,right})
                    Cylinder("Marquee support pole",new Vector3(x,0,z),.055f,eave,S.DarkWood);
                Beam("Roof frame left pitch",new Vector3(left,eave-.08f,z),new Vector3(mid,eave+rise-.08f,z),.065f,S.DarkWood);
                Beam("Roof frame right pitch",new Vector3(mid,eave+rise-.08f,z),new Vector3(right,eave-.08f,z),.065f,S.DarkWood);
                if(bay>0&&bay<4)LightingBuilder.Bulb(new Vector3(mid,eave+rise-.4f,z),10,3.2f,false);
            }
            Beam("Marquee ridge beam",new Vector3((left+right)/2,eave+rise-.07f,front),new Vector3((left+right)/2,eave+rise-.07f,rear),.065f,S.DarkWood);
            foreach(float z in new[]{front,rear})
            {
                if(z==front&&!frontValance)continue;
                var save=Parent;Parent=Group("Roof edge fabric valance",new Vector3(left,eave,z));
                WeddingYardBuilder.Swag((right-left)/2,.45f,S.Pink);
                Parent=Group("Second valance span",new Vector3((right-left)/2,0,0));
                WeddingYardBuilder.Swag((right-left)/2,.45f,S.Pink);Parent=save;
            }
            Parent=old;
        }
        static void GableCurtain(float left,float right,float rise,float front,float eave)
        {
            // Match the five lower curtain bays: fourteen pleats per bay.
            // Sample each fold several times so its depth survives mesh tessellation.
            var cloth=Grid("Pleated ivory gable curtain",320,24,(u,v)=>
            {
                float ridge=(1-Mathf.Abs(u*2-1))*rise;
                float fold=Mathf.Sin(u*Mathf.PI*140)*Mathf.Lerp(.065f,.018f,v);
                float billow=Mathf.Sin(Mathf.Repeat(u*5,1)*Mathf.PI)*Mathf.Sin(v*Mathf.PI)*.09f;
                float y=Mathf.Lerp(eave-.12f,eave+ridge,v);
                return new Vector3(Mathf.Lerp(left,right,u),y,front+.05f+fold+billow);
            },S.Cream,Vector2.one,true);
            // World-height UVs keep the woven texture size consistent with the curtains
            // below, instead of stretching the weave to fit the triangular silhouette.
            var mesh=cloth.GetComponent<MeshFilter>().sharedMesh;
            var vertices=mesh.vertices;var uv=new Vector2[vertices.Length];
            for(int i=0;i<vertices.Length;i++)
                uv[i]=new Vector2((vertices[i].x-left)*15/(right-left),(vertices[i].y-eave)*3/eave);
            mesh.uv=uv;mesh.RecalculateTangents();
        }
    }
}
