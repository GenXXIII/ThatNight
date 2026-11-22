using System;
using System.Collections.Generic;
using UnityEngine;

public static class BuildCleanCarpetNetwork
{
    public static string Run()
    {
        ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

        var wedding = GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
        if (wedding == null) return "No wedding root";

        // Remove old unified carpet group
        var oldUnified = GameObject.Find("Ceremonial Wedding Carpet Network");
        if (oldUnified != null) UnityEngine.Object.DestroyImmediate(oldUnified);

        // Remove old entrance carpet if exists
        var gate = GameObject.Find("Realistic Cambodian wedding entrance gate - Khlok Pka Sla");
        if (gate != null) {
            var oldEntCarpet = gate.transform.Find("Entrance red carpet runner aisle");
            if (oldEntCarpet != null) UnityEngine.Object.DestroyImmediate(oldEntCarpet.gameObject);
        }

        // Remove old stage aisle carpet if exists
        var gaatSah = GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
        if (gaatSah != null) {
            var oldStageCarpet = gaatSah.transform.Find("Ceremonial hair cutting center aisle carpet");
            if (oldStageCarpet != null) UnityEngine.Object.DestroyImmediate(oldStageCarpet.gameObject);
        }

        var carpetRoot = new GameObject("Ceremonial Wedding Carpet Network");
        carpetRoot.transform.SetParent(wedding.transform, false);
        carpetRoot.transform.localPosition = Vector3.zero;
        carpetRoot.isStatic = false;

        var pinkMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Pink;
        var goldSilk = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Carpet gold piping", new Color(.82f, .65f, .18f), .45f, .25f);

        ThatNight.ChapterOne.Editor.MeshFactory.Parent = carpetRoot.transform;
        float groundY = 0.018f;

        // ==============================================================
        // 1. CARPET SURFACES (Dusty Rose Wedding Cloth)
        // ==============================================================

        // 1A. Main Promenade South Segment: From Entrance (Z = -18.0) to Cross-Aisle (Z = -3.4)
        // Width 2.0m: X in [-1.0, +1.0]
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Main promenade south", 
            new Vector3(0, groundY, -10.7f), new Vector3(2.0f, 0.008f, 14.6f), pinkMat);

