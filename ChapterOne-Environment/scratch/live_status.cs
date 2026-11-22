using UnityEngine;
using ThatNight.ChapterOne;
var p=Object.FindFirstObjectByType<PlayerInteraction>();
return "Play="+Application.isPlaying+" frames="+Time.frameCount+" Walking="+p.explorer.Walking+" grounded="+(p.explorer.Body?p.explorer.Body.isGrounded:false)+" camera="+p.transform.position+" GPU="+SystemInfo.graphicsDeviceName+" dt="+Time.unscaledDeltaTime+" batches="+UnityEditor.UnityStats.batches+" triangles="+UnityEditor.UnityStats.triangles;
