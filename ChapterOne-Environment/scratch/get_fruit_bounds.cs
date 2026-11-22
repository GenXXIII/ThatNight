var fruit = UnityEngine.GameObject.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (fruit == null) return "No fruit object found";
var mr = fruit.GetComponent<UnityEngine.MeshRenderer>();
return "Bounds center: " + mr.bounds.center + " | size: " + mr.bounds.size + " | min: " + mr.bounds.min + " | max: " + mr.bounds.max;
