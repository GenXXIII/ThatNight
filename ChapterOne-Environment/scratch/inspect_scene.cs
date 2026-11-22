var music = UnityEngine.GameObject.Find("Traditional Cambodian wedding music ensemble");
if (music != null) {
    UnityEngine.Debug.Log("Music pos: " + music.transform.position);
    var bounds = new UnityEngine.Bounds(music.transform.position, UnityEngine.Vector3.zero);
    bool first = true;
    foreach (var r in music.GetComponentsInChildren<UnityEngine.Renderer>()) {
        if (first) { bounds = r.bounds; first = false; }
        else bounds.Encapsulate(r.bounds);
    }
    UnityEngine.Debug.Log("Music total bounds: min=" + bounds.min + " max=" + bounds.max + " center=" + bounds.center + " size=" + bounds.size);
} else {
    UnityEngine.Debug.Log("Music not found by exact name!");
}

// Find tent poles or diagonal objects near the music area
UnityEngine.Debug.Log("--- SEARCHING NEAR MUSIC ---");
foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    if (go.transform.position.x > 3.0f && go.transform.position.x < 12.0f && go.transform.position.z > -10.0f && go.transform.position.z < 5.0f) {
        if (go.name.ToLower().Contains("pole") || go.name.ToLower().Contains("tent") || go.name.ToLower().Contains("wire") || go.name.ToLower().Contains("beam") || go.name.ToLower().Contains("strut")) {
            UnityEngine.Debug.Log("Found near music: " + go.name + " at " + go.transform.position + " rot=" + go.transform.rotation.eulerAngles);
        }
    }
}

// Check stairs position and water jar
foreach (var go in UnityEngine.Object.FindObjectsByType<UnityEngine.GameObject>(UnityEngine.FindObjectsSortMode.None)) {
    if (go.name.ToLower().Contains("stair") || go.name.ToLower().Contains("water") || go.name.ToLower().Contains("jar") || go.name.ToLower().Contains("washing")) {
        UnityEngine.Debug.Log("Stair/water prop: " + go.name + " at " + go.transform.position);
    }
}

return "Done inspecting";
