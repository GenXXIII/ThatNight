var all = UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None);
var list = new System.Collections.Generic.List<string>();
foreach(var g in all) {
    if (g.name.Contains("music") || g.name.Contains("Music") || g.name.Contains("Pinpeat")) {
        list.Add($"{g.name} at {g.transform.position} (parent={g.transform.parent?.name})");
    }
}
return string.Join("\n", list);
