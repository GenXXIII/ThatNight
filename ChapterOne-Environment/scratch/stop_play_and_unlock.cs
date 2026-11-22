if (UnityEditor.EditorApplication.isPlaying) {
    UnityEditor.EditorApplication.isPlaying = false;
    return "Exited Play Mode! You are now back in Edit Mode.";
}

var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
int count = 0;
if (house != null) {
    foreach (var t in house.GetComponentsInChildren<UnityEngine.Transform>(true)) {
        if (t.gameObject.isStatic) {
            t.gameObject.isStatic = false;
            count++;
        }
    }
}

return "Already in Edit Mode. Unlocked " + count + " objects in house.";
