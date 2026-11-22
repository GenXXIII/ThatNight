var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
var roots = scene.GetRootGameObjects();
var sb = new System.Text.StringBuilder();
sb.AppendLine("Scene: " + scene.name + " (" + roots.Length + " roots)");
foreach (var r in roots) {
    sb.AppendLine("- " + r.name + " (children: " + r.transform.childCount + ")");
}
return sb.ToString();
