var fruit = UnityEngine.GameObject.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (fruit == null) return "No fruit object found";
var sb = new System.Text.StringBuilder();
sb.AppendLine("Name: " + fruit.name + " | pos: " + fruit.transform.localPosition + " | rot: " + fruit.transform.localEulerAngles);
foreach (UnityEngine.Transform t in fruit.transform) {
    sb.AppendLine("  child: " + t.name);
}
return sb.ToString();
