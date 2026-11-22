var fruit = UnityEngine.GameObject.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (fruit == null) return "No fruit object found";
return "Fruit pos: " + fruit.transform.localPosition + " | rot: " + fruit.transform.localEulerAngles;
