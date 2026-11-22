var sb = new System.Text.StringBuilder();

var wedding = UnityEngine.GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
if (wedding != null) {
    sb.AppendLine("Wedding pos: " + wedding.transform.position + " rot=" + wedding.transform.rotation.eulerAngles);
}

// Check what is at the orange selected pole in Image 2
// In Image 2, the user had an object selected with orange outline!
// We can check UnityEditor.Selection.activeGameObject!
if (UnityEditor.Selection.activeGameObject != null) {
    sb.AppendLine("Selection: " + UnityEditor.Selection.activeGameObject.name + " at world " + UnityEditor.Selection.activeGameObject.transform.position + " local " + UnityEditor.Selection.activeGameObject.transform.localPosition);
    var p = UnityEditor.Selection.activeGameObject.transform.parent;
    if (p != null) sb.AppendLine("  Parent: " + p.name);
}

// Find all children under wedding
if (wedding != null) {
    foreach (UnityEngine.Transform child in wedding.transform) {
        sb.AppendLine("Child: " + child.name + " localPos=" + child.localPosition + " worldPos=" + child.position);
    }
}

UnityEngine.Debug.Log(sb.ToString());
return sb.ToString();
