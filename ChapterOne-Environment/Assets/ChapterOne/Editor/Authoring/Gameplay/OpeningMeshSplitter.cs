using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne.Editor
{
    public static class OpeningMeshSplitter
    {
        public static Transform Extract(MeshFilter source,string name,Vector3 pivot,Func<Vector3,bool> select)
        {
            var original=source.sharedMesh;var vertices=original.vertices;
            var moving=new List<int>[original.subMeshCount];var remaining=new List<int>[original.subMeshCount];int count=0;
            for(int sub=0;sub<original.subMeshCount;sub++)
            {
                moving[sub]=new List<int>();remaining[sub]=new List<int>();var indices=original.GetTriangles(sub);
                for(int i=0;i<indices.Length;i+=3)
                {
                    bool take=select(vertices[indices[i]])&&select(vertices[indices[i+1]])&&select(vertices[indices[i+2]]);
                    var destination=take?moving[sub]:remaining[sub];
                    destination.Add(indices[i]);destination.Add(indices[i+1]);destination.Add(indices[i+2]);if(take)count++;
                }
            }
            if(count==0)return null;
            source.sharedMesh=Subset(original,remaining,Vector3.zero,source.name+" fixed");
            var leaf=CharacterArtBuilder.Group(name,source.transform,pivot);
            leaf.gameObject.AddComponent<MeshFilter>().sharedMesh=Subset(original,moving,pivot,name);
            leaf.gameObject.AddComponent<MeshRenderer>().sharedMaterials=source.GetComponent<MeshRenderer>().sharedMaterials;
            leaf.gameObject.AddComponent<MeshCollider>().sharedMesh=leaf.GetComponent<MeshFilter>().sharedMesh;
            return leaf;
        }
        public static void Restore(MeshFilter source)
        {
            var leaves=source.GetComponentsInChildren<HingedOpening>();if(leaves.Length==0)return;
            var merged=new Mesh{name=source.name+" restored",indexFormat=IndexFormat.UInt32};
            var parts=new System.Collections.Generic.List<CombineInstance>();
            var subMeshes=new System.Collections.Generic.List<Mesh>();
            for(int sub=0;sub<source.sharedMesh.subMeshCount;sub++)
            {
                parts.Clear();parts.Add(new CombineInstance{mesh=source.sharedMesh,subMeshIndex=sub,transform=Matrix4x4.identity});
                foreach(var leaf in leaves)parts.Add(new CombineInstance{mesh=leaf.GetComponent<MeshFilter>().sharedMesh,subMeshIndex=sub,transform=Matrix4x4.Translate(leaf.transform.localPosition)});
                var mesh=new Mesh{indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(parts.ToArray(),true,true);subMeshes.Add(mesh);
            }
            parts.Clear();foreach(var mesh in subMeshes)parts.Add(new CombineInstance{mesh=mesh,transform=Matrix4x4.identity});
            merged.CombineMeshes(parts.ToArray(),false,true);source.sharedMesh=merged;
            foreach(var mesh in subMeshes)UnityEngine.Object.DestroyImmediate(mesh);
            foreach(var leaf in leaves)UnityEngine.Object.DestroyImmediate(leaf.gameObject);
        }
        static Mesh Subset(Mesh source,List<int>[] groups,Vector3 pivot,string name)
        {
            var oldV=source.vertices;var oldN=source.normals;var oldUV=source.uv;var oldC=source.colors;var oldT=source.tangents;
            var map=new Dictionary<int,int>();var v=new List<Vector3>();var n=new List<Vector3>();var uv=new List<Vector2>();var c=new List<Color>();var t=new List<Vector4>();
            var lists=new List<int>[groups.Length];
            for(int sub=0;sub<groups.Length;sub++)
            {
                lists[sub]=new List<int>();
                foreach(int index in groups[sub])
                {
                    if(!map.TryGetValue(index,out int next))
                    {
                        next=v.Count;map.Add(index,next);v.Add(oldV[index]-pivot);
                        n.Add(oldN.Length==oldV.Length?oldN[index]:Vector3.up);uv.Add(oldUV.Length==oldV.Length?oldUV[index]:Vector2.zero);
                        c.Add(oldC.Length==oldV.Length?oldC[index]:Color.white);t.Add(oldT.Length==oldV.Length?oldT[index]:new Vector4(1,0,0,1));
                    }
                    lists[sub].Add(next);
                }
            }
            var mesh=new Mesh{name=name,indexFormat=IndexFormat.UInt32};mesh.SetVertices(v);mesh.SetNormals(n);mesh.SetUVs(0,uv);mesh.SetColors(c);mesh.SetTangents(t);mesh.subMeshCount=groups.Length;
            for(int i=0;i<lists.Length;i++)mesh.SetTriangles(lists[i],i);mesh.RecalculateBounds();
            string folder="Assets/ChapterOne/Gameplay/Meshes";System.IO.Directory.CreateDirectory(folder);
            AssetDatabase.CreateAsset(mesh,AssetDatabase.GenerateUniqueAssetPath(folder+"/"+name+".asset"));return mesh;
        }
    }
}
