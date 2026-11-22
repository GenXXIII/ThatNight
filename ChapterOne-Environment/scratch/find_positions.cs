var gate = UnityEngine.GameObject.Find("Realistic Cambodian wedding entrance gate - Khlok Pka Sla");
var stairs = UnityEngine.GameObject.Find("Wooden entrance staircase");
var stage = UnityEngine.GameObject.Find("Ceremonial hair cutting stage platform");
var stageAisle = UnityEngine.GameObject.Find("Ceremonial hair cutting center aisle carpet");
var entranceCarpet = UnityEngine.GameObject.Find("Entrance red carpet runner aisle");
var house = UnityEngine.GameObject.Find("01 HOUSE - raised timber home");

string res = "";
res += $"Gate: {gate?.transform.position}\n";
res += $"EntranceCarpet: {entranceCarpet?.transform.position}\n";
if (entranceCarpet != null) {
    var r = entranceCarpet.GetComponent<UnityEngine.Renderer>();
    if (r != null) res += $"EntranceCarpet Bounds: min={r.bounds.min}, max={r.bounds.max}, center={r.bounds.center}\n";
}
res += $"House: {house?.transform.position}\n";
res += $"Stairs: {stairs?.transform.position}\n";
if (stairs != null) {
    var r = stairs.GetComponent<UnityEngine.Renderer>();
    if (r != null) res += $"Stairs Bounds: min={r.bounds.min}, max={r.bounds.max}, center={r.bounds.center}\n";
}
var footWash = UnityEngine.GameObject.Find("Ceremonial foot washing water basin");
res += $"FootWash: {footWash?.transform.position}\n";

res += $"Stage: {stage?.transform.position}\n";
if (stage != null) {
    var r = stage.GetComponent<UnityEngine.Renderer>();
    if (r != null) res += $"Stage Bounds: min={r.bounds.min}, max={r.bounds.max}, center={r.bounds.center}\n";
}
res += $"StageAisle: {stageAisle?.transform.position}\n";
if (stageAisle != null) {
    var r = stageAisle.GetComponent<UnityEngine.Renderer>();
    if (r != null) res += $"StageAisle Bounds: min={r.bounds.min}, max={r.bounds.max}, center={r.bounds.center}\n";
}
return res;
