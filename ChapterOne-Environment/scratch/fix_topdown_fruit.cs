var fruit = UnityEngine.GameObject.Find("Wedding fruit procession offering display - Chhoeng Phle Chheu");
if (fruit == null) return "No fruit object found";

string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var camObj = new UnityEngine.GameObject("FruitCaptureCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 50f;
cam.fieldOfView = 75f;

var fillObj = new UnityEngine.GameObject("FruitFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 18f;
fill.intensity = 2.5f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.85f);
fill.transform.position = fruit.transform.position + UnityEngine.Vector3.up * 2.2f;

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

// Top-down view at 2.4m above floor (under ceiling rafters)
cam.transform.position = fruit.transform.position + UnityEngine.Vector3.up * 2.45f;
cam.transform.LookAt(fruit.transform.position, fruit.transform.forward);

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_36_wedding_fruits_topdown.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Top-down re-rendered under ceiling";
