var all = UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None);
var list = new System.Collections.Generic.List<string>();
foreach(var g in all) {
    var p = g.transform.position;
    if (p.x >= -3 && p.x <= 15 && p.z >= -15 && p.z <= 12) {
        if (g.name.Contains("pole") || g.name.Contains("arch") || g.name.Contains("desk") || g.name.Contains("chair") || g.name.Contains("Table") || g.name.Contains("table") || g.name.Contains("easel") || g.name.Contains("Sound") || g.name.Contains("Crate")) {
            list.Add($"{g.name} at {p}");
        }
    }
}
return string.Join("\n", list);
