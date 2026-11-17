using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class HouseWeddingBuilder
    {
        public static void Build()
        {
            var old=Parent;Parent=Group("Upstairs household wedding decorations",Vector3.up*2.5f);
            var gold=S.Pbr("Golden ceremony cloth","Fabric035",new Color(.78f,.59f,.24f),.18f);
            var red=S.Pbr("Red ceremony floor mat","Fabric035",new Color(.43f,.055f,.035f),.07f);
            LivingRoomFloorBuilder.Build(red,gold);
            ProcessionFruit(red,gold);
            var weddingRoot=Parent;Parent=Group("Right bride and groom wrist tying ceremony",Vector3.zero);
            // Everything rests on the floor mat; the open front faces the fruit to the left.
            WeddingOfferingBuilder.WristThreads(new Vector3(3.3f,.04f,-2.7f));
            WeddingOfferingBuilder.WristThreads(new Vector3(3.3f,.04f,-1.3f));
            WeddingOfferingBuilder.Candle(new Vector3(2.4f,.04f,-3.2f));
            WeddingOfferingBuilder.Candle(new Vector3(2.4f,.04f,-.8f));
            WeddingOfferingBuilder.Flowers(new Vector3(5.65f,.04f,-3.1f));
            WeddingOfferingBuilder.Flowers(new Vector3(5.65f,.04f,-.7f));
            for(int i=0;i<2;i++)
            {
                var cushion=Group(i==0?"Bride ceremony seat":"Groom ceremony seat",new Vector3(4.5f,.04f,-2.7f+i*1.4f));
                cushion.localRotation=Quaternion.Euler(0,-90,0);
                var ceremony=Parent;Parent=cushion;
                Box("Gold floor cushion base",new Vector3(0,.065f,0),new Vector3(.82f,.13f,.68f),gold);
                Ball("Soft ceremony cushion top",new Vector3(0,.13f,0),new Vector3(.87f,.18f,.72f),gold);
                Box("Cushion rear embroidered edging",new Vector3(0,.145f,-.30f),new Vector3(.78f,.025f,.045f),red);
                Parent=ceremony;
            }
            Parent=weddingRoot;
            // Overhead fabric stays above circulation; no curtains obstruct bedroom doors.
            foreach(float z in new[]{-5.25f,-.25f})for(int span=0;span<4;span++)
            {
                var save=Parent;Parent=Group("Living room wedding drape",new Vector3(-6.8f+span*3.4f,2.35f,z));
                WeddingYardBuilder.Swag(3.4f,.24f,S.Cream);
                Parent=Group("Pink ceremony fabric trim",new Vector3(0,-.09f,-.035f));
                WeddingYardBuilder.Swag(3.4f,.24f,S.Pink);Parent=save;
            }
            LightingBuilder.Bulb(new Vector3(3.8f,2.12f,-1.5f),4,1.6f,false);
            Parent=old;
        }
        static void ProcessionFruit(Material red,Material gold)
        {
            var old=Parent;Parent=Group("Left processional fruit trays beside seating",new Vector3(-3.15f,.015f,-2.3f));
            for(int i=0;i<5;i++)
            {
                float x=i==4?0:(i%2==0?-.60f:.60f),z=-.9f+(i/2)*.9f;
                WeddingOfferingBuilder.FruitTray(new Vector3(x,.025f,z),i,i%2!=0);
            }
            Parent=old;
        }
    }
}
