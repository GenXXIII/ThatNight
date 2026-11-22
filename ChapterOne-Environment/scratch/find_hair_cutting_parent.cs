var zone = UnityEngine.GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
if (zone == null) return "No hair cutting zone found";
return "Found: " + zone.name + " | parent: " + (zone.transform.parent != null ? zone.transform.parent.name : "root") + " | pos: " + zone.transform.position;
