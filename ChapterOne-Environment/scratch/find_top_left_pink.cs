var sb = new System.Text.StringBuilder();

foreach (var mf in UnityEngine.Object.FindObjectsByType<UnityEngine.MeshFilter>(UnityEngine.FindObjectsSortMode.None)) {
    var r = mf.GetComponent<UnityEngine.Renderer>();
    if (r == null || !r.enabled) continue;
    var b = r.bounds;
    // Check if bounds overlaps X in [-2, 0.85] and Z in [8.0, 12.0]
    if (b.min.x < 0.85f && b.max.x > -2.0f && b.min.z < 12.0f && b.max.z > 8.0f && b.center.y < 22.0f) {
        sb.AppendLine("Mesh in top-left region: " + mf.gameObject.name + " mat=" + r.sharedMaterial?.name + " bounds=" + b);
    }
}

return sb.ToString();
