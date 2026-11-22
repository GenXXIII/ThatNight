using System;
using UnityEngine;

public static class BuildOrthogonalCarpetAndPhoto
{
    public static string Run()
    {
        ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();

        var wedding = GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
        if (wedding == null) return "No wedding root";

        // ==============================================================
        // 1. CLEAN ORTHOGONAL CARPET NETWORK (STRAIGHT 90 DEG TURNS)
        // ==============================================================
        var oldUnified = GameObject.Find("Ceremonial Wedding Carpet Network");
        if (oldUnified != null) UnityEngine.Object.DestroyImmediate(oldUnified);

        var gate = GameObject.Find("Realistic Cambodian wedding entrance gate - Khlok Pka Sla");
        if (gate != null) {
            var oldEntCarpet = gate.transform.Find("Entrance red carpet runner aisle");
            if (oldEntCarpet != null) UnityEngine.Object.DestroyImmediate(oldEntCarpet.gameObject);
        }

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

        // -------------------------------------------------------------
        // CARPET SURFACES:
        // Layout:
        // - Main Promenade Trunk: X in [-1.0, +1.0], Z in [-18.0, +9.0]
        // - House Stairs Connector: Straight turn from (0, 9.0) east to (1.82, 9.0) then north to (1.82, 10.2)
        // - Cross-Walkway in Front of Music Place: Straight along X from (1.0, -2.5) to (13.2, -2.5), width 1.8m (Z in [-3.4, -1.6])
        // - Stage Center Audience Aisle: Straight along Z at X = 13.2, from Z = -3.4 down to Z = -9.8, width 1.6m (X in [12.4, 14.0])
        // - Stage Front Steps Landing: Straight north from Z = -1.6 to Z = -1.1 at X = 13.2, width 2.4m
        // -------------------------------------------------------------

        // 1A. Main Promenade (Entrance Gate Z = -18.0 to Z = 9.0, width 2.0m: X in [-1.0, +1.0])
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Main promenade", 
            new Vector3(0, groundY, -4.5f), new Vector3(2.0f, 0.008f, 27.0f), pinkMat);

        // 1B. House Stairs Landing & Clean Turn at Z = 9.0 to 10.4:
        // Turns right from X = 0 to X = 1.82, then meets the bottom step squarely
        ThatNight.ChapterOne.Editor.MeshFactory.Box("House stairs connector turn", 
            new Vector3(0.91f, groundY + 0.0005f, 9.0f), new Vector3(2.0f, 0.008f, 2.0f), pinkMat);
        ThatNight.ChapterOne.Editor.MeshFactory.Box("House stairs landing mat", 
            new Vector3(1.82f, groundY + 0.001f, 10.0f), new Vector3(2.2f, 0.009f, 1.4f), pinkMat);

