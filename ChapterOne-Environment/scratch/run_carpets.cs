ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding == null) return "No wedding root";

// Remove old entrance carpet
var gate = UnityEngine.GameObject.Find("Realistic Cambodian wedding entrance gate - Khlok Pka Sla");
if (gate != null) {
    var oldEntCarpet = gate.transform.Find("Entrance red carpet runner aisle");
    if (oldEntCarpet != null) UnityEngine.Object.DestroyImmediate(oldEntCarpet.gameObject);
}

// Remove old stage aisle carpet
var gaatSah = UnityEngine.GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
if (gaatSah != null) {
    var oldStageCarpet = gaatSah.transform.Find("Ceremonial hair cutting center aisle carpet");
    if (oldStageCarpet != null) UnityEngine.Object.DestroyImmediate(oldStageCarpet.gameObject);
}

// Remove existing unified carpet group if any
var oldUnified = UnityEngine.GameObject.Find("Ceremonial Wedding Carpet Network");
if (oldUnified != null) UnityEngine.Object.DestroyImmediate(oldUnified);

var carpetRoot = new UnityEngine.GameObject("Ceremonial Wedding Carpet Network");
carpetRoot.transform.SetParent(wedding.transform, false);
carpetRoot.transform.localPosition = UnityEngine.Vector3.zero;
carpetRoot.isStatic = false;

var pinkMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Pink;
var goldSilk = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Carpet gold piping", new UnityEngine.Color(.82f, .65f, .18f), .45f, .25f);

ThatNight.ChapterOne.Editor.MeshFactory.Parent = carpetRoot.transform;

float groundY = 0.018f; // local Y inside wedding (wedding is at hill elevation 21m)

// -------------------------------------------------------------
// 1. MAIN PROMENADE: ENTRANCE GATE (Z = -18.0) TO HOUSE STAIRS (Z = 10.0, X = 1.82)
// -------------------------------------------------------------
float mainWidth = 2.0f;
int mainSteps = 60;
ThatNight.ChapterOne.Editor.MeshFactory.Grid("Main entrance to house carpet", 8, mainSteps, (u, v) =>
{
    float z = UnityEngine.Mathf.Lerp(-18.0f, 10.0f, v);
    // Smooth curve from X = 0 to X = 1.82 near the house (v > 0.70, which is Z > 1.6)
    float tCurve = UnityEngine.Mathf.Clamp01((v - 0.70f) / 0.30f);
    float smoothT = tCurve * tCurve * (3f - 2f * tCurve);
    float centerX = UnityEngine.Mathf.Lerp(0f, 1.82f, smoothT);

    float dx_dz = (tCurve > 0 && tCurve < 1) ? (1.82f * 6f * tCurve * (1f - tCurve) / 8.4f) : 0f;
    UnityEngine.Vector2 fwd = new UnityEngine.Vector2(dx_dz, 1f).normalized;
    UnityEngine.Vector2 norm = new UnityEngine.Vector2(fwd.y, -fwd.x);

    float offset = (u - 0.5f) * mainWidth;
    return new UnityEngine.Vector3(centerX + norm.x * offset, groundY, z + norm.y * offset);
}, pinkMat, new UnityEngine.Vector2(2, 16), true);

