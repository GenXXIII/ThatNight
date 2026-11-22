var camGO = new UnityEngine.GameObject("PerfectVerifyCam");
var cam = camGO.AddComponent<UnityEngine.Camera>();
cam.clearFlags = UnityEngine.CameraClearFlags.Skybox;
cam.fieldOfView = 58f;
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

// 1. House stairs and water jar platform view (matching Image 1)
snap(new UnityEngine.Vector3(-1.0f, 22.8f, 4.5f), new UnityEngine.Vector3(2.5f, 21.6f, 9.8f), "real_perfect_stairs_and_jar_view.png");

// 2. Music ensemble front view (matching Image 2)
snap(new UnityEngine.Vector3(3.5f, 22.0f, -2.5f), new UnityEngine.Vector3(5.5f, 21.6f, 2.5f), "real_perfect_music_view.png");

// 3. Perspective showing the clean turn into the music cross-aisle
snap(new UnityEngine.Vector3(-0.5f, 22.2f, -4.5f), new UnityEngine.Vector3(4.0f, 21.3f, 0.5f), "real_perfect_turn_to_music_view.png");

UnityEngine.Object.DestroyImmediate(camGO);
return "Perfect snaps done";
