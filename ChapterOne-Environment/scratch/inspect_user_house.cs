string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";

var sunObj = new UnityEngine.GameObject("InspectSun");
var sun = sunObj.AddComponent<UnityEngine.Light>();
sun.type = UnityEngine.LightType.Directional;
sun.color = new UnityEngine.Color(1.0f, 0.96f, 0.90f);
sun.intensity = 1.2f;
sun.transform.rotation = UnityEngine.Quaternion.Euler(45f, -30f, 0f);

var fillObj = new UnityEngine.GameObject("InspectFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 35f;
fill.intensity = 1.8f;
fill.color = new UnityEngine.Color(1.0f, 0.94f, 0.85f);

var camObj = new UnityEngine.GameObject("InspectCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 200f;
cam.fieldOfView = 70f;

var rt = new UnityEngine.RenderTexture(1280, 720, 24);
cam.targetTexture = rt;
var tex = new UnityEngine.Texture2D(1280, 720, UnityEngine.TextureFormat.RGB24, false);

System.Action<string> saveRender = (filename) =>
{
    cam.Render();
    UnityEngine.RenderTexture.active = rt;
    tex.ReadPixels(new UnityEngine.Rect(0, 0, 1280, 720), 0, 0);
    tex.Apply();
    UnityEngine.RenderTexture.active = null;
    System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, filename), tex.EncodeToPNG());
};

// House coordinates in world space
// 1. Wide view from front door looking across the entire living room
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(0f, 3.85f, -5.5f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(0f, 2.75f, -1.5f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(0f, 4.0f, -1.5f));
saveRender("user_house_wide_overview.png");

// 2. View of Left End (Wrist-Tying Ceremony area)
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-3.0f, 3.75f, -4.5f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-5.5f, 2.65f, -1.8f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-5.0f, 3.8f, -1.8f));
saveRender("user_house_left_wrist_tying.png");

// 3. View of Right End (Walk Food Procession & Furniture)
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(3.0f, 3.75f, -4.5f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.5f, 2.65f, -1.8f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.0f, 3.8f, -1.8f));
saveRender("user_house_right_food.png");

// 4. View of Left Corner (Cupboard, hat rack, sleeping mats)
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-3.5f, 3.65f, -1.5f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-6.5f, 2.75f, -4.5f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-5.5f, 3.6f, -3.5f));
saveRender("user_house_left_corner.png");

// 5. View of Right Corner (Salon sitting set, tea table, thermos)
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(3.5f, 3.65f, -1.5f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.8f, 2.75f, -4.5f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.5f, 3.6f, -3.5f));
saveRender("user_house_right_corner.png");

// Clean up temporary objects completely
UnityEngine.GameObject.DestroyImmediate(sunObj);
UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Inspection captures completed successfully!";
