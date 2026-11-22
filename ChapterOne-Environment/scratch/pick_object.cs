var sv = UnityEditor.SceneView.lastActiveSceneView;
var c = sv.camera;

// Raycast pixel (200, 255)
float vx = 200f / 779f;
float vy = 1f - (255f / 574f);
var ray = c.ViewportPointToRay(new UnityEngine.Vector3(vx, vy, 0));

// Find all raycast hits
var hits = UnityEngine.Physics.RaycastAll(ray, 100f);
var sb = new System.Text.StringBuilder();
sb.AppendLine("Physics hits: " + hits.Length);
foreach (var h in hits) {
    sb.AppendLine("  Hit: " + h.collider.gameObject.name + " at " + h.point);
}

// Also check HandleUtility.PickGameObject in Editor
var pick = UnityEditor.HandleUtility.PickGameObject(new UnityEngine.Vector2(200f, 255f), false);
sb.AppendLine("Pick at (200, 255): " + (pick != null ? pick.name : "null"));

return sb.ToString();
