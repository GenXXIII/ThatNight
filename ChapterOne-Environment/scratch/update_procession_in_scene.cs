// Ensure SurfaceLibrary is prepared
ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house == null) return "No House found";

// 1. Destroy old food feast object
var oldFood = house.transform.Find("Walk ceremony food procession feast - Pithy Hae Chumneun");
if (oldFood != null) {
    UnityEngine.Object.DestroyImmediate(oldFood.gameObject);
}
var oldFruit = house.transform.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (oldFruit != null) {
    UnityEngine.Object.DestroyImmediate(oldFruit.gameObject);
}

// 2. Move Tu Kanchok away from the window cutout to solid wall
var cupboard = house.transform.Find("Carved teak glass display cupboard - Tu Kanchok");
if (cupboard != null) {
    cupboard.localPosition = new UnityEngine.Vector3(-7.5f + 0.35f, 2.455f, -4.6f);
}

// 3. Build new paired wedding fruit procession under House
ThatNight.ChapterOne.Editor.MeshFactory.Parent = house.transform;
System.Type builderType = null;
foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
    builderType = asm.GetType("ThatNight.ChapterOne.Editor.StiltHouseBuilder");
    if (builderType != null) break;
}
var method = builderType != null ? builderType.GetMethod("BuildWeddingFruitProcession", 
    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public) : null;
try {
    method.Invoke(null, new object[]{ new UnityEngine.Vector3(5.1f, 2.455f, -1.6f) });
} catch (System.Reflection.TargetInvocationException tie) {
    return "Inner Exception: " + tie.InnerException.ToString();
} catch (System.Exception ex) {
    return "Exception: " + ex.ToString();
}
ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

// 4. Render top-down preview of the new paired fruit procession
string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("FruitCheckCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 50f;
cam.fieldOfView = 55f;

var fillObj = new UnityEngine.GameObject("FruitFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 20f;
fill.intensity = 2.0f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.85f);
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.1f, 5.0f, -1.6f));

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// Angle down at the paired fruit procession
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.1f, 6.2f, -1.6f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.1f, 2.455f, -1.6f)));

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_paired_wedding_fruits_topdown.png"), tex.EncodeToPNG());

// Also render the cleared window where Tu Kanchok was moved away
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-4.5f, 3.65f, -3.5f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-7.5f, 3.65f, -3.5f)));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_cleared_window_view.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

// Mark scene dirty and save
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

return "Paired fruit procession updated, window cleared, and preview rendered successfully!";
