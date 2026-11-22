var sb = new System.Text.StringBuilder();

// Look for what was captured in media_1795147640074_30f16718.jpg
// In that image, we see a window cutout and 3 horizontal planks across it.
// Let's check all MeshRenderers in the house.
var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house != null) {
    var mrs = house.GetComponentsInChildren<UnityEngine.MeshRenderer>(true);
    foreach (var mr in mrs) {
        var mf = mr.GetComponent<UnityEngine.MeshFilter>();
        if (mf == null || mf.sharedMesh == null) continue;
        var mesh = mf.sharedMesh;
        // Check if mesh name or object name has "shelf" or "wall" or "cupboard"
        if (mr.name.Contains("cupboard") || mr.name.Contains("wall") || mr.name.Contains("window") || mr.name.Contains("plank")) {
            sb.AppendLine("MR: " + mr.name + " | Parent: " + mr.transform.parent.name + " | LocalPos: " + mr.transform.localPosition);
        }
    }
}

return sb.ToString();
