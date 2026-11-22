var sb = new System.Text.StringBuilder();

// Search for any MeshRenderer with vertices near the back wall windows
// Window 1: local (-4.2f, 1.6f, 6.25f)
// Window 2: local (4.2f, 1.6f, 6.25f)

var allMR = UnityEngine.Object.FindObjectsByType<UnityEngine.MeshRenderer>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
foreach (var mr in allMR) {
    if (mr.transform.parent != null && mr.transform.parent.name.Contains("RENDER BATCHES")) continue;
    var local = ThatNight.ChapterOne.SiteLayout.HouseLocal(mr.transform.position);
    // If it's in the bedroom back wall area
    if (local.z > 4.5f && local.z < 6.5f) {
        sb.AppendLine("Obj: " + mr.gameObject.name + 
            " | Parent: " + (mr.transform.parent != null ? mr.transform.parent.name : "null") + 
            " | Pos: " + local.ToString("F2") + 
            " | BoundsCenter: " + ThatNight.ChapterOne.SiteLayout.HouseLocal(mr.bounds.center).ToString("F2") + 
            " | BoundsSize: " + mr.bounds.size.ToString("F2"));
    }
}

return sb.ToString();
