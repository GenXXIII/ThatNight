using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    // Traditional Cambodian Wedding Hair Cutting Ceremony (Gaat Sah / ពិធីកាត់សក់)
    // Dedicated right-side ceremonial pavilion with authentic Cambodian wedding chairs flanking the aisle.
    public static class WeddingEventBuilder
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

            // 2. Twin Ceremonial Hair Cutting Chairs for Bride and Groom + 3 Attendants each side
            BuildCeremonialChairs(new Vector3(13.2f, stageH, 2.2f));

            // 3. Central Ceremonial Hair Cutting Tool Table and Brass Tray
            BuildHairCuttingTools(new Vector3(13.2f, stageH, .85f));

            // 4. Consecrated Water Blessing Basin (Placed neatly at front-left of stage, clear of attendant chairs)
            BuildBlessingBasin(new Vector3(11.4f, stageH, .35f));

            // 5. Traditional Cambodian Wedding Music Ensemble (Moved behind to Z = 2.5m, aligning near stage rear)
            BuildTraditionalMusic(new Vector3(6.8f, 0, 3.0f));

            // 6. Floral Canopy and Hanging Garlands
            BuildCeremonyCanopy(new Vector3(13.2f, stageH, 1.0f), stageW, stageD);

            // 7. Authentic Decorated Wedding Chairs in Front (Left Side & Right Side Wings)
            BuildWeddingAudienceChairs();

            Parent = old;
        }

        static void BuildCeremonialChairs(Vector3 center)
        {
            var chairsGroup = ModelGroup("Ceremonial hair cutting chairs - Bride Groom and Attendants", center);
            var prevParent = Parent;
            Parent = chairsGroup;

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var pinkSilk = S.Plain("Bridesmaid rose silk", new Color(.85f, .42f, .55f), .40f, .20f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);

            // 1. GRAND HIGH-FRAME ROYAL THRONES FOR BRIDE (left, -0.72m) AND GROOM (right, +0.72m)
            for (int side = -1; side <= 1; side += 2)
            {
                var cp = new Vector3(side * .72f, 0, 0);

                // 4 heavy carved timber throne legs with brass claw feet
                for (int x = -1; x <= 1; x += 2)
                    for (int z = -1; z <= 1; z += 2)
                    {
                        var legPos = cp + new Vector3(x * .28f, .22f, z * .28f);
                        Box("Throne leg", legPos, new Vector3(.075f, .44f, .075f), S.DarkWood);
                        Box("Throne brass foot", legPos + Vector3.down * .19f, new Vector3(.09f, .06f, .09f), S.Brass);
                    }

                // Lower carved throne stretchers & apron
                Box("Throne apron front", cp + new Vector3(0, .38f, -.28f), new Vector3(.56f, .08f, .04f), S.DarkWood);
                Box("Throne apron back", cp + new Vector3(0, .38f, .28f), new Vector3(.56f, .08f, .04f), S.DarkWood);
                Box("Throne apron left", cp + new Vector3(-.28f, .38f, 0), new Vector3(.04f, .08f, .56f), S.DarkWood);
                Box("Throne apron right", cp + new Vector3(.28f, .38f, 0), new Vector3(.04f, .08f, .56f), S.DarkWood);

                // Stately throne seat frame & thick royal gold cushion
                Box("Throne seat frame", cp + Vector3.up * .44f, new Vector3(.76f, .09f, .72f), S.DarkWood);
                Box("Throne royal gold cushion", cp + Vector3.up * .54f, new Vector3(.72f, .13f, .68f), goldSilk);
                Box("Throne crimson piping", cp + Vector3.up * .54f, new Vector3(.74f, .04f, .70f), redSilk);

                // Carved armrests with padded gold silk arm cushions and brass scroll terminals
                for (int arm = -1; arm <= 1; arm += 2)
                {
                    var armP = cp + new Vector3(arm * .34f, .72f, -.02f);
                    Box("Armrest post front", armP + new Vector3(0, -.12f, -.26f), new Vector3(.05f, .24f, .05f), S.DarkWood);
                    Box("Armrest post back", armP + new Vector3(0, -.12f, .26f), new Vector3(.05f, .24f, .05f), S.DarkWood);
                    Box("Armrest carved rail", armP, new Vector3(.07f, .05f, .58f), S.DarkWood);
                    Box("Armrest padded silk", armP + Vector3.up * .035f, new Vector3(.06f, .025f, .50f), goldSilk);
                    Ball("Armrest scroll finial", armP + Vector3.forward * -.30f, Vector3.one * .075f, S.Brass);
                }

                // MAJESTIC HIGH-FRAME BACKREST (Soaring high frame up to 1.95m!)
                // Tall carved side pilaster pillars
                for (int p = -1; p <= 1; p += 2)
                {
                    Box("High frame side pillar", cp + new Vector3(p * .34f, 1.15f, .31f), new Vector3(.07f, 1.40f, .07f), S.DarkWood);
                    Ball("High frame pillar capital", cp + new Vector3(p * .34f, 1.86f, .31f), Vector3.one * .085f, S.Brass);
                }

                // Grand carved top arch crest beam
                Box("High frame top arch beam", cp + new Vector3(0, 1.80f, .31f), new Vector3(.72f, .08f, .06f), S.DarkWood);
                Grid("High frame arched crown pediment", 16, 3, (u, v) => new Vector3((u - .5f) * .68f, 1.80f + Mathf.Sin(u * Mathf.PI) * .15f + v * .04f, .31f), S.DarkWood, Vector2.one, true);
                Ball("High frame apex royal crown crest", cp + new Vector3(0, 1.98f, .31f), new Vector3(.20f, .18f, .06f), S.Brass);

                // High-back carved timber structural backing
                Box("High frame back backing", cp + new Vector3(0, 1.16f, .315f), new Vector3(.66f, 1.25f, .04f), S.DarkWood);

                // Grand royal gold silk padded upholstery panel (soaring tall!)
                Box("High frame royal gold upholstery", cp + new Vector3(0, 1.15f, .29f), new Vector3(.58f, 1.18f, .04f), goldSilk);

                // Ceremonial crimson silk borders framing the gold panel
                Box("High frame crimson border top", cp + new Vector3(0, 1.73f, .288f), new Vector3(.56f, .04f, .035f), redSilk);
                Box("High frame crimson border left", cp + new Vector3(-.28f, 1.15f, .288f), new Vector3(.04f, 1.18f, .035f), redSilk);
                Box("High frame crimson border right", cp + new Vector3(.28f, 1.15f, .288f), new Vector3(.04f, 1.18f, .035f), redSilk);

                // Center royal brass rosette medallion
                Ball("High frame center royal rosette", cp + new Vector3(0, 1.45f, .27f), Vector3.one * .09f, S.Brass);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(chairsGroup, addCollider: true);

            // 2. Traditional Khmer 3 Bridesmaid Chairs on Bride's Left (Neang Phnom) - ALL YELLOW / GOLD SILK
            // Curving FORWARD in a graceful semicircle toward the audience and angled inward toward the bride & groom
            Vector3[] brideAttendantPositions = {
                center + new Vector3(-1.50f, 0, -.22f),
                center + new Vector3(-2.22f, 0, -.58f),
                center + new Vector3(-2.85f, 0, -1.05f)
            };
            float[] brideAttendantYaws = { -16f, -30f, -45f };

            for (int i = 0; i < 3; i++)
            {
                // All yellow / gold silk matching the stage royal theme
                BuildStageAttendantChair(brideAttendantPositions[i], brideAttendantYaws[i], goldSilk, goldSilk, redSilk, redSilk, "Bridesmaid chair " + (i + 1));
            }

            // 3. Traditional Khmer 3 Groomsman Chairs on Groom's Right (A-phnom) - ALL YELLOW / GOLD SILK
            // Curving FORWARD in a graceful semicircle toward the audience and angled inward toward the bride & groom
            Vector3[] groomAttendantPositions = {
                center + new Vector3(1.50f, 0, -.22f),
                center + new Vector3(2.22f, 0, -.58f),
                center + new Vector3(2.85f, 0, -1.05f)
            };
            float[] groomAttendantYaws = { 16f, 30f, 45f };

            for (int i = 0; i < 3; i++)
            {
                // All yellow / gold silk matching the stage royal theme
                BuildStageAttendantChair(groomAttendantPositions[i], groomAttendantYaws[i], goldSilk, goldSilk, redSilk, redSilk, "Groomsman chair " + (i + 1));
            }
        }

        static void BuildStageAttendantChair(Vector3 p, float yaw, Material cushionMat, Material backMat, Material sashMat, Material rosetteMat, string title)
        {
            var old = Parent;
            Parent = ModelGroup(title, p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);

            // 4 carved timber legs
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Box("Chair leg", new Vector3(x * .18f, .22f, z * .18f), new Vector3(.045f, .44f, .045f), S.DarkWood);

            // Sturdy lower cross-stretchers between legs for authentic wooden joinery
            Box("Stretcher left", new Vector3(-.18f, .12f, 0), new Vector3(.03f, .03f, .34f), S.DarkWood);
            Box("Stretcher right", new Vector3(.18f, .12f, 0), new Vector3(.03f, .03f, .34f), S.DarkWood);
            Box("Stretcher front", new Vector3(0, .12f, -.18f), new Vector3(.34f, .03f, .03f), S.DarkWood);
            Box("Stretcher back", new Vector3(0, .12f, .18f), new Vector3(.34f, .03f, .03f), S.DarkWood);

            // Seat frame & upholstered silk cushion with delicate gold piping
            Box("Chair seat frame", Vector3.up * .43f, new Vector3(.46f, .05f, .46f), S.DarkWood);
            Box("Chair silk cushion", Vector3.up * .485f, new Vector3(.44f, .08f, .44f), cushionMat);
            Box("Cushion gold piping", Vector3.up * .485f, new Vector3(.455f, .02f, .455f), goldSilk);

            // ATTENDANT BACKREST (Graceful medium-tall height, ~1.12m, with yellow royal silk upholstery)
            for (int s = -1; s <= 1; s += 2)
            {
                Box("Back upright", new Vector3(s * .18f, .76f, .19f), new Vector3(.04f, .60f, .04f), S.DarkWood);
                Ball("Upright finial", new Vector3(s * .18f, 1.08f, .19f), Vector3.one * .05f, S.Brass);
            }

            // Elegant carved top rail connecting uprights
            Box("Backrest top rail", new Vector3(0, 1.04f, .19f), new Vector3(.38f, .05f, .035f), S.DarkWood);
            Ball("Backrest carved crest", new Vector3(0, 1.08f, .19f), new Vector3(.10f, .05f, .03f), S.Brass);

            // Carved backrest frame
            Box("Backrest frame", new Vector3(0, .76f, .19f), new Vector3(.34f, .52f, .030f), S.DarkWood);

            // Silk upholstery on front face of backrest (all royal yellow gold silk)
            Box("Backrest silk upholstery", new Vector3(0, .76f, .172f), new Vector3(.29f, .46f, .015f), backMat);

            // Decorative ceremonial ribbon sash wrapped across rear of backrest
            Box("Backrest sash ribbon", new Vector3(0, .68f, .192f), new Vector3(.35f, .07f, .018f), sashMat);

            // Silk rosette floral knot centered on sash
            Ball("Backrest silk rosette", new Vector3(0, .68f, .21f), Vector3.one * .06f, rosetteMat);

            var chairGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(chairGroup, addCollider: true);
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

        static void BuildTraditionalMusic(Vector3 center)
        {
            var musicGroup = ModelGroup("Traditional Cambodian wedding music ensemble - Pinpeat", center);
            var prevParent = Parent;
            Parent = musicGroup;

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);

            // MUSICIAN PLATFORM EQUAL TO HALF OF STAGE FRONT-BACK (5.5m wide x 3.5m deep)
            Box("Musician platform border", Vector3.up * .01f, new Vector3(5.6f, .02f, 3.6f), S.DarkWood);
            Grid("Musician woven reed mat", 24, 16, (u, v) => new Vector3((u - .5f) * 5.4f, .02f, (v - .5f) * 3.4f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f), new Vector2(4, 3), true);
            Grid("Musician ceremonial crimson carpet", 20, 14, (u, v) => new Vector3((u - .5f) * 5.0f, .025f, (v - .5f) * 3.0f), redSilk, new Vector2(3, 2), true);

            // 6 SEATED MUSICIAN POSITIONS (2 rows of 3, each with large silk cushion and ample sitting room)
            // BACK ROW (z = +0.55m):
            // 1. Left: Roneat Ek (Boat Xylophone)
            // 2. Center: Chapei Dang Veng (Cambodian Long-Neck Lute)
            // 3. Right: Tro Sau & Tro Ou (Two-String Spike Fiddles)
            // FRONT ROW (z = -0.55m):
            // 4. Left: Skor Samphor (Ceremonial Barrel Drum)
            // 5. Center: Khloy (Bamboo Flutes on presentation stand)
            // 6. Right: Chhing & Bronze Temple Gong (Hand cymbals & gong)

            // --- MUSICIAN 1: RONEAT EK (Xylophone) ---
            var m1Pos = new Vector3(-1.55f, 0, .55f);
            Box("Musician 1 silk cushion", m1Pos + Vector3.up * .045f, new Vector3(.70f, .06f, .62f), goldSilk);
            var roneatP = m1Pos + new Vector3(0, 0, .58f);
            Cylinder("Roneat pedestal stand", roneatP + Vector3.up * .04f, .22f, .07f, S.DarkWood, 16);
            Box("Roneat carved boat resonator", roneatP + Vector3.up * .16f, new Vector3(1.15f, .22f, .28f), S.DarkWood, true);
            for (int key = 0; key < 18; key++)
            {
                float kx = -.52f + key * .061f;
                float ky = .30f + Mathf.Pow((key / 17f - .5f) * 2, 2) * .15f;
                Box("Roneat bamboo key", roneatP + new Vector3(kx, ky, 0), new Vector3(.05f, .015f, .26f - Mathf.Abs(key - 8.5f) * .006f), S.Wood);
            }
            Beam("Roneat mallet 1", roneatP + new Vector3(-.14f, .42f, .16f), roneatP + new Vector3(-.14f, .38f, .02f), .012f, S.Wood);
            Beam("Roneat mallet 2", roneatP + new Vector3(.14f, .42f, .16f), roneatP + new Vector3(.14f, .38f, .02f), .012f, S.Wood);

            // --- MUSICIAN 2: CHAPEI DANG VENG (UNESCO Long-Necked Lute) ---
            var m2Pos = new Vector3(0, 0, .55f);
            Box("Musician 2 silk cushion", m2Pos + Vector3.up * .045f, new Vector3(.70f, .06f, .62f), goldSilk);
            var chapeiP = m2Pos + new Vector3(0, 0, .52f);
            Box("Chapei low resting cradle", chapeiP + Vector3.up * .05f, new Vector3(.24f, .08f, .14f), S.DarkWood);
            Ball("Chapei soundbox body", chapeiP + Vector3.up * .13f, new Vector3(.38f, .10f, .48f), S.DarkWood);
            Box("Chapei soundboard face", chapeiP + Vector3.up * .18f, new Vector3(.34f, .01f, .44f), S.Wood);
            Beam("Chapei long slender neck", chapeiP + new Vector3(0, .18f, .22f), chapeiP + new Vector3(0, .32f, 1.25f), .034f, S.DarkWood);
            Grid("Chapei carved naga headstock", 12, 2, (u, v) => chapeiP + new Vector3((u - .5f) * .07f, .32f + Mathf.Sin(u * Mathf.PI) * .14f + v * .025f, 1.25f + u * .15f), S.DarkWood, Vector2.one, true);
            Ball("Chapei brass finial", chapeiP + new Vector3(0, .46f, 1.40f), Vector3.one * .045f, S.Brass);
            Beam("Chapei peg 1", chapeiP + new Vector3(-.07f, .34f, 1.28f), chapeiP + new Vector3(.07f, .34f, 1.28f), .012f, S.Wood);
            Beam("Chapei peg 2", chapeiP + new Vector3(-.07f, .38f, 1.34f), chapeiP + new Vector3(.07f, .38f, 1.34f), .012f, S.Wood);
            Beam("Chapei silk string 1", chapeiP + new Vector3(-.008f, .19f, -.16f), chapeiP + new Vector3(-.008f, .33f, 1.26f), .004f, goldSilk);
            Beam("Chapei silk string 2", chapeiP + new Vector3(.008f, .19f, -.16f), chapeiP + new Vector3(.008f, .33f, 1.26f), .004f, goldSilk);

            // --- MUSICIAN 3: TRO SAU & TRO OU (Two-String Spike Fiddles) ---
            var m3Pos = new Vector3(1.55f, 0, .55f);
            Box("Musician 3 silk cushion", m3Pos + Vector3.up * .045f, new Vector3(.70f, .06f, .62f), goldSilk);
            var troP = m3Pos + new Vector3(0, 0, .50f);
            // Tro Ou (Coconut shell resonator)
            Ball("Tro Ou coconut body", troP + new Vector3(-.18f, .14f, 0), new Vector3(.18f, .18f, .16f), S.Plain("Aged coconut shell", new Color(.30f, .18f, .10f), .3f));
            Beam("Tro Ou hardwood spike neck", troP + new Vector3(-.18f, .02f, 0), troP + new Vector3(-.18f, .78f, 0), .026f, S.DarkWood);
            Beam("Tro Ou tuning peg 1", troP + new Vector3(-.26f, .68f, 0), troP + new Vector3(-.10f, .68f, 0), .014f, S.Wood);
            Beam("Tro Ou horsehair bow", troP + new Vector3(-.32f, .26f, .08f), troP + new Vector3(-.04f, .26f, -.04f), .012f, S.Wood);
            // Tro Sau (Bamboo/Hardwood cylindrical resonator)
            Cylinder("Tro Sau resonator body", troP + new Vector3(.18f, .12f, 0), .065f, .18f, S.Wood, 16);
            Beam("Tro Sau hardwood spike neck", troP + new Vector3(.18f, .02f, 0), troP + new Vector3(.18f, .82f, 0), .024f, S.DarkWood);
            Beam("Tro Sau tuning peg 1", troP + new Vector3(.10f, .72f, 0), troP + new Vector3(.26f, .72f, 0), .014f, S.Wood);
            Beam("Tro Sau horsehair bow", troP + new Vector3(.04f, .24f, .08f), troP + new Vector3(.32f, .24f, -.04f), .012f, S.Wood);

            // --- MUSICIAN 4: SKOR SAMPHOR (Double-Headed Barrel Drum) ---
            var m4Pos = new Vector3(-1.55f, 0, -.55f);
            Box("Musician 4 silk cushion", m4Pos + Vector3.up * .045f, new Vector3(.70f, .06f, .62f), goldSilk);
            var drumP = m4Pos + new Vector3(0, 0, -.48f);
            Box("Samphor cradle stand left", drumP + new Vector3(-.20f, .16f, 0), new Vector3(.05f, .32f, .28f), S.DarkWood);
            Box("Samphor cradle stand right", drumP + new Vector3(.20f, .16f, 0), new Vector3(.05f, .32f, .28f), S.DarkWood);
            Beam("Samphor cradle stretcher", drumP + new Vector3(-.20f, .06f, 0), drumP + new Vector3(.20f, .06f, 0), .035f, S.DarkWood);
            Cylinder("Samphor barrel drum body", drumP + Vector3.up * .26f, .19f, .44f, S.DarkWood, 20);
            Ball("Samphor leather head 1", drumP + new Vector3(0, .26f, -.225f), new Vector3(.35f, .35f, .04f), S.Clay);
            Ball("Samphor leather head 2", drumP + new Vector3(0, .26f, .225f), new Vector3(.35f, .35f, .04f), S.Clay);
            Cylinder("Samphor tuning center paste disk", drumP + Vector3.up * .39f, .065f, .015f, S.Black, 16);

            // --- MUSICIAN 5: KHLOY (Traditional Khmer Bamboo Flutes on Stand) ---
            var m5Pos = new Vector3(0, 0, -.55f);
            Box("Musician 5 silk cushion", m5Pos + Vector3.up * .045f, new Vector3(.70f, .06f, .62f), goldSilk);
            var fluteP = m5Pos + new Vector3(0, 0, -.48f);
            Box("Flute carved wooden tray", fluteP + Vector3.up * .06f, new Vector3(.66f, .04f, .26f), S.Plain("Lacquer tray red", new Color(.62f, .12f, .15f), .4f));
            // Khloy Ek (Lead flute)
            Beam("Khloy Ek bamboo body", fluteP + new Vector3(-.26f, .10f, -.04f), fluteP + new Vector3(.26f, .10f, -.04f), .022f, S.Wood);
            Ball("Khloy Ek fipple mouthpiece", fluteP + new Vector3(-.26f, .10f, -.04f), Vector3.one * .026f, S.DarkWood);
            // Khloy Thom (Bass flute)
            Beam("Khloy Thom bamboo body", fluteP + new Vector3(-.28f, .10f, .04f), fluteP + new Vector3(.28f, .10f, .04f), .028f, S.Wood);
            Ball("Khloy Thom fipple mouthpiece", fluteP + new Vector3(-.28f, .10f, .04f), Vector3.one * .032f, S.DarkWood);

            // --- MUSICIAN 6: CHHING & CEREMONIAL BRONZE GONG ---
            var m6Pos = new Vector3(1.55f, 0, -.55f);
            Box("Musician 6 silk cushion", m6Pos + Vector3.up * .045f, new Vector3(.70f, .06f, .62f), goldSilk);
            var chhingP = m6Pos + new Vector3(-.20f, 0, -.48f);
            // Chhing hand cymbals on golden pedestal tray
            Lathe("Chhing brass pedestal dish", chhingP, new[]{ new Vector2(.05f, 0), new Vector2(.025f, .05f), new Vector2(.10f, .09f) }, S.Brass, 16);
            Cylinder("Chhing brass cymbal 1", chhingP + new Vector3(0, .105f, -.035f), .065f, .022f, S.Brass, 18);
            Cylinder("Chhing brass cymbal 2", chhingP + new Vector3(0, .105f, .035f), .065f, .022f, S.Brass, 18);
            Beam("Chhing red silk cord", chhingP + new Vector3(0, .12f, -.035f), chhingP + new Vector3(0, .12f, .035f), .008f, redSilk);

            // Suspended Bronze Temple Gong on Carved Frame
            var gongP = m6Pos + new Vector3(.22f, 0, -.48f);
            Cylinder("Gong frame post left", gongP + new Vector3(-.25f, 0, 0), .035f, 1.25f, S.DarkWood, 12);
            Cylinder("Gong frame post right", gongP + new Vector3(.25f, 0, 0), .035f, 1.25f, S.DarkWood, 12);
            Beam("Gong top crossbar", gongP + new Vector3(-.28f, 1.22f, 0), gongP + new Vector3(.28f, 1.22f, 0), .045f, S.DarkWood);
            Cylinder("Suspended bronze gong", gongP + Vector3.up * .78f, .22f, .035f, S.Brass, 24);
            Ball("Gong center boss nipple", gongP + new Vector3(0, .78f, -.022f), new Vector3(.075f, .075f, .05f), S.Brass);
            Beam("Gong padded mallet", gongP + new Vector3(0, .45f, -.08f), gongP + new Vector3(0, .25f, 0), .016f, S.Wood);

            // Center ensemble offering bowl with jasmine and incense
            Lathe("Ensemble blessing incense bowl", new Vector3(0, 0, 0), new[]{ new Vector2(.06f, 0), new Vector2(.03f, .05f), new Vector2(.12f, .10f) }, S.Brass, 16);
            Ball("Incense jasmine flowers", new Vector3(0, .11f, 0), Vector3.one * .08f, S.Plain("Jasmine white", Color.white, .2f));

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
        /// Positioned in front of the ceremony stage covering the full audience pavilion.
        /// Replaces the removed pink aisle with center seating for guests (10 columns across x 5 rows = 50 chairs).
        /// </summary>
        static void BuildWeddingAudienceChairs()
        {
            var old = Parent;

            float[] rowZs = { -4.5f, -6.2f, -7.9f, -9.6f, -11.3f }; // 5 Rows

            // Columns spanning left wing, center (replacing pink aisle), and right wing:
            float[] colXs = { 8.6f, 9.7f, 10.8f, 11.9f, 12.7f, 13.7f, 14.5f, 15.6f, 16.7f, 17.8f };
            float[] colYaws = { 20f, 16f, 12f, 8f, 2f, -2f, -8f, -12f, -16f, -20f };

            for (int r = 0; r < rowZs.Length; r++)
            {
                float z = rowZs[r];
                for (int c = 0; c < colXs.Length; c++)
                {
                    WeddingGuestChair(new Vector3(colXs[c], 0, z), colYaws[c]);
                }
            }

            Parent = old;
        }

        static void WeddingGuestChair(Vector3 p, float yaw)
        {
            var old = Parent;
            Parent = ModelGroup("Wedding ceremony audience chair with fabric cover", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);

            // Inner chair structural frame (underneath fabric)
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Box("Chair leg", new Vector3(x * .18f, .22f, z * .18f), new Vector3(.045f, .44f, .045f), S.DarkWood);

            // 1. Padded fabric seat cover (matching banquet yard dusty rose fabric)
            Box("Fabric seat cover", new Vector3(0, .45f, 0), new Vector3(.46f, .05f, .46f), S.Pink);

            // 2. Full-length draped fabric skirt covering the 4 legs down to the ground
            Grid("Front fabric skirt", 16, 6, (u, v) => new Vector3((u - .5f) * .46f + Mathf.Sin(u * Mathf.PI * 4) * .015f * (1 - v), .44f - v * .41f, .23f + Mathf.Sin(v * Mathf.PI * .5f) * .015f), S.Pink, new Vector2(1, 2), true);
            Grid("Rear fabric skirt", 16, 6, (u, v) => new Vector3((u - .5f) * .46f + Mathf.Sin(u * Mathf.PI * 4) * .015f * (1 - v), .44f - v * .41f, -.24f - Mathf.Sin(v * Mathf.PI * .5f) * .015f), S.Pink, new Vector2(1, 2), true);
            Grid("Left fabric skirt", 16, 6, (u, v) => new Vector3(-.23f - Mathf.Sin(v * Mathf.PI * .5f) * .015f, .44f - v * .41f, (u - .5f) * .46f + Mathf.Sin(u * Mathf.PI * 4) * .015f * (1 - v)), S.Pink, new Vector2(1, 2), true);
            Grid("Right fabric skirt", 16, 6, (u, v) => new Vector3(.23f + Mathf.Sin(v * Mathf.PI * .5f) * .015f, .44f - v * .41f, (u - .5f) * .46f + Mathf.Sin(u * Mathf.PI * 4) * .015f * (1 - v)), S.Pink, new Vector2(1, 2), true);

            // 3. Fitted fabric backrest slipcover (enveloping back at local z = -0.19m)
            Box("Fabric backrest front", new Vector3(0, .69f, -.175f), new Vector3(.44f, .50f, .03f), S.Pink);
            Box("Fabric backrest rear", new Vector3(0, .69f, -.215f), new Vector3(.44f, .50f, .03f), S.Pink);
            Grid("Fabric backrest crest", 16, 3, (u, v) => new Vector3((u - .5f) * .44f, .90f + Mathf.Sin(u * Mathf.PI) * .07f + v * .03f, -.195f), S.Pink, Vector2.one, true);

            // 4. Ceremonial Silk Ribbon Sash & Bow Rosette
            // Gold silk sash band wrapped around the backrest
            Box("Backrest gold silk sash band", new Vector3(0, .62f, -.195f), new Vector3(.46f, .085f, .05f), goldSilk);

            // Ornate bow knot and ribbon tails on the back (facing audience entrance at local -Z)
            Ball("Sash knot rosette", new Vector3(0, .62f, -.24f), Vector3.one * .07f, redSilk);
            Tube("Bow loop left", new Vector3(-.07f, .62f, -.25f), .055f, .016f, goldSilk, Quaternion.Euler(0, 30f, 0), 12);
            Tube("Bow loop right", new Vector3(.07f, .62f, -.25f), .055f, .016f, goldSilk, Quaternion.Euler(0, -30f, 0), 12);
            Beam("Bow ribbon tail 1", new Vector3(-.03f, .60f, -.25f), new Vector3(-.06f, .38f, -.26f), .035f, goldSilk);
            Beam("Bow ribbon tail 2", new Vector3(.03f, .60f, -.25f), new Vector3(.06f, .38f, -.26f), .035f, goldSilk);

            var chairGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(chairGroup, addCollider: true);
        }
    }
}
