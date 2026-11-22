using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    internal static class ForestTreeCollision
    {
        // Recover exact ground-level trunk rings from the existing generated, combined meshes.
        // Their UV signature is 11 vertices, U=0..2 and V=5/9. This avoids recooking entire canopies.
        public static int Add(Transform parent)
        {
            var village=GameObject.Find("05 VILLAGE - wooded hometown with scattered houses");
            if(!village)return 0;
            var roots=new GameObject("Distant forest trunk collisions").transform;roots.SetParent(parent);
            var placed=new HashSet<Vector2Int>();int count=0;
            foreach(var filter in village.GetComponentsInChildren<MeshFilter>())
            {
                if(!filter.name.Contains("Chunk")||filter.GetComponent<Collider>())continue;
                var mesh=filter.sharedMesh;if(!mesh||!mesh.isReadable)continue;
                var vertices=mesh.vertices;var uv=mesh.uv;
                for(int i=0;i+10<vertices.Length;i++)
                {
                    if(uv.Length!=vertices.Length || Mathf.Abs(uv[i].x)>.001f || Mathf.Abs(uv[i].y-5f/9f)>.001f || Mathf.Abs(uv[i+10].x-2)>.001f)continue;
                    bool ring=true;var center=Vector3.zero;
                    for(int j=0;j<10;j++)
                    {
                        if(Mathf.Abs(vertices[i+j].y-vertices[i].y)>.001f || Mathf.Abs(uv[i+j].x-j*.2f)>.002f){ring=false;break;}
                        center+=vertices[i+j];
                    }
                    if(!ring || (vertices[i]-vertices[i+10]).sqrMagnitude>.001f)continue;
                    center=filter.transform.TransformPoint(center*.1f);
                    if(Mathf.Abs(center.y-GroundDetailBuilder.HillHeight(center.x,center.z))>.15f)continue;
                    var key=new Vector2Int(Mathf.RoundToInt(center.x*10),Mathf.RoundToInt(center.z*10));
                    if(!placed.Add(key))continue;
                    float radius=Vector3.Distance(center,filter.transform.TransformPoint(vertices[i]));
                    var go=new GameObject("Forest trunk");go.transform.SetParent(roots);go.transform.position=center;
                    var c=go.AddComponent<CapsuleCollider>();c.radius=radius*.85f;c.height=12;c.center=Vector3.up*5;
                    count++;i+=10;
                }
            }
            return count;
        }
    }
}
