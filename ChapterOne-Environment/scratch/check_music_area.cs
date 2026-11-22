var zone = UnityEngine.GameObject.Find("02 WEDDING YARD - banquet and marquee");
if (zone == null) return "No wedding yard found";

var sb = new System.Text.StringBuilder();
foreach (UnityEngine.Transform t in zone.GetComponentsInChildren<UnityEngine.Transform>()) {
    if (t.name.Contains("music") || t.name.Contains("Music") || t.name.Contains("PA") || t.name.Contains("Stage") || t.name.Contains("stage")) {
        sb.AppendLine(t.name + " | pos: " + t.position + " | localPos: " + t.localPosition);
    }
}
return sb.ToString();
