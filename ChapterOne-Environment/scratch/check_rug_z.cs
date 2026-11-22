var sb = new System.Text.StringBuilder();

var go = UnityEngine.GameObject.Find("Traditional Cambodian wedding music ensemble - Pinpeat");
if (go != null) {
    var mf = go.GetComponent<UnityEngine.MeshFilter>();
    var verts = mf.sharedMesh.vertices;
    // Find all vertices with Y close to ground (the rug vertices)
    float minRugZ = float.MaxValue;
    float maxRugZ = float.MinValue;
    foreach (var v in verts) {
        var wv = go.transform.TransformPoint(v);
        if (wv.y < 21.1f) {
            if (wv.z < minRugZ) minRugZ = wv.z;
            if (wv.z > maxRugZ) maxRugZ = wv.z;
        }
    }
    sb.AppendLine("Rug World Z: min=" + minRugZ + " max=" + maxRugZ);
}

return sb.ToString();
