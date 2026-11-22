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

// 1A. Main Promenade: Entrance Gate (Z = -18.0) to Z = 8.0, width 2.0m (X in [-1.0, 1.0])
ThatNight.ChapterOne.Editor.MeshFactory.Box("Main promenade", 
    new UnityEngine.Vector3(0, groundY, -5.0f), new UnityEngine.Vector3(2.0f, 0.008f, 26.0f), pinkMat);

// 1B. House Stairs & Water Jar Landing Mat: X in [0.85, 5.4], Z in [8.0, 10.4] (width 4.55m, depth 2.4m)
// Seamlessly covers the bottom of the house stairs and the traditional foot washing station!
ThatNight.ChapterOne.Editor.MeshFactory.Box("House stairs and water jar landing mat", 
    new UnityEngine.Vector3(3.125f, groundY + 0.001f, 9.20f), new UnityEngine.Vector3(4.55f, 0.009f, 2.4f), pinkMat);

// 1C. Cross-Walkway in Front of Music: From X = 1.0 to X = 13.2, Z in [-0.60, 1.20] (width 1.8m)
// Flush with front edge of the traditional music ensemble rug at Z = 1.20!
ThatNight.ChapterOne.Editor.MeshFactory.Box("Cross walkway in front of music", 
    new UnityEngine.Vector3(7.1f, groundY + 0.0005f, 0.30f), new UnityEngine.Vector3(12.2f, 0.008f, 1.8f), pinkMat);

// 1D. Stage Center Audience Aisle: From Z = -0.60 down to Z = -9.8 at X = 13.2, width 1.6m (X in [12.4, 14.0])
ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage center audience aisle", 
    new UnityEngine.Vector3(13.2f, groundY + 0.0002f, -5.20f), new UnityEngine.Vector3(1.6f, 0.008f, 9.2f), pinkMat);

// 1E. Stage Front Steps Landing Mat: At X = 13.2, Z in [1.20, 2.10]
ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage front steps landing mat", 
    new UnityEngine.Vector3(13.2f, groundY + 0.001f, 1.65f), new UnityEngine.Vector3(2.4f, 0.009f, 0.9f), pinkMat);

// -------------------------------------------------------------
// CONTINUOUS PERIMETER GOLD PIPING
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

// BORDER 1: West edge from entrance up to Z = 8.0 -> turns east across cut-off to X = 0.85 -> north to stairs base (Z = 10.4) -> east across stairs & water jar (to X = 5.4) -> south down east edge of landing (to Z = 8.0) -> west along south edge of landing (to X = 1.0) -> south down promenade east edge to cross-walkway (Z = 1.20)
drawBorder("WestBorderAndHouseLanding", new[]{
    new UnityEngine.Vector3(-1.0f, groundY, -18.0f),
    new UnityEngine.Vector3(-1.0f, groundY, 8.0f),
    new UnityEngine.Vector3(0.85f, groundY, 8.0f),
    new UnityEngine.Vector3(0.85f, groundY, 10.4f),  // meets bottom-left staircase post!
    new UnityEngine.Vector3(5.40f, groundY, 10.4f),  // across bottom of stairs and water jar platform!
    new UnityEngine.Vector3(5.40f, groundY, 8.0f),   // down east edge of landing
    new UnityEngine.Vector3(1.00f, groundY, 8.0f),   // west back to main promenade edge
    new UnityEngine.Vector3(1.00f, groundY, 1.20f)   // south down promenade east edge to cross-walkway
});

// BORDER 2: North edge of cross-walkway in front of music ensemble -> Stage front landing -> East edge of audience aisle -> South hem of audience aisle
drawBorder("NorthMusicAndStageBorder", new[]{
    new UnityEngine.Vector3(1.00f, groundY, 1.20f),  // meets BORDER 1 cleanly at (1.0, 1.20)
    new UnityEngine.Vector3(12.0f, groundY, 1.20f),  // flush in front of music ensemble
    new UnityEngine.Vector3(12.0f, groundY, 2.10f),  // west edge of stage landing
    new UnityEngine.Vector3(14.4f, groundY, 2.10f),  // north edge of stage landing
    new UnityEngine.Vector3(14.4f, groundY, 1.20f),  // east edge of stage landing
    new UnityEngine.Vector3(14.0f, groundY, 1.20f),  // align with audience aisle
    new UnityEngine.Vector3(14.0f, groundY, -9.8f),  // east edge of audience aisle
    new UnityEngine.Vector3(12.4f, groundY, -9.8f)   // south hem of audience aisle
});

// BORDER 3: West edge of audience aisle -> South edge of cross-walkway -> East edge of promenade -> South entrance hem
drawBorder("SouthCrossAndAisleBorder", new[]{
    new UnityEngine.Vector3(12.4f, groundY, -9.8f),  // meets BORDER 2 at south hem
    new UnityEngine.Vector3(12.4f, groundY, -0.60f), // north along west edge of audience aisle
    new UnityEngine.Vector3(1.00f, groundY, -0.60f), // west along south edge of cross-walkway
    new UnityEngine.Vector3(1.00f, groundY, -18.0f), // south along east edge of promenade
    new UnityEngine.Vector3(-1.0f, groundY, -18.0f)  // south entrance hem, meets BORDER 1!
});

ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

foreach (var t in carpetRoot.GetComponentsInChildren<UnityEngine.Transform>(true)) {
    t.gameObject.isStatic = false;
}

UnityEditor.EditorUtility.SetDirty(wedding);
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

return "Success: Applied perfect paint layout!";
