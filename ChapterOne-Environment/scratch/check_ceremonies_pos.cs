var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house == null) return "No House found";
var sb = new System.Text.StringBuilder();
foreach (UnityEngine.Transform t in house.transform) {
    if (t.name.Contains("wrist") || t.name.Contains("fruit") || t.name.Contains("Walk") || t.name.Contains("procession")) {
        sb.AppendLine(t.name + " | localPos: " + t.localPosition);
    }
}
return sb.ToString();
