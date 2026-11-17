using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;

namespace ThatNight.ChapterOne.Editor
{
    internal static class LivingRoomFloorBuilder
    {
        public static void Build(Material cloth,Material trim)
        {
            var old=Parent;Parent=Group("Living room wedding floor covering with seating cutout",Vector3.zero);
            float left=-SiteLayout.HouseWidth/2+.08f,right=SiteLayout.HouseWidth/2-.08f;
            float front=-SiteLayout.HouseDepth/2+.08f,back=.92f;
            const float cutRight=-4.2f,cutFront=-4.5f,cutBack=-1.7f;
            Panel(cutRight,right,front,back,cloth);
            Panel(left,cutRight,front,cutFront,cloth);
            Panel(left,cutRight,cutBack,back,cloth);
            // A continuous gold border follows the exposed timber seating bay as well.
            Vector2[] outline={new Vector2(left,front),new Vector2(right,front),new Vector2(right,back),
                new Vector2(left,back),new Vector2(left,cutBack),new Vector2(cutRight,cutBack),
                new Vector2(cutRight,cutFront),new Vector2(left,cutFront)};
            for(int i=0;i<outline.Length;i++)
            {
                Vector2 a=outline[i],b=outline[(i+1)%outline.Length],mid=(a+b)*.5f;
                Box("Continuous wedding carpet gold border",new Vector3(mid.x,.029f,mid.y),
                    new Vector3(Mathf.Max(.05f,Mathf.Abs(b.x-a.x)),.007f,Mathf.Max(.05f,Mathf.Abs(b.y-a.y))),trim);
            }
            Parent=old;
        }
        static void Panel(float left,float right,float front,float back,Material cloth)
        {
            var go=Grid("Living room woven floor covering",1,1,(u,v)=>new Vector3(Mathf.Lerp(left,right,u),.025f,Mathf.Lerp(front,back,v)),cloth,Vector2.one);
            var mesh=go.GetComponent<MeshFilter>().sharedMesh;var vertices=mesh.vertices;var uv=new Vector2[vertices.Length];
            for(int i=0;i<vertices.Length;i++)uv[i]=new Vector2(vertices[i].x,vertices[i].z)*.6f;
            mesh.uv=uv;mesh.RecalculateTangents();
        }
    }
}
