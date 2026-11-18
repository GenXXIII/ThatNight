using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class BedroomFanBuilder
    {
        public static void Build(Vector3 p)
        {
            var old=Parent;Parent=ModelGroup("Older pedestal bedroom fan",p); var fanGroup=Parent;
            var green=S.Plain("Faded green fan enamel",new Color(.29f,.39f,.23f),.24f);
            Ball("Heavy fan base",new Vector3(0,.07f,0),new Vector3(.48f,.14f,.39f),green);
            Cylinder("Fan pedestal",new Vector3(0,.10f,0),.045f,.85f,green);
            var hub=new Vector3(0,1.17f,-.03f);
            Ball("Fan motor housing",hub+Vector3.forward*.12f,new Vector3(.18f,.18f,.26f),green);
            for(int i=0;i<3;i++)
            {
                float a=i*Mathf.PI*2/3;
                var blade=Ball("Three blade fan rotor",hub+new Vector3(Mathf.Sin(a)*.13f,Mathf.Cos(a)*.13f,0),new Vector3(.13f,.30f,.025f),green);
                blade.transform.localRotation=Quaternion.Euler(0,0,-a*Mathf.Rad2Deg);
            }
            for(int side=-1;side<=1;side+=2)
            {
                var centre=hub+Vector3.forward*(side*.065f);
                for(int ring=1;ring<=5;ring++)Tube("Fan wire guard ring",centre,.06f*ring,.0035f,S.Metal,Quaternion.identity,48);
                for(int i=0;i<24;i++)
                {
                    float a=i*Mathf.PI/12;
                    Beam("Fan guard spoke",centre,centre+new Vector3(Mathf.Sin(a)*.30f,Mathf.Cos(a)*.30f,0),.0035f,S.Metal);
                }
            }
            Ball("Fan central cap",hub-Vector3.forward*.085f,new Vector3(.11f,.11f,.035f),S.Cream);
            Cable("Fan electrical cord",new Vector3(0,.1f,.1f),new Vector3(-.48f,.025f,.32f),.025f,.008f,S.Black);
            Parent=old; ModelCombiner.CombineModel(fanGroup, addCollider: true, useCapsuleCollider: true);
        }
    }
}
