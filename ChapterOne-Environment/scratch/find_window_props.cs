var all = UnityEngine.Object.FindObjectsByType<UnityEngine.MeshRenderer>(UnityEngine.FindObjectsSortMode.None);
var sb = new System.Text.StringBuilder();
foreach (var mr in all) {
    var p = mr.transform.position;
    // Check near back wall of house: House is around Z=22, HillElevation=21
    // In world coords: SiteLayout.HousePoint(new Vector3(-3.75f, 2.5f, 4.55f))
    var local = ThatNight.ChapterOne.SiteLayout.HouseLocal(p);
    if (local.z > 3.0f && local.z < 7.0f) {
        sb.AppendLine(mr.gameObject.name + " (local: " + local.ToString("F2") + ", bounds size: " + mr.bounds.size.ToString("F2") + ")");
    }
}
return sb.ToString();