// Gold border piping along main promenade
for (int side = -1; side <= 1; side += 2)
{
    int s = side;
    ThatNight.ChapterOne.Editor.MeshFactory.Grid("Main carpet gold border " + (s < 0 ? "L" : "R"), 2, mainSteps, (u, v) =>
    {
        float z = UnityEngine.Mathf.Lerp(-18.0f, 10.0f, v);
        float tCurve = UnityEngine.Mathf.Clamp01((v - 0.70f) / 0.30f);
        float smoothT = tCurve * tCurve * (3f - 2f * tCurve);
        float centerX = UnityEngine.Mathf.Lerp(0f, 1.82f, smoothT);

        float dx_dz = (tCurve > 0 && tCurve < 1) ? (1.82f * 6f * tCurve * (1f - tCurve) / 8.4f) : 0f;
        UnityEngine.Vector2 fwd = new UnityEngine.Vector2(dx_dz, 1f).normalized;
        UnityEngine.Vector2 norm = new UnityEngine.Vector2(fwd.y, -fwd.x);

        float offset = (s * 0.5f * mainWidth) + (u - 0.5f) * 0.05f;
        return new UnityEngine.Vector3(centerX + norm.x * offset, groundY + 0.002f, z + norm.y * offset);
    }, goldSilk, new UnityEngine.Vector2(1, 16), true);
}

// -------------------------------------------------------------
// 2. CONNECTING CURVED BRANCH: MAIN WALKWAY (0, -12.2) -> STAGE AISLE (13.2, -10.5)
// -------------------------------------------------------------
UnityEngine.Vector2 p0 = new UnityEngine.Vector2(0f, -12.2f);
UnityEngine.Vector2 p1 = new UnityEngine.Vector2(5.5f, -12.4f);
UnityEngine.Vector2 p2 = new UnityEngine.Vector2(10.5f, -11.8f);
UnityEngine.Vector2 p3 = new UnityEngine.Vector2(13.2f, -10.5f);

float branchWidth = 1.8f;
int branchSteps = 36;

ThatNight.ChapterOne.Editor.MeshFactory.Grid("Connecting cross-aisle carpet to stage", 8, branchSteps, (u, v) =>
{
    float t = v;
    float omt = 1f - t;
    UnityEngine.Vector2 pos = omt * omt * omt * p0 + 3f * omt * omt * t * p1 + 3f * omt * t * t * p2 + t * t * t * p3;
    UnityEngine.Vector2 tang = 3f * omt * omt * (p1 - p0) + 6f * omt * t * (p2 - p1) + 3f * t * t * (p3 - p2);
    UnityEngine.Vector2 norm = new UnityEngine.Vector2(-tang.y, tang.x).normalized;

    float offset = (u - 0.5f) * branchWidth;
    return new UnityEngine.Vector3(pos.x + norm.x * offset, groundY + 0.001f, pos.y + norm.y * offset);
}, pinkMat, new UnityEngine.Vector2(2, 8), true);

// Gold border piping along connecting branch
for (int side = -1; side <= 1; side += 2)
{
    int s = side;
    ThatNight.ChapterOne.Editor.MeshFactory.Grid("Branch gold border " + (s < 0 ? "L" : "R"), 2, branchSteps, (u, v) =>
    {
        float t = v;
        float omt = 1f - t;
        UnityEngine.Vector2 pos = omt * omt * omt * p0 + 3f * omt * omt * t * p1 + 3f * omt * t * t * p2 + t * t * t * p3;
        UnityEngine.Vector2 tang = 3f * omt * omt * (p1 - p0) + 6f * omt * t * (p2 - p1) + 3f * t * t * (p3 - p2);
        UnityEngine.Vector2 norm = new UnityEngine.Vector2(-tang.y, tang.x).normalized;

        float offset = (s * 0.5f * branchWidth) + (u - 0.5f) * 0.05f;
        return new UnityEngine.Vector3(pos.x + norm.x * offset, groundY + 0.003f, pos.y + norm.y * offset);
    }, goldSilk, new UnityEngine.Vector2(1, 8), true);
}

// -------------------------------------------------------------
// 3. CEREMONIAL STAGE AISLE: (13.2, -10.5) UP TO STAGE FRONT STEPS (13.2, -2.0)
// -------------------------------------------------------------
float stageAisleWidth = 1.6f;
int stageSteps = 28;
ThatNight.ChapterOne.Editor.MeshFactory.Grid("Ceremonial stage center aisle carpet", 6, stageSteps, (u, v) =>
{
    float z = UnityEngine.Mathf.Lerp(-10.5f, -2.0f, v);
    float offset = (u - 0.5f) * stageAisleWidth;
    return new UnityEngine.Vector3(13.2f + offset, groundY + 0.0015f, z);
}, pinkMat, new UnityEngine.Vector2(2, 8), true);

