var sb = new System.Text.StringBuilder();

// Check all mesh renderers that have vertices near X in [-2, 1], Z in [8, 12]
foreach (var mf in UnityEngine.Object.FindObjectsByType<UnityEngine.MeshFilter>(UnityEngine.FindObjectsSortMode.None)) {
    var mr = mf.GetComponent<UnityEngine.Renderer>();
    if (mr == null || !mr.enabled) continue;
    var mesh = mf.sharedMesh;
    if (mesh == null) continue;
    
    bool foundInZone = false;
    foreach (var v in mesh.vertices) {
        var wv = mf.transform.TransformPoint(v);
        if (wv.x >= -1.5f && wv.x <= 0.85f && wv.z >= 8.05f && wv.z <= 12.0f && Mathf.Abs(wv.y - 21.02f) < 0.2f) {
            foundInZone = true;
            break;
        }
    }
    if (foundInZone) {
        sb.AppendLine("FOUND OBJECT RENDERING IN TOP-LEFT ZONE: " + mf.gameObject.name + " parent=" + mf.transform.parent?.name + " mat=" + mr.sharedMaterial?.name);
    }
}

return sb.ToString();
