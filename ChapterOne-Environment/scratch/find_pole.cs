var sb = new System.Text.StringBuilder();

System.Func<UnityEngine.Transform, string> getPath = null;
getPath = (t) => {
    if (t.parent == null) return t.name;
    return getPath(t.parent) + "/" + t.name;
};

// Search for any MeshFilter near X in [0, 8], Z in [-6, 2], Y in [20, 25] (world coords)
foreach (var mf in UnityEngine.Object.FindObjectsByType<UnityEngine.MeshFilter>(UnityEngine.FindObjectsSortMode.None)) {
    var rend = mf.GetComponent<UnityEngine.Renderer>();
    if (rend != null) {
        var b = rend.bounds;
        if (b.center.x > -2 && b.center.x < 10 && b.center.z > -6 && b.center.z < 6 && b.center.y > 20 && b.center.y < 26) {
            sb.AppendLine("MF: " + mf.gameObject.name + " path=" + getPath(mf.transform) + " bounds=" + b);
        }
    }
}

return sb.ToString();
