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

            // 1. 2X EXTENDED COUNTRY & MOUNTAIN TERRAIN GRID (400m wide x 560m long = 224,000 m^2)
            // Covering x in [-280, 120] (400m wide), z in [-160, 400] (560m long)
            // 10 columns by 14 rows of 40m tiles = 140 tiles with 1.66m vertex spacing (optimized for Intel UHD 630)
            for (int cx = -7; cx < 3; cx++)
                for (int cz = -4; cz < 10; cz++)
                {
                    int xIndex = cx, zIndex = cz;
                    var tile = Grid("Continuous countryside terrain tile", 24, 24, (u, v) =>
                    {
                        float px = (xIndex + u) * 40f, pz = (zIndex + v) * 40f;
                        return new Vector3(px, GroundDetailBuilder.HillHeight(px, pz), pz);
                    }, S.Soil, new Vector2(4, 4), false, GroundDetailBuilder.UnifiedGroundShade);

                    var mc = tile.AddComponent<MeshCollider>();
                    mc.sharedMesh = tile.GetComponent<MeshFilter>().sharedMesh;
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

        // Precise architectural safety check: tightly protects buildings while allowing lush surrounding forest!
        public static bool IsInsideWeddingOrHouseZone(float x, float z, float buffer = 3.5f)
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
            if (Mathf.Abs(x) <= 3.5f + buffer && z >= -18.0f - buffer && z <= -13.0f - buffer)
                return true;

            return false;
        }

        // =========================================================================================
        // REALISTIC BOTANICAL ARCHITECTURE (DENSE SIGHTLINE-BLOCKING HORROR JUNGLE)
        // Optimized 1x1 quads (75% fewer triangles = silky smooth 60 FPS scroll & drag in Scene View!)
        // =========================================================================================

        static void VolumetricLeafCluster(Vector3 center, float size, Quaternion rotation, float heightNorm, int seed)
        {
            Color brightGreen = new Color(0.44f, 0.64f, 0.32f);
            Color deepGreen = new Color(0.22f, 0.38f, 0.16f);
            Color clusterColor = Color.Lerp(deepGreen, brightGreen, Mathf.Clamp01(heightNorm));

            // 1. Five 36-degree vertical star fans creating dense 360-degree volume (1x1 quad = 4 tris each)
            for (int i = 0; i < 5; i++)
            {
                Quaternion starRot = rotation * Quaternion.Euler(0, i * 36f, 0);
                Grid("Canopy fan " + i, 1, 1,
                    (u, v) => center + starRot * new Vector3((u - .5f) * size, (v - .5f) * size * 0.75f, 0),
                    S.Foliage, Vector2.one, true, (p) => clusterColor * (0.94f + (i % 3) * 0.04f));
            }

            // 2. Two tilted horizontal canopy roof shelves (blocks view from above, solid canopy dome)
            Quaternion horizRotA = rotation * Quaternion.Euler(18f, 35f, 0);
            Grid("Canopy top shelf A", 1, 1,
                (u, v) => center + Vector3.up * (size * 0.20f) + horizRotA * new Vector3((u - .5f) * size * 1.15f, 0, (v - .5f) * size * 0.90f),
                S.Foliage, Vector2.one, true, (p) => clusterColor * 1.06f);

            Quaternion horizRotB = rotation * Quaternion.Euler(-18f, 85f, 0);
            Grid("Canopy top shelf B", 1, 1,
                (u, v) => center + Vector3.up * (size * 0.32f) + horizRotB * new Vector3((u - .5f) * size * 1.10f, 0, (v - .5f) * size * 0.88f),
                S.Foliage, Vector2.one, true, (p) => clusterColor * 1.10f);

            // 3. Dense upward dome cap
            Quaternion capRot = rotation * Quaternion.Euler(38f, 60f, 0);
            Grid("Canopy dome cap", 1, 1,
                (u, v) => center + Vector3.up * (size * 0.40f) + capRot * new Vector3((u - .5f) * size * 0.95f, (v - .5f) * size * 0.70f, 0),
                S.Foliage, Vector2.one, true, (p) => clusterColor * 1.14f);
        }

        // =========================================================================================
        // 1A. 0m - 50m NEAR-HOUSE SLENDER TREE (3X SMALLER):
        // Slender 3x smaller trunk (radius 0.24m - 0.32m = diam ~0.5m), height 12m - 14.5m
        // Delicate, light, human-scaled - never crowds or overpowers the house & wedding marquee!
        // =========================================================================================

        static void SlenderSmallVillageTree(Vector3 p, float height, int seed)
        {
            if (IsInsideWeddingOrHouseZone(p.x, p.z, 3.5f)) return;
            var old = Parent;
            Parent = ModelGroup("Slender Small Village Tree", p);

            // 3x smaller trunk: radius 0.24m - 0.32m (diameter 0.48m - 0.64m)
            float trunkRadius = Mathf.Lerp(0.24f, 0.32f, Mathf.InverseLerp(12, 15, height));
            float branchHeight = height * 0.45f; // ~5.5m - 6.5m high (clear open eye level!)

            var profile = new Vector2[] {
                new Vector2(trunkRadius * 1.10f, -0.4f),
                new Vector2(trunkRadius, 0.0f),
                new Vector2(trunkRadius, branchHeight * 0.5f),
                new Vector2(trunkRadius * 0.90f, branchHeight),
                new Vector2(trunkRadius * 0.55f, height)
            };
            Lathe("Slender 3x Small Trunk", Vector3.zero, profile, S.DarkWood, 8);

            Vector3 branchOrigin = Vector3.up * branchHeight;
            Vector3 summit = Vector3.up * height;

            // 5 spreading light boughs
            int branchCount = 5;
            for (int b = 0; b < branchCount; b++)
            {
                float ang = b * Mathf.PI * 2f / branchCount + seed * 1.5f;
                var bDir = new Vector3(Mathf.Sin(ang), Random.Range(0.22f, 0.40f), Mathf.Cos(ang)).normalized;
                float bLen = Random.Range(4.2f, 6.2f);
                Vector3 bStart = Vector3.Lerp(branchOrigin, summit, (b + 0.30f) / branchCount);
                Vector3 bTip = bStart + bDir * bLen;

                Beam("Small bough", bStart, bTip, trunkRadius * 0.30f, S.DarkWood);

                var rot = Quaternion.Euler(Random.Range(5, 18), (b * 72 + seed * 3) % 360, 0);
                float foliageScale = Random.Range(6.8f, 9.2f);
                VolumetricLeafCluster(bTip, foliageScale, rot, 0.70f, seed + b * 7);
                VolumetricLeafCluster(Vector3.Lerp(bStart, bTip, 0.50f), foliageScale * 0.85f, rot * Quaternion.Euler(0, 45, 0), 0.60f, seed + b * 7 + 3);
            }

            // Central apex crown dome
            VolumetricLeafCluster(summit, 9.2f, Quaternion.Euler(0, (seed * 43) % 360, 0), 0.95f, seed + 80);
            VolumetricLeafCluster(summit + Vector3.up * 1.6f, 7.2f, Quaternion.Euler(0, (seed * 89) % 360, 0), 1.0f, seed + 90);

            var treeObj = Parent;
            Parent = old;
            var cap = treeObj.gameObject.AddComponent<CapsuleCollider>();
            cap.center = new Vector3(0, height * 0.5f, 0);
            cap.radius = trunkRadius * 1.15f;
            cap.height = height;
            ModelCombiner.CombineModel(treeObj, addCollider: false);
        }

        // =========================================================================================
        // 1B. 50m - 100m MID-ZONE FAT VILLAGE TREE:
        // FATTER TRUNK (diam 1.04m - 1.44m) with full, lush, spreading 6-bough rounded crown!
        // Elegant mature Cambodian village tree (Mango, Jackfruit)
        // =========================================================================================

        static void FatVillageTree(Vector3 p, float height, int seed)
        {
            if (IsInsideWeddingOrHouseZone(p.x, p.z, 3.5f)) return;
            var old = Parent;
            Parent = ModelGroup("Fat Village Tree", p);

            // FATTER TRUNK: radius 0.52m - 0.72m (diameter 1.04m - 1.44m)
            float trunkRadius = Mathf.Lerp(0.52f, 0.72f, Mathf.InverseLerp(13, 17, height));
            float branchHeight = height * 0.44f; // ~6.0m - 7.5m high (open eye level!)

            var profile = new Vector2[] {
                new Vector2(trunkRadius * 1.12f, -0.4f),
                new Vector2(trunkRadius, 0.0f),
                new Vector2(trunkRadius, branchHeight * 0.5f),
                new Vector2(trunkRadius * 0.95f, branchHeight),
                new Vector2(trunkRadius * 0.60f, height)
            };
            Lathe("Fat Village Round Trunk", Vector3.zero, profile, S.DarkWood, 10);

            Vector3 branchOrigin = Vector3.up * branchHeight;
            Vector3 summit = Vector3.up * height;

            // Spreading branch scaffold packed with lush leaves (6 wide boughs for a full, fat canopy!)
            int branchCount = 6;
            for (int b = 0; b < branchCount; b++)
            {
                float ang = b * Mathf.PI * 2f / branchCount + seed * 1.5f;
                var bDir = new Vector3(Mathf.Sin(ang), Random.Range(0.20f, 0.38f), Mathf.Cos(ang)).normalized;
                float bLen = Random.Range(6.0f, 8.8f);
                Vector3 bStart = Vector3.Lerp(branchOrigin, summit, (b + 0.30f) / branchCount);
                Vector3 bTip = bStart + bDir * bLen;

                Beam("Village bough", bStart, bTip, trunkRadius * 0.30f, S.DarkWood);

                var rot = Quaternion.Euler(Random.Range(5, 18), (b * 60 + seed * 3) % 360, 0);
                float foliageScale = Random.Range(10.0f, 13.5f);
                VolumetricLeafCluster(bTip, foliageScale, rot, 0.70f, seed + b * 7);
                VolumetricLeafCluster(Vector3.Lerp(bStart, bTip, 0.50f), foliageScale * 0.88f, rot * Quaternion.Euler(0, 45, 0), 0.60f, seed + b * 7 + 3);
            }

            // Central apex crown dome (large and full)
            VolumetricLeafCluster(summit, 13.5f, Quaternion.Euler(0, (seed * 43) % 360, 0), 0.95f, seed + 80);
            VolumetricLeafCluster(summit + Vector3.up * 2.0f, 10.5f, Quaternion.Euler(0, (seed * 89) % 360, 0), 1.0f, seed + 90);

            var treeObj = Parent;
            Parent = old;
            var cap = treeObj.gameObject.AddComponent<CapsuleCollider>();
            cap.center = new Vector3(0, height * 0.5f, 0);
            cap.radius = trunkRadius * 1.15f;
            cap.height = height;
            ModelCombiner.CombineModel(treeObj, addCollider: false);
        }

        // =========================================================================================
        // 2. DEEP FOREST JUNGLE TREE (EXCELLENT GAMEPLAY SIGHTLINES & EASY NAVIGATION!)
        // Branches start high at 7.0m - 12.0m so player has clean eye-level views between trunks!
        // Ancient deep rainforest scale: height 26m - 36m, trunk diameter 2.0m - 3.2m
        // =========================================================================================

        static void DeepForestJungleTree(Vector3 p, float height, int seed)
        {
            if (IsInsideWeddingOrHouseZone(p.x, p.z, 3.5f)) return;
            var old = Parent;
            Parent = ModelGroup("Deep Forest Jungle Tree", p);

            // Solid ancient trunk (radius 1.0m - 1.6m = diameter 2.0m - 3.2m)
            float trunkRadius = Mathf.Lerp(1.0f, 1.60f, Mathf.InverseLerp(26, 36, height));
            // Lower branches lifted high (branchHeight ~10m - 15m) for clear player visibility and easy navigation!
            float branchHeight = height * 0.42f;
            float midBranchHeight = height * 0.65f;

            // Equal round trunk sinking -0.6m into dirt (NO pedestal disc!)
            var profile = new Vector2[] {
                new Vector2(trunkRadius, -0.6f),
                new Vector2(trunkRadius, 0.0f),
                new Vector2(trunkRadius, branchHeight * 0.6f),
                new Vector2(trunkRadius * 0.85f, branchHeight),
                new Vector2(trunkRadius * 0.55f, height)
            };
            Lathe("Ancient Round Trunk", Vector3.zero, profile, S.DarkWood, 10);

            Vector3 summit = Vector3.up * height;

            // TIER 1: MID-LEVEL SPREADING CANOPY BOUGHS (Lifted high so eye-level is completely clear!)
            int branchCount = 6;
            for (int b = 0; b < branchCount; b++)
            {
                float ang = b * Mathf.PI * 2f / branchCount + seed * 1.8f;
                var bDir = new Vector3(Mathf.Sin(ang), Random.Range(0.18f, 0.38f), Mathf.Cos(ang)).normalized;
                float bLen = Random.Range(7.5f, 12.0f);
                Vector3 bStart = Vector3.Lerp(Vector3.up * branchHeight, Vector3.up * midBranchHeight, (b + 0.2f) / branchCount);
                Vector3 bTip = bStart + bDir * bLen;

                Beam("Mid canopy bough", bStart, bTip, trunkRadius * 0.28f, S.DarkWood);

                var rot = Quaternion.Euler(Random.Range(5, 18), (b * 60 + seed * 5) % 360, 0);
                float foliageScale = Random.Range(10.5f, 14.5f);
                VolumetricLeafCluster(bTip, foliageScale, rot, 0.65f, seed + b * 13);
                VolumetricLeafCluster(Vector3.Lerp(bStart, bTip, 0.55f), foliageScale * 0.85f, rot * Quaternion.Euler(0, 45, 0), 0.55f, seed + b * 13 + 5);
            }

            // TIER 2: HIGH SUMMIT CROWN DOME
            VolumetricLeafCluster(summit, 16.0f, Quaternion.Euler(0, (seed * 37) % 360, 0), 0.95f, seed + 100);
            VolumetricLeafCluster(summit + Vector3.up * 2.8f, 13.0f, Quaternion.Euler(0, (seed * 73) % 360, 0), 1.0f, seed + 110);

            var treeObj = Parent;
            Parent = old;
            var cap = treeObj.gameObject.AddComponent<CapsuleCollider>();
            cap.center = new Vector3(0, height * 0.5f, 0);
            cap.radius = trunkRadius * 1.15f;
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
                VolumetricLeafCluster(sapPos + Vector3.up * sH, 2.8f, Quaternion.Euler(15, seed * 19, 0), 0.60f, seed * 23);
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

            // =====================================================================================
            // 3. DEDICATED RIGHT-SIDE HILL FILLING PASS (MUST FILL WITH MEDIUM TREES!)
            // User explicit request: 'about right side must fill medium tree'
            // BUT: 'remove tree beside of road' -> 16m road clearance keeps the road corridor open!
            // Fills the eastern mountain slope (x in [22m, 85m], z in [-135m, 65m])
            // with lush, beautiful, medium trees (FatVillageTree)!
            // =====================================================================================
            for (float rz = -135.0f; rz <= 65.0f; rz += 10.0f)
            {
                for (float rx = 22.0f; rx <= 85.0f; rx += 9.0f)
                {
                    int hillSeed = (int)(rx * 37 + rz * 23) + 7000;
                    float jx = Random.Range(-2.5f, 2.5f);
                    float jz = Random.Range(-2.5f, 2.5f);
                    float fx = rx + jx;
                    float fz = rz + jz;

                    if (IsInsideWeddingOrHouseZone(fx, fz, 3.5f)) continue;

                    // STRICT ROAD CLEARANCE: NO TREES BESIDE THE ROAD! (16m clearance corridor)
                    float distToRoad = GroundDetailBuilder.DistanceToHillRoad(new Vector2(fx, fz));
                    if (distToRoad < 16.0f && fz < SiteLayout.Front + 5.0f) continue;

                    // Beside marquee / catering tents: keep trees outside tent canvas
                    if (fz >= SiteLayout.Front + 5.0f)
                    {
                        if (fz <= 12.0f && fx < 24.5f) continue;
                        if (fz > 12.0f && fz <= 32.0f && fx < 25.5f) continue;
                        if (fz > 32.0f && fx < 24.0f) continue;
                    }

                    var pt = new Vector3(fx, GroundDetailBuilder.HillHeight(fx, fz), fz);
                    FatVillageTree(pt, Random.Range(13.5f, 16.5f), hillSeed);
                    treeCount++;
                }
            }

            // =====================================================================================
            // 4. MAIN SURROUNDING FOREST GRID:
            // Spans x in [-260, 105] (365m wide), z in [-155, 360] (515m long)
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
                for (int c = 0; c < cols; c++)
                {
                    int cellSeed = (r * cols + c) * 37 + 2006;
                    
                    // Cell center
                    float cx = minX + (c + 0.5f) * stepX;
                    float cz = minZ + (r + 0.5f) * stepZ;

                    // Skip the right-side hill since the dedicated pass fills it completely!
                    if (cx >= 20.0f && cz >= -140.0f && cz <= 70.0f) continue;

                    // Large organic chaotic jitter breaks any lines or rows
                    float jx = Random.Range(-stepX * 0.40f, stepX * 0.40f);
                    float jz = Random.Range(-stepZ * 0.40f, stepZ * 0.40f);
                    float px = cx + jx;
                    float pz = cz + jz;

                    // Skip if inside wedding marquee, house, kitchen, or cattle shelter (tight 3.5m buffer)
                    if (IsInsideWeddingOrHouseZone(px, pz, 3.5f)) continue;

                    // STRICT ROAD CLEARANCE: NO TREES BESIDE THE ROAD! (16m clearance corridor)
                    float distToRoad = GroundDetailBuilder.DistanceToHillRoad(new Vector2(px, pz));
                    if (distToRoad < 16.0f && pz < SiteLayout.Front + 5.0f) continue;

                    // Distance from house & wedding clearing center (0, 15)
                    float distFromClearing = Vector2.Distance(new Vector2(px, pz), new Vector2(0f, 15f));

                    var pt = new Vector3(px, GroundDetailBuilder.HillHeight(px, pz), pz);

                    // =================================================================================
                    // TREE SELECTION:
                    // - 0m to 50m around house: 3X SMALLER SLENDER TREES (spacious & light)
                    // - 50m to 100m: FAT VILLAGE TREES
                    // - 100m+: COLOSSAL DEEP JUNGLE GIANTS
                    // =================================================================================
                    if (distFromClearing < 50.0f)
                    {
                        // Zone 1: 0m to 50m -> 3x smaller slender tree (decreased density)
                        if ((cellSeed % 3) == 0)
                        {
                            SlenderSmallVillageTree(pt, Random.Range(12.0f, 14.5f), cellSeed);
                            treeCount++;
                        }
                    }
                    else if (distFromClearing < 100.0f)
                    {
                        // Zone 2: 50m to 100m -> Fat village tree (decreased density)
                        if ((cellSeed % 3) == 0)
                        {
                            FatVillageTree(pt, Random.Range(13.5f, 16.5f), cellSeed);
                            treeCount++;
                        }
                    }
                    else
                    {
                        // Zone 3: 100m+ -> Full dense deep jungle giants
                        DeepForestJungleTree(pt, Random.Range(26.0f, 36.0f), cellSeed);
                        treeCount++;
                    }

                    // Companion tree in deep forest beyond 100m
                    if (Random.value < 0.12f && distFromClearing >= 100.0f)
                    {
                        float compAngle = Random.Range(0f, Mathf.PI * 2f);
                        float compDist = Random.Range(4.0f, 7.2f);
                        float cpx = px + Mathf.Cos(compAngle) * compDist;
                        float cpz = pz + Mathf.Sin(compAngle) * compDist;

                        float compDistToRoad = GroundDetailBuilder.DistanceToHillRoad(new Vector2(cpx, cpz));

                        if (!IsInsideWeddingOrHouseZone(cpx, cpz, 3.5f) && 
                            (compDistToRoad >= 16.0f || cpz >= SiteLayout.Front + 5.0f) &&
                            cpx >= minX && cpx <= maxX && cpz >= minZ && cpz <= maxZ)
                        {
                            var cpt = new Vector3(cpx, GroundDetailBuilder.HillHeight(cpx, cpz), cpz);
                            DeepForestJungleTree(cpt, Random.Range(25.0f, 34.0f), cellSeed + 99);
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

                        int featType = (cellSeed + r * 5) % 4;
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
                        }
                        featureCount++;
                    }
                }
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