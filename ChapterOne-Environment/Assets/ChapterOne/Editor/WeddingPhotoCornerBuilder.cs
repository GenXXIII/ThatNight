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

            // 2. Ceremonial Red Carpet Runner Aisle
            var carpetGroup = ModelGroup("Entrance red carpet runner aisle", Vector3.zero);
            prevParent = Parent;
            Parent = carpetGroup;
            Grid("Ceremonial entrance red carpet", 16, 24, (u, v) => new Vector3((u - .5f) * 2.6f, .015f, -3.8f + v * 7.5f), S.Pink, new Vector2(2, 6), true);
            Parent = prevParent;
            ModelCombiner.CombineModel(carpetGroup, addCollider: false);

            // 3. Wedding Blessing Reception Table (Tok Krap Sla)
            BuildReceptionDesk(new Vector3(3.2f, 0, -.8f));

            // 4. Gilded Wedding Portrait Photo Easel
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

            // Timber tripod artist easel
            Beam("Easel front leg left", new Vector3(-.32f, 0, -.22f), new Vector3(0, 1.85f, 0), .035f, S.DarkWood);
            Beam("Easel front leg right", new Vector3(.32f, 0, -.22f), new Vector3(0, 1.85f, 0), .035f, S.DarkWood);
            Beam("Easel back leg", new Vector3(0, 0, .45f), new Vector3(0, 1.85f, 0), .032f, S.DarkWood);
            Beam("Easel horizontal picture shelf", new Vector3(-.42f, .85f, -.14f), new Vector3(.42f, .85f, -.14f), .04f, S.DarkWood);

            // Ornate gilded picture frame with official 2006 wedding portrait
            var frameCenter = new Vector3(0, 1.25f, -.10f);
            var frameRot = Quaternion.Euler(-12, 0, 0);
            Box("Gilded portrait frame", frameCenter, frameRot * new Vector3(.72f, .94f, .05f), S.Brass, true);
            Box("Studio couple wedding photo", frameCenter + frameRot * new Vector3(0, 0, -.03f), frameRot * new Vector3(.60f, .82f, .01f), S.Cream);

            // Red silk ribbon bow and fresh marigold garland on top corner of the portrait
            WeddingYardBuilder.Rosette(frameCenter + frameRot * new Vector3(.32f, .44f, -.04f), .14f);
            for (int b = 0; b < 5; b++)
            {
                Ball("Portrait marigold bloom", frameCenter + frameRot * new Vector3(.32f, .44f - b * .12f, -.04f), Vector3.one * .055f, S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f));
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(easelGroup, addCollider: true);
        }
    }
}
