using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Authentic Cambodian Village Wedding Entrance Gate (Khlok Pka Sla / ខ្លោងទ្វារអាពាហ៍ពិពាហ៍)
    // Features traditional silk-wrapped pillars, live banana trees tied to posts, coconut frond crest,
    // hanging welcome banner, red carpet aisle, and reception blessing desk.
    internal static class WeddingPhotoCornerBuilder
    {
        public static void Build()
        {
            var old = Parent;
            Parent = Group("Realistic Cambodian wedding entrance gate - Khlok Pka Sla", new Vector3(0, 0, SiteLayout.RoofFront));

            // 1. Dual Towering Entrance Pillars with Silk Wraps and Live Banana Trees
            var gateGroup = ModelGroup("Ceremonial wedding entrance archway", Vector3.zero);
            var prevParent = Parent;
            Parent = gateGroup;

            float gateWidth = 4.4f;
            float gateHeight = 4.2f;

            for (int side = -1; side <= 1; side += 2)
            {
                var pp = new Vector3(side * (gateWidth * .5f), 0, 0);

                // Heavy timber/bamboo gateway pillar
                Cylinder("Gate structural timber pillar", pp, .14f, gateHeight, S.DarkWood, 16);

                // Pleated dusty rose & gold silk sash wrapping
                for (int wrap = 0; wrap < 7; wrap++)
                {
                    float wy = .4f + wrap * .52f;
                    Tube("Pleated silk pillar wrap", pp + Vector3.up * wy, .165f, .05f, wrap % 2 == 0 ? S.Pink : S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f), Quaternion.Euler(15, 0, 0), 16);
                }

                // Live young banana tree bound to the front of the pillar (Traditional Khmer fertility blessing)
                var bananaPos = pp + new Vector3(0, 0, -.26f);
                Cylinder("Young banana trunk", bananaPos, .09f, 2.6f, S.Leaf, 14);
                // Hanging bunch of young green bananas
                Ball("Hanging banana stalk bunch", bananaPos + new Vector3(0, 1.85f, -.16f), new Vector3(.24f, .38f, .22f), S.Leaf);
                // Arching green banana leaves
                for (int leaf = 0; leaf < 5; leaf++)
                {
                    float la = leaf * .85f - 1.7f;
                    var ld = new Vector3(Mathf.Sin(la) * .85f, 0, Mathf.Cos(la) * .85f - .4f);
                    Grid("Arching fresh banana leaf", 12, 4, (u, v) => bananaPos + Vector3.up * (2.6f + Mathf.Sin(u * Mathf.PI) * .65f - u * .55f) + ld * u * 1.8f + Vector3.Cross(ld, Vector3.up).normalized * (v - .5f) * .55f * Mathf.Sin(u * Mathf.PI), S.Leaf, Vector2.one, true);
                }

                // Silk tie ribbon binding banana tree to pillar
                Tube("Golden silk tie ribbon", bananaPos + Vector3.up * 1.4f, .18f, .04f, S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f), Quaternion.identity, 16);

                // Large ceremonial brass/terracotta entrance flower urn flanking the gate
                var urnPos = pp + new Vector3(side * .85f, 0, -.35f);
                Lathe("Ceremonial entrance water urn", urnPos, new[]{
                    new Vector2(.14f, 0), new Vector2(.10f, .08f), new Vector2(.26f, .38f), new Vector2(.22f, .55f), new Vector2(.28f, .60f), new Vector2(.25f, .62f), new Vector2(.12f, .15f)
                }, S.Clay, 24);
                Cylinder("Urn blessing water", urnPos + Vector3.up * .52f, .24f, .01f, S.Water, 20);
                Ball("Floating pink lotus bloom", urnPos + Vector3.up * .53f, new Vector3(.14f, .08f, .14f), S.Pink);
            }

            // Overhead lintel beam connecting pillars
            Beam("Gate overhead lintel beam", new Vector3(-gateWidth * .5f, gateHeight, 0), new Vector3(gateWidth * .5f, gateHeight, 0), .14f, S.DarkWood);

            // Woven coconut palm frond arched pediment (Traditional chevron pattern)
            Grid("Arched coconut palm frond pediment", 36, 8, (u, v) =>
            {
                float x = (u - .5f) * (gateWidth + .6f);
                float arch = Mathf.Sin(u * Mathf.PI);
                float y = gateHeight + arch * 1.15f + (v - .5f) * .35f;
                float z = (v - .5f) * .12f;
                return new Vector3(x, y, z);
            }, S.Leaf, new Vector2(6, 1), true);

            // Carved Khmer peaked pediment crest (Kbach Pka Chan)
            Ball("Khmer peaked gable crest", new Vector3(0, gateHeight + 1.45f, 0), new Vector3(.32f, .48f, .12f), S.Brass);

            // Hanging red and gold welcome signboard in Khmer: "សិរីសួស្តី អាពាហ៍ពិពាហ៍"
            Box("Welcome signboard board", new Vector3(0, gateHeight - .28f, 0), new Vector3(2.6f, .52f, .04f), S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f), true);
            Box("Signboard gold border", new Vector3(0, gateHeight - .28f, 0), new Vector3(2.68f, .58f, .035f), S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f));

            // Hanging jasmine flower garlands and marigold tassels
            for (int j = 0; j < 9; j++)
            {
                float jx = -1.6f + j * .4f;
                for (int b = 0; b < 5; b++)
                {
                    Ball("Gate hanging jasmine flower", new Vector3(jx, gateHeight - .6f - b * .14f, 0), Vector3.one * .055f, b % 2 == 0 ? S.Cream : S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f));
                }
            }

            // Silk corner rosettes
            WeddingYardBuilder.Rosette(new Vector3(-gateWidth * .5f, gateHeight, -.08f), .28f);
            WeddingYardBuilder.Rosette(new Vector3(gateWidth * .5f, gateHeight, -.08f), .28f);

            Parent = prevParent;
            ModelCombiner.CombineModel(gateGroup, addCollider: true);

            // 2. Entrance greeting photo easel & greeting desk
            BuildReceptionDesk(new Vector3(3.2f, 0, -.8f));
            BuildPhotoEasel(new Vector3(-2.8f, 0, -.8f));

            Parent = old;
        }

        static void BuildReceptionDesk(Vector3 p)
        {
            var deskGroup = ModelGroup("Wedding reception greeting table - Tok Krap Sla", p);
            var prevParent = Parent;
            Parent = deskGroup;

            // Carved wooden table draped in pleated silk
            PropsBuilder.Table(Vector3.zero, new Vector2(1.6f, .85f), .76f, S.DarkWood);
            Grid("Table gold pleated silk drape", 32, 10, (u, v) =>
            {
                float x = (u - .5f) * 1.68f;
                float y = .77f - v * .68f;
                return new Vector3(x, y, -.44f);
            }, S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f), new Vector2(3, 1), true);

            // Miniature Khmer Temple / Pavilion Money Box (Prorop Phka Sla for wedding envelopes)
            var boxPos = new Vector3(-.35f, .78f, 0);
            Box("Temple money box base", boxPos + Vector3.up * .12f, new Vector3(.38f, .24f, .28f), S.DarkWood, true);
            // Peaked roof with envelope donation slot
            Box("Temple box peaked roof", boxPos + Vector3.up * .28f, new Vector3(.44f, .12f, .34f), S.Roof);
            Box("Roof slot for money envelopes", boxPos + Vector3.up * .345f, new Vector3(.14f, .02f, .025f), S.Black);
            Ball("Temple spire finial", boxPos + Vector3.up * .42f, new Vector3(.06f, .14f, .06f), S.Brass);

            // Leather-bound Guest Blessing Book (Siev Phov Cham Noung Dai) & Pens
            var bookPos = new Vector3(.32f, .78f, 0);
            Box("Guest registry book", bookPos + Vector3.up * .018f, new Vector3(.32f, .035f, .24f), S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f));
            Box("Registry open white pages", bookPos + Vector3.up * .038f, new Vector3(.29f, .01f, .21f), S.Cream);
            Cylinder("Pen holder ceramic cup", bookPos + new Vector3(.22f, 0, .08f), .035f, .09f, S.Porcelain, 16);
            Beam("Ballpoint signing pen", bookPos + new Vector3(.22f, .08f, .08f), bookPos + new Vector3(.24f, .18f, .09f), .007f, S.Metal);

            // Silver tray of wedding favors (rolled betel leaves & jasmine corsages)
            var favorPos = new Vector3(0, .78f, .12f);
            Lathe("Favor silver tray", favorPos, new[]{ new Vector2(0, 0), new Vector2(.14f, 0), new Vector2(.18f, .025f) }, S.Metal, 18);
            for (int f = 0; f < 6; f++)
            {
                float a = f * Mathf.PI / 3;
                Ball("Fresh jasmine boutonniere", favorPos + new Vector3(Mathf.Sin(a) * .09f, .035f, Mathf.Cos(a) * .09f), Vector3.one * .045f, S.Cream);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(deskGroup, addCollider: true);
        }

        static void BuildPhotoEasel(Vector3 p)
        {
            var easelGroup = ModelGroup("Gilded wedding portrait welcome easel", p);
            var prevParent = Parent;
            Parent = easelGroup;

            var darkWood = S.DarkWood;
            var brassMat = S.Brass;
            var creamMat = S.Cream;
            var goldSilk = S.Plain("Carpet gold piping", new Color(.82f, .65f, .18f), .45f, .25f);
            var jasmineMat = S.Plain("Jasmine white", Color.white, .2f);
            var marigoldMat = S.Plain("Marigold flower yellow", new Color(.95f, .72f, .08f), .1f);
            var lotusMat = S.Plain("Lotus petal pink", new Color(.88f, .42f, .55f), .2f);

            // Load authentic Cambodian wedding portrait texture
            var portraitTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Textures/CambodianWeddingPortrait.jpg");
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("ThatNight/WeatheredSurface") ?? Shader.Find("Standard");
            var portraitMat = new Material(shader) { name = "CambodianWeddingPortrait_Material" };
            if (portraitTex != null)
            {
                if (portraitMat.HasProperty("_BaseMap")) portraitMat.SetTexture("_BaseMap", portraitTex);
                if (portraitMat.HasProperty("_MainTex")) portraitMat.SetTexture("_MainTex", portraitTex);
                if (portraitMat.HasProperty("_BaseColor")) portraitMat.SetColor("_BaseColor", Color.white);
                if (portraitMat.HasProperty("_Color")) portraitMat.SetColor("_Color", Color.white);
                portraitMat.mainTexture = portraitTex;
                portraitMat.color = Color.white;
                portraitMat.SetFloat("_Smoothness", 0.30f);
            }
            else
            {
                portraitMat = creamMat;
            }

            // 1. Dedicated Circular Display Mat on Ground
            Cylinder("Photo display ground mat", Vector3.up * 0.005f, 1.15f, 0.006f, S.Pink, 32);

            // 2. Artist Exhibition Easel (Positioned BEHIND the picture frame, tilted back slightly)
            float shelfY = 0.86f;
            float tiltAngle = -8f;
            var shelfRot = Quaternion.Euler(tiltAngle, 0, 0);

            // Easel tripod legs BEHIND the display ledge so they never cross in front of the portrait!
            // Legs descend from Y = 2.15m at Z = +0.02m down to ground at Z = +0.18m (front legs) and Z = +0.75m (back leg)
            Beam("Easel center mast", new Vector3(0, 0.05f, 0.16f), new Vector3(0, 2.20f, -0.02f), 0.045f, darkWood);
            Beam("Easel left leg", new Vector3(-0.46f, 0, 0.16f), new Vector3(0, 2.05f, 0.02f), 0.04f, darkWood);
            Beam("Easel right leg", new Vector3(0.46f, 0, 0.16f), new Vector3(0, 2.05f, 0.02f), 0.04f, darkWood);
            Beam("Easel back stay", new Vector3(0, 0, 0.72f), new Vector3(0, 1.95f, 0.04f), 0.035f, darkWood);

            // Horizontal display shelf ledge sitting in front of the mast
            var shelfPos = new Vector3(0, shelfY, 0.02f);
            Box("Picture display shelf", shelfPos, new Vector3(1.02f, 0.045f, 0.14f), darkWood);
            Box("Shelf front security lip", shelfPos + new Vector3(0, 0.03f, -0.06f), new Vector3(1.02f, 0.035f, 0.025f), brassMat);

            // 3. Grand Sculpted Gilded Picture Frame (Resting ON the shelf, facing incoming guests at -Z)
            var frameCenter = new Vector3(0, 1.48f, -0.06f);
            float frameW = 0.88f, frameH = 1.16f;

            var frameObj = new GameObject("Gilded Wedding Picture Frame");
            frameObj.transform.SetParent(easelGroup.transform, false);
            frameObj.transform.localPosition = frameCenter;
            frameObj.transform.localRotation = shelfRot;
            frameObj.AddComponent<ThatNight.ChapterOne.ModelRoot>();

            Parent = frameObj.transform;

            // Deep gilded frame molding
            Box("Frame top molding", new Vector3(0, frameH * 0.5f, 0), new Vector3(frameW + 0.12f, 0.10f, 0.06f), brassMat);
            Box("Frame bottom molding", new Vector3(0, -frameH * 0.5f, 0), new Vector3(frameW + 0.12f, 0.10f, 0.06f), brassMat);
            Box("Frame left molding", new Vector3(-frameW * 0.5f, 0, 0), new Vector3(0.10f, frameH, 0.06f), brassMat);
            Box("Frame right molding", new Vector3(frameW * 0.5f, 0, 0), new Vector3(0.10f, frameH, 0.06f), brassMat);

            // 4 Filigree corner rosettes on the frame
            for (int cx = -1; cx <= 1; cx += 2)
                for (int cy = -1; cy <= 1; cy += 2)
                {
                    Ball("Corner filigree", new Vector3(cx * (frameW * 0.5f), cy * (frameH * 0.5f), -0.025f), Vector3.one * 0.08f, brassMat);
                }

            // Gilded Inner Matting
            Box("Inner gold matting", new Vector3(0, 0, 0.005f), new Vector3(frameW - 0.04f, frameH - 0.04f, 0.015f), goldSilk);

            // The Authentic Wedding Couple Portrait Canvas (Dedicated properly UV-mapped planar mesh)
            PortraitCanvas("Wedding couple studio portrait", new Vector3(0, 0, -0.018f), 0.74f, 1.02f, portraitMat);

            // 4. Traditional Cambodian Floral & Silk Decor (Phka Pchong & Silk Bow)
            Vector3 bowCorner = new Vector3(frameW * 0.5f - 0.02f, frameH * 0.5f + 0.02f, -0.04f);
            Ball("Rosette center knot", bowCorner, Vector3.one * 0.10f, S.Plain("Bridal crimson silk", new Color(.65f, .12f, .16f), .35f, .15f));
            for (int petal = 0; petal < 8; petal++)
            {
                float a = petal * Mathf.PI / 4f;
                Ball("Rosette petal", bowCorner + new Vector3(Mathf.Cos(a) * 0.06f, Mathf.Sin(a) * 0.06f, 0), new Vector3(0.08f, 0.09f, 0.04f), S.Pink);
            }

            // Top floral swag along the top frame edge
            for (int g = 0; g < 14; g++)
            {
                float u = g / 13f;
                float gx = Mathf.Lerp(-frameW * 0.45f, frameW * 0.45f, u);
                float gy = frameH * 0.5f + Mathf.Sin(u * Mathf.PI) * 0.04f;
                Ball("Garland bloom", new Vector3(gx, gy, -0.035f), Vector3.one * 0.045f, g % 2 == 0 ? jasmineMat : marigoldMat);
            }

            // Cascading jasmine and lotus garland down the right side
            for (int c = 0; c < 10; c++)
            {
                float cy = (frameH * 0.5f) - (c * 0.085f);
                float cx = frameW * 0.52f + Mathf.Sin(c * 0.5f) * 0.02f;
                Ball("Cascading lotus bloom", new Vector3(cx, cy, -0.035f), Vector3.one * 0.05f, c % 3 == 0 ? lotusMat : jasmineMat);
            }

            Beam("Ribbon tail left", bowCorner, bowCorner + new Vector3(-0.06f, -0.45f, 0.02f), 0.035f, S.Pink);
            Beam("Ribbon tail right", bowCorner, bowCorner + new Vector3(0.03f, -0.62f, 0.02f), 0.035f, goldSilk);

            Parent = easelGroup.transform;

            // 5. Flanking Brass Flower Urn Pedestals
            for (int side = -1; side <= 1; side += 2)
            {
                Vector3 urnP = new Vector3(side * 0.82f, 0, 0.05f);
                Cylinder("Urn pedestal base", urnP + Vector3.up * 0.04f, 0.16f, 0.08f, darkWood, 16);
                Cylinder("Urn fluted column", urnP + Vector3.up * 0.28f, 0.06f, 0.40f, brassMat, 16);
                Lathe("Urn flower dish", urnP + Vector3.up * 0.48f, new[]{
                    new Vector2(0, 0), new Vector2(0.18f, 0.02f), new Vector2(0.22f, 0.14f), new Vector2(0.14f, 0.18f)
                }, brassMat, 18);
                Ball("Urn floral dome", urnP + Vector3.up * 0.62f, new Vector3(0.36f, 0.22f, 0.36f), lotusMat);
                for (int b = 0; b < 6; b++)
                {
                    float a = b * Mathf.PI / 3f;
                    Ball("Urn lotus bud", urnP + new Vector3(Mathf.Sin(a) * 0.14f, 0.68f, Mathf.Cos(a) * 0.14f), Vector3.one * 0.06f, jasmineMat);
                }
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(easelGroup, addCollider: true);
        }

        static GameObject PortraitCanvas(string name, Vector3 pos, float width, float height, Material mat)
        {
            var go = new GameObject(name);
            if (Parent != null) go.transform.SetParent(Parent, false);
            go.transform.localPosition = pos;
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;

            var mesh = new Mesh { name = name };
            float hw = width * 0.5f;
            float hh = height * 0.5f;

            // Front face facing -Z (towards viewer) and Back face facing +Z
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-hw, -hh, 0), // 0: bottom-left
                new Vector3( hw, -hh, 0), // 1: bottom-right
                new Vector3( hw,  hh, 0), // 2: top-right
                new Vector3(-hw,  hh, 0), // 3: top-left

                new Vector3( hw, -hh, 0.008f), // 4: back bottom-right
                new Vector3(-hw, -hh, 0.008f), // 5: back bottom-left
                new Vector3(-hw,  hh, 0.008f), // 6: back top-left
                new Vector3( hw,  hh, 0.008f)  // 7: back top-right
            };

            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),

                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            Vector3[] normals = new Vector3[]
            {
                Vector3.back, Vector3.back, Vector3.back, Vector3.back,
                Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward
            };

            Color[] colors = new Color[]
            {
                Color.white, Color.white, Color.white, Color.white,
                Color.white, Color.white, Color.white, Color.white
            };

            int[] triangles = new int[]
            {
                0, 2, 1, 0, 3, 2, // Front face facing -Z
                4, 6, 5, 4, 7, 6  // Back face facing +Z
            };

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.colors = colors;
            mesh.triangles = triangles;
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            mf.sharedMesh = mesh;
            return go;
        }
    }
}
