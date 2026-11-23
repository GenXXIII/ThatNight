using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    public static class ForestTerrainLayout
    {
        static readonly Vector2[] HillRoad = {
            new Vector2(48, -165f), new Vector2(48, -150f), new Vector2(48, -135f), 
            new Vector2(48, -122f), new Vector2(48, -111.5f), 
            new Vector2(52, -99), new Vector2(50.5f, -86.5f),
            new Vector2(47, -74), new Vector2(44, -63), new Vector2(39, -54),
            new Vector2(34, -46), new Vector2(29, -39), new Vector2(24, -33),
            new Vector2(20, -28), new Vector2(15, -23), new Vector2(11, -20),
            new Vector2(6, -18), new Vector2(1, -15.5f)
        };

        public static float HillHeight(float x, float z)
        {
            var p = new Vector2(x, z);
            float roadDist, roadProgress = ClosestRoadProgress(p, out roadDist);
            float flat = SiteLayout.HillElevation - .12f; // 20.88m (exact wedding and house ground level)
            float lowland = 3.55f; // Entrance road start level

            Vector2 roadCenter = RoadCenter(roadProgress);

            // =========================================================================
            // 1. WEDDING & HOUSE ZONE PROTECTION: PERFECTLY FLAT (20.88m)
            // Inside the wedding marquee, courtyard, house, kitchen, and cattle shelter
            // (x in [-24, 24], z in [-18, 48]), the ground is locked flat at 20.88m!
            // ZERO bumps or positive roll so chairs, tables, and decor NEVER get buried!
            // =========================================================================
            float dxWedding = Mathf.Max(0f, Mathf.Abs(x) - 23.0f);
            float dzWedding = z < -18.0f ? (-18.0f - z) : z > 48.0f ? (z - 48.0f) : 0f;
            float distFromYard = Mathf.Sqrt(dxWedding * dxWedding + dzWedding * dzWedding);
            float yardFactor = Mathf.Clamp01(distFromYard / 8.0f); // 0 inside yard, 1 outside

            // =========================================================================
            // 2. LEFT SIDE OF ROAD: EQUAL TO WEDDING ZONE ELEVATION (20.88m)
            // The entire plateau to the left (West) of the road is at flat = 20.88m!
            // Only at the far southern entrance approach (z < -130m) does it gently roll down like a real hill.
            // =========================================================================
            float leftLandHeight = flat;

            if (z < -130.0f)
            {
                float southExit = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-165.0f, -130.0f, z));
                leftLandHeight = Mathf.Lerp(lowland, flat, southExit);
            }

            // Rolling countryside hill swells ONLY OUTSIDE the yard (yardFactor = 0 inside yard!)
            float naturalRoll = (Mathf.Sin(x * 0.045f + 1.2f) * 0.55f + Mathf.Cos(z * 0.055f) * 0.45f) * yardFactor * Mathf.Clamp01(-z / 25.0f);

            // Smooth road climb along the road spline (starts at lowland 3.55m, climbs to 20.88m)
            float climb = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.02f, .96f, roadProgress));
            float roadHeight = Mathf.Lerp(lowland, flat, climb);

            // Road embankment blend: wide, natural sloping shoulder (from 4.5m to 16.0m)
            // Creates a real, natural, gradual hillside slope up to the 20.88m plateau without cliffs!
            float roadInfluence = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(4.5f, 16.0f, roadDist));

            // =========================================================================
            // 3. WHOLE RIGHT SIDE: REAL, DEEP, IMPASSABLE MOUNTAIN BLUFF
            // Covers the entire right side of the map from front (z=-165m) to back (z=400m)!
            // Aggressive incline (>60 degrees) climbing +30m to +55m so player CANNOT climb up!
            // Toe starts cleanly with SmoothStep blend around marquee to ELIMINATE any sharp crease!
            // =========================================================================
            float rightHillRise = 0f;
            float roadStartX = roadCenter.x + 12.0f;
            float yardStartX = 32.5f;

            float hillStartX;
            if (z >= -18.0f)
            {
                hillStartX = yardStartX;
            }
            else
            {
                float t = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-70.0f, -18.0f, z));
                hillStartX = Mathf.Max(yardStartX, Mathf.Lerp(roadStartX, yardStartX, t));
            }

            if (p.x > hillStartX)
            {
                float dist = p.x - hillStartX;
                float toeBlend = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(dist / 6.0f));

                // Seamless connection to northern mountains:
                // Right-side hill starts at +55m beside the village, and smoothly scales up to +240m
                // as it travels north (z > 70m to 1100m) to connect seamlessly with the northern mountains!
                float northRamp = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(70.0f, 1100.0f, z));
                float heightMultiplier = Mathf.Lerp(1.0f, 4.4f, northRamp);

                // Steep wall: climbs +28m in first 10m (slope > 65 degrees, completely unclimbable!)
                float steepWall = Mathf.Clamp01(dist / 10.0f);
                float wallRise = Mathf.Pow(steepWall, 0.70f) * 28.5f * heightMultiplier;

                // Crests into deep craggy mountain ridge reaching +48m to +58m beside village, and +240m up north!
                float upperSlope = Mathf.Clamp01(Mathf.Max(0f, dist - 10.0f) / 45.0f);
                float ridgeRise = Mathf.Pow(upperSlope, 1.10f) * 26.0f * heightMultiplier;

                // Geological vertical clefts only high up the slope (dist > 12m), NEVER at base
                float highDist = Mathf.Max(0f, dist - 12.0f);
                float verticalClefts = -Mathf.Pow(Mathf.Abs(Mathf.Sin(p.y * 0.075f + p.x * 0.035f)), 1.8f) * 4.5f * Mathf.SmoothStep(0f, 1f, highDist / 8.0f) * heightMultiplier;

                // Craggy rock shelves, spurs and buttresses jutting outward
                float rockSpurs = (Mathf.Sin(p.y * 0.052f + 0.9f) * 4.5f + Mathf.Cos(p.y * 0.11f) * 2.2f) * Mathf.SmoothStep(0f, 1f, highDist / 6.0f) * heightMultiplier;

                // Rugged geological benches
                float ledges = Mathf.Sin(dist * 0.22f + p.y * 0.045f) * 2.5f * Mathf.SmoothStep(0f, 1f, highDist / 8.0f) * heightMultiplier;

                // Natural rock surface roughness
                float cragRoughness = (Mathf.PerlinNoise(p.x * 0.18f + 50f, p.y * 0.18f + 110f) - 0.5f) * 1.5f * Mathf.SmoothStep(0f, 1f, dist / 6.0f) * heightMultiplier;

                rightHillRise = (wallRise + ridgeRise + verticalClefts + rockSpurs + ledges + cragRoughness) * toeBlend;
            }

            // =========================================================================
            // 4. IMPASSABLE EXTENDED MOUNTAIN BARRIER (LEFT, BEHIND, AND CONNECTING TO RIGHT)
            // Designed with the EXACT same unclimbable steep cliffs and rugged crags as the Right Hill!
            // - Left (West): x in [-1760m, -1440m] climbs +30m in first 10m (slope > 65 deg), reaching +240m!
            // - Behind (North): z in [1560m, 1880m] climbs +30m in first 10m (slope > 65 deg), reaching +240m!
            // - Connects seamlessly with the right-side hill in the Northeast corner!
            // =========================================================================
            float westHillRise = 0f;
            float westStartX = -1440.0f;
            if (x < westStartX)
            {
                float distW = westStartX - x;
                float toeBlendW = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(distW / 6.0f));

                // Steep unclimbable wall: climbs +30m in first 10m (slope > 65 degrees!)
                float steepWallW = Mathf.Clamp01(distW / 10.0f);
                float wallRiseW = Mathf.Pow(steepWallW, 0.70f) * 30.0f;

                // Crests into deep craggy mountain ridge reaching +180m to +240m
                float upperSlopeW = Mathf.Clamp01(Mathf.Max(0f, distW - 10.0f) / 180.0f);
                float ridgeRiseW = Mathf.Pow(upperSlopeW, 0.88f) * 140.0f + Mathf.Pow(upperSlopeW, 1.4f) * 80.0f;

                // Geological vertical clefts (only high up the slope)
                float highDistW = Mathf.Max(0f, distW - 12.0f);
                float verticalCleftsW = -Mathf.Pow(Mathf.Abs(Mathf.Sin(z * 0.035f + x * 0.015f)), 1.8f) * 16.0f * Mathf.SmoothStep(0f, 1f, highDistW / 10.0f);

                // Craggy rock shelves, spurs and buttresses jutting outward
                float rockSpursW = (Mathf.Sin(z * 0.022f + 0.9f) * 20.0f + Mathf.Cos(z * 0.048f) * 10.0f) * Mathf.SmoothStep(0f, 1f, highDistW / 8.0f);

                // Rugged geological benches
                float ledgesW = Mathf.Sin(distW * 0.14f + z * 0.035f) * 12.0f * Mathf.SmoothStep(0f, 1f, highDistW / 10.0f);

                // Natural rock surface roughness
                float cragRoughnessW = (Mathf.PerlinNoise(x * 0.035f + 40f, z * 0.035f + 80f) - 0.5f) * 15.0f * Mathf.SmoothStep(0f, 1f, distW / 8.0f);

                westHillRise = (wallRiseW + ridgeRiseW + verticalCleftsW + rockSpursW + ledgesW + cragRoughnessW) * toeBlendW;
            }

            float northHillRise = 0f;
            float northStartZ = 1560.0f;
            if (z > northStartZ)
            {
                float distN = z - northStartZ;
                float toeBlendN = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(distN / 6.0f));

                // Steep unclimbable wall: climbs +30m in first 10m (slope > 65 degrees!)
                float steepWallN = Mathf.Clamp01(distN / 10.0f);
                float wallRiseN = Mathf.Pow(steepWallN, 0.70f) * 30.0f;

                // Crests into deep craggy mountain ridge reaching +180m to +240m
                float upperSlopeN = Mathf.Clamp01(Mathf.Max(0f, distN - 10.0f) / 180.0f);
                float ridgeRiseN = Mathf.Pow(upperSlopeN, 0.88f) * 140.0f + Mathf.Pow(upperSlopeN, 1.4f) * 80.0f;

                // Geological vertical clefts (only high up the slope)
                float highDistN = Mathf.Max(0f, distN - 12.0f);
                float verticalCleftsN = -Mathf.Pow(Mathf.Abs(Mathf.Sin(x * 0.035f + z * 0.015f)), 1.8f) * 16.0f * Mathf.SmoothStep(0f, 1f, highDistN / 10.0f);

                // Craggy rock shelves, spurs and buttresses jutting outward
                float rockSpursN = (Mathf.Sin(x * 0.022f + 0.9f) * 20.0f + Mathf.Cos(x * 0.048f) * 10.0f) * Mathf.SmoothStep(0f, 1f, highDistN / 8.0f);

                // Rugged geological benches
                float ledgesN = Mathf.Sin(distN * 0.14f + x * 0.035f) * 12.0f * Mathf.SmoothStep(0f, 1f, highDistN / 10.0f);

                // Natural rock surface roughness
                float cragRoughnessN = (Mathf.PerlinNoise(x * 0.035f + 60f, z * 0.035f + 120f) - 0.5f) * 15.0f * Mathf.SmoothStep(0f, 1f, distN / 8.0f);

                northHillRise = (wallRiseN + ridgeRiseN + verticalCleftsN + rockSpursN + ledgesN + cragRoughnessN) * toeBlendN;
            }

            // Seamless corner blend (NW corner where West and North meet)
            float outerMountainRise = Mathf.Max(westHillRise, northHillRise) + Mathf.Min(westHillRise, northHillRise) * 0.35f;

            // Subtle micro ground texture only outside the yard
            float micro = (Mathf.PerlinNoise(x * 0.30f + 15f, z * 0.30f + 25f) - 0.5f) * 0.06f * yardFactor;

            float terrainBase = leftLandHeight + naturalRoll + rightHillRise + outerMountainRise + micro;

            return Mathf.Lerp(terrainBase, roadHeight, roadInfluence);
        }

        public static float DistanceToHillRoad(Vector2 p)
        {
            float distance;
            ClosestRoadProgress(p, out distance);
            return distance;
        }

        static float ClosestRoadProgress(Vector2 p, out float distance)
        {
            const int samples = 120;
            distance = 1000f;
            float progress = 0;
            Vector2 a = RoadCenter(0);
            for (int i = 1; i <= samples; i++)
            {
                Vector2 b = RoadCenter(i / (float)samples);
                Vector2 segment = b - a;
                float along = Mathf.Clamp01(Vector2.Dot(p - a, segment) / segment.sqrMagnitude);
                float candidate = Vector2.Distance(p, a + segment * along);
                if (candidate < distance)
                {
                    distance = candidate;
                    progress = (i - 1 + along) / samples;
                }
                a = b;
            }
            return progress;
        }

        static Vector2 RoadCenter(float t)
        {
            float scaled = t * (HillRoad.Length - 1);
            int i = Mathf.Clamp(Mathf.FloorToInt(scaled), 0, HillRoad.Length - 2);
            float u = scaled - i;

            Vector2 p0 = HillRoad[Mathf.Max(i - 1, 0)];
            Vector2 p1 = HillRoad[i];
            Vector2 p2 = HillRoad[i + 1];
            Vector2 p3 = HillRoad[Mathf.Min(i + 2, HillRoad.Length - 1)];

            return 0.5f * (
                (2f * p1) +
                (-p0 + p2) * u +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * (u * u) +
                (-p0 + 3f * p1 - 3f * p2 + p3) * (u * u * u)
            );
        }

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
    }
}
