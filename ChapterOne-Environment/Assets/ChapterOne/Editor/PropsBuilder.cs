using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class PropsBuilder
    {
        public static void Table(Vector3 p,Vector2 size,float height,Material mat)
        {
            Box("Worn table top",p+Vector3.up*height,new Vector3(size.x,.07f,size.y),mat);
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Box("Table leg",p+new Vector3(x*(size.x/2-.1f),height/2,z*(size.y/2-.1f)),new Vector3(.07f,height,.07f),S.DarkWood);
        }
        public static void Bench(Vector3 p,float length)
        {
            Box("Bench seat",p+Vector3.up*.45f,new Vector3(length,.075f,.51f),S.Wood);
            for(int s=-1;s<=1;s+=2)
            {Box("Bench leg",p+new Vector3(s*(length/2-.16f),.23f,0),new Vector3(.085f,.46f,.42f),S.DarkWood);Box("Back upright",p+new Vector3(s*(length/2-.12f),.7f,.25f),new Vector3(.065f,.9f,.065f),S.DarkWood);}
            Box("Bench back crest",p+new Vector3(0,1.08f,.25f),new Vector3(length,.095f,.08f),S.Wood);
            for(int i=0;i<14;i++)Box("Backrest spindle",p+new Vector3(-length*.43f+i*length*.86f/13,.78f,.25f),new Vector3(.038f,.53f,.035f),S.Wood);
        }
        public static void Cabinet(Vector3 p)
        {
            Box("Carved teak cupboard",p+Vector3.up*.9f,new Vector3(1.5f,1.55f,.5f),S.DarkWood);
            Box("Cupboard crown",p+Vector3.up*1.72f,new Vector3(1.62f,.09f,.6f),S.Wood);
            for(int s=-1;s<=1;s+=2)
            {
                int sign=s;Grid("Carved door panel",1,1,(u,v)=>p+new Vector3(sign*.365f+(u-.5f)*.7f,.2f+v*1.4f,-.26f),S.Carving,Vector2.one,true);
                Tube("Brass ring handle",p+new Vector3(sign*.09f,.94f,-.29f),.045f,.008f,S.Brass,Quaternion.identity,16);
                Box("Cupboard foot",p+new Vector3(sign*.6f,.09f,0),new Vector3(.12f,.18f,.4f),S.Wood);
            }
        }
        public static void Jar(Vector3 p,float scale)
        {
            var old=Parent;Parent=Group("Large earthen water jar",p);Parent.localScale=Vector3.one*scale;
            var glaze=S.Pbr("Worn earthenware glaze","Ground054",new Color(.39f,.26f,.14f),.22f);glaze.SetFloat("_BumpScale",.12f);
            Lathe("Glazed water jar",Vector3.zero,new[]{new Vector2(.25f,0),new Vector2(.30f,.025f),new Vector2(.35f,.07f),new Vector2(.40f,.14f),new Vector2(.44f,.24f),new Vector2(.46f,.36f),new Vector2(.455f,.46f),new Vector2(.435f,.56f),new Vector2(.40f,.65f),new Vector2(.35f,.73f),new Vector2(.29f,.79f),new Vector2(.24f,.82f),new Vector2(.24f,.86f),new Vector2(.225f,.88f),new Vector2(.2f,.87f),new Vector2(.21f,.81f)},glaze,36);
            Cylinder("Water inside jar",new Vector3(0,.76f,0),.2f,.008f,S.Water);Parent=old;
        }
        public static void Basket(Vector3 p,float scale)
        {
            var old=Parent;Parent=Group("Woven basket",p);Parent.localScale=Vector3.one*scale;
            Lathe("Basket body",Vector3.zero,new[]{new Vector2(.23f,0),new Vector2(.32f,.08f),new Vector2(.37f,.55f),new Vector2(.35f,.59f)},S.Wood);
            for(int i=0;i<13;i++)Tube("Horizontal wicker weave",new Vector3(0,.05f+i*.042f,0),.24f+i*.009f,.008f,S.DarkWood,Quaternion.Euler(90,0,0),24);
            for(int i=0;i<18;i++){float a=i*Mathf.PI/9;Beam("Basket vertical weave",new Vector3(Mathf.Cos(a)*.23f,.02f,Mathf.Sin(a)*.23f),new Vector3(Mathf.Cos(a)*.36f,.58f,Mathf.Sin(a)*.36f),.011f,S.Cream);}
            Parent=old;
        }
        public static void CeramicSet(Vector3 p)
        {
            Lathe("Tea tray",p,new[]{new Vector2(0,0),new Vector2(.28f,0),new Vector2(.3f,.025f),new Vector2(.29f,.04f)},S.Metal);
            Lathe("Small teapot",p+new Vector3(0,.04f,0),new[]{new Vector2(.04f,0),new Vector2(.095f,.04f),new Vector2(.08f,.11f),new Vector2(.04f,.14f),new Vector2(0,.14f)},S.Porcelain);
            Beam("Teapot spout",p+new Vector3(.06f,.08f,0),p+new Vector3(.16f,.16f,0),.035f,S.Porcelain);
            Tube("Teapot handle",p+new Vector3(-.085f,.11f,0),.055f,.009f,S.Porcelain,Quaternion.identity,16);
            for(int s=-1;s<=1;s+=2)Cup(p+new Vector3(s*.19f,.035f,-.11f));
        }
        public static void Cup(Vector3 p)
        {
            Lathe("Porcelain tea bowl",p,new[]{new Vector2(.025f,0),new Vector2(.042f,.012f),new Vector2(.052f,.065f),new Vector2(.046f,.066f),new Vector2(.037f,.015f)},S.Porcelain,20);
        }
        public static void Pot(Vector3 p,float r)
        {
            Lathe("Aluminum cooking pot",p,new[]{new Vector2(r*.7f,0),new Vector2(r,.04f),new Vector2(r,r*.7f),new Vector2(r*.98f,r*.72f),new Vector2(0,r*.77f)},S.Metal,32);
            Ball("Pot lid handle",p+Vector3.up*(r*.81f),new Vector3(.13f,.055f,.07f),S.Black);
            for(int s=-1;s<=1;s+=2)Tube("Pot side handle",p+new Vector3(s*r,r*.53f,0),r*.18f,.015f,S.Black,Quaternion.Euler(0,90,0),16);
        }
        public static void Chair(Vector3 p,float yaw)
        {
            var old=Parent;Parent=Group("Red plastic chair",p);Parent.localEulerAngles=new Vector3(0,yaw,0);
            var plastic=S.Plain("Faded red plastic",new Color(.33f,.075f,.055f),.22f);
            Box("Molded seat",new Vector3(0,.44f,0),new Vector3(.43f,.045f,.44f),plastic);
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Beam("Splayed chair leg",new Vector3(x*.22f,0,z*.23f),new Vector3(x*.17f,.44f,z*.17f),.044f,plastic);
            for(int s=-1;s<=1;s+=2)Beam("Back upright",new Vector3(s*.18f,.43f,.17f),new Vector3(s*.20f,.94f,.26f),.045f,plastic);
            for(int i=0;i<5;i++){float x=(i-2)*.07f;Beam("Slotted chair back",new Vector3(x,.5f,.19f),new Vector3(x,.88f+.07f*(1-Mathf.Abs(x)/.2f),.25f),.035f,plastic);}
            Grid("Rounded chair back crest",16,2,(u,v)=>new Vector3((u-.5f)*.42f,.88f+Mathf.Sin(u*Mathf.PI)*.08f+v*.045f,.26f),plastic,Vector2.one,true);
            Parent=old;
        }
        public static void Motorbike(Vector3 position,float yaw,Color color)
        {
            var old=Parent;Parent=Group("2000s step-through motorcycle",position);Parent.localEulerAngles=new Vector3(0,yaw,0);
            var paint=S.Plain("Motorcycle paint "+color.ToString(),color,.48f,.25f);
            for(int s=-1;s<=1;s+=2)
            {
                var c=new Vector3(0,.34f,s*.64f);
                Tube("Rubber tire",c,.29f,.045f,S.Black,Quaternion.Euler(0,90,0),40);
                Tube("Wheel rim",c,.245f,.015f,S.Metal,Quaternion.Euler(0,90,0),32);
                for(int j=0;j<16;j++){float a=j*Mathf.PI/8;Beam("Wheel spoke",c,c+new Vector3(0,Mathf.Cos(a)*.24f,Mathf.Sin(a)*.24f),.0045f,S.Metal);}
                Ball("Wheel hub",c,new Vector3(.17f,.095f,.095f),S.Metal);
            }
            Beam("Motorbike frame",new Vector3(0,.4f,-.6f),new Vector3(0,.78f,.32f),.09f,S.Metal);
            Beam("Lower frame",new Vector3(0,.43f,.53f),new Vector3(0,.43f,-.3f),.1f,S.Metal);
            Ball("Engine casing",new Vector3(0,.41f,-.03f),new Vector3(.3f,.3f,.37f),S.Metal);
            Box("Black saddle",new Vector3(0,.83f,-.22f),new Vector3(.36f,.1f,.78f),S.Black);
            Ball("Painted rear body",new Vector3(0,.64f,-.23f),new Vector3(.34f,.28f,.74f),paint);
            Ball("Step through shield",new Vector3(0,.68f,.34f),new Vector3(.39f,.55f,.22f),paint);
            for(int side=-1;side<=1;side+=2)Beam("Front suspension",new Vector3(side*.075f,.33f,.64f),new Vector3(side*.075f,.98f,.44f),.035f,S.Metal);
            Beam("Handlebar",new Vector3(-.34f,1.05f,.42f),new Vector3(.34f,1.05f,.42f),.035f,S.Metal);
            for(int side=-1;side<=1;side+=2)
            {Beam("Rubber grip",new Vector3(side*.23f,1.05f,.42f),new Vector3(side*.37f,1.05f,.42f),.045f,S.Black);Beam("Mirror stem",new Vector3(side*.21f,1.05f,.44f),new Vector3(side*.28f,1.31f,.48f),.012f,S.Metal);Ball("Rear view mirror",new Vector3(side*.28f,1.32f,.48f),new Vector3(.13f,.09f,.035f),S.Black);}
            Ball("Headlamp",new Vector3(0,1.0f,.52f),new Vector3(.22f,.14f,.10f),S.Porcelain);
            Beam("Exhaust pipe",new Vector3(.18f,.28f,.08f),new Vector3(.18f,.36f,-.8f),.07f,S.Metal);
            Beam("Side stand",new Vector3(0,.38f,-.1f),new Vector3(-.25f,.02f,-.1f),.018f,S.Metal);
            Parent=old;
        }
        public static void Generator(Vector3 position)
        {
            var old=Parent;Parent=Group("Portable petrol generator",position);
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Beam("Steel roll cage upright",new Vector3(x*.42f,.05f,z*.31f),new Vector3(x*.42f,.75f,z*.31f),.035f,S.Black);
            for(int y=0;y<=1;y++)for(int s=-1;s<=1;s+=2){Beam("Generator frame rail",new Vector3(-.42f,.08f+y*.65f,s*.31f),new Vector3(.42f,.08f+y*.65f,s*.31f),.035f,S.Black);Beam("Frame cross rail",new Vector3(s*.42f,.08f+y*.65f,-.31f),new Vector3(s*.42f,.08f+y*.65f,.31f),.035f,S.Black);}
            Box("Fuel tank",new Vector3(0,.66f,0),new Vector3(.74f,.13f,.52f),S.Rust);
            Cylinder("Fuel cap",new Vector3(.16f,.735f,0),.045f,.025f,S.Black);
            Ball("Alternator casing",new Vector3(.18f,.34f,0),new Vector3(.34f,.37f,.43f),S.Metal);
            Box("Engine block",new Vector3(-.18f,.34f,0),new Vector3(.25f,.32f,.32f),S.Black);
            for(int j=0;j<7;j++)Box("Cooling fin",new Vector3(-.18f,.22f+j*.035f,-.03f),new Vector3(.3f,.012f,.34f),S.Metal);
            Box("Generator control panel",new Vector3(.16f,.46f,-.235f),new Vector3(.34f,.19f,.035f),S.Black);
            for(int i=0;i<2;i++)Ball("Power outlet",new Vector3(.08f+i*.14f,.46f,-.26f),new Vector3(.065f,.065f,.018f),S.Porcelain);
            Parent=old;
        }
    }
}
