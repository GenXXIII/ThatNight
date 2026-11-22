ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding == null) return "No wedding root found";

var oldZone = wedding.transform.Find("Right hair cutting ceremony zone - Gaat Sah");
if (oldZone != null) {
    UnityEngine.Object.DestroyImmediate(oldZone.gameObject);
}

ThatNight.ChapterOne.Editor.MeshFactory.Parent = wedding.transform;
System.Type eventType = null;
foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
    eventType = asm.GetType("ThatNight.ChapterOne.Editor.WeddingEventBuilder");
    if (eventType != null) break;
}
var buildMethod = eventType != null ? eventType.GetMethod("Build", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic) : null;
try {
    buildMethod.Invoke(null, null);
} catch (System.Reflection.TargetInvocationException tie) {
    return "Inner Exception: " + tie.InnerException.ToString();
} catch (System.Exception ex) {
    return "Exception: " + ex.ToString();
}
ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

// Ensure all new objects in the zone are non-static
var newZone = wedding.transform.Find("Right hair cutting ceremony zone - Gaat Sah");
if (newZone != null) {
    foreach (var t in newZone.GetComponentsInChildren<UnityEngine.Transform>(true)) {
        t.gameObject.isStatic = false;
    }
}

// 1. Screenshot of the Stage: High-Frame Thrones & Half-Frame Attendants
string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("CeremonyCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 60f;
cam.fieldOfView = 50f;

var fillObj = new UnityEngine.GameObject("CeremonyFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 25f;
fill.intensity = 2.2f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.88f);
fill.transform.position = new UnityEngine.Vector3(13.2f, 24.5f, 0.5f);

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// 1. Front Stage View (Bride & Groom High Thrones + Half-Frame Attendants)
cam.transform.position = new UnityEngine.Vector3(13.2f, 22.4f, -2.5f);
cam.transform.LookAt(new UnityEngine.Vector3(13.2f, 22.4f, 2.0f));

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_stage_high_and_half_frame_chairs.png"), tex.EncodeToPNG());

// 2. View showing Stage AND Music Place on the Left of the Stage
cam.transform.position = new UnityEngine.Vector3(10.5f, 23.2f, -4.5f);
cam.transform.LookAt(new UnityEngine.Vector3(11.5f, 21.8f, 1.2f));

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_music_left_of_stage.png"), tex.EncodeToPNG());

// 3. View of the Audience rows with Fabric Slipcovers
cam.transform.position = new UnityEngine.Vector3(13.2f, 23.5f, -11.5f);
cam.transform.LookAt(new UnityEngine.Vector3(13.2f, 22.0f, -4.5f));

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_ceremony_audience_covered_chairs.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

return "Ceremony zone successfully rebuilt with high-frame thrones, half-frame attendant chairs, off-stage left music, and fabric-covered audience chairs!";
