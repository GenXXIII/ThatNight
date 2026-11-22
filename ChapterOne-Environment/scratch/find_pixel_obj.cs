var sb = new System.Text.StringBuilder();

// Find any object whose renderer bounds contain (-1.71, 21.02, 5.02)
var target = new UnityEngine.Vector3(-1.71f, 21.02f, 5.02f);
foreach (var r in UnityEngine.Object.FindObjectsByType<UnityEngine.Renderer>(UnityEngine.FindObjectsSortMode.None)) {
    if (r.bounds.Contains(target)) {
        sb.AppendLine("Contains target: " + r.gameObject.name + " mat=" + r.sharedMaterial?.name);
    }
}

// Also check distance to all renderers
sb.AppendLine("Closest renderers to " + target + ":");
var list = new System.Collections.Generic.List<System.Tuple<float, UnityEngine.Renderer>>();
foreach (var r in UnityEngine.Object.FindObjectsByType<UnityEngine.Renderer>(UnityEngine.FindObjectsSortMode.None)) {
    float d = UnityEngine.Vector3.Distance(r.bounds.center, target);
    if (d < 10f) {
        list.Add(new System.Tuple<float, UnityEngine.Renderer>(d, r));
    }
}
list.Sort((a, b) => a.Item1.CompareTo(b.Item1));
foreach (var item in list) {
    sb.AppendLine("  d=" + item.Item1.ToString("F2") + " " + item.Item2.gameObject.name + " mat=" + item.Item2.sharedMaterial?.name + " bounds=" + item.Item2.bounds);
}

return sb.ToString();
