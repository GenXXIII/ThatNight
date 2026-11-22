var sv = UnityEditor.SceneView.lastActiveSceneView;
var c = sv.camera;

// In media_1795161393438_d87d51f9.jpg (779x574):
// Let's raycast pixel (200, 255)
float vx = 200f / 779f;
float vy = 1f - (255f / 574f);
var ray = c.ViewportPointToRay(new UnityEngine.Vector3(vx, vy, 0));

float t = (21.02f - ray.origin.y) / ray.direction.y;
var hitPos = ray.origin + ray.direction * t;

// Also let's raycast the red lines!
// Red Path 1: (92, 288), (250, 221), (404, 186)
var r1_start = c.ViewportPointToRay(new UnityEngine.Vector3(92f/779f, 1f - 288f/574f, 0));
var pt1_start = r1_start.origin + r1_start.direction * ((21.02f - r1_start.origin.y) / r1_start.direction.y);

var r1_mid = c.ViewportPointToRay(new UnityEngine.Vector3(250f/779f, 1f - 221f/574f, 0));
var pt1_mid = r1_mid.origin + r1_mid.direction * ((21.02f - r1_mid.origin.y) / r1_mid.direction.y);

var r1_end = c.ViewportPointToRay(new UnityEngine.Vector3(404f/779f, 1f - 186f/574f, 0));
var pt1_end = r1_end.origin + r1_end.direction * ((21.02f - r1_end.origin.y) / r1_end.direction.y);

// Red Path 2: (300, 425), (625, 390), (560, 176)
var r2_start = c.ViewportPointToRay(new UnityEngine.Vector3(300f/779f, 1f - 425f/574f, 0));
var pt2_start = r2_start.origin + r2_start.direction * ((21.02f - r2_start.origin.y) / r2_start.direction.y);

var r2_corner = c.ViewportPointToRay(new UnityEngine.Vector3(625f/779f, 1f - 390f/574f, 0));
var pt2_corner = r2_corner.origin + r2_corner.direction * ((21.02f - r2_corner.origin.y) / r2_corner.direction.y);

var r2_end = c.ViewportPointToRay(new UnityEngine.Vector3(560f/779f, 1f - 176f/574f, 0));
var pt2_end = r2_end.origin + r2_end.direction * ((21.02f - r2_end.origin.y) / r2_end.direction.y);

var sb = new System.Text.StringBuilder();
sb.AppendLine("Pixel (200, 255) hits ground at: " + hitPos);
sb.AppendLine("Red Path 1 (North border):");
sb.AppendLine("  Start: " + pt1_start);
sb.AppendLine("  Mid:   " + pt1_mid);
sb.AppendLine("  End:   " + pt1_end);
sb.AppendLine("Red Path 2 (South border):");
sb.AppendLine("  Start:  " + pt2_start);
sb.AppendLine("  Corner: " + pt2_corner);
sb.AppendLine("  End:    " + pt2_end);

return sb.ToString();
