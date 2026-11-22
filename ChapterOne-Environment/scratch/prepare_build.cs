using UnityEngine;
using ThatNight.ChapterOne;
var p=Object.FindFirstObjectByType<PlayerInteraction>();
foreach(Transform finger in p.body.rightHand)if(finger.name.StartsWith("Finger "))finger.localRotation=Quaternion.Euler(-72,0,0);
p.transform.SetPositionAndRotation(p.arrival.position,p.arrival.rotation);
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
return "Ready to save and build";
