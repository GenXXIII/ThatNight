var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house == null) return "No House found";
var sb = new System.Text.StringBuilder();
foreach (UnityEngine.Transform t in house.transform) {
    if (t.name.Contains("procession") || t.name.Contains("Walk") || t.name.Contains("fruit") || t.name.Contains("Food") || t.name.Contains("cupboard")) {
        sb.AppendLine(t.name + " (children: " + t.childCount + ")");
    }
}
return sb.ToString();
