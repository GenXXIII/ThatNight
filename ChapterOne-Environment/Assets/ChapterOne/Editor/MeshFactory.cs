using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object=UnityEngine.Object;

namespace ThatNight.ChapterOne.Editor
{
    public static class MeshFactory
    {
        public static Transform Parent;
        public static Transform Group(string name,Vector3 p)
        {var g=new GameObject(name);g.transform.SetParent(Parent,false);g.transform.localPosition=p;return g.transform;}
        public static Transform ModelGroup(string name,Vector3 p)
        {
            var g=Group(name,p);
            var root=g.gameObject.AddComponent<ThatNight.ChapterOne.ModelRoot>();
            root.modelName=name;
            return g;
        }
        public static GameObject MeshObject(string name,Mesh mesh,Material mat)
        {
            var g=Group(name,Vector3.zero).gameObject;g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=mat;g.isStatic=true;return g;
        }
        public static GameObject Grid(string name,int cols,int rows,Func<float,float,Vector3> point,Material mat,Vector2 tiling,bool twoSided=false,Func<Vector3,Color> shade=null)
        {
            var v=new List<Vector3>();var uv=new List<Vector2>();var colors=new List<Color>();var t=new List<int>();
            for(int y=0;y<=rows;y++)for(int x=0;x<=cols;x++)
            {float u=x/(float)cols,w=y/(float)rows;Vector3 p=point(u,w);v.Add(p);uv.Add(Vector2.Scale(new Vector2(u,w),tiling));colors.Add(shade==null?Color.white:shade(p));}
            for(int y=0;y<rows;y++)for(int x=0;x<cols;x++){int a=y*(cols+1)+x,b=a+cols+1;t.AddRange(new[]{a,b,a+1,a+1,b,b+1});}
            if(twoSided){int n=v.Count;v.AddRange(v.ToArray());uv.AddRange(uv.ToArray());colors.AddRange(colors.ToArray());var front=t.ToArray();for(int i=0;i<front.Length;i+=3)t.AddRange(new[]{n+front[i],n+front[i+2],n+front[i+1]});}
            var m=new Mesh{name=name,indexFormat=IndexFormat.UInt32};m.SetVertices(v);m.SetUVs(0,uv);m.SetColors(colors);m.SetTriangles(t,0);m.RecalculateNormals();m.RecalculateTangents();m.RecalculateBounds();return MeshObject(name,m,mat);
        }
        public static GameObject Box(string name,Vector3 p,Vector3 size,Material mat,bool collider=false,float grainScale=1)
        {
            var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var colors=new List<Color>();var tris=new List<int>();
            Vector3[] ns={Vector3.forward,Vector3.back,Vector3.right,Vector3.left,Vector3.up,Vector3.down};
            foreach(var n in ns)
            {
                var up=Mathf.Abs(n.y)>.5f?Vector3.forward:Vector3.up;var right=Vector3.Cross(up,n);
                Vector3 center=Vector3.Scale(n,size)*.5f,u=Vector3.Scale(right,size)*.5f,w=Vector3.Scale(up,size)*.5f;
                int first=vertices.Count;
                for(int j=0;j<4;j++)
                {
                    float a=j==0||j==3?-1:1,b=j<2?-1:1;var local=center+u*a+w*b;vertices.Add(local);normals.Add(n);
                    bool timber=mat==SurfaceLibrary.Wood||mat==SurfaceLibrary.DarkWood;
                    uv.Add(timber?new Vector2((b+1)*w.magnitude/(grainScale*2),(a+1)*u.magnitude/(grainScale*2)+Mathf.Repeat(p.x*.371f+p.z*.713f,1)):new Vector2((a+1)*u.magnitude/grainScale,(b+1)*w.magnitude/grainScale));
                    float ao=Mathf.Lerp(.82f,1,Mathf.InverseLerp(-size.y/2,size.y/2,local.y));colors.Add(new Color(1,1,1,ao));
                }
                tris.AddRange(new[]{first,first+2,first+1,first,first+3,first+2});
            }
            // Above corners are clockwise when seen from inside; reverse to face out.
            for(int i=0;i<tris.Count;i+=3){int tmp=tris[i+1];tris[i+1]=tris[i+2];tris[i+2]=tmp;}
            var mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetColors(colors);mesh.SetTriangles(tris,0);mesh.RecalculateTangents();mesh.RecalculateBounds();
            var go=MeshObject(name,mesh,mat);go.transform.localPosition=p;if(collider)go.AddComponent<BoxCollider>().size=size;return go;
        }
        public static GameObject Ball(string name,Vector3 p,Vector3 size,Material mat)
        {
            var go=Grid(name,12,8,(u,v)=>Vector3.Scale(new Vector3(Mathf.Sin(v*Mathf.PI)*Mathf.Cos(-u*Mathf.PI*2),Mathf.Cos(v*Mathf.PI),Mathf.Sin(v*Mathf.PI)*Mathf.Sin(-u*Mathf.PI*2)),size*.5f),mat,Vector2.one);
            go.transform.localPosition=p;return go;
        }
        public static GameObject Lathe(string name,Vector3 p,Vector2[] profile,Material mat,int segments=24)
        {
            return Grid(name,segments,profile.Length-1,(u,v)=>{var ring=profile[Mathf.RoundToInt(v*(profile.Length-1))];return p+new Vector3(Mathf.Sin(u*Mathf.PI*2)*ring.x,ring.y,Mathf.Cos(u*Mathf.PI*2)*ring.x);},mat,Vector2.one,true);
        }
        public static GameObject Beam(string name,Vector3 a,Vector3 b,float thickness,Material mat)
        {var g=Box(name,(a+b)/2,new Vector3(thickness,(b-a).magnitude,thickness),mat);g.transform.localRotation=Quaternion.FromToRotation(Vector3.up,(b-a).normalized);return g;}
        public static void Cylinder(string name,Vector3 p,float radius,float height,Material mat,int sides=16)
        {Lathe(name,p,new[]{new Vector2(0,0),new Vector2(radius,0),new Vector2(radius,height),new Vector2(0,height)},mat,sides);}
        public static void Tube(string name,Vector3 center,float radius,float tube,Material mat,Quaternion rotation,int segments=32)
        {
            Grid(name,segments,6,(u,v)=>{float a=u*Mathf.PI*2,b=v*Mathf.PI*2;return center+rotation*new Vector3(Mathf.Cos(a)*(radius+Mathf.Cos(b)*tube),Mathf.Sin(a)*(radius+Mathf.Cos(b)*tube),Mathf.Sin(b)*tube);},mat,new Vector2(3,1));
        }
        public static void Cable(string name,Vector3 a,Vector3 b,float sag,float width,Material mat)
        {var last=a;for(int i=1;i<=16;i++){float t=i/16f;var next=Vector3.Lerp(a,b,t)-Vector3.up*Mathf.Sin(t*Mathf.PI)*sag;Beam(name,last,next,width,mat);last=next;}}
        public static Mesh Save(Mesh mesh,string path)
        {
            var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(!saved){AssetDatabase.CreateAsset(mesh,path);return mesh;}
            saved.Clear();saved.indexFormat=mesh.indexFormat;saved.vertices=mesh.vertices;saved.normals=mesh.normals;saved.tangents=mesh.tangents;saved.uv=mesh.uv;saved.colors=mesh.colors;saved.triangles=mesh.triangles;saved.bounds=mesh.bounds;Object.DestroyImmediate(mesh);EditorUtility.SetDirty(saved);return saved;
        }
        public static void BakeCategory(Transform root,string prefix)
        {
            var groups=new SortedDictionary<string,List<CombineInstance>>();var materials=new Dictionary<string,Material>();
            foreach(var r in root.GetComponentsInChildren<MeshRenderer>())
            {
                var f=r.GetComponent<MeshFilter>();if(!f||!r.gameObject.isStatic)continue;
                var p=r.bounds.center;string key=Mathf.FloorToInt(p.x/8)+"_"+Mathf.FloorToInt(p.z/8)+"_"+r.sharedMaterial.name;
                if(!groups.ContainsKey(key)){groups[key]=new List<CombineInstance>();materials[key]=r.sharedMaterial;}
                groups[key].Add(new CombineInstance{mesh=f.sharedMesh,transform=root.worldToLocalMatrix*r.transform.localToWorldMatrix});
                var g=r.gameObject;Object.DestroyImmediate(r);Object.DestroyImmediate(f);if(g.transform.childCount==0&&g.GetComponents<Component>().Length==1)Object.DestroyImmediate(g);
            }
            var previous=Parent;Parent=root;
            foreach(var group in groups)
            {
                var mesh=new Mesh{name=prefix+" "+group.Key,indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(group.Value.ToArray(),true,true);mesh.RecalculateBounds();
                mesh=Save(mesh,"Assets/ChapterOne/Models/"+prefix+"_"+group.Key+".asset");
                var bakedObj=MeshObject(group.Key,mesh,materials[group.Key]);
                if(prefix=="Village")bakedObj.GetComponent<MeshRenderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            Parent=previous;PrefabUtility.SaveAsPrefabAsset(root.gameObject,"Assets/ChapterOne/Prefabs/"+prefix+".prefab");
        }
    }
}
