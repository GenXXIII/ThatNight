var camGO = new UnityEngine.GameObject("VerifyUserPaintCamera");
var cam = camGO.AddComponent<UnityEngine.Camera>();
cam.clearFlags = UnityEngine.CameraClearFlags.Skybox;
cam.fieldOfView = 55f;
cam.nearClipPlane = 0.1f;
cam.farClipPlane = 500f;

System.Action<UnityEngine.Vector3, UnityEngine.Vector3, string> snap = (pos, target, filename) => {
    cam.transform.position = pos;
    cam.transform.LookAt(target);
    var rt = UnityEngine.RenderTexture.GetTemporary(1280, 720, 24, UnityEngine.RenderTextureFormat.ARGB32);
    cam.targetTexture = rt;
    cam.Render();
    UnityEngine.RenderTexture.active = rt;
    var tex = new UnityEngine.Texture2D(rt.width, rt.height, UnityEngine.TextureFormat.RGB24, false);
    tex.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
    tex.Apply();
    byte[] bytes = tex.EncodeToPNG();
    UnityEngine.Object.DestroyImmediate(tex);
    cam.targetTexture = null;
    UnityEngine.RenderTexture.active = null;
    UnityEngine.RenderTexture.ReleaseTemporary(rt);
    System.IO.File.WriteAllBytes("C:/Users/USER/.gemini/antigravity/brain/f90628c8-503d-4f55-8f70-7aa09151ef68/" + filename, bytes);
};

// View 1: Matching User Image 1 (House stairs + Water jar platform landing view)
// In Image 1, looking from slightly south of the turn towards the stairs and water jar
snap(new UnityEngine.Vector3(-1.5f, 22.8f, 3.8f), new UnityEngine.Vector3(2.5f, 21.8f, 9.5f), "real_user_paint_stairs_and_jar.png");

// View 1B: Closer elevated view of the stairs + water jar landing
snap(new UnityEngine.Vector3(0.0f, 23.5f, 5.0f), new UnityEngine.Vector3(3.0f, 21.0f, 9.0f), "real_user_paint_stairs_landing_detail.png");

// View 2: Matching User Image 2 (Looking across cross-walkway towards music ensemble)
// In Image 2, looking at the music stage and checking if pole is gone and carpet is in front
snap(new UnityEngine.Vector3(1.2f, 22.2f, -1.5f), new UnityEngine.Vector3(6.5f, 21.5f, 2.0f), "real_user_paint_music_cross_walkway.png");

// View 3: Complete Carpet Network High Angle
snap(new UnityEngine.Vector3(6.0f, 32.0f, -5.0f), new UnityEngine.Vector3(6.0f, 21.0f, 0.0f), "real_user_paint_network_overview.png");

UnityEngine.Object.DestroyImmediate(camGO);
return "Screenshots captured successfully!";
