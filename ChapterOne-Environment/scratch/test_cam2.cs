var camGO = new UnityEngine.GameObject("VerifyCam2");
var cam = camGO.AddComponent<UnityEngine.Camera>();
cam.clearFlags = UnityEngine.CameraClearFlags.Skybox;
cam.fieldOfView = 60f;
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

// Image 2 angle: In front of music stage, looking at music stage
snap(new UnityEngine.Vector3(4.5f, 22.0f, -3.2f), new UnityEngine.Vector3(5.5f, 21.6f, 2.5f), "test_music_stage_user_view.png");

UnityEngine.Object.DestroyImmediate(camGO);
return "Snap done";
