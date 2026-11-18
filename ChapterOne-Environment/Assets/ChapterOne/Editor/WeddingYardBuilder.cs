using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    internal static class WeddingYardBuilder
    {
        public static Transform Build()
        {
            Parent = null;
            var root = Group("02 WEDDING - full marquee with seating dance and catering", Vector3.up * SiteLayout.HillElevation);
            Parent = root;

            // 1. Open-air Marquee Canopy
            WeddingRoofBuilder.Build();

            // 2. Organic Mountain-View Guest Banquet Seating Layout
            var seatingZone = Group("Left and right guest banquet seating zones", Vector3.zero);
            var prevParent = Parent;
            Parent = seatingZone;

            // Left Seating Wing (12 Tables arranged in 3 gentle terraced curves overlooking the mountain valley)
            Vector3[] leftTablePositions = {
                // Front row overlooking the mountain slope
                new Vector3(-18.5f, 0, -8.5f), new Vector3(-14.2f, 0, -9.0f), new Vector3(-9.8f, 0, -8.8f), new Vector3(-5.5f, 0, -8.2f),
                // Mid row
                new Vector3(-18.8f, 0, -3.8f), new Vector3(-14.4f, 0, -4.2f), new Vector3(-10.0f, 0, -4.0f), new Vector3(-5.6f, 0, -3.5f),
                // Rear row near the stilt house
                new Vector3(-18.5f, 0, 1.2f), new Vector3(-14.2f, 0, 0.8f), new Vector3(-9.8f, 0, 1.0f), new Vector3(-5.5f, 0, 1.5f)
            };

            foreach (var pos in leftTablePositions)
            {
                DiningTable(pos);
                for (int chair = 0; chair < 6; chair++)
                {
                    float a = chair * Mathf.PI / 3 + .15f;
                    PropsBuilder.Chair(pos + new Vector3(Mathf.Sin(a) * 1.26f, 0, Mathf.Cos(a) * 1.26f), a * Mathf.Rad2Deg + 180);
                }
            }

            // Central / Family Tables (3 Tables near the central aisle, leaving the right pavilion clear for Gaat Sah)
            Vector3[] familyTablePositions = {
                new Vector3(5.5f, 0, -8.2f), new Vector3(5.6f, 0, -3.5f), new Vector3(5.5f, 0, 1.5f)
            };

            foreach (var pos in familyTablePositions)
            {
                DiningTable(pos);
                for (int chair = 0; chair < 6; chair++)
                {
                    float a = chair * Mathf.PI / 3 + .25f;
                    PropsBuilder.Chair(pos + new Vector3(Mathf.Sin(a) * 1.26f, 0, Mathf.Cos(a) * 1.26f), a * Mathf.Rad2Deg + 180);
                }
            }

            Parent = prevParent;

            // 3. Right Side Traditional Cambodian Hair Cutting Ceremony (Gaat Sah)
            WeddingEventBuilder.Build();

            // 4. Authentic Cambodian Wedding Entrance Gate (Khlok Pka Sla)
            WeddingPhotoCornerBuilder.Build();

            // 5. Catering Kitchen Corner
            WeddingCateringBuilder.Build();

            Parent = null;
            return root;
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

        static void DiningTable(Vector3 p)
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

            // 6 Complete place settings (Porcelain bowls, tea cups, bamboo chopsticks, tumblers)
            for (int i = 0; i < 6; i++)
            {
                float a = i * Mathf.PI / 3;
                var place = new Vector3(Mathf.Sin(a) * .73f, .8f, Mathf.Cos(a) * .73f);
                Lathe("Porcelain plate", place, new[]{ new Vector2(0, 0), new Vector2(.09f, 0), new Vector2(.14f, .02f), new Vector2(.14f, .028f) }, S.Porcelain);
                PropsBuilder.Cup(place + new Vector3(.08f, .035f, 0));
                for (int j = 0; j < 2; j++)
                    Beam("Chopstick", place + new Vector3(-.13f - j * .013f, .027f, -.09f), place + new Vector3(-.13f - j * .013f, .027f, .12f), .004f, S.Wood);
                Lathe("Drinking tumbler", place + new Vector3(.15f, .01f, -.09f), new[]{ new Vector2(.027f, 0), new Vector2(.033f, .11f), new Vector2(.029f, .112f), new Vector2(.025f, .02f) }, S.Metal, 16);
            }

            var tableGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(tableGroup, addCollider: true);
        }
    }
}
