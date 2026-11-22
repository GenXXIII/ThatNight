var zone = UnityEngine.GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
if (zone == null) return "No hair cutting zone found";
var sb = new System.Text.StringBuilder();
sb.AppendLine("Hair cutting zone pos: " + zone.transform.position);
foreach (UnityEngine.Transform t in zone.transform) {
    sb.AppendLine(t.name + " | pos: " + t.position + " | localPos: " + t.localPosition);
}
return sb.ToString();
