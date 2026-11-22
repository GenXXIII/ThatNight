ThatNight.ChapterOne.Editor.EnvironmentBuild.Generate();

string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";

var sunObj = new UnityEngine.GameObject("InspectionSun");
var sun = sunObj.AddComponent<UnityEngine.Light>();
sun.type = UnityEngine.LightType.Directional;
sun.color = new UnityEngine.Color(1.0f, 0.95f, 0.88f);
sun.intensity = 1.35f;
sun.transform.rotation = UnityEngine.Quaternion.Euler(45f, -30f, 0f);

var fillObj = new UnityEngine.GameObject("InteriorFill");
var fill = fillObj.AddComponent<UnityEngine.Light>();
fill.type = UnityEngine.LightType.Point;
fill.range = 30f;
fill.intensity = 1.6f;
fill.color = new UnityEngine.Color(1.0f, 0.92f, 0.80f);

var camObj = new UnityEngine.GameObject("InspectionCam");
var cam = camObj.AddComponent<UnityEngine.Camera>();
cam.nearClipPlane = 0.05f;
cam.farClipPlane = 200f;
cam.fieldOfView = 65f;

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

// 1. Hair-Cutting Ceremony Stage - Top-Down View showing forward curving crescent chairs
cam.transform.position = new UnityEngine.Vector3(13.2f, 26.5f, 1.8f);
cam.transform.LookAt(new UnityEngine.Vector3(13.2f, 21.0f, 1.8f));
fill.transform.position = new UnityEngine.Vector3(13.2f, 25.0f, 1.8f);
saveRender("real_hair_cutting_chairs_topdown.png");

// 2. Hair-Cutting Ceremony Stage - Front Eye-Level Perspective
cam.transform.position = new UnityEngine.Vector3(13.2f, 22.2f, -1.8f);
cam.transform.LookAt(new UnityEngine.Vector3(13.2f, 22.0f, 2.2f));
fill.transform.position = new UnityEngine.Vector3(13.2f, 23.5f, 1.5f);
saveRender("real_hair_cutting_stage_front.png");

// 3. House Living Room - Left End: Wrist-Tying Ceremony (Sen Chang Dai) at far left wall facing RIGHT (+X)
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-4.2f, 3.65f, -4.2f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-5.75f, 2.75f, -1.6f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(-5.5f, 3.6f, -1.6f));
saveRender("real_living_room_wrist_tying_left.png");

// 4. House Living Room - Right End: Walk Ceremony Procession Food at far right wall & Salon set
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(4.2f, 3.65f, -4.2f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.75f, 2.75f, -1.6f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(5.5f, 3.6f, -1.6f));
saveRender("real_living_room_walk_food_right.png");

// 5. House Living Room - Wide Panorama showing Wrist-Tying on Far Left, open middle, and Walk Food on Far Right
cam.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(0f, 3.95f, -5.4f));
cam.transform.LookAt(ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(0f, 2.85f, -1.6f)));
fill.transform.position = ThatNight.ChapterOne.SiteLayout.HousePoint(new UnityEngine.Vector3(0f, 3.8f, -1.6f));
saveRender("real_living_room_wide_overview.png");

UnityEngine.GameObject.DestroyImmediate(sunObj);
UnityEngine.GameObject.DestroyImmediate(fillObj);
UnityEngine.GameObject.DestroyImmediate(camObj);
UnityEngine.GameObject.DestroyImmediate(rt);
UnityEngine.GameObject.DestroyImmediate(tex);

return "Regeneration and all updated views rendered successfully!";