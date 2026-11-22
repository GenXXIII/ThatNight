var fruit = UnityEngine.GameObject.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (fruit == null) return "No fruit object found";
return "Fruit found | Parent: " + fruit.transform.parent.name + 
    " | localPos: " + fruit.transform.localPosition + 
    " | worldPos: " + fruit.transform.position;
