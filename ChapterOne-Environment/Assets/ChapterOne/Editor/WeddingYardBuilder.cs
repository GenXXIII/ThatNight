using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    public static class WeddingYardBuilder
    {
        public static Transform Build()
        {
            if (S.Wood == null) S.Prepare();
            Parent = null;
            var root = Group("02 WEDDING - full marquee with seating dance and catering", Vector3.up * SiteLayout.HillElevation);
            Parent = root;

            // 1. Open-air Marquee Canopy
            WeddingRoofBuilder.Build();

            // 2. Organic Mountain-View Guest Banquet Seating Layout
            var seatingZone = Group("Left and right guest banquet seating zones", Vector3.zero);
            var prevParent = Parent;
            Parent = seatingZone;

            // Left Seating Wing (20 Tables arranged in a clean 4 columns x 5 rows grid)
            Vector3[] leftTablePositions = {
                // Row 1 (Front row overlooking mountain slope)
                new Vector3(-18.5f, 0, -8.5f), new Vector3(-14.2f, 0, -8.5f), new Vector3(-9.9f, 0, -8.5f), new Vector3(-5.5f, 0, -8.5f),
                // Row 2
                new Vector3(-18.5f, 0, -4.0f), new Vector3(-14.2f, 0, -4.0f), new Vector3(-9.9f, 0, -4.0f), new Vector3(-5.5f, 0, -4.0f),
                // Row 3
                new Vector3(-18.5f, 0,  0.5f), new Vector3(-14.2f, 0,  0.5f), new Vector3(-9.9f, 0,  0.5f), new Vector3(-5.5f, 0,  0.5f),
                // Row 4
                new Vector3(-18.5f, 0,  5.0f), new Vector3(-14.2f, 0,  5.0f), new Vector3(-9.9f, 0,  5.0f), new Vector3(-5.5f, 0,  5.0f),
                // Row 5 (Rear row near back marquee eave)
                new Vector3(-18.5f, 0,  9.5f), new Vector3(-14.2f, 0,  9.5f), new Vector3(-9.9f, 0,  9.5f), new Vector3(-5.5f, 0,  9.5f)
            };

            foreach (var pos in leftTablePositions)
            {
                DiningTable(pos);
                for (int chair = 0; chair < 6; chair++)
                {
                    float a = chair * Mathf.PI / 3 + .15f;
                    // Chairs face inward toward the banquet table
                    PropsBuilder.Chair(pos + new Vector3(Mathf.Sin(a) * 1.26f, 0, Mathf.Cos(a) * 1.26f), a * Mathf.Rad2Deg);
                }
            }

            // (Loudspeakers and beverage crates removed per user request)

            Parent = prevParent;

            // 3. Right Side Traditional Cambodian Hair Cutting Ceremony (Gaat Sah)
            WeddingEventBuilder.Build();

            // 4. Authentic Cambodian Wedding Entrance Gate (Khlok Pka Sla)
            WeddingPhotoCornerBuilder.Build();

            // 5. Unified Ceremonial Wedding Carpet Network (Entrance to House, and Branching to Stage)
            BuildCeremonialCarpetNetwork();

            // 6. Catering Kitchen Corner
            WeddingCateringBuilder.Build();

            Parent = null;
            return root;
        }

                        public static void BuildCeremonialCarpetNetwork()
        {
            var carpetGroup = ModelGroup("Ceremonial Wedding Carpet Network", Vector3.zero);
            var prevParent = Parent;
            Parent = carpetGroup;

            var pinkMat = S.Pink;
            var goldSilk = S.Plain("Carpet gold piping", new Color(.82f, .65f, .18f), .45f, .25f);
            float groundY = 0.018f;

            // 1A. Main Promenade: Straight ceremonial carpet runner from entrance (Z = -18.0) up to approach zone (Z = 6.50m)
            Box("Main promenade", 
                new Vector3(0f, groundY, -5.75f), new Vector3(1.97f, 0.008f, 24.50f), pinkMat);

            // 1B. Cross-Walkway in Front of Music Ensemble
            Box("Cross walkway in front of music", 
                new Vector3(5.3925f, groundY + 0.0005f, -0.13f), new Vector3(8.815f, 0.008f, 1.73f), pinkMat);

            // Double-sided Quad Helper for seamless non-culled angled runners:
            System.Func<string, Vector3, Vector3, Vector3, Vector3, GameObject> MakeDoubleSidedQuad = (name, p0, p1, p2, p3) =>
            {
                var go = new GameObject(name);
                if (Parent != null) go.transform.SetParent(Parent, false);
                var mf = go.AddComponent<MeshFilter>();
                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = pinkMat;

                var mesh = new Mesh { name = name };
                mesh.vertices = new Vector3[] { p0, p1, p2, p3 };
                mesh.uv = new Vector2[]
                {
                    new Vector2(p0.x * 0.5f, p0.z * 0.5f),
                    new Vector2(p1.x * 0.5f, p1.z * 0.5f),
                    new Vector2(p2.x * 0.5f, p2.z * 0.5f),
                    new Vector2(p3.x * 0.5f, p3.z * 0.5f)
                };
                mesh.normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                mesh.colors = new Color[] { Color.white, Color.white, Color.white, Color.white };
                mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2, 0, 1, 2, 0, 2, 3 };
                mesh.RecalculateTangents();
                mesh.RecalculateBounds();
                mf.sharedMesh = mesh;
                return go;
            };

            // World Space Coordinate Anchors for Stair Arrival & Smooth Elbow Turn:
            // Bottom step riser corners (flush against wooden riser):
            Vector3 pA = new Vector3(-0.123f, groundY + 0.001f, 9.333f);
            Vector3 pB = new Vector3(-0.782f, groundY + 0.001f, 10.813f);
            // Arrival apron outer corners (facing into yard along descent direction):
            Vector3 pB_out = new Vector3(-2.244f, groundY + 0.001f, 10.162f);
            Vector3 pA_out = new Vector3(-1.585f, groundY + 0.001f, 8.682f);

            // Promenade north head connection points at Z = 6.50m:
            Vector3 pPromW = new Vector3(-0.985f, groundY, 6.500f);
            Vector3 pPromE = new Vector3( 0.985f, groundY, 6.500f);

            // 1C. Stair Arrival Apron Quad: Flat rectangular apron meeting bottom step riser A->B flush
            MakeDoubleSidedQuad("StairArrivalApronQuad", pA, pB, pB_out, pA_out);

            // 1D. Carpet Elbow Transition Quad: Smoothly curves the central promenade into the arrival apron
            MakeDoubleSidedQuad("CarpetElbowTransitionQuad", pPromW, pA_out, pA, pPromE);

            // -------------------------------------------------------------
            // CONTINUOUS PERIMETER GOLD PIPING (Zero internal lines, zero yellow under wood)
            // -------------------------------------------------------------
            float pipeWidth = 0.055f;
            float pipeHeight = 0.007f;

            System.Action<string, Vector3[]> drawBorder = (name, pts) =>
            {
                if (pts.Length < 2) return;
                for (int i = 0; i < pts.Length - 1; i++)
                {
                    Vector3 vA = pts[i];
                    Vector3 vB = pts[i + 1];
                    Vector3 dir = (vB - vA).normalized;
                    Vector3 center = (vA + vB) * 0.5f;
                    float len = Vector3.Distance(vA, vB) + pipeWidth;
                    Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

                    var beam = Box(name + "_" + i, center + Vector3.up * 0.008f, new Vector3(pipeWidth, pipeHeight, len), goldSilk);
                    beam.transform.localRotation = rot;
                }
            };

            // Perimeter gold piping: West branch up to bottom step, and East branch from bottom step
            // Note: pB -> pA along the bottom step riser is left open so the ceremonial red carpet meets the wooden riser cleanly with zero yellow under wood.
            drawBorder("CarpetPerimeterBorderWest", new[]{
                new Vector3(-0.985f, groundY, -18.000f),
                pPromW,
                pA_out,
                pB_out,
                pB
            });

            drawBorder("CarpetPerimeterBorderEast", new[]{
                pA,
                pPromE,
                new Vector3( 0.985f, groundY,   0.750f),
                new Vector3( 9.800f, groundY,   0.750f),
                new Vector3( 9.800f, groundY,  -1.010f),
                new Vector3( 0.985f, groundY,  -1.010f),
                new Vector3( 0.985f, groundY, -18.000f),
                new Vector3(-0.985f, groundY, -18.000f)
            });

            Parent = prevParent;
            foreach (var t in carpetGroup.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.isStatic = false;
            }
        }

        static GameObject CarpetQuad(string name, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Material mat)
        {
            var go = new GameObject(name);
            if (Parent != null) go.transform.SetParent(Parent, false);
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;

            var mesh = new Mesh { name = name };
            Vector3[] vertices = new Vector3[] { p0, p1, p2, p3 };
            Vector2[] uvs = new Vector2[]
            {
                new Vector2(p0.x * 0.5f, p0.z * 0.5f),
                new Vector2(p1.x * 0.5f, p1.z * 0.5f),
                new Vector2(p2.x * 0.5f, p2.z * 0.5f),
                new Vector2(p3.x * 0.5f, p3.z * 0.5f)
            };
            Vector3[] normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
            Color[] colors = new Color[] { Color.white, Color.white, Color.white, Color.white };
            int[] triangles = new int[] { 0, 2, 1, 0, 3, 2 };

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.colors = colors;
            mesh.triangles = triangles;
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            mf.sharedMesh = mesh;
            return go;
        }

        public static void Swag(float width, float drop, Material mat)
        {
            Grid("Gathered hanging wedding fabric", 48, 12, (u, v) => new Vector3(u * width, -Mathf.Sin(u * Mathf.PI) * drop - v * .32f, Mathf.Sin(u * Mathf.PI * 22) * .026f + Mathf.Sin(v * Mathf.PI) * .10f), mat, new Vector2(3, 1), true);
        }

        public static void Rosette(Vector3 p, float r)
        {
            for (int petal = 0; petal < 8; petal++)
            {
                float a = petal * Mathf.PI / 4;
                var rose = Ball("Gathered fabric rosette", p + new Vector3(Mathf.Cos(a) * r * .45f, Mathf.Sin(a) * r * .45f, 0), new Vector3(r * .75f, r * .9f, r * .5f), S.Pink);
                rose.transform.localRotation = Quaternion.Euler(0, 0, -a * Mathf.Rad2Deg);
            }
        }

        public static void DiningTable(Vector3 p)
        {
            var old = Parent;
            Parent = ModelGroup("Round banquet table set", p);

            // Sturdy wooden round banquet table
            Cylinder("Round banquet table", Vector3.up * .72f, .94f, .06f, S.Wood, 36);

            // Draped dusty rose cloth with scalloped drop
            Grid("Draped round tablecloth", 64, 14, (u, v) =>
            {
                float a = u * Mathf.PI * 2, r = v < .65f ? v / .65f * .98f : .98f + Mathf.Sin(a * 24) * .023f * (v - .65f) / .35f;
                return new Vector3(Mathf.Sin(a) * r, .79f - Mathf.Max(0, v - .65f) / .35f * .65f, Mathf.Cos(a) * r);
            }, S.Pink, new Vector2(4, 2), true);

            // Metal lazy susan turntable (Phan)
            Cylinder("Metal serving turntable", Vector3.up * .805f, .52f, .016f, S.Metal, 36);

            // Steaming central soup hotpot
            PropsBuilder.Pot(new Vector3(0, .83f, 0), .17f);

            // Plastic wedding tissue box
            Box("Wedding plastic tissue box", new Vector3(.28f, .835f, 0), new Vector3(.14f, .065f, .09f), S.Pink);

            // 6 Complete place settings (Porcelain bowls, tea cups, bamboo chopsticks, tumblers, beer cans, condiment saucers)
            for (int i = 0; i < 6; i++)
            {
                float a = i * Mathf.PI / 3;
                var place = new Vector3(Mathf.Sin(a) * .73f, .8f, Mathf.Cos(a) * .73f);
                Lathe("Porcelain plate", place, new[]{ new Vector2(0, 0), new Vector2(.09f, 0), new Vector2(.14f, .02f), new Vector2(.14f, .028f) }, S.Porcelain);
                PropsBuilder.Cup(place + new Vector3(.08f, .035f, 0));
                for (int j = 0; j < 2; j++)
                    Beam("Chopstick", place + new Vector3(-.13f - j * .013f, .027f, -.09f), place + new Vector3(-.13f - j * .013f, .027f, .12f), .004f, S.Wood);
                Lathe("Drinking tumbler", place + new Vector3(.15f, .01f, -.09f), new[]{ new Vector2(.027f, 0), new Vector2(.033f, .11f), new Vector2(.029f, .112f), new Vector2(.025f, .02f) }, S.Metal, 16);

                // Authentic Angkor / Tiger beer can
                var canMat = (i % 2 == 0) ? S.Plain("Angkor beer can", new Color(.85f, .18f, .16f), .55f) : S.Plain("Tiger beer can", new Color(.18f, .35f, .75f), .55f);
                Cylinder("Cold beer can", place + new Vector3(-.16f, .01f, -.08f), .027f, .095f, canMat, 12);

                // Condiment saucer with chili & lime
                var saucerPos = place + new Vector3(-.07f, .01f, .11f);
                Lathe("Ceramic condiment saucer", saucerPos, new[]{ new Vector2(0, 0), new Vector2(.035f, 0), new Vector2(.045f, .015f) }, S.Porcelain, 12);
                Ball("Fresh green lime wedge", saucerPos + Vector3.up * .016f, new Vector3(.025f, .015f, .018f), S.Leaf);
                Ball("Red bird's eye chili", saucerPos + new Vector3(.012f, .016f, .008f), new Vector3(.016f, .008f, .008f), S.Plain("Bird eye chili red", new Color(.78f, .14f, .16f), .25f));
            }

            var tableGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(tableGroup, addCollider: true);
        }
    }
}
