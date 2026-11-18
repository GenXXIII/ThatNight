using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Traditional Cambodian Wedding Hair Cutting Ceremony (Gaat Sah / ពិធីកាត់សក់)
    // Dedicated right-side ceremonial pavilion with authentic Cambodian wedding chairs flanking the aisle.
    internal static class WeddingEventBuilder
    {
        public static void Build()
        {
            var old = Parent;
            Parent = Group("Right hair cutting ceremony zone - Gaat Sah", Vector3.zero);

            // 1. Raised Teak Ceremonial Stage Platform
            var stageGroup = ModelGroup("Ceremonial hair cutting stage platform", new Vector3(13.2f, 0, 1.0f));
            var prevParent = Parent;
            Parent = stageGroup;

            float stageW = 8.8f, stageD = 7.6f, stageH = .24f;
            Box("Carved teak stage platform", Vector3.up * (stageH * .5f), new Vector3(stageW, stageH, stageD), S.DarkWood, true);
            Box("Platform front access step", new Vector3(0, stageH * .25f, -stageD * .5f - .35f), new Vector3(3.6f, stageH * .5f, .7f), S.Wood, true);

            Grid("Ceremonial reed floor mat", 24, 16, (u, v) => new Vector3((u - .5f) * (stageW - .6f), stageH + .01f, (v - .5f) * (stageD - .6f)), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f), new Vector2(4, 3), true);
            Grid("Ceremonial red and gold silk carpet", 18, 12, (u, v) => new Vector3((u - .5f) * 5.2f, stageH + .02f, (v - .5f) * 4.4f), S.Pink, new Vector2(3, 2), true);

            for (int cx = -1; cx <= 1; cx += 2)
                for (int cz = -1; cz <= 1; cz += 2)
                {
                    var cp = new Vector3(cx * (stageW * .5f - .25f), stageH, cz * (stageD * .5f - .25f));
                    Cylinder("Stage corner post", cp, .08f, 1.25f, S.DarkWood, 16);
                    Ball("Brass lotus finial", cp + Vector3.up * 1.32f, new Vector3(.14f, .18f, .14f), S.Brass);
                }

            Parent = prevParent;
            ModelCombiner.CombineModel(stageGroup, addCollider: true);

            // 2. Twin Ceremonial Hair Cutting Chairs for Bride and Groom
            BuildCeremonialChairs(new Vector3(13.2f, stageH, 2.2f));

            // 3. Central Ceremonial Hair Cutting Tool Table and Brass Tray
            BuildHairCuttingTools(new Vector3(13.2f, stageH, .85f));

            // 4. Consecrated Water Blessing Basin
            BuildBlessingBasin(new Vector3(11.2f, stageH, .85f));

            // 5. Semicircle of Elder & Family Blessing Stools
            BuildElderStools(new Vector3(13.2f, stageH, 1.0f));

            // 6. Traditional Cambodian Wedding Music Ensemble (Pinpeat / Mahori Corner)
            BuildTraditionalMusic(new Vector3(16.0f, stageH, -1.2f));

            // 7. Floral Canopy and Hanging Garlands
            BuildCeremonyCanopy(new Vector3(13.2f, stageH, 1.0f), stageW, stageD);

            // 8. Authentic Decorated Wedding Chairs in Front (Left Side & Right Side Wings)
            BuildWeddingAudienceChairs();

            Parent = old;
        }

        static void BuildCeremonialChairs(Vector3 center)
        {
            var chairsGroup = ModelGroup("Ceremonial hair cutting chairs - Bride and Groom", center);
            var prevParent = Parent;
            Parent = chairsGroup;

            for (int side = -1; side <= 1; side += 2)
            {
                var cp = new Vector3(side * .68f, 0, 0);
                var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);

                for (int x = -1; x <= 1; x += 2)
                    for (int z = -1; z <= 1; z += 2)
                    {
                        var legPos = cp + new Vector3(x * .26f, .22f, z * .26f);
                        Box("Chair leg", legPos, new Vector3(.06f, .44f, .06f), S.DarkWood);
                    }

                Box("Chair seat frame", cp + Vector3.up * .44f, new Vector3(.66f, .08f, .64f), S.DarkWood);
                Box("Chair royal silk cushion", cp + Vector3.up * .53f, new Vector3(.62f, .12f, .60f), goldSilk);
                Box("Cushion crimson piping", cp + Vector3.up * .53f, new Vector3(.64f, .04f, .62f), S.Pink);

                for (int arm = -1; arm <= 1; arm += 2)
                {
                    var armP = cp + new Vector3(arm * .31f, .68f, -.02f);
                    Box("Armrest post", armP + Vector3.down * .12f, new Vector3(.04f, .24f, .04f), S.DarkWood);
                    Box("Armrest rest", armP, new Vector3(.06f, .04f, .52f), S.DarkWood);
                }

                var backP = cp + new Vector3(0, .92f, .28f);
                Box("Carved backrest frame", backP, new Vector3(.62f, .76f, .06f), S.DarkWood);
                Box("Backrest silk upholstery", backP + Vector3.forward * -.015f, new Vector3(.52f, .66f, .04f), goldSilk);
                Ball("Backrest carved crown finial", backP + Vector3.up * .42f, new Vector3(.18f, .14f, .06f), S.Brass);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(chairsGroup, addCollider: true);
        }

        static void BuildHairCuttingTools(Vector3 center)
        {
            var toolsGroup = ModelGroup("Ceremonial hair cutting tray and blessing tools", center);
            var prevParent = Parent;
            Parent = toolsGroup;

            Box("Carved low offering table", Vector3.up * .18f, new Vector3(1.4f, .36f, .85f), S.DarkWood, true);
            Grid("Table ceremonial silk runner", 12, 8, (u, v) => new Vector3((u - .5f) * 1.35f, .365f, (v - .5f) * .80f), S.Pink, new Vector2(2, 1), true);

            var trayPos = Vector3.up * .37f;
            Lathe("Ornate brass pedestal tray (Phan)", trayPos, new[]{
                new Vector2(.14f, 0), new Vector2(.05f, .08f), new Vector2(.24f, .14f), new Vector2(.28f, .17f), new Vector2(.26f, .19f)
            }, S.Brass, 24);

            Beam("Golden blessing scissors 1", trayPos + new Vector3(-.08f, .20f, 0), trayPos + new Vector3(.08f, .20f, .06f), .012f, S.Brass);
            Beam("Golden blessing scissors 2", trayPos + new Vector3(-.08f, .20f, 0), trayPos + new Vector3(.08f, .20f, -.06f), .012f, S.Brass);

            Box("Carved ceremonial wooden comb", trayPos + new Vector3(0, .20f, -.11f), new Vector3(.16f, .01f, .055f), S.Wood);

            Lathe("Silver holy perfume vial", trayPos + new Vector3(.09f, .19f, .09f), new[]{
                new Vector2(.03f, 0), new Vector2(.055f, .04f), new Vector2(.025f, .11f), new Vector2(.018f, .14f)
            }, S.Metal, 16);

            for (int f = 0; f < 8; f++)
            {
                float fa = f * Mathf.PI / 4f;
                Ball("Jasmine blessing blossom", trayPos + new Vector3(Mathf.Cos(fa) * .21f, .19f, Mathf.Sin(fa) * .21f), Vector3.one * .035f, S.Cream);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(toolsGroup, addCollider: true);
        }

        static void BuildBlessingBasin(Vector3 center)
        {
            var basinGroup = ModelGroup("Consecrated holy water blessing basin", center);
            var prevParent = Parent;
            Parent = basinGroup;

            Lathe("Silver basin stand", Vector3.zero, new[]{
                new Vector2(.18f, 0), new Vector2(.08f, .15f), new Vector2(.12f, .42f), new Vector2(.26f, .55f)
            }, S.Metal, 20);

            Lathe("Ornate consecrated water bowl", Vector3.up * .55f, new[]{
                new Vector2(.24f, 0), new Vector2(.32f, .16f), new Vector2(.31f, .24f)
            }, S.Metal, 24);

            Cylinder("Consecrated water surface", Vector3.up * .74f, .30f, .01f, S.Water, 24);

            for (int p = 0; p < 7; p++)
            {
                float a = p * Mathf.PI * 2 / 7f;
                Ball("Floating lotus petal", new Vector3(Mathf.Sin(a) * .16f, .75f, Mathf.Cos(a) * .16f), new Vector3(.06f, .01f, .04f), S.Pink);
            }
            Ball("Floating central lotus flower", new Vector3(0, .755f, 0), new Vector3(.09f, .05f, .09f), S.Pink);

            Parent = prevParent;
            ModelCombiner.CombineModel(basinGroup, addCollider: true);
        }

        static void BuildElderStools(Vector3 center)
        {
            var stoolsGroup = ModelGroup("Elder blessing stools - Gaat Sah", center);
            var prevParent = Parent;
            Parent = stoolsGroup;

            for (int i = 0; i < 4; i++)
            {
                float angle = -1.05f + i * .70f;
                var sp = new Vector3(Mathf.Sin(angle) * 2.85f, 0, Mathf.Cos(angle) * 2.45f);

                for (int leg = -1; leg <= 1; leg += 2)
                    for (int lz = -1; lz <= 1; lz += 2)
                        Box("Stool leg", sp + new Vector3(leg * .16f, .15f, lz * .16f), new Vector3(.045f, .30f, .045f), S.DarkWood);

                Box("Stool seat frame", sp + Vector3.up * .30f, new Vector3(.44f, .06f, .44f), S.DarkWood);
                Box("Stool silk cushion", sp + Vector3.up * .36f, new Vector3(.40f, .08f, .40f), i % 2 == 0 ? S.Pink : S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f));
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(stoolsGroup, addCollider: true);
        }

        static void BuildTraditionalMusic(Vector3 center)
        {
            var musicGroup = ModelGroup("Traditional Cambodian wedding music ensemble - Pinpeat", center);
            var prevParent = Parent;
            Parent = musicGroup;

            Grid("Musician woven reed mat", 16, 12, (u, v) => new Vector3((u - .5f) * 3.2f, .01f, (v - .5f) * 2.4f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f), new Vector2(3, 2), true);

            var roneatPos = new Vector3(-.5f, 0, .35f);
            Box("Roneat carved boat resonator", roneatPos + Vector3.up * .14f, new Vector3(1.15f, .22f, .28f), S.DarkWood, true);
            for (int key = 0; key < 18; key++)
            {
                float kx = -.52f + key * .061f;
                float ky = .28f + Mathf.Pow((key / 17f - .5f) * 2, 2) * .16f;
                Box("Roneat bamboo key", roneatPos + new Vector3(kx, ky, 0), new Vector3(.05f, .015f, .26f - Mathf.Abs(key - 8.5f) * .006f), S.Wood);
            }
            Beam("Roneat mallet 1", roneatPos + new Vector3(-.12f, .42f, .16f), roneatPos + new Vector3(-.12f, .38f, .02f), .012f, S.Wood);
            Beam("Roneat mallet 2", roneatPos + new Vector3(.12f, .42f, .16f), roneatPos + new Vector3(.12f, .38f, .02f), .012f, S.Wood);

            var drumPos = new Vector3(.6f, 0, .4f);
            Box("Drum wooden stand left", drumPos + new Vector3(-.18f, .16f, 0), new Vector3(.05f, .32f, .28f), S.DarkWood);
            Box("Drum wooden stand right", drumPos + new Vector3(.18f, .16f, 0), new Vector3(.05f, .32f, .28f), S.DarkWood);
            Cylinder("Sampho barrel drum body", drumPos + Vector3.up * .26f, .17f, .42f, S.DarkWood, 20);
            Ball("Drum leather head 1", drumPos + new Vector3(0, .26f, -.215f), new Vector3(.33f, .33f, .04f), S.Clay);
            Ball("Drum leather head 2", drumPos + new Vector3(0, .26f, .215f), new Vector3(.33f, .33f, .04f), S.Clay);

            var gongPos = new Vector3(-.6f, 0, -.6f);
            Cylinder("Gong timber frame post left", gongPos + new Vector3(-.35f, 0, 0), .04f, 1.25f, S.DarkWood, 12);
            Cylinder("Gong timber frame post right", gongPos + new Vector3(.35f, 0, 0), .04f, 1.25f, S.DarkWood, 12);
            Beam("Gong top crossbar", gongPos + new Vector3(-.38f, 1.22f, 0), gongPos + new Vector3(.38f, 1.22f, 0), .05f, S.DarkWood);
            Cylinder("Bronze wedding gong", gongPos + Vector3.up * .78f, .26f, .04f, S.Brass, 24);
            Ball("Gong center nipple", gongPos + new Vector3(0, .78f, -.025f), new Vector3(.09f, .09f, .06f), S.Brass);

            var audioPos = new Vector3(.6f, 0, -.6f);
            Box("Audio equipment wooden table", audioPos + Vector3.up * .28f, new Vector3(.68f, .56f, .48f), S.DarkWood, true);
            Box("2006 PA amplifier receiver", audioPos + Vector3.up * .60f, new Vector3(.44f, .14f, .34f), S.Metal);
            Box("Microphone on low stand", audioPos + new Vector3(.18f, .72f, .06f), new Vector3(.03f, .24f, .03f), S.Metal);
            Box("Wooden PA speaker cabinet", audioPos + new Vector3(0, .95f, 0), new Vector3(.38f, .52f, .28f), S.Black);
            Cylinder("Speaker woofer cone", audioPos + new Vector3(0, .92f, -.145f), .13f, .01f, S.Metal, 20);

            Parent = prevParent;
            ModelCombiner.CombineModel(musicGroup, addCollider: true);
        }

        static void BuildCeremonyCanopy(Vector3 p, float w, float d)
        {
            var canopyGroup = ModelGroup("Ceremonial hair cutting floral canopy", p);
            var prevParent = Parent;
            Parent = canopyGroup;

            float ch = 3.2f;
            Beam("Canopy frame beam left", new Vector3(-w * .5f + .25f, ch, -d * .5f + .25f), new Vector3(-w * .5f + .25f, ch, d * .5f - .25f), .06f, S.Wood);
            Beam("Canopy frame beam right", new Vector3(w * .5f - .25f, ch, -d * .5f + .25f), new Vector3(w * .5f - .25f, ch, d * .5f - .25f), .06f, S.Wood);
            Beam("Canopy frame beam back", new Vector3(-w * .5f + .25f, ch, d * .5f - .25f), new Vector3(w * .5f - .25f, ch, d * .5f - .25f), .06f, S.Wood);

            Grid("Canopy rear silk swag", 32, 8, (u, v) =>
            {
                float x = (u - .5f) * (w - 1.0f);
                float y = ch - Mathf.Sin(u * Mathf.PI) * .42f - v * .26f;
                return new Vector3(x, y, d * .5f - .26f);
            }, S.Pink, new Vector2(4, 1), true);

            for (int g = 0; g < 9; g++)
            {
                float gx = -3.2f + g * .8f;
                for (int b = 0; b < 6; b++)
                {
                    Ball("Hanging garland bloom", new Vector3(gx, ch - .35f - b * .15f, d * .5f - .28f), Vector3.one * .065f, b % 2 == 0 ? S.Cream : S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f));
                }
            }

            WeddingYardBuilder.Rosette(new Vector3(-w * .5f + .3f, ch - .15f, d * .5f - .3f), .24f);
            WeddingYardBuilder.Rosette(new Vector3(w * .5f - .3f, ch - .15f, d * .5f - .3f), .24f);

            Parent = prevParent;
            ModelCombiner.CombineModel(canopyGroup, addCollider: false);
        }

        /// <summary>
        /// Authentic Decorated Cambodian Wedding Chairs:
        /// Positioned in front of the ceremony on both the LEFT SIDE and RIGHT SIDE,
        /// leaving a central aisle runner leading directly to the hair cutting stage steps.
        /// Each chair has a gold silk cushion, dark wood frame, and crimson silk sash bow.
        /// </summary>
        static void BuildWeddingAudienceChairs()
        {
            var old = Parent;

            // Ceremonial red carpet runner down the center aisle (x = 13.2f)
            Grid("Ceremonial hair cutting center aisle carpet", 16, 24, (u, v) => new Vector3(13.2f + (u - .5f) * 1.6f, .01f, -3.2f - v * 5.6f), S.Pink, new Vector2(2, 6), true);

            // ==========================================
            // LEFT SIDE WING (In front, left side)
            // Relatives & guests seated on left, angled inward toward stage
            // ==========================================
            // Row 1 (Front row): 2 chairs
            WeddingGuestChair(new Vector3(10.7f, 0, -4.5f), 12f);
            WeddingGuestChair(new Vector3(11.8f, 0, -4.5f), 10f);

            // Row 2 (Middle row): 3 chairs
            WeddingGuestChair(new Vector3(9.7f, 0, -6.2f), 15f);
            WeddingGuestChair(new Vector3(10.8f, 0, -6.2f), 13f);
            WeddingGuestChair(new Vector3(11.9f, 0, -6.2f), 10f);

            // Row 3 (Back row): 3 chairs
            WeddingGuestChair(new Vector3(9.7f, 0, -7.9f), 16f);
            WeddingGuestChair(new Vector3(10.8f, 0, -7.9f), 14f);
            WeddingGuestChair(new Vector3(11.9f, 0, -7.9f), 11f);

            // ==========================================
            // RIGHT SIDE WING (In front, right side)
            // Relatives & guests seated on right, angled inward toward stage
            // ==========================================
            // Row 1 (Front row): 2 chairs
            WeddingGuestChair(new Vector3(14.6f, 0, -4.5f), -10f);
            WeddingGuestChair(new Vector3(15.7f, 0, -4.5f), -12f);

            // Row 2 (Middle row): 3 chairs
            WeddingGuestChair(new Vector3(14.5f, 0, -6.2f), -10f);
            WeddingGuestChair(new Vector3(15.6f, 0, -6.2f), -13f);
            WeddingGuestChair(new Vector3(16.7f, 0, -6.2f), -15f);

            // Row 3 (Back row): 3 chairs
            WeddingGuestChair(new Vector3(14.5f, 0, -7.9f), -11f);
            WeddingGuestChair(new Vector3(15.6f, 0, -7.9f), -14f);
            WeddingGuestChair(new Vector3(16.7f, 0, -7.9f), -16f);

            Parent = old;
        }

        static void WeddingGuestChair(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Decorated Cambodian wedding chair", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);

            // Carved timber legs
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Box("Chair leg", new Vector3(x * .18f, .22f, z * .18f), new Vector3(.045f, .44f, .045f), S.DarkWood);

            // Seat frame & royal gold silk cushion
            Box("Chair seat frame", Vector3.up * .43f, new Vector3(.46f, .05f, .46f), S.DarkWood);
            Box("Chair gold silk cushion", Vector3.up * .485f, new Vector3(.44f, .08f, .44f), goldSilk);

            // Carved backrest uprights and upholstered back
            for (int s = -1; s <= 1; s += 2)
                Box("Back upright", new Vector3(s * .18f, .72f, .19f), new Vector3(.04f, .55f, .04f), S.DarkWood);
            Box("Backrest silk upholstery", new Vector3(0, .74f, .19f), new Vector3(.33f, .44f, .035f), goldSilk);
            Box("Backrest crimson sash band", new Vector3(0, .68f, .19f), new Vector3(.35f, .10f, .045f), redSilk);
            Ball("Chair back silk rosette", new Vector3(0, .68f, .22f), Vector3.one * .075f, S.Pink);

            var chairGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(chairGroup, addCollider: true);
        }
    }
}
