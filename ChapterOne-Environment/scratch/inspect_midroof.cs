var sb = new System.Text.StringBuilder();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
var midRoof = wedding.transform.Find("Middle roof over entrance aisle");
if (midRoof != null) {
    foreach (UnityEngine.Transform t in midRoof) {
        sb.AppendLine("MidRoof child: " + t.name + " localPos=" + t.localPosition + " localRot=" + t.localEulerAngles);
    }
}

return sb.ToString();
