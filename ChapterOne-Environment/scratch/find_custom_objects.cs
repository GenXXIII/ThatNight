var allGO = UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
var sb = new System.Text.StringBuilder();

foreach (var go in allGO) {
    if (go.transform.parent != null && go.transform.parent.name.Contains("RENDER BATCHES")) continue;
    // Look for anything named "shelf", "plank", "board", "cube", "GameObject"
    string n = go.name.ToLower();
    if (n.Contains("shelf") || n.Contains("cube") || n.Contains("plank") || n.Contains("board") || n.Contains("custom") || n.Contains("window")) {
        sb.AppendLine(go.name + " (parent: " + (go.transform.parent != null ? go.transform.parent.name : "null") + ", pos: " + go.transform.position + ")");
    }
}

return sb.ToString();
