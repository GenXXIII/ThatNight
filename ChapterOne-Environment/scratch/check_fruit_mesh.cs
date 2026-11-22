var fruit = UnityEngine.GameObject.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (fruit == null) return "No fruit object found";
var mf = fruit.GetComponent<UnityEngine.MeshFilter>();
var mr = fruit.GetComponent<UnityEngine.MeshRenderer>();
if (mf == null || mf.sharedMesh == null) return "No mesh found on fruit!";
return "Mesh name: " + mf.sharedMesh.name + " | verts: " + mf.sharedMesh.vertexCount + " | submeshes: " + mf.sharedMesh.subMeshCount + " | mats: " + mr.sharedMaterials.Length;
