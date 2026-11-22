ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding == null) return "No wedding root";

// Rebuild Ceremonial Wedding Carpet Network
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

// -------------------------------------------------------------
// PINK CARPET MESHES (Strictly inside the gold border perimeter)
// -------------------------------------------------------------

// 1A. Main Promenade: From Z = -18.0 up to Z = 8.0 (Stroke 1 horizontal line)
// Width: X in [-1.0, 1.0]. Inset by 1.5cm on the outer edges so gold piping caps it completely!
ThatNight.ChapterOne.Editor.MeshFactory.Box("Main promenade", 
    new UnityEngine.Vector3(0f, groundY, -5.0f), new UnityEngine.Vector3(1.97f, 0.008f, 25.97f), pinkMat);

// 1B. House Stairs & Water Jar Approach Corridor (Between Stroke 1 and Stroke 2):
// Runs East from the promenade (X = 0.98) to the water jar platform (X = 5.38), Z in [6.02, 7.98] (width 1.96m)
ThatNight.ChapterOne.Editor.MeshFactory.Box("House approach corridor", 
    new UnityEngine.Vector3(3.18f, groundY + 0.0005f, 7.00f), new UnityEngine.Vector3(4.40f, 0.008f, 1.96f), pinkMat);

// 1C. House Stairs & Water Jar Landing Mat (North of the corridor, in front of stairs & jar):
// From X = 0.90 to X = 5.38, Z from Z = 7.98 to Z = 10.18 (depth 2.20m)
// Notice: West of X = 0.90 at Z > 8.0 is 100% EMPTY DIRT (the top-left corner is cut off)!
ThatNight.ChapterOne.Editor.MeshFactory.Box("House stairs and water jar landing mat", 
    new UnityEngine.Vector3(3.14f, groundY + 0.001f, 9.08f), new UnityEngine.Vector3(4.48f, 0.009f, 2.20f), pinkMat);

// 1D. Cross-Walkway in Front of Music: From X = 0.98 to X = 13.2, Z in [-0.58, 1.18] (width 1.76m)
// Flush in front of the music ensemble rug at Z = 1.20!
ThatNight.ChapterOne.Editor.MeshFactory.Box("Cross walkway in front of music", 
    new UnityEngine.Vector3(7.1f, groundY + 0.0005f, 0.30f), new UnityEngine.Vector3(12.2f, 0.008f, 1.76f), pinkMat);

// 1E. Stage Center Audience Aisle: From Z = -0.58 down to Z = -9.78 at X = 13.2, width 1.56m (X in [12.42, 13.98])
ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage center audience aisle", 
    new UnityEngine.Vector3(13.2f, groundY + 0.0002f, -5.18f), new UnityEngine.Vector3(1.56f, 0.008f, 9.20f), pinkMat);

// 1F. Stage Front Steps Landing Mat: At X = 13.2, Z in [1.18, 2.08] (width 2.36m)
ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage front steps landing mat", 
    new UnityEngine.Vector3(13.2f, groundY + 0.001f, 1.63f), new UnityEngine.Vector3(2.36f, 0.009f, 0.90f), pinkMat);

// -------------------------------------------------------------
// CONTINUOUS PERIMETER GOLD PIPING (Zero pink outside)
// -------------------------------------------------------------
float pipeWidth = 0.055f;
float pipeHeight = 0.007f;

System.Action<string, UnityEngine.Vector3[]> drawBorder = (name, pts) =>
{
    if (pts.Length < 2) return;
    for (int i = 0; i < pts.Length - 1; i++)
    {
        UnityEngine.Vector3 pA = pts[i];
        UnityEngine.Vector3 pB = pts[i + 1];
        UnityEngine.Vector3 dir = (pB - pA).normalized;
        UnityEngine.Vector3 center = (pA + pB) * 0.5f;
        float len = UnityEngine.Vector3.Distance(pA, pB) + pipeWidth; // slight extension at corners to prevent any pink corner exposure
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(dir, UnityEngine.Vector3.up);

        var beam = ThatNight.ChapterOne.Editor.MeshFactory.Box(name + "_" + i, center + UnityEngine.Vector3.up * 0.004f, new UnityEngine.Vector3(pipeWidth, pipeHeight, len), goldSilk);
        beam.transform.localRotation = rot;
    }
};

