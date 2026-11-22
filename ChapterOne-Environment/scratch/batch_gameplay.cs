using UnityEngine;
using ThatNight.ChapterOne.Editor;
var cam=Object.FindFirstObjectByType<ThatNight.ChapterOne.EnvironmentCamera>().GetComponent<Camera>();cam.fieldOfView=65;cam.nearClipPlane=.04f;
return typeof(FirstPersonGameplayBuilder).Assembly.GetType("ThatNight.ChapterOne.Editor.EnvironmentPerformanceOptimizer").GetMethod("ApplyToScene").Invoke(null,new object[]{cam});
