var chair = UnityEngine.GameObject.Find("Wedding banquet chair with fabric cover");
if (chair == null) return "No chair found";
return "Chair pos: " + chair.transform.position + " | localPos: " + chair.transform.localPosition + " | parent: " + chair.transform.parent.name;
