var chairs = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.GameObject>();
var sb = new System.Text.StringBuilder();
int chairCount = 0;
foreach (var go in chairs) {
    if (go.name.Contains("chair") || go.name.Contains("Chair")) {
        chairCount++;
        if (chairCount <= 10) {
            sb.AppendLine(go.name + " | parent: " + (go.transform.parent != null ? go.transform.parent.name : "root") + " | pos: " + go.transform.position);
        }
    }
}
sb.AppendLine("Total chairs found: " + chairCount);
return sb.ToString();
