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
buildMethod.Invoke(null, null);
ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

// Ensure all new objects in the zone are non-static
var newZone = wedding.transform.Find("Right hair cutting ceremony zone - Gaat Sah");
if (newZone != null) {
    foreach (var t in newZone.GetComponentsInChildren<UnityEngine.Transform>(true)) {
        t.gameObject.isStatic = false;
    }
}

string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("CeremonyCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 65f;
cam.fieldOfView = 55f;

var fillObj = new UnityEngine.GameObject("CeremonyFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 30f;
fill.intensity = 2.4f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.88f);
fill.transform.position = new UnityEngine.Vector3(13.2f, 25.0f, -4.0f);

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// 1. Close-up of the Enlarged 6-Musician Ensemble on the Left of the Stage
cam.transform.position = new UnityEngine.Vector3(4.2f, 23.5f, -1.8f);
cam.transform.LookAt(new UnityEngine.Vector3(6.8f, 21.3f, 1.2f));

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_6_musicians_instruments_closeup.png"), tex.EncodeToPNG());

// 2. Wide view from back of the 6 audience rows looking toward stage and side music
cam.transform.position = new UnityEngine.Vector3(13.2f, 24.2f, -15.5f);
cam.transform.LookAt(new UnityEngine.Vector3(13.2f, 22.2f, 0f));

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_expanded_audience_6_rows_12_cols.png"), tex.EncodeToPNG());

// 3. Angled view of the entire audience seating wings
cam.transform.position = new UnityEngine.Vector3(5.0f, 24.0f, -14.0f);
cam.transform.LookAt(new UnityEngine.Vector3(13.2f, 22.0f, -6.5f));

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_audience_seating_wings_angle.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

var musicObj = UnityEngine.GameObject.Find("Traditional Cambodian wedding music ensemble - Pinpeat");
string mPos = musicObj != null ? musicObj.transform.position.ToString() : "null";

return "Expanded audience seating (+2 rows, +4 columns) and 6-musician ensemble rebuilt! Music pos: " + mPos;
