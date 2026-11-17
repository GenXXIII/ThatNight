using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Low household daybed for sitting under the raised house, based on the supplied references.
    internal static class UnderHouseSittingBuilder
    {
        public static void Build(Vector3 position)
        {
            var old=Parent;Parent=Group("Under house wooden sitting platform",position);
            const float width=3,depth=1.8f,top=.56f;
            for(int x=-1;x<=1;x++)for(int z=-1;z<=1;z+=2)
            {
                var leg=new Vector3(x*(width/2-.15f),0,z*(depth/2-.14f));
                Box("Daybed square timber leg",leg+Vector3.up*.26f,new Vector3(.13f,.52f,.13f),S.DarkWood,true);
                Box("Daybed leg shoulder",leg+Vector3.up*.44f,new Vector3(.18f,.13f,.18f),S.Wood);
            }
            for(int side=-1;side<=1;side+=2)
            {
                Box("Daybed long apron",new Vector3(0,.44f,side*(depth/2-.06f)),new Vector3(width,.18f,.12f),S.Wood);
                Box("Daybed end apron",new Vector3(side*(width/2-.06f),.44f,0),new Vector3(.12f,.18f,depth),S.Wood);
                Box("Daybed lower stretcher",new Vector3(0,.18f,side*(depth/2-.14f)),new Vector3(width-.3f,.07f,.07f),S.DarkWood);
            }
            for(int i=0;i<18;i++)
                Box("Sitting platform timber slat",new Vector3(0,top-.035f,-depth/2+(i+.5f)*depth/18),new Vector3(width,.07f,depth/18-.009f),i%5==0?S.DarkWood:S.Wood,true);
            var reed=S.Plain("Faded reed sitting mat",new Color(.52f,.39f,.21f),.08f);
            var reedLight=S.Plain("Light reed mat weave",new Color(.64f,.52f,.32f),.06f);
            var edging=S.Plain("Worn red mat binding",new Color(.28f,.10f,.07f),.08f);
            Box("Woven sitting mat",new Vector3(-.18f,top+.009f,0),new Vector3(2.35f,.018f,1.45f),reed);
            for(int i=0;i<64;i++)
            {
                float x=-1.34f+i*2.32f/63;
                Box("Fine reed mat stripe",new Vector3(x,top+.020f,0),new Vector3(.012f,.004f,1.4f),i%2==0?reedLight:reed);
            }
            for(int s=-1;s<=1;s+=2)
            {
                Box("Mat edge binding",new Vector3(-.18f,top+.023f,s*.715f),new Vector3(2.35f,.008f,.025f),edging);
                Box("Mat end binding",new Vector3(-.18f+s*1.165f,top+.023f,0),new Vector3(.025f,.008f,1.45f),edging);
            }
            Ball("Faded cotton resting cushion",new Vector3(.87f,top+.11f,.46f),new Vector3(.65f,.2f,.38f),S.Cream);
            Parent=old;
        }
    }
}
