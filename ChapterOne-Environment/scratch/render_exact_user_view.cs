var camGO = new UnityEngine.GameObject("CamExactSceneView");
var cam = camGO.AddComponent<UnityEngine.Camera>();
cam.clearFlags = UnityEngine.CameraClearFlags.Skybox;
cam.fieldOfView = 60f;
cam.nearClipPlane = 0.1f;
cam.farClipPlane = 500f;
cam.transform.position = new UnityEngine.Vector3(-1.91f, 24.70f, -0.84f);
cam.transform.rotation = UnityEngine.Quaternion.Euler(38.83f, 48.73f, 0f);

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
System.IO.File.WriteAllBytes("C:/Users/USER/.gemini/antigravity/brain/f90628c8-503d-4f55-8f70-7aa09151ef68/exact_user_view_verified.png", bytes);

UnityEngine.Object.DestroyImmediate(camGO);
return "Snap rendered";
