var sb = new System.Text.StringBuilder();

System.Action<string> InspectGroup = (name) => {
    var go = UnityEngine.GameObject.Find(name);
    if (go == null) { sb.AppendLine("Not found: " + name); return; }
    sb.AppendLine("=== " + name + " ===");
    sb.AppendLine("Children count: " + go.transform.childCount);
    for (int i = 0; i < go.transform.childCount; i++) {
        var child = go.transform.GetChild(i);
        var col = child.GetComponent<UnityEngine.Collider>();
        var mf = child.GetComponent<UnityEngine.MeshFilter>();
        sb.AppendLine("  [" + i + "] " + child.name + 
            " | pos: " + child.localPosition.ToString("F2") + 
            " | Col: " + (col != null ? col.GetType().Name : "none") + 
            " | Mesh: " + (mf != null && mf.sharedMesh != null ? mf.sharedMesh.name : "none"));
    }
};

InspectGroup("Bedroom 1 - bridal suite");
InspectGroup("Bedroom 2 - family room");

return sb.ToString();
