var sv = UnityEditor.SceneView.lastActiveSceneView;
if (sv == null || sv.camera == null) return "No SceneView found";
var cam = sv.camera.transform;
var local = ThatNight.ChapterOne.SiteLayout.HouseLocal(cam.position);
return "SceneView Cam Pos: " + cam.position + " | Local House Pos: " + local + " | Cam Forward: " + cam.forward;
