var camGO = new UnityEngine.GameObject("CamExactTest2");
var cam = camGO.AddComponent<UnityEngine.Camera>();
cam.fieldOfView = 60f;
cam.transform.position = new UnityEngine.Vector3(-1.91f, 24.70f, -0.84f);
cam.transform.rotation = UnityEngine.Quaternion.Euler(38.83f, 48.73f, 0f);

float vx = 270f / 1280f;
float vy = 1f - (400f / 720f);
var ray = cam.ViewportPointToRay(new UnityEngine.Vector3(vx, vy, 0));
float t = (21.02f - ray.origin.y) / ray.direction.y;
var hitPos = ray.origin + ray.direction * t;

UnityEngine.Object.DestroyImmediate(camGO);
return "Pixel (270, 400) hits ground at: " + hitPos;
