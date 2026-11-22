var sb = new System.Text.StringBuilder();

var ceremony = UnityEngine.GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
if (ceremony != null) {
    foreach (var r in ceremony.GetComponentsInChildren<UnityEngine.Renderer>()) {
        sb.AppendLine("Ceremony: " + r.gameObject.name + " bounds=" + r.bounds);
    }
}

return sb.ToString();
