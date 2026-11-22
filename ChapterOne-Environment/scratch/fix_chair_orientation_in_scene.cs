ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var seatingZone = UnityEngine.GameObject.Find("Left and right guest banquet seating zones");
if (seatingZone == null) return "No seating zone found";

// Destroy existing chairs under seatingZone
var toDestroy = new System.Collections.Generic.List<UnityEngine.GameObject>();
foreach (UnityEngine.Transform t in seatingZone.transform) {
    if (t.name.ToLower().Contains("chair")) {
        toDestroy.Add(t.gameObject);
    }
}
foreach (var go in toDestroy) {
    UnityEngine.Object.DestroyImmediate(go);
}

// 12 Banquet Table positions from WeddingYardBuilder
UnityEngine.Vector3[] leftTablePositions = {
    // Front row overlooking the mountain slope
    new UnityEngine.Vector3(-18.5f, 0, -8.5f), new UnityEngine.Vector3(-14.2f, 0, -9.0f), new UnityEngine.Vector3(-9.8f, 0, -8.8f), new UnityEngine.Vector3(-5.5f, 0, -8.2f),
    // Mid row
    new UnityEngine.Vector3(-18.8f, 0, -3.8f), new UnityEngine.Vector3(-14.4f, 0, -4.2f), new UnityEngine.Vector3(-10.0f, 0, -4.0f), new UnityEngine.Vector3(-5.6f, 0, -3.5f),
    // Rear row near the stilt house
    new UnityEngine.Vector3(-18.5f, 0, 1.2f), new UnityEngine.Vector3(-14.2f, 0, 0.8f), new UnityEngine.Vector3(-9.8f, 0, 1.0f), new UnityEngine.Vector3(-5.5f, 0, 1.5f)
};

ThatNight.ChapterOne.Editor.MeshFactory.Parent = seatingZone.transform;

int chairCount = 0;
foreach (var pos in leftTablePositions) {
    for (int chair = 0; chair < 6; chair++) {
        float a = chair * UnityEngine.Mathf.PI / 3f + .15f;
        var chairPos = pos + new UnityEngine.Vector3(UnityEngine.Mathf.Sin(a) * 1.26f, 0, UnityEngine.Mathf.Cos(a) * 1.26f);
        // Facing INWARD toward the table center
        float yaw = a * UnityEngine.Mathf.Rad2Deg;
        ThatNight.ChapterOne.Editor.PropsBuilder.Chair(chairPos, yaw);
        chairCount++;
    }
}

ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

// Render verification screenshot of the table with chairs facing INWARD
string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var table = UnityEngine.GameObject.Find("Round banquet table set");
if (table == null) return "No table found for preview";

var camObj = new UnityEngine.GameObject("TableCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 50f;
cam.fieldOfView = 50f;

var fillObj = new UnityEngine.GameObject("TableFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 15f;
fill.intensity = 2.0f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.85f);
fill.transform.position = table.transform.position + UnityEngine.Vector3.up * 2.5f;

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// Angled view looking down at the table and chairs
cam.transform.position = table.transform.position + new UnityEngine.Vector3(2.6f, 2.0f, 2.6f);
cam.transform.LookAt(table.transform.position + UnityEngine.Vector3.up * 0.45f);

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_chairs_facing_table.png"), tex.EncodeToPNG());

// Side profile showing seats tucked toward table edge
cam.transform.position = table.transform.position + new UnityEngine.Vector3(-3.2f, 1.4f, 0.5f);
cam.transform.LookAt(table.transform.position + UnityEngine.Vector3.up * 0.45f);

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_chairs_facing_table_side.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

return "Rebuilt " + chairCount + " chairs facing inward toward tables successfully!";
