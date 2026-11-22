using UnityEngine;
using ThatNight.ChapterOne;
var p=Object.FindFirstObjectByType<PlayerInteraction>();var held=p.HeldItem;
return "Held="+(held?held.displayName:"none")+" parent="+(held?held.transform.parent.name:"none")+" Busy="+p.Busy+" Locked="+p.explorer.ControlsLocked;
