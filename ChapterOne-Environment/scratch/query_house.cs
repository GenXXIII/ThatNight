var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house == null) return "No House found";
var sb = new System.Text.StringBuilder();
sb.AppendLine("House has " + house.transform.childCount + " children:");
foreach (UnityEngine.Transform child in house.transform) {
    sb.AppendLine("- " + child.name + " (pos: " + child.localPosition.ToString("F2") + ", rot: " + child.localEulerAngles.ToString("F1") + ")");
}
return sb.ToString();
