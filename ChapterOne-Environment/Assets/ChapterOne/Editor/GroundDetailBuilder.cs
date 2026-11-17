using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S=ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class GroundDetailBuilder
    {
        static readonly Vector2[] Path={new Vector2(6,-18),new Vector2(0,-18),new Vector2(0,-5),new Vector2(2,6),new Vector2(2,11),new Vector2(12,16),new Vector2(12,32),new Vector2(-2.5f,34.7f)};
        // The approach stays on the property's far-right forest edge. The wedding
        // remains hidden until the final bend at its front-right corner.
        static readonly Vector2[] HillRoad={new Vector2(48,-111.5f),new Vector2(52,-99),new Vector2(50.5f,-86.5f),new Vector2(47,-74),new Vector2(44,-63),new Vector2(39,-54),new Vector2(34,-46),new Vector2(29,-39),new Vector2(24,-33),new Vector2(20,-28),new Vector2(15,-23),new Vector2(11,-20),new Vector2(6,-18)};
        static float SegmentDistance(Vector2 p,Vector2 a,Vector2 b)
        {var d=b-a;return Vector2.Distance(p,a+d*Mathf.Clamp01(Vector2.Dot(p-a,d)/d.sqrMagnitude));}
        static float PathWear(Vector3 p)
        {
            var q=new Vector2(p.x,p.z);float distance=100;
            for(int i=1;i<Path.Length;i++)distance=Mathf.Min(distance,SegmentDistance(q,Path[i-1],Path[i]));
            distance=Mathf.Min(distance,SegmentDistance(q,new Vector2(12,32),new Vector2(12,SiteLayout.CattleShelterPosition.z)));
            distance=Mathf.Min(distance,SegmentDistance(q,new Vector2(12,SiteLayout.CattleShelterPosition.z),new Vector2(SiteLayout.CattleShelterPosition.x-SiteLayout.CattleShelterWidth/2,SiteLayout.CattleShelterPosition.z)));
            return 1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.55f,1.65f,distance));
        }
        public static float HillHeight(float x,float z)
        {
            // Keep the forest land level with the wedding. Only the approach-road
            // corridor descends, so the site no longer reads as a round isolated mound.
            var p=new Vector2(x,z);float distance,progress=ClosestRoadProgress(p,out distance);
            float flat=SiteLayout.HillElevation-.12f;
            float climb=Mathf.SmoothStep(0,1,Mathf.InverseLerp(.42f,1,progress));
            float roadHeight=Mathf.Lerp(3.55f,flat,climb);
            // A broad low verge keeps the small earth banks away from the wheel path.
            float roadInfluence=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(8f,30f,distance));
            float natural=(Mathf.PerlinNoise(x*.045f+20,z*.045f+40)-.5f)*.16f;
            return Mathf.Lerp(flat,roadHeight,roadInfluence)+natural*(1-roadInfluence*.8f);
        }
        public static float Height(float x,float z)=>HillHeight(x,z)+.025f+Mathf.PerlinNoise(x*.43f+70,z*.43f+40)*.018f;
        public static float DistanceToHillRoad(Vector2 p)
        {
            float distance;ClosestRoadProgress(p,out distance);return distance;
        }
        static float ClosestRoadProgress(Vector2 p,out float distance)
        {
            // Sample the same Catmull-Rom centerline used by the visible road mesh.
            // This prevents the road from clipping into the bank at curved sections.
            const int samples=120;distance=1000;float progress=0;Vector2 a=RoadCenter(0);
            for(int i=1;i<=samples;i++)
            {
                Vector2 b=RoadCenter(i/(float)samples),segment=b-a;
                float along=Mathf.Clamp01(Vector2.Dot(p-a,segment)/segment.sqrMagnitude);
                float candidate=Vector2.Distance(p,a+segment*along);
                if(candidate<distance){distance=candidate;progress=(i-1+along)/samples;}
                a=b;
            }
            return progress;
        }
        static Vector2 RoadCenter(float t)
        {
            float scaled=Mathf.Clamp01(t)*(HillRoad.Length-1);int i=Mathf.Min(HillRoad.Length-2,Mathf.FloorToInt(scaled));float f=scaled-i;
            Vector2 p0=HillRoad[Mathf.Max(0,i-1)],p1=HillRoad[i],p2=HillRoad[i+1],p3=HillRoad[Mathf.Min(HillRoad.Length-1,i+2)];
            return .5f*((2*p1)+(-p0+p2)*f+(2*p0-5*p1+4*p2-p3)*f*f+(-p0+3*p1-3*p2+p3)*f*f*f);
        }
        public static Vector2 HillRoadCenter(float progress)=>RoadCenter(progress);
        public static Vector3 HillRoadPoint(float u,float v)
        {
            Vector2 center=RoadCenter(v),before=RoadCenter(Mathf.Max(0,v-.01f)),after=RoadCenter(Mathf.Min(1,v+.01f));
            Vector2 tangent=(after-before).normalized,side=new Vector2(tangent.y,-tangent.x);float offset=(u-.5f)*4.6f;
            Vector2 p=center+side*offset;float crown=(1-Mathf.Abs(u-.5f)*2)*.10f;
            return new Vector3(p.x,HillHeight(p.x,p.y)+.16f+crown,p.y);
        }
        public static Color HillRoadShade(Vector3 p)
        {
            float distance=DistanceToHillRoad(new Vector2(p.x,p.z));
            float rut=Mathf.Exp(-Mathf.Pow((distance-.88f)/.22f,2));
            float verge=Mathf.SmoothStep(0,1,Mathf.InverseLerp(1.7f,2.3f,distance));
            return new Color(1,verge*.65f,rut*.32f,.4f+rut*.5f);
        }
        public static Color Yard(Vector3 p)
        {
            Vector3 local=SiteLayout.HouseLocal(p);float hw=SiteLayout.HouseWidth/2,hd=SiteLayout.HouseDepth/2;
            float house=(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(hw-.1f,hw+1,Mathf.Abs(local.x))))*(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(hd-.1f,hd+.8f,Mathf.Abs(local.z))));
            float shade=1-house*.42f;
            for(int x=-1;x<=1;x++)for(int z=-1;z<=1;z++)shade-=Mathf.Exp(-new Vector2(local.x-x*(hw-.5f),local.z-z*(hd-.55f)).sqrMagnitude/.3f)*.18f;
            float edge=Mathf.Min(SiteLayout.HalfWidth-Mathf.Abs(p.x),Mathf.Min(p.z-SiteLayout.Front,SiteLayout.Back-p.z));
            float grass=(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.7f,4.8f,edge)))*.85f;
            float patch=Mathf.PerlinNoise(p.x*.14f+40,p.z*.14f+20);
            grass=Mathf.Max(grass,Mathf.SmoothStep(0,1,Mathf.InverseLerp(.54f,.76f,patch))*.48f);
            float wear=PathWear(p);grass*=1-wear;
            if(SiteLayout.InWeddingRoof(p.x,p.z))grass=0;
            Vector3 wedding=p-SiteLayout.WeddingPosition,kitchen=p-SiteLayout.KitchenPosition,cattle=p-SiteLayout.CattleShelterPosition;
            if(house>.4f||Mathf.Abs(wedding.x)<SiteLayout.CanopyWidth/2+.3f&&Mathf.Abs(wedding.z)<SiteLayout.CanopyDepth/2+.3f||Mathf.Abs(kitchen.x)<SiteLayout.KitchenWidth/2+.2f&&Mathf.Abs(kitchen.z)<SiteLayout.KitchenDepth/2+.2f||Mathf.Abs(cattle.x)<SiteLayout.CattleShelterWidth/2+.3f&&Mathf.Abs(cattle.z)<SiteLayout.CattleShelterDepth/2+.3f)grass=0;
            float damp=Mathf.Exp(-new Vector2(kitchen.x+2.5f,kitchen.z+2.8f).sqrMagnitude/2.8f)*.62f;
            return new Color(Mathf.Clamp(shade,.35f,1),grass,damp,wear);
        }
        public static Color Forest(Vector3 p)
        {
            float road=p.z<SiteLayout.Front+2?1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(2.2f,3.7f,DistanceToHillRoad(new Vector2(p.x,p.z)))):0;
            float grass=(.68f+Mathf.PerlinNoise(p.x*.19f+20,p.z*.19f+30)*.28f)*(1-road);
            return new Color(.9f,grass,.08f+road*.16f,road*.65f);
        }
        public static Color Road(Vector3 p)
        {
            float cross=Mathf.Abs(p.z-SiteLayout.RoadZ)<3?p.z-SiteLayout.RoadZ:p.x-SiteLayout.SideLaneX;
            float rut=Mathf.Exp(-Mathf.Pow((Mathf.Abs(cross)-.82f)/.22f,2));
            float verge=Mathf.SmoothStep(0,1,Mathf.InverseLerp(1.45f,2.5f,Mathf.Abs(cross)));
            return new Color(1,verge*.75f,rut*.28f,.3f+rut*.5f);
        }
        public static void Dressing()
        {
            var randomState=Random.state;Random.InitState(200607);
            var leaf=S.Plain("Fallen dry mango leaves",new Color(.25f,.18f,.075f),.09f);
            for(int i=0;i<1100;i++)
            {
                var p=new Vector3(Random.Range(-21.5f,21.5f),0,Random.Range(SiteLayout.Front+.5f,SiteLayout.Back-.5f));var sample=Yard(p);
                if(sample.g<.35f||sample.a>.2f)continue;p.y=Height(p.x,p.z);
                for(int blade=0;blade<5;blade++)
                {
                    float a=i*2.4f+blade*1.26f,height=Random.Range(.12f,.32f);var d=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));var side=new Vector3(d.z,0,-d.x);
                    Grid("Uneven yard edge grass",1,3,(u,v)=>p+Vector3.up*v*height+d*v*v*.13f+side*(u-.5f)*.024f*(1-v),S.Leaf,Vector2.one,true);
                }
                if(i%3==0)
                {
                    var d=Quaternion.Euler(0,i*137,0);Grid("Fallen mango leaf",4,2,(u,v)=>p+Vector3.up*.012f+d*new Vector3((u-.5f)*.18f,Mathf.Sin(u*Mathf.PI)*.012f,(v-.5f)*.065f*Mathf.Sin(u*Mathf.PI)),leaf,Vector2.one,true);
                }
            }
            Random.state=randomState;
        }
    }
}