// BORDER 1: Main promenade west edge -> Stroke 1 (turns east across cut-off at Z = 8.0 to X = 0.88) -> turns north along stairs to Z = 10.2 -> turns east across bottom of stairs & water jar platform to X = 5.4 -> Stroke 2 (turns south along water jar to Z = 6.0 -> turns west along south of corridor to X = 1.0) -> south down promenade east edge to cross-walkway at Z = 1.20
drawBorder("WestBorderAndHouseLanding", new[]{
    new UnityEngine.Vector3(-1.0f, groundY, -18.0f),
    new UnityEngine.Vector3(-1.0f, groundY, 8.0f),      // West edge of promenade ends at Z = 8.0
    new UnityEngine.Vector3(0.88f, groundY, 8.0f),      // STROKE 1: Horizontal run east to bottom of stairs
    new UnityEngine.Vector3(0.88f, groundY, 10.2f),     // STROKE 1: Turns north to meet bottom-left staircase post!
    new UnityEngine.Vector3(5.40f, groundY, 10.2f),     // Across bottom of stairs & water jar platform!
    new UnityEngine.Vector3(5.40f, groundY, 6.0f),      // STROKE 2: Turns south down east edge of water jar
    new UnityEngine.Vector3(1.00f, groundY, 6.0f),      // STROKE 2: Horizontal run west back to promenade east edge!
    new UnityEngine.Vector3(1.00f, groundY, 1.20f)      // South along promenade east edge to north of cross-walkway
});

// BORDER 2: North edge of cross-walkway in front of music ensemble -> Stage front landing -> East edge of audience aisle -> South hem of audience aisle
drawBorder("NorthMusicAndStageBorder", new[]{
    new UnityEngine.Vector3(1.00f, groundY, 1.20f),     // meets BORDER 1 cleanly at (1.0, 1.20)
    new UnityEngine.Vector3(12.0f, groundY, 1.20f),     // flush in front of music ensemble
    new UnityEngine.Vector3(12.0f, groundY, 2.10f),     // west edge of stage landing
    new UnityEngine.Vector3(14.4f, groundY, 2.10f),     // north edge of stage landing
    new UnityEngine.Vector3(14.4f, groundY, 1.20f),     // east edge of stage landing
    new UnityEngine.Vector3(14.0f, groundY, 1.20f),     // align with audience aisle
    new UnityEngine.Vector3(14.0f, groundY, -9.8f),     // east edge of audience aisle
    new UnityEngine.Vector3(12.4f, groundY, -9.8f)      // south hem of audience aisle
});

// BORDER 3: West edge of audience aisle -> South edge of cross-walkway -> East edge of promenade -> South entrance hem
drawBorder("SouthCrossAndAisleBorder", new[]{
    new UnityEngine.Vector3(12.4f, groundY, -9.8f),     // meets BORDER 2 at south hem
    new UnityEngine.Vector3(12.4f, groundY, -0.60f),    // north along west edge of audience aisle
    new UnityEngine.Vector3(1.00f, groundY, -0.60f),    // west along south edge of cross-walkway
    new UnityEngine.Vector3(1.00f, groundY, -18.0f),    // south along east edge of promenade
    new UnityEngine.Vector3(-1.0f, groundY, -18.0f)     // south entrance hem, meets BORDER 1!
});

ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

foreach (var t in carpetRoot.GetComponentsInChildren<UnityEngine.Transform>(true)) {
    t.gameObject.isStatic = false;
}

UnityEditor.EditorUtility.SetDirty(wedding);
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

return "Success: Applied precise user paint layout with zero pink outside yellow borders!";
