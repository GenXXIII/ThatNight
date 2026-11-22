var camGO = new UnityEngine.GameObject("CamCheckRay");
var cam = camGO.AddComponent<UnityEngine.Camera>();
cam.fieldOfView = 58f;
cam.transform.position = new UnityEngine.Vector3(-1.0f, 22.8f, 4.5f);
cam.transform.LookAt(new UnityEngine.Vector3(2.5f, 21.6f, 9.8f));

// In media_1795161393438_d87d51f9.jpg (779x574):
// Pixel (200, 255): let's cast a ray from the camera through this viewport point
float vx = 200f / 779f;
float vy = 1f - (255f / 574f); // invert Y for viewport
var ray = cam.ViewportPointToRay(new UnityEngine.Vector3(vx, vy, 0));

// Find where ray intersects ground plane Y = 21.02
float t = (21.02f - ray.origin.y) / ray.direction.y;
var hitPos = ray.origin + ray.direction * t;

UnityEngine.Object.DestroyImmediate(camGO);
return "Pixel (200, 255) hits ground at World Pos: " + hitPos;
