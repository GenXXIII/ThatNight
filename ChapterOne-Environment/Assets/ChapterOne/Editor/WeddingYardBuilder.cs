using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class WeddingYardBuilder
    {
        public static Transform Build()
        {
            Parent=null;var root=Group("02 WEDDING - full marquee with seating dance and catering",Vector3.up*SiteLayout.HillElevation);Parent=root;
            WeddingRoofBuilder.Build();
            var old=Parent;Parent=Group("Left guest seating zone",Vector3.zero);
            for(int row=0;row<5;row++)for(int col=0;col<4;col++)
            {
                var c=new Vector3(-19+col*4.6f,0,-8+row*4);DiningTable(c);
                for(int chair=0;chair<6;chair++)
                {float a=chair*Mathf.PI/3+.12f;PropsBuilder.Chair(c+new Vector3(Mathf.Sin(a)*1.24f,0,Mathf.Cos(a)*1.24f),a*Mathf.Rad2Deg+180);}
            }
            Parent=old;
            WeddingEventBuilder.Build();
            WeddingPhotoCornerBuilder.Build();
            WeddingCateringBuilder.Build();
            Parent=null;return root;
        }
        public static void Swag(float width,float drop,Material mat)
        {
            Grid("Gathered hanging wedding fabric",48,12,(u,v)=>new Vector3(u*width,-Mathf.Sin(u*Mathf.PI)*drop-v*.32f,Mathf.Sin(u*Mathf.PI*22)*.026f+Mathf.Sin(v*Mathf.PI)*.10f),mat,new Vector2(3,1),true);
        }
        public static void Rosette(Vector3 p,float r)
        {
            for(int petal=0;petal<8;petal++)
            {float a=petal*Mathf.PI/4;var rose=Ball("Gathered fabric rosette",p+new Vector3(Mathf.Cos(a)*r*.45f,Mathf.Sin(a)*r*.45f,0),new Vector3(r*.75f,r*.9f,r*.5f),S.Pink);rose.transform.localRotation=Quaternion.Euler(0,0,-a*Mathf.Rad2Deg);}
        }
        static void DiningTable(Vector3 p)
        {
            Cylinder("Round banquet table",p+Vector3.up*.72f,.93f,.06f,S.Wood,36);
            Grid("Draped round tablecloth",64,14,(u,v)=>{float a=u*Mathf.PI*2,r=v<.65f?v/.65f*.97f:.97f+Mathf.Sin(a*24)*.023f*(v-.65f)/.35f;return p+new Vector3(Mathf.Sin(a)*r,.79f-Mathf.Max(0,v-.65f)/.35f*.65f,Mathf.Cos(a)*r);},S.Pink,new Vector2(4,2),true);
            Cylinder("Metal serving turntable",p+Vector3.up*.805f,.51f,.016f,S.Metal,36);
            PropsBuilder.Pot(p+new Vector3(0,.83f,0),.16f);
            for(int i=0;i<6;i++)
            {
                float a=i*Mathf.PI/3;var place=p+new Vector3(Mathf.Sin(a)*.73f,.8f,Mathf.Cos(a)*.73f);
                Lathe("Porcelain plate",place,new[]{new Vector2(0,0),new Vector2(.09f,0),new Vector2(.14f,.02f),new Vector2(.14f,.028f)},S.Porcelain);
                PropsBuilder.Cup(place+new Vector3(.08f,.035f,0));
                for(int j=0;j<2;j++)Beam("Chopstick",place+new Vector3(-.13f-j*.013f,.027f,-.09f),place+new Vector3(-.13f-j*.013f,.027f,.12f),.004f,S.Wood);
                Lathe("Drinking tumbler",place+new Vector3(.15f,.01f,-.09f),new[]{new Vector2(.027f,0),new Vector2(.033f,.11f),new Vector2(.029f,.112f),new Vector2(.025f,.02f)},S.Metal,16);
            }
        }
    }
}