        // 1B. Main Promenade Intersection: Z in [-3.4, -1.6], X in [-1.0, +1.0]
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Main promenade junction", 
            new Vector3(0, groundY, -2.5f), new Vector3(2.0f, 0.008f, 1.8f), pinkMat);

        // 1C. Main Promenade North Segment: From Cross-Aisle (Z = -1.6) curving to House Stairs (Z = 10.0)
        int northSteps = 36;
        ThatNight.ChapterOne.Editor.MeshFactory.Grid("Main promenade north to house", 8, northSteps, (u, v) =>
        {
            float z = Mathf.Lerp(-1.6f, 10.0f, v);
            // Curves from X = 0 to X = 1.82 (the stairs center) for v > 0.4 (Z > 3.0)
            float tCurve = Mathf.Clamp01((v - 0.40f) / 0.60f);
            float smoothT = tCurve * tCurve * (3f - 2f * tCurve);
            float centerX = Mathf.Lerp(0f, 1.82f, smoothT);

            float dx_dz = (tCurve > 0 && tCurve < 1) ? (1.82f * 6f * tCurve * (1f - tCurve) / 6.96f) : 0f;
            Vector2 fwd = new Vector2(dx_dz, 1f).normalized;
            Vector2 norm = new Vector2(fwd.y, -fwd.x);

            float offset = (u - 0.5f) * 2.0f;
            return new Vector3(centerX + norm.x * offset, groundY, z + norm.y * offset);
        }, pinkMat, new Vector2(2, 10), true);

        // 1D. House Staircase Landing Mat: Centered at (1.82, 10.0)
        ThatNight.ChapterOne.Editor.MeshFactory.Box("House stairs landing mat", 
            new Vector3(1.82f, groundY + 0.001f, 10.0f), new Vector3(2.3f, 0.009f, 1.2f), pinkMat);

        // 1E. Cross-Walkway in Front of Music Place: From Main Promenade (X = 1.0) to Stage (X = 13.2)
        // Width 1.8m: Z in [-3.4, -1.6]
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Cross-walkway in front of music", 
            new Vector3(7.1f, groundY + 0.0005f, -2.5f), new Vector3(12.2f, 0.008f, 1.8f), pinkMat);

        // 1F. Stage Center Aisle: From Cross-Walkway (Z = -3.4) down through Audience to Z = -9.8
        // Width 1.6m: X in [12.4, 14.0]
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage center audience aisle", 
            new Vector3(13.2f, groundY, -6.6f), new Vector3(1.6f, 0.008f, 6.4f), pinkMat);

        // 1G. Stage Front Steps Landing Mat: At (13.2, -1.8)
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage front steps landing mat", 
            new Vector3(13.2f, groundY + 0.001f, -1.8f), new Vector3(2.4f, 0.009f, 0.8f), pinkMat);

        // ==============================================================
        // 2. SEAMLESS OUTER PERIMETER GOLD PIPING (NO CROSSING LINES!)
        // ==============================================================

        // Helper to draw a continuous gold ribbon along 3D points
        Action<string, Vector3[]> drawGoldBorder = (name, pts) =>
        {
            if (pts.Length < 2) return;
            ThatNight.ChapterOne.Editor.MeshFactory.Grid(name, 2, pts.Length - 1, (u, v) =>
            {
                int seg = Mathf.Min(Mathf.FloorToInt(v * (pts.Length - 1)), pts.Length - 2);
                float frac = v * (pts.Length - 1) - seg;
                Vector3 pA = pts[seg];
                Vector3 pB = pts[seg + 1];
                Vector3 center = Vector3.Lerp(pA, pB, frac);
                Vector3 dir = (pB - pA).normalized;
                Vector3 norm = new Vector3(-dir.z, 0, dir.x);

                float offset = (u - 0.5f) * 0.05f;
                return center + norm * offset + Vector3.up * 0.003f;
            }, goldSilk, new Vector2(1, pts.Length * 2), true);
        };

        // BORDER 1: Continuous Western (Left) Edge of Main Promenade (Entrance -> House)
        var westBorderPts = new List<Vector3>();
        // Straight south run
        westBorderPts.Add(new Vector3(-1.0f, groundY, -18.0f));
        westBorderPts.Add(new Vector3(-1.0f, groundY, -1.6f));
        // Curved north run towards house
        for (int i = 0; i <= 20; i++)
        {
            float v = i / 20f;
            float z = Mathf.Lerp(-1.6f, 10.0f, v);
            float tCurve = Mathf.Clamp01((v - 0.40f) / 0.60f);
            float smoothT = tCurve * tCurve * (3f - 2f * tCurve);
            float centerX = Mathf.Lerp(0f, 1.82f, smoothT);
            float dx_dz = (tCurve > 0 && tCurve < 1) ? (1.82f * 6f * tCurve * (1f - tCurve) / 6.96f) : 0f;
            Vector2 fwd = new Vector2(dx_dz, 1f).normalized;
            Vector2 norm = new Vector2(fwd.y, -fwd.x);
            westBorderPts.Add(new Vector3(centerX - norm.x * 1.0f, groundY, z - norm.y * 1.0f));
        }
        drawGoldBorder("West border main promenade", westBorderPts.ToArray());

        // BORDER 2: East Edge South -> Turns into South Edge of Cross-Walkway -> Turns into West Edge of Stage Aisle
        var southBranchBorderPts = new List<Vector3>();
        southBranchBorderPts.Add(new Vector3(1.0f, groundY, -18.0f));
        southBranchBorderPts.Add(new Vector3(1.0f, groundY, -4.0f));
        // Smooth fillet curve at the junction (from X=1, Z=-4.0 to X=1.6, Z=-3.4)
        for (int i = 0; i <= 6; i++)
        {
            float a = i * Mathf.PI / 12f; // 0 to 90 deg
            southBranchBorderPts.Add(new Vector3(1.0f + (1f - Mathf.Cos(a)) * 0.6f, groundY, -4.0f + Mathf.Sin(a) * 0.6f));
        }
        // South edge of cross-walkway
        southBranchBorderPts.Add(new Vector3(11.8f, groundY, -3.4f));
        // Smooth fillet curve turning into stage aisle (from X=11.8, Z=-3.4 to X=12.4, Z=-4.0)
        for (int i = 0; i <= 6; i++)
        {
            float a = i * Mathf.PI / 12f;
            southBranchBorderPts.Add(new Vector3(11.8f + Mathf.Sin(a) * 0.6f, groundY, -3.4f - (1f - Mathf.Cos(a)) * 0.6f));
        }
        // Down the west edge of stage aisle
        southBranchBorderPts.Add(new Vector3(12.4f, groundY, -9.8f));
        // End hem across stage aisle
        southBranchBorderPts.Add(new Vector3(14.0f, groundY, -9.8f));
        drawGoldBorder("South cross border", southBranchBorderPts.ToArray());

        // BORDER 3: East Edge of Stage Aisle -> North Edge of Cross-Walkway -> Turns into East Edge of Main Promenade
        var northBranchBorderPts = new List<Vector3>();
        // East edge of stage aisle
        northBranchBorderPts.Add(new Vector3(14.0f, groundY, -9.8f));
        northBranchBorderPts.Add(new Vector3(14.0f, groundY, -1.8f));
        // Across front steps landing
        northBranchBorderPts.Add(new Vector3(14.4f, groundY, -1.8f));
        northBranchBorderPts.Add(new Vector3(14.4f, groundY, -1.4f));
        northBranchBorderPts.Add(new Vector3(12.0f, groundY, -1.4f));
        northBranchBorderPts.Add(new Vector3(12.0f, groundY, -1.6f));
        // North edge of cross-walkway in front of music place
        northBranchBorderPts.Add(new Vector3(1.6f, groundY, -1.6f));
        // Smooth fillet curve turning north towards house (from X=1.6, Z=-1.6 to X=1.0, Z=-1.0)
        for (int i = 0; i <= 6; i++)
        {
            float a = i * Mathf.PI / 12f;
            northBranchBorderPts.Add(new Vector3(1.6f - (1f - Mathf.Cos(a)) * 0.6f, groundY, -1.6f + Mathf.Sin(a) * 0.6f));
        }
        // Continue along east edge of main promenade north towards house
        for (int i = 0; i <= 20; i++)
        {
            float v = i / 20f;
            float z = Mathf.Lerp(-1.0f, 10.0f, v);
            float tCurve = Mathf.Clamp01((v - 0.35f) / 0.65f);
            float smoothT = tCurve * tCurve * (3f - 2f * tCurve);
            float centerX = Mathf.Lerp(0f, 1.82f, smoothT);
            float dx_dz = (tCurve > 0 && tCurve < 1) ? (1.82f * 6f * tCurve * (1f - tCurve) / 7.15f) : 0f;
            Vector2 fwd = new Vector2(dx_dz, 1f).normalized;
            Vector2 norm = new Vector2(fwd.y, -fwd.x);
            northBranchBorderPts.Add(new Vector3(centerX + norm.x * 1.0f, groundY, z + norm.y * 1.0f));
        }
        drawGoldBorder("North cross border", northBranchBorderPts.ToArray());

        // Border at Entrance Gate Hem (Z = -18.0)
        drawGoldBorder("Entrance hem", new[]{ new Vector3(-1.0f, groundY, -18.0f), new Vector3(1.0f, groundY, -18.0f) });

        // Border at House Stairs Landing Hem (Z = 10.0)
        drawGoldBorder("House stairs hem", new[]{ new Vector3(0.67f, groundY, 10.6f), new Vector3(2.97f, groundY, 10.6f) });

        ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

        foreach (var t in carpetRoot.GetComponentsInChildren<Transform>(true)) {
            t.gameObject.isStatic = false;
        }

        // ==============================================================
        // 3. CAPTURE VERIFICATION SCREENSHOTS
        // ==============================================================
        string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
        var camObj = new GameObject("CarpetCam");
        var cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 120f;
        cam.fieldOfView = 65f;

        var fillObj = new GameObject("CarpetFill");
        var fill = fillObj.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 1.0f;
        fill.color = new Color(1.0f, 0.96f, 0.92f);
        fill.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

        var rt = new RenderTexture(1280, 720, 24);
        cam.targetTexture = rt;
        var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);

        // Shot 1: The cross-connection directly in front of the music place to stage
        cam.transform.position = new Vector3(5.5f, 25.5f, -6.5f);
        cam.transform.LookAt(new Vector3(7.5f, 21.0f, -1.5f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_infront_of_music.png"), tex.EncodeToPNG());

        // Shot 2: View from entrance showing the clean unbroken promenade leading to house
        cam.transform.position = new Vector3(0f, 23.5f, -17.5f);
        cam.transform.LookAt(new Vector3(0.5f, 21.5f, -1.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_clean_promenade.png"), tex.EncodeToPNG());

        // Shot 3: House stairs landing showing perfect alignment
        cam.transform.position = new Vector3(1.82f, 24.5f, 5.0f);
        cam.transform.LookAt(new Vector3(1.82f, 21.5f, 10.5f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_clean_stairs.png"), tex.EncodeToPNG());

        // Shot 4: Stage aisle meeting the cross-walkway
        cam.transform.position = new Vector3(13.2f, 24.2f, -12.0f);
        cam.transform.LookAt(new Vector3(13.2f, 21.5f, -3.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_clean_stage_aisle.png"), tex.EncodeToPNG());

        UnityEngine.Object.DestroyImmediate(fillObj);
        UnityEngine.Object.DestroyImmediate(camObj);
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(tex);

        return "Successfully generated clean seamless carpet network connected in front of music place!";
    }
}
