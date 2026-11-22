var sb = new System.Text.StringBuilder();

// Search for any renderer in the entire scene with a material named 'pink' or 'rose' or 'cloth'
foreach (var r in UnityEngine.Object.FindObjectsByType<UnityEngine.Renderer>(UnityEngine.FindObjectsSortMode.None)) {
    if (r.sharedMaterial != null && (r.sharedMaterial.name.ToLower().Contains("pink") || r.sharedMaterial.name.ToLower().Contains("rose"))) {
        sb.AppendLine("Pink renderer: " + r.gameObject.name + " parent=" + r.transform.parent?.name + " bounds=" + r.bounds);
    }
}

return sb.ToString();
