using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static partial class LandscapeBuilder
    {
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
    }
}
