var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
int houseCount = 0;
if (house != null) {
    foreach (var t in house.GetComponentsInChildren<UnityEngine.Transform>(true)) {
        if (t.gameObject.isStatic) {
            t.gameObject.isStatic = false;
            houseCount++;
        }
    }
}

var yard = UnityEngine.GameObject.Find("02 WEDDING YARD - banquet and marquee");
int yardCount = 0;
if (yard != null) {
    foreach (var t in yard.GetComponentsInChildren<UnityEngine.Transform>(true)) {
        if (t.gameObject.isStatic) {
            t.gameObject.isStatic = false;
            yardCount++;
        }
    }
}

UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

return "Unlocked " + houseCount + " objects in house and " + yardCount + " objects in wedding yard. All objects are now dynamic and fully movable in Scene view!";
