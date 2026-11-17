using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class WeddingEventBuilder
    {
        public static void Build()
        {
            var old=Parent;Parent=Group("Right dancing and traditional event zone",Vector3.zero);
            // Wide unobstructed temporary timber dance floor, separated from the aisle.
            for(int i=0;i<40;i++)
                Box("Dance floor plank",new Vector3(12,.035f,-7.5f+i*.265f),new Vector3(13,.07f,.258f),S.Floor);
            Box("Low ceremony platform",new Vector3(12,.16f,7.1f),new Vector3(12,.32f,3.8f),S.DarkWood);
            Box("Ceremony step",new Vector3(12,.08f,4.9f),new Vector3(4,.16f,.6f),S.Wood);
            Grid("Ceremonial floor cloth",8,8,(u,v)=>new Vector3(8+u*8,.325f,5.4f+v*3.3f),S.Pink,new Vector2(4,2),true);
            PropsBuilder.Table(new Vector3(12,.33f,7.8f),new Vector2(2.6f,.9f),.45f,S.Cream);
            for(int i=0;i<3;i++)
            {
                var p=new Vector3(11.25f+i*.75f,.82f,7.8f);
                Lathe("Ceremonial brass offering tray",p,new[]{new Vector2(.1f,0),new Vector2(.08f,.12f),new Vector2(.28f,.2f),new Vector2(.3f,.23f)},S.Brass);
                for(int j=0;j<5;j++){float a=j*Mathf.PI*2/5;Ball("Fruit offering",p+new Vector3(Mathf.Cos(a)*.14f,.29f,Mathf.Sin(a)*.14f),Vector3.one*.14f,S.Clay);}
            }
            for(int i=0;i<5;i++)PropsBuilder.Chair(new Vector3(20,0,-5+i*1.6f),90);
            Parent=old;
        }
    }
}
