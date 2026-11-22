var sb = new System.Text.StringBuilder();

// Find posts near X in [-2, 4], Z in [8, 12]
foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    if (go.transform.position.z > 7 && go.transform.position.z < 12 && Mathf.Abs(go.transform.position.x) < 5) {
        if (go.name.ToLower().Contains("post") || go.name.ToLower().Contains("pillar") || go.name.ToLower().Contains("column") || go.name.ToLower().Contains("stilt")) {
            sb.AppendLine("House post: " + go.name + " world=" + go.transform.position);
        }
    }
}

return sb.ToString();
