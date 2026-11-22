using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static partial class LandscapeBuilder
    {
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
    }
}
