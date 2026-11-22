var sb = new System.Text.StringBuilder();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding != null) {
    foreach (UnityEngine.Transform t in wedding.transform) {
        if (t.name.ToLower().Contains("carpet")) {
            sb.AppendLine("Carpet Root: " + t.name + " active=" + t.gameObject.activeSelf);
            foreach (UnityEngine.Transform c in t) {
                var r = c.GetComponent<UnityEngine.Renderer>();
                sb.AppendLine("   Child: " + c.name + " active=" + c.gameObject.activeSelf + " bounds=" + r?.bounds + " mat=" + r?.sharedMaterial?.name);
            }
        }
    }
}

return sb.ToString();
