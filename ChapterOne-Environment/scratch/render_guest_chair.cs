string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var guestChair = UnityEngine.GameObject.Find("Decorated Cambodian wedding chair");
if (guestChair == null) return "No guest chair found";

var camObj = new UnityEngine.GameObject("GuestChairCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 50f;
cam.fieldOfView = 50f;

var fillObj = new UnityEngine.GameObject("ChairFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 15f;
fill.intensity = 2.0f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.85f);
fill.transform.position = guestChair.transform.position + UnityEngine.Vector3.up * 2.5f;

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

cam.transform.position = guestChair.transform.position + new UnityEngine.Vector3(1.5f, 1.2f, 1.8f);
cam.transform.LookAt(guestChair.transform.position + UnityEngine.Vector3.up * 0.4f);

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "current_wedding_guest_chair.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Guest chair rendered at " + guestChair.transform.position;
