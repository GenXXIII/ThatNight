using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    public static class StiltHouseBuilder
    {
        public static Transform Build()
        {
            if (S.Wood == null) S.Prepare();
            Parent = null;
            var root = Group("01 HOUSE - raised timber home", SiteLayout.HousePosition);
            root.localRotation = SiteLayout.HouseRotation;
            root.localScale = Vector3.one * SiteLayout.HouseScale;
            Parent = root;

            float halfWidth = SiteLayout.HouseWidth / 2, halfDepth = SiteLayout.HouseDepth / 2;

            // 1. Foundation footings and main stilt posts
            // All structural timber posts moved to the EXTERIOR EDGES of the house
            // ZERO interior posts cutting through the living hall or bedrooms!
            // Front doorway post (marked with X) and central under-floor post removed completely.
            var frameGroup = ModelGroup("Stilt house timber foundation and posts", Vector3.zero);
            var prevParent = Parent;
            Parent = frameGroup;

            float postSize = 0.24f;
            float postHalf = postSize * 0.5f; // 0.12m
            float pillarH = 5.50f;
            float pillarCenterY = 2.50f; // from y = -0.25m in soil to y = 5.25m at roof tie beams

            float leftX = -halfWidth + postHalf;   // -7.38m (flush inside left exterior wall)
            float rightX = halfWidth - postHalf;   // +7.38m (flush inside right exterior wall)
            float frontZ = -halfDepth + postHalf;  // -6.13m (flush inside front exterior wall)
            float backZ = halfDepth - postHalf;    // +6.13m (flush inside back exterior wall)
            const float partitionZ = 1f;           // bedroom partition wall line
            float midZ = partitionZ;               // +1.00m

            // Outer Perimeter Structural Timber Posts (soil y = -0.25m to roof y = 5.25m)
            Vector3[] postPositions = new Vector3[] {
                new Vector3(leftX, 0, frontZ),   // Front-Left corner
                new Vector3(rightX, 0, frontZ),  // Front-Right corner
                new Vector3(-0.85f, 0, frontZ),  // Doorway Left entrance pillar (from roof to soil)
                new Vector3(0.85f, 0, frontZ),   // Doorway Right entrance pillar (from roof to soil)
                new Vector3(leftX, 0, midZ),     // Left wall at partition line
                new Vector3(rightX, 0, midZ),    // Right wall at partition line
                new Vector3(leftX, 0, backZ),    // Back-Left corner (Bedroom 1)
                new Vector3(rightX, 0, backZ),   // Back-Right corner (Bedroom 2)
                new Vector3(0f, 0, backZ)        // Back Center (between bedrooms)
            };

            foreach (var pos in postPositions)
            {
                Box("Weathered structural post", pos + Vector3.up * pillarCenterY, new Vector3(postSize, pillarH, postSize), S.DarkWood, true);

                // Inward diagonal brace under the floor (stops at bottom of floor y = 2.42m)
                float braceDx = (pos.x < -1f) ? 0.85f : ((pos.x > 1f) ? -0.85f : 0f);
                float braceDz = (pos.z < 0f) ? 0.85f : -0.85f;
                Beam("Diagonal timber brace", pos + Vector3.up * 1.65f, pos + new Vector3(braceDx, 2.42f, braceDz), 0.11f, S.DarkWood);
            }

            // Under-floor heavy timber girders running along the outer post lines and center
            Box("Main floor girder Left", new Vector3(leftX, 2.30f, 0), new Vector3(.22f, .24f, SiteLayout.HouseDepth), S.DarkWood, false);
            Box("Main floor girder Right", new Vector3(rightX, 2.30f, 0), new Vector3(.22f, .24f, SiteLayout.HouseDepth), S.DarkWood, false);
            Box("Main floor girder Center", new Vector3(0, 2.30f, 0), new Vector3(.22f, .24f, SiteLayout.HouseDepth), S.DarkWood, false);

            Box("Main floor tie-beam Front", new Vector3(0, 2.30f, frontZ), new Vector3(SiteLayout.HouseWidth, .24f, .22f), S.DarkWood, false);
            Box("Main floor tie-beam Back", new Vector3(0, 2.30f, backZ), new Vector3(SiteLayout.HouseWidth, .24f, .22f), S.DarkWood, false);
            Box("Main floor tie-beam Mid", new Vector3(0, 2.30f, midZ), new Vector3(SiteLayout.HouseWidth, .24f, .22f), S.DarkWood, false);

            // Secondary floor joists spanning across the girders every 0.9m
            for (float jz = -halfDepth + .75f; jz <= halfDepth - .75f; jz += .90f)
            {
                Box("Under-floor joist", new Vector3(0, 2.39f, jz), new Vector3(SiteLayout.HouseWidth - .2f, .09f, .10f), S.Wood, false);
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
            // Outer veranda posts (vx = -1 and +1)
            // No gray concrete footings - timber posts extend directly into soil
            for (int vx = -1; vx <= 1; vx += 2)
            {
                var vp = new Vector3(vx * (verandaWidth / 2 - .2f), 0, -verandaDepth / 2 + .15f);
                Box("Veranda post", vp + Vector3.up * 2.45f, new Vector3(.20f, 5.3f, .20f), S.DarkWood, true);
            }

            // Two Full Veranda Entrance Pillars flanking the staircase opening (from roof structure down to soil)
            float stairGap = 1.7f;
            for (int sx = -1; sx <= 1; sx += 2)
            {
                float px = sx * (stairGap * 0.5f); // +/- 0.85m
                var vp = new Vector3(px, 0, -verandaDepth / 2 + .15f);
                Box("Veranda entrance pillar", vp + Vector3.up * 2.45f, new Vector3(.20f, 5.3f, .20f), S.DarkWood, true);
            }
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
            }

            // Side railings
            for (int side = -1; side <= 1; side += 2)
            {
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

            // 3. Wooden Entrance Staircase (Traditional Khmer turning staircase)
            BuildEntranceStaircase(root);

            PropsBuilder.SpiritHouse(new Vector3(-halfWidth + 2.8f, .02f, -halfDepth - 3.8f), 35f);

            // 4. Exterior Walls
            Wall(new Vector3(0, 2.5f, -halfDepth), 0, SiteLayout.HouseWidth, doorway: true, windows: true);
            Wall(new Vector3(0, 2.5f, halfDepth), 180, SiteLayout.HouseWidth, doorway: false, windows: true);
            Wall(new Vector3(-halfWidth, 2.5f, 0), 90, SiteLayout.HouseDepth, doorway: false, windows: true);
            Wall(new Vector3(halfWidth, 2.5f, 0), -90, SiteLayout.HouseDepth, doorway: false, windows: true);

            // Double wooden door panels (modeled closed flush with zero center gap and zero light leak)
            var doorsGroup = ModelGroup("Double wooden entrance doors", new Vector3(0, 2.5f, -halfDepth));
            prevParent = Parent;
            Parent = doorsGroup;

            // Solid doorway frame lintel, sill, jambs, and top transom canopy
            Box("Entrance doorway frame lintel", new Vector3(0, 2.30f, 0), new Vector3(1.28f, .08f, .16f), S.DarkWood, true);
            Box("Entrance doorway frame sill", new Vector3(0, 0.04f, 0), new Vector3(1.28f, .08f, .16f), S.DarkWood, true);
            Box("Entrance doorway frame jamb left", new Vector3(-.58f, 1.15f, 0), new Vector3(.08f, 2.26f, .14f), S.DarkWood, true);
            Box("Entrance doorway frame jamb right", new Vector3(.58f, 1.15f, 0), new Vector3(.08f, 2.26f, .14f), S.DarkWood, true);
            Box("Entrance doorway top transom canopy", new Vector3(0, 2.50f, 0), new Vector3(1.28f, .48f, .16f), S.DarkWood, true);

            var leftDoor = Group("Left door leaf", new Vector3(-.54f, 1.18f, 0));
            leftDoor.localEulerAngles = Vector3.zero;
            Parent = leftDoor;
            Box("Door panel left", new Vector3(.275f, 0, 0), new Vector3(.56f, 2.2f, .045f), S.DarkWood, true);
            Tube("Door brass pull ring left", new Vector3(.48f, 0, -.035f), .045f, .008f, S.Brass, Quaternion.identity, 16);
            Parent = doorsGroup;

            var rightDoor = Group("Right door leaf", new Vector3(.54f, 1.18f, 0));
            rightDoor.localEulerAngles = Vector3.zero;
            Parent = rightDoor;
            Box("Door panel right", new Vector3(-.275f, 0, 0), new Vector3(.56f, 2.2f, .045f), S.DarkWood, true);
            // Overlapping center astragal batten (Reap Tvear) sealing the center gap completely
            Box("Center overlapping astragal", new Vector3(-.55f, 0, -.025f), new Vector3(.08f, 2.22f, .045f), S.DarkWood, true);
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
            const float doorHalfWidth = .6f;
            float bedroomCenter = halfWidth / 2;

            // Interior bedroom partition wall
            var partitionGroup = ModelGroup("Bedroom partition wall and headers", Vector3.zero);
            prevParent = Parent;
            Parent = partitionGroup;
            for (int side = -1; side <= 1; side += 2)
            {
                float doorX = side * bedroomCenter;
                // Solid doorway transom header spanning from top of door jamb (4.70m) up to the wall plate (5.20m)
                Box("Bedroom doorway header", new Vector3(doorX, 4.95f, partitionZ), new Vector3(1.28f, .50f, .14f), S.Wood, true);
                for (int edge = -1; edge <= 1; edge += 2)
                    Box("Bedroom door jamb", new Vector3(doorX + edge * .60f, 3.62f, partitionZ), new Vector3(.08f, 2.24f, .16f), S.DarkWood, true);
                float outerWidth = halfWidth - bedroomCenter - doorHalfWidth;
                Box("Bedroom outer partition", new Vector3(side * (halfWidth - outerWidth / 2), 3.85f, partitionZ), new Vector3(outerWidth, 2.7f, .10f), S.Wood, true);

                // Dedicated Authentic Cambodian Wooden Bedroom Door
                BuildBedroomDoor(doorX, partitionZ, side);
            }
            Box("Bedroom central partition", new Vector3(0, 3.85f, partitionZ), new Vector3(bedroomCenter * 2 - doorHalfWidth * 2, 2.7f, .10f), S.Wood, true);
            Box("Bedroom dividing wall", new Vector3(0, 3.85f, partitionZ + (halfDepth - partitionZ) / 2), new Vector3(.10f, 2.7f, halfDepth - partitionZ), S.Wood, true);

            // --- FULL WALL ENCLOSURE: COVER TOP OF WALL TO FIT FLUSH WITH ROOF (NO GAPS TO OUTSIDE) ---
            // 0. Solid horizontal attic timber tie plate spanning across the entire house width at the top of the partition wall:
            Box("Attic wall top plate timber", new Vector3(0, 5.16f, partitionZ), new Vector3(SiteLayout.HouseWidth + 0.1f, 0.12f, 0.14f), S.DarkWood, true);

            // 1. Upper partition wall spandrel: seals the entire gap from y = 5.10m up to the roof rafters / tie beams
            int plankCount = Mathf.RoundToInt((SiteLayout.HouseWidth - 0.2f) / 0.18f);
            for (int p = 0; p < plankCount; p++)
            {
                float px = -halfWidth + 0.10f + (p + 0.5f) * (SiteLayout.HouseWidth - 0.2f) / plankCount;
                float roofHipY = 5.25f + 2.2f * Mathf.Clamp01(1f - Mathf.Max(Mathf.Abs(px) / (halfWidth + 0.6f), Mathf.Abs(partitionZ) / (halfDepth + 0.6f)));
                float spandrelH = Mathf.Max(roofHipY - 5.10f + 0.25f, 0.30f);
                Box("Upper partition spandrel plank", new Vector3(px, 5.10f + spandrelH * 0.5f, partitionZ), new Vector3(0.182f, spandrelH, 0.09f), S.Wood, true);
            }

            // 2. Upper bedroom dividing wall: seals between Bedroom 1 and 2 from y = 5.10m up to roof ridge
            int divPlanks = Mathf.RoundToInt((halfDepth - partitionZ) / 0.18f);
            for (int dp = 0; dp < divPlanks; dp++)
            {
                float dz = partitionZ + (dp + 0.5f) * (halfDepth - partitionZ) / divPlanks;
                float roofHipY = 5.25f + 2.2f * Mathf.Clamp01(1f - Mathf.Max(0f, Mathf.Abs(dz) / (halfDepth + 0.6f)));
                float divH = Mathf.Max(roofHipY - 5.10f + 0.25f, 0.30f);
                Box("Upper dividing wall spandrel plank", new Vector3(0, 5.10f + divH * 0.5f, dz), new Vector3(0.09f, divH, 0.182f), S.Wood, true);
            }

            // 3. Authentic tongue-and-groove timber ceilings over both bedrooms (sealing bedrooms from attic/sky)
            float bedCeilZ = partitionZ + (halfDepth - partitionZ) * 0.5f;
            float bedCeilDepth = halfDepth - partitionZ;
            Box("Bedroom 1 timber ceiling (Bridal)", new Vector3(-halfWidth * 0.5f, 5.12f, bedCeilZ), new Vector3(halfWidth - 0.05f, 0.035f, bedCeilDepth), S.Wood, true);
            Box("Bedroom 2 timber ceiling (Family)", new Vector3(halfWidth * 0.5f, 5.12f, bedCeilZ), new Vector3(halfWidth - 0.05f, 0.035f, bedCeilDepth), S.Wood, true);

            // 4. Exterior eave fascia enclosure boards: seals top of exterior walls flush into roof eaves
            Box("Eave fascia front", new Vector3(0, 5.18f, -halfDepth - 0.015f), new Vector3(SiteLayout.HouseWidth + 0.1f, 0.18f, 0.05f), S.DarkWood);
            Box("Eave fascia back", new Vector3(0, 5.18f, halfDepth + 0.015f), new Vector3(SiteLayout.HouseWidth + 0.1f, 0.18f, 0.05f), S.DarkWood);
            Box("Eave fascia left", new Vector3(-halfWidth - 0.015f, 5.18f, 0), new Vector3(0.05f, 0.18f, SiteLayout.HouseDepth + 0.1f), S.DarkWood);
            Box("Eave fascia right", new Vector3(halfWidth + 0.015f, 5.18f, 0), new Vector3(0.05f, 0.18f, SiteLayout.HouseDepth + 0.1f), S.DarkWood);

            Parent = prevParent;
            ModelCombiner.CombineModel(partitionGroup, addCollider: true);

            // Living Room Ancestral Altar Shelf on rear partition wall
            PropsBuilder.AltarShelf(new Vector3(0.011f, 4.150f, 0.843f), 88.3f);

            // Traditional Cambodian Wall Clock (wooden case with pendulum)
            PropsBuilder.CambodianWallClock(new Vector3(1.361f, 4.250f, 0.940f), 0f);

            // Traditional Cambodian Buddhist & Lunar Tear-off Wall Calendar
            PropsBuilder.CambodianLunarCalendar(new Vector3(-1.337f, 4.200f, 0.940f), 0f);

            // Traditional Cambodian Kroma Scarf Peg Rack on side timber wall
            PropsBuilder.KromaPegRack(new Vector3(-7.440f, 3.850f, -2.400f), 90f);

            // WEDDING DECORATION A: Authentic Cambodian Wedding Fruit Procession (Chhoeng Phle Chheu) at LEFT SIDE OF THE HOUSE per user custom
            BuildWeddingFruitProcession(new Vector3(-4.867f, 2.506f, -2.352f), 356.5f);

            // WEDDING DECORATION B: Authentic Cambodian Wrist-Tying Ceremony Altar at RIGHT SIDE OF THE HOUSE per user custom
            BuildAuthenticCambodianWristTying(new Vector3(5.968f, 2.492f, -2.528f));

            // Flanking Floor-Standing Ceremonial Floral Pedestal Urns (Tho Phka Rongkar) flanking wrist-tying altar
            PropsBuilder.FloorStandingFlowerUrn(new Vector3(7.010f, 2.455f, -5.183f));
            PropsBuilder.FloorStandingFlowerUrn(new Vector3(6.996f, 2.455f, 0.094f));

            // WEDDING DECORATION C: Central Guest Floor Gathering Area in Middle Living Room (fills empty space)
            BuildCentralWeddingReceptionArea(new Vector3(0.0f, 2.500f, -2.500f));

            // WEDDING DECORATION D: Wedding Gift Reception Display Table along partition wall under altar
            BuildWeddingGiftReceptionTable(new Vector3(0.011f, 2.455f, 0.720f));

            // Authentic Cambodian Living Room Furniture & Wedding Additions:
            // 1. Carved Teak Glass Showcase Display Cupboard (Tu Kanchok)
            PropsBuilder.CambodianDisplayCupboard(new Vector3(-6.499f, 2.455f, 0.656f), 2.8f);

            // 2. Traditional Handcrafted Teak Salon Sitting Set
            PropsBuilder.CambodianSalonSet(new Vector3(2.124f, 2.512f, -5.769f), 179.0f);

            // 3. Bundles of Rolled Reed Sleeping Mats (Kanthiel Romchek) in corner for overnight wedding guests
            PropsBuilder.RolledSleepingMats(new Vector3(-7.227f, 2.462f, -5.979f), 25.0f);

            // 4. Traditional Wooden Coat & Hat Stand
            PropsBuilder.CambodianHatAndCoatStand(new Vector3(-7.122f, 2.512f, -5.464f));

            // 5. Hanging Traditional Cambodian Wedding Paper Lanterns (Khom Rongkar) from rafters
            PropsBuilder.HangingWeddingLantern(new Vector3(-5.1f, 4.4f, -1.6f));
            PropsBuilder.HangingWeddingLantern(new Vector3(5.1f, 4.4f, -1.6f));
            PropsBuilder.HangingWeddingLantern(new Vector3(0f, 4.5f, -2.5f));

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

            // Authentic rural lived-in details: heavy mortar/pestle, drying chili/garlic strings, firewood pile, vintage bicycle
            BuildUnderHouseLivedInDetails(new Vector3(-3.2f, .02f, 1.5f));

            // Lighting
            LightingBuilder.Bulb(new Vector3(-.25f, 4.8f, -halfDepth - .2f), 6.8f, 3.5f, true);
            LightingBuilder.Bulb(new Vector3(0, 4.5f, -.15f), 6.5f, 2.8f, false);

            Parent = null;
            return root;
        }

        static void BuildWeddingFruitProcession(Vector3 p, float yaw = 0f)
        {
            var fruitGroup = ModelGroup("Wedding fruit procession offering display - Chhoeng Phle Chheu", p);
            if (yaw != 0f) fruitGroup.transform.localEulerAngles = new Vector3(0, yaw, 0);
            var prevParent = Parent;
            Parent = fruitGroup;

            // Long woven reed floor runner with red silk border (scaled to fit all 36 trays comfortably)
            Box("Procession feast floor mat", Vector3.up * .01f, new Vector3(2.9f, .02f, 5.5f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f));
            Grid("Ceremonial crimson carpet runner", 20, 32, (u, v) => new Vector3((u - .5f) * 2.7f, .025f, (v - .5f) * 5.3f), S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f), new Vector2(3, 6), true);

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var crimsonSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);

            // 18 Auspicious Cambodian Wedding Offering Types (18 Pairs = 36 Trays Total)
            Material mangoMat = S.Plain("Ripe mango orange", new Color(.92f, .55f, .10f), .35f);
            Material dragonMat = S.Plain("Dragon fruit red", new Color(.85f, .15f, .35f), .35f);
            Material pomeloMat = S.Plain("Fresh green pomelo", new Color(.38f, .62f, .16f), .25f);
            Material bananaMat = S.Plain("Auspicious banana yellow", new Color(.88f, .78f, .18f), .25f);
            Material coconutMat = S.Plain("Young green coconut", new Color(.45f, .60f, .25f), .25f);
            Material orangeMat = S.Plain("Sweet orange citrus", new Color(.90f, .48f, .08f), .30f);
            Material appleMat = S.Plain("Auspicious red apple", new Color(.80f, .12f, .15f), .30f);
            Material mangosteenMat = S.Plain("Purple mangosteen", new Color(.32f, .10f, .25f), .25f);
            Material betelMat = S.Plain("Ceremonial betel and areca", new Color(.28f, .52f, .16f), .25f);

            Material pineappleMat = S.Plain("Golden pineapple", new Color(.85f, .68f, .12f), .25f);
            Material rambutanMat = S.Plain("Crimson rambutan", new Color(.88f, .18f, .20f), .30f);
            Material longanMat = S.Plain("Golden longan", new Color(.78f, .65f, .38f), .20f);
            Material papayaMat = S.Plain("Ripe orange papaya", new Color(.95f, .50f, .12f), .30f);
            Material guavaMat = S.Plain("Fresh guava green", new Color(.48f, .72f, .22f), .25f);
            Material starfruitMat = S.Plain("Golden starfruit", new Color(.90f, .82f, .15f), .30f);
            Material grapeMat = S.Plain("Dark red grape", new Color(.40f, .10f, .22f), .35f);
            Material watermelonMat = S.Plain("Green striped watermelon", new Color(.22f, .48f, .18f), .25f);
            Material sweetsMat = S.Plain("Royal golden sweet", new Color(.96f, .78f, .18f), .35f);

            // Lane 1 (Columns 0 & 1): 9 pairs for rows 0 to 8
            Material[] lane1Mats = { mangoMat, dragonMat, pomeloMat, bananaMat, coconutMat, orangeMat, appleMat, mangosteenMat, betelMat };
            string[] lane1Names = { "Golden Mango", "Dragon Fruit", "Green Pomelo", "Auspicious Banana", "Young Coconut", "Sweet Orange", "Red Apple", "Purple Mangosteen", "Sacred Betel and Areca" };

            // Lane 2 (Columns 2 & 3): 9 pairs for rows 0 to 8
            Material[] lane2Mats = { pineappleMat, rambutanMat, longanMat, papayaMat, guavaMat, starfruitMat, grapeMat, watermelonMat, sweetsMat };
            string[] lane2Names = { "Golden Pineapple", "Crimson Rambutan", "Sweet Longan", "Ripe Papaya", "Fresh Guava", "Golden Starfruit", "Red Grape", "Green Watermelon", "Royal Wedding Sweets" };

            float[] colXs = { -1.02f, -.34f, .34f, 1.02f };
            float[] rowZs = { 2.16f, 1.62f, 1.08f, .54f, 0f, -.54f, -1.08f, -1.62f, -2.16f };

            for (int r = 0; r < 9; r++)
            {
                float rz = rowZs[r];

                // Lane 1: Pair in Col 0 and Col 1
                Material fMat1 = lane1Mats[r];
                string name1 = lane1Names[r];
                for (int c = 0; c < 2; c++)
                {
                    BuildSingleOfferingTray(colXs[c], rz, fMat1, name1, r, c, goldSilk, crimsonSilk);
                }

                // Lane 2: Pair in Col 2 and Col 3
                Material fMat2 = lane2Mats[r];
                string name2 = lane2Names[r];
                for (int c = 2; c < 4; c++)
                {
                    BuildSingleOfferingTray(colXs[c], rz, fMat2, name2, r, c, goldSilk, crimsonSilk);
                }
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(fruitGroup, addCollider: true);
        }

        static void BuildSingleOfferingTray(float x, float z, Material fMat, string fruitName, int row, int col, Material goldSilk, Material crimsonSilk)
        {
            var tp = new Vector3(x, .03f, z);

            // Ornate Cambodian Pedestal Bowl (Phan)
            Lathe("Fruit pedestal tray - " + fruitName, tp, new[]{
                new Vector2(.075f, 0), new Vector2(.035f, .055f), new Vector2(.18f, .12f), new Vector2(.22f, .15f)
            }, (row + col) % 2 == 0 ? S.Brass : S.Metal, 18);

            // Traditional Arched Golden Procession Carrying Handle
            Tube("Pedestal gold arch handle", tp + Vector3.up * .30f, .20f, .013f, S.Brass, Quaternion.Euler(0, 0, 90f), 18);

            // Auspicious ceremonial ribbon sash and rosette bow
            Tube("Pedestal red silk sash", tp + Vector3.up * .14f, .22f, .020f, crimsonSilk, Quaternion.identity, 16);
            Ball("Sash gold rosette", tp + new Vector3(0, .14f, -.21f), Vector3.one * .042f, goldSilk);

            // Fruit contents based on name
            if (fruitName.Contains("Banana"))
            {
                for (int b = -2; b <= 2; b++)
                    Ball("Banana finger", tp + new Vector3(b * .06f, .19f, 0), new Vector3(.045f, .13f, .065f), fMat);
            }
            else if (fruitName.Contains("Coconut"))
            {
                Ball("Young coconut 1", tp + new Vector3(-.065f, .22f, -.035f), new Vector3(.14f, .17f, .14f), fMat);
                Ball("Young coconut 2", tp + new Vector3(.065f, .22f, .035f), new Vector3(.14f, .17f, .14f), fMat);
            }
            else if (fruitName.Contains("Pineapple"))
            {
                Ball("Pineapple body 1", tp + new Vector3(-.05f, .22f, 0), new Vector3(.12f, .18f, .12f), fMat);
                Ball("Pineapple body 2", tp + new Vector3(.05f, .22f, 0), new Vector3(.12f, .18f, .12f), fMat);
                Cylinder("Crown leaves 1", tp + new Vector3(-.05f, .33f, 0), .07f, .08f, S.Leaf, 12);
                Cylinder("Crown leaves 2", tp + new Vector3(.05f, .33f, 0), .07f, .08f, S.Leaf, 12);
            }
            else if (fruitName.Contains("Watermelon"))
            {
                Ball("Watermelon 1", tp + new Vector3(-.06f, .22f, 0), new Vector3(.15f, .15f, .20f), fMat);
                Ball("Watermelon 2", tp + new Vector3(.06f, .22f, 0), new Vector3(.15f, .15f, .20f), fMat);
            }
            else if (fruitName.Contains("Betel"))
            {
                for (int s = -2; s <= 2; s++)
                    Ball("Betel leaf cone", tp + new Vector3(s * .05f, .18f, 0), new Vector3(.04f, .10f, .08f), fMat);
                Ball("Areca nut", tp + new Vector3(0, .24f, 0), Vector3.one * .07f, S.Plain("Areca orange", new Color(.78f, .42f, .15f), .25f));
            }
            else if (fruitName.Contains("Sweets"))
            {
                for (int sw = 0; sw < 6; sw++)
                {
                    float sa = sw * Mathf.PI * 2 / 6f;
                    Ball("Sweet piece", tp + new Vector3(Mathf.Sin(sa) * .08f, .18f, Mathf.Cos(sa) * .08f), Vector3.one * .065f, fMat);
                }
                Ball("Apex sweet", tp + Vector3.up * .24f, Vector3.one * .06f, fMat);
            }
            else if (fruitName.Contains("Grape"))
            {
                for (int g = 0; g < 9; g++)
                {
                    float ga = g * Mathf.PI * 2 / 9f;
                    Ball("Grape cluster", tp + new Vector3(Mathf.Sin(ga) * .08f, .18f + (g % 3) * .03f, Mathf.Cos(ga) * .08f), Vector3.one * .055f, fMat);
                }
            }
            else // Mangoes, Dragon fruits, Pomelos, Oranges, Apples, Mangosteens, Rambutans, Longans, Papayas, Guavas, Starfruits
            {
                for (int f = 0; f < 5; f++)
                {
                    float fa = f * Mathf.PI * 2 / 5f;
                    Ball("Offering fruit " + fruitName, tp + new Vector3(Mathf.Sin(fa) * .09f, .18f, Mathf.Cos(fa) * .09f), Vector3.one * .10f, fMat);
                }
                Ball("Apex fruit " + fruitName, tp + Vector3.up * .26f, Vector3.one * .095f, fMat);
            }
        }

        /// <summary>
        /// Authentic Cambodian Wrist-Tying Ceremony Altar (Sen Chang Dai / Sompeas Ptem).
        /// Rebuilt to strictly adhere to authentic Khmer wedding traditions:
        /// 1. Couple kneels on red/gold silk mats facing EAST (to the right, +X).
        /// 2. Forearms rest forward across the signature long horizontal bolster (Khnaoy Sompeas Ptem).
        /// 3. Red blessing strings (Khsae Chang Dai) with pinned traditional money banknotes (Cham Noung Dai).
        /// 4. Sacred ancestral bronze sword (Preah Khan) rested in front of the groom.
        /// 5. Traditional Slator (betel/areca cone towers) and 5-tiered Bay Sey.
        /// 6. Popil candleholders, holy water bowl, ancestral chicken feast platter, and floor seating for elders.
        /// </summary>
        /// <summary>
        /// Authentic Cambodian Wrist-Tying Ceremony Altar (Sen Chang Dai / Sompeas Ptem).
        /// Completely redesigned for professional visual fidelity and authentic Cambodian cultural aesthetics:
        /// 1. Grand woven rush mat foundation with rich crimson velvet and gold-rope bordered carpet.
        /// 2. Twin regal plush kneeling cushions for bride and groom with tufted centers and gold corner tassels.
        /// 3. Traditional Khmer wrist-resting bolster (Khnaoy Sompeas Ptem) on a carved wooden cradle with silk rosettes.
        /// <summary>
        /// Authentic Cambodian Wrist-Tying Ceremony Altar (Sen Chang Dai / Sompeas Ptem)
        /// Faithfully modeled from authentic Cambodian wedding ceremony reference:
        /// - Grand rectangular golden ceremonial bolster pillow (Khnoy Ptem) draped in shimmering golden embroidered Khmer silk.
        /// - Sacred ancestral matrimonial heirloom sword (Preah Khan / Leuk Daab) resting horizontally across the bolster.
        /// - Upright slender yellow beeswax ceremonial candles standing in front.
        /// - Silver chalices with consecrated red yarn threads (Khsae Chang Dai), trimming scissors, and fresh white jasmine garlands (Phka Malis).
        /// - Twin royal gold brocade kneeling cushions for bride and groom, with elder sitting cushions on the sides.
        /// </summary>
        static void BuildAuthenticCambodianWristTying(Vector3 p)
        {
            var ceremonyGroup = ModelGroup("Sacred wrist-tying ceremony altar - Sen Chang Dai", p);
            ceremonyGroup.transform.localEulerAngles = new Vector3(0, 90f, 0);
            var prevParent = Parent;
            Parent = ceremonyGroup;

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);
            var sequinGold = S.Plain("Gold brocade sequin", new Color(.92f, .78f, .22f), .55f, .4f);
            var yellowCandleMat = S.Plain("Yellow beeswax taper candle", new Color(.94f, .78f, .12f), .25f);

            // 1. Base: Large woven Cambodian rush mat and ceremonial crimson & gold silk carpet
            Box("Ceremony woven floor mat", Vector3.up * .01f, new Vector3(3.6f, .015f, 3.2f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f));
            Grid("Ceremonial red silk carpet", 20, 18, (u, v) => new Vector3((u - .5f) * 3.2f, .025f, (v - .5f) * 2.8f), redSilk, new Vector2(3, 3), true);
            // Flat rectangular gold border trim along carpet perimeter
            Box("Carpet gold border N", new Vector3(0, .03f, 1.38f), new Vector3(3.22f, .01f, .06f), goldSilk);
            Box("Carpet gold border S", new Vector3(0, .03f, -1.38f), new Vector3(3.22f, .01f, .06f), goldSilk);
            Box("Carpet gold border W", new Vector3(-1.58f, .03f, 0), new Vector3(.06f, .01f, 2.76f), goldSilk);
            Box("Carpet gold border E", new Vector3(1.58f, .03f, 0), new Vector3(.06f, .01f, 2.76f), goldSilk);

            // 2. Twin Royal Kneeling Cushions for Bride (left, -0.44m) and Groom (right, +0.44m)
            for (int side = -1; side <= 1; side += 2)
            {
                var cushionPos = new Vector3(side * .44f, .03f, -.52f);
                // Plush cushion body
                Box("Royal kneeling cushion", cushionPos + Vector3.up * .07f, new Vector3(.66f, .14f, .66f), goldSilk, true);
                Box("Cushion crimson velvet border", cushionPos + Vector3.up * .07f, new Vector3(.70f, .035f, .70f), redSilk);
                // Central tufted button
                Ball("Cushion tufted center button", cushionPos + Vector3.up * .145f, Vector3.one * .045f, redSilk);
                // 4 Gold tassels at cushion corners
                for (int cx = -1; cx <= 1; cx += 2)
                    for (int cz = -1; cz <= 1; cz += 2)
                        Ball("Corner gold tassel", cushionPos + new Vector3(cx * .32f, .04f, cz * .32f), Vector3.one * .035f, goldSilk);
            }

            // 3. Central Authentic Rectangular Golden Bolster Pillow (Khnoy Ptem / Khnaoy Sompeas Ptem)
            var bolsterPos = new Vector3(0, .03f, .06f);
            float bolsterW = 1.18f;
            float bolsterH = 0.22f;
            float bolsterD = 0.36f;
            float bolsterCenterY = bolsterH * 0.5f; // 0.11m

            // Main golden silk bolster pillow body
            Box("Khnoy Ptem golden bolster body", bolsterPos + Vector3.up * bolsterCenterY, new Vector3(bolsterW, bolsterH, bolsterD), goldSilk, true);
            // Shimmering gold brocade & sequin ornamental front/back decorative bands
            Box("Bolster gold sequin band front", bolsterPos + new Vector3(0, bolsterCenterY, -bolsterD * 0.5f - 0.005f), new Vector3(bolsterW, bolsterH * 0.55f, 0.015f), sequinGold);
            Box("Bolster gold sequin band back", bolsterPos + new Vector3(0, bolsterCenterY, bolsterD * 0.5f + 0.005f), new Vector3(bolsterW, bolsterH * 0.55f, 0.015f), sequinGold);
            // Auspicious crimson piping along top and bottom edges
            Box("Bolster crimson edge trim top", bolsterPos + Vector3.up * (bolsterH - 0.01f), new Vector3(bolsterW + 0.02f, 0.025f, bolsterD + 0.02f), redSilk);
            Box("Bolster crimson edge trim bot", bolsterPos + Vector3.up * 0.015f, new Vector3(bolsterW + 0.02f, 0.025f, bolsterD + 0.02f), redSilk);

            // Pleated silk end caps with gold tassels on left and right sides
            for (int end = -1; end <= 1; end += 2)
            {
                var endP = bolsterPos + new Vector3(end * (bolsterW * 0.5f + 0.015f), bolsterCenterY, 0);
                Box("Bolster silk end cap", endP, new Vector3(0.025f, bolsterH * 0.88f, bolsterD * 0.88f), redSilk);
                Ball("Bolster end gold rosette", endP + new Vector3(end * 0.02f, 0, 0), Vector3.one * 0.055f, goldSilk);
            }

            // 4. Sacred Matrimonial Sword (Preah Khan / Leuk Daab)
            // Laid horizontally directly across the center top of the golden bolster pillow
            var swordY = bolsterH + 0.018f; // rests right on top
            // Double-edged silver steel blade
            Beam("Preah Khan sword blade", bolsterPos + new Vector3(-.36f, swordY, 0), bolsterPos + new Vector3(.30f, swordY, 0), .026f, S.Metal);
            // Carved golden hilt pommel and dragon grip
            Box("Sword golden hilt grip", bolsterPos + new Vector3(-.42f, swordY, 0), new Vector3(.10f, .032f, .032f), S.Brass);
            Ball("Sword lotus pommel finial", bolsterPos + new Vector3(-.48f, swordY, 0), Vector3.one * .045f, S.Brass);
            // Ornate golden crossguard
            Box("Sword ornate crossguard", bolsterPos + new Vector3(-.36f, swordY, 0), new Vector3(.022f, .12f, .042f), S.Brass);
            // Auspicious ceremonial red ribbon tied around hilt
            Box("Sword tied red ribbon", bolsterPos + new Vector3(-.38f, swordY, 0), new Vector3(.03f, .04f, .05f), redSilk);

            // 5. Upright Slender Yellow Ceremonial Candles standing directly in front of the bolster
            for (int c = 0; c < 5; c++)
            {
                float cx = -.36f + c * .18f;
                var candlePos = bolsterPos + new Vector3(cx, 0, -.26f);
                // Small engraved brass holder dish
                Lathe("Candle brass dish", candlePos, new[]{ new Vector2(0, 0), new Vector2(.032f, 0), new Vector2(.038f, .016f) }, S.Brass, 12);
                // Slender yellow beeswax candle
                Cylinder("Yellow ceremonial candle", candlePos + Vector3.up * .016f, .009f, .22f, yellowCandleMat, 10);
                // Warm flickering flame tip
                Ball("Candle flame tip", candlePos + Vector3.up * .245f, new Vector3(.012f, .024f, .012f), S.Bulb);
            }

            // 6. Consecrated Red Wrist Yarn Strings (Khsae Chang Dai) & Trimming Scissors (Kandray Meas)
            // Left pedestal bowl: Red strings bundle
            var stringBowlPos = bolsterPos + new Vector3(-.75f, 0, -.08f);
            Lathe("Silver red thread chalice", stringBowlPos, new[]{
                new Vector2(.07f, 0), new Vector2(.035f, .07f), new Vector2(.16f, .16f), new Vector2(.14f, .19f)
            }, S.Metal, 18);
            Ball("Consecrated red yarn bundle", stringBowlPos + Vector3.up * .17f, new Vector3(.22f, .12f, .22f), redSilk);

            // Golden trimming scissors resting beside chalice
            var scissP = stringBowlPos + new Vector3(.14f, .015f, 0);
            Beam("Scissor blade 1", scissP, scissP + new Vector3(.07f, .004f, .03f), .007f, S.Brass);
            Beam("Scissor blade 2", scissP, scissP + new Vector3(.07f, .004f, -.03f), .007f, S.Brass);

            // 7. Fresh White Jasmine Flower Garland Bowl (Phka Malis)
            // Right pedestal bowl: fragrant jasmine blossoms and garlands
            var jasmineBowlPos = bolsterPos + new Vector3(.75f, 0, -.08f);
            Lathe("Silver jasmine bowl chalice", jasmineBowlPos, new[]{
                new Vector2(.07f, 0), new Vector2(.035f, .07f), new Vector2(.16f, .16f), new Vector2(.14f, .19f)
            }, S.Metal, 18);
            Ball("Fragrant jasmine blossoms", jasmineBowlPos + Vector3.up * .17f, new Vector3(.22f, .10f, .22f), S.Cream);
            // Jasmine garland rings draped over the rim
            for (int j = 0; j < 3; j++)
            {
                float ja = j * Mathf.PI * 2f / 3f;
                Ball("Jasmine garland cluster", jasmineBowlPos + new Vector3(Mathf.Sin(ja) * .12f, .16f, Mathf.Cos(ja) * .12f), Vector3.one * .055f, S.Cream);
            }

            // 8. Silver Holy Water Bowl (Bat Teuk Mon) with Fragrant Blossoms
            var waterBowlPos = bolsterPos + new Vector3(-.75f, 0, .26f);
            Lathe("Silver holy water bowl", waterBowlPos, new[]{
                new Vector2(.06f, 0), new Vector2(.13f, .09f), new Vector2(.12f, .10f)
            }, S.Metal, 16);
            Cylinder("Holy water surface", waterBowlPos + Vector3.up * .075f, .11f, .01f, S.Water, 16);
            Ball("Floating jasmine flower", waterBowlPos + Vector3.up * .085f, Vector3.one * .035f, S.Cream);

            // 9. Ceremonial Betel Nut Presentation Chalice (Hop Sla)
            var betelBowlPos = bolsterPos + new Vector3(.75f, 0, .26f);
            Lathe("Brass betel chalice", betelBowlPos, new[]{ new Vector2(0, 0), new Vector2(.11f, 0), new Vector2(.13f, .035f) }, S.Brass, 16);
            Ball("Green betel leaf cluster", betelBowlPos + Vector3.up * .055f, new Vector3(.16f, .06f, .16f), S.Leaf);
            Ball("Areca palm nut", betelBowlPos + Vector3.up * .09f, Vector3.one * .045f, S.Plain("Areca orange", new Color(.88f, .55f, .18f), .25f));

            // 10. Elder Sitting Cushions flanking Left and Right sides for parents/relatives
            for (int side = -1; side <= 1; side += 2)
            {
                var elderPos = new Vector3(side * 1.18f, .03f, -.36f);
                Box("Elder sitting cushion", elderPos + Vector3.up * .045f, new Vector3(.58f, .09f, .58f), redSilk, true);
                Box("Elder cushion gold border", elderPos + Vector3.up * .045f, new Vector3(.60f, .025f, .60f), goldSilk);
                Ball("Elder cushion center button", elderPos + Vector3.up * .095f, Vector3.one * .04f, goldSilk);
            }
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

            Parent = prevParent;
            ModelCombiner.CombineModel(ceremonyGroup, addCollider: true);
        }

        static void BuildCentralWeddingReceptionArea(Vector3 p)
        {
            var centerGroup = ModelGroup("Central Cambodian wedding guest reception floor gathering area", p);
            var prevParent = Parent;
            Parent = centerGroup;

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);
            var thermosCrimson = S.Plain("Vintage 2006 Thermos Tin", new Color(.78f, .16f, .18f), .55f, .30f);
            var greenTeaLeaf = S.Plain("Fresh betel leaves green", new Color(.26f, .50f, .14f), .25f);

            // 1. Traditional Cambodian Handwoven Reed Mat (Kanthiel Romchek) with burgundy cotton border
            Box("Central reception woven floor mat", Vector3.up * .01f, new Vector3(3.2f, .018f, 2.6f), S.Plain("Living room reed mat", new Color(.52f, .40f, .22f), .05f));
            // Ceremonial Crimson Silk Carpet Runner in center of the woven mat
            Grid("Reception crimson silk carpet", 18, 14, (u, v) => new Vector3((u - .5f) * 2.85f, .023f, (v - .5f) * 2.25f), redSilk, new Vector2(3, 2), true);
            // Flat gold brocade embroidered border around carpet
            Box("Carpet gold border N", new Vector3(0, .028f, 1.11f), new Vector3(2.87f, .008f, .05f), goldSilk);
            Box("Carpet gold border S", new Vector3(0, .028f, -1.11f), new Vector3(2.87f, .008f, .05f), goldSilk);
            Box("Carpet gold border W", new Vector3(-1.41f, .028f, 0), new Vector3(.05f, .008f, 2.22f), goldSilk);
            Box("Carpet gold border E", new Vector3(1.41f, .028f, 0), new Vector3(.05f, .008f, 2.22f), goldSilk);

            // 2. AUTHENTIC 2006 LOW CARVED TEAK TEA TABLE (Tok Teab)
            var tp = new Vector3(0, .03f, .05f);
            float tableW = 1.12f;
            float tableD = 0.62f;
            float tableH = 0.32f;

            // Polished teak tabletop with beveled edge molding
            Box("Low tea table top polished teak", tp + Vector3.up * (tableH - .015f), new Vector3(tableW, .030f, tableD), S.Wood, true);
            Box("Table top edge bevel frame", tp + Vector3.up * (tableH - .015f), new Vector3(tableW + .024f, .015f, tableD + .024f), S.DarkWood);
            // Hand-carved decorative lotus apron underneath top
            Box("Low table carved apron N", tp + new Vector3(0, tableH - .055f, -tableD * .5f + .04f), new Vector3(tableW - .14f, .055f, .022f), S.Carving);
            Box("Low table carved apron S", tp + new Vector3(0, tableH - .055f, tableD * .5f - .04f), new Vector3(tableW - .14f, .055f, .022f), S.Carving);
            Box("Low table carved apron W", tp + new Vector3(-tableW * .5f + .04f, tableH - .055f, 0), new Vector3(.022f, .055f, tableD - .14f), S.Carving);
            Box("Low table carved apron E", tp + new Vector3(tableW * .5f - .04f, tableH - .055f, 0), new Vector3(.022f, .055f, tableD - .14f), S.Carving);

            // 4 Traditional turned baluster legs with lathe rings and carved scroll feet
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    var legP = tp + new Vector3(x * (tableW * .5f - .07f), 0, z * (tableD * .5f - .07f));
                    Cylinder("Table turned baluster leg", legP, .028f, tableH - .035f, S.DarkWood, 12);
                    Ball("Leg turned upper ring", legP + Vector3.up * (tableH - .10f), new Vector3(.062f, .025f, .062f), S.Carving);
                    Ball("Leg carved foot bun", legP + Vector3.up * .022f, new Vector3(.058f, .035f, .058f), S.DarkWood);
                }

            // Lower perimeter wooden stretcher rails
            Box("Table stretcher N", tp + new Vector3(0, .065f, -tableD * .5f + .07f), new Vector3(tableW - .14f, .022f, .022f), S.DarkWood);
            Box("Table stretcher S", tp + new Vector3(0, .065f, tableD * .5f - .07f), new Vector3(tableW - .14f, .022f, .022f), S.DarkWood);
            Box("Table stretcher W", tp + new Vector3(-tableW * .5f + .07f, .065f, 0), new Vector3(.022f, .022f, tableD - .14f), S.DarkWood);
            Box("Table stretcher E", tp + new Vector3(tableW * .5f - .07f, .065f, 0), new Vector3(.022f, .022f, tableD - .14f), S.DarkWood);

            // Embroidered raw silk runner along center of low table
            Box("Table central silk runner", tp + new Vector3(0, tableH + .003f, 0), new Vector3(tableW + .01f, .004f, .28f), redSilk);
            Box("Runner gold border N", tp + new Vector3(0, tableH + .006f, .135f), new Vector3(tableW + .01f, .003f, .015f), goldSilk);
            Box("Runner gold border S", tp + new Vector3(0, tableH + .006f, -.135f), new Vector3(tableW + .01f, .003f, .015f), goldSilk);

            // 3. AUTHENTIC 2006 TABLEWARE & CEREMONY ITEMS ON TABLE:
            // A. Iconic 2006 Floral Tin Vacuum Thermos Flask (Phle Te / Khmer Hot Water Thermos)
            var thermosP = tp + new Vector3(-.36f, tableH + .005f, -.08f);
            Cylinder("Thermos chrome base", thermosP, .068f, .035f, S.Metal, 16);
            Cylinder("Thermos floral tin body", thermosP + Vector3.up * .035f, .065f, .26f, thermosCrimson, 16);
            // Peony flower decorative emblem band
            Box("Thermos floral peony emblem", thermosP + new Vector3(0, .16f, -.066f), new Vector3(.065f, .065f, .005f), goldSilk);
            Cylinder("Thermos chrome shoulder", thermosP + Vector3.up * .295f, .066f, .035f, S.Metal, 16);
            Cylinder("Thermos pour neck", thermosP + Vector3.up * .33f, .032f, .040f, S.Metal, 14);
            Cylinder("Thermos cork stopper", thermosP + Vector3.up * .365f, .025f, .025f, S.Wood, 12);
            // Vintage curved wire carry handle
            Tube("Thermos wire carry handle", thermosP + new Vector3(.068f, .22f, 0), .085f, .006f, S.Metal, Quaternion.Euler(0, 90f, 0), 16);

            // B. Traditional Ceramic Chinese Tea Set on Serving Tray
            var teaTrayP = tp + new Vector3(.04f, tableH + .005f, 0);
            Lathe("Tea serving lacquer tray", teaTrayP, new[]{ new Vector2(0, 0), new Vector2(.17f, 0), new Vector2(.185f, .022f) }, S.Plain("Dark red lacquer", new Color(.45f, .08f, .10f), .6f), 18);
            // Porcelain teapot (Bum Te)
            var potP = teaTrayP + new Vector3(-.04f, .015f, 0);
            Lathe("Porcelain teapot body", potP, new[]{
                new Vector2(.035f, 0), new Vector2(.072f, .045f), new Vector2(.055f, .11f), new Vector2(.042f, .13f)
            }, S.Porcelain, 16);
            Ball("Teapot lid knob", potP + Vector3.up * .145f, Vector3.one * .022f, S.Brass);
            Beam("Teapot spout", potP + new Vector3(.05f, .05f, 0), potP + new Vector3(.105f, .10f, 0), .018f, S.Porcelain);
            Tube("Teapot arched handle", potP + new Vector3(-.07f, .065f, 0), .045f, .009f, S.Porcelain, Quaternion.Euler(0, 90f, 0), 14);

            // 4 Handleless Chinese porcelain tea cups (Phaing Te) arranged on tray
            for (int tc = 0; tc < 4; tc++)
            {
                float tca = tc * Mathf.PI * 2f / 4f + 0.4f;
                var cupP = teaTrayP + new Vector3(Mathf.Sin(tca) * .115f, .015f, Mathf.Cos(tca) * .115f);
                Lathe("Porcelain tea cup", cupP, new[]{ new Vector2(.016f, 0), new Vector2(.028f, .035f), new Vector2(.030f, .042f) }, S.Porcelain, 12);
                Ball("Cup jasmine tea liquid", cupP + Vector3.up * .030f, new Vector3(.050f, .005f, .050f), S.Plain("Jasmine tea amber", new Color(.75f, .55f, .18f), .4f));
            }

            // C. Authentic Cambodian Matrimonial Betel & Areca Chalice (Hop Sla)
            var betelP = tp + new Vector3(.38f, tableH + .005f, -.04f);
            Lathe("Engraved silver betel footed chalice", betelP, new[]{
                new Vector2(.045f, 0), new Vector2(.022f, .035f), new Vector2(.068f, .075f), new Vector2(.072f, .09f)
            }, S.Metal, 16);
            // Fresh folded betel leaves (Sla) arranged standing in chalice
            for (int bl = -2; bl <= 2; bl++)
                Ball("Folded betel leaf cone", betelP + new Vector3(bl * .022f, .11f, (bl % 2) * .015f), new Vector3(.024f, .075f, .045f), greenTeaLeaf);
            // Sliced areca nuts (Phle Sla)
            Ball("Areca nut slice 1", betelP + new Vector3(-.025f, .125f, -.025f), Vector3.one * .028f, S.Plain("Areca nut orange", new Color(.78f, .42f, .15f), .25f));
            Ball("Areca nut slice 2", betelP + new Vector3(.025f, .125f, -.025f), Vector3.one * .028f, S.Plain("Areca nut orange", new Color(.78f, .42f, .15f), .25f));
            // Miniature brass lime paste jar (Kol Kanhchoeu)
            Lathe("Brass lime jar", betelP + new Vector3(0, .09f, .035f), new[]{ new Vector2(.018f, 0), new Vector2(.024f, .032f), new Vector2(.012f, .048f) }, S.Brass, 12);

            // D. Auspicious Cambodian Wedding Sweet Treats Platter (Chan Num Kar)
            var treatP = tp + new Vector3(-.18f, tableH + .005f, .14f);
            Lathe("Porcelain treats dish", treatP, new[]{ new Vector2(0, 0), new Vector2(.095f, 0), new Vector2(.11f, .020f) }, S.Porcelain, 16);
            // Traditional fried sesame cakes / golden Num Kom treats
            for (int tr = 0; tr < 5; tr++)
            {
                float tra = tr * Mathf.PI * 2f / 5f;
                Ball("Golden wedding sweet pastry", treatP + new Vector3(Mathf.Sin(tra) * .05f, .025f, Mathf.Cos(tra) * .05f), new Vector3(.035f, .022f, .035f), S.Plain("Golden pastry", new Color(.88f, .68f, .22f), .25f));
            }
            Ball("Apex sweet treat", treatP + Vector3.up * .042f, Vector3.one * .030f, S.Plain("Golden pastry", new Color(.88f, .68f, .22f), .25f));

            // E. Authentic 2006 Nokia Hand-Phone resting on the living room tea table (Scene 03 / 04 Phone Call)
            PropsBuilder.MobilePhone2006(tp + new Vector3(-.28f, tableH + .005f, -.22f), 26f);

            // 4. AUTHENTIC 2006 KHMER FLOOR SEATING:
            // A. Twin Traditional Triangular Wedge Bolster Pillows (Khnaoy Pha-aek) at the head of the seating area
            for (int tri = -1; tri <= 1; tri += 2)
            {
                var triP = tp + new Vector3(tri * .95f, 0, .10f);
                // 3-sided triangular wedge body
                float prismL = .55f;
                float prismH = .24f;
                float prismW = .28f;
                // Triangular backrest modeled via stacked angled cushioned sections
                Box("Khnaoy Pha-aek base wedge", triP + Vector3.up * (prismH * .28f), new Vector3(prismW, prismH * .55f, prismL), goldSilk, true);
                Box("Khnaoy Pha-aek mid wedge", triP + Vector3.up * (prismH * .62f), new Vector3(prismW * .70f, prismH * .38f, prismL), goldSilk, true);
                Box("Khnaoy Pha-aek ridge apex", triP + Vector3.up * (prismH * .90f), new Vector3(prismW * .38f, prismH * .22f, prismL), goldSilk, true);
                // Crimson silk decorative end caps
                for (int ep = -1; ep <= 1; ep += 2)
                {
                    Box("Wedge crimson end cap", triP + new Vector3(0, prismH * .48f, ep * (prismL * .5f + .008f)), new Vector3(prismW * .92f, prismH * .90f, .016f), redSilk);
                    Ball("End cap gold rosette", triP + new Vector3(0, prismH * .48f, ep * (prismL * .5f + .020f)), Vector3.one * .042f, goldSilk);
                }
            }

            // B. 4 Authentic Low Quilted Cambodian Silk Sitting Cushions (Kanthiel Angkoey)
            Vector3[] seatOffsets = new Vector3[] {
                new Vector3(-.52f, 0, -.68f),
                new Vector3(.52f, 0, -.68f),
                new Vector3(-.52f, 0, .72f),
                new Vector3(.52f, 0, .72f)
            };
            for (int s = 0; s < seatOffsets.Length; s++)
            {
                var sp = tp + seatOffsets[s];
                float padW = .54f;
                float padD = .54f;
                float padH = .065f;
                // Quilted cushion body
                Box("Quilted sitting cushion body", sp + Vector3.up * (padH * .5f), new Vector3(padW, padH, padD), (s % 2 == 0) ? goldSilk : redSilk, true);
                // Contrast piping border
                Box("Cushion contrast border trim", sp + Vector3.up * (padH * .5f), new Vector3(padW + .022f, .024f, padD + .022f), (s % 2 == 0) ? redSilk : goldSilk);
                // Tufted button center
                Ball("Cushion quilted center button", sp + Vector3.up * padH, Vector3.one * .035f, (s % 2 == 0) ? redSilk : goldSilk);
                // 4 Gold corner tassels
                for (int cx = -1; cx <= 1; cx += 2)
                    for (int cz = -1; cz <= 1; cz += 2)
                        Ball("Cushion corner gold tassel", sp + new Vector3(cx * (padW * .5f - .02f), .022f, cz * (padD * .5f - .02f)), Vector3.one * .028f, goldSilk);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(centerGroup, addCollider: true);
        }

        static void BuildWeddingGiftReceptionTable(Vector3 p)
        {
            var giftGroup = ModelGroup("Wedding gift reception display table - Tok Cham Noung Dai", p);
            var prevParent = Parent;
            Parent = giftGroup;

            var goldSilk = S.Plain("Khmer bridal gold silk", new Color(.82f, .65f, .18f), .45f, .25f);
            var redSilk = S.Plain("Ceremonial crimson silk", new Color(.65f, .12f, .16f), .35f, .15f);
            var ivoryLace = S.Plain("Ivory wedding lace cloth", new Color(.94f, .92f, .86f), .15f);
            var goldFoil = S.Plain("Wedding gold foil trim", new Color(.92f, .78f, .22f), .60f, .35f);

            float tableW = 2.15f;
            float tableD = 0.68f;
            float tableTopY = 0.72f;

            // 1. REAL 2006 TABLE STRUCTURE: 4 turned legs and wooden frame (open underneath, showing floor!)
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    var legP = new Vector3(x * (tableW * 0.5f - 0.08f), 0, z * (tableD * 0.5f - 0.08f));
                    Cylinder("Table leg", legP, .032f, tableTopY - .03f, S.DarkWood, 14);
                    Ball("Table leg turned bead", legP + Vector3.up * .25f, new Vector3(.072f, .04f, .072f), S.Carving);
                }

            // Lower perimeter leg stretchers
            Box("Stretcher front", new Vector3(0, .14f, -tableD * 0.5f + 0.08f), new Vector3(tableW - .16f, .035f, .035f), S.DarkWood);
            Box("Stretcher back", new Vector3(0, .14f, tableD * 0.5f - 0.08f), new Vector3(tableW - .16f, .035f, .035f), S.DarkWood);
            Box("Stretcher left", new Vector3(-tableW * 0.5f + 0.08f, .14f, 0), new Vector3(.035f, .035f, tableD - .16f), S.DarkWood);
            Box("Stretcher right", new Vector3(tableW * 0.5f - 0.08f, .14f, 0), new Vector3(.035f, .035f, tableD - .16f), S.DarkWood);

            // Tabletop wooden frame
            Box("Table wooden top", Vector3.up * (tableTopY - 0.015f), new Vector3(tableW, .03f, tableD), S.Wood, true);

            // 2. AUTHENTIC 2006 DRAPED FABRIC TABLECLOTH (Ivory lace tablecloth with natural 22cm hanging drop skirt)
            float dropH = 0.24f;
            // Top cloth surface
            Box("Ivory lace tablecloth top", Vector3.up * tableTopY, new Vector3(tableW + .04f, .01f, tableD + .04f), ivoryLace);
            // Front hanging drop skirt (soft hanging fabric, stops 48cm above floor!)
            Box("Tablecloth front drop", new Vector3(0, tableTopY - dropH * 0.5f, -tableD * 0.5f - .02f), new Vector3(tableW + .04f, dropH, .012f), ivoryLace);
            // Left and Right hanging drop skirts
            Box("Tablecloth left drop", new Vector3(-tableW * 0.5f - .02f, tableTopY - dropH * 0.5f, 0), new Vector3(.012f, dropH, tableD + .04f), ivoryLace);
            Box("Tablecloth right drop", new Vector3(tableW * 0.5f + .02f, tableTopY - dropH * 0.5f, 0), new Vector3(.012f, dropH, tableD + .04f), ivoryLace);
            // Scalloped gold lace hem border along the bottom of the drop
            Box("Lace gold hem front", new Vector3(0, tableTopY - dropH, -tableD * 0.5f - .021f), new Vector3(tableW + .04f, .025f, .014f), goldFoil);
            Box("Lace gold hem left", new Vector3(-tableW * 0.5f - .021f, tableTopY - dropH, 0), new Vector3(.014f, .025f, tableD + .04f), goldFoil);
            Box("Lace gold hem right", new Vector3(tableW * 0.5f + .021f, tableTopY - dropH, 0), new Vector3(.014f, .025f, tableD + .04f), goldFoil);

            // Ceremonial Crimson Satin Central Runner
            Box("Ceremonial crimson satin runner", new Vector3(0, tableTopY + .006f, 0), new Vector3(tableW + .042f, .005f, .34f), redSilk);
            Box("Runner gold trim N", new Vector3(0, tableTopY + .009f, .17f), new Vector3(tableW + .042f, .004f, .02f), goldSilk);
            Box("Runner gold trim S", new Vector3(0, tableTopY + .009f, -.17f), new Vector3(tableW + .042f, .004f, .02f), goldSilk);

            // 3. ICONIC 2006 KHMER WEDDING DONATION BOX (Bop Cham Noung Dai / Bop Sombot Kar)
            // Beautiful crimson and gold decorated wedding gift chest in center
            var chestP = new Vector3(0, tableTopY + .01f, -.04f);
            Box("Donation gift chest base", chestP + Vector3.up * .12f, new Vector3(.32f, .24f, .24f), redSilk, true);
            Box("Chest gold corner molding L", chestP + new Vector3(-.155f, .12f, 0), new Vector3(.015f, .24f, .245f), goldFoil);
            Box("Chest gold corner molding R", chestP + new Vector3(.155f, .12f, 0), new Vector3(.015f, .24f, .245f), goldFoil);
            Box("Chest lid bevel", chestP + Vector3.up * .25f, new Vector3(.33f, .025f, .25f), goldSilk);
            // Cash gift envelope slot on top
            Box("Chest envelope slot", chestP + Vector3.up * .265f, new Vector3(.12f, .008f, .022f), S.Black);
            // Traditional golden wedding heart emblem
            Ball("Chest gold heart emblem", chestP + new Vector3(0, .14f, -.125f), Vector3.one * .055f, goldFoil);
            Ball("Lid miniature wedding rosette", chestP + Vector3.up * .28f, Vector3.one * .04f, redSilk);

            // 4. 2006 WEDDING GUEST REGISTRY LEDGER BOOK & PEN (Siphou Kat Chhmuoh)
            // Hardcover ledger book open on the right side of the table where family records guest gifts
            var bookP = new Vector3(.44f, tableTopY + .01f, -.02f);
            Box("Ledger book cover", bookP + Vector3.up * .006f, new Vector3(.28f, .012f, .34f), S.Plain("Ledger blue cover", new Color(.16f, .28f, .55f), .25f));
            Box("Ledger open white pages", bookP + Vector3.up * .015f, new Vector3(.26f, .008f, .32f), S.Plain("Ledger ruled paper", new Color(.96f, .95f, .91f), .1f));
            // Classic 2006 blue ballpoint pen resting on the ledger
            var penP = bookP + new Vector3(.06f, .022f, .04f);
            Beam("Scribe ballpoint pen body", penP + new Vector3(-.06f, 0, 0), penP + new Vector3(.06f, 0, 0), .007f, S.Plain("Blue pen plastic", new Color(.12f, .32f, .82f), .6f));
            Ball("Pen white click cap", penP + new Vector3(.065f, 0, 0), Vector3.one * .007f, S.Cream);

            // Red wedding gift envelopes (Sombot Cham Noung Dai) beside the ledger
            Box("Red wedding cash envelope 1", bookP + new Vector3(-.16f, .012f, -.08f), new Vector3(.12f, .006f, .18f), redSilk);
            Box("Envelope gold calligraphy 1", bookP + new Vector3(-.16f, .016f, -.08f), new Vector3(.04f, .002f, .06f), goldFoil);
            Box("Red wedding cash envelope 2", bookP + new Vector3(-.14f, .018f, .08f), new Vector3(.18f, .006f, .11f), redSilk);

            // 5. WEDDING SOUVENIR FAVORS TRAY (Kanchraeng Amnaoy)
            // Left side: Silver presentation tray with small wrapped guest keepsake pouches
            var favorP = new Vector3(-.44f, tableTopY + .01f, -.02f);
            Lathe("Silver favor tray", favorP, new[]{ new Vector2(0, 0), new Vector2(.16f, 0), new Vector2(.18f, .025f) }, S.Metal, 18);
            for (int f = 0; f < 6; f++)
            {
                float fa = f * Mathf.PI * 2f / 6f;
                var fp = favorP + new Vector3(Mathf.Sin(fa) * .09f, .028f, Mathf.Cos(fa) * .09f);
                Ball("Wedding favor silk pouch", fp, new Vector3(.05f, .035f, .05f), (f % 2 == 0) ? S.Pink : goldSilk);
                Ball("Pouch gold ribbon tie", fp + Vector3.up * .022f, Vector3.one * .016f, goldSilk);
            }

            // 6. TWIN PORCELAIN LOTUS VASES on far outer table ends
            for (int v = -1; v <= 1; v += 2)
            {
                var vaseP = new Vector3(v * .88f, tableTopY + .01f, 0);
                Lathe("Porcelain wedding flower vase", vaseP, new[]{
                    new Vector2(.04f, 0), new Vector2(.072f, .06f), new Vector2(.034f, .18f), new Vector2(.058f, .24f)
                }, S.Porcelain, 16);
                Ball("Pink lotus flower bud", vaseP + Vector3.up * .28f, new Vector3(.085f, .13f, .085f), S.Pink);
                Cylinder("Lotus flower stem", vaseP + Vector3.up * .16f, .007f, .22f, S.Leaf, 8);
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(giftGroup, addCollider: true);
        }

        static void BuildBedroomDoor(float doorX, float partitionZ, int side)
        {
            float hingeX = doorX + side * 0.59f;
            var doorLeaf = Group((side < 0 ? "Bedroom 1 (Bridal)" : "Bedroom 2 (Family)") + " wooden door leaf", new Vector3(hingeX, 2.50f, partitionZ));
            doorLeaf.localEulerAngles = Vector3.zero; // Modeled closed flush (0 degrees)
            var prevParent = Parent;
            Parent = doorLeaf;

            float doorW = 1.18f;
            float doorH = 2.24f;
            float doorThick = 0.045f;
            float leafCenterX = -side * (doorW * 0.5f);

            // 0. Continuous solid timber door core board (guarantees ZERO see-through gaps or slots anywhere)
            Box("Door solid core panel", new Vector3(leafCenterX, doorH * 0.5f, 0), new Vector3(doorW, doorH, 0.038f), S.DarkWood, true);

            // 1. Solid dark teak outer door frame (stiles, top rail, bottom kick rail, lock rail)
            Box("Door frame stile hinge", new Vector3(-side * 0.045f, doorH * 0.5f, 0), new Vector3(0.09f, doorH, doorThick), S.DarkWood, true);
            Box("Door frame stile strike", new Vector3(-side * (doorW - 0.045f), doorH * 0.5f, 0), new Vector3(0.09f, doorH, doorThick), S.DarkWood, true);
            Box("Door frame top rail", new Vector3(leafCenterX, doorH - 0.06f, 0), new Vector3(doorW - 0.18f, 0.12f, doorThick), S.DarkWood, true);
            Box("Door frame bottom kick rail", new Vector3(leafCenterX, 0.09f, 0), new Vector3(doorW - 0.18f, 0.18f, doorThick), S.DarkWood, true);
            Box("Door frame mid lock rail", new Vector3(leafCenterX, 1.05f, 0), new Vector3(doorW - 0.18f, 0.12f, doorThick), S.DarkWood, true);
            Box("Door frame center vertical muntin", new Vector3(leafCenterX, doorH * 0.5f, 0), new Vector3(0.07f, doorH - 0.30f, doorThick), S.DarkWood, true);

            // 2. 4 Recessed traditional carved wooden panels (flush to rails and stiles, backed by solid core)
            float panelW = (doorW - 0.25f) * 0.5f;
            // Upper panels (1.11m to 2.10m = 0.99m height)
            float upperPanelH = 0.99f;
            float upperPanelY = 1.605f;
            for (int p = 0; p < 2; p++)
            {
                float px = -side * (0.09f + panelW * 0.5f + p * (panelW + 0.07f));
                Box("Door upper carved panel", new Vector3(px, upperPanelY, 0), new Vector3(panelW + 0.01f, upperPanelH, 0.024f), S.Carving, true);
                Box("Door upper panel molding", new Vector3(px, upperPanelY, 0), new Vector3(panelW + 0.02f, upperPanelH + 0.02f, 0.030f), S.Wood);
            }
            // Lower panels (0.18m to 0.99m = 0.81m height)
            float lowerPanelH = 0.81f;
            float lowerPanelY = 0.585f;
            for (int p = 0; p < 2; p++)
            {
                float px = -side * (0.09f + panelW * 0.5f + p * (panelW + 0.07f));
                Box("Door lower carved panel", new Vector3(px, lowerPanelY, 0), new Vector3(panelW + 0.01f, lowerPanelH, 0.024f), S.Carving, true);
                Box("Door lower panel molding", new Vector3(px, lowerPanelY, 0), new Vector3(panelW + 0.02f, lowerPanelH + 0.02f, 0.030f), S.Wood);
            }

            // 3. Traditional Cambodian Brass Hardware
            float handleX = -side * (doorW - 0.12f);
            // Living room side brass backplate & ring pull
            Box("Brass handle escutcheon plate living room", new Vector3(handleX, 1.05f, -doorThick * 0.5f - 0.004f), new Vector3(0.045f, 0.16f, 0.008f), S.Brass);
            Ball("Brass handle mount boss", new Vector3(handleX, 1.08f, -doorThick * 0.5f - 0.012f), Vector3.one * 0.024f, S.Brass);
            Tube("Brass ring door pull", new Vector3(handleX, 1.04f, -doorThick * 0.5f - 0.018f), 0.036f, 0.007f, S.Brass, Quaternion.identity, 16);

            // Bedroom interior slide bolt lock (Khlav Tvear)
            Box("Wooden slide latch casing", new Vector3(handleX, 1.05f, doorThick * 0.5f + 0.012f), new Vector3(0.14f, 0.045f, 0.022f), S.DarkWood);
            Beam("Brass slide bolt pin", new Vector3(handleX - 0.05f, 1.05f, doorThick * 0.5f + 0.012f), new Vector3(handleX + 0.06f, 1.05f, doorThick * 0.5f + 0.012f), 0.014f, S.Brass);

            // 4. Heavy forged iron/brass strap hinges connecting to the door frame
            for (int h = 0; h < 2; h++)
            {
                float hy = (h == 0) ? 0.38f : 1.82f;
                Box("Hinge strap door", new Vector3(-side * 0.08f, hy, -doorThick * 0.5f - 0.003f), new Vector3(0.14f, 0.035f, 0.006f), S.Metal);
                Cylinder("Hinge pin knuckle", new Vector3(0, hy - 0.025f, -doorThick * 0.5f - 0.003f), 0.010f, 0.05f, S.Metal, 8);
            }

            Parent = prevParent;
        }

        static void BuildInteriorWeddingDrapes(float partitionZ, float halfWidth)
        {
            var drapesGroup = ModelGroup("Living room festive wedding silk swags", Vector3.zero);
            var prevParent = Parent;
            Parent = drapesGroup;

            for (int side = -1; side <= 1; side += 2)
            {
                float doorX = side * (halfWidth / 2f);
                float xInner = side * 0.85f;
                float xOuter = side * (halfWidth - 0.35f);

                // Continuous graceful silk swag across this half of the house
                Grid("Living room festive silk swag", 36, 6, (u, v) =>
                {
                    // u from 0 (inner, near center altar) to 1 (outer, near outer timber wall)
                    float x = Mathf.Lerp(xInner, xOuter, u);
                    // Draped curve with lowest sag near the bedroom door (u ~ 0.5)
                    float sag = Mathf.Sin(u * Mathf.PI) * 0.35f;
                    float y = Mathf.Lerp(4.78f, 4.85f, u) - sag - v * 0.18f;
                    return new Vector3(x, y, partitionZ - 0.10f); // Cleanly in front of wall planks!
                }, S.Pink, new Vector2(4, 1), true);

                // Large Decorative Silk Rosette pinned right above the bedroom doorway
                var rosetteP = new Vector3(doorX, 4.65f, partitionZ - 0.14f);
                WeddingYardBuilder.Rosette(rosetteP, 0.22f);
                Ball("Rosette gold center", rosetteP + new Vector3(0, 0, -0.02f), Vector3.one * 0.07f, S.Plain("Silk Gold", new Color(.88f, .72f, .20f), .5f));

                // Hanging silk ribbon tails flanking the doorway
                Beam("Ribbon tail left", rosetteP + new Vector3(-0.06f, 0, -0.01f), rosetteP + new Vector3(-0.12f, -0.38f, -0.01f), 0.025f, S.Pink);
                Beam("Ribbon tail right", rosetteP + new Vector3(0.06f, 0, -0.01f), rosetteP + new Vector3(0.12f, -0.38f, -0.01f), 0.025f, S.Pink);
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
                if (doorway && Mathf.Abs(x) < .55f)
                {
                    // Upper wall header plank above entrance door closing gap to tie beam
                    Box("Upper doorway header plank", new Vector3(x, 2.38f, 0), new Vector3(.185f, .45f, .035f), S.Wood);
                    continue;
                }
                bool isWin = windows && (Mathf.Abs(x - width * .28f) < .45f || Mathf.Abs(x + width * .28f) < .45f);
                if (isWin)
                {
                    Box("Lower wall plank", new Vector3(x, .55f, 0), new Vector3(.185f, 1.1f, .035f), S.Wood);
                    Box("Upper wall header plank", new Vector3(x, 2.35f, 0), new Vector3(.185f, .5f, .035f), S.Wood);
                }
                else
                {
                    Box("Vertical wall plank", new Vector3(x, 1.3f, 0), new Vector3(.185f, 2.6f, .035f), S.Wood);
                }
            }
            if (windows)
            {
                foreach (float winSide in new float[] { -1f, 1f })
                {
                    float winCenter = winSide * width * 0.28f;
                    // 1. Solid window frame: sill, header, and vertical jambs
                    Box("Window sill", new Vector3(winCenter, 1.08f, 0), new Vector3(0.92f, 0.06f, 0.14f), S.DarkWood, true);
                    Box("Window header", new Vector3(winCenter, 2.12f, 0), new Vector3(0.92f, 0.06f, 0.14f), S.DarkWood, true);
                    Box("Window jamb left", new Vector3(winCenter - 0.43f, 1.60f, 0), new Vector3(0.06f, 0.98f, 0.12f), S.DarkWood, true);
                    Box("Window jamb right", new Vector3(winCenter + 0.43f, 1.60f, 0), new Vector3(0.06f, 0.98f, 0.12f), S.DarkWood, true);

                    // 2. Authentic Cambodian Turned Wooden Security Bars (Reang Bong-ouy) - Exactly 5 bars
                    foreach (float barOffset in new float[] { -0.28f, -0.14f, 0.00f, 0.14f, 0.28f })
                    {
                        Cylinder("Window security bar", new Vector3(winCenter + barOffset, 1.11f, 0), 0.016f, 0.98f, S.DarkWood, 12);
                    }

                    // 3. Wooden Shutters
                    Box("Window open wooden shutter left", new Vector3(winCenter - 0.41f, 1.60f, -0.19f), new Vector3(0.035f, 0.95f, 0.38f), S.DarkWood, true);
                    Box("Window open wooden shutter right", new Vector3(winCenter + 0.41f, 1.60f, -0.19f), new Vector3(0.035f, 0.95f, 0.38f), S.DarkWood, true);
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

        static void BuildUnderHouseLivedInDetails(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Authentic Cambodian under-house rustic props", p);

            var chiliRed = S.Plain("Dried red chili pod", new Color(.78f, .14f, .16f), .15f);
            var garlicWhite = S.Plain("Garlic bulb", new Color(.92f, .90f, .84f), .10f);
            var darkWood = S.DarkWood;

            // 1. Traditional Cambodian Heavy Wooden Mortar & Pestle (Tbal Kbal / Tbal Dambay)
            var mortarPos = new Vector3(1.2f, 0, -.8f);
            Lathe("Heavy wooden mortar bowl", mortarPos, new[]{
                new Vector2(.14f, 0), new Vector2(.18f, .04f), new Vector2(.16f, .22f), new Vector2(.20f, .42f), new Vector2(.18f, .45f), new Vector2(.11f, .20f)
            }, darkWood, 16);
            // Thick wooden pestle resting inside diagonally
            Beam("Carved wooden pestle", mortarPos + new Vector3(-.05f, .22f, 0), mortarPos + new Vector3(.18f, .62f, .08f), .045f, S.Wood);

            // 2. Hanging Dried Chili Strings and Garlic Braids drying from overhead under-floor joists
            for (int s = 0; s < 4; s++)
            {
                float sx = -1.2f + s * .65f;
                var hangTop = new Vector3(sx, 2.38f, 0);
                Beam("Drying string jute cord", hangTop, hangTop + Vector3.down * .75f, .004f, S.Cream);
                if (s % 2 == 0)
                {
                    // String of dried red bird's eye chilies
                    for (int c = 0; c < 8; c++)
                    {
                        float a = c * 1.8f;
                        Ball("Dried chili pepper pod", hangTop + new Vector3(Mathf.Cos(a) * .035f, -.15f - c * .068f, Mathf.Sin(a) * .035f), new Vector3(.018f, .045f, .018f), chiliRed);
                    }
                }
                else
                {
                    // Braid of air-drying purple garlic and shallots
                    for (int g = 0; g < 6; g++)
                    {
                        float a = g * 2.1f;
                        Ball("Air-drying garlic bulb", hangTop + new Vector3(Mathf.Cos(a) * .045f, -.18f - g * .085f, Mathf.Sin(a) * .045f), Vector3.one * .042f, garlicWhite);
                    }
                }
            }

            // 3. Stack of split dry firewood logs (Os) for cooking hearth
            var woodpilePos = new Vector3(-1.8f, 0, 1.1f);
            for (int layer = 0; layer < 3; layer++)
                for (int log = 0; log < 4; log++)
                {
                    var lp = woodpilePos + new Vector3((layer % 2 == 0) ? log * .14f : 0, .05f + layer * .09f, (layer % 2 == 0) ? 0 : log * .14f);
                    Cylinder("Split firewood log", lp, .045f, .75f, S.Bark, 8);
                }

            // 4. Vintage Rural Roadster Bicycle (Kohn) leaned against stilt pillar
            var bikePos = new Vector3(2.6f, 0, .6f);
            var bikeGroup = Group("Vintage roadster bicycle", bikePos);
            bikeGroup.localEulerAngles = new Vector3(0, 35f, 6f);
            var prevBikeParent = Parent;
            Parent = bikeGroup;

            // Wheels (front and rear 26-inch wheels)
            Tube("Rear bike wheel rim", new Vector3(0, .34f, -.55f), .32f, .014f, S.Metal, Quaternion.Euler(0, 90, 0), 20);
            Tube("Rear bike rubber tire", new Vector3(0, .34f, -.55f), .34f, .018f, S.Black, Quaternion.Euler(0, 90, 0), 20);
            Tube("Front bike wheel rim", new Vector3(0, .34f, .55f), .32f, .014f, S.Metal, Quaternion.Euler(0, 90, 0), 20);
            Tube("Front bike rubber tire", new Vector3(0, .34f, .55f), .34f, .018f, S.Black, Quaternion.Euler(0, 90, 0), 20);

            // Diamond steel frame tubes
            Beam("Bottom bracket shell", new Vector3(0, .26f, 0), new Vector3(0, .26f, .05f), .035f, S.DarkWood);
            Beam("Seat tube", new Vector3(0, .26f, 0), new Vector3(0, .75f, -.12f), .024f, S.DarkWood);
            Beam("Down tube", new Vector3(0, .26f, 0), new Vector3(0, .72f, .42f), .024f, S.DarkWood);
            Beam("Top tube", new Vector3(0, .74f, -.10f), new Vector3(0, .72f, .42f), .022f, S.DarkWood);
            Beam("Chainstay", new Vector3(0, .26f, 0), new Vector3(0, .34f, -.55f), .018f, S.DarkWood);
            Beam("Seatstay", new Vector3(0, .74f, -.10f), new Vector3(0, .34f, -.55f), .018f, S.DarkWood);
            Beam("Front fork", new Vector3(0, .72f, .42f), new Vector3(0, .34f, .55f), .022f, S.DarkWood);

            // Chrome curved handlebars and leather saddle
            Beam("Handlebar stem", new Vector3(0, .72f, .42f), new Vector3(0, .86f, .40f), .02f, S.Metal);
            Beam("Handlebar chrome bar", new Vector3(-.24f, .86f, .34f), new Vector3(.24f, .86f, .34f), .018f, S.Metal);
            Box("Leather bicycle seat", new Vector3(0, .78f, -.14f), new Vector3(.14f, .04f, .22f), S.Black);
            Box("Rear steel parcel rack", new Vector3(0, .65f, -.45f), new Vector3(.15f, .02f, .38f), S.Metal);

            Parent = prevBikeParent;

            var underCombined = Parent;
            Parent = old;
            ModelCombiner.CombineModel(underCombined, addCollider: true);
        }

        public static void BuildEntranceStaircase(Transform houseRoot)
        {
            var oldStairs = houseRoot.Find("Wooden entrance staircase");
            if (oldStairs != null) UnityEngine.Object.DestroyImmediate(oldStairs.gameObject);

            var prevParent = Parent;
            Parent = houseRoot;
            var stairGroup = ModelGroup("Wooden entrance staircase", Vector3.zero);
            Parent = stairGroup;

            float verandaZ = -12.5f / 2 - 1.85f; // -8.10m
            float stepRise = 1.20f / 7f; // 0.1714m
            float stepRun = 0.28f;
            float stairWidth = 1.50f;

            // --- FLIGHT 1 (Upper flight): 7 steps descending along local -Z from veranda to landing ---
            // (Protruding threshold block removed per user request)
            for (int i = 0; i < 7; i++)
            {
                float y = 2.40f - (i + 1) * stepRise + stepRise * 0.5f;
                float z = verandaZ - (i + 0.5f) * stepRun;
                Box("Upper stair tread", new Vector3(0f, y, z), new Vector3(stairWidth, 0.16f, stepRun), S.Wood, true);
            }

            // Upper flight stringers, handrails, and balusters
            for (int side = -1; side <= 1; side += 2)
            {
                float x = side * (stairWidth * 0.5f + 0.05f); // +/- 0.80m
                Vector3 pTop = new Vector3(x, 2.40f + 0.85f, verandaZ + 0.1f);
                Vector3 pBot = new Vector3(x, 1.20f + 0.85f, verandaZ - 7 * stepRun); // (x, 2.05m, -10.06m)
                Beam("Upper stair stringer", new Vector3(side * 0.70f, 2.40f + 0.02f, verandaZ + 0.1f), new Vector3(side * 0.70f, 1.20f + 0.02f, verandaZ - 7 * stepRun), 0.13f, S.DarkWood);
                Beam("Upper stair handrail", pTop, pBot, 0.08f, S.Wood);
                for (int b = 0; b < 6; b++)
                {
                    float f = (b + 0.5f) / 6f;
                    Vector3 bp = Vector3.Lerp(pTop, pBot, f) - Vector3.up * 0.40f;
                    Box("Upper stair baluster", bp, new Vector3(0.045f, 0.70f, 0.045f), S.Wood);
                }
            }

            // --- INTERMEDIATE LANDING (Thnal Chhnuor): 1.60m x 1.60m deck at local Y = 1.20m ---
            float landWidth = 1.60f;
            float landDepth = 1.60f;
            float landY = 1.20f;
            float northZ = verandaZ - 7 * stepRun; // -10.06m
            float southZ = northZ - landDepth;     // -11.66m
            float landZ = (northZ + southZ) * 0.5f; // -10.86m
            float eastX = landWidth * 0.5f;        // +0.80m
            float westX = -landWidth * 0.5f;       // -0.80m

            // 4 Continuous Timber Pillars extending from soil (y = -0.25m) up to newel posts (y = 2.15m)
            // Each post has width 0.12m x 0.12m and total height 2.40m
            float postSize = 0.12f;
            float postHalf = postSize * 0.5f;
            float pillarH = 2.40f;
            float pillarCenterY = -0.25f + pillarH * 0.5f; // 0.95m

            // 1. Southeast Pillar: anchors South rim beam, East rim beam, South handrail, and East handrail!
            Box("Landing SE pillar", new Vector3(eastX - postHalf, pillarCenterY, southZ + postHalf), new Vector3(postSize, pillarH, postSize), S.DarkWood, true);

            // 2. Southwest Pillar: anchors South rim beam, South handrail, Flight 2 South handrail & stringer!
            Box("Landing SW pillar", new Vector3(westX + postHalf, pillarCenterY, southZ + postHalf), new Vector3(postSize, pillarH, postSize), S.DarkWood, true);

            // 3. Northwest Pillar: anchors North rim beam, Flight 1 West handrail/stringer, Flight 2 North handrail/stringer!
            Box("Landing NW pillar", new Vector3(westX + postHalf, pillarCenterY, northZ - postHalf), new Vector3(postSize, pillarH, postSize), S.DarkWood, true);

            // 4. Northeast Pillar: anchors North rim beam, East rim beam, Flight 1 East handrail/stringer, Landing East handrail!
            Box("Landing NE pillar", new Vector3(eastX - postHalf, pillarCenterY, northZ - postHalf), new Vector3(postSize, pillarH, postSize), S.DarkWood, true);

            // Landing Floor Deck Planks (flush with outer post edges, matching stair wood color)
            Box("Landing deck", new Vector3(0f, landY - 0.02f, landZ), new Vector3(landWidth, 0.04f, landDepth), S.Wood, true);

            // Perimeter Heavy Timber Rim Beams neatly fitted between posts (NEVER protruding past the pillars!)
            float rimH = 0.14f;
            float rimW = 0.10f;
            float rimY = landY - 0.04f - rimH * 0.5f; // 1.09m
            float innerSpanX = landWidth - postSize * 2f; // 1.36m (fits cleanly between posts)
            float innerSpanZ = landDepth - postSize * 2f; // 1.36m (fits cleanly between posts)

            // South rim beam: fits cleanly between SW and SE pillars
            Box("Landing rim beam south", new Vector3(0f, rimY, southZ + postHalf), new Vector3(innerSpanX, rimH, rimW), S.DarkWood, true);
            // East rim beam: fits cleanly between SE and NE pillars
            Box("Landing rim beam east", new Vector3(eastX - postHalf, rimY, landZ), new Vector3(rimW, rimH, innerSpanZ), S.DarkWood, true);
            // North rim beam: fits cleanly between NW and NE pillars
            Box("Landing rim beam north", new Vector3(0f, rimY, northZ - postHalf), new Vector3(innerSpanX, rimH, rimW), S.DarkWood, true);

            // East Railing: runs between NE pillar and SE pillar along eastX - postHalf
            float railX = eastX - postHalf;
            float eastSpan = landDepth - postSize * 2f;
            Box("Landing east handrail", new Vector3(railX, landY + 0.85f, landZ), new Vector3(0.08f, 0.08f, eastSpan), S.Wood);
            Box("Landing east base rail", new Vector3(railX, landY + 0.04f, landZ), new Vector3(0.06f, 0.06f, eastSpan), S.DarkWood);
            for (int b = 0; b < 6; b++)
            {
                float bz = southZ + postSize + (b + 0.5f) * (eastSpan / 6f);
                Box("Landing east baluster", new Vector3(railX, landY + 0.45f, bz), new Vector3(0.045f, 0.74f, 0.045f), S.Wood);
            }

            // South Railing: runs between SW pillar and SE pillar along southZ + postHalf
            float railZ = southZ + postHalf;
            float southSpan = landWidth - postSize * 2f;
            Box("Landing south handrail", new Vector3(0f, landY + 0.85f, railZ), new Vector3(southSpan, 0.08f, 0.08f), S.Wood);
            Box("Landing south base rail", new Vector3(0f, landY + 0.04f, railZ), new Vector3(southSpan, 0.06f, 0.06f), S.DarkWood);
            for (int b = 0; b < 6; b++)
            {
                float bx = westX + postSize + (b + 0.5f) * (southSpan / 6f);
                Box("Landing south baluster", new Vector3(bx, landY + 0.45f, railZ), new Vector3(0.045f, 0.74f, 0.045f), S.Wood);
            }

            // --- FLIGHT 2 (Lower flight): 7 steps turning 90 degrees to West (local -X) ---
            float treadWidth = landDepth - postSize * 2f; // 1.36m (fits cleanly between SW and NW pillars)
            for (int i = 0; i < 7; i++)
            {
                float y = landY - (i + 1) * stepRise + stepRise * 0.5f;
                float x = westX - (i + 0.5f) * stepRun;
                Box("Lower stair tread", new Vector3(x, y, landZ), new Vector3(stepRun, 0.16f, treadWidth), S.Wood, true);
            }

            // Flight 2 Handrails: connect directly to Landing SW and NW pillars!
            float bottomX = westX - 7 * stepRun; // -2.76m
            for (int side = -1; side <= 1; side += 2)
            {
                // side = -1 -> southZ + postHalf (SW pillar center!)
                // side = +1 -> northZ - postHalf (NW pillar center!)
                float z = (side == -1) ? (southZ + postHalf) : (northZ - postHalf);
                Vector3 pTop = new Vector3(westX + postHalf, landY + 0.85f, z);
                Vector3 pBot = new Vector3(bottomX, 0.00f + 0.85f, z);

                Beam("Lower stair stringer", new Vector3(westX + postHalf, landY + 0.02f, z), new Vector3(bottomX, 0.02f, z), 0.13f, S.DarkWood);
                Beam("Lower stair handrail", pTop, pBot, 0.08f, S.Wood);
                // Carved bottom newel post at bottom of flight extending into soil
                Box("Lower newel post", new Vector3(bottomX, 0.42f, z), new Vector3(postSize, 1.34f, postSize), S.DarkWood, true);

                for (int b = 0; b < 6; b++)
                {
                    float f = (b + 0.5f) / 6f;
                    Vector3 bp = Vector3.Lerp(pTop, pBot, f) - Vector3.up * 0.40f;
                    Box("Lower stair baluster", bp, new Vector3(0.045f, 0.70f, 0.045f), S.Wood);
                }
            }

            Parent = prevParent;
            ModelCombiner.CombineModel(stairGroup, addCollider: true);

            // 4. Traditional Foot Washing Station (Earthenware Water Jar & Slatted Teak Platform)
            // Cleanly placed beside the North rail of the bottom step (local Z = -9.41m)
            var oldWash = houseRoot.Find("Traditional foot washing station");
            if (oldWash != null) UnityEngine.Object.DestroyImmediate(oldWash.gameObject);
            Parent = houseRoot;
            PropsBuilder.FootWashStation(new Vector3(-2.76f, 0.02f, -9.41f));
            Parent = prevParent;
        }
    }
}
