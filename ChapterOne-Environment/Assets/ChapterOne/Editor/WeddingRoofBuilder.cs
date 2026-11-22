using UnityEngine;
using static ThatNight.ChapterOne.Editor.MeshFactory;
using S = ThatNight.ChapterOne.Editor.SurfaceLibrary;

namespace ThatNight.ChapterOne.Editor
{
    public static class WeddingRoofBuilder
    {
        public static void Build()
        {
            EnsureMaterials();
            var wedding = GameObject.Find("02 WEDDING - full marquee with seating dance and catering");
            if (wedding != null && MeshFactory.Parent == null)
            {
                MeshFactory.Parent = wedding.transform;
            }
            // Front gables set to false so the magnificent mountain panorama is completely open and visible!
            Section("Left roof over guest seating", -22, -3, 1.5f, SiteLayout.EntranceSideFront, SiteLayout.WeddingRear, true, false, false);
            Section("Middle roof over entrance aisle", -3, 3, 1.9f, SiteLayout.RoofFront, SiteLayout.WeddingRear, false, false, true);
            Section("Right roof over dance and ceremonies", 3, 22, 1.5f, SiteLayout.EntranceSideFront, SiteLayout.WeddingRear, true, false, false);
        }

        public static void EnsureMaterials()
        {
            if (S.DarkWood == null) S.Prepare();
            if (S.DarkWood == null || S.DarkWood.shader == null || S.DarkWood.shader.name == "Standard")
            {
                S.DarkWood = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Dark hardwood.mat");
                S.Wood = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Weathered teak.mat");
                S.Cream = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Ivory wedding cloth.mat");
                S.Pink = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Dusty rose wedding cloth.mat");
                S.Bulb = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Warm incandescent glass.mat");
            }
        }

        public static void Section(string name, float left, float right, float rise, float front, float rear, bool frontValance = true, bool frontGable = false, bool isCenter = false)
        {
            var old = Parent;
            Parent = ModelGroup(name, Vector3.zero);
            float eave = SiteLayout.WeddingEave;
            if (frontGable)
                GableCurtain(left, right, rise, front, eave);

            int bands = right - left > 10 ? 8 : 4;
            for (int b = 0; b < bands; b++)
            {
                int band = b;
                Grid("Pitched wedding canvas panel", 12, 28, (u, v) =>
                {
                    float t = (band + u) / bands;
                    float ridge = 1 - Mathf.Abs(t * 2 - 1);
                    return new Vector3(Mathf.Lerp(left, right, t), eave + ridge * rise -
                        Mathf.Sin(u * Mathf.PI) * .045f - Mathf.Sin(v * Mathf.PI * 4) * .035f, Mathf.Lerp(front, rear, v));
                }, band % 2 == 0 ? S.Cream : S.Pink, new Vector2(1, 8), true);
            }

            // Unified structural bay grid (exact 5.25m bays from -10.5m to +10.5m + entrance porch at -14.0m)
            var zBays = new System.Collections.Generic.List<float>();
            if (isCenter) zBays.Add(-14.0f);
            float[] sharedBays = { -10.5f, -5.25f, 0.0f, 5.25f, 10.5f };
            foreach (var bz in sharedBays)
            {
                if (bz >= front - 0.01f && bz <= rear + 0.01f)
                    zBays.Add(bz);
            }

            float mid = (left + right) / 2;
            for (int i = 0; i < zBays.Count; i++)
            {
                float z = zBays[i];

                // Single ownership of interior colonnade poles along the aisle (x = -3 and x = +3):
                if (!isCenter)
                {
                    // Side pavilions only spawn their outer boundary poles (x = -22 and x = +22)
                    float outerX = (left < -10f) ? left : right;
                    Cylinder("Marquee support pole", new Vector3(outerX, 0, z), .055f, eave, S.DarkWood);
                }
                else
                {
                    // Center aisle colonnade:
                    // Left pole at x = -3 (clean uniform 2m gap to red carpet)
                    Cylinder("Marquee support pole", new Vector3(left, 0, z), .055f, eave, S.DarkWood);

                    // Right pole at x = +3:
                    // Omit at z = 0.0m to leave cross-walkway into music ensemble 100% open
                    // Support pole at z = 10.5m is present to fully support the marquee roof
                    bool isCrossWalkway = Mathf.Abs(z - 0.0f) < 0.1f;
                    if (!isCrossWalkway)
                    {
                        Cylinder("Marquee support pole", new Vector3(right, 0, z), .055f, eave, S.DarkWood);
                    }
                }

                Beam("Roof frame left pitch", new Vector3(left, eave - .08f, z), new Vector3(mid, eave + rise - .08f, z), .065f, S.DarkWood);
                Beam("Roof frame right pitch", new Vector3(mid, eave + rise - .08f, z), new Vector3(right, eave - .08f, z), .065f, S.DarkWood);
                if (i > 0 && i < zBays.Count - 1) LightingBuilder.Bulb(new Vector3(mid, eave + rise - .4f, z), 10, 3.2f, false);
            }

            Beam("Marquee ridge beam", new Vector3((left + right) / 2, eave + rise - .07f, front), new Vector3((left + right) / 2, eave + rise - .07f, rear), .065f, S.DarkWood);

            // Festoon fairy string lights along marquee eaves
            float eaveLen = rear - front;
            int festoonCount = Mathf.RoundToInt(eaveLen / 1.5f);
            for (int f = 0; f < festoonCount; f++)
            {
                float fz = front + (f + 0.5f) * (eaveLen / festoonCount);
                foreach (float fx in new[] { left, right })
                {
                    float sag = Mathf.Sin((f % 2 == 0 ? 0.35f : 0.65f) * Mathf.PI) * 0.08f;
                    Ball("Festoon warm fairy bulb", new Vector3(fx, eave - 0.12f - sag, fz), Vector3.one * 0.055f, S.Bulb);
                }
            }

            foreach (float z in new[] { front, rear })
            {
                if (z == front && !frontValance) continue;
                var save = Parent;
                Parent = Group("Roof edge fabric valance", new Vector3(left, eave, z));
                WeddingYardBuilder.Swag((right - left) / 2, .45f, S.Pink);
                Parent = Group("Second valance span", new Vector3((right - left) / 2, 0, 0));
                WeddingYardBuilder.Swag((right - left) / 2, .45f, S.Pink);
                Parent = save;
            }

            var sectionGroup = Parent;
            Parent = old;
            ModelCombiner.CombineModel(sectionGroup, addCollider: false);
        }

        static void GableCurtain(float left, float right, float rise, float front, float eave)
        {
            var cloth = Grid("Pleated ivory gable curtain", 320, 24, (u, v) =>
            {
                float ridge = (1 - Mathf.Abs(u * 2 - 1)) * rise;
                float fold = Mathf.Sin(u * Mathf.PI * 140) * Mathf.Lerp(.065f, .018f, v);
                float billow = Mathf.Sin(Mathf.Repeat(u * 5, 1) * Mathf.PI) * Mathf.Sin(v * Mathf.PI) * .09f;
                float y = Mathf.Lerp(eave - .12f, eave + ridge, v);
                return new Vector3(Mathf.Lerp(left, right, u), y, front + .05f + fold + billow);
            }, S.Cream, Vector2.one, true);

            var mesh = cloth.GetComponent<MeshFilter>().sharedMesh;
            var vertices = mesh.vertices; var uv = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                uv[i] = new Vector2((vertices[i].x - left) * 15 / (right - left), (vertices[i].y - eave) * 3 / eave);
            mesh.uv = uv; mesh.RecalculateTangents();
        }
    }
}
