var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house == null) return "No house";
var sb = new System.Text.StringBuilder();
foreach (UnityEngine.Transform t in house.transform) {
    sb.AppendLine(t.name + " | pos: " + t.localPosition.ToString("F2") + " | rot: " + t.localEulerAngles.ToString("F1") + " | active: " + t.gameObject.activeSelf);
}
return sb.ToString();
