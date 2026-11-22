ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house == null) return "No House found";

var oldFruit = house.transform.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
UnityEngine.Vector3 savedPos = new UnityEngine.Vector3(-4.17f, 2.57f, -4.30f);
UnityEngine.Quaternion savedRot = UnityEngine.Quaternion.Euler(0, 91.39f, 0);

if (oldFruit != null) {
    savedPos = oldFruit.localPosition;
    savedRot = oldFruit.localRotation;
    UnityEngine.Object.DestroyImmediate(oldFruit.gameObject);
}

// Build new 36-tray fruit procession under House
ThatNight.ChapterOne.Editor.MeshFactory.Parent = house.transform;
System.Type builderType = null;
foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
    builderType = asm.GetType("ThatNight.ChapterOne.Editor.StiltHouseBuilder");
    if (builderType != null) break;
}
var method = builderType != null ? builderType.GetMethod("BuildWeddingFruitProcession", 
    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public) : null;

try {
    method.Invoke(null, new object[]{ savedPos });
} catch (System.Reflection.TargetInvocationException tie) {
    return "Inner Exception: " + tie.InnerException.ToString();
} catch (System.Exception ex) {
    return "Exception: " + ex.ToString();
}
ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

// Ensure new object has saved rotation and is NOT static
var newFruit = house.transform.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (newFruit != null) {
    newFruit.localRotation = savedRot;
    newFruit.gameObject.isStatic = false;
}

// Render verification screenshot
string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("FruitCaptureCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 50f;
cam.fieldOfView = 55f;

var fillObj = new UnityEngine.GameObject("FruitFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 18f;
fill.intensity = 2.5f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.85f);
fill.transform.position = newFruit.transform.position + UnityEngine.Vector3.up * 2.8f;

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// 1. Top-down view
cam.transform.position = newFruit.transform.position + UnityEngine.Vector3.up * 4.2f;
cam.transform.LookAt(newFruit.transform.position, newFruit.transform.forward);

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_36_wedding_fruits_topdown.png"), tex.EncodeToPNG());

// 2. Perspective view looking down the aisle of paired fruit trays
cam.transform.position = newFruit.transform.position + UnityEngine.Vector3.up * 1.8f - newFruit.transform.forward * 3.4f;
cam.transform.LookAt(newFruit.transform.position + UnityEngine.Vector3.up * 0.4f);

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_36_wedding_fruits_perspective.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

return "36 fruit trays successfully built and verified in scene!";
