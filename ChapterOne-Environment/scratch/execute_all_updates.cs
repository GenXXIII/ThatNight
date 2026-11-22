ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding == null) return "No wedding root found";

// ==============================================================
// 1. REBUILD CEREMONY ZONE (Stage, Music, Audience)
// ==============================================================
var oldCeremonyZone = wedding.transform.Find("Right hair cutting ceremony zone - Gaat Sah");
if (oldCeremonyZone != null) {
    UnityEngine.Object.DestroyImmediate(oldCeremonyZone.gameObject);
}

ThatNight.ChapterOne.Editor.MeshFactory.Parent = wedding.transform;
System.Type eventType = null;
foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
    eventType = asm.GetType("ThatNight.ChapterOne.Editor.WeddingEventBuilder");
    if (eventType != null) break;
}
var buildCeremonyMethod = eventType != null ? eventType.GetMethod("Build", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic) : null;
buildCeremonyMethod.Invoke(null, null);
ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

var newCeremonyZone = wedding.transform.Find("Right hair cutting ceremony zone - Gaat Sah");
if (newCeremonyZone != null) {
    foreach (var t in newCeremonyZone.GetComponentsInChildren<UnityEngine.Transform>(true)) {
        t.gameObject.isStatic = false;
    }
}

// ==============================================================
// 2. REBUILD LEFT BANQUET ZONE (4 COLUMNS x 5 ROWS = 20 TABLES)
// ==============================================================
var seatingZone = wedding.transform.Find("Left and right guest banquet seating zones");
if (seatingZone != null) {
    var toDestroy = new System.Collections.Generic.List<UnityEngine.GameObject>();
    for (int i = 0; i < seatingZone.childCount; i++) {
        var child = seatingZone.GetChild(i).gameObject;
        if (child.name.Contains("Round banquet table set") || child.name.Contains("Wedding banquet chair")) {
            toDestroy.Add(child);
        }
    }
    foreach (var g in toDestroy) {
        UnityEngine.Object.DestroyImmediate(g);
    }

    System.Type yardBuilderType = null;
    foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
        yardBuilderType = asm.GetType("ThatNight.ChapterOne.Editor.WeddingYardBuilder");
        if (yardBuilderType != null) break;
    }
    var diningTableMethod = yardBuilderType.GetMethod("DiningTable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

    System.Type propsBuilderType = null;
    foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
        propsBuilderType = asm.GetType("ThatNight.ChapterOne.Editor.PropsBuilder");
        if (propsBuilderType != null) break;
    }
    var chairMethod = propsBuilderType.GetMethod("Chair", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

    UnityEngine.Vector3[] leftTablePositions = {
        // Row 1 (Front row overlooking mountain slope)
        new UnityEngine.Vector3(-18.5f, 0, -8.5f), new UnityEngine.Vector3(-14.2f, 0, -8.5f), new UnityEngine.Vector3(-9.9f, 0, -8.5f), new UnityEngine.Vector3(-5.5f, 0, -8.5f),
        // Row 2
        new UnityEngine.Vector3(-18.5f, 0, -4.0f), new UnityEngine.Vector3(-14.2f, 0, -4.0f), new UnityEngine.Vector3(-9.9f, 0, -4.0f), new UnityEngine.Vector3(-5.5f, 0, -4.0f),
        // Row 3
        new UnityEngine.Vector3(-18.5f, 0,  0.5f), new UnityEngine.Vector3(-14.2f, 0,  0.5f), new UnityEngine.Vector3(-9.9f, 0,  0.5f), new UnityEngine.Vector3(-5.5f, 0,  0.5f),
        // Row 4
        new UnityEngine.Vector3(-18.5f, 0,  5.0f), new UnityEngine.Vector3(-14.2f, 0,  5.0f), new UnityEngine.Vector3(-9.9f, 0,  5.0f), new UnityEngine.Vector3(-5.5f, 0,  5.0f),
        // Row 5 (Rear row near back marquee eave)
        new UnityEngine.Vector3(-18.5f, 0,  9.5f), new UnityEngine.Vector3(-14.2f, 0,  9.5f), new UnityEngine.Vector3(-9.9f, 0,  9.5f), new UnityEngine.Vector3(-5.5f, 0,  9.5f)
    };

    foreach (var pos in leftTablePositions) {
        ThatNight.ChapterOne.Editor.MeshFactory.Parent = seatingZone;
        diningTableMethod.Invoke(null, new object[] { pos });

        for (int chair = 0; chair < 6; chair++) {
            float a = chair * UnityEngine.Mathf.PI / 3 + .15f;
            ThatNight.ChapterOne.Editor.MeshFactory.Parent = seatingZone;
            UnityEngine.Vector3 chairPos = pos + new UnityEngine.Vector3(UnityEngine.Mathf.Sin(a) * 1.26f, 0, UnityEngine.Mathf.Cos(a) * 1.26f);
            float chairYaw = a * UnityEngine.Mathf.Rad2Deg;
            chairMethod.Invoke(null, new object[] { chairPos, chairYaw });
        }
    }
    ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

    foreach (var t in seatingZone.GetComponentsInChildren<UnityEngine.Transform>(true)) {
        t.gameObject.isStatic = false;
    }
}

// ==============================================================
// 3. CAPTURE HIGH QUALITY SCREENSHOTS
// ==============================================================
string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("CeremonyCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 85f;
cam.fieldOfView = 58f;

var fillObj = new UnityEngine.GameObject("CeremonyFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 35f;
fill.intensity = 2.4f;
fill.color = new UnityEngine.Color(1.0f, 0.96f, 0.90f);
fill.transform.position = new UnityEngine.Vector3(10.0f, 25.0f, -2.0f);

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// 1. Close-up of Music Platform: 5.5m wide x 3.5m deep (half of stage front-back), 6 musicians and instruments
cam.transform.position = new UnityEngine.Vector3(4.0f, 24.2f, -2.4f);
cam.transform.LookAt(new UnityEngine.Vector3(6.8f, 21.2f, 0.5f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_music_place_half_stage_closeup.png"), tex.EncodeToPNG());

// 2. Wide View of Left Banquet Dining: 4 columns x 5 rows = 20 tables with fabric-covered chairs
fill.transform.position = new UnityEngine.Vector3(-12.0f, 25.5f, 0f);
cam.transform.position = new UnityEngine.Vector3(-12.0f, 26.0f, -14.5f);
cam.transform.LookAt(new UnityEngine.Vector3(-12.0f, 21.8f, 1.5f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_banquet_4x5_wide.png"), tex.EncodeToPNG());

// 3. Wide View of Ceremony Stage, Restored 4-row Audience, and Left Music Place
fill.transform.position = new UnityEngine.Vector3(12.0f, 25.0f, -3.0f);
cam.transform.position = new UnityEngine.Vector3(11.0f, 24.0f, -13.0f);
cam.transform.LookAt(new UnityEngine.Vector3(11.0f, 22.0f, 1.0f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_ceremony_stage_and_audience.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Successfully updated both Music Place (half-stage depth 3.5m x 5.5m wide for 6 musicians), restored 4-row audience, and rebuilt 4x5 banquet tables!";
