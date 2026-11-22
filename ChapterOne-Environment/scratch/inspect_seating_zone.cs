var seatingZone = UnityEngine.GameObject.Find("Left and right guest banquet seating zones");
if (seatingZone == null) return "No seating zone";
var sb = new System.Text.StringBuilder();
sb.AppendLine("Child count: " + seatingZone.transform.childCount);
for (int i = 0; i < Mathf.Min(seatingZone.transform.childCount, 25); i++) {
    var c = seatingZone.transform.GetChild(i);
    sb.AppendLine(c.name + " | pos: " + c.position + " | localPos: " + c.localPosition);
}
return sb.ToString();
