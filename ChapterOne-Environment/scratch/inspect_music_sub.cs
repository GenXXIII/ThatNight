var sb = new System.Text.StringBuilder();

var music = UnityEngine.GameObject.Find("Traditional Cambodian wedding music ensemble - Pinpeat");
if (music != null) {
    sb.AppendLine("Music bounds: " + music.GetComponent<UnityEngine.Renderer>()?.bounds);
    foreach (var r in music.GetComponentsInChildren<UnityEngine.Renderer>()) {
        sb.AppendLine("  Sub: " + r.name + " bounds=" + r.bounds);
    }
}

return sb.ToString();
