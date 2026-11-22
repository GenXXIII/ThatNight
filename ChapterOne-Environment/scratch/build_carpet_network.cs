using System;
using UnityEngine;

public static class TestCarpets
{
    public static string Run()
    {
        var wedding = GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
        if (wedding == null) return "No wedding root";

        // Remove old entrance carpet
        var gate = GameObject.Find("Realistic Cambodian wedding entrance gate - Khlok Pka Sla");
        if (gate != null) {
            var oldEntCarpet = gate.transform.Find("Entrance red carpet runner aisle");
            if (oldEntCarpet != null) UnityEngine.Object.DestroyImmediate(oldEntCarpet.gameObject);
        }

        // Remove old stage aisle carpet
        var gaatSah = GameObject.Find("Right hair cutting ceremony zone - Gaat Sah");
        if (gaatSah != null) {
            var oldStageCarpet = gaatSah.transform.Find("Ceremonial hair cutting center aisle carpet");
            if (oldStageCarpet != null) UnityEngine.Object.DestroyImmediate(oldStageCarpet.gameObject);
        }

        // Remove existing unified carpet group if any
        var oldUnified = GameObject.Find("Ceremonial Wedding Carpet Network");
        if (oldUnified != null) UnityEngine.Object.DestroyImmediate(oldUnified);

        var carpetRoot = new GameObject("Ceremonial Wedding Carpet Network");
        carpetRoot.transform.SetParent(wedding.transform, false);
        carpetRoot.transform.localPosition = Vector3.zero;
        carpetRoot.isStatic = false;

        var S_Pink = ThatNight.ChapterOne.Editor.SurfaceLibrary.Pink;
        var goldSilk = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Carpet gold piping", new Color(.82f, .65f, .18f), .45f, .25f);

        ThatNight.ChapterOne.Editor.MeshFactory.Parent = carpetRoot.transform;

        float groundY = 0.016f; // local Y inside wedding (wedding is at hill elevation 21m)

        // -------------------------------------------------------------
        // 1. MAIN PROMENADE: ENTRANCE GATE (Z = -18.0) TO HOUSE STAIRS (Z = 9.8)
        // -------------------------------------------------------------
        float mainWidth = 2.0f;
        int mainSteps = 56;
        ThatNight.ChapterOne.Editor.MeshFactory.Grid("Main entrance to house carpet", 8, mainSteps, (u, v) =>
        {
            float z = Mathf.Lerp(-18.0f, 9.8f, v);
            // Smooth curve from X = 0 to X = 0.8 near the house (v > 0.75, which is Z > 2.8)
            float tCurve = Mathf.Clamp01((v - 0.72f) / 0.28f);
            float smoothT = tCurve * tCurve * (3f - 2f * tCurve);
            float centerX = Mathf.Lerp(0f, 0.8f, smoothT);

            // Normal direction (perpendicular to path)
            float dx_dz = (tCurve > 0 && tCurve < 1) ? (0.8f * 6f * tCurve * (1f - tCurve) / (9.8f - (-18f) * 0.28f)) : 0f;
            Vector2 fwd = new Vector2(dx_dz, 1f).normalized;
            Vector2 norm = new Vector2(fwd.y, -fwd.x);

            float offset = (u - 0.5f) * mainWidth;
            return new Vector3(centerX + norm.x * offset, groundY, z + norm.y * offset);
        }, S_Pink, new Vector2(2, 16), true);

        // Gold border piping along main promenade
        for (int side = -1; side <= 1; side += 2)
        {
            int s = side;
            ThatNight.ChapterOne.Editor.MeshFactory.Grid("Main carpet gold border " + (s < 0 ? "L" : "R"), 2, mainSteps, (u, v) =>
            {
                float z = Mathf.Lerp(-18.0f, 9.8f, v);
                float tCurve = Mathf.Clamp01((v - 0.72f) / 0.28f);
                float smoothT = tCurve * tCurve * (3f - 2f * tCurve);
                float centerX = Mathf.Lerp(0f, 0.8f, smoothT);

                float dx_dz = (tCurve > 0 && tCurve < 1) ? (0.8f * 6f * tCurve * (1f - tCurve) / 7.78f) : 0f;
                Vector2 fwd = new Vector2(dx_dz, 1f).normalized;
                Vector2 norm = new Vector2(fwd.y, -fwd.x);

                float offset = (s * 0.5f * mainWidth) + (u - 0.5f) * 0.05f;
                return new Vector3(centerX + norm.x * offset, groundY + 0.002f, z + norm.y * offset);
            }, goldSilk, new Vector2(1, 16), true);
        }

        // -------------------------------------------------------------
        // 2. CONNECTING CURVED BRANCH: MAIN WALKWAY (0, -11.0) -> STAGE AISLE (13.2, -10.5)
        // -------------------------------------------------------------
        // Cubic Bezier curve connecting seamlessly:
        // P0: (0.0, -11.0), P1: (5.0, -11.3), P2: (9.5, -11.2), P3: (13.2, -10.5)
        Vector2 p0 = new Vector2(0f, -11.0f);
        Vector2 p1 = new Vector2(5.0f, -11.3f);
        Vector2 p2 = new Vector2(9.5f, -11.2f);
        Vector2 p3 = new Vector2(13.2f, -10.5f);

        float branchWidth = 1.8f;
        int branchSteps = 32;

        ThatNight.ChapterOne.Editor.MeshFactory.Grid("Connecting cross-aisle carpet to stage", 8, branchSteps, (u, v) =>
        {
            // Cubic bezier formula
            float t = v;
            float omt = 1f - t;
            Vector2 pos = omt * omt * omt * p0 + 3f * omt * omt * t * p1 + 3f * omt * t * t * p2 + t * t * t * p3;

            // Derivative for tangent
            Vector2 tang = 3f * omt * omt * (p1 - p0) + 6f * omt * t * (p2 - p1) + 3f * t * t * (p3 - p2);
            Vector2 norm = new Vector2(-tang.y, tang.x).normalized;

            float offset = (u - 0.5f) * branchWidth;
            return new Vector3(pos.x + norm.x * offset, groundY + 0.001f, pos.y + norm.y * offset);
        }, S_Pink, new Vector2(2, 8), true);

        // Gold border piping along connecting branch
        for (int side = -1; side <= 1; side += 2)
        {
            int s = side;
            ThatNight.ChapterOne.Editor.MeshFactory.Grid("Branch gold border " + (s < 0 ? "L" : "R"), 2, branchSteps, (u, v) =>
            {
                float t = v;
                float omt = 1f - t;
                Vector2 pos = omt * omt * omt * p0 + 3f * omt * omt * t * p1 + 3f * omt * t * t * p2 + t * t * t * p3;
                Vector2 tang = 3f * omt * omt * (p1 - p0) + 6f * omt * t * (p2 - p1) + 3f * t * t * (p3 - p2);
                Vector2 norm = new Vector2(-tang.y, tang.x).normalized;

                float offset = (s * 0.5f * branchWidth) + (u - 0.5f) * 0.05f;
                return new Vector3(pos.x + norm.x * offset, groundY + 0.003f, pos.y + norm.y * offset);
            }, goldSilk, new Vector2(1, 8), true);
        }

        // -------------------------------------------------------------
        // 3. CEREMONIAL STAGE AISLE: (13.2, -10.5) UP TO STAGE FRONT STEPS (13.2, -2.0)
        // -------------------------------------------------------------
        float stageAisleWidth = 1.6f;
        int stageSteps = 24;
        ThatNight.ChapterOne.Editor.MeshFactory.Grid("Ceremonial stage center aisle carpet", 6, stageSteps, (u, v) =>
        {
            float z = Mathf.Lerp(-10.5f, -2.0f, v);
            float offset = (u - 0.5f) * stageAisleWidth;
            return new Vector3(13.2f + offset, groundY + 0.0015f, z);
        }, S_Pink, new Vector2(2, 8), true);

        // Gold border piping along stage aisle
        for (int side = -1; side <= 1; side += 2)
        {
            int s = side;
            ThatNight.ChapterOne.Editor.MeshFactory.Grid("Stage aisle gold border " + (s < 0 ? "L" : "R"), 2, stageSteps, (u, v) =>
            {
                float z = Mathf.Lerp(-10.5f, -2.0f, v);
                float offset = (s * 0.5f * stageAisleWidth) + (u - 0.5f) * 0.05f;
                return new Vector3(13.2f + offset, groundY + 0.0035f, z);
            }, goldSilk, new Vector2(1, 8), true);
        }

        // -------------------------------------------------------------
        // 4. STAGE FRONT STEP LANDING RUNNER
        // -------------------------------------------------------------
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage entrance greeting landing", new Vector3(13.2f, groundY + 0.002f, -2.0f), new Vector3(2.4f, 0.01f, 0.9f), S_Pink);

        // -------------------------------------------------------------
        // 5. HOUSE STAIRCASE LANDING RUNNER
        // -------------------------------------------------------------
        ThatNight.ChapterOne.Editor.MeshFactory.Box("House staircase bottom landing", new Vector3(0.8f, groundY + 0.002f, 9.8f), new Vector3(2.4f, 0.01f, 1.2f), S_Pink);

        ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

        // Ensure all objects are non-static
        foreach (var t in carpetRoot.GetComponentsInChildren<Transform>(true)) {
            t.gameObject.isStatic = false;
        }

        // -------------------------------------------------------------
        // 6. SCREENSHOTS FOR VERIFICATION
        // -------------------------------------------------------------
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

        // Shot 1: High Angle bird's eye view showing Entrance -> House and Branch -> Stage
        cam.transform.position = new Vector3(6.5f, 38.0f, -10.0f);
        cam.transform.LookAt(new Vector3(6.5f, 21.0f, -1.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_network_high_overview.png"), tex.EncodeToPNG());

        // Shot 2: Perspective from Entrance Gate looking forward down the carpet to House and Stage branch
        cam.transform.position = new Vector3(0f, 23.2f, -17.5f);
        cam.transform.LookAt(new Vector3(3.0f, 21.5f, -4.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_entrance_perspective.png"), tex.EncodeToPNG());

        // Shot 3: View of the House Staircase landing showing carpet connecting to the bottom of the stairs
        cam.transform.position = new Vector3(0.8f, 24.5f, 5.0f);
        cam.transform.LookAt(new Vector3(0.8f, 21.5f, 10.5f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_house_stairs_connection.png"), tex.EncodeToPNG());

        UnityEngine.Object.DestroyImmediate(fillObj);
        UnityEngine.Object.DestroyImmediate(camObj);
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(tex);

        return "Successfully created continuous ceremonial pink carpet network from entrance to house and connecting to stage!";
    }
}
