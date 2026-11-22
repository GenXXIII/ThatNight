var sb = new System.Text.StringBuilder();

var go = UnityEngine.GameObject.Find("Wedding open pavilion decorations and ceremonial arch");
if (go != null) {
    sb.AppendLine("Found 'Wedding open pavilion decorations and ceremonial arch'");
    foreach (UnityEngine.Transform t in go.transform) {
        sb.AppendLine("  Child: " + t.name + " pos=" + t.position);
    }
}

// Also let's check ALL objects with 'guy', 'rope', 'tie', 'brace', 'anchor', 'pole', 'post' in their name in the whole scene!
foreach (var g in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    string n = g.name.ToLower();
    if (n.Contains("guy") || n.Contains("rope") || n.Contains("brace") || n.Contains("anchor") || n.Contains("cable") || n.Contains("stay")) {
        sb.AppendLine("Cable/Rope/Brace: " + g.name + " at " + g.transform.position);
    }
}

return sb.ToString();
