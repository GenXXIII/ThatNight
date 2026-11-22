var music = UnityEngine.GameObject.Find("Traditional Cambodian wedding music ensemble - Pinpeat");
if (music == null) return "No music found";
return "Music pos: " + music.transform.position + " | localPos: " + music.transform.localPosition + " | parent: " + music.transform.parent.name;
