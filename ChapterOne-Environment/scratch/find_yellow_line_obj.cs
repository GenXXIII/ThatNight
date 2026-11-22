var sb = new System.Text.StringBuilder();

// Find any object with 'border' or 'gold' or 'piping' within 1.5m of (-0.88, 21.02, 5.29)
foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    float d = UnityEngine.Vector3.Distance(go.transform.position, new UnityEngine.Vector3(-0.88f, 21.02f, 5.29f));
    if (d < 3.0f) {
        sb.AppendLine("Near (-0.88, 21.02, 5.29): " + go.name + " pos=" + go.transform.position + " rot=" + go.transform.eulerAngles);
    }
}

return sb.ToString();
