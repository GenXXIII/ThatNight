var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");
if (house == null) return "No house";
int staticCount = 0;
int dynamicCount = 0;
foreach (var t in house.GetComponentsInChildren<UnityEngine.Transform>(true)) {
    if (t.gameObject.isStatic) staticCount++;
    else dynamicCount++;
}
return "House transforms: static = " + staticCount + ", dynamic = " + dynamicCount;
