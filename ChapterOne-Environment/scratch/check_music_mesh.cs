var go = UnityEngine.GameObject.Find("Traditional Cambodian wedding music ensemble - Pinpeat");
if (go != null) {
    var mf = go.GetComponent<UnityEngine.MeshFilter>();
    var verts = mf.sharedMesh.vertices;
    float minZ = float.MaxValue, maxZ = float.MinValue;
    float minX = float.MaxValue, maxX = float.MinValue;
    foreach (var v in verts) {
        var wv = go.transform.TransformPoint(v);
        if (wv.z < minZ) minZ = wv.z;
        if (wv.z > maxZ) maxZ = wv.z;
        if (wv.x < minX) minX = wv.x;
        if (wv.x > maxX) maxX = wv.x;
    }
    return "Music ensemble mesh: X in [" + minX + ", " + maxX + "] Z in [" + minZ + ", " + maxZ + "]";
}
return "Not found";
