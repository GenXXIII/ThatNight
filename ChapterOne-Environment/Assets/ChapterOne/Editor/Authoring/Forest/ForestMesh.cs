using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne.Editor
{
    // Direct mesh authoring avoids creating thousands of temporary leaf GameObjects.
    internal sealed class ForestMesh
    {
        readonly List<Vector3> vertices = new();
        readonly List<Vector2> uvs = new();
        readonly List<Color> colors = new();
        readonly List<int> triangles = new();
        public int VertexCount => vertices.Count;
        public void Triangle(Vector3 a, Vector3 b, Vector3 c, Color color, float sway = 0)
        {
            int n = vertices.Count;
            vertices.Add(a); vertices.Add(b); vertices.Add(c);
            if(sway>0){uvs.Add(new Vector2(0,0));uvs.Add(new Vector2(.5f,sway));uvs.Add(new Vector2(1,0));}
            else
            {
                var normal=Vector3.Cross(b-a,c-a).normalized;
                Vector2 Project(Vector3 p)=>Mathf.Abs(normal.y)>.6f?new Vector2(p.x,p.z)*.4f:Mathf.Abs(normal.x)>.6f?new Vector2(p.z,p.y)*.4f:new Vector2(p.x,p.y)*.4f;
                uvs.Add(Project(a));uvs.Add(Project(b));uvs.Add(Project(c));
            }
            colors.Add(color * .8f); colors.Add(color); colors.Add(color * .9f);
            triangles.Add(n); triangles.Add(n + 1); triangles.Add(n + 2);
        }
        public void Leaf(Vector3 start, Vector3 end, Vector3 side, Color color)
        {
            var mid = Vector3.Lerp(start, end, .43f) + Vector3.up * .045f;
            Triangle(start, mid + side, end, color, 1);
            Triangle(start, end, mid - side, color, 1);
        }
        public void Rod(Vector3 a, Vector3 b, float r0, float r1, Color color, int sides = 7)
        {
            var axis = (b - a).normalized;
            var right = Vector3.Cross(axis, Mathf.Abs(axis.y) > .9f ? Vector3.forward : Vector3.up).normalized;
            var up = Vector3.Cross(axis, right);
            for (int i = 0; i < sides; i++)
            {
                float t = i * Mathf.PI * 2 / sides, t1 = (i + 1) * Mathf.PI * 2 / sides;
                var n0 = right * Mathf.Cos(t) + up * Mathf.Sin(t);
                var n1 = right * Mathf.Cos(t1) + up * Mathf.Sin(t1);
                Triangle(a + n0 * r0, b + n0 * r1, b + n1 * r1, color);
                Triangle(a + n0 * r0, b + n1 * r1, a + n1 * r0, color);
                Triangle(b, b + n1 * r1, b + n0 * r1, color * .65f);
            }
        }
        public void Rock(Vector3 center, Vector3 scale, float seed)
        {
            Vector3 Point(int ring, int side)
            {
                float v = ring * Mathf.PI / 5, u = side * Mathf.PI * 2 / 9;
                float noise = 1 + .15f * Mathf.Sin(side * 7.1f + ring * 2.5f + seed);
                return center + Vector3.Scale(new Vector3(Mathf.Sin(v) * Mathf.Cos(u), Mathf.Cos(v), Mathf.Sin(v) * Mathf.Sin(u)) * noise, scale);
            }
            for (int r = 0; r < 5; r++) for (int s = 0; s < 9; s++)
            {
                Color tint = r < 2 ? new Color(.46f, .58f, .39f) : new Color(.65f, .68f, .62f);
                Triangle(Point(r,s), Point(r+1,s), Point(r,s+1), tint);
                Triangle(Point(r,s+1), Point(r+1,s), Point(r+1,s+1), tint);
            }
        }
        public Mesh Create(string name, Vector3 origin)
        {
            var mesh = new Mesh { name = name, indexFormat = IndexFormat.UInt32 };
            for (int i = 0; i < vertices.Count; i++) vertices[i] -= origin;
            mesh.SetVertices(vertices); mesh.SetUVs(0, uvs); mesh.SetColors(colors); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateTangents(); mesh.RecalculateBounds(); return mesh;
        }
    }
}
