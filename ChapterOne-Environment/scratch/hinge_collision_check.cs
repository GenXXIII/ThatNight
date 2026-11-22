using UnityEngine;
using ThatNight.ChapterOne;
var lines=new System.Collections.Generic.List<string>();int pass=0;
foreach(var h in Object.FindObjectsByType<HingedOpening>(FindObjectsSortMode.None)){
 var previous=h.transform.localRotation;var center=Quaternion.Euler(0,h.closedAngle,0)*h.GetComponent<MeshFilter>().sharedMesh.bounds.center;
 var origin=h.transform.parent.TransformPoint(h.transform.localPosition+center+Vector3.back*.4f);var direction=h.transform.parent.TransformDirection(Vector3.forward);
 var ray=new Ray(origin,direction);var shape=h.GetComponent<Collider>();float length=.8f*h.transform.lossyScale.z;
 h.transform.localRotation=Quaternion.Euler(0,h.closedAngle,0);Physics.SyncTransforms();bool closed=shape.Raycast(ray,out var a,length);
 h.transform.localRotation=Quaternion.Euler(0,h.openAngle,0);Physics.SyncTransforms();bool open=shape.Raycast(ray,out var b,length);
 h.transform.localRotation=previous;Physics.SyncTransforms();
 if(closed&&!open)pass++;else lines.Add("FAIL "+h.name+" closed="+closed+" open="+open);
}
lines.Add("Closed blocks / open clears: "+pass+" of 24 leaves");System.IO.File.WriteAllLines("Review/Gameplay/hinge-collision-check.txt",lines);return string.Join("\n",lines);
