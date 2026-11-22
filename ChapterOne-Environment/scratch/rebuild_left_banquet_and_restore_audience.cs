using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class RebuildLeftBanquetAndAudience
{
    public static string Run()
    {
        string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";

        // ==============================================================
        // 1. REBUILD LEFT BANQUET SEATING (4 COLUMNS x 5 ROWS = 20 TABLES)
        // ==============================================================
        var weddingRoot = GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
        Transform seatingZone = null;
        if (weddingRoot != null)
        {
            seatingZone = weddingRoot.transform.Find("Left and right guest banquet seating zones");
        }

        if (seatingZone == null)
        {
            var all = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            foreach (var t in all)
            {
                if (t.name == "Left and right guest banquet seating zones")
                {
                    seatingZone = t;
                    break;
                }
            }
        }

        if (seatingZone == null)
        {
            return "Error: Could not find 'Left and right guest banquet seating zones'";
        }

        // Delete existing banquet tables and chairs under seatingZone
        var toDestroy = new List<GameObject>();
        for (int i = 0; i < seatingZone.childCount; i++)
        {
            var child = seatingZone.GetChild(i).gameObject;
            if (child.name.Contains("Round banquet table set") || child.name.Contains("Wedding banquet chair"))
            {
                toDestroy.Add(child);
            }
        }
        foreach (var g in toDestroy)
        {
            UnityEngine.Object.DestroyImmediate(g);
        }

        // Setup MeshFactory Parent
        var meshFactoryType = Type.GetType("ThatNight.ChapterOne.Editor.MeshFactory, Assembly-CSharp-Editor");
        var yardBuilderType = Type.GetType("ThatNight.ChapterOne.Editor.WeddingYardBuilder, Assembly-CSharp-Editor");
        var propsBuilderType = Type.GetType("ThatNight.ChapterOne.Editor.PropsBuilder, Assembly-CSharp-Editor");
        var eventBuilderType = Type.GetType("ThatNight.ChapterOne.Editor.WeddingEventBuilder, Assembly-CSharp-Editor");

        var parentProp = meshFactoryType.GetProperty("Parent", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        var diningTableMethod = yardBuilderType.GetMethod("DiningTable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        var chairMethod = propsBuilderType.GetMethod("Chair", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        parentProp.SetValue(null, seatingZone);

        // 20 Tables: 4 columns x 5 rows
        Vector3[] leftTablePositions = {
            // Row 1 (Front row overlooking mountain slope)
            new Vector3(-18.5f, 0, -8.5f), new Vector3(-14.2f, 0, -8.5f), new Vector3(-9.9f, 0, -8.5f), new Vector3(-5.5f, 0, -8.5f),
            // Row 2
            new Vector3(-18.5f, 0, -4.0f), new Vector3(-14.2f, 0, -4.0f), new Vector3(-9.9f, 0, -4.0f), new Vector3(-5.5f, 0, -4.0f),
            // Row 3
            new Vector3(-18.5f, 0,  0.5f), new Vector3(-14.2f, 0,  0.5f), new Vector3(-9.9f, 0,  0.5f), new Vector3(-5.5f, 0,  0.5f),
            // Row 4
            new Vector3(-18.5f, 0,  5.0f), new Vector3(-14.2f, 0,  5.0f), new Vector3(-9.9f, 0,  5.0f), new Vector3(-5.5f, 0,  5.0f),
            // Row 5 (Rear row near back marquee eave)
            new Vector3(-18.5f, 0,  9.5f), new Vector3(-14.2f, 0,  9.5f), new Vector3(-9.9f, 0,  9.5f), new Vector3(-5.5f, 0,  9.5f)
        };

        foreach (var pos in leftTablePositions)
        {
            parentProp.SetValue(null, seatingZone);
            diningTableMethod.Invoke(null, new object[] { pos });

            for (int chair = 0; chair < 6; chair++)
            {
                float a = chair * Mathf.PI / 3 + .15f;
                parentProp.SetValue(null, seatingZone);
                Vector3 chairPos = pos + new Vector3(Mathf.Sin(a) * 1.26f, 0, Mathf.Cos(a) * 1.26f);
                float chairYaw = a * Mathf.Rad2Deg;
                chairMethod.Invoke(null, new object[] { chairPos, chairYaw });
            }
        }

        // ==============================================================
        // 2. RESTORE HAIR CUTTING AUDIENCE SEATING (4 ROWS x 4 COLUMNS)
        // ==============================================================
        var gehtSahZone = GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
        if (gehtSahZone != null)
        {
            var audToDestroy = new List<GameObject>();
            for (int i = 0; i < gehtSahZone.transform.childCount; i++)
            {
                var c = gehtSahZone.transform.GetChild(i).gameObject;
                if (c.name.Contains("Wedding ceremony audience chair") || c.name.Contains("center aisle carpet"))
                {
                    audToDestroy.Add(c);
                }
            }
            foreach (var g in audToDestroy)
            {
                UnityEngine.Object.DestroyImmediate(g);
            }

            parentProp.SetValue(null, gehtSahZone.transform);
            var buildAudMethod = eventBuilderType.GetMethod("BuildWeddingAudienceChairs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            buildAudMethod.Invoke(null, null);
        }

        // Unlock isStatic on all objects in weddingRoot
        if (weddingRoot != null)
        {
            foreach (var t in weddingRoot.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.isStatic = false;
            }
        }
        if (gehtSahZone != null)
        {
            foreach (var t in gehtSahZone.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.isStatic = false;
            }
        }

        // Reset Parent
        parentProp.SetValue(null, null);

        // ==============================================================
        // 3. CAPTURE VERIFICATION SCREENSHOTS
        // ==============================================================
        var camObj = new GameObject("TempVerificationCam");
        var cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.fieldOfView = 65f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1000f;

        var fillObj = new GameObject("TempVerificationFillLight");
        var fill = fillObj.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.85f;
        fill.color = new Color(1f, 0.95f, 0.9f);
        fillObj.transform.rotation = Quaternion.Euler(35f, -40f, 0);

        var rt = new RenderTexture(1280, 720, 24);
        cam.targetTexture = rt;
        var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);

        // Shot 1: Wide view of the Left Banquet Wing showing all 4 columns x 5 rows
        cam.transform.position = new Vector3(-12.0f, 25.5f, -14.5f);
        cam.transform.LookAt(new Vector3(-12.0f, 22.0f, 1.5f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_left_banquet_4x5_wide.png"), tex.EncodeToPNG());

        // Shot 2: High angled perspective showing all 20 tables filling the tent
        cam.transform.position = new Vector3(-1.0f, 27.5f, -11.0f);
        cam.transform.LookAt(new Vector3(-14.0f, 21.0f, 2.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_left_banquet_4x5_high_angle.png"), tex.EncodeToPNG());

        // Shot 3: Restored Hair Cutting Ceremony Audience Seating (4 rows under canopy)
        cam.transform.position = new Vector3(13.2f, 23.5f, -12.5f);
        cam.transform.LookAt(new Vector3(13.2f, 22.5f, 1.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_hair_cutting_audience_4_rows.png"), tex.EncodeToPNG());

        UnityEngine.Object.DestroyImmediate(fillObj);
        UnityEngine.Object.DestroyImmediate(camObj);
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(tex);

        return $"Successfully rebuilt 20 tables (4 columns x 5 rows) on left banquet wing and restored hair cutting ceremony audience seating (4 rows)!";
    }
}
