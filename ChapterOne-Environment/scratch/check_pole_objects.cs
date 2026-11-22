var sb = new System.Text.StringBuilder();

// Check all objects within 3m of (3.0, 21.0, -1.75)
foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    float d = UnityEngine.Vector3.Distance(go.transform.position, new UnityEngine.Vector3(3.0f, 21.0f, -1.75f));
    if (d < 4.0f) {
        sb.AppendLine("Near (3, 21, -1.75): " + go.name + " pos=" + go.transform.position + " rot=" + go.transform.eulerAngles);
    }
}

return sb.ToString();
