var sb = new System.Text.StringBuilder();

System.Func<UnityEngine.Transform, string> getPath = null;
getPath = (t) => {
    if (t.parent == null) return t.name;
    return getPath(t.parent) + "/" + t.name;
};

foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    var mf = go.GetComponent<UnityEngine.MeshFilter>();
    var mr = go.GetComponent<UnityEngine.MeshRenderer>();
    if (mf != null && mr != null && mr.enabled) {
        var b = mr.bounds;
        if (b.center.x >= -2f && b.center.x <= 2f && b.center.z >= 5f && b.center.z <= 12f && b.center.y >= 20.5f && b.center.y <= 21.5f) {
            sb.AppendLine("GO: '" + go.name + "' path=" + getPath(go.transform) + " bounds=" + b + " mat=" + mr.sharedMaterial?.name);
        }
    }
}

return sb.ToString();
