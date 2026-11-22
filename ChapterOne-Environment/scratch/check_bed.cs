var bed = UnityEngine.GameObject.Find("Handcrafted Cambodian bridal canopy bed");
if (bed == null) return "No bed found";
var mr = bed.GetComponent<UnityEngine.MeshRenderer>();
var col = bed.GetComponent<UnityEngine.Collider>();
return "Bed found: " + bed.name + " | mr.enabled = " + (mr != null ? mr.enabled.ToString() : "no mr") + " | col.enabled = " + (col != null ? col.enabled.ToString() : "no col");
