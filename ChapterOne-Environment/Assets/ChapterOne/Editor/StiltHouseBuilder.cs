using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class StiltHouseBuilder
    {
        public static Transform Build()
        {
            Parent = null;
            var root = Group("01 HOUSE - raised timber home", SiteLayout.HousePosition);
            root.localRotation = SiteLayout.HouseRotation;
            root.localScale = Vector3.one * SiteLayout.HouseScale;
            Parent = root;

            float halfWidth = SiteLayout.HouseWidth / 2, halfDepth = SiteLayout.HouseDepth / 2;

            // 1. Foundation footings and main stilt posts
            // NO PILLAR IN THE MIDDLE OF THE HOUSE:
            // x == 0 && z == 0 is supported underneath the floor only, leaving the interior living room 100% open!
            var frameGroup = ModelGroup("Stilt house timber foundation and posts", Vector3.zero);
            var prevParent = Parent;
            Parent = frameGroup;
            for (int x = -1; x <= 1; x++)
                for (int z = -1; z <= 1; z++)
                {
                    var p = new Vector3(x * (halfWidth - .5f), 0, z * (halfDepth - .55f));
                    Box("Concrete footing", p + Vector3.up * .2f, new Vector3(.5f, .4f, .5f), S.Metal, true);

                    if (x == 0 && z == 0)
                    {
                        // Structural under-floor post only! Stops at bottom of floor (2.4m) so it NEVER cuts into the living hall!
                        Box("Under-floor central support post", p + Vector3.up * 1.2f, new Vector3(.23f, 2.4f, .23f), S.DarkWood, true);
                        continue;
                    }

                    Box("Weathered structural post", p + Vector3.up * 2.6f, new Vector3(.23f, 5.2f, .23f), S.DarkWood, true);
                    Beam("Diagonal timber brace", p + Vector3.up * 1.65f, p + new Vector3(.85f, 2.42f, 0), .11f, S.DarkWood);
                }

            Box("Floor collision", new Vector3(0, 2.36f, 0), new Vector3(SiteLayout.HouseWidth, .16f, SiteLayout.HouseDepth), S.DarkWood, true);
            int boards = Mathf.RoundToInt(SiteLayout.HouseWidth / .25f);
            float boardWidth = SiteLayout.HouseWidth / boards;
            for (int i = 0; i < boards; i++)
                Box("Upper floor board", new Vector3(-halfWidth + (i + .5f) * boardWidth, 2.455f, 0), new Vector3(boardWidth - .006f, .09f, SiteLayout.HouseDepth), S.Floor, false, 2.8f);
            Parent = prevParent;
            ModelCombiner.CombineModel(frameGroup, addCollider: true);

            // 2. Front Veranda (Rong Doung)
            float verandaDepth = 1.85f;
            float verandaWidth = SiteLayout.HouseWidth * 0.75f;
            float verandaZ = -halfDepth - verandaDepth / 2;
            var verandaGroup = ModelGroup("Front veranda (Rong Doung) and railing", new Vector3(0, 0, verandaZ));
            prevParent = Parent;
            Parent = verandaGroup;

            Box("Veranda floor deck", new Vector3(0, 2.455f, 0), new Vector3(verandaWidth, .09f, verandaDepth), S.Floor, true);
            for (int vx = -1; vx <= 1; vx++)
            {
                var vp = new Vector3(vx * (verandaWidth / 2 - .2f), 0, -verandaDepth / 2 + .15f);
                Box("Veranda footing", vp + Vector3.up * .2f, new Vector3(.4f, .4f, .4f), S.Metal, true);
                Box("Veranda post", vp + Vector3.up * 2.6f, new Vector3(.18f, 5.0f, .18f), S.DarkWood, true);
            }
            float stairGap = 1.7f;
            for (int side = -1; side <= 1; side += 2)
            {
                float frontSegWidth = (verandaWidth - stairGap) / 2;
                float frontSegX = side * (stairGap / 2 + frontSegWidth / 2);
                Box("Veranda handrail", new Vector3(frontSegX, 3.25f, -verandaDepth / 2 + .15f), new Vector3(frontSegWidth, .08f, .08f), S.Wood);
                Box("Veranda base rail", new Vector3(frontSegX, 2.58f, -verandaDepth / 2 + .15f), new Vector3(frontSegWidth, .06f, .06f), S.DarkWood);
                int balusterCount = Mathf.RoundToInt(frontSegWidth / .18f);
                for (int b = 0; b < balusterCount; b++)
                {
                    float bx = frontSegX - frontSegWidth / 2 + (b + .5f) * (frontSegWidth / balusterCount);
                    Box("Veranda baluster", new Vector3(bx, 2.91f, -verandaDepth / 2 + .15f), new Vector3(.04f, .6f, .04f), S.Wood);
                }

                float sideX = side * (verandaWidth / 2 - .15f);
                Box("Veranda side handrail", new Vector3(sideX, 3.25f, 0), new Vector3(.08f, .08f, verandaDepth), S.Wood);
                Box("Veranda side base rail", new Vector3(sideX, 2.58f, 0), new Vector3(.06f, .06f, verandaDepth), S.DarkWood);
                for (int b = 0; b < 9; b++)
                {
                    float bz = -verandaDepth / 2 + .2f + b * (verandaDepth - .4f) / 8f;
                    Box("Veranda side baluster", new Vector3(sideX, 2.91f, bz), new Vector3(.04f, .6f, .04f), S.Wood);
                }
            }
            Roof(new Vector3(0, 5.0f, 0), verandaWidth + .6f, verandaDepth + .6f, 1.4f, S.Roof, false);
            Parent = prevParent;
            ModelCombiner.CombineModel(verandaGroup, addCollider: true);

            // 3. Wooden Entrance Staircase
            float stairStartZ = verandaZ - verandaDepth / 2;
            var stairGroup = ModelGroup("Wooden entrance staircase", Vector3.zero);
            prevParent = Parent;
            Parent = stairGroup;
            Box("Entrance landing", new Vector3(0, 2.4f, stairStartZ + .2f), new Vector3(1.8f, .18f, .5f), S.Floor, true);
            for (int i = 0; i < 14; i++)
                Box("Solid stair tread", new Vector3(0, .085f + i * .175f, stairStartZ - 3.8f + i * .265f), new Vector3(1.65f, .16f, .28f), S.Wood, true);
            for (int side = -1; side <= 1; side += 2)
            {
                Beam("Stair side stringer", new Vector3(side * .76f, .02f, stairStartZ - 3.9f), new Vector3(side * .76f, 2.42f, stairStartZ + .1f), .13f, S.DarkWood);
                Beam("Worn stair handrail", new Vector3(side * .91f, .87f, stairStartZ - 3.9f), new Vector3(side * .91f, 3.27f, stairStartZ + .1f), .08f, S.Wood);
                for (int i = 0; i < 7; i++)
                    Box("Stair baluster", new Vector3(side * .91f, .55f + i * .35f, stairStartZ - 3.7f + i * .54f), new Vector3(.045f, .8f, .045f), S.Wood);
            }
            Parent = prevParent;
            ModelCombiner.CombineModel(stairGroup, addCollider: true);

            PropsBuilder.FootWashStation(new Vector3(1.45f, .02f, stairStartZ - 3.9f));
            PropsBuilder.SpiritHouse(new Vector3(-halfWidth + 2.8f, .02f, -halfDepth - 3.8f), 35f);

            // 4. Exterior Walls
            Wall(new Vector3(0, 2.5f, -halfDepth), 0, SiteLayout.HouseWidth, doorway: true, windows: true);
            Wall(new Vector3(0, 2.5f, halfDepth), 180, SiteLayout.HouseWidth, doorway: false, windows: true);
            Wall(new Vector3(-halfWidth, 2.5f, 0), 90, SiteLayout.HouseDepth, doorway: false, windows: true);
            Wall(new Vector3(halfWidth, 2.5f, 0), -90, SiteLayout.HouseDepth, doorway: false, windows: true);

            // Double wooden door panels
            var doorsGroup = ModelGroup("Double wooden entrance doors", new Vector3(0, 2.5f, -halfDepth));
            prevParent = Parent;
            Parent = doorsGroup;
            var leftDoor = Group("Left door leaf", new Vector3(-.56f, 1.18f, 0));
            leftDoor.localEulerAngles = new Vector3(0, -22f, 0);
            Parent = leftDoor;
            Box("Door panel left", new Vector3(.27f, 0, 0), new Vector3(.55f, 2.2f, .04f), S.DarkWood, true);
            Tube("Door brass pull ring left", new Vector3(.48f, 0, -.035f), .045f, .008f, S.Brass, Quaternion.identity, 16);
            Parent = doorsGroup;

            var rightDoor = Group("Right door leaf", new Vector3(.56f, 1.18f, 0));
            rightDoor.localEulerAngles = new Vector3(0, 28f, 0);
            Parent = rightDoor;
            Box("Door panel right", new Vector3(-.27f, 0, 0), new Vector3(.55f, 2.2f, .04f), S.DarkWood, true);
            Tube("Door brass pull ring right", new Vector3(-.48f, 0, -.035f), .045f, .008f, S.Brass, Quaternion.identity, 16);
            Parent = prevParent;
            ModelCombiner.CombineModel(doorsGroup, addCollider: true);

            // 5. Roof Structure (Tie beams, rafters, and hip roof)
            var roofFrameGroup = ModelGroup("Exposed roof trusses and rafters", Vector3.zero);
            prevParent = Parent;
            Parent = roofFrameGroup;
            for (int j = 0; j < 9; j++)
            {
                float x = Mathf.Lerp(-halfWidth + .4f, halfWidth - .4f, j / 8f);
                Beam("Roof tie beam", new Vector3(x, 5.05f, -halfDepth - .1f), new Vector3(x, 5.05f, halfDepth + .1f), .16f, S.DarkWood);
                float h = 5.18f + 2.2f * Mathf.Clamp01((halfWidth + .5f - Mathf.Abs(x)) / ((SiteLayout.HouseDepth + 1.2f) * .35f));
                Beam("Exposed rafter", new Vector3(x, 5.14f, -halfDepth - .2f), new Vector3(x, h, 0), .095f, S.DarkWood);
                Beam("Exposed rafter", new Vector3(x, 5.14f, halfDepth + .2f), new Vector3(x, h, 0), .095f, S.DarkWood);
            }
            Parent = prevParent;
            ModelCombiner.CombineModel(roofFrameGroup, addCollider: false);

            Roof(new Vector3(0, 5.25f, 0), SiteLayout.HouseWidth + 1.2f, SiteLayout.HouseDepth + 1.2f, 2.2f, S.Roof, true);

            // 6. Living Room Interior
            const float partitionZ = 1f;
            const float doorHalfWidth = .6f;
            float bedroomCenter = halfWidth / 2;

            // Interior bedroom partition wall
            var partitionGroup = ModelGroup("Bedroom partition wall and headers", Vector3.zero);
            prevParent = Parent;
            Parent = partitionGroup;
            for (int side = -1; side <= 1; side += 2)
            {
                float doorX = side * bedroomCenter;
                Box("Bedroom doorway header", new Vector3(doorX, 4.975f, partitionZ), new Vector3(1.2f, .45f, .12f), S.Wood, true);
                for (int edge = -1; edge <= 1; edge += 2)
                    Box("Bedroom door jamb", new Vector3(doorX + edge * .64f, 3.62f, partitionZ), new Vector3(.08f, 2.24f, .16f), S.DarkWood, true);
                float outerWidth = halfWidth - bedroomCenter - doorHalfWidth;
                Box("Bedroom outer partition", new Vector3(side * (halfWidth - outerWidth / 2), 3.85f, partitionZ), new Vector3(outerWidth, 2.7f, .10f), S.Wood, true);
            }
            Box("Bedroom central partition", new Vector3(0, 3.85f, partitionZ), new Vector3(bedroomCenter * 2 - doorHalfWidth * 2, 2.7f, .10f), S.Wood, true);
            Box("Bedroom dividing wall", new Vector3(0, 3.85f, partitionZ + (halfDepth - partitionZ) / 2), new Vector3(.10f, 2.7f, halfDepth - partitionZ), S.Wood, true);
            Parent = prevParent;
            ModelCombiner.CombineModel(partitionGroup, addCollider: true);

            // Living Room Ancestral Altar Shelf on rear partition wall
            PropsBuilder.AltarShelf(new Vector3(0, 4.15f, partitionZ - .08f));

            // Scene 03 Key Prop: 2006 classic button mobile phone on small side shelf
            PropsBuilder.MobilePhone2006(new Vector3(1.85f, 3.32f, -1.80f), 18f);

            // WEDDING DECORATION A: Fruit Procession Offering Display (Chhoeng Phle Chheu)
            BuildFruitProcession(new Vector3(-2.8f, 2.5f, -1.4f));

            // WEDDING DECORATION B: Authentic Cambodian Wrist-Tying Ceremony Altar (Sen Chang Dai / Sompeas Ptem)
            BuildAuthenticCambodianWristTying(new Vector3(0, 2.5f, -.15f));

            // Festive silk wall swags and rosettes along interior wall headers
            BuildInteriorWeddingDrapes(partitionZ, halfWidth);

            // 7. Bedrooms
            BedroomBuilder.Build("Bedroom 1 - bridal suite", -bedroomCenter, halfDepth, bridal: true);
            BedroomBuilder.Build("Bedroom 2 - family room", bedroomCenter, halfDepth, bridal: false);

            // 8. Ground-level Under-house living area
            UnderHouseSittingBuilder.Build(new Vector3(-4, .03f, -3.8f));
            PropsBuilder.Jar(new Vector3(-3.5f, .03f, -2), .9f);
            PropsBuilder.Basket(new Vector3(-3, .03f, .5f), .75f);
            PropsBuilder.Basket(new Vector3(-2.1f, .03f, .7f), .95f);
            PropsBuilder.Bench(new Vector3(-2, 0, 2.4f), 2.5f);
            Grid("Hanging hammock", 32, 12, (u, v) => new Vector3(-.1f + u * 3.4f, 1.55f - Mathf.Sin(u * Mathf.PI) * .68f + Mathf.Abs(v - .5f) * .23f, 1.8f + (v - .5f) * .9f), S.Cream, new Vector2(3, 1), true);

            // Lighting
            LightingBuilder.Bulb(new Vector3(-.25f, 4.8f, -halfDepth - .2f), 6.8f, 3.5f, true);
            LightingBuilder.Bulb(new Vector3(0, 4.5f, -.15f), 6.5f, 2.8f, false);

            Parent = null;
            return root;
        }

        static void BuildFruitProcession(Vector3 p)
        {
            var fruitGroup = ModelGroup("Wedding fruit procession offering display - Chhoeng Phle Chheu", p);
            var prevParent = Parent;
            Parent = fruitGroup;

            // Long woven bamboo/reed mat
            Box("Fruit mat floor runner", Vector3.up * .01f, new Vector3(2.4f, .02f, 3.6f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f));

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var mangoMat = S.Plain("Ripe mango orange", new Color(.92f, .55f, .10f), .35f);
            var dragonMat = S.Plain("Dragon fruit red", new Color(.85f, .15f, .35f), .35f);
            var bananaMat = S.Plain("Auspicious banana yellow", new Color(.88f, .78f, .18f), .25f);
            var pomeloMat = S.Plain("Fresh green pomelo", new Color(.38f, .62f, .16f), .25f);

            Material[] fruitMats = { mangoMat, dragonMat, bananaMat, pomeloMat };

            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                {
                    var tp = new Vector3(-.85f + c * .56f, .02f, -1.35f + r * .85f);
                    Lathe("Fruit pedestal tray", tp, new[]{
                        new Vector2(.08f, 0), new Vector2(.035f, .06f), new Vector2(.20f, .14f), new Vector2(.22f, .16f)
                    }, (r + c) % 2 == 0 ? S.Brass : S.Metal, 18);

                    var fMat = fruitMats[(r + c) % 4];
                    for (int f = 0; f < 5; f++)
                    {
                        float fa = f * Mathf.PI * 2 / 5f;
                        Ball("Offering fruit", tp + new Vector3(Mathf.Sin(fa) * .09f, .19f, Mathf.Cos(fa) * .09f), Vector3.one * .11f, fMat);
                    }
                    Ball("Apex blessing fruit", tp + Vector3.up * .28f, Vector3.one * .10f, fMat);

                    Tube("Tray gold ribbon wrap", tp + Vector3.up * .16f, .22f, .025f, goldSilk, Quaternion.identity, 16);
                    Ball("Ribbon gold rosette", tp + new Vector3(0, .16f, -.21f), Vector3.one * .045f, goldSilk);
                }

            Parent = prevParent;
            ModelCombiner.CombineModel(fruitGroup, addCollider: true);
        }

        /// <summary>
        /// Authentic Cambodian Wrist-Tying Ceremony Altar (Sen Chang Dai / Sompeas Ptem).
        /// Rebuilt to strictly adhere to authentic Khmer wedding traditions:
        /// 1. Couple kneels on red/gold silk mats.
        /// 2. Forearms rest forward across the signature long horizontal bolster (Khnaoy Sompeas Ptem).
        /// 3. Red blessing strings (Khsae Chang Dai) with pinned traditional money banknotes (Cham Noung Dai).
        /// 4. Sacred ancestral bronze sword (Preah Khan) rested in front of the groom.
        /// 5. Traditional Slator (betel/areca cone towers) and 5-tiered Bay Sey.
        /// 6. Popil candleholders, holy water bowl, ancestral chicken feast platter, and floor seating for elders.
        /// </summary>
        static void BuildAuthenticCambodianWristTying(Vector3 p)
        {
            var ceremonyGroup = ModelGroup("Sacred wrist-tying ceremony altar - Sen Chang Dai", p);
            var prevParent = Parent;
            Parent = ceremonyGroup;

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);

            // 1. Base: Double woven reed mat + ceremonial red & gold silk carpet
            Box("Ceremony woven floor mat", Vector3.up * .01f, new Vector3(3.4f, .02f, 2.8f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f));
            Grid("Ceremonial red silk carpet", 18, 14, (u, v) => new Vector3((u - .5f) * 3.0f, .025f, (v - .5f) * 2.4f), redSilk, new Vector2(3, 2), true);

            // 2. Twin Royal Kneeling Cushions for Bride (left) and Groom (right)
            for (int side = -1; side <= 1; side += 2)
            {
                var cushionPos = new Vector3(side * .48f, .03f, -.45f);
                Box("Kneeling silk cushion", cushionPos + Vector3.up * .075f, new Vector3(.64f, .14f, .64f), goldSilk, true);
                Box("Cushion crimson border", cushionPos + Vector3.up * .075f, new Vector3(.68f, .04f, .68f), redSilk);
            }

            // 3. Signature Cambodian Wrist-Resting Bolster (Khnaoy Sompeas Ptem / Khnaoy Chang Dai)
            // Long horizontal bolster placed directly in front of couple where their forearms/wrists rest
            var bolsterPos = new Vector3(0, .03f, .02f);
            Cylinder("Wrist resting bolster body", bolsterPos + Vector3.up * .14f, .12f, 1.38f, goldSilk, 20);
            // Red silk embroidered end caps
            for (int cap = -1; cap <= 1; cap += 2)
            {
                Ball("Bolster silk end cap", bolsterPos + new Vector3(cap * .70f, .14f, 0), new Vector3(.06f, .24f, .24f), redSilk);
                Tube("Bolster gold tassel ring", bolsterPos + new Vector3(cap * .68f, .14f, 0), .125f, .02f, goldSilk, Quaternion.Euler(0, 90, 0), 16);
            }

            // 4. Consecrated Red Yarn Strings (Khsae Chang Dai) & Money Blessing (Cham Noung Dai)
            // Silver chalice filled with red strings
            var stringChalicePos = new Vector3(.85f, .03f, .12f);
            Lathe("Silver string chalice", stringChalicePos, new[]{
                new Vector2(.08f, 0), new Vector2(.04f, .08f), new Vector2(.18f, .18f), new Vector2(.16f, .22f)
            }, S.Metal, 20);
            Ball("Consecrated red strings bundle", stringChalicePos + Vector3.up * .18f, new Vector3(.24f, .14f, .24f), redSilk);

            // Traditional Cambodian Banknotes (Cham Noung Dai) laid on silver blessing tray
            var moneyTrayPos = new Vector3(.85f, .03f, -.32f);
            Lathe("Blessing money tray", moneyTrayPos, new[]{ new Vector2(0, 0), new Vector2(.14f, 0), new Vector2(.18f, .025f) }, S.Metal, 18);
            // Stacked crisp Riel and USD banknotes
            Box("Blessing banknotes stack", moneyTrayPos + Vector3.up * .035f, new Vector3(.18f, .025f, .10f), S.Plain("Cambodian Riel banknotes", new Color(.48f, .62f, .48f), .15f));
            Box("Riel red blessing ribbon", moneyTrayPos + Vector3.up * .048f, new Vector3(.025f, .01f, .102f), redSilk);

            // Golden trimming scissors (Kandray Meas)
            var scissorPos = stringChalicePos + new Vector3(-.12f, .18f, .04f);
            Beam("Scissor blade 1", scissorPos, scissorPos + new Vector3(.08f, .005f, .04f), .008f, S.Brass);
            Beam("Scissor blade 2", scissorPos, scissorPos + new Vector3(.08f, .005f, -.04f), .008f, S.Brass);

            // 5. Sacred Ancestral Bronze Sword (Preah Khan Reach)
            // Rested across a red velvet cushion in front of the groom (right side)
            var swordPos = new Vector3(.38f, .03f, .26f);
            Box("Sword ceremonial velvet pillow", swordPos + Vector3.up * .03f, new Vector3(.72f, .06f, .16f), redSilk);
            Beam("Preah Khan bronze blade", swordPos + new Vector3(-.28f, .07f, 0), swordPos + new Vector3(.24f, .07f, 0), .022f, S.Brass);
            Ball("Sword ornate hilt pommel", swordPos + new Vector3(-.31f, .07f, 0), Vector3.one * .045f, S.Brass);
            Tube("Sword yellow silk ribbon", swordPos + new Vector3(-.02f, .07f, 0), .035f, .015f, goldSilk, Quaternion.Euler(0, 90, 0), 12);

            // 6. Traditional Slator (Areca & Betel Nut Towers)
            // Two authentic towers flanking the wrist bolster
            for (int side = -1; side <= 1; side += 2)
            {
                var slatorPos = new Vector3(side * .88f, .03f, -.08f);
                // Fresh banana-trunk cylindrical core wrapped in folded betel leaves
                Cylinder("Slator banana trunk core", slatorPos, .09f, .38f, S.Leaf, 16);
                // Tier of folded betel leaves (Sla Mul)
                for (int tier = 0; tier < 3; tier++)
                {
                    Tube("Betel leaf ring", slatorPos + Vector3.up * (.08f + tier * .11f), .11f, .03f, S.Leaf, Quaternion.identity, 16);
                }
                // Crowned with fresh areca palm flowers (Pka Sla) and yellow marigolds
                Ball("Pka Sla areca palm flower crown", slatorPos + Vector3.up * .42f, new Vector3(.14f, .16f, .14f), S.Cream);
                Ball("Marigold top flower", slatorPos + Vector3.up * .50f, Vector3.one * .075f, S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f));
            }

            // 7. Dual 5-Tiered Banana-Leaf Offering Cones (Bay Sey 5 Chon)
            for (int side = -1; side <= 1; side += 2)
            {
                var baySeyPos = new Vector3(side * 1.25f, .03f, .22f);
                Lathe("Bay Sey brass pedestal", baySeyPos, new[]{ new Vector2(.08f, 0), new Vector2(.035f, .06f), new Vector2(.16f, .12f) }, S.Brass, 16);
                for (int tier = 0; tier < 5; tier++)
                {
                    float ty = .12f + tier * .09f;
                    float r = Mathf.Lerp(.14f, .04f, tier / 4f);
                    Cylinder("Bay Sey leaf tier", baySeyPos + Vector3.up * ty, r, .08f, S.Leaf, 14);
                    Ball("Marigold flower ring", baySeyPos + Vector3.up * (ty + .075f), Vector3.one * .035f, S.Plain("Marigold flower yellow", new Color(.94f, .72f, .08f), .1f));
                }
                Ball("Bay Sey top candle", baySeyPos + Vector3.up * .58f, new Vector3(.025f, .06f, .025f), S.Cream);
            }

            // 8. Matrimonial Popil Blessing Ceremony Instruments (Tean Bongvil Popil)
            var popilTrayPos = new Vector3(-.85f, .03f, .18f);
            Lathe("Popil ceremonial tray", popilTrayPos, new[]{ new Vector2(0, 0), new Vector2(.15f, 0), new Vector2(.19f, .03f) }, S.Brass, 18);
            for (int side = -1; side <= 1; side += 2)
            {
                var popilPos = popilTrayPos + new Vector3(side * .08f, .035f, 0);
                // Leaf-shaped brass Popil holder
                Box("Popil leaf holder", popilPos + Vector3.up * .02f, new Vector3(.06f, .01f, .12f), S.Brass);
                Cylinder("Popil beeswax candle", popilPos + Vector3.up * .08f, .012f, .12f, S.Cream, 10);
                Ball("Popil candle flame glow", popilPos + Vector3.up * .15f, Vector3.one * .018f, S.Bulb);
            }

            // 9. Ancestral Feast Platter (Spei Sen Preah Phum / Neak Ta)
            var feastPos = new Vector3(-.38f, .03f, .38f);
            Lathe("Brass ancestral feast platter", feastPos, new[]{ new Vector2(0, 0), new Vector2(.18f, 0), new Vector2(.22f, .035f) }, S.Brass, 20);
            Ball("Whole boiled golden chicken", feastPos + Vector3.up * .08f, new Vector3(.22f, .12f, .16f), S.Plain("Boiled chicken golden skin", new Color(.88f, .72f, .34f), .25f));
            for (int e = 0; e < 3; e++)
            {
                float ea = e * Mathf.PI * 2 / 3f;
                Ball("Hardboiled egg", feastPos + new Vector3(Mathf.Sin(ea) * .11f, .06f, Mathf.Cos(ea) * .11f), new Vector3(.05f, .07f, .05f), S.Cream);
            }
            // Traditional sticky rice cake (Num Ansom) wrapped in banana leaves
            Cylinder("Num Ansom sticky rice cake", feastPos + new Vector3(0, .05f, -.12f), .045f, .22f, S.Leaf, 12);
            // Twin glasses of sacred rice wine (Sra Sa)
            for (int w = -1; w <= 1; w += 2)
            {
                Cylinder("Rice wine glass", feastPos + new Vector3(w * .12f, .035f, .11f), .022f, .05f, S.Porcelain, 12);
            }

            // 10. Silver Holy Water Bowl (Bat Teuk Mon) with Jasmine blossoms & sprinkler
            var waterBowlPos = new Vector3(-.85f, .03f, -.32f);
            Lathe("Holy water silver bowl", waterBowlPos, new[]{
                new Vector2(.06f, 0), new Vector2(.13f, .09f), new Vector2(.12f, .10f)
            }, S.Metal, 16);
            Cylinder("Holy water surface", waterBowlPos + Vector3.up * .075f, .11f, .01f, S.Water, 16);
            Ball("Floating jasmine flower", waterBowlPos + Vector3.up * .085f, Vector3.one * .035f, S.Cream);
            Beam("Sacred leaf sprinkler (Slek Khtom)", waterBowlPos + new Vector3(0, .09f, 0), waterBowlPos + new Vector3(-.12f, .18f, -.08f), .012f, S.Leaf);

            // 11. Brass Incense Burner (Dhup) with burning joss sticks
            var incensePos = new Vector3(0, .03f, .48f);
            Lathe("Incense burner chalice", incensePos, new[]{
                new Vector2(.05f, 0), new Vector2(.08f, .06f), new Vector2(.075f, .07f)
            }, S.Brass, 14);
            for (int inc = -1; inc <= 1; inc++)
            {
                Beam("Red incense joss stick", incensePos + new Vector3(inc * .015f, .06f, 0), incensePos + new Vector3(inc * .025f, .24f, 0), .004f, redSilk);
                Ball("Burning incense tip glow", incensePos + new Vector3(inc * .025f, .242f, 0), Vector3.one * .008f, S.Bulb);
            }

            // 12. Semicircle of 5 Plush Silk Floor Cushions for Elders sitting intimately on the timber floor
            float radius = 1.65f;
            for (int i = 0; i < 5; i++)
            {
                float angle = -1.25f + i * .625f;
                var sp = new Vector3(Mathf.Sin(angle) * radius, .03f, Mathf.Cos(angle) * (radius * .85f) - .6f);
                Box("Elder floor cushion", sp + Vector3.up * .05f, new Vector3(.44f, .10f, .44f), (i % 2 == 0) ? redSilk : goldSilk);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(ceremonyGroup, addCollider: true);
        }

        static void BuildInteriorWeddingDrapes(float partitionZ, float halfWidth)
        {
            var drapesGroup = ModelGroup("Living room festive wedding silk swags", Vector3.zero);
            var prevParent = Parent;
            Parent = drapesGroup;

            for (int side = -1; side <= 1; side += 2)
            {
                float sx = side * (halfWidth * .5f);
                Grid("Partition silk swag", 32, 6, (u, v) =>
                {
                    float x = sx + (u - .5f) * (halfWidth * .85f);
                    float y = 4.88f - Mathf.Sin(u * Mathf.PI) * .35f - v * .18f;
                    return new Vector3(x, y, partitionZ - .06f);
                }, S.Pink, new Vector2(3, 1), true);

                WeddingYardBuilder.Rosette(new Vector3(sx, 4.88f, partitionZ - .08f), .18f);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(drapesGroup, addCollider: false);
        }

        static void Wall(Vector3 p, float yaw, float width, bool doorway, bool windows)
        {
            var previous = Parent;
            Parent = ModelGroup("Vertical plank wall and shutters", p);
            Parent.localEulerAngles = new Vector3(0, yaw, 0);
            int count = Mathf.RoundToInt(width / .19f);
            for (int i = 0; i < count; i++)
            {
                float x = -width / 2 + (i + .5f) * width / count;
                if (doorway && Mathf.Abs(x) < .55f) continue;
                bool isWin = windows && (Mathf.Abs(x - width * .28f) < .45f || Mathf.Abs(x + width * .28f) < .45f);
                if (isWin)
                {
                    Box("Lower wall plank", new Vector3(x, .55f, 0), new Vector3(.185f, 1.1f, .035f), S.Wood);
                    Box("Upper wall header plank", new Vector3(x, 2.35f, 0), new Vector3(.185f, .5f, .035f), S.Wood);
                    if (Mathf.Abs(x - width * .28f) < .15f || Mathf.Abs(x + width * .28f) < .15f)
                    {
                        Box("Window sill", new Vector3(x, 1.08f, 0), new Vector3(.85f, .06f, .12f), S.DarkWood, true);
                        Box("Window open wooden shutter left", new Vector3(x - .35f, 1.6f, -.16f), new Vector3(.035f, .95f, .38f), S.DarkWood, true);
                        Box("Window open wooden shutter right", new Vector3(x + .35f, 1.6f, -.16f), new Vector3(.035f, .95f, .38f), S.DarkWood, true);
                    }
                }
                else
                {
                    Box("Vertical wall plank", new Vector3(x, 1.3f, 0), new Vector3(.185f, 2.6f, .035f), S.Wood);
                }
            }
            var wallGroup = Parent;
            Parent = previous;
            ModelCombiner.CombineModel(wallGroup, addCollider: true);
        }

        static void Roof(Vector3 p, float width, float depth, float height, Material mat, bool dormer = false)
        {
            var old = Parent;
            Parent = ModelGroup("Clay tile hip roof", p);
            Grid("Front roof plane", 28, 12, (u, v) => new Vector3((u - .5f) * Mathf.Lerp(width, width - depth * .8f, v), v * height, -depth / 2 + v * depth / 2), mat, new Vector2(8, 4), true);
            Grid("Back roof plane", 28, 12, (u, v) => new Vector3((u - .5f) * Mathf.Lerp(width, width - depth * .8f, v), v * height, depth / 2 - v * depth / 2), mat, new Vector2(8, 4), true);
            Grid("Left roof hip", 16, 12, (u, v) => new Vector3(-width / 2 + v * depth * .4f, v * height, (u - .5f) * depth * (1 - v)), mat, new Vector2(4, 4), true);
            Grid("Right roof hip", 16, 12, (u, v) => new Vector3(width / 2 - v * depth * .4f, v * height, (u - .5f) * depth * (1 - v)), mat, new Vector2(4, 4), true);

            var ridgeLength = width - depth * .8f;
            Box("Decorative terracotta ridge crest", new Vector3(0, height + .08f, 0), new Vector3(ridgeLength, .16f, .16f), mat);

            var roofCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(roofCombined, addCollider: true);
        }
    }
}
