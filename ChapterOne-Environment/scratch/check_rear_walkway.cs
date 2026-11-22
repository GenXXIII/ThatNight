var all = UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None);
var list = new System.Collections.Generic.List<string>();
foreach(var g in all) {
    var p = g.transform.position;
    if (p.x >= 2 && p.x <= 15 && p.z >= 3 && p.z <= 12) {
        list.Add($"{g.name} at {p}");
    }
}
return list.Count == 0 ? "Completely clear area!" : string.Join("\n", list);
