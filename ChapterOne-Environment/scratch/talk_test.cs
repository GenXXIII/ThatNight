using UnityEngine;
using ThatNight.ChapterOne;
var p=Object.FindFirstObjectByType<PlayerInteraction>();var g=System.Array.Find(Object.FindObjectsByType<WeddingGuest>(FindObjectsSortMode.None),x=>x.displayName=="Dara");
p.explorer.SetWalking(false);p.transform.position=g.transform.position+new Vector3(0,1.7f,-1.45f);p.transform.LookAt(g.transform.position+Vector3.up*1.5f);p.explorer.SetWalking(true);p.Talk(g);
return "Conversation="+p.Speaker.displayName+" controlsLocked="+p.explorer.ControlsLocked;
