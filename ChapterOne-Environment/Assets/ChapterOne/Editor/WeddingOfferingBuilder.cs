using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Reference-based fruit and ceremonial props, shared by household wedding dressing.
    internal static class WeddingOfferingBuilder
    {
        public static void FruitTray(Vector3 p,int kind,bool silver=false)
        {
            var old=Parent;Parent=Group("Wedding fruit offering tray "+kind,p);
            var metal=silver?S.Metal:S.Brass;
            Lathe("Raised ceremonial pedestal tray",Vector3.zero,new[]{new Vector2(0,0),new Vector2(.18f,0),new Vector2(.20f,.025f),new Vector2(.10f,.09f),new Vector2(.075f,.20f),new Vector2(.13f,.25f),new Vector2(.34f,.28f),new Vector2(.37f,.32f),new Vector2(.36f,.34f),new Vector2(0,.30f)},metal,32);
            Tube("Rolled offering tray rim",new Vector3(0,.335f,0),.36f,.012f,metal,Quaternion.Euler(90,0,0),36);
            for(int i=0;i<20;i++)
            {
                float a=i*Mathf.PI/10;
                Ball("Tray embossed rim bead",new Vector3(Mathf.Sin(a)*.36f,.34f,Mathf.Cos(a)*.36f),Vector3.one*.025f,metal);
            }
            var orange=S.Plain("Orange peel",new Color(.85f,.31f,.035f),.18f);
            var apple=S.Plain("Red apple skin",new Color(.53f,.055f,.025f),.30f);
            var longan=S.Plain("Longan shell",new Color(.50f,.31f,.13f),.09f);
            if(kind==1)
            {
                for(int i=0;i<9;i++)Banana(new Vector3((i%3-1)*.12f,.36f+(i/3)*.08f,-.09f),i*33);
            }
            else if(kind==3)
            {
                var green=S.Plain("Watermelon rind",new Color(.20f,.36f,.08f),.24f);
                Grid("Striped whole watermelon",40,20,(u,v)=>{
                    float a=u*Mathf.PI*2,b=v*Mathf.PI;
                    return new Vector3(Mathf.Sin(b)*Mathf.Cos(a)*.23f,.56f+Mathf.Cos(b)*.24f,Mathf.Sin(b)*Mathf.Sin(a)*.20f);
                },green,Vector2.one,false,q=>{
                    float a=Mathf.Atan2(q.z,q.x);float stripe=.57f+.43f*Mathf.SmoothStep(0,1,(Mathf.Sin(a*10+q.y*4)+1)/2);
                    return new Color(stripe,stripe, stripe,1);
                });
            }
            else
            {
                int rings=kind==2?4:3;
                float r=kind==2?.055f:.095f;
                for(int layer=0;layer<rings;layer++)
                {
                    int count=layer==rings-1?1:(kind==2?12:7)-layer*2;
                    float radius=count==1?0:.22f-layer*.06f;
                    for(int i=0;i<count;i++)
                    {
                        float a=i*Mathf.PI*2/count+layer*.5f;
                        var c=new Vector3(Mathf.Cos(a)*radius,.35f+r+layer*r*1.5f,Mathf.Sin(a)*radius);
                        Ball(kind==2?"Longan fruit":kind==4?"Red apple":"Mandarin orange",c,new Vector3(r*2,r*1.85f,r*2),kind==2?longan:kind==4?apple:orange);
                        if(kind!=2)Beam("Fruit stalk",c+Vector3.up*r*.85f,c+Vector3.up*(r+.025f),.009f,S.DarkWood);
                    }
                }
            }
            Parent=old;
        }
        static void Banana(Vector3 p,float yaw)
        {
            var rot=Quaternion.Euler(0,yaw,0);
            var yellow=S.Plain("Ripe banana peel",new Color(.78f,.65f,.11f),.16f);
            Grid("Curved offering banana",14,8,(u,v)=>{
                float a=v*Mathf.PI*2,r=.026f*Mathf.Sin(u*Mathf.PI)+.008f;
                return p+rot*new Vector3((u-.5f)*.32f,.075f*Mathf.Sin(u*Mathf.PI)+Mathf.Cos(a)*r,Mathf.Sin(a)*r);
            },yellow,Vector2.one,true);
            Beam("Banana dark tip",p+rot*new Vector3(.16f,0,0),p+rot*new Vector3(.18f,-.01f,0),.012f,S.DarkWood);
        }
        public static void WristThreads(Vector3 p)
        {
            var old=Parent;Parent=Group("Cotton wrist tying threads in ceremonial bowl",p);
            Lathe("Silver wrist thread bowl",Vector3.zero,new[]{new Vector2(0,0),new Vector2(.12f,0),new Vector2(.13f,.035f),new Vector2(.20f,.09f),new Vector2(.23f,.15f),new Vector2(.225f,.16f),new Vector2(.20f,.13f),new Vector2(.10f,.035f),new Vector2(0,.035f)},S.Metal,32);
            var cotton=S.Plain("Natural cotton blessing thread",new Color(.90f,.85f,.70f),.03f);
            for(int strand=0;strand<16;strand++)
            {
                float offset=(strand-7.5f)*.011f;
                var previous=new Vector3(-.14f,.10f,offset);
                for(int segment=1;segment<=16;segment++)
                {
                    float t=segment/16f;
                    var next=new Vector3(-.14f+t*.28f,.10f+Mathf.Sin(t*Mathf.PI)*.014f+(strand%3)*.006f,offset+Mathf.Sin(t*Mathf.PI*2)*.025f);
                    Beam("Loose cotton tying strand",previous,next,.004f,cotton);previous=next;
                }
            }
            for(int i=0;i<3;i++)Tube("Coiled cotton thread",new Vector3(-.04f+i*.04f,.13f+i*.004f,0),.08f,.004f,cotton,Quaternion.Euler(90,0,0),32);
            Parent=old;
        }
        public static void Candle(Vector3 p)
        {
            Cylinder("Gold candle saucer",p,.12f,.025f,S.Brass,24);
            Cylinder("Green wrapped candle holder",p+Vector3.up*.025f,.045f,.18f,S.Leaf,24);
            var wax=S.Plain("Red ceremony candle wax",new Color(.70f,.035f,.02f),.18f);
            Cylinder("Red wedding candle",p+Vector3.up*.18f,.025f,.34f,wax,20);
            Beam("Unlit candle wick",p+Vector3.up*.52f,p+Vector3.up*.545f,.004f,S.Black);
        }
        public static void Flowers(Vector3 p)
        {
            Lathe("Ceremony flower vase",p,new[]{new Vector2(.10f,0),new Vector2(.15f,.04f),new Vector2(.17f,.20f),new Vector2(.08f,.38f),new Vector2(.10f,.42f)},S.Metal,24);
            for(int i=0;i<9;i++)
            {
                float a=i*2.4f;var tip=p+new Vector3(Mathf.Sin(a)*.22f,.58f+(i%3)*.09f,Mathf.Cos(a)*.22f);
                Beam("Flower stem",p+Vector3.up*.35f,tip,.009f,S.Leaf);
                for(int j=0;j<6;j++)
                {
                    float b=j*Mathf.PI/3;
                    Ball("Ceremony flower petal",tip+new Vector3(Mathf.Sin(b)*.045f,0,Mathf.Cos(b)*.045f),new Vector3(.07f,.055f,.07f),i%2==0?S.Cream:S.Pink);
                }
                Ball("Flower gold centre",tip+Vector3.up*.02f,Vector3.one*.035f,S.Brass);
            }
        }
    }
}
