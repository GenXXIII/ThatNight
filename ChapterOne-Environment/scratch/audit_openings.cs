using UnityEngine;
using ThatNight.ChapterOne;
var result=new System.Collections.Generic.List<string>();
foreach(var h in Object.FindObjectsByType<HingedOpening>(FindObjectsSortMode.None)){
var mesh=h.GetComponent<MeshFilter>().sharedMesh;var closed=Quaternion.Euler(0,h.closedAngle,0)*mesh.bounds.center;var open=Quaternion.Euler(0,h.openAngle,0)*mesh.bounds.center;
result.Add(h.name+" vertices="+mesh.vertexCount+" closed="+closed+" open="+open+" angles="+h.closedAngle+","+h.openAngle);
}
result.Add("Guests="+Object.FindObjectsByType<WeddingGuest>(FindObjectsSortMode.None).Length+" Pickups="+Object.FindObjectsByType<PickupItem>(FindObjectsSortMode.None).Length);
return string.Join("\n",result);
