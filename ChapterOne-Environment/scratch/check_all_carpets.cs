var all = UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None);
var list = new System.Collections.Generic.List<string>();
foreach(var g in all) {
    if (g.name.ToLower().Contains("carpet") || g.name.ToLower().Contains("runner") || g.name.ToLower().Contains("aisle")) {
        list.Add($"{g.name} at {g.transform.position} (parent={g.transform.parent?.name})");
    }
}
return string.Join("\n", list);
