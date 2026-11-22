var sb = new System.Text.StringBuilder();

var carpetRoot = UnityEngine.GameObject.Find("Ceremonial Wedding Carpet Network");
if (carpetRoot != null) {
    foreach (var r in carpetRoot.GetComponentsInChildren<UnityEngine.Renderer>()) {
        sb.AppendLine("Carpet Part: " + r.gameObject.name + " bounds=" + r.bounds);
    }
}

return sb.ToString();