// Gold border piping along stage aisle
for (int side = -1; side <= 1; side += 2)
{
    int s = side;
    ThatNight.ChapterOne.Editor.MeshFactory.Grid("Stage aisle gold border " + (s < 0 ? "L" : "R"), 2, stageSteps, (u, v) =>
    {
        float z = UnityEngine.Mathf.Lerp(-10.5f, -2.0f, v);
        float offset = (s * 0.5f * stageAisleWidth) + (u - 0.5f) * 0.05f;
        return new UnityEngine.Vector3(13.2f + offset, groundY + 0.0035f, z);
    }, goldSilk, new UnityEngine.Vector2(1, 8), true);
}

// -------------------------------------------------------------
// 4. STAGE FRONT STEP LANDING RUNNER
// -------------------------------------------------------------
ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage entrance greeting landing", new UnityEngine.Vector3(13.2f, groundY + 0.002f, -2.0f), new UnityEngine.Vector3(2.4f, 0.01f, 0.9f), pinkMat);

// -------------------------------------------------------------
// 5. HOUSE STAIRCASE LANDING RUNNER (Centered directly in front of stairs at X = 1.82)
// -------------------------------------------------------------
ThatNight.ChapterOne.Editor.MeshFactory.Box("House staircase bottom landing", new UnityEngine.Vector3(1.82f, groundY + 0.002f, 10.0f), new UnityEngine.Vector3(2.4f, 0.01f, 1.4f), pinkMat);

ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

// Ensure all objects are non-static
foreach (var t in carpetRoot.GetComponentsInChildren<UnityEngine.Transform>(true)) {
    t.gameObject.isStatic = false;
}

// -------------------------------------------------------------
// 6. SCREENSHOTS FOR VERIFICATION
// -------------------------------------------------------------
string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("CarpetCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.clearFlags = UnityEngine.CameraClearFlags.Skybox;
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 120f;
cam.fieldOfView = 65f;

var fillObj = new UnityEngine.GameObject("CarpetFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Directional;
fill.intensity = 1.0f;
fill.color = new UnityEngine.Color(1.0f, 0.96f, 0.92f);
fill.transform.rotation = UnityEngine.Quaternion.Euler(45f, -30f, 0f);

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// Shot 1: Perspective from Entrance Gate looking down the carpet to House and Stage branch
cam.transform.position = new UnityEngine.Vector3(0f, 23.2f, -17.5f);
cam.transform.LookAt(new UnityEngine.Vector3(2.5f, 21.4f, -4.0f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_from_entrance.png"), tex.EncodeToPNG());

// Shot 2: View of the House Staircase landing showing carpet centered squarely on the stairs
cam.transform.position = new UnityEngine.Vector3(1.82f, 24.5f, 5.0f);
cam.transform.LookAt(new UnityEngine.Vector3(1.82f, 21.5f, 10.5f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_stairs_landing.png"), tex.EncodeToPNG());

// Shot 3: Ceremony Stage and Audience view showing the branch connecting into the stage aisle
cam.transform.position = new UnityEngine.Vector3(8.0f, 24.5f, -14.0f);
cam.transform.LookAt(new UnityEngine.Vector3(13.2f, 21.8f, -4.0f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_stage_connection.png"), tex.EncodeToPNG());

// Shot 4: Broad overview under the tent roof
cam.transform.position = new UnityEngine.Vector3(6.0f, 25.5f, -16.0f);
cam.transform.LookAt(new UnityEngine.Vector3(6.0f, 21.0f, 0.0f));
cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_network_overview.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Successfully built connected ceremonial carpet network with Dusty Rose wedding cloth!";
