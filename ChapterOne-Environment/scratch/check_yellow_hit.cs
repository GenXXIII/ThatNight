var sv = UnityEditor.SceneView.lastActiveSceneView;
var c = sv.camera;

// Raycast pixel (200, 275) - the horizontal yellow line
float vx = 200f / 779f;
float vy = 1f - (275f / 574f);
var ray = c.ViewportPointToRay(new UnityEngine.Vector3(vx, vy, 0));
float t = (21.02f - ray.origin.y) / ray.direction.y;
var hitPos = ray.origin + ray.direction * t;

// Raycast pixel (200, 400) - south on the promenade
var raySouth = c.ViewportPointToRay(new UnityEngine.Vector3(200f/779f, 1f - 400f/574f, 0));
var hitSouth = raySouth.origin + raySouth.direction * ((21.02f - raySouth.origin.y) / raySouth.direction.y);

var sb = new System.Text.StringBuilder();
sb.AppendLine("Horizontal yellow line at pixel (200, 275) hits ground at: " + hitPos);
sb.AppendLine("Promenade south at pixel (200, 400) hits ground at: " + hitSouth);

return sb.ToString();
