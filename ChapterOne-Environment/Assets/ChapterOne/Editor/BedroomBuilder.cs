using UnityEngine;
using UnityEngine.Rendering;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Household furnishings follow the user's wooden-house references and circa-2006 brief.
    internal static class BedroomBuilder
    {
        public static void Build(string name,float centerX,float back,bool bridal)
        {
            var old=Parent;Parent=Group(name,new Vector3(centerX,2.5f,back-1.7f));
            var bedding=S.Pbr(bridal?"Rose bedroom cotton":"Indigo bedroom cotton","Fabric035",
                bridal?new Color(.65f,.30f,.29f):new Color(.22f,.31f,.39f),.06f);
            bedding.SetFloat("_Desaturate",1);
            Bed(bedding,bridal);
            Wardrobe(new Vector3(2.2f,0,.85f));
            PropsBuilder.Table(new Vector3(2.1f,0,-.8f),new Vector2(1.15f,.55f),.70f,S.Wood);
            Box("Dressing mirror timber frame",new Vector3(2.1f,1.15f,-.59f),new Vector3(.65f,.83f,.075f),S.DarkWood);
            Box("Aged dressing mirror glass",new Vector3(2.1f,1.15f,-.635f),new Vector3(.55f,.72f,.012f),Mirror());
            var comb=Box("Small dressing comb",new Vector3(1.82f,.75f,-.88f),new Vector3(.18f,.018f,.04f),S.DarkWood);
            for(int i=0;i<12;i++)Box("Comb tooth",new Vector3(1.74f+i*.014f,.75f,-.915f),new Vector3(.006f,.016f,.05f),S.DarkWood);
            Cylinder("Small toiletry tin",new Vector3(2.4f,.745f,-.83f),.065f,.07f,S.Metal,20);
            PropsBuilder.Table(new Vector3(-2.2f,0,.55f),new Vector2(.55f,.55f),.49f,S.DarkWood);
            Lathe("Enamel bedside water pitcher",new Vector3(-2.2f,.53f,.55f),new[]{new Vector2(.07f,0),new Vector2(.10f,.03f),new Vector2(.09f,.24f),new Vector2(.11f,.27f),new Vector2(.095f,.275f),new Vector2(.07f,.06f)},S.Porcelain,24);
            Tube("Pitcher handle",new Vector3(-2.31f,.69f,.55f),.085f,.012f,S.Metal,Quaternion.identity,20);
            PropsBuilder.Cup(new Vector3(-2.04f,.53f,.40f));
            ReedMat(new Vector3(-.9f,.015f,-1.9f));
            Storage(new Vector3(-2.5f,0,-2.65f),bridal);
            BedroomFanBuilder.Build(new Vector3(-2.85f,0,-.95f));
            PropsBuilder.Basket(new Vector3(2.8f,.01f,-2.25f),.7f);
            LightingBuilder.Bulb(new Vector3(.2f,2.2f,-1),5,1.6f,false);
            Parent=old;
        }
        static Material Mirror()=>S.Plain("Aged bedroom mirror",new Color(.34f,.40f,.38f),.72f,.8f);
        static void Bed(Material bedding,bool bridal)
        {
            var old=Parent;Parent=Group("Wooden bed with thin cotton bedding",new Vector3(-.9f,0,0));
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)
                Box("Bed turned square foot",new Vector3(x*.78f,.24f,z*1.02f),new Vector3(.10f,.48f,.10f),S.DarkWood);
            Box("Bed timber side frame",new Vector3(0,.40f,0),new Vector3(1.75f,.19f,2.2f),S.Wood);
            Box("Thin cotton mattress",new Vector3(0,.535f,0),new Vector3(1.66f,.12f,2.12f),S.Cream);
            Box("Faded cotton bed cover",new Vector3(0,.605f,0),new Vector3(1.67f,.025f,2.12f),bedding);
            Box("Timber bed headboard",new Vector3(0,.82f,1.12f),new Vector3(1.8f,.64f,.085f),S.DarkWood);
            for(int i=0;i<7;i++)Box("Headboard inset wood panel",new Vector3((i-3)*.23f,.82f,1.07f),new Vector3(.18f,.43f,.025f),S.Wood);
            for(int i=0;i<(bridal?2:1);i++)
                Ball("Cotton sleeping pillow",new Vector3(bridal?(i-.5f)*.74f:0,.72f,.73f),new Vector3(bridal?.67f:1.0f,.19f,.40f),S.Cream);
            if(bridal)for(int row=0;row<5;row++)for(int col=0;col<5;col++)
            {
                var p=new Vector3((col-2)*.31f,.626f,-.78f+row*.28f);
                for(int petal=0;petal<5;petal++)
                {
                    float a=petal*Mathf.PI*2/5;
                    Ball("Woven floral bedspread motif",p+new Vector3(Mathf.Sin(a)*.035f,0,Mathf.Cos(a)*.035f),new Vector3(.05f,.005f,.05f),S.Cream);
                }
            }
            else for(int i=0;i<12;i++)Box("Striped family bedcover",new Vector3(0,.625f,-.90f+i*.15f),new Vector3(1.62f,.004f,.015f),S.Cream);
            Box("Folded cotton blanket",new Vector3(0,.66f,-.76f),new Vector3(1.5f,.075f,.45f),bedding);
            MosquitoNet();Parent=old;
        }
        static void MosquitoNet()
        {
            var net=S.Plain("Sheer bedroom mosquito gauze",new Color(.92f,.88f,.78f,.22f),.03f);
            net.shader=Shader.Find("Standard");net.SetFloat("_Mode",2);
            net.SetInt("_SrcBlend",(int)BlendMode.SrcAlpha);net.SetInt("_DstBlend",(int)BlendMode.OneMinusSrcAlpha);
            net.SetInt("_ZWrite",0);net.EnableKeyword("_ALPHABLEND_ON");net.renderQueue=3000;
            for(int side=-1;side<=1;side+=2)
            {
                int s=side;
                Beam("Mosquito net suspension pole",new Vector3(s*.87f,.42f,1.08f),new Vector3(s*.87f,2.13f,1.08f),.025f,S.DarkWood);
                Grid("Sheer side mosquito net",36,24,(u,v)=>new Vector3(s*(.86f+Mathf.Sin(v*Mathf.PI)*.06f),.65f+v*1.45f,-.88f+u*1.98f),net,new Vector2(4,3),true);
                Grid("Gathered open net curtain",20,24,(u,v)=>new Vector3(s*.85f+(u-.5f)*(.13f+.24f*Mathf.Abs(v-.5f)),.62f+v*1.49f,-.99f+Mathf.Sin(u*Mathf.PI*10)*.035f),S.Cream,Vector2.one,true);
                Beam("Net tie cord",new Vector3(s*.85f-.12f,1.25f,-1.02f),new Vector3(s*.85f+.12f,1.25f,-1.02f),.012f,S.Pink);
            }
            Grid("Draped mosquito net canopy",24,24,(u,v)=>new Vector3((u-.5f)*1.8f,2.12f-Mathf.Sin(u*Mathf.PI)*.13f,(v-.5f)*2.25f),net,new Vector2(4,4),true);
        }
        static void Wardrobe(Vector3 p)
        {
            var old=Parent;Parent=Group("Footed wooden bedroom wardrobe",p);
            Box("Wardrobe body",new Vector3(0,1.08f,0),new Vector3(1.4f,1.95f,.59f),S.DarkWood);
            Box("Wardrobe crown",new Vector3(0,2.09f,0),new Vector3(1.52f,.09f,.68f),S.Wood);
            for(int s=-1;s<=1;s+=2)
            {
                Box("Wardrobe panel door",new Vector3(s*.34f,1.12f,-.31f),new Vector3(.64f,1.78f,.045f),S.Wood);
                Box("Wardrobe inset panel",new Vector3(s*.34f,1.12f,-.34f),new Vector3(.48f,1.51f,.022f),s<0?Mirror():S.DarkWood);
                Tube("Wardrobe brass handle",new Vector3(s*.10f,1.1f,-.38f),.035f,.007f,S.Brass,Quaternion.identity,16);
                Box("Wardrobe foot",new Vector3(s*.55f,.07f,0),new Vector3(.12f,.14f,.48f),S.Wood);
            }
            Parent=old;
        }
        static void ReedMat(Vector3 p)
        {
            var reed=S.Plain("Bedroom woven reed mat",new Color(.49f,.36f,.18f),.05f);
            Box("Bedside woven floor mat",p,new Vector3(2.4f,.02f,1.25f),reed);
            for(int i=0;i<45;i++)Box("Reed mat weave",p+new Vector3((i-22)*.052f,.012f,0),new Vector3(.013f,.004f,1.2f),S.Cream);
            for(int s=-1;s<=1;s+=2)Box("Reed mat red edge",p+new Vector3(0,.015f,s*.60f),new Vector3(2.4f,.006f,.035f),S.Pink);
        }
        static void Storage(Vector3 p,bool bridal)
        {
            var paint=S.Plain(bridal?"Old red travel case":"Family dark storage chest",bridal?new Color(.30f,.07f,.045f):new Color(.12f,.19f,.18f),.15f);
            Box(bridal?"Bridal travel case":"Household storage trunk",p+Vector3.up*.22f,new Vector3(1.10f,.44f,.55f),paint);
            Box("Storage lid rim",p+Vector3.up*.40f,new Vector3(1.12f,.035f,.57f),S.Metal);
            for(int s=-1;s<=1;s+=2)Box("Storage metal latch",p+new Vector3(s*.35f,.32f,-.29f),new Vector3(.045f,.11f,.022f),S.Metal);
            Beam("Case carry handle",p+new Vector3(-.10f,.22f,-.31f),p+new Vector3(.10f,.22f,-.31f),.025f,S.Black);
            Box("Folded checked household cloth",p+Vector3.up*.47f,new Vector3(.64f,.055f,.37f),S.Cream);
            for(int i=0;i<9;i++)Box("Krama lengthwise check",p+new Vector3((i-4)*.065f,.50f,0),new Vector3(.018f,.004f,.36f),S.Pink);
            for(int i=0;i<5;i++)Box("Krama crosswise check",p+new Vector3(0,.503f,(i-2)*.065f),new Vector3(.63f,.004f,.018f),S.Pink);
        }
    }
}
