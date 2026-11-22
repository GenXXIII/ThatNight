var sb = new System.Text.StringBuilder();

var sel = UnityEditor.Selection.activeGameObject;
sb.AppendLine("Current Selection: " + (sel != null ? sel.name : "null"));
if (sel != null) {
    var p = sel.transform;
    string path = p.name;
    while (p.parent != null) {
        p = p.parent;
        path = p.name + "/" + path;
    }
    sb.AppendLine("  Hierarchy path: " + path);
    sb.AppendLine("  Local pos: " + sel.transform.localPosition + ", World pos: " + sel.transform.position);
    var colliders = sel.GetComponents<UnityEngine.Collider>();
    sb.AppendLine("  Colliders count: " + colliders.Length);
    foreach (var c in colliders) sb.AppendLine("    " + c.GetType().Name + " bounds: " + c.bounds);
}

return sb.ToString();
