using UnityEngine;
using UnityEditor;
var lines=new System.Collections.Generic.List<string>();
lines.Add("PLAY="+EditorApplication.isPlaying+" DIRTY="+UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty);
foreach(var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())lines.Add("ROOT "+root.name+" "+root.transform.position);
foreach(var f in Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))
{
 if(f.name.Contains("wall and shutters")||f.name.Contains("partition wall")||f.name.Contains("entrance doors")||f.name.Contains("wardrobe")||f.name.Contains("wooden door leaf"))
  lines.Add(f.name+" local="+f.transform.localPosition+" world="+f.transform.position+" rot="+f.transform.eulerAngles+" mesh="+(f.sharedMesh?f.sharedMesh.vertexCount:0)+" bounds="+(f.sharedMesh?f.sharedMesh.bounds.ToString():"none")+" children="+f.transform.childCount);
}
return string.Join("\n",lines);
