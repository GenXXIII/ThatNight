var sb = new System.Text.StringBuilder();

System.Func<UnityEngine.Transform, string> getPath = null;
getPath = (t) => {
    if (t.parent == null) return t.name;
    return getPath(t.parent) + "/" + t.name;
};

foreach (var mr in UnityEngine.Object.FindObjectsByType<UnityEngine.MeshRenderer>(UnityEngine.FindObjectsSortMode.None)) {
    if (mr.sharedMaterial != null && (mr.sharedMaterial.name.ToLower().Contains("rose") || mr.sharedMaterial.name.ToLower().Contains("pink"))) {
        var mf = mr.GetComponent<UnityEngine.MeshFilter>();
        if (mf != null && mf.sharedMesh != null) {
            var verts = mf.sharedMesh.vertices;
            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;
            foreach (var v in verts) {
                var wv = mr.transform.TransformPoint(v);
                if (wv.x < minX) minX = wv.x;
                if (wv.x > maxX) maxX = wv.x;
                if (wv.z < minZ) minZ = wv.z;
                if (wv.z > maxZ) maxZ = wv.z;
            }
            sb.AppendLine("Pink obj: '" + mr.gameObject.name + "' path=" + getPath(mr.transform) + " X in [" + minX + ", " + maxX + "] Z in [" + minZ + ", " + maxZ + "]");
        }
    }
}

return sb.ToString();
