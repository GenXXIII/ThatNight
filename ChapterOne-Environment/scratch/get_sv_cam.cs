var sv = UnityEditor.SceneView.lastActiveSceneView;
if (sv != null) {
    var c = sv.camera;
    return "SceneView cam pos=" + c.transform.position + " rot=" + c.transform.rotation.eulerAngles + " fov=" + c.fieldOfView;
}
return "No SceneView";
