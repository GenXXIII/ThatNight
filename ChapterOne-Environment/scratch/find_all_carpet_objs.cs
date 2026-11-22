var sb = new System.Text.StringBuilder();

foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    if (go.name.ToLower().Contains("carpet") || go.name.ToLower().Contains("promenade") || go.name.ToLower().Contains("landing") || go.name.ToLower().Contains("aisle")) {
        var r = go.GetComponent<UnityEngine.Renderer>();
        if (r != null) {
            sb.AppendLine("Obj: " + go.name + " parent=" + go.transform.parent?.name + " bounds=" + r.bounds);
        }
    }
}

return sb.ToString();
