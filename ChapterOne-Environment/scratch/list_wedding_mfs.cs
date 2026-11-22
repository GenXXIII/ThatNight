var sb = new System.Text.StringBuilder();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding != null) {
    // Check all MeshFilters under wedding
    foreach (var mf in wedding.GetComponentsInChildren<UnityEngine.MeshFilter>()) {
        sb.AppendLine("Wedding MF: " + mf.gameObject.name);
    }
}

return sb.ToString();
