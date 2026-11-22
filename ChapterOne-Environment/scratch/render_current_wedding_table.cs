string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
var table = UnityEngine.GameObject.Find("Round banquet table set");
if (table == null) return "No table found";

var camObj = new UnityEngine.GameObject("TableCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 50f;
cam.fieldOfView = 50f;

var fillObj = new UnityEngine.GameObject("TableFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 15f;
fill.intensity = 2.0f;
fill.color = new UnityEngine.Color(1.0f, 0.95f, 0.85f);
fill.transform.position = table.transform.position + UnityEngine.Vector3.up * 2.5f;

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

cam.transform.position = table.transform.position + new UnityEngine.Vector3(2.5f, 1.8f, 2.5f);
cam.transform.LookAt(table.transform.position + UnityEngine.Vector3.up * 0.5f);

cam.Render();
UnityEngine.RenderTexture.active = rt;
tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
tex.Apply();
UnityEngine.RenderTexture.active = null;
System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "current_wedding_table_and_chairs.png"), tex.EncodeToPNG());

UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Table rendered at " + table.transform.position;
