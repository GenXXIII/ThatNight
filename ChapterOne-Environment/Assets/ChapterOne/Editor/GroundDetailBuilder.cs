using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    public static class GroundDetailBuilder
    {
        static readonly Vector2[] Path = {
            new Vector2(0, -14), new Vector2(0, -4), new Vector2(0, 8),
            new Vector2(-2.5f, 16), new Vector2(-2.5f, 28), new Vector2(-2.5f, 36),
            new Vector2(6, 30), new Vector2(14, 34), new Vector2(18.2f, 38.5f),
            new Vector2(10, 1), new Vector2(13.2f, 1)
        };

        // Natural winding road approach curve extending all the way to map boundary at z=-165m
        // Natural winding road approach curve extending all the way to map boundary at z=-165m
        static readonly Vector2[] HillRoad = {
            new Vector2(48, -165f), new Vector2(48, -150f), new Vector2(48, -135f), 
            new Vector2(48, -122f), new Vector2(48, -111.5f), 
            new Vector2(52, -99), new Vector2(50.5f, -86.5f),
            new Vector2(47, -74), new Vector2(44, -63), new Vector2(39, -54),
            new Vector2(34, -46), new Vector2(29, -39), new Vector2(24, -33),
            new Vector2(20, -28), new Vector2(15, -23), new Vector2(11, -20),
            new Vector2(6, -18), new Vector2(1, -15.5f)
        };

        // Cambodian Termite Mound Positions (Phnom Kmoch - iconic red earth cones in rural jungle)
        public static readonly Vector2[] TermiteMounds = {
            new Vector2(-22f, 58f), new Vector2(14f, 62f), new Vector2(-45f, 95f),
            new Vector2(-18f, 135f), new Vector2(-38f, -4f), new Vector2(-68f, 18f)
        };

        static float SegmentDistance(Vector2 p, Vector2 a, Vector2 b)
        {
            var d = b - a;
            return Vector2.Distance(p, a + d * Mathf.Clamp01(Vector2.Dot(p - a, d) / d.sqrMagnitude));
        }

        public static float PathWear(Vector3 p)
        {
            var p2 = new Vector2(p.x, p.z);
            float d = float.MaxValue;
            for (int i = 0; i < Path.Length - 1; i++)
            {
                if (i == 5 || i == 8) continue;
                d = Mathf.Min(d, SegmentDistance(p2, Path[i], Path[i + 1]));
            }
            return Mathf.Clamp01(1f - d / 1.35f);
        }

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

                // Steep wall: climbs +28m in first 10m (slope > 65 degrees, completely unclimbable!)
                float steepWall = Mathf.Clamp01(dist / 10.0f);
                float wallRise = Mathf.Pow(steepWall, 0.70f) * 28.5f;

                // Crests into deep craggy mountain ridge reaching +48m to +58m
                float upperSlope = Mathf.Clamp01(Mathf.Max(0f, dist - 10.0f) / 45.0f);
                float ridgeRise = Mathf.Pow(upperSlope, 1.10f) * 26.0f;

                // Geological vertical clefts only high up the slope (dist > 12m), NEVER at base
                float highDist = Mathf.Max(0f, dist - 12.0f);
                float verticalClefts = -Mathf.Pow(Mathf.Abs(Mathf.Sin(p.y * 0.075f + p.x * 0.035f)), 1.8f) * 4.5f * Mathf.SmoothStep(0f, 1f, highDist / 8.0f);

                // Craggy rock shelves, spurs and buttresses jutting outward
                float rockSpurs = (Mathf.Sin(p.y * 0.052f + 0.9f) * 4.5f + Mathf.Cos(p.y * 0.11f) * 2.2f) * Mathf.SmoothStep(0f, 1f, highDist / 6.0f);

                // Rugged geological benches
                float ledges = Mathf.Sin(dist * 0.22f + p.y * 0.045f) * 2.5f * Mathf.SmoothStep(0f, 1f, highDist / 8.0f);

                // Natural rock surface roughness
                float cragRoughness = (Mathf.PerlinNoise(p.x * 0.18f + 50f, p.y * 0.18f + 110f) - 0.5f) * 1.5f * Mathf.SmoothStep(0f, 1f, dist / 6.0f);

                rightHillRise = (wallRise + ridgeRise + verticalClefts + rockSpurs + ledges + cragRoughness) * toeBlend;
            }

            // Subtle micro ground texture only outside the yard
            float micro = (Mathf.PerlinNoise(x * 0.30f + 15f, z * 0.30f + 25f) - 0.5f) * 0.06f * yardFactor;

            float terrainBase = leftLandHeight + naturalRoll + rightHillRise + micro;

            return Mathf.Lerp(terrainBase, roadHeight, roadInfluence);
        }

        public static float Height(float x, float z) => HillHeight(x, z);

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

        public static Vector2 HillRoadCenter(float progress) => RoadCenter(progress);

        public static Vector3 HillRoadPoint(float u, float v)
        {
            float t = v;
            Vector2 center = RoadCenter(t);
            Vector2 before = RoadCenter(Mathf.Max(0, t - .003f));
            Vector2 after = RoadCenter(Mathf.Min(1, t + .003f));
            Vector2 tangent = (after - before).normalized;
            Vector2 side = new Vector2(-tangent.y, tangent.x);

            float width = 4.6f;
            float roadHeight = HillHeight(center.x, center.y);
            float crown = (1f - 4f * (u - .5f) * (u - .5f)) * .10f;
            Vector2 p = center + side * ((u - .5f) * width);

            return new Vector3(p.x, roadHeight + crown + 0.035f, p.y);
        }

        public static Color HillRoadShade(Vector3 p)
        {
            var p2 = new Vector2(p.x, p.z);
            float roadDist;
            ClosestRoadProgress(p2, out roadDist);

            float edge = Mathf.Clamp01(roadDist / 2.3f);
            float trackA = Mathf.Exp(-Mathf.Pow(roadDist - 0.95f, 2) / 0.18f);
            float trackB = Mathf.Exp(-Mathf.Pow(roadDist + 0.95f, 2) / 0.18f);
            float track = Mathf.Max(trackA, trackB);

            float shade = Mathf.Lerp(0.72f, 0.95f, track);
            float wear = Mathf.Clamp01(track * 0.95f + (1f - edge) * 0.65f);

            return new Color(shade, 0.0000f, 0.0f, wear);
        }

        public static Color UnifiedGroundShade(Vector3 p)
        {
            float dxWedding = p.x < -20f ? -20f - p.x : p.x > 8f ? p.x - 8f : 0f;
            float dzWedding = p.z < -14f ? -14f - p.z : p.z > 10.5f ? p.z - 10.5f : 0f;
            float distWedding = Mathf.Sqrt(dxWedding * dxWedding + dzWedding * dzWedding);

            float dxGaatSah = p.x < 8f ? 8f - p.x : p.x > 18f ? p.x - 18f : 0f;
            float dzGaatSah = p.z < -9f ? -9f - p.z : p.z > 9f ? p.z - 9f : 0f;
            float distGaatSah = Mathf.Sqrt(dxGaatSah * dxGaatSah + dzGaatSah * dzGaatSah);

            float dxHouse = p.x < -10f ? -10f - p.x : p.x > 8f ? p.x - 8f : 0f;
            float dzHouse = p.z < 14f ? 14f - p.z : p.z > 30f ? p.z - 30f : 0f;
            float distHouse = Mathf.Sqrt(dxHouse * dxHouse + dzHouse * dzHouse);

            float dxKitchen = Mathf.Max(0f, Mathf.Abs(p.x - (-2f)) - 5f);
            float dzKitchen = Mathf.Max(0f, Mathf.Abs(p.z - 39f) - 5f);
            float distKitchen = Mathf.Sqrt(dxKitchen * dxKitchen + dzKitchen * dzKitchen);

            float dxCattle = Mathf.Max(0f, Mathf.Abs(p.x - 18f) - 4f);
            float dzCattle = Mathf.Max(0f, Mathf.Abs(p.z - 38f) - 6f);
            float distCattle = Mathf.Sqrt(dxCattle * dxCattle + dzCattle * dzCattle);

            float dxGate = Mathf.Max(0f, Mathf.Abs(p.x - .5f) - 4.5f);
            float dzGate = Mathf.Max(0f, Mathf.Abs(p.z - (-15f)) - 3.5f);
            float distGate = Mathf.Sqrt(dxGate * dxGate + dzGate * dzGate);

            float minDist = Mathf.Min(distWedding, Mathf.Min(distGaatSah, Mathf.Min(distHouse, Mathf.Min(distKitchen, Mathf.Min(distCattle, distGate)))));

            float noise = (Mathf.PerlinNoise(p.x * 0.075f + 14f, p.z * 0.075f + 35f) - 0.5f) * 6.8f
                        + (Mathf.PerlinNoise(p.x * 0.20f + 55f, p.z * 0.20f + 78f) - 0.5f) * 2.4f;

            float organicDist = minDist + noise;
            float wear = PathWear(p);

            float roadDist = DistanceToHillRoad(new Vector2(p.x, p.z));
            if (roadDist < 3.4f && p.z < SiteLayout.Front + 2)
            {
                organicDist = Mathf.Min(organicDist, 0f);
                wear = Mathf.Max(wear, 1f - roadDist / 3.4f);
            }

            float grass = Mathf.SmoothStep(0.0f, 1.0f, Mathf.InverseLerp(1.5f, 8.5f, organicDist));
            grass *= (1.0f - wear * 0.88f);

            if (SiteLayout.InWeddingRoof(p.x, p.z, 0.4f)) grass = 0;
            Vector3 local = SiteLayout.HouseLocal(p);
            if (Mathf.Abs(local.x) < SiteLayout.HouseWidth * .5f + .2f && Mathf.Abs(local.z) < SiteLayout.HouseDepth * .5f + .2f) grass = 0;

            float shade = 1.0f;
            if (Mathf.Abs(local.x) < SiteLayout.HouseWidth * .5f && Mathf.Abs(local.z) < SiteLayout.HouseDepth * .5f) shade = 0.45f;
            else if (SiteLayout.InWeddingRoof(p.x, p.z)) shade = 0.85f;

            float damp = Mathf.Exp(-new Vector2(p.x - (-2.5f), p.z - 41f).sqrMagnitude / 6.0f) * 0.55f;

            return new Color(shade, grass, damp, wear);
        }

        public static void Dressing()
        {
            var old = Parent;
            Parent = ModelGroup("Yard border grass and fallen leaves", Vector3.zero);
            var randomState = Random.state;
            Random.InitState(200607);
            var leaf = S.Plain("Fallen dry mango leaves", new Color(.25f, .18f, .075f), .09f);

            for (int i = 0; i < 1400; i++)
            {
                var p = new Vector3(Random.Range(-35f, 35f), 0, Random.Range(-25f, 55f));
                var sample = UnifiedGroundShade(p);
                if (sample.g < .25f || sample.g > .88f || sample.a > .25f) continue;
                p.y = Height(p.x, p.z);
                for (int blade = 0; blade < 5; blade++)
                {
                    float a = i * 2.4f + blade * 1.26f, height = Random.Range(.14f, .34f);
                    var d = new Vector3(Mathf.Sin(a), 0, Mathf.Cos(a));
                    var side = new Vector3(d.z, 0, -d.x);
                    Grid("Transitional edge grass", 1, 3, (u, v) => p + Vector3.up * v * height + d * v * v * .13f + side * (u - .5f) * .024f * (1 - v), S.Leaf, Vector2.one, true);
                }
                if (i % 3 == 0)
                {
                    var d = Quaternion.Euler(0, i * 137, 0);
                    Grid("Fallen forest leaf", 4, 2, (u, v) => p + Vector3.up * .012f + d * new Vector3((u - .5f) * .18f, Mathf.Sin(u * Mathf.PI) * .012f, (v - .5f) * .065f * Mathf.Sin(u * Mathf.PI)), leaf, Vector2.one, true);
                }
            }
            Random.state = randomState;
            var group = Parent;
            Parent = old;
            ModelCombiner.CombineModel(group, addCollider: false);
        }
    }
}
