using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static partial class LandscapeBuilder
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
            // UNIFORM RESOLUTION (10x10 quads per 40m tile = 4.0m grid spacing everywhere):
            // Using the exact same quad resolution across EVERY tile guarantees that every shared boundary
            // vertex between adjacent tiles evaluates HillHeight at the exact same coordinates.
            // This 100% eliminates all T-junctions, gaps, holes, and seam tears!
            var terrainChunks = new Transform[16];
            for (int i = 0; i < 16; i++)
                terrainChunks[i] = Group("Terrain Region Chunk " + (i + 1), Vector3.zero);

            const int quads = 10;

            for (int cx = -46; cx < 6; cx++)
                for (int cz = -4; cz < 49; cz++)
                {
                    int xIndex = cx, zIndex = cz;

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

        static void BuildWeddingFrontage(float front)
        {
            // Dedicated wedding marquee, gate, and carpet network are built exclusively by WeddingYardBuilder under '02 WEDDING'
        }

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
    }
}
