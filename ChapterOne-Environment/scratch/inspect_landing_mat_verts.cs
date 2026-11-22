var sb = new System.Text.StringBuilder();

var go = UnityEngine.GameObject.Find("House stairs and water jar landing mat");
if (go != null) {
    var mf = go.GetComponent<UnityEngine.MeshFilter>();
    var verts = mf.sharedMesh.vertices;
    sb.AppendLine("House stairs and water jar landing mat vertex count: " + verts.Length);
    foreach (var v in verts) {
        var wv = go.transform.TransformPoint(v);
        sb.AppendLine("  Local: " + v + " -> World: " + wv);
    }
}

return sb.ToString();
