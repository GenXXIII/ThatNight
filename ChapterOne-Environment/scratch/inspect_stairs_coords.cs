var sb = new System.Text.StringBuilder();

// 1. Water jar and platform
foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    string n = go.name.ToLower();
    if (n.Contains("jar") || n.Contains("water") || n.Contains("platform") || n.Contains("stair") || n.Contains("tread") || n.Contains("step")) {
        if (go.transform.position.z > 5 && go.transform.position.z < 25 && Mathf.Abs(go.transform.position.x) < 10) {
            sb.AppendLine("Stair/Jar prop: " + go.name + " world=" + go.transform.position + " rot=" + go.transform.eulerAngles);
            var r = go.GetComponent<UnityEngine.Renderer>();
            if (r != null) sb.AppendLine("   bounds=" + r.bounds);
        }
    }
}

// 2. Middle roof poles
var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
sb.AppendLine("Wedding pos: " + wedding?.transform.position);

return sb.ToString();
