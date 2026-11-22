var sb = new System.Text.StringBuilder();

// Search all mesh filters in the scene for submeshes or bounds matching these 3 shelves
var allMF = UnityEngine.Object.FindObjectsByType<UnityEngine.MeshFilter>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
foreach (var mf in allMF) {
    if (mf.sharedMesh == null) continue;
    // Look at submeshes or bounds
    var b = mf.sharedMesh.bounds;
    // Check if this object is inside the house
    var worldCenter = mf.transform.TransformPoint(b.center);
    var localH = ThatNight.ChapterOne.SiteLayout.HouseLocal(worldCenter);
    if (Mathf.Abs(localH.x) < 8f && Mathf.Abs(localH.z) < 7f && localH.y > 2f) {
        sb.AppendLine("Obj: " + mf.gameObject.name + " | Parent: " + (mf.transform.parent != null ? mf.transform.parent.name : "null") + 
            " | LocalH: " + localH.ToString("F2") + " | Bounds: " + b.size.ToString("F2"));
    }
}

return sb.ToString();
