using UnityEngine;
using ThatNight.ChapterOne;
var p=Object.FindFirstObjectByType<PlayerInteraction>();p.CloseDialogue();
p.explorer.SetWalking(false);p.transform.position=new Vector3(2.2f,22.62f,-16.25f);p.transform.LookAt(new Vector3(2.3f,21.875f,-15.4f));p.explorer.SetWalking(true);
var item=System.Array.Find(Object.FindObjectsByType<PickupItem>(FindObjectsSortMode.None),x=>x.displayName=="Tea cup");item.Use(p);return "Pickup reach started="+p.Busy;
