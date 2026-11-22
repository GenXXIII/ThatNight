var sb = new System.Text.StringBuilder();

var ceremony = UnityEngine.GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
if (ceremony != null) {
    sb.AppendLine("Ceremony pos=" + ceremony.transform.position);
    foreach (var r in ceremony.GetComponentsInChildren<UnityEngine.Renderer>()) {
        if (r.name.ToLower().Contains("stage") || r.name.ToLower().Contains("platform") || r.name.ToLower().Contains("carpet") || r.name.ToLower().Contains("rug")) {
            sb.AppendLine("  Ceremony part: " + r.name + " bounds=" + r.bounds);
        }
    }
}

return sb.ToString();
