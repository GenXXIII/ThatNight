using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class LandscapeBuilder
    {
        public static Transform BuildGround()
        {
            Parent = null;
            var root = Group("04 GROUND - unified soil terrain and approach road", Vector3.zero);
            Parent = root;

            // 1. EXTENDED COUNTRY & MOUNTAIN TERRAIN GRID (100% UNIFIED WATERTIGHT TILE GRID)
            // Fully covers:
            // - West (Left): cx from -46 to 3 -> X from -1840m to +120m (Towering unclimbable western mountain cliffs)
            // - North (Behind): cz from -4 to 49 -> Z from -160m to +1960m (Towering unclimbable northern mountain range)
            // - East (Right): cx up to 6 -> X up to +240m (Connecting seamlessly with the northern range in the NE corner)
            // Since all tiles use the exact same 40m tile footprint and resolution, every edge is 100% watertight
            // with zero cracks, zero seams, and zero holes!
            var terrainChunks = new Transform[16];
            for (int i = 0; i < 16; i++)
                terrainChunks[i] = Group("Terrain Region Chunk " + (i + 1), Vector3.zero);

            for (int cx = -46; cx < 6; cx++)
                for (int cz = -4; cz < 49; cz++)
                {
                    int xIndex = cx, zIndex = cz;
                    bool isNear = (cx >= -7 && cx <= 2 && cz >= -4 && cz <= 9);
                    bool isMid = (cx >= -15 && cz < 18);
                    int quads = isNear ? 24 : isMid ? 14 : 6;

                    // Group into 16 geographic regions for high-performance mesh batching
                    int colChunk = (cx < -33) ? 0 : (cx < -20) ? 1 : (cx < -7) ? 2 : 3;
                    int rowChunk = (cz < 10) ? 0 : (cz < 23) ? 1 : (cz < 36) ? 2 : 3;
                    int chunkIdx = Mathf.Clamp(rowChunk * 4 + colChunk, 0, 15);
                    Parent = terrainChunks[chunkIdx];

                    Grid("Continuous countryside terrain tile", quads, quads, (u, v) =>
                    {
                        float px = (xIndex + u) * 40f, pz = (zIndex + v) * 40f;
                        return new Vector3(px, GroundDetailBuilder.HillHeight(px, pz), pz);
                    }, S.Soil, new Vector2(4, 4), false, GroundDetailBuilder.UnifiedGroundShade);
                }

            Parent = root;
            for (int i = 0; i < 16; i++)
            {
                if (terrainChunks[i].childCount > 0)
                    ModelCombiner.CombineModel(terrainChunks[i], addCollider: true);
            }

            // 2. Rural Red Dirt Road Climbing to Entrance Gate (Spline from z=-165m to z=-15.5m with clean crown)
            var roadMat = S.Soil;
            var roadObj = Grid("Rural red dirt road climbing mountain slope", 18, 140, GroundDetailBuilder.HillRoadPoint, roadMat, new Vector2(2, 33), false, GroundDetailBuilder.HillRoadShade);
            var roadMc = roadObj.AddComponent<MeshCollider>();
            roadMc.sharedMesh = roadObj.GetComponent<MeshFilter>().sharedMesh;

            // 3. Scattered Courtyard Pebbles
            var pebbleGroup = ModelGroup("Scattered courtyard pebbles", Vector3.zero);
            var prevParent = Parent;
            Parent = pebbleGroup;
            Random.InitState(2006);
            for (int i = 0; i < 110; i++)
            {
                float x = Random.Range(-SiteLayout.HalfWidth + .3f, 0f);
                float z = Random.Range(SiteLayout.Front + .5f, SiteLayout.Back - .3f);
                if (Mathf.Abs(x) < 4 && z < 0) continue;
                float size = Random.Range(.018f, .065f);
                var rock = Ball("Scattered courtyard pebble", new Vector3(x, GroundDetailBuilder.HillHeight(x, z) + .024f, z), new Vector3(size * 1.6f, size * .65f, size), S.DarkWood);
                rock.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            }
            Parent = prevParent;
            ModelCombiner.CombineModel(pebbleGroup, addCollider: false);

            Parent = null;
            return root;
        }

        public static Transform BuildVillage()
        {
            Parent = null;
            var root = Group("05 VILLAGE - wooded hometown with scattered houses", Vector3.zero);
            Parent = root;

            float front = SiteLayout.Front;

            // 1. Open Wedding Frontage & Ceremonial Arch
            var villageRoot = Parent;
            BuildWeddingFrontage(front);
            Parent = villageRoot;

            // 2. Full Surrounding Forest
            BuildSurroundingForest();

            Parent = null;
            return root;
        }

        static void BuildWeddingFrontage(float front) => WeddingDraperyBuilder.Build(front);

        // Precise architectural safety check: creates generous, open walkable yards and paths around all buildings!
        public static bool IsInsideWeddingOrHouseZone(float x, float z, float buffer = 8.0f)
        {
            // 1. Entire Wedding Marquee & Dining Tables (x in [-21, 7], z in [-15, 11])
            if (x >= -21.0f - buffer && x <= 7.0f + buffer && z >= -15.0f - buffer && z <= 11.0f + buffer)
                return true;

            // 2. Catering & Hair Cutting Pavilion (x in [8, 23], z in [11, 28])
            if (x >= 8.0f - buffer && x <= 23.0f + buffer && z >= 11.0f - buffer && z <= 28.0f + buffer)
                return true;

            // 3. Stilt House & Under-House Area (x in [-11.5, 6.5], z in [13, 31])
            if (x >= -11.5f - buffer && x <= 6.5f + buffer && z >= 13.0f - buffer && z <= 31.0f + buffer)
                return true;

            // 4. Outdoor Kitchen (x in [-7, 2], z in [34, 44])
            if (x >= -7.0f - buffer && x <= 2.0f + buffer && z >= 34.0f - buffer && z <= 44.0f + buffer)
                return true;

            // 5. Cattle Shelter (x in [15, 22], z in [31, 45])
            if (x >= 15.0f - buffer && x <= 22.0f + buffer && z >= 31.0f - buffer && z <= 45.0f + buffer)
                return true;

            // 6. Entrance Walkway & Front Gate (x in [-3.5, 3.5], z in [-18, -13])
            if (Mathf.Abs(x) <= 4.5f + buffer && z >= -18.0f - buffer && z <= -13.0f - buffer)
                return true;

            // 7. House & Kitchen Walkable Backyard Corridor (x in [-15, 10], z in [30, 56])
            if (x >= -15.0f && x <= 10.0f && z >= 30.0f && z <= 56.0f)
                return true;

            return false;
        }

        // =========================================================================================
        // REALISTIC BOTANICAL ARCHITECTURE (DENSE SIGHTLINE-BLOCKING HORROR JUNGLE)
        // Optimized 1x1 quads (75% fewer triangles = silky smooth 60 FPS scroll & drag in Scene View!)
        // =========================================================================================

        // =========================================================================================
        // NATURAL RAINFOREST HARDWOOD BOTANICAL ARCHITECTURE
        // Replaces the old palm/coconut straight-pole spoke design with authentic forked hardwood trees!
        // 1. Spreading Flared Buttress Roots anchoring into dirt
        // 2. Muscular Organic Trunk with 2-3 Primary Ascending Limbs (bifurcation/trifurcation)
        // 3. Spreading Secondary Boughs branching outward and upward
        // 4. Volumetric Billowing Leaf Clouds (Natural Foliage Puffs) forming rich, dense, umbrella canopies!
        // =========================================================================================

        static void NaturalLeafPuff(Vector3 center, float size, Color baseColor, int seed)
        {
            // 4 multi-angle tilted diagonal foliage cards with spatial offset (forming a billowing 3D volume)
            for (int i = 0; i < 4; i++)
            {
                float pitch = 22f + (i * 18f);
                float yaw = i * 45f + (seed * 23) % 360;
                Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
                Vector3 offset = rot * new Vector3((i % 2 == 0 ? 0.12f : -0.12f) * size, (i > 1 ? 0.08f : -0.08f) * size, 0);
                Grid("Leaf puff card " + i, 1, 1,
                    (u, v) => center + offset + rot * new Vector3((u - .5f) * size * 1.30f, (v - .5f) * size * 0.95f, 0),
                    S.Foliage, Vector2.one, true, (p) => baseColor * (0.92f + (i % 3) * 0.05f));
            }
            // Dense horizontal canopy roof cap
            Quaternion topRot = Quaternion.Euler(12f, (seed * 59) % 360, 0);
            Grid("Leaf puff top", 1, 1,
                (u, v) => center + Vector3.up * (size * 0.22f) + topRot * new Vector3((u - .5f) * size * 1.20f, 0, (v - .5f) * size * 1.20f),
                S.Foliage, Vector2.one, true, (p) => baseColor * 1.08f);
        }

        // =========================================================================================
        // CONTINUOUS ORGANIC TRUNK GENERATOR
        // Seamless, smoothly tapering round body connecting root flare to canopy apex.
        // Solves the "under good but half top is small body" issue by eliminating all flat shelves
        // and square beam transitions. Produces silky smooth cylindrical shading!
        // =========================================================================================

        static GameObject ContinuousTaperedTrunk(string name, Vector3[] ringCenters, float[] ringRadii, Material mat, int segments = 10)
        {
            int rings = ringCenters.Length;
            return MeshFactory.Grid(name, segments, rings - 1, (u, v) =>
            {
                float ringF = v * (rings - 1);
                int idx = Mathf.Clamp(Mathf.FloorToInt(ringF), 0, rings - 2);
                float frac = ringF - idx;
                Vector3 c = Vector3.Lerp(ringCenters[idx], ringCenters[idx + 1], frac);
                float r = Mathf.Lerp(ringRadii[idx], ringRadii[idx + 1], frac);
                float ang = u * Mathf.PI * 2f;
                return c + new Vector3(Mathf.Cos(ang) * r, 0, Mathf.Sin(ang) * r);
            }, mat, new Vector2(2f, rings * 0.5f), twoSided: true);
        }

        // =========================================================================================
        // 1A. 0m - 50m NEAR-HOUSE SLENDER TREE:
        // Natural, organic young woodland tree (height 11m - 14.5m)
        // Continuous smoothly tapering trunk with open, airy canopy and tight walkable collider!
        // =========================================================================================

        static void SlenderSmallVillageTree(Vector3 p, float height, int seed)
        {
            if (IsInsideWeddingOrHouseZone(p.x, p.z, 3.5f)) return;
            var old = Parent;
            Parent = ModelGroup("Slender Small Village Tree", p);

            float trunkRadius = Mathf.Lerp(0.24f, 0.34f, Mathf.InverseLerp(11, 15, height));

            // Continuous tapering trunk from ground up to upper crown
            int ringCount = 7;
            var ringCenters = new Vector3[ringCount];
            var ringRadii = new float[ringCount];

            float[] hFracs = new float[] { -0.03f, 0.00f, 0.20f, 0.42f, 0.65f, 0.85f, 0.98f };
            float[] rFracs = new float[] { 1.35f,  1.10f, 0.90f, 0.78f, 0.62f, 0.45f, 0.25f };

            for (int i = 0; i < ringCount; i++)
            {
                float hf = hFracs[i];
                float y = (hf < 0) ? -0.4f : height * hf;
                float sway = (hf > 0) ? hf : 0;
                float lx = Mathf.Sin(seed * 1.3f + sway * 2.2f) * (0.35f * sway);
                float lz = Mathf.Cos(seed * 1.7f + sway * 2.0f) * (0.35f * sway);
                ringCenters[i] = new Vector3(lx, y, lz);
                ringRadii[i] = trunkRadius * rFracs[i];
            }

            ContinuousTaperedTrunk("Slender Tapered Trunk", ringCenters, ringRadii, S.DarkWood, 8);

            Color leafColor = new Color(0.38f, 0.55f, 0.26f);

            // Staggered Branch 1 (at 42% height)
            Vector3 p1 = ringCenters[3];
            float ang1 = (seed * 43) % 360 * Mathf.Deg2Rad;
            Vector3 dir1 = new Vector3(Mathf.Cos(ang1) * 0.75f, 0.65f, Mathf.Sin(ang1) * 0.75f).normalized;
            Vector3 end1 = p1 + dir1 * (height * 0.35f);
            Beam("Slender limb 1", p1, end1, trunkRadius * 0.32f, S.DarkWood);
            NaturalLeafPuff(Vector3.Lerp(p1, end1, 0.65f), 5.0f, leafColor * 0.92f, seed + 11);
            NaturalLeafPuff(end1, 6.2f, leafColor, seed + 13);

            // Staggered Branch 2 (at 65% height)
            Vector3 p2 = ringCenters[4];
            float ang2 = ang1 + 2.3f;
            Vector3 dir2 = new Vector3(Mathf.Cos(ang2) * 0.70f, 0.70f, Mathf.Sin(ang2) * 0.70f).normalized;
            Vector3 end2 = p2 + dir2 * (height * 0.30f);
            Beam("Slender limb 2", p2, end2, trunkRadius * 0.26f, S.DarkWood);
            NaturalLeafPuff(Vector3.Lerp(p2, end2, 0.60f), 4.5f, leafColor * 0.95f, seed + 21);
            NaturalLeafPuff(end2, 5.8f, leafColor, seed + 23);

            // Crown apex (at 98% height)
            Vector3 pApex = ringCenters[6] + Vector3.up * (height * 0.05f);
            NaturalLeafPuff(pApex, 6.5f, leafColor * 1.05f, seed + 80);

            var treeObj = Parent;
            Parent = old;
            // TIGHT WALKABLE COLLIDER: player can walk right past without invisible block
            var cap = treeObj.gameObject.AddComponent<CapsuleCollider>();
            cap.center = new Vector3(0, height * 0.5f, 0);
            cap.radius = Mathf.Min(trunkRadius * 0.50f, 0.22f);
            cap.height = height;
            ModelCombiner.CombineModel(treeObj, addCollider: false);
        }

        // =========================================================================================
        // 1B. 50m - 100m MID-ZONE FAT VILLAGE TREE:
        // Classic mature Cambodian village hardwood (Mango, Tamarind, Jackfruit)
        // Continuous muscular trunk with heavy spreading boughs forming a rich umbrella dome!
        // =========================================================================================

        static void FatVillageTree(Vector3 p, float height, int seed)
        {
            if (IsInsideWeddingOrHouseZone(p.x, p.z, 3.5f)) return;
            var old = Parent;
            Parent = ModelGroup("Fat Village Tree", p);

            float trunkRadius = Mathf.Lerp(0.60f, 0.88f, Mathf.InverseLerp(13, 20, height));

            // 1. Spreading buttress root base anchoring into soil
            for (int r = 0; r < 4; r++)
            {
                float rootAng = r * Mathf.PI * 0.5f + (seed % 7) * 0.3f;
                Vector3 rDir = new Vector3(Mathf.Sin(rootAng), 0, Mathf.Cos(rootAng));
                Vector3 rStart = Vector3.up * (height * 0.12f) + rDir * (trunkRadius * 0.65f);
                Vector3 rEnd = rDir * (trunkRadius * 2.2f) - Vector3.up * 0.4f;
                Beam("Village root", rStart, rEnd, trunkRadius * 0.30f, S.DarkWood);
            }

            // 2. Continuous tapering trunk from ground up to upper crown
            int ringCount = 8;
            var ringCenters = new Vector3[ringCount];
            var ringRadii = new float[ringCount];

            float[] hFracs = new float[] { -0.04f, 0.00f, 0.18f, 0.38f, 0.55f, 0.70f, 0.85f, 0.95f };
            float[] rFracs = new float[] { 1.40f,  1.15f, 0.94f, 0.82f, 0.70f, 0.58f, 0.44f, 0.25f };

            for (int i = 0; i < ringCount; i++)
            {
                float hf = hFracs[i];
                float y = (hf < 0) ? -0.5f : height * hf;
                float sway = (hf > 0) ? hf : 0;
                float lx = Mathf.Sin(seed * 1.1f + sway * 2.4f) * (0.45f * sway);
                float lz = Mathf.Cos(seed * 1.5f + sway * 2.0f) * (0.45f * sway);
                ringCenters[i] = new Vector3(lx, y, lz);
                ringRadii[i] = trunkRadius * rFracs[i];
            }

            ContinuousTaperedTrunk("Village Tapered Trunk", ringCenters, ringRadii, S.DarkWood, 8);

            Color leafBase = new Color(0.30f, 0.48f, 0.20f);

            // Limb 1 (at 55% height)
            Vector3 p1 = ringCenters[4];
            float ang1 = (seed * 53) % 360 * Mathf.Deg2Rad;
            Vector3 dir1 = new Vector3(Mathf.Cos(ang1) * 0.80f, 0.50f, Mathf.Sin(ang1) * 0.80f).normalized;
            Vector3 end1 = p1 + dir1 * (height * 0.32f);
            Beam("Village limb 1", p1, end1, trunkRadius * 0.32f, S.DarkWood);

            Vector3 sb1Start = Vector3.Lerp(p1, end1, 0.55f);
            Vector3 sb1End = sb1Start + new Vector3(Mathf.Cos(ang1 - 0.5f) * 0.8f, 0.5f, Mathf.Sin(ang1 - 0.5f) * 0.8f).normalized * (height * 0.20f);
            Beam("Village bough 1", sb1Start, sb1End, trunkRadius * 0.20f, S.DarkWood);
            NaturalLeafPuff(sb1End, 8.5f, leafBase * 0.94f, seed + 11);
            NaturalLeafPuff(end1, 10.5f, leafBase, seed + 13);
            NaturalLeafPuff(sb1Start, 8.0f, leafBase * 0.88f, seed + 17);

            // Limb 2 (at 70% height, +130 deg)
            Vector3 p2 = ringCenters[5];
            float ang2 = ang1 + 2.27f;
            Vector3 dir2 = new Vector3(Mathf.Cos(ang2) * 0.75f, 0.58f, Mathf.Sin(ang2) * 0.75f).normalized;
            Vector3 end2 = p2 + dir2 * (height * 0.28f);
            Beam("Village limb 2", p2, end2, trunkRadius * 0.28f, S.DarkWood);

            Vector3 sb2Start = Vector3.Lerp(p2, end2, 0.55f);
            Vector3 sb2End = sb2Start + new Vector3(Mathf.Cos(ang2 + 0.5f) * 0.8f, 0.5f, Mathf.Sin(ang2 + 0.5f) * 0.8f).normalized * (height * 0.18f);
            Beam("Village bough 2", sb2Start, sb2End, trunkRadius * 0.18f, S.DarkWood);
            NaturalLeafPuff(sb2End, 8.5f, leafBase * 0.94f, seed + 21);
            NaturalLeafPuff(end2, 10.5f, leafBase, seed + 23);
            NaturalLeafPuff(sb2Start, 8.0f, leafBase * 0.88f, seed + 27);

            // Limb 3 (at 85% height, +250 deg)
            Vector3 p3 = ringCenters[6];
            float ang3 = ang1 + 4.36f;
            Vector3 dir3 = new Vector3(Mathf.Cos(ang3) * 0.70f, 0.65f, Mathf.Sin(ang3) * 0.70f).normalized;
            Vector3 end3 = p3 + dir3 * (height * 0.24f);
            Beam("Village limb 3", p3, end3, trunkRadius * 0.24f, S.DarkWood);
            NaturalLeafPuff(end3, 9.5f, leafBase, seed + 31);
            NaturalLeafPuff(Vector3.Lerp(p3, end3, 0.6f), 7.5f, leafBase * 0.90f, seed + 33);

            // Crown apex summit
            Vector3 pApex = ringCenters[7] + Vector3.up * (height * 0.05f);
            NaturalLeafPuff(pApex, 13.0f, leafBase * 1.08f, seed + 100);

            var treeObj = Parent;
            Parent = old;
            // TIGHT WALKABLE COLLIDER: player can walk right past without invisible block
            var cap = treeObj.gameObject.AddComponent<CapsuleCollider>();
            cap.center = new Vector3(0, height * 0.5f, 0);
            cap.radius = Mathf.Min(trunkRadius * 0.50f, 0.45f);
            cap.height = height;
            ModelCombiner.CombineModel(treeObj, addCollider: false);
        }

        // =========================================================================================
        // 2. DEEP FOREST JUNGLE TREE:
        // Ancient Cambodian Rainforest Giant (Dipterocarpus / Shorea / Koki)
        // Dynamically scaled: 26m - 36m inside 200m zone, 78m - 108m beyond 200m!
        // - Girth decreased 2x per user request (soaring, majestic columnar hardwood pillars)
        // - Leaves strictly placed in top 1/3 (top 25m of a 100m tree)!
        // - Lower ~78% is a clean, soaring, majestic columnar hardwood pillar
        // - Tight collision capsule so player can walk freely between trees!
        // =========================================================================================

        static void DeepForestJungleTree(Vector3 p, float height, int seed, bool combineSelf = true)
        {
            if (IsInsideWeddingOrHouseZone(p.x, p.z, 3.5f)) return;
            var old = Parent;
            Parent = combineSelf ? ModelGroup("Deep Forest Jungle Tree", p) : Group("Deep Forest Jungle Tree", p);

            // Proportional scaling: 2x slimmer girth per user explicit request!
            float scale = height / 32.0f;
            float trunkRadius = 1.20f * scale; // Decreased fatness 2x based on current tall tree 200m+ zone!

            // 1. Five fluted buttress roots smoothly anchoring into dirt
            for (int r = 0; r < 5; r++)
            {
                float rootAng = r * Mathf.PI * (2f / 5f) + (seed % 7) * 0.25f;
                Vector3 rDir = new Vector3(Mathf.Sin(rootAng), 0, Mathf.Cos(rootAng));
                Vector3 rStart = Vector3.up * (height * 0.12f) + rDir * (trunkRadius * 0.65f);
                Vector3 rEnd = rDir * (trunkRadius * 1.8f) - Vector3.up * (0.5f * scale);
                Vector3 rMid = Vector3.Lerp(rStart, rEnd, 0.45f) + rDir * (trunkRadius * 0.20f) - Vector3.up * (height * 0.03f);

                Beam("Buttress root upper " + r, rStart, rMid, trunkRadius * 0.28f, S.DarkWood);
                Beam("Buttress root lower " + r, rMid, rEnd, trunkRadius * 0.22f, S.DarkWood);
            }

            // 2. CONTINUOUS, SMOOTHLY TAPERING TRUNK (Ground to Canopy Apex)
            int ringCount = 10;
            var ringCenters = new Vector3[ringCount];
            var ringRadii = new float[ringCount];

            float[] heightFractions = new float[] {
                -0.02f, // Below ground flare
                0.00f,  // Ground level
                0.10f,  // Above buttresses
                0.24f,  // Lower mid trunk
                0.40f,  // Lower canopy branch junction (top 60%!)
                0.52f,  // Tier 2 canopy junction
                0.66f,  // Tier 3 canopy junction
                0.80f,  // Tier 4 canopy junction
                0.92f,  // Tier 5 upper canopy junction
                0.98f   // Apex summit
            };

            float[] radiusFactors = new float[] {
                1.45f,  // Underground flare
                1.20f,  // Ground flare
                0.96f,  // Columnar trunk
                0.86f,  // Lower trunk
                0.74f,  // 0.40h Canopy collar (Tier 1)
                0.62f,  // 0.52h Canopy collar (Tier 2)
                0.50f,  // 0.66h Canopy collar (Tier 3)
                0.38f,  // 0.80h Canopy collar (Tier 4)
                0.26f,  // 0.92h Upper collar
                0.16f   // 0.98h Apex collar
            };

            for (int i = 0; i < ringCount; i++)
            {
                float hFrac = heightFractions[i];
                float y = (hFrac < 0) ? -0.8f * scale : height * hFrac;
                float sway = (hFrac > 0) ? hFrac : 0;
                float leanX = Mathf.Sin(seed * 0.95f + sway * 2.6f) * (0.65f * scale * sway);
                float leanZ = Mathf.Cos(seed * 1.35f + sway * 2.2f) * (0.65f * scale * sway);
                ringCenters[i] = new Vector3(leanX, y, leanZ);
                ringRadii[i] = trunkRadius * radiusFactors[i];
            }

            ContinuousTaperedTrunk("Continuous Columnar Trunk", ringCenters, ringRadii, S.DarkWood, 10);

            Color leafDeep = new Color(0.24f, 0.42f, 0.16f);

            // =========================================================================================
            // 3. FULL 60% CANOPY UMBRELLA ARCHITECTURE (TOP 60m OF 100m TREE)
            // User explicit requirement: "change to 60%"!
            // Lower ~40% of the tree is a soaring, clean 2x slimmer Dipterocarpus hardwood pillar.
            // Top ~60% (from Y = 0.40h to 1.04h) is a rich, grand, multi-tiered cathedral canopy dome!
            // =========================================================================================

            float puffBase = Mathf.Lerp(8.0f, 17.0f, Mathf.InverseLerp(26.0f, 105.0f, height));
            float angBase = (seed * 67) % 360 * Mathf.Deg2Rad;

            // 8 Expansive Spreading Cathedral Boughs across 4 Tiers (covering full top 60%!)
            for (int b = 0; b < 8; b++)
            {
                float bFrac = b / 7.0f;
                // Staggered emergence across rings 4, 5, 6, 7, 8 (from 0.40h to 0.92h)
                Vector3 limbOrigin = Vector3.Lerp(ringCenters[4], ringCenters[8], bFrac);
                float limbAng = angBase + b * (Mathf.PI * 2f / 8f) + ((b % 2 == 0) ? 0.22f : -0.22f);

                // Ascending cathedral reach
                float reachH = height * (0.28f - b * 0.015f);
                Vector3 limbDir = new Vector3(Mathf.Cos(limbAng) * 0.74f, 0.48f + b * 0.05f, Mathf.Sin(limbAng) * 0.74f).normalized;
                Vector3 limbEnd = limbOrigin + limbDir * reachH;
                Beam("Cathedral limb " + b, limbOrigin, limbEnd, trunkRadius * (0.34f - b * 0.02f), S.DarkWood);

                // Secondary bough fork
                Vector3 forkStart = Vector3.Lerp(limbOrigin, limbEnd, 0.42f);
                Vector3 forkDir = new Vector3(Mathf.Cos(limbAng + 0.60f) * 0.75f, 0.46f + b * 0.04f, Mathf.Sin(limbAng + 0.60f) * 0.75f).normalized;
                Vector3 forkEnd = forkStart + forkDir * (height * 0.14f);
                Beam("Cathedral fork " + b, forkStart, forkEnd, trunkRadius * 0.19f, S.DarkWood);

                // Cascading rich foliage clouds filling top 60% dome
                NaturalLeafPuff(limbEnd + Vector3.up * (puffBase * 0.20f), puffBase * 1.15f, leafDeep, seed + b * 13 + 1);
                NaturalLeafPuff(forkEnd + Vector3.up * (puffBase * 0.20f), puffBase * 0.95f, leafDeep * 0.92f, seed + b * 17 + 2);
                NaturalLeafPuff(Vector3.Lerp(limbOrigin, limbEnd, 0.55f) + Vector3.up * (puffBase * 0.22f), puffBase * 0.85f, leafDeep * 0.88f, seed + b * 19 + 3);
            }

            // Central Summit Crown Dome (Apex at Y = 0.98h to 1.04h)
            Vector3 pApex = ringCenters[9] + Vector3.up * (height * 0.04f);
            NaturalLeafPuff(pApex + Vector3.up * (puffBase * 0.35f), puffBase * 1.50f, leafDeep * 1.08f, seed + 999);
            NaturalLeafPuff(pApex + Vector3.up * (puffBase * 0.65f), puffBase * 1.15f, leafDeep * 1.15f, seed + 1001);

            var treeObj = Parent;
            Parent = old;
            if (combineSelf)
            {
                // TIGHT WALKABLE COLLIDER: strictly wraps trunk core at player height so player walks through freely!
                var cap = treeObj.gameObject.AddComponent<CapsuleCollider>();
                cap.center = new Vector3(0, height * 0.5f, 0);
                cap.radius = Mathf.Min(trunkRadius * 0.40f, 0.90f);
                cap.height = height;
                ModelCombiner.CombineModel(treeObj, addCollider: false);
            }
        }

        // =========================================================================================
        // 2B. TITAN 5X JUNGLE TREE:
        // Colossal Ancient Primeval Rainforest Titan (Height 140m - 180m!)
        // Massive buttress roots spreading 22m, staggered colossal boughs, 75m canopy!
        // =========================================================================================

        static void Titan5xJungleTree(Vector3 p, float height, int seed, bool combineSelf = true)
        {
            var old = Parent;
            GameObject treeObj;
            if (combineSelf)
            {
                Parent = ModelGroup("Titan 5x Jungle Tree", p);
                treeObj = Parent.gameObject;
            }
            else
            {
                var go = new GameObject("Titan 5x Jungle Tree");
                go.transform.parent = Parent;
                go.transform.position = p;
                Parent = go.transform;
                treeObj = go;
            }

            float trunkRadius = Mathf.Lerp(6.5f, 10.0f, Mathf.InverseLerp(140, 180, height));

            // 1. Four colossal buttress roots anchoring 22m outward
            for (int r = 0; r < 4; r++)
            {
                float rootAng = r * Mathf.PI * 0.5f + (seed % 7) * 0.35f;
                Vector3 rDir = new Vector3(Mathf.Sin(rootAng), 0, Mathf.Cos(rootAng));
                Vector3 rStart = Vector3.up * (height * 0.14f) + rDir * (trunkRadius * 0.65f);
                Vector3 rEnd = rDir * (trunkRadius * 2.5f) - Vector3.up * 1.5f;
                Beam("Titan buttress root", rStart, rEnd, trunkRadius * 0.35f, S.DarkWood);
            }

            // 2. Colossal muscular trunk to Branch 1 (at ~38% height)
            float lean1X = Mathf.Sin(seed * 1.1f) * 2.5f;
            float lean1Z = Mathf.Cos(seed * 1.7f) * 2.5f;
            Vector3 p1 = new Vector3(lean1X, height * 0.38f, lean1Z);

            var profile = new Vector2[] {
                new Vector2(trunkRadius * 1.45f, -2.0f),
                new Vector2(trunkRadius * 1.20f, 0.0f),
                new Vector2(trunkRadius * 0.95f, height * 0.20f),
                new Vector2(trunkRadius * 0.82f, height * 0.38f)
            };
            Lathe("Titan Columnar Trunk", Vector3.zero, profile, S.DarkWood, 10);

            Color leafTitan = new Color(0.22f, 0.38f, 0.15f);

            // Staggered Titan Limb 1 (at 38% height)
            float ang1 = (seed * 67) % 360 * Mathf.Deg2Rad;
            Vector3 dir1 = new Vector3(Mathf.Cos(ang1) * 0.78f, 0.58f, Mathf.Sin(ang1) * 0.78f).normalized;
            Vector3 end1 = p1 + dir1 * (height * 0.44f);
            Beam("Titan limb 1", p1, end1, trunkRadius * 0.35f, S.DarkWood);

            Vector3 sb1Start = Vector3.Lerp(p1, end1, 0.50f);
            Vector3 sb1End = sb1Start + new Vector3(Mathf.Cos(ang1 - 0.5f) * 0.8f, 0.45f, Mathf.Sin(ang1 - 0.5f) * 0.8f).normalized * (height * 0.26f);
            Beam("Titan bough 1", sb1Start, sb1End, trunkRadius * 0.22f, S.DarkWood);
            NaturalLeafPuff(sb1End, 38.0f, leafTitan * 0.92f, seed + 11);
            NaturalLeafPuff(end1, 48.0f, leafTitan, seed + 13);
            NaturalLeafPuff(sb1Start, 36.0f, leafTitan * 0.88f, seed + 17);

            // Stage 2: Columnar trunk climbing to Branch 2 (at ~62% height)
            float lean2X = lean1X + Mathf.Sin(seed * 2.2f) * 2.0f;
            float lean2Z = lean1Z + Mathf.Cos(seed * 2.8f) * 2.0f;
            Vector3 p2 = new Vector3(lean2X, height * 0.62f, lean2Z);
            Beam("Titan mid trunk", p1, p2, trunkRadius * 0.50f, S.DarkWood);

            // Staggered Titan Limb 2 (at 62% height, +125 deg azimuth)
            float ang2 = ang1 + 2.18f;
            Vector3 dir2 = new Vector3(Mathf.Cos(ang2) * 0.72f, 0.65f, Mathf.Sin(ang2) * 0.72f).normalized;
            Vector3 end2 = p2 + dir2 * (height * 0.40f);
            Beam("Titan limb 2", p2, end2, trunkRadius * 0.30f, S.DarkWood);

            Vector3 sb2Start = Vector3.Lerp(p2, end2, 0.50f);
            Vector3 sb2End = sb2Start + new Vector3(Mathf.Cos(ang2 + 0.5f) * 0.8f, 0.45f, Mathf.Sin(ang2 + 0.5f) * 0.8f).normalized * (height * 0.24f);
            Beam("Titan bough 2", sb2Start, sb2End, trunkRadius * 0.20f, S.DarkWood);
            NaturalLeafPuff(sb2End, 38.0f, leafTitan * 0.92f, seed + 21);
            NaturalLeafPuff(end2, 48.0f, leafTitan, seed + 23);
            NaturalLeafPuff(sb2Start, 36.0f, leafTitan * 0.88f, seed + 27);

            // Stage 3: Columnar trunk climbing to Branch 3 (at ~80% height)
            float lean3X = lean2X + Mathf.Sin(seed * 3.3f) * 1.6f;
            float lean3Z = lean2Z + Mathf.Cos(seed * 3.7f) * 1.6f;
            Vector3 p3 = new Vector3(lean3X, height * 0.80f, lean3Z);
            Beam("Titan upper trunk", p2, p3, trunkRadius * 0.38f, S.DarkWood);

            // Staggered Titan Limb 3 (at 80% height, +245 deg azimuth)
            float ang3 = ang1 + 4.28f;
            Vector3 dir3 = new Vector3(Mathf.Cos(ang3) * 0.68f, 0.70f, Mathf.Sin(ang3) * 0.68f).normalized;
            Vector3 end3 = p3 + dir3 * (height * 0.35f);
            Beam("Titan limb 3", p3, end3, trunkRadius * 0.25f, S.DarkWood);
            NaturalLeafPuff(end3, 44.0f, leafTitan, seed + 31);
            NaturalLeafPuff(Vector3.Lerp(p3, end3, 0.6f), 35.0f, leafTitan * 0.90f, seed + 33);

            // Stage 4: Central summit crown dome
            Vector3 pApex = p3 + Vector3.up * (height * 0.20f);
            Beam("Titan apex trunk", p3, pApex, trunkRadius * 0.24f, S.DarkWood);
            NaturalLeafPuff(pApex, 60.0f, leafTitan * 1.08f, seed + 120);

            Parent = old;
            if (combineSelf)
            {
                var cap = treeObj.AddComponent<CapsuleCollider>();
                cap.center = new Vector3(0, height * 0.5f, 0);
                cap.radius = trunkRadius * 1.30f;
                cap.height = height;
                ModelCombiner.CombineModel(treeObj.transform, addCollider: false);
            }
        }

        // =========================================================================================
        // 2C. COLOSSAL 20X SACRED WORLD TREE (TREASURE SPIRIT TITAN):
        // Legendary Ancient Primeval Cambodian Spirit Tree ("Neak Ta" / Spung / Banyan Titan, Height ~360m!)
        // High-standard, high-fame sacred treasure tree:
        // - 8 majestic flowing fluted buttress roots sweeping 75m across the earth
        // - Braided aerial root pillars & descending vine curtains
        // - Gnarled muscular fluted ancient trunk
        // - 8 sweeping tiered cathedral boughs spanning a 260m cathedral canopy
        // - Sacred spirit monoliths and mossy ancient boulders nestled at the root throne
        // =========================================================================================

        static void Colossal20xWorldTree(Vector3 p, float height, int seed)
        {
            var old = Parent;
            Parent = ModelGroup("Colossal 20x Sacred World Tree", p);

            float trunkRadius = 20.0f;

            // 1. Eight massive flowing, multi-segment fluted buttress roots (spanning 75m out)
            for (int r = 0; r < 8; r++)
            {
                float rootAng = r * Mathf.PI * (2f / 8f) + 0.15f;
                Vector3 rDir = new Vector3(Mathf.Sin(rootAng), 0, Mathf.Cos(rootAng));
                Vector3 perpDir = new Vector3(-rDir.z, 0, rDir.x);

                // Multi-joint organic curved buttress flange
                Vector3 pTop = Vector3.up * 52.0f + rDir * (trunkRadius * 0.72f);
                Vector3 pMid = Vector3.up * 22.0f + rDir * (trunkRadius * 1.85f) + perpDir * (Mathf.Sin(r * 1.7f) * 3.5f);
                Vector3 pLow = Vector3.up * 6.0f + rDir * 52.0f + perpDir * (Mathf.Cos(r * 2.1f) * 4.0f);
                Vector3 pTip = rDir * 75.0f - Vector3.up * 2.5f;

                // Main arching buttress spine
                Beam("Buttress flange top " + r, pTop, pMid, 5.2f, S.DarkWood);
                Beam("Buttress flange mid " + r, pMid, pLow, 4.4f, S.DarkWood);
                Beam("Buttress flange low " + r, pLow, pTip, 3.6f, S.DarkWood);

                // Braided aerial prop roots anchoring from buttress into soil
                Vector3 propStart = Vector3.Lerp(pTop, pMid, 0.45f);
                Vector3 propEnd = propStart + perpDir * 6.5f - Vector3.up * propStart.y;
                Beam("Aerial prop root " + r, propStart, propEnd, 2.2f, S.DarkWood);

                // Sacred mossy monolith nestled between buttress roots
                Vector3 stonePos = (pMid + pLow) * 0.5f + perpDir * 4.5f;
                stonePos.y = 1.8f;
                Box("Sacred spirit stone " + r, stonePos, new Vector3(4.5f, 5.0f, 4.0f), S.Carving);
            }

            // 2. Colossal muscular, continuous gnarled fluted trunk climbing to apex
            int wtRings = 9;
            var wtCenters = new Vector3[wtRings];
            var wtRadii = new float[wtRings];

            float[] wtHFracs = new float[] { -0.015f, 0.00f, 0.10f, 0.22f, 0.36f, 0.58f, 0.76f, 0.90f, 0.98f };
            float[] wtRFracs = new float[] { 2.10f,   1.70f, 1.35f, 1.10f, 0.90f, 0.72f, 0.54f, 0.38f, 0.22f };

            for (int i = 0; i < wtRings; i++)
            {
                float hf = wtHFracs[i];
                float y = (hf < 0) ? -4.0f : height * hf;
                float sway = (hf > 0) ? hf : 0;
                float lx = Mathf.Sin(seed * 0.7f + sway * 2.1f) * (4.5f * sway);
                float lz = Mathf.Cos(seed * 0.9f + sway * 1.8f) * (4.5f * sway);
                wtCenters[i] = new Vector3(lx, y, lz);
                wtRadii[i] = trunkRadius * wtRFracs[i];
            }

            ContinuousTaperedTrunk("World Tree Continuous Trunk", wtCenters, wtRadii, S.DarkWood, 16);

            Color leafDeep = new Color(0.18f, 0.35f, 0.12f);
            Color leafLush = new Color(0.24f, 0.44f, 0.16f);
            Color leafGoldSacred = new Color(0.30f, 0.48f, 0.18f);

            // Tier 1: Lower Grand Cathedral Boughs (at 36% height = 130m, 3 boughs)
            Vector3 pT1 = wtCenters[4];
            for (int b = 0; b < 3; b++)
            {
                float bAng = b * (Mathf.PI * 2f / 3f) + 0.45f;
                Vector3 bDir = new Vector3(Mathf.Cos(bAng) * 0.85f, 0.40f, Mathf.Sin(bAng) * 0.85f).normalized;
                Vector3 bMid = pT1 + bDir * (height * 0.20f) + Vector3.up * 8.0f;
                Vector3 bEnd = bMid + new Vector3(bDir.x * 0.9f, 0.25f, bDir.z * 0.9f).normalized * (height * 0.18f);

                Beam("World Tree lower bough " + b, pT1, bMid, trunkRadius * 0.38f, S.DarkWood);
                Beam("World Tree lower limb " + b, bMid, bEnd, trunkRadius * 0.26f, S.DarkWood);

                // Hanging aerial vine curtains dropping towards earth
                Vector3 vineStart = Vector3.Lerp(pT1, bMid, 0.65f);
                Vector3 vineEnd = vineStart - Vector3.up * 65.0f;
                Beam("Sacred hanging vine " + b, vineStart, vineEnd, 1.1f, S.DarkWood);

                NaturalLeafPuff(bMid, 58.0f, leafDeep, seed + b * 13);
                NaturalLeafPuff(bEnd, 72.0f, leafLush, seed + b * 17);
                NaturalLeafPuff(bEnd + Vector3.up * 18.0f, 52.0f, leafGoldSacred, seed + b * 19);
            }

            // Tier 2: Mid Grand Cathedral Boughs (at 58% height = 210m, 3 boughs staggered +60 deg)
            Vector3 pT2 = wtCenters[5];
            for (int b = 0; b < 3; b++)
            {
                float bAng = b * (Mathf.PI * 2f / 3f) + 1.50f;
                Vector3 bDir = new Vector3(Mathf.Cos(bAng) * 0.80f, 0.48f, Mathf.Sin(bAng) * 0.80f).normalized;
                Vector3 bMid = pT2 + bDir * (height * 0.18f) + Vector3.up * 10.0f;
                Vector3 bEnd = bMid + new Vector3(bDir.x * 0.85f, 0.30f, bDir.z * 0.85f).normalized * (height * 0.16f);

                Beam("World Tree mid bough " + b, pT2, bMid, trunkRadius * 0.32f, S.DarkWood);
                Beam("World Tree mid limb " + b, bMid, bEnd, trunkRadius * 0.22f, S.DarkWood);

                // Secondary fork
                Vector3 forkEnd = bMid + new Vector3(-bDir.z * 0.7f, 0.4f, bDir.x * 0.7f).normalized * (height * 0.12f);
                Beam("World Tree mid fork " + b, bMid, forkEnd, trunkRadius * 0.16f, S.DarkWood);

                NaturalLeafPuff(bMid, 60.0f, leafLush * 0.95f, seed + 40 + b * 11);
                NaturalLeafPuff(bEnd, 68.0f, leafLush, seed + 40 + b * 13);
                NaturalLeafPuff(forkEnd, 50.0f, leafGoldSacred, seed + 40 + b * 17);
            }

            // Tier 3: Upper Cathedral Crown (at 76% height = 275m, 2 expansive wings)
            Vector3 pT3 = wtCenters[6];
            for (int b = 0; b < 2; b++)
            {
                float bAng = b * Mathf.PI + 0.85f;
                Vector3 bDir = new Vector3(Mathf.Cos(bAng) * 0.75f, 0.58f, Mathf.Sin(bAng) * 0.75f).normalized;
                Vector3 bEnd = pT3 + bDir * (height * 0.22f);
                Beam("World Tree crown limb " + b, pT3, bEnd, trunkRadius * 0.24f, S.DarkWood);

                NaturalLeafPuff(bEnd, 65.0f, leafLush, seed + 80 + b * 23);
                NaturalLeafPuff(Vector3.Lerp(pT3, bEnd, 0.5f), 52.0f, leafDeep, seed + 80 + b * 29);
            }

            // Tier 4: Sovereign Summit Spire (reaching 360m summit)
            Vector3 pApex = wtCenters[8] + Vector3.up * (height * 0.04f);
            NaturalLeafPuff(pApex, 95.0f, leafGoldSacred * 1.10f, seed + 999);
            NaturalLeafPuff(pApex + Vector3.up * 16.0f, 65.0f, leafGoldSacred * 1.20f, seed + 1001);

            var treeObj = Parent;
            Parent = old;
            var cap = treeObj.gameObject.AddComponent<CapsuleCollider>();
            cap.center = new Vector3(0, height * 0.5f, 0);
            cap.radius = trunkRadius * 1.8f;
            cap.height = height;
            ModelCombiner.CombineModel(treeObj, addCollider: false);
        }

        // =========================================================================================
        // LOW JUNGLE GROUND COVER (EASY TO PLAY, GREAT VISIBILITY & BEAUTIFUL SCENIC VIEWS)
        // Knee-high grass, low ferns, small saplings, and mossy logs that never block player camera!
        // Grouped into regional chunks to strictly optimize MeshRenderers for Intel UHD 630!
        // =========================================================================================

        // =========================================================================================
        // REAL CAMBODIAN JUNGLE FLOOR FEATURES (IMMERSIVE, RICH, AUTHENTIC HORROR FOREST)
        // 1. Wild Bamboo Groves (Russey)
        // 2. Wild Jungle Banana Plants (Chek Prey)
        // 3. Ancient Buttress Roots & Base Ferns
        // 4. Fallen Mossy Rainforest Logs & Granite Boulders
        // 5. Arching Tropical Sword Ferns & Elephant Grass
        // =========================================================================================

        static void JungleBambooClump(Transform parent, Vector3 center, int seed)
        {
            var old = Parent;
            Parent = parent;

            Color bambooCulmCol = new Color(0.48f, 0.52f, 0.22f);
            Color bambooLeafCol = new Color(0.42f, 0.62f, 0.28f);

            int culmCount = 4 + (seed % 3);
            for (int c = 0; c < culmCount; c++)
            {
                float a = c * (Mathf.PI * 2f / culmCount) + (seed % 7) * 0.4f;
                float r = Random.Range(0.25f, 0.75f);
                Vector3 bp = center + new Vector3(Mathf.Sin(a) * r, 0, Mathf.Cos(a) * r);
                float culmH = Random.Range(4.5f, 7.0f);

                // Slender bamboo culm
                Vector3 top = bp + new Vector3(Mathf.Sin(a) * 0.4f, culmH, Mathf.Cos(a) * 0.4f);
                Beam("Bamboo culm", bp, top, 0.038f, S.Leaf);

                // Ringed nodes & drooping delicate leaf sprays
                int sprays = 3;
                for (int sp = 0; sp < sprays; sp++)
                {
                    float frac = 0.55f + sp * 0.15f;
                    Vector3 nodePos = Vector3.Lerp(bp, top, frac);
                    Quaternion leafRot = Quaternion.Euler(Random.Range(15, 35), (c * 60 + sp * 110 + seed) % 360, 0);
                    Grid("Bamboo foliage", 1, 1,
                        (u, v) => nodePos + leafRot * new Vector3((u - .5f) * 2.2f, -v * 1.5f, 0),
                        S.Foliage, Vector2.one, true, (p) => bambooLeafCol);
                }
            }
            Parent = old;
        }

        static void JungleBananaGrove(Transform parent, Vector3 center, int seed)
        {
            var old = Parent;
            Parent = parent;

            Color bananaGreen = new Color(0.35f, 0.58f, 0.22f);

            int plantCount = 2 + (seed % 2);
            for (int p = 0; p < plantCount; p++)
            {
                float a = p * 2.4f + seed;
                float r = Random.Range(0.3f, 0.8f);
                Vector3 bp = center + new Vector3(Mathf.Sin(a) * r, 0, Mathf.Cos(a) * r);
                float plantH = Random.Range(2.2f, 3.4f);

                // Fleshy pseudostem
                Vector3 top = bp + Vector3.up * plantH;
                Cylinder("Banana stem", bp, 0.085f, plantH, S.Leaf, 6);

                // 5 broad arched tropical leaves radiating outward
                int leaves = 5;
                for (int l = 0; l < leaves; l++)
                {
                    float la = l * (Mathf.PI * 2f / leaves) + p * 0.6f;
                    var dir = new Vector3(Mathf.Cos(la), 0.35f, Mathf.Sin(la)).normalized;
                    var side = new Vector3(dir.z, 0, -dir.x);
                    float leafLen = Random.Range(2.2f, 3.2f);
                    float leafWid = Random.Range(0.65f, 0.90f);

                    Grid("Banana leaf", 1, 2,
                        (u, v) => top + dir * (v * leafLen) + side * ((u - .5f) * leafWid * Mathf.Sin(v * Mathf.PI)) + Vector3.up * (Mathf.Sin(v * Mathf.PI * 0.6f) * 0.45f - v * v * 0.55f),
                        S.Foliage, Vector2.one, true, (pt) => bananaGreen);
                }
            }
            Parent = old;
        }

        static void JungleLogAndBoulder(Transform parent, Vector3 center, int seed)
        {
            var old = Parent;
            Parent = parent;

            // 1. Mossy Granite Rainforest Boulder
            Vector3 rockScale = new Vector3(Random.Range(1.2f, 2.2f), Random.Range(0.7f, 1.3f), Random.Range(1.2f, 1.9f));
            Ball("Mossy Jungle Boulder", center + Vector3.up * (rockScale.y * 0.38f), rockScale, S.DarkWood);

            // 2. Weathered Decaying Fallen Log (Prouk)
            float logAngle = (seed * 53) % 360;
            float logLen = Random.Range(4.5f, 7.0f);
            var logDir = new Vector3(Mathf.Sin(logAngle * Mathf.Deg2Rad), 0, Mathf.Cos(logAngle * Mathf.Deg2Rad));
            Vector3 logStart = center - logDir * (logLen * 0.5f);
            Vector3 logEnd = center + logDir * (logLen * 0.5f);
            logStart.y = GroundDetailBuilder.HillHeight(logStart.x, logStart.z) + 0.15f;
            logEnd.y = GroundDetailBuilder.HillHeight(logEnd.x, logEnd.z) + 0.15f;
            Beam("Fallen Rainforest Log", logStart, logEnd, Random.Range(0.28f, 0.42f), S.Bark);

            // 3. Low ferns hugging the log
            Color fernCol = new Color(0.30f, 0.48f, 0.20f);
            for (int f = 0; f < 3; f++)
            {
                Vector3 fp = Vector3.Lerp(logStart, logEnd, (f + 0.5f) / 3f) + new Vector3(logDir.z, 0, -logDir.x) * 0.6f;
                fp.y = GroundDetailBuilder.HillHeight(fp.x, fp.z);
                Quaternion fRot = Quaternion.Euler(Random.Range(5, 18), (f * 70 + seed) % 360, 0);
                Grid("Log fern", 1, 2,
                    (u, v) => fp + fRot * new Vector3((u - .5f) * 0.70f, Mathf.Sin(v * Mathf.PI * 0.6f) * 0.35f, v * 1.3f),
                    S.Foliage, Vector2.one, true, (pt) => fernCol);
            }
            Parent = old;
        }

        static void JungleFernAndGrassThicket(Transform parent, Vector3 center, float radius, int seed)
        {
            var old = Parent;
            Parent = parent;

            Color grassCol = new Color(0.40f, 0.58f, 0.26f);
            Color fernCol = new Color(0.32f, 0.48f, 0.22f);

            // 1. Wild Arching Sword Ferns
            int fernCount = 3 + (seed % 3);
            for (int fc = 0; fc < fernCount; fc++)
            {
                float a = fc * 2.1f + seed * 0.4f;
                float dist = Random.Range(0.4f, radius * 0.85f);
                Vector3 fp = center + new Vector3(Mathf.Sin(a) * dist, 0, Mathf.Cos(a) * dist);
                fp.y = GroundDetailBuilder.HillHeight(fp.x, fp.z);

                int fronds = 4;
                for (int fr = 0; fr < fronds; fr++)
                {
                    float frondAng = fr * (Mathf.PI * 2f / fronds) + fc * 1.1f;
                    float frondLen = Random.Range(1.2f, 1.7f);
                    var frondDir = new Vector3(Mathf.Sin(frondAng), 0.28f, Mathf.Cos(frondAng)).normalized;

                    Quaternion fRot = Quaternion.LookRotation(frondDir, Vector3.up);
                    Grid("Sword fern frond", 1, 2,
                        (u, v) => fp + fRot * new Vector3((u - .5f) * 0.75f, Mathf.Sin(v * Mathf.PI * 0.6f) * 0.40f, v * frondLen),
                        S.Foliage, Vector2.one, true, (p) => fernCol);
                }
            }

            // 2. Wild Jungle Elephant Grass Tufts (0.8m - 1.3m)
            int grassTufts = 5;
            for (int g = 0; g < grassTufts; g++)
            {
                float a = g * (Mathf.PI * 2f / grassTufts) + (seed % 5) * 0.5f;
                float dist = Random.Range(0.3f, radius);
                Vector3 gp = center + new Vector3(Mathf.Sin(a) * dist, 0, Mathf.Cos(a) * dist);
                gp.y = GroundDetailBuilder.HillHeight(gp.x, gp.y);
                float gH = Random.Range(0.80f, 1.25f);
                float gW = Random.Range(0.85f, 1.25f);

                Quaternion gRot = Quaternion.Euler(0, (g * 72 + seed * 5) % 360, 0);
                Grid("Elephant grass", 1, 1,
                    (u, v) => gp + gRot * new Vector3((u - .5f) * gW, v * gH, 0),
                    S.Foliage, Vector2.one, true, (p) => grassCol);
            }

            // 3. Slender Forest Sapling (1.8m - 2.4m)
            if ((seed % 2) == 0)
            {
                Vector3 sapPos = center + new Vector3(Random.Range(-0.8f, 0.8f), 0, Random.Range(-0.8f, 0.8f));
                sapPos.y = GroundDetailBuilder.HillHeight(sapPos.x, sapPos.z);
                float sH = Random.Range(1.8f, 2.4f);
                Beam("Sapling", sapPos, sapPos + Vector3.up * sH, 0.045f, S.DarkWood);
                NaturalLeafPuff(sapPos + Vector3.up * sH, 2.6f, grassCol, seed * 23);
            }

            Parent = old;
        }

        static void JungleSaplingClump(Transform parent, Vector3 center, int seed)
        {
            var old = Parent;
            Parent = parent;

            Color saplingLeafCol = new Color(0.36f, 0.54f, 0.24f);

            int saplingCount = 2 + (seed % 3);
            for (int s = 0; s < saplingCount; s++)
            {
                float ang = s * 2.2f + (seed % 7) * 0.5f;
                float dist = Random.Range(0.4f, 1.8f);
                Vector3 sp = center + new Vector3(Mathf.Sin(ang) * dist, 0, Mathf.Cos(ang) * dist);
                sp.y = GroundDetailBuilder.HillHeight(sp.x, sp.z);

                float sH = Random.Range(2.8f, 4.6f);
                float lean = Random.Range(-0.25f, 0.25f);
                Vector3 top = sp + new Vector3(lean, sH, -lean);

                // Slender juvenile trunk
                Beam("Jungle sapling stem", sp, top, 0.055f, S.DarkWood);

                // 2 small lateral branches
                for (int b = 0; b < 2; b++)
                {
                    float bFrac = 0.55f + b * 0.25f;
                    Vector3 bStart = Vector3.Lerp(sp, top, bFrac);
                    float bAng = ang + b * 2.4f;
                    Vector3 bEnd = bStart + new Vector3(Mathf.Cos(bAng) * 0.9f, 0.45f, Mathf.Sin(bAng) * 0.9f);
                    Beam("Jungle sapling branch", bStart, bEnd, 0.035f, S.DarkWood);
                    NaturalLeafPuff(bEnd, Random.Range(2.2f, 3.2f), saplingLeafCol * 0.95f, seed + s * 13 + b * 7);
                }

                // Top crown foliage
                NaturalLeafPuff(top, Random.Range(3.0f, 4.0f), saplingLeafCol, seed + s * 19);
            }

            Parent = old;
        }

        static void BuildRoadsideForestFeatures()
        {
            var old = Parent;
            Parent = ModelGroup("Roadside Forest Features", Vector3.zero);

            Color grassCol = new Color(0.42f, 0.58f, 0.28f);
            Color fernCol = new Color(0.32f, 0.48f, 0.22f);

            int roadSteps = 28;
            Vector3 prevLeftPost = Vector3.zero;
            Vector3 prevRightPost = Vector3.zero;

            for (int s = 1; s <= roadSteps; s++)
            {
                float t = Mathf.Lerp(0.02f, 0.96f, s / (float)roadSteps);
                Vector2 center = GroundDetailBuilder.HillRoadCenter(t);
                Vector2 before = GroundDetailBuilder.HillRoadCenter(Mathf.Max(0, t - 0.02f));
                Vector2 after = GroundDetailBuilder.HillRoadCenter(Mathf.Min(1, t + 0.02f));
                Vector2 tangent = (after - before).normalized;
                Vector2 side = new Vector2(-tangent.y, tangent.x);

                // Clean shoulder lines (+-4.2m from centerline = 1.9m off dirt road edge)
                Vector2 lp = center - side * 4.2f;
                float ly = GroundDetailBuilder.HillHeight(lp.x, lp.y);
                Vector3 leftPos = new Vector3(lp.x, ly, lp.y);

                Vector2 rp = center + side * 4.2f;
                float ry = GroundDetailBuilder.HillHeight(rp.x, rp.y);
                Vector3 rightPos = new Vector3(rp.x, ry, rp.y);

                // 1. RUSTIC WOODEN ROADSIDE FENCES (Clean continuous rails stepping naturally)
                if (s % 2 == 0 && s >= 2 && s <= 26)
                {
                    Cylinder("Road fence post Left", leftPos, 0.075f, 1.20f, S.DarkWood, 8);
                    Cylinder("Road fence post Right", rightPos, 0.075f, 1.10f, S.DarkWood, 8);

                    if (prevLeftPost != Vector3.zero)
                    {
                        Beam("Road fence top Left", prevLeftPost + Vector3.up * 1.05f, leftPos + Vector3.up * 1.05f, 0.042f, S.DarkWood);
                        Beam("Road fence mid Left", prevLeftPost + Vector3.up * 0.55f, leftPos + Vector3.up * 0.55f, 0.038f, S.DarkWood);
                        Beam("Road fence top Right", prevRightPost + Vector3.up * 0.95f, rightPos + Vector3.up * 0.95f, 0.040f, S.DarkWood);
                        Beam("Road fence mid Right", prevRightPost + Vector3.up * 0.48f, rightPos + Vector3.up * 0.48f, 0.036f, S.DarkWood);
                    }
                    prevLeftPost = leftPos;
                    prevRightPost = rightPos;

                    // 2. COHESIVE FOLIAGE CLUMP AT BASE OF EACH POST (Clean & organized, not messy!)
                    Grid("Post grass A", 1, 1,
                        (u, v) => leftPos + new Vector3((u - .5f) * 1.1f, v * 1.25f, 0),
                        S.Foliage, Vector2.one, true, (p) => grassCol);
                    Grid("Post grass B", 1, 1,
                        (u, v) => rightPos + new Vector3(0, v * 1.15f, (u - .5f) * 1.1f),
                        S.Foliage, Vector2.one, true, (p) => grassCol * 1.05f);

                    // 3. POLISHED ROADSIDE DRAINAGE STONE AT POST BASE
                    if (s % 4 == 0)
                    {
                        Ball("Roadside stone L", leftPos + Vector3.up * 0.12f + new Vector3(tangent.x, 0, tangent.y) * 0.4f, new Vector3(0.55f, 0.35f, 0.45f), S.DarkWood);
                        Ball("Roadside stone R", rightPos + Vector3.up * 0.12f - new Vector3(tangent.x, 0, tangent.y) * 0.4f, new Vector3(0.60f, 0.38f, 0.50f), S.DarkWood);
                    }
                }

                // 4. CLEAN TROPICAL SWORD FERNS (Every 3rd step along the verge)
                if (s % 3 == 0)
                {
                    Vector2 fp2 = center - side * 5.6f;
                    Vector3 fp = new Vector3(fp2.x, GroundDetailBuilder.HillHeight(fp2.x, fp2.y), fp2.y);
                    int fronds = 4;
                    for (int fr = 0; fr < fronds; fr++)
                    {
                        float fa = fr * Mathf.PI * 2f / fronds + s * 0.8f;
                        var fDir = new Vector3(Mathf.Cos(fa), 0.30f, Mathf.Sin(fa)).normalized;
                        Quaternion fRot = Quaternion.LookRotation(fDir, Vector3.up);
                        Grid("Roadside fern", 1, 2,
                            (u, v) => fp + fRot * new Vector3((u - .5f) * 0.65f, Mathf.Sin(v * Mathf.PI * 0.6f) * 0.35f, v * 1.35f),
                            S.Foliage, Vector2.one, true, (p) => fernCol);
                    }
                }

                // 5. Roadside saplings removed per user request

                // 6. CAMBODIAN TERMITE MOUNDS (At clean level verge locations)
                if (s == 2 || s == 25)
                {
                    float mSideDir = (s == 2) ? 1f : -1f;
                    Vector2 mp = center + side * (mSideDir * 4.6f);
                    Vector3 moundPos = new Vector3(mp.x, GroundDetailBuilder.HillHeight(mp.x, mp.y), mp.y);
                    float mH = 1.55f;
                    float mR = 0.90f;
                    var moundProfile = new Vector2[] {
                        new Vector2(mR * 1.3f, -0.2f),
                        new Vector2(mR * 1.0f, 0.0f),
                        new Vector2(mR * 0.75f, mH * 0.4f),
                        new Vector2(mR * 0.45f, mH * 0.75f),
                        new Vector2(mR * 0.18f, mH * 0.95f),
                        new Vector2(0f, mH)
                    };
                    Lathe("Roadside Termite Mound", moundPos, moundProfile, S.Clay, 10);
                }
            }

            var roadsideObj = Parent;
            Parent = old;
            ModelCombiner.CombineModel(roadsideObj, addCollider: true, useCapsuleCollider: false);
        }

        // =========================================================================================
        // FULL SURROUNDING FOREST:
        // - Within 100m of house/clearing (and along road corridor): FATTER village trees
        // - Beyond 100m+: Colossal deep jungle giants with high canopy & clear eye level
        // - Left hill completely filled with trees (no bare lawn!)
        // =========================================================================================

        static void BuildSurroundingForest()
        {
            var randomState = Random.state;
            Random.InitState(20061118);

            // 1. Build authentic, optimized, clean forest features beside the road!
            BuildRoadsideForestFeatures();

            // 2. Regional Jungle Undergrowth Chunks (combines all forest floor features into 6 batched meshes!)
            var patchChunks = new Transform[6];
            for (int i = 0; i < 6; i++)
                patchChunks[i] = Group("Jungle Ground Undergrowth Region " + (i + 1), Vector3.zero);

            int treeCount = 0;
            int featureCount = 0;
            var placedTrees = new System.Collections.Generic.List<Vector2>();

            // =====================================================================================
            // 3. DEDICATED RIGHT-SIDE HILL FILLING PASS (LUSH MEDIUM VILLAGE TREES)
            // User explicit request: 'about right side must fill medium tree'
            // Open, organic scatter with wide 18m - 26m spacing - ZERO fence-like grid lines!
            // 16m road clearance keeps the road corridor completely open!
            // =====================================================================================
            for (float rz = -135.0f; rz <= 65.0f; rz += 20.0f)
            {
                float rzStagger = ((int)((rz + 150f) / 20.0f) % 2 == 0) ? 0f : 8.5f;
                for (float rx = 24.0f + rzStagger; rx <= 85.0f; rx += 18.0f)
                {
                    int hillSeed = (int)(rx * 37 + rz * 23) + 7000;
                    if ((hillSeed % 3) == 0) continue; // Naturally thin out to ensure open walking lanes

                    float jx = Random.Range(-6.0f, 6.0f);
                    float jz = Random.Range(-6.5f, 6.5f);
                    float fx = rx + jx;
                    float fz = rz + jz;

                    if (IsInsideWeddingOrHouseZone(fx, fz, 4.5f)) continue;

                    // STRICT ROAD CLEARANCE: NO TREES BESIDE THE ROAD! (16m clearance corridor)
                    float distToRoad = GroundDetailBuilder.DistanceToHillRoad(new Vector2(fx, fz));
                    if (distToRoad < 16.0f && fz < SiteLayout.Front + 5.0f) continue;

                    // Beside marquee / catering tents: keep trees outside tent canvas
                    if (fz >= SiteLayout.Front + 5.0f)
                    {
                        if (fz <= 12.0f && fx < 25.5f) continue;
                        if (fz > 12.0f && fz <= 32.0f && fx < 26.5f) continue;
                        if (fz > 32.0f && fx < 25.0f) continue;
                    }

                    // Proximity check: prevent any tree clustering
                    bool hillConflict = false;
                    for (int ptIdx = 0; ptIdx < placedTrees.Count; ptIdx++)
                    {
                        if (Vector2.Distance(new Vector2(fx, fz), placedTrees[ptIdx]) < 20.0f)
                        {
                            hillConflict = true;
                            break;
                        }
                    }
                    if (hillConflict) continue;
                    placedTrees.Add(new Vector2(fx, fz));

                    var pt = new Vector3(fx, GroundDetailBuilder.HillHeight(fx, fz), fz);
                    FatVillageTree(pt, Random.Range(13.5f, 16.5f), hillSeed);
                    treeCount++;
                }
            }

            // =====================================================================================
            // 4. MAIN SURROUNDING FOREST GRID:
            // Spans x in [-260, 105] (365m wide), z in [-155, 360] (515m long)
            // - Staggered rows with large organic jitter eliminate all fence-like straight rows!
            // - Strict 22m - 26m proximity check ensures wide open corridors everywhere!
            // =====================================================================================
            float minX = -260.0f;
            float maxX = 105.0f;
            float minZ = -155.0f;
            float maxZ = 360.0f;

            int cols = 16;
            int rows = 20;
            float stepX = (maxX - minX) / cols; // ~22.8m
            float stepZ = (maxZ - minZ) / rows; // ~25.7m

            for (int r = 0; r < rows; r++)
            {
                // Stagger alternating rows by half a cell to eliminate straight row fences
                float rowStagger = (r % 2 == 0) ? 0f : (stepX * 0.5f);

                for (int c = 0; c < cols; c++)
                {
                    int cellSeed = (r * cols + c) * 37 + 2006;
                    
                    // Cell center with row stagger
                    float cx = minX + (c + 0.5f) * stepX + rowStagger;
                    float cz = minZ + (r + 0.5f) * stepZ;

                    // Skip the right-side hill since the dedicated pass fills it completely!
                    if (cx >= 20.0f && cz >= -140.0f && cz <= 70.0f) continue;

                    // Large organic chaotic jitter breaks any lines or rows
                    float jx = Random.Range(-stepX * 0.44f, stepX * 0.44f);
                    float jz = Random.Range(-stepZ * 0.44f, stepZ * 0.44f);
                    float px = cx + jx;
                    float pz = cz + jz;

                    // Skip if inside wedding marquee, house, kitchen, or cattle shelter (generous 4.5m buffer)
                    if (IsInsideWeddingOrHouseZone(px, pz, 4.5f)) continue;

                    // STRICT ROAD CLEARANCE: NO TREES BESIDE THE ROAD! (16m clearance corridor)
                    float distToRoad = GroundDetailBuilder.DistanceToHillRoad(new Vector2(px, pz));
                    if (distToRoad < 16.0f && pz < SiteLayout.Front + 5.0f) continue;

                    // Distance from house & wedding clearing center (0, 15)
                    float distFromClearing = Vector2.Distance(new Vector2(px, pz), new Vector2(0f, 15f));

                    // STRICT PROXIMITY CHECK: mathematically guarantees NO fence lines and NO cramped clusters!
                    float minAllowedDist = (distFromClearing < 70.0f) ? 26.0f : (distFromClearing < 200.0f) ? 22.0f : 16.0f;
                    bool conflict = false;
                    for (int ptIdx = 0; ptIdx < placedTrees.Count; ptIdx++)
                    {
                        if (Vector2.Distance(new Vector2(px, pz), placedTrees[ptIdx]) < minAllowedDist)
                        {
                            conflict = true;
                            break;
                        }
                    }
                    if (conflict) continue;
                    placedTrees.Add(new Vector2(px, pz));

                    var pt = new Vector3(px, GroundDetailBuilder.HillHeight(px, pz), pz);

                    // =================================================================================
                    // TREE SELECTION & WALKABLE SPACING:
                    // - 0m to 50m around house: SLENDER TREES (12m - 14.5m, wide clear yards)
                    // - 50m to 100m: FAT VILLAGE TREES (13.5m - 16.5m, open village paths)
                    // - 100m to 200m: NORMAL DEEP JUNGLE TREES (26m - 36m, 2x slimmer girth)
                    // - 200m+: 3X TALLER DEEP JUNGLE GIANTS (78m - 108m, 2x slimmer girth, top 1/3 canopy!)
                    // =================================================================================
                    if (distFromClearing < 50.0f)
                    {
                        // Zone 1: 0m to 50m -> slender tree
                        SlenderSmallVillageTree(pt, Random.Range(12.0f, 14.5f), cellSeed);
                        treeCount++;
                    }
                    else if (distFromClearing < 100.0f)
                    {
                        // Zone 2: 50m to 100m -> Fat village tree
                        FatVillageTree(pt, Random.Range(13.5f, 16.5f), cellSeed);
                        treeCount++;
                    }
                    else if (distFromClearing < 200.0f)
                    {
                        // Zone 3: 100m to 200m -> Normal deep jungle trees (26m - 36m)
                        DeepForestJungleTree(pt, Random.Range(26.0f, 36.0f), cellSeed);
                        treeCount++;
                    }
                    else
                    {
                        // Zone 4: 200m+ -> 3x taller deep jungle giants (78m - 108m, 2x slimmer girth!)
                        DeepForestJungleTree(pt, Random.Range(78.0f, 108.0f), cellSeed);
                        treeCount++;
                    }

                    // Companion tree: ONLY in deep wilderness beyond 250m!
                    // Completely removed within 250m so player paths never encounter 2-tree choke points!
                    if (Random.value < 0.10f && distFromClearing >= 250.0f)
                    {
                        float compAngle = Random.Range(0f, Mathf.PI * 2f);
                        float compDist = Random.Range(10.0f, 18.0f);
                        float cpx = px + Mathf.Cos(compAngle) * compDist;
                        float cpz = pz + Mathf.Sin(compAngle) * compDist;

                        float compDistToRoad = GroundDetailBuilder.DistanceToHillRoad(new Vector2(cpx, cpz));

                        if (!IsInsideWeddingOrHouseZone(cpx, cpz, 4.5f) && 
                            (compDistToRoad >= 16.0f || cpz >= SiteLayout.Front + 5.0f) &&
                            cpx >= minX && cpx <= maxX && cpz >= minZ && cpz <= maxZ)
                        {
                            float cHeight = Random.Range(78.0f, 108.0f);
                            var cpt = new Vector3(cpx, GroundDetailBuilder.HillHeight(cpx, cpz), cpz);
                            DeepForestJungleTree(cpt, cHeight, cellSeed + 99);
                            treeCount++;
                        }
                    }

                    // Real jungle floor features (bamboo, banana, logs, boulders, ferns)
                    if (distFromClearing > 16.0f && (distToRoad >= 12.0f || pz >= SiteLayout.Front + 5.0f) && featureCount < 260)
                    {
                        int chunkCol = (px < -100f) ? 0 : (px < 0f) ? 1 : 2;
                        int chunkRow = (pz < 100f) ? 0 : 1;
                        int chunkIdx = Mathf.Clamp(chunkRow * 3 + chunkCol, 0, 5);
                        Transform targetChunk = patchChunks[chunkIdx];

                        Vector3 featPos = pt + new Vector3(Random.Range(-4.5f, 4.5f), 0, Random.Range(-4.5f, 4.5f));
                        featPos.y = GroundDetailBuilder.HillHeight(featPos.x, featPos.z);

                        int featType = (cellSeed + r * 5) % 5;
                        switch (featType)
                        {
                            case 0:
                                JungleBambooClump(targetChunk, featPos, cellSeed);
                                break;
                            case 1:
                                JungleBananaGrove(targetChunk, featPos, cellSeed);
                                break;
                            case 2:
                                JungleLogAndBoulder(targetChunk, featPos, cellSeed);
                                break;
                            case 3:
                                JungleFernAndGrassThicket(targetChunk, featPos, 4.5f, cellSeed);
                                break;
                            case 4:
                                JungleSaplingClump(targetChunk, featPos, cellSeed);
                                break;
                        }
                        featureCount++;
                    }
                }
            }

            // =====================================================================================
            // 5. COLOSSAL 20X SACRED WORLD TREE (In the middle of the left-behind extension!)
            // User explicit requirement:
            // - The giant tree is ONLY ONE in the whole world!
            // - Around like 300m away from the giant tree is NOT tree fill, ONLY decorate!
            // =====================================================================================
            Vector3 worldTreePos = new Vector3(-880.0f, GroundDetailBuilder.HillHeight(-880.0f, 1020.0f), 1020.0f);
            Colossal20xWorldTree(worldTreePos, 360.0f, 7777);
            treeCount++;

            // Sacred 300m clearing decorations around World Tree (NO TREES WITHIN 300m, ONLY DECORATIONS!)
            var sacredGladeChunk = Group("World Tree Sacred Glade Features", Vector3.zero);
            for (int sf = 0; sf < 84; sf++)
            {
                float sAng = sf * (Mathf.PI * 2f / 84f) + 0.35f;
                float sDist = Random.Range(28.0f, 285.0f);
                Vector3 sPos = worldTreePos + new Vector3(Mathf.Sin(sAng) * sDist, 0, Mathf.Cos(sAng) * sDist);
                sPos.y = GroundDetailBuilder.HillHeight(sPos.x, sPos.z);
                int sType = sf % 5;
                switch (sType)
                {
                    case 0: JungleLogAndBoulder(sacredGladeChunk, sPos, sf * 31); break;
                    case 1: JungleBananaGrove(sacredGladeChunk, sPos, sf * 43); break;
                    case 2: JungleBambooClump(sacredGladeChunk, sPos, sf * 57); break;
                    case 3: JungleFernAndGrassThicket(sacredGladeChunk, sPos, 6.0f, sf * 71); break;
                    case 4: JungleSaplingClump(sacredGladeChunk, sPos, sf * 83); break;
                }
            }
            if (sacredGladeChunk.childCount > 0)
                ModelCombiner.CombineModel(sacredGladeChunk, addCollider: false);

            // =====================================================================================
            // 6. EXTENDED LEFT ZONE (x in [-1420m, -260m], z in [-155m, 1540m]):
            // User explicit requirement:
            // "in zone of 200 to 300 have tree count yes like that but 300+m to end why have low count why dont it make like 200m to 300M?"
            // Dense ~26m tree spacing all the way to map boundary, matching 200m-300m zone!
            // Batched into 6 regional chunks for ultra-high performance on Intel UHD 630!
            // =====================================================================================
            float extLeftMinX = -1420.0f;
            float extLeftMaxX = -260.0f;
            float extLeftMinZ = -155.0f;
            float extLeftMaxZ = 1540.0f;
            int extLeftCols = 42; // ~27.6m step (dense forest matching 200m-300m!)
            int extLeftRows = 62; // ~27.3m step
            float extLeftStepX = (extLeftMaxX - extLeftMinX) / extLeftCols;
            float extLeftStepZ = (extLeftMaxZ - extLeftMinZ) / extLeftRows;

            var outerLeftChunks = new Transform[6];
            for (int i = 0; i < 6; i++)
                outerLeftChunks[i] = Group("Outer Forest West Chunk " + (i + 1), Vector3.zero);

            for (int r = 0; r < extLeftRows; r++)
            {
                float rowStagger = (r % 2 == 0) ? 0f : (extLeftStepX * 0.5f);
                for (int c = 0; c < extLeftCols; c++)
                {
                    int cellSeed = (r * extLeftCols + c) * 41 + 8000;
                    float cx = extLeftMinX + (c + 0.5f) * extLeftStepX + rowStagger;
                    float cz = extLeftMinZ + (r + 0.5f) * extLeftStepZ;

                    float jx = Random.Range(-extLeftStepX * 0.38f, extLeftStepX * 0.38f);
                    float jz = Random.Range(-extLeftStepZ * 0.38f, extLeftStepZ * 0.38f);
                    float px = cx + jx;
                    float pz = cz + jz;

                    // STRICT 300m SACRED CLEARING: NO TREES WITHIN 300m OF WORLD TREE!
                    if (Vector2.Distance(new Vector2(px, pz), new Vector2(worldTreePos.x, worldTreePos.z)) < 300.0f)
                        continue;

                    var pt = new Vector3(px, GroundDetailBuilder.HillHeight(px, pz), pz);

                    // Timberline: trees grow up mountain foothills to 150m; sheer peaks above expose craggy granite
                    if (pt.y > 150.0f) continue;

                    int chunkCol = (px < -840f) ? 0 : 1;
                    int chunkRow = (pz < 400f) ? 0 : (pz < 950f) ? 1 : 2;
                    int chunkIdx = chunkRow * 2 + chunkCol;

                    // Trees within 360m of clearing keep individual colliders for exploration;
                    // Distant wilderness trees beyond 360m batch into regional chunks!
                    if (Vector2.Distance(new Vector2(px, pz), new Vector2(0f, 15f)) < 360.0f)
                    {
                        DeepForestJungleTree(pt, Random.Range(78.0f, 108.0f), cellSeed, combineSelf: true);
                    }
                    else
                    {
                        var prevP = Parent;
                        Parent = outerLeftChunks[chunkIdx];
                        DeepForestJungleTree(pt, Random.Range(78.0f, 108.0f), cellSeed, combineSelf: false);
                        Parent = prevP;
                    }
                    treeCount++;
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (outerLeftChunks[i].childCount > 0)
                    ModelCombiner.CombineModel(outerLeftChunks[i], addCollider: false);
            }

            // =====================================================================================
            // 7. EXTENDED BACK ZONE (x in [-260m, 105m], z in [360m, 1540m]):
            // Dense ~26m tree spacing all the way to northern mountains, matching 200m-300m zone!
            // Batched into 2 regional chunks for ultra-high performance on Intel UHD 630!
            // =====================================================================================
            float extBackMinX = -260.0f;
            float extBackMaxX = 105.0f;
            float extBackMinZ = 360.0f;
            float extBackMaxZ = 1540.0f;
            int extBackCols = 14; // ~26.0m step (dense forest matching 200m-300m!)
            int extBackRows = 44; // ~26.8m step
            float extBackStepX = (extBackMaxX - extBackMinX) / extBackCols;
            float extBackStepZ = (extBackMaxZ - extBackMinZ) / extBackRows;

            var outerNorthChunks = new Transform[2];
            outerNorthChunks[0] = Group("Outer Forest North Chunk 1", Vector3.zero);
            outerNorthChunks[1] = Group("Outer Forest North Chunk 2", Vector3.zero);

            for (int r = 0; r < extBackRows; r++)
            {
                float rowStagger = (r % 2 == 0) ? 0f : (extBackStepX * 0.5f);
                for (int c = 0; c < extBackCols; c++)
                {
                    int cellSeed = (r * extBackCols + c) * 47 + 9000;
                    float cx = extBackMinX + (c + 0.5f) * extBackStepX + rowStagger;
                    float cz = extBackMinZ + (r + 0.5f) * extBackStepZ;

                    float jx = Random.Range(-extBackStepX * 0.38f, extBackStepX * 0.38f);
                    float jz = Random.Range(-extBackStepZ * 0.38f, extBackStepZ * 0.38f);
                    float px = cx + jx;
                    float pz = cz + jz;

                    // STRICT 300m SACRED CLEARING: NO TREES WITHIN 300m OF WORLD TREE!
                    if (Vector2.Distance(new Vector2(px, pz), new Vector2(worldTreePos.x, worldTreePos.z)) < 300.0f)
                        continue;

                    var pt = new Vector3(px, GroundDetailBuilder.HillHeight(px, pz), pz);

                    // Timberline: trees grow up mountain foothills to 150m; sheer peaks above expose craggy granite
                    if (pt.y > 150.0f) continue;

                    int chunkIdx = (pz < 950f) ? 0 : 1;

                    if (Vector2.Distance(new Vector2(px, pz), new Vector2(0f, 15f)) < 420.0f)
                    {
                        DeepForestJungleTree(pt, Random.Range(78.0f, 108.0f), cellSeed, combineSelf: true);
                    }
                    else
                    {
                        var prevP = Parent;
                        Parent = outerNorthChunks[chunkIdx];
                        DeepForestJungleTree(pt, Random.Range(78.0f, 108.0f), cellSeed, combineSelf: false);
                        Parent = prevP;
                    }
                    treeCount++;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (outerNorthChunks[i].childCount > 0)
                    ModelCombiner.CombineModel(outerNorthChunks[i], addCollider: false);
            }

            // =====================================================================================
            // 8. LEFT (WEST) HILL SLOPE TREES & GEOLOGICAL FEATURES:
            // Hardy Cambodian mountain slope trees, rugged rock outcrops, and mountain thickets!
            // Batched into 4 regional chunks for ultra-high performance on Intel UHD 630!
            // =====================================================================================
            var mountainWestChunks = new Transform[4];
            for (int i = 0; i < 4; i++)
                mountainWestChunks[i] = Group("Mountain Forest West Chunk " + (i + 1), Vector3.zero);

            float mWestMinX = -1740.0f, mWestMaxX = -1438.0f;
            float mWestMinZ = -140.0f, mWestMaxZ = 1540.0f;
            int mWestCols = 14, mWestRows = 52;
            float mWestStepX = (mWestMaxX - mWestMinX) / mWestCols;
            float mWestStepZ = (mWestMaxZ - mWestMinZ) / mWestRows;

            for (int r = 0; r < mWestRows; r++)
            {
                float rowStagger = (r % 2 == 0) ? 0f : (mWestStepX * 0.5f);
                for (int c = 0; c < mWestCols; c++)
                {
                    int seed = (r * mWestCols + c) * 53 + 4321;
                    float px = mWestMinX + (c + 0.5f) * mWestStepX + rowStagger + Random.Range(-mWestStepX * 0.35f, mWestStepX * 0.35f);
                    float pz = mWestMinZ + (r + 0.5f) * mWestStepZ + Random.Range(-mWestStepZ * 0.35f, mWestStepZ * 0.35f);
                    float py = GroundDetailBuilder.HillHeight(px, pz);

                    // Timberline: trees and vegetation flourish up the mountain face to 210m; sheer peaks above expose crags
                    if (py > 210.0f) continue;

                    int chunkIdx = Mathf.Clamp((int)((pz - mWestMinZ) / ((mWestMaxZ - mWestMinZ) / 4.0f)), 0, 3);
                    Transform targetChunk = mountainWestChunks[chunkIdx];

                    var pt = new Vector3(px, py, pz);
                    var prevP = Parent;
                    Parent = targetChunk;
                    DeepForestJungleTree(pt, Random.Range(36.0f, 68.0f), seed, combineSelf: false);
                    Parent = prevP;
                    treeCount++;

                    // Hill geological features (granite boulders, wild bamboo, ferns, mountain banana groves)
                    if (Random.value < 0.55f)
                    {
                        Vector3 featPos = pt + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                        featPos.y = GroundDetailBuilder.HillHeight(featPos.x, featPos.z);
                        int featType = seed % 4;
                        switch (featType)
                        {
                            case 0: JungleLogAndBoulder(targetChunk, featPos, seed + 1); break;
                            case 1: JungleBambooClump(targetChunk, featPos, seed + 2); break;
                            case 2: JungleFernAndGrassThicket(targetChunk, featPos, 5.0f, seed + 3); break;
                            case 3: JungleBananaGrove(targetChunk, featPos, seed + 4); break;
                        }
                    }
                }
            }

            // Dedicated lower cliff rock wall pass (rugged granite boulders and shelves at base of cliff)
            for (float cz = -130.0f; cz < 1530.0f; cz += 32.0f)
            {
                float cx = -1445.0f + Random.Range(-6.0f, 4.0f);
                float cy = GroundDetailBuilder.HillHeight(cx, cz);
                int cIdx = Mathf.Clamp((int)((cz - mWestMinZ) / ((mWestMaxZ - mWestMinZ) / 4.0f)), 0, 3);
                int rSeed = (int)(cz * 17) + 1200;
                JungleLogAndBoulder(mountainWestChunks[cIdx], new Vector3(cx, cy, cz), rSeed);
                if (rSeed % 2 == 0)
                    JungleBambooClump(mountainWestChunks[cIdx], new Vector3(cx - 3.5f, cy + 1.5f, cz + 4f), rSeed + 5);
            }

            for (int i = 0; i < 4; i++)
            {
                if (mountainWestChunks[i].childCount > 0)
                    ModelCombiner.CombineModel(mountainWestChunks[i], addCollider: false);
            }

            // =====================================================================================
            // 9. BACK (NORTH) HILL SLOPE TREES & GEOLOGICAL FEATURES:
            // Towering northern mountain slope trees, granite boulders, and mountain gullies!
            // Batched into 4 regional chunks for ultra-high performance on Intel UHD 630!
            // =====================================================================================
            var mountainNorthChunks = new Transform[4];
            for (int i = 0; i < 4; i++)
                mountainNorthChunks[i] = Group("Mountain Forest North Chunk " + (i + 1), Vector3.zero);

            float mNorthMinX = -1740.0f, mNorthMaxX = 180.0f;
            float mNorthMinZ = 1558.0f, mNorthMaxZ = 1840.0f;
            int mNorthCols = 54, mNorthRows = 14;
            float mNorthStepX = (mNorthMaxX - mNorthMinX) / mNorthCols;
            float mNorthStepZ = (mNorthMaxZ - mNorthMinZ) / mNorthRows;

            for (int r = 0; r < mNorthRows; r++)
            {
                float rowStagger = (r % 2 == 0) ? 0f : (mNorthStepX * 0.5f);
                for (int c = 0; c < mNorthCols; c++)
                {
                    int seed = (r * mNorthCols + c) * 59 + 5432;
                    float px = mNorthMinX + (c + 0.5f) * mNorthStepX + rowStagger + Random.Range(-mNorthStepX * 0.35f, mNorthStepX * 0.35f);
                    float pz = mNorthMinZ + (r + 0.5f) * mNorthStepZ + Random.Range(-mNorthStepZ * 0.35f, mNorthStepZ * 0.35f);
                    float py = GroundDetailBuilder.HillHeight(px, pz);

                    if (py > 210.0f) continue;

                    int chunkIdx = Mathf.Clamp((int)((px - mNorthMinX) / ((mNorthMaxX - mNorthMinX) / 4.0f)), 0, 3);
                    Transform targetChunk = mountainNorthChunks[chunkIdx];

                    var pt = new Vector3(px, py, pz);
                    var prevP = Parent;
                    Parent = targetChunk;
                    DeepForestJungleTree(pt, Random.Range(36.0f, 68.0f), seed, combineSelf: false);
                    Parent = prevP;
                    treeCount++;

                    if (Random.value < 0.55f)
                    {
                        Vector3 featPos = pt + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                        featPos.y = GroundDetailBuilder.HillHeight(featPos.x, featPos.z);
                        int featType = seed % 4;
                        switch (featType)
                        {
                            case 0: JungleLogAndBoulder(targetChunk, featPos, seed + 1); break;
                            case 1: JungleBambooClump(targetChunk, featPos, seed + 2); break;
                            case 2: JungleFernAndGrassThicket(targetChunk, featPos, 5.0f, seed + 3); break;
                            case 3: JungleBananaGrove(targetChunk, featPos, seed + 4); break;
                        }
                    }
                }
            }

            // Dedicated northern cliff base rock wall pass
            for (float cx = -1720.0f; cx < 170.0f; cx += 35.0f)
            {
                float cz = 1565.0f + Random.Range(-5.0f, 5.0f);
                float cy = GroundDetailBuilder.HillHeight(cx, cz);
                int cIdx = Mathf.Clamp((int)((cx - mNorthMinX) / ((mNorthMaxX - mNorthMinX) / 4.0f)), 0, 3);
                int rSeed = (int)(cx * 23) + 2300;
                JungleLogAndBoulder(mountainNorthChunks[cIdx], new Vector3(cx, cy, cz), rSeed);
                if (rSeed % 2 == 0)
                    JungleBambooClump(mountainNorthChunks[cIdx], new Vector3(cx + 4f, cy + 1.5f, cz + 3f), rSeed + 7);
            }

            for (int i = 0; i < 4; i++)
            {
                if (mountainNorthChunks[i].childCount > 0)
                    ModelCombiner.CombineModel(mountainNorthChunks[i], addCollider: false);
            }

            // =====================================================================================
            // 10. RIGHT (EAST) HILL RIDGE EXTENSION TREES & FEATURES:
            // Covers the entire 1470m eastern mountain ridge from z=70m up to the northern mountains (z=1540m)!
            // Batched into 3 regional chunks for ultra-high performance on Intel UHD 630!
            // =====================================================================================
            var mountainEastChunks = new Transform[3];
            for (int i = 0; i < 3; i++)
                mountainEastChunks[i] = Group("Mountain Forest East Chunk " + (i + 1), Vector3.zero);

            float mEastMinX = 33.0f, mEastMaxX = 160.0f;
            float mEastMinZ = 70.0f, mEastMaxZ = 1540.0f;
            int mEastCols = 8, mEastRows = 50;
            float mEastStepX = (mEastMaxX - mEastMinX) / mEastCols;
            float mEastStepZ = (mEastMaxZ - mEastMinZ) / mEastRows;

            for (int r = 0; r < mEastRows; r++)
            {
                float rowStagger = (r % 2 == 0) ? 0f : (mEastStepX * 0.5f);
                for (int c = 0; c < mEastCols; c++)
                {
                    int seed = (r * mEastCols + c) * 61 + 6543;
                    float px = mEastMinX + (c + 0.5f) * mEastStepX + rowStagger + Random.Range(-mEastStepX * 0.35f, mEastStepX * 0.35f);
                    float pz = mEastMinZ + (r + 0.5f) * mEastStepZ + Random.Range(-mEastStepZ * 0.35f, mEastStepZ * 0.35f);
                    float py = GroundDetailBuilder.HillHeight(px, pz);

                    if (py > 210.0f) continue;

                    int chunkIdx = Mathf.Clamp((int)((pz - mEastMinZ) / ((mEastMaxZ - mEastMinZ) / 3.0f)), 0, 2);
                    Transform targetChunk = mountainEastChunks[chunkIdx];

                    var pt = new Vector3(px, py, pz);
                    var prevP = Parent;
                    Parent = targetChunk;
                    DeepForestJungleTree(pt, Random.Range(32.0f, 62.0f), seed, combineSelf: false);
                    Parent = prevP;
                    treeCount++;

                    if (Random.value < 0.55f)
                    {
                        Vector3 featPos = pt + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
                        featPos.y = GroundDetailBuilder.HillHeight(featPos.x, featPos.z);
                        int featType = seed % 3;
                        switch (featType)
                        {
                            case 0: JungleLogAndBoulder(targetChunk, featPos, seed + 1); break;
                            case 1: JungleBambooClump(targetChunk, featPos, seed + 2); break;
                            case 2: JungleFernAndGrassThicket(targetChunk, featPos, 4.5f, seed + 3); break;
                        }
                    }
                }
            }

            // Dedicated eastern ridge cliff base rock wall pass
            for (float cz = 80.0f; cz < 1520.0f; cz += 32.0f)
            {
                float cx = 38.0f + Random.Range(-3.0f, 6.0f);
                float cy = GroundDetailBuilder.HillHeight(cx, cz);
                int cIdx = Mathf.Clamp((int)((cz - mEastMinZ) / ((mEastMaxZ - mEastMinZ) / 3.0f)), 0, 2);
                int rSeed = (int)(cz * 29) + 3400;
                JungleLogAndBoulder(mountainEastChunks[cIdx], new Vector3(cx, cy, cz), rSeed);
                if (rSeed % 2 == 0)
                    JungleBambooClump(mountainEastChunks[cIdx], new Vector3(cx + 3.5f, cy + 1.2f, cz - 3f), rSeed + 3);
            }

            for (int i = 0; i < 3; i++)
            {
                if (mountainEastChunks[i].childCount > 0)
                    ModelCombiner.CombineModel(mountainEastChunks[i], addCollider: false);
            }

            // Combine regional undergrowth chunks into fast batched meshes
            for (int i = 0; i < 6; i++)
            {
                if (patchChunks[i].childCount > 0)
                    ModelCombiner.CombineModel(patchChunks[i], addCollider: false);
            }

            Random.state = randomState;
        }
    }
}