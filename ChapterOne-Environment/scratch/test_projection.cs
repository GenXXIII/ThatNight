var sb = new System.Text.StringBuilder();

// Let's create a camera matching the user's view in media_1795161393438_d87d51f9.jpg
// In media_1795161393438_d87d51f9.jpg, the camera is viewing the stairs from around (-1.2, 22.8, 3.8) looking at (2.5, 21.8, 9.5)
// Let's test what world coordinates project to pixels in a 779x574 camera!

var camGO = new UnityEngine.GameObject("TestProjCam");
var cam = camGO.AddComponent<UnityEngine.Camera>();
cam.fieldOfView = 55f;
cam.transform.position = new UnityEngine.Vector3(-1.5f, 22.8f, 3.8f);
cam.transform.LookAt(new UnityEngine.Vector3(2.5f, 21.8f, 9.5f));

// Project key points
UnityEngine.Vector3[] testPts = new[] {
    new UnityEngine.Vector3(-1.0f, 21.02f, 8.0f),
    new UnityEngine.Vector3(1.0f, 21.02f, 8.0f),
    new UnityEngine.Vector3(1.0f, 21.02f, 5.5f),
    new UnityEngine.Vector3(1.0f, 21.02f, 6.0f),
    new UnityEngine.Vector3(0.88f, 21.02f, 9.8f),
    new UnityEngine.Vector3(5.2f, 21.02f, 10.0f),
    new UnityEngine.Vector3(5.2f, 21.02f, 5.5f)
};

foreach (var pt in testPts) {
    var sp = cam.WorldToScreenPoint(pt);
    // GUI y is inverted: GUI y = 574 - sp.y * (574f / cam.pixelHeight)
    sb.AppendLine("World " + pt + " -> Screen (" + sp.x + ", " + sp.y + ") z=" + sp.z);
}

UnityEngine.Object.DestroyImmediate(camGO);
return sb.ToString();
