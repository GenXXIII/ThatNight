var sb = new System.Text.StringBuilder();

var stairs = UnityEngine.GameObject.Find("Wooden entrance staircase");
if (stairs != null) {
    sb.AppendLine("Stairs pos: " + stairs.transform.position + " rot=" + stairs.transform.eulerAngles);
    var mf = stairs.GetComponent<UnityEngine.MeshFilter>();
    if (mf != null && mf.sharedMesh != null) {
        var verts = mf.sharedMesh.vertices;
        // Transform vertices to world space
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        UnityEngine.Vector3 lowestZVert = UnityEngine.Vector3.zero;
        foreach (var v in verts) {
            var wv = stairs.transform.TransformPoint(v);
            if (wv.z < minZ) {
                minZ = wv.z;
                lowestZVert = wv;
            }
            if (wv.z > maxZ) maxZ = wv.z;
        }
        sb.AppendLine("Stairs World Z range: [" + minZ + ", " + maxZ + "]");
        sb.AppendLine("Lowest Z vertex: " + lowestZVert);
    }
}

var jar = UnityEngine.GameObject.Find("Large earthen water jar");
foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    if (go.name.Contains("jar") && go.transform.position.z > 5) {
        sb.AppendLine("Jar pos: " + go.transform.position + " bounds=" + go.GetComponent<UnityEngine.Renderer>()?.bounds);
    }
    if (go.name.Contains("platform") && go.transform.position.z > 5) {
        sb.AppendLine("Platform pos: " + go.transform.position + " bounds=" + go.GetComponent<UnityEngine.Renderer>()?.bounds);
    }
}

return sb.ToString();
