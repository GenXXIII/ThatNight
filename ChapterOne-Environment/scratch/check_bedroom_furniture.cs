var b1 = UnityEngine.GameObject.Find("Bedroom 1 - bridal suite");
var b2 = UnityEngine.GameObject.Find("Bedroom 2 - family room");
var sb = new System.Text.StringBuilder();

System.Action<UnityEngine.GameObject> CheckGroup = (g) => {
    if (g == null) return;
    sb.AppendLine("=== " + g.name + " ===");
    foreach (UnityEngine.Transform t in g.transform) {
        var col = t.GetComponent<UnityEngine.Collider>();
        var rend = t.GetComponent<UnityEngine.MeshRenderer>();
        sb.AppendLine("  Object: " + t.name + " | Renderer: " + (rend != null && rend.enabled) + " | Collider: " + (col != null ? col.GetType().Name : "NONE"));
    }
};

CheckGroup(b1);
CheckGroup(b2);

return sb.ToString();
