using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static partial class LandscapeBuilder
    {
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
