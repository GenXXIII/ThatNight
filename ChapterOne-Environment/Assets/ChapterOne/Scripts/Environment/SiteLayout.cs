using UnityEngine;

namespace ThatNight.ChapterOne
{
    // Shared by the builders and inspection cameras so site changes stay aligned.
    public static class SiteLayout
    {
        public const float HalfWidth=22, Front=-18, Back=46, HillElevation=21;
        public const float Width=HalfWidth*2, Depth=Back-Front;
        public const float RoadZ=Front-4.5f, SideLaneX=HalfWidth+4.4f;
        public const float HouseWidth=15, HouseDepth=12.5f, HouseScale=1.08f;
        public const float WeddingFront=-12,WeddingRear=10.5f,RoofFront=-14,WeddingEave=4.5f;
        public const float EntranceHalfWidth=3f;
        public const float EntranceSideFront=WeddingFront+1.5f;
        public const float CateringLeft=12,CateringRight=22,CateringBack=27.5f;
        public const float CanopyWidth=44, CanopyDepth=WeddingRear-RoofFront;
        public const float KitchenWidth=7.5f, KitchenDepth=8.2f;
        public const float CattleShelterWidth=3.6f, CattleShelterDepth=12;
        public static readonly Vector3 HousePosition=new Vector3(-2.5f,HillElevation,22);
        public static readonly Quaternion HouseRotation=Quaternion.Euler(0,-24,0);
        public static readonly Vector3 WeddingPosition=new Vector3(0,HillElevation,(RoofFront+WeddingRear)/2);
        public static bool InWeddingRoof(float x,float z,float margin=0)=>
            (Mathf.Abs(x)<=HalfWidth+margin&&z>=RoofFront-margin&&z<=WeddingRear+margin)||
            (x>=CateringLeft-margin&&x<=CateringRight+margin&&z>=WeddingRear-margin&&z<=CateringBack+margin);
        public static readonly Vector3 KitchenPosition=new Vector3(-2.5f,HillElevation,39);
        public static readonly Vector3 CattleShelterPosition=new Vector3(18.2f,HillElevation,38.5f);
        public static Vector3 HousePoint(Vector3 local)=>HousePosition+HouseRotation*(local*HouseScale);
        public static Vector3 HouseLocal(Vector3 world)=>Quaternion.Inverse(HouseRotation)*(world-HousePosition)/HouseScale;
    }
}