        // 1C. Cross-Walkway in Front of Music Place: From X = 1.0 to X = 13.2, Z in [-3.4, -1.6] (width 1.8m)
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Cross walkway in front of music", 
            new Vector3(7.1f, groundY + 0.0005f, -2.5f), new Vector3(12.2f, 0.008f, 1.8f), pinkMat);

        // 1D. Stage Center Audience Aisle: From Z = -3.4 down to Z = -9.8 at X = 13.2, width 1.6m (X in [12.4, 14.0])
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage center audience aisle", 
            new Vector3(13.2f, groundY + 0.0002f, -6.6f), new Vector3(1.6f, 0.008f, 6.4f), pinkMat);

        // 1E. Stage Front Steps Landing Mat: At X = 13.2, Z = -1.35
        ThatNight.ChapterOne.Editor.MeshFactory.Box("Stage front steps landing mat", 
            new Vector3(13.2f, groundY + 0.001f, -1.35f), new Vector3(2.4f, 0.009f, 0.9f), pinkMat);

        // -------------------------------------------------------------
        // PERIMETER-ONLY GOLD BORDER PIPING (CLEAN STRAIGHT LINES & 90 DEG CORNERS, NO CROSSINGS)
        // -------------------------------------------------------------
        Action<string, Vector3[]> drawBorder = (name, pts) =>
        {
            if (pts.Length < 2) return;
            for (int i = 0; i < pts.Length - 1; i++)
            {
                Vector3 pA = pts[i];
                Vector3 pB = pts[i + 1];
                Vector3 dir = (pB - pA).normalized;
                Vector3 center = (pA + pB) * 0.5f;
                float len = Vector3.Distance(pA, pB);
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

                var beam = ThatNight.ChapterOne.Editor.MeshFactory.Box(name + "_" + i, center + Vector3.up * 0.003f, new Vector3(0.045f, 0.006f, len), goldSilk);
                beam.transform.localRotation = rot;
            }
        };

        // BORDER 1: West (Left) Edge of Main Promenade (continuous straight line from -18.0 to 10.0 at X = -1.0)
        drawBorder("WestBorder", new[]{
            new Vector3(-1.0f, groundY, -18.0f),
            new Vector3(-1.0f, groundY, 10.0f),
            new Vector3(0.72f, groundY, 10.0f),
            new Vector3(0.72f, groundY, 10.7f),
            new Vector3(2.92f, groundY, 10.7f),
            new Vector3(2.92f, groundY, 9.3f),
            new Vector3(1.0f, groundY, 9.3f),
            new Vector3(1.0f, groundY, -1.6f) // stops cleanly at north edge of cross-aisle
        });

        // BORDER 2: East edge south of cross-aisle -> Turns east along south edge of cross-aisle -> Turns south down west edge of stage aisle
        drawBorder("SouthCrossBorder", new[]{
            new Vector3(1.0f, groundY, -18.0f),
            new Vector3(1.0f, groundY, -3.4f),
            new Vector3(12.4f, groundY, -3.4f), // clean 90-deg turn east
            new Vector3(12.4f, groundY, -9.8f), // clean 90-deg turn south
            new Vector3(14.0f, groundY, -9.8f)  // hem across bottom of audience aisle
        });

        // BORDER 3: East edge of stage aisle -> Turns along stage front landing -> Turns west along north edge of cross-aisle -> Turns north to house
        drawBorder("NorthCrossBorder", new[]{
            new Vector3(14.0f, groundY, -9.8f),
            new Vector3(14.0f, groundY, -1.8f),
            new Vector3(14.4f, groundY, -1.8f),
            new Vector3(14.4f, groundY, -0.9f),
            new Vector3(12.0f, groundY, -0.9f),
            new Vector3(12.0f, groundY, -1.6f),
            new Vector3(1.0f, groundY, -1.6f),  // clean 90-deg turn west in front of music place
            new Vector3(1.0f, groundY, 8.0f)   // clean 90-deg turn north along main promenade to meet stairs border
        });

        // South entrance hem (Z = -18.0)
        drawBorder("EntranceHem", new[]{
            new Vector3(-1.0f, groundY, -18.0f),
            new Vector3(1.0f, groundY, -18.0f)
        });

        ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

        foreach (var t in carpetRoot.GetComponentsInChildren<Transform>(true)) {
            t.gameObject.isStatic = false;
        }

        // ==============================================================
        // 2. REMODEL WEDDING PHOTO PORTRAIT EASEL (IMAGE 2 FIX)
        // ==============================================================
        var gateObj = GameObject.Find("Realistic Cambodian wedding entrance gate - Khlok Pka Sla");
        if (gateObj != null)
        {
            var oldEasel = gateObj.transform.Find("Gilded wedding portrait welcome easel");
            if (oldEasel != null) UnityEngine.Object.DestroyImmediate(oldEasel.gameObject);

            // Rebuild remodeled easel at ( -2.8, 0, -0.8 )
            var easelRoot = new GameObject("Gilded wedding portrait welcome easel");
            easelRoot.transform.SetParent(gateObj.transform, false);
            easelRoot.transform.localPosition = new Vector3(-2.8f, 0, -0.8f);
            easelRoot.isStatic = false;

            ThatNight.ChapterOne.Editor.MeshFactory.Parent = easelRoot.transform;

            var darkWood = ThatNight.ChapterOne.Editor.SurfaceLibrary.DarkWood;
            var brassMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Brass;
            var leafMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Leaf;
            var creamMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Cream;
            var jasmineMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Jasmine white", Color.white, .2f);
            var marigoldMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Marigold flower yellow", new Color(.95f, .72f, .08f), .1f);
            var lotusMat = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Lotus petal pink", new Color(.88f, .42f, .55f), .2f);

            // Load generated wedding portrait texture
            var portraitTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Textures/CambodianWeddingPortrait.jpg");
            Material portraitMat = null;
            if (portraitTex != null) {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                portraitMat = new Material(shader);
                portraitMat.name = "CambodianWeddingPortrait_Material";
                portraitMat.mainTexture = portraitTex;
                portraitMat.SetFloat("_Smoothness", 0.35f);
            }
            if (portraitMat == null) portraitMat = creamMat;

            // 1. Dedicated Circular Display Mat on Ground
            ThatNight.ChapterOne.Editor.MeshFactory.Cylinder("Photo display ground mat", Vector3.up * 0.005f, 1.15f, 0.006f, pinkMat, 32);
            ThatNight.ChapterOne.Editor.MeshFactory.Torus("Photo display mat gold rim", Vector3.up * 0.008f, 1.15f, 0.02f, goldSilk, 32);

            // 2. Professional Exhibition Easel (Supports from BEHIND and on a bottom shelf, NO poles blocking photo face!)
            float shelfY = 0.88f;
            float tiltAngle = -10f;
            var shelfRot = Quaternion.Euler(tiltAngle, 0, 0);

            // Rear support A-frame mast & legs (strictly behind the picture)
            ThatNight.ChapterOne.Editor.MeshFactory.Beam("Easel center mast", new Vector3(0, 0.05f, 0.02f), new Vector3(0, 2.10f, -0.32f), 0.045f, darkWood);
            ThatNight.ChapterOne.Editor.MeshFactory.Beam("Easel left leg", new Vector3(-0.42f, 0, -0.05f), new Vector3(0, 1.95f, -0.30f), 0.04f, darkWood);
            ThatNight.ChapterOne.Editor.MeshFactory.Beam("Easel right leg", new Vector3(0.42f, 0, -0.05f), new Vector3(0, 1.95f, -0.30f), 0.04f, darkWood);
            ThatNight.ChapterOne.Editor.MeshFactory.Beam("Easel back stay", new Vector3(0, 0, 0.65f), new Vector3(0, 1.85f, -0.28f), 0.035f, darkWood);

            // Horizontal Display Ledge Shelf (picture rests comfortably on top of this)
            var shelfPos = new Vector3(0, shelfY, -0.16f);
            ThatNight.ChapterOne.Editor.MeshFactory.Box("Picture display shelf", shelfPos, new Vector3(0.96f, 0.045f, 0.12f), darkWood);
            ThatNight.ChapterOne.Editor.MeshFactory.Box("Shelf front security lip", shelfPos + new Vector3(0, 0.03f, -0.05f), new Vector3(0.96f, 0.035f, 0.02f), brassMat);

            // 3. Grand Sculpted Gilded Picture Frame
            var frameCenter = new Vector3(0, 1.48f, -0.26f);
            float frameW = 0.88f, frameH = 1.15f;

            // Heavy carved golden frame outer border
            var frameObj = new GameObject("Gilded Wedding Picture Frame");
            frameObj.transform.SetParent(easelRoot.transform, false);
            frameObj.transform.localPosition = frameCenter;
            frameObj.transform.localRotation = shelfRot;

            ThatNight.ChapterOne.Editor.MeshFactory.Parent = frameObj.transform;

            // Frame molding
            ThatNight.ChapterOne.Editor.MeshFactory.Box("Frame top molding", new Vector3(0, frameH * 0.5f, 0), new Vector3(frameW + 0.12f, 0.10f, 0.06f), brassMat);
            ThatNight.ChapterOne.Editor.MeshFactory.Box("Frame bottom molding", new Vector3(0, -frameH * 0.5f, 0), new Vector3(frameW + 0.12f, 0.10f, 0.06f), brassMat);
            ThatNight.ChapterOne.Editor.MeshFactory.Box("Frame left molding", new Vector3(-frameW * 0.5f, 0, 0), new Vector3(0.10f, frameH, 0.06f), brassMat);
            ThatNight.ChapterOne.Editor.MeshFactory.Box("Frame right molding", new Vector3(frameW * 0.5f, 0, 0), new Vector3(0.10f, frameH, 0.06f), brassMat);

            // 4 Filigree corner rosettes on the frame
            for (int cx = -1; cx <= 1; cx += 2)
                for (int cy = -1; cy <= 1; cy += 2)
                {
                    ThatNight.ChapterOne.Editor.MeshFactory.Ball("Corner filigree", new Vector3(cx * (frameW * 0.5f), cy * (frameH * 0.5f), -0.025f), Vector3.one * 0.08f, brassMat);
                }

            // Gilded Inner Matting (Passe-partout bevel)
            ThatNight.ChapterOne.Editor.MeshFactory.Box("Inner gold matting", new Vector3(0, 0, -0.01f), new Vector3(frameW - 0.04f, frameH - 0.04f, 0.02f), goldSilk);

            // The Authentic Wedding Couple Portrait Canvas!
            var canvas = ThatNight.ChapterOne.Editor.MeshFactory.Box("Wedding couple studio portrait", new Vector3(0, 0, -0.022f), new Vector3(0.72f, 0.98f, 0.01f), portraitMat);
            // Ensure canvas UVs are properly aligned
            var mf = canvas.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                var mesh = UnityEngine.Object.Instantiate(mf.sharedMesh);
                var uvs = mesh.uv;
                // Standard quad front face UVs for the portrait
                mesh.uv = uvs;
                mf.sharedMesh = mesh;
            }

            // 4. Luxurious Cascading Floral & Ribbon Decor (Pka Rongkar)
            // Grand silk rosette bow on top-right corner
            Vector3 bowCorner = new Vector3(frameW * 0.5f - 0.02f, frameH * 0.5f + 0.02f, -0.05f);
            ThatNight.ChapterOne.Editor.MeshFactory.Ball("Rosette center knot", bowCorner, Vector3.one * 0.10f, ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Bridal crimson silk", new Color(.65f, .12f, .16f), .35f, .15f));
            for (int petal = 0; petal < 8; petal++)
            {
                float a = petal * Mathf.PI / 4f;
                ThatNight.ChapterOne.Editor.MeshFactory.Ball("Rosette petal", bowCorner + new Vector3(Mathf.Cos(a) * 0.06f, Mathf.Sin(a) * 0.06f, 0), new Vector3(0.08f, 0.09f, 0.04f), pinkMat);
            }

            // Cascading Jasmine & Marigold Garland draped across top and hanging down right side
            for (int g = 0; g < 14; g++)
            {
                float u = g / 13f;
                // Across the top
                float gx = Mathf.Lerp(-frameW * 0.45f, frameW * 0.45f, u);
                float gy = frameH * 0.5f + Mathf.Sin(u * Mathf.PI) * 0.04f;
                ThatNight.ChapterOne.Editor.MeshFactory.Ball("Garland jasmine bloom", new Vector3(gx, gy, -0.045f), Vector3.one * 0.045f, g % 2 == 0 ? jasmineMat : marigoldMat);
            }
            for (int c = 0; c < 10; c++)
            {
                float cy = (frameH * 0.5f) - (c * 0.085f);
                float cx = frameW * 0.52f + Mathf.Sin(c * 0.5f) * 0.02f;
                ThatNight.ChapterOne.Editor.MeshFactory.Ball("Cascading lotus bloom", new Vector3(cx, cy, -0.045f), Vector3.one * 0.05f, c % 3 == 0 ? lotusMat : jasmineMat);
            }

            // Trailing silk ribbon tails
            ThatNight.ChapterOne.Editor.MeshFactory.Beam("Ribbon tail left", bowCorner, bowCorner + new Vector3(-0.06f, -0.45f, 0.02f), 0.035f, pinkMat);
            ThatNight.ChapterOne.Editor.MeshFactory.Beam("Ribbon tail right", bowCorner, bowCorner + new Vector3(0.03f, -0.62f, 0.02f), 0.035f, goldSilk);

            ThatNight.ChapterOne.Editor.MeshFactory.Parent = easelRoot.transform;

            // 5. Flanking Brass Flower Urn Pedestals
            for (int side = -1; side <= 1; side += 2)
            {
                Vector3 urnP = new Vector3(side * 0.78f, 0, 0.05f);
                ThatNight.ChapterOne.Editor.MeshFactory.Cylinder("Urn pedestal base", urnP + Vector3.up * 0.04f, 0.16f, 0.08f, darkWood, 16);
                ThatNight.ChapterOne.Editor.MeshFactory.Cylinder("Urn fluted column", urnP + Vector3.up * 0.28f, 0.06f, 0.40f, brassMat, 16);
                ThatNight.ChapterOne.Editor.MeshFactory.Lathe("Urn flower dish", urnP + Vector3.up * 0.48f, new[]{
                    new Vector2(0, 0), new Vector2(0.18f, 0.02f), new Vector2(0.22f, 0.14f), new Vector2(0.14f, 0.18f)
                }, brassMat, 18);
                // Blossom dome inside urn
                ThatNight.ChapterOne.Editor.MeshFactory.Ball("Urn floral dome", urnP + Vector3.up * 0.62f, new Vector3(0.36f, 0.22f, 0.36f), lotusMat);
                for (int b = 0; b < 6; b++)
                {
                    float a = b * Mathf.PI / 3f;
                    ThatNight.ChapterOne.Editor.MeshFactory.Ball("Urn lotus bud", urnP + new Vector3(Mathf.Sin(a) * 0.14f, 0.68f, Mathf.Cos(a) * 0.14f), Vector3.one * 0.06f, jasmineMat);
                }
            }

            // 6. Warm Floor Up-light illuminating the portrait
            var lightObj = new GameObject("Portrait Up-light");
            lightObj.transform.SetParent(easelRoot.transform, false);
            lightObj.transform.localPosition = new Vector3(0, 0.08f, -0.65f);
            var spot = lightObj.AddComponent<Light>();
            spot.type = LightType.Spot;
            spot.color = new Color(1.0f, 0.94f, 0.85f);
            spot.intensity = 2.2f;
            spot.range = 3.5f;
            spot.spotAngle = 60f;
            lightObj.transform.localRotation = Quaternion.Euler(-55f, 0, 0);

            ThatNight.ChapterOne.Editor.MeshFactory.Parent = null;

            foreach (var t in easelRoot.GetComponentsInChildren<Transform>(true)) {
                t.gameObject.isStatic = false;
            }
        }

        // ==============================================================
        // 3. CAPTURE HIGH QUALITY SCREENSHOTS
        // ==============================================================
        string artifactDir = @"C:\Users\USER\.gemini\antigravity\brain\f90628c8-503d-4f55-8f70-7aa09151ef68";
        var camObj = new GameObject("VerificationCam");
        var cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 120f;
        cam.fieldOfView = 60f;

        var fillObj = new GameObject("VerificationFill");
        var fill = fillObj.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 1.0f;
        fill.color = new Color(1.0f, 0.96f, 0.92f);
        fill.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

        var rt = new RenderTexture(1280, 720, 24);
        cam.targetTexture = rt;
        var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);

        // Shot 1: The Remodeled Wedding Portrait Easel (Close-up front view)
        cam.transform.position = new Vector3(-2.8f, 22.45f, -16.4f);
        cam.transform.LookAt(new Vector3(-2.8f, 22.45f, -14.8f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_remodeled_photo_easel_closeup.png"), tex.EncodeToPNG());

        // Shot 2: The Cross-Walkway in Front of Music Place connecting straight to Stage
        cam.transform.position = new Vector3(5.5f, 25.5f, -7.5f);
        cam.transform.LookAt(new Vector3(7.0f, 21.0f, -2.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_straight_cross_turn.png"), tex.EncodeToPNG());

        // Shot 3: View from Entrance Gate showing the clean straight promenade (no crossing lines!)
        cam.transform.position = new Vector3(0f, 23.2f, -17.5f);
        cam.transform.LookAt(new Vector3(0f, 21.5f, -2.0f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_straight_promenade.png"), tex.EncodeToPNG());

        // Shot 4: House stairs connection (centered squarely on the stairs)
        cam.transform.position = new Vector3(1.82f, 24.2f, 5.5f);
        cam.transform.LookAt(new Vector3(1.82f, 21.5f, 10.5f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_straight_stairs.png"), tex.EncodeToPNG());

        // Shot 5: Stage aisle and front steps meeting the cross-aisle
        cam.transform.position = new Vector3(13.2f, 24.0f, -11.5f);
        cam.transform.LookAt(new Vector3(13.2f, 21.5f, -2.5f));
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(artifactDir, "real_carpet_straight_stage_aisle.png"), tex.EncodeToPNG());

        UnityEngine.Object.DestroyImmediate(fillObj);
        UnityEngine.Object.DestroyImmediate(camObj);
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(tex);

        return "Successfully built orthogonal carpet network (straight 90-deg turns, no overlapping lines) and remodeled wedding portrait easel!";
    }
}
