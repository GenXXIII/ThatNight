ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding == null) return "No wedding root";

var oldUnified = UnityEngine.GameObject.Find("Ceremonial Wedding Carpet Network");
if (oldUnified != null) UnityEngine.Object.DestroyImmediate(oldUnified);

var carpetRoot = new UnityEngine.GameObject("Ceremonial Wedding Carpet Network");
carpetRoot.transform.SetParent(wedding.transform, false);
carpetRoot.transform.localPosition = UnityEngine.Vector3.zero;
carpetRoot.isStatic = false;

var pinkMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Pink;
var goldSilk = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Carpet gold piping", new UnityEngine.Color(.82f, .65f, .18f), .45f, .25f);

ThatNight.ChapterOne.Editor.MeshFactory.Parent = carpetRoot.transform;
float groundY = 0.018f;

// 1A. Main Promenade: Entrance Gate (Z = -18.0) to Z = 8.5, width 2.0m: X in [-1.0, +1.0]
ThatNight.ChapterOne.Editor.MeshFactory.Box("Main promenade", 
    new UnityEngine.Vector3(0, groundY, -4.75f), new UnityEngine.Vector3(2.0f, 0.008f, 26.5f), pinkMat);

// 1B. House Stairs Landing: Clean single rectangular landing from X = -1.0 to 3.0, Z = 8.5 to 10.6
ThatNight.ChapterOne.Editor.MeshFactory.Box("House stairs landing mat", 
    new UnityEngine.Vector3(1.0f, groundY + 0.001f, 9.55f), new UnityEngine.Vector3(4.0f, 0.009f, 2.1f), pinkMat);

// 1C. Cross-Walkway in Front of Music Place: From X = 1.0 to X = 13.2, Z in [-3.4, -1.6] (width 1.8m)
ThatNight.ChapterOne.Editor.MeshFactory.Box("Cross walkway in front of music", 
    new UnityEngine.Vector3(7.1f, groundY + 0.0005f, -2.5f), new UnityEngine.Vector3(12.2f, 0.008f, 1.8f), pinkMat);

// 1D. Stage Center Audience Aisle: From Z = -3.4 down to Z = -9.8 at X = 13.2, width 1.6m (X in [12.4, 14.0])
ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage center audience aisle", 
    new UnityEngine.Vector3(13.2f, groundY + 0.0002f, -6.6f), new UnityEngine.Vector3(1.6f, 0.008f, 6.4f), pinkMat);

// 1E. Stage Front Steps Landing Mat: At X = 13.2, Z = -1.35
ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage front steps landing mat", 
    new UnityEngine.Vector3(13.2f, groundY + 0.001f, -1.35f), new UnityEngine.Vector3(2.4f, 0.009f, 0.9f), pinkMat);

// -------------------------------------------------------------
// PERIMETER-ONLY GOLD BORDER PIPING
// -------------------------------------------------------------
System.Action<string, UnityEngine.Vector3[]> drawBorder = (name, pts) =>
{
    if (pts.Length < 2) return;
    for (int i = 0; i < pts.Length - 1; i++)
    {
        UnityEngine.Vector3 pA = pts[i];
        UnityEngine.Vector3 pB = pts[i + 1];
        UnityEngine.Vector3 dir = (pB - pA).normalized;
        UnityEngine.Vector3 center = (pA + pB) * 0.5f;
        float len = UnityEngine.Vector3.Distance(pA, pB);
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(dir, UnityEngine.Vector3.up);

        var beam = ThatNight.ChapterOne.Editor.MeshFactory.Box(name + "_" + i, center + UnityEngine.Vector3.up * 0.003f, new UnityEngine.Vector3(0.045f, 0.006f, len), goldSilk);
        beam.transform.localRotation = rot;
    }
};

// BORDER 1: West (Left) Edge of Main Promenade up to the House Stairs Landing, then enclosing the stairs landing
drawBorder("WestBorderAndStairsLanding", new[]{
    new UnityEngine.Vector3(-1.0f, groundY, -18.0f),
    new UnityEngine.Vector3(-1.0f, groundY, 10.6f),  // runs all the way north to back of stairs landing
    new UnityEngine.Vector3(3.0f, groundY, 10.6f),   // runs east across back of stairs landing
    new UnityEngine.Vector3(3.0f, groundY, 8.5f),    // runs south down east edge of stairs landing
    new UnityEngine.Vector3(1.0f, groundY, 8.5f),    // runs west along front of landing to main walkway
    new UnityEngine.Vector3(1.0f, groundY, -1.6f)    // runs south along east edge of main walkway to north of cross-walkway
});

// BORDER 2: East edge south of cross-walkway -> Turns east along south edge of cross-walkway -> Turns south down west edge of stage aisle
drawBorder("SouthCrossBorder", new[]{
    new UnityEngine.Vector3(1.0f, groundY, -18.0f),
    new UnityEngine.Vector3(1.0f, groundY, -3.4f),
    new UnityEngine.Vector3(12.4f, groundY, -3.4f), // clean 90-deg turn east
    new UnityEngine.Vector3(12.4f, groundY, -9.8f), // clean 90-deg turn south
    new UnityEngine.Vector3(14.0f, groundY, -9.8f)  // hem across bottom of audience aisle
});

// BORDER 3: East edge of stage aisle -> Turns along stage front landing -> Turns west along north edge of cross-walkway
drawBorder("NorthCrossBorder", new[]{
    new UnityEngine.Vector3(14.0f, groundY, -9.8f),
    new UnityEngine.Vector3(14.0f, groundY, -1.8f),
    new UnityEngine.Vector3(14.4f, groundY, -1.8f),
    new UnityEngine.Vector3(14.4f, groundY, -0.9f),
    new UnityEngine.Vector3(12.0f, groundY, -0.9f),
    new UnityEngine.Vector3(12.0f, groundY, -1.6f),
    new UnityEngine.Vector3(1.0f, groundY, -1.6f)   // meets BORDER 1 cleanly at (1.0, -1.6)!
});

// South entrance hem (Z = -18.0)
drawBorder("EntranceHem", new[]{
    new UnityEngine.Vector3(-1.0f, groundY, -18.0f),
    new UnityEngine.Vector3(1.0f, groundY, -18.0f)
});

ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

foreach (var t in carpetRoot.GetComponentsInChildren<UnityEngine.Transform>(true)) {
    t.gameObject.isStatic = false;
}

// -------------------------------------------------------------
// VERIFICATION SCREENSHOT OF REFINED STAIRS
// -------------------------------------------------------------
string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("VerificationCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.clearFlags = UnityEngine.CameraClearFlags.Skybox;
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 120f;
cam.fieldOfView = 60f;

var fillObj = new UnityEngine.GameObject("VerificationFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Directional;
fill.intensity = 1.0f;
fill.color = new UnityEngine.Color(1.0f, 0.96f, 0.92f);
fill.transform.rotation = UnityEngine.Quaternion.Euler(45f, -30f, 0f);

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

cam.transform.position = new UnityEngine.Vector3(1.5f, 24.2f, 5.0f);
cam.transform.LookAt(new UnityEngine.Vector3(1.5f, 21.5f, 10.5f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_refined_stairs.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Refined stairs landing executed!";
