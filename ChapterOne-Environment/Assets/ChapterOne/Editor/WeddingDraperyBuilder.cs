using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Open-air mountain wedding pavilion decoration: open sides, celebratory valances & ceremonial entrance arch.
    internal static class WeddingDraperyBuilder
    {
        const float Height = SiteLayout.WeddingEave, Opening = SiteLayout.EntranceHalfWidth, Rear = SiteLayout.WeddingRear;

        public static void Build(float front)
        {
            front = SiteLayout.WeddingFront;
            var old = Parent;
            Parent = ModelGroup("Wedding open pavilion decorations and ceremonial arch", Vector3.up * SiteLayout.HillElevation);

            float half = SiteLayout.HalfWidth;
            float sideFront = SiteLayout.EntranceSideFront;

            // 1. Festive Corner and Entrance Pavilion Pillars with gathered silk sashes and gold rosettes
            Vector3[] pillarPositions = {
                new Vector3(-half, 0, sideFront), new Vector3(half, 0, sideFront),
                new Vector3(-half, 0, Rear), new Vector3(half, 0, Rear),
                new Vector3(-Opening, 0, sideFront), new Vector3(Opening, 0, sideFront),
                new Vector3(-Opening, 0, SiteLayout.RoofFront), new Vector3(Opening, 0, SiteLayout.RoofFront)
            };

            foreach (var pos in pillarPositions)
            {
                PillarWrap(pos, Height);
            }

            // 2. Ceremonial Entrance Arch (Khlok Pka Sla) at the front of the wedding runner
            CeremonialEntranceArch(new Vector3(0, 0, SiteLayout.RoofFront));

            // 3. Central Fabric Wedding Runner leading to the stilt house
            Grid("Fabric wedding entrance runner", 6, 90, (u, v) =>
            {
                float x = Mathf.Lerp(-1.1f, 1.1f, u), z = Mathf.Lerp(SiteLayout.RoofFront, Rear, v);
                return new Vector3(x, GroundDetailBuilder.Height(x, z) - SiteLayout.HillElevation + .014f, z);
            }, S.Pink, new Vector2(2, 14), true);

            // Overhead festive hanging bulb at entrance
            LightingBuilder.Bulb(new Vector3(0, 3.4f, front + .35f), 6, 2.5f, false);

            var draperyGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(draperyGroup, addCollider: false);
        }

        static void PillarWrap(Vector3 pos, float height)
        {
            // Festive gathered silk wraps around pavilion support posts
            Cylinder("Bamboo pavilion pillar", pos, .065f, height, S.DarkWood, 16);
            Tube("Silk pillar wrap lower", pos + Vector3.up * 1.2f, .085f, .018f, S.Pink, Quaternion.identity, 16);
            Tube("Silk pillar wrap upper", pos + Vector3.up * (height - .6f), .085f, .018f, S.Pink, Quaternion.identity, 16);
            WeddingYardBuilder.Rosette(pos + Vector3.up * (height - .55f) + Vector3.back * .08f, .22f);
        }

        static void CeremonialEntranceArch(Vector3 p)
        {
            // Traditional Khmer arched wedding gate (Rong Pka Sla)
            float archWidth = 3.2f, archHeight = 3.6f;
            for (int side = -1; side <= 1; side += 2)
            {
                var basePos = p + new Vector3(side * archWidth / 2, 0, 0);
                Cylinder("Arch post base", basePos, .10f, .35f, S.Clay);
                Cylinder("Arch bamboo timber post", basePos + Vector3.up * .35f, .075f, archHeight - .35f, S.Wood, 16);
                WeddingYardBuilder.Rosette(basePos + Vector3.up * (archHeight - .2f), .26f);
            }

            // Arched curved lintel with jasmine and marigold garlands
            Grid("Arched floral wedding lintel", 32, 6, (u, v) =>
            {
                float a = u * Mathf.PI;
                float x = (u - .5f) * archWidth;
                float y = archHeight + Mathf.Sin(a) * .65f + (v - .5f) * .18f;
                return p + new Vector3(x, y, 0);
            }, S.Pink, new Vector2(4, 1), true);

            // Gold trimmed welcome banner across arch
            Box("Wedding welcome board", p + new Vector3(0, archHeight + .45f, 0), new Vector3(2.2f, .42f, .04f), S.Cream);
            Box("Welcome board gold trim", p + new Vector3(0, archHeight + .45f, -.025f), new Vector3(2.28f, .48f, .01f), S.Brass);
        }
    }
}
