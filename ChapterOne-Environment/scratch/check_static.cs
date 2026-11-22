var all = UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None);
var list = new System.Collections.Generic.List<string>();
foreach(var g in all) {
    if (g.transform.parent != null && g.transform.parent.name.Contains("HOUSE")) continue;
    var p = g.transform.position;
    if (p.x >= -22 && p.x <= -3 && p.z >= 3 && p.z <= 11) {
        list.Add($"{g.name} at {p}");
    }
}
return list.Count == 0 ? "Empty area - perfectly clear for rows 4 & 5!" : string.Join("\n", list);
