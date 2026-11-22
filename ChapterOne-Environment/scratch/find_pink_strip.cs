var all = UnityEngine.Object.FindObjectsByType<UnityEngine.MeshRenderer>(UnityEngine.FindObjectsSortMode.None);
var list = new System.Collections.Generic.List<string>();
foreach(var r in all) {
    if (r.bounds.Contains(new UnityEngine.Vector3(-0.7f, 21.02f, 8.0f))) {
        list.Add($"{r.gameObject.name} (parent={r.transform.parent?.name}, mat={r.sharedMaterial?.name}) bounds={r.bounds}");
    }
}
return list.Count == 0 ? "Nothing at that point" : string.Join("\n", list);
