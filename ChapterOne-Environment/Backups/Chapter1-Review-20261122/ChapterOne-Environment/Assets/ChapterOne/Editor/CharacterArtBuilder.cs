// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// CharacterArtBuilder.cs — Authentic 2006 Cambodian Character Art & Modeling
//
// Creates culturally authentic Cambodian characters for a circa-2006 rural
// Battambang wedding:
// - Mother of the Bride: Biological mother, silk blouse, traditional Sampot, shawl
// - Groom: Royal golden Khmer wedding tunic & Sampot Chong Kben
// - Monks & Achar: Theravada Buddhist saffron robes, white ceremonial Achar
// - Guests: Relatives, aunties in silk, uncles, young cousins, banquet cooks
// - Player: First person body & hands
// ============================================================================

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne.Editor
{
    public static class CharacterArtBuilder
    {
        const string MatFolder = "Assets/ChapterOne/Gameplay/Materials/";
        const string MeshFolder = "Assets/ChapterOne/Gameplay/Meshes/";

        // ── Shared Materials ───────────────────────────────────────────────
        public static Material SkinTan, SkinFair, SkinElder;
        public static Material DarkHair, GrayHair, ShavedHead;
        public static Material KhmerGoldSilk, KhmerCrimsonSilk, KhmerEmeraldSilk, KhmerLotusSilk, KhmerIvorySilk, KhmerRoyalPurple;
        public static Material SaffronMonkRobe, AcharWhiteCeremonial;
        public static Material CharcoalTrousers, FormalBlackShoe, TraditionalSandal;
        public static Material CambodianKramaRed, CambodianKramaBlue;
        public static Material GoldJewelry, JadeBangle, JasmineFlower;
        public static Material FlashlightMetal, FlashlightGlass;
        public static Material Shirt, Pants, Skin, Hair, Shoe, Metal, Glass;

        public static void Prepare()
        {
            Directory.CreateDirectory(MatFolder);
            Directory.CreateDirectory(MeshFolder);

            // Skin tones (authentic Southeast Asian / Cambodian)
            SkinTan   = Mat("Skin Khmer Tan", new Color(0.72f, 0.51f, 0.38f), 0.28f);
            SkinFair  = Mat("Skin Khmer Fair", new Color(0.78f, 0.58f, 0.44f), 0.25f);
            SkinElder = Mat("Skin Khmer Elder", new Color(0.66f, 0.46f, 0.33f), 0.32f);
            Skin = SkinTan;

            // Hair
            DarkHair   = Mat("Hair Jet Black", new Color(0.025f, 0.020f, 0.018f), 0.4f);
            GrayHair   = Mat("Hair Khmer Elder", new Color(0.28f, 0.27f, 0.26f), 0.25f);
            ShavedHead = Mat("Head Monk Shaved", new Color(0.68f, 0.48f, 0.36f), 0.2f);
            Hair = DarkHair;

            // Authentic Khmer Wedding Silks (Sompot & Sbai)
            KhmerGoldSilk     = Mat("Khmer Silk Royal Gold", new Color(0.88f, 0.72f, 0.20f), 0.55f);
            KhmerCrimsonSilk  = Mat("Khmer Silk Crimson", new Color(0.70f, 0.16f, 0.22f), 0.5f);
            KhmerEmeraldSilk  = Mat("Khmer Silk Emerald", new Color(0.14f, 0.45f, 0.28f), 0.5f);
            KhmerLotusSilk    = Mat("Khmer Silk Lotus Pink", new Color(0.85f, 0.45f, 0.55f), 0.45f);
            KhmerIvorySilk    = Mat("Khmer Silk Ivory", new Color(0.92f, 0.90f, 0.84f), 0.4f);
            KhmerRoyalPurple  = Mat("Khmer Silk Royal Purple", new Color(0.44f, 0.18f, 0.48f), 0.5f);
            Shirt = KhmerIvorySilk;

            // Monastic & Ceremonial
            SaffronMonkRobe       = Mat("Monk Saffron Civara", new Color(0.86f, 0.48f, 0.08f), 0.18f);
            AcharWhiteCeremonial  = Mat("Achar White Ceremonial", new Color(0.95f, 0.95f, 0.93f), 0.3f);

            // Trousers & Footwear
            CharcoalTrousers   = Mat("Formal Trousers Charcoal", new Color(0.18f, 0.19f, 0.21f), 0.15f);
            FormalBlackShoe    = Mat("Formal Leather Shoes", new Color(0.04f, 0.04f, 0.04f), 0.5f);
            TraditionalSandal  = Mat("Traditional Sandals", new Color(0.24f, 0.16f, 0.11f), 0.12f);
            Pants = CharcoalTrousers;
            Shoe = FormalBlackShoe;

            // Cambodian Krama (checkered cotton cloth)
            CambodianKramaRed  = Mat("Cambodian Krama Red Pattern", new Color(0.78f, 0.22f, 0.22f), 0.15f);
            CambodianKramaBlue = Mat("Cambodian Krama Blue Pattern", new Color(0.22f, 0.38f, 0.62f), 0.15f);

            // Traditional Cambodian Jewelry & Details
            GoldJewelry    = Mat("Khmer Antique Gold", new Color(0.92f, 0.78f, 0.22f), 0.85f);
            JadeBangle     = Mat("Khmer Jade Bangle", new Color(0.22f, 0.62f, 0.42f), 0.75f);
            JasmineFlower  = Mat("Khmer Jasmine Garland", new Color(0.96f, 0.96f, 0.92f), 0.1f);

            // Props
            FlashlightMetal = Mat("Flashlight Black Metal", new Color(0.07f, 0.08f, 0.075f), 0.6f);
            FlashlightGlass = Mat("Flashlight Lens Glass", new Color(0.7f, 0.74f, 0.67f), 0.9f);
            Metal = FlashlightMetal;
            Glass = FlashlightGlass;
        }

        public static Material Mat(string name, Color color, float smoothness = 0.2f)
        {
            string path = MatFolder + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!mat)
            {
                mat = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = color;
            mat.SetFloat("_Glossiness", smoothness);
            mat.enableInstancing = true;
            return mat;
        }

        public static Transform Group(string name, Transform parent, Vector3 position)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.localPosition = position;
            return t;
        }

        public static Transform Part(string name, Transform parent, Vector3 position, Vector3 scale,
                                     Material material, PrimitiveType type = PrimitiveType.Capsule)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            if (material) go.GetComponent<MeshRenderer>().sharedMaterial = material;
            return go.transform;
        }

        static Transform Limb(string name, Transform parent, Vector3 position, Material upperMat, Material lowerMat, Material footMat, bool isLeg)
        {
            var joint = Group(name, parent, position);
            Part(name + " upper", joint, new Vector3(0, -0.20f, 0), new Vector3(isLeg ? 0.16f : 0.11f, 0.21f, isLeg ? 0.17f : 0.12f), upperMat);
            Part(name + " lower", joint, new Vector3(0, -0.56f, 0.015f), new Vector3(isLeg ? 0.13f : 0.085f, 0.19f, isLeg ? 0.14f : 0.09f), lowerMat);

            if (isLeg)
                Part("Shoe", joint, new Vector3(0, -0.82f, 0.075f), new Vector3(0.14f, 0.09f, 0.28f), footMat, PrimitiveType.Cube);
            else
                Part("Hand", joint, new Vector3(0, -0.77f, 0.025f), new Vector3(0.085f, 0.13f, 0.06f), lowerMat, PrimitiveType.Sphere);

            Merge(joint);
            return joint;
        }

        public static Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var result = FindChildRecursive(parent.GetChild(i), name);
                if (result != null) return result;
            }
            return null;
        }

        // ================================================================
        // 1. MOTHER OF THE BRIDE (BIOLOGICAL MOTHER)
        // ================================================================
        public static MotherNPC KhmerMother(Transform root)
        {
            var mother = root.gameObject.AddComponent<MotherNPC>();
            mother.displayName = "Mother";

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ChapterOne/Characters/Mother/Female_Adult_01.fbx");
            if (prefab != null)
            {
                var instance = Object.Instantiate(prefab, root);
                instance.name = "Realistic Humanoid Mother";
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;

                // Setup authentic materials
                var smr = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    var bodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/MotherBody.mat");
                    if (!bodyMat)
                    {
                        bodyMat = new Material(Shader.Find("Standard"));
                        var bodyTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_body_color.tga");
                        var bodyNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_body_normal.tga");
                        if (bodyTex) bodyMat.mainTexture = bodyTex;
                        if (bodyNorm) { bodyMat.SetTexture("_BumpMap", bodyNorm); bodyMat.EnableKeyword("_NORMALMAP"); }
                        bodyMat.color = new Color(0.92f, 0.88f, 0.84f);
                        AssetDatabase.CreateAsset(bodyMat, "Assets/ChapterOne/Gameplay/Materials/MotherBody.mat");
                    }

                    var headMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/MotherHead.mat");
                    if (!headMat)
                    {
                        headMat = new Material(Shader.Find("Standard"));
                        var headTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_head_color.tga");
                        var headNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_head_normal.tga");
                        if (headTex) headMat.mainTexture = headTex;
                        if (headNorm) { headMat.SetTexture("_BumpMap", headNorm); headMat.EnableKeyword("_NORMALMAP"); }
                        headMat.color = new Color(0.92f, 0.88f, 0.84f);
                        AssetDatabase.CreateAsset(headMat, "Assets/ChapterOne/Gameplay/Materials/MotherHead.mat");
                    }

                    smr.sharedMaterials = new Material[] { bodyMat, headMat, headMat };
                }

                // Head tracking bone
                Transform headBone = FindChildRecursive(instance.transform, "Bip01 Head");
                mother.head = headBone;

                // Bones for natural human arm resting pose
                mother.lUpperArm = FindChildRecursive(instance.transform, "Bip01 L UpperArm");
                mother.rUpperArm = FindChildRecursive(instance.transform, "Bip01 R UpperArm");
                mother.lForearm = FindChildRecursive(instance.transform, "Bip01 L Forearm");
                mother.rForearm = FindChildRecursive(instance.transform, "Bip01 R Forearm");

                if (mother.lUpperArm) mother.lUpperArm.localEulerAngles = new Vector3(5.35f, 277.5f, 354.74f);
                if (mother.rUpperArm) mother.rUpperArm.localEulerAngles = new Vector3(354.65f, 82.5f, 354.74f);
                if (mother.lForearm) mother.lForearm.localEulerAngles = new Vector3(1.08f, 352.0f, 2.98f);
                if (mother.rForearm) mother.rForearm.localEulerAngles = new Vector3(358.92f, 8.0f, 2.98f);
            }

            var col = root.gameObject.GetComponent<CapsuleCollider>();
            if (!col) col = root.gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.72f; col.radius = 0.28f; col.center = Vector3.up * 0.86f;
            return mother;
        }

        // ================================================================
        // 2. THE GROOM (BRIDE'S HUSBAND)
        // ================================================================
        public static GroomNPC KhmerGroom(Transform root)
        {
            var groom = root.gameObject.AddComponent<GroomNPC>();
            groom.displayName = "Groom";

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ChapterOne/Characters/Groom/Male_Adult_01.fbx");
            if (prefab != null)
            {
                var instance = Object.Instantiate(prefab, root);
                instance.name = "Realistic Humanoid Groom";
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;

                var smr = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    var bodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/GroomBody.mat");
                    if (!bodyMat)
                    {
                        bodyMat = new Material(Shader.Find("Standard"));
                        var bodyTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Groom/m002_body_color.tga");
                        var bodyNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Groom/m002_body_normal.tga");
                        if (bodyTex) bodyMat.mainTexture = bodyTex;
                        if (bodyNorm) { bodyMat.SetTexture("_BumpMap", bodyNorm); bodyMat.EnableKeyword("_NORMALMAP"); }
                        bodyMat.color = new Color(0.92f, 0.88f, 0.84f);
                        AssetDatabase.CreateAsset(bodyMat, "Assets/ChapterOne/Gameplay/Materials/GroomBody.mat");
                    }

                    var headMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/GroomHead.mat");
                    if (!headMat)
                    {
                        headMat = new Material(Shader.Find("Standard"));
                        var headTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Groom/m002_head_color.tga");
                        var headNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Groom/m002_head_normal.tga");
                        if (headTex) headMat.mainTexture = headTex;
                        if (headNorm) { headMat.SetTexture("_BumpMap", headNorm); headMat.EnableKeyword("_NORMALMAP"); }
                        headMat.color = new Color(0.92f, 0.88f, 0.84f);
                        AssetDatabase.CreateAsset(headMat, "Assets/ChapterOne/Gameplay/Materials/GroomHead.mat");
                    }

                    smr.sharedMaterials = new Material[] { bodyMat, headMat, headMat };
                }

                Transform headBone = FindChildRecursive(instance.transform, "Bip01 Head");
                groom.head = headBone;

                // Arm resting posture bones
                groom.lUpperArm = FindChildRecursive(instance.transform, "Bip01 L UpperArm");
                groom.rUpperArm = FindChildRecursive(instance.transform, "Bip01 R UpperArm");
                groom.lForearm = FindChildRecursive(instance.transform, "Bip01 L Forearm");
                groom.rForearm = FindChildRecursive(instance.transform, "Bip01 R Forearm");

                if (groom.lUpperArm) groom.lUpperArm.localEulerAngles = new Vector3(5.35f, 277.5f, 354.74f);
                if (groom.rUpperArm) groom.rUpperArm.localEulerAngles = new Vector3(354.65f, 82.5f, 354.74f);
                if (groom.lForearm) groom.lForearm.localEulerAngles = new Vector3(1.08f, 352.0f, 2.98f);
                if (groom.rForearm) groom.rForearm.localEulerAngles = new Vector3(358.92f, 8.0f, 2.98f);
            }

            var col = root.gameObject.GetComponent<CapsuleCollider>();
            if (!col) col = root.gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.80f; col.radius = 0.30f; col.center = Vector3.up * 0.90f;
            return groom;
        }

        // ================================================================
        // 3. BUDDHIST MONK
        // ================================================================
        public static WeddingGuest BuddhistMonk(Transform root, string name)
        {
            var monk = root.gameObject.AddComponent<WeddingGuest>();
            monk.displayName = name;
            monk.role = "Venerable Monk";
            monk.greeting = "May the Triple Gem bless this auspicious union with peace, harmony, and long life.";
            monk.weddingAnswer = "The morning blessing chant brings merit and protection to this couple and their ancestors.";
            monk.helpAnswer = "Walk the middle path, speak with kindness, and always care for one another.";

            var robe = Group("Monk Robes", root, Vector3.zero);
            Part("Robe Torso", robe, new Vector3(0, 1.20f, 0), new Vector3(0.38f, 0.35f, 0.28f), SaffronMonkRobe);
            Part("Bare Right Shoulder", robe, new Vector3(0.18f, 1.45f, 0), new Vector3(0.12f, 0.15f, 0.12f), SkinTan);
            Part("Lower Robe", robe, new Vector3(0, 0.65f, 0), new Vector3(0.40f, 0.58f, 0.32f), SaffronMonkRobe);
            var fan = Part("Chanting Fan Handle", robe, new Vector3(-0.08f, 1.05f, 0.22f), new Vector3(0.02f, 0.32f, 0.02f), GoldJewelry, PrimitiveType.Cylinder);
            Part("Fan Palm Face", fan, new Vector3(0, 0.22f, 0), new Vector3(0.24f, 0.24f, 0.015f), KhmerCrimsonSilk, PrimitiveType.Cylinder);

            Merge(robe);

            Limb("Left Leg", root, new Vector3(-0.09f, 0.75f, 0), SaffronMonkRobe, SkinTan, TraditionalSandal, true);
            Limb("Right Leg", root, new Vector3(0.09f, 0.75f, 0), SaffronMonkRobe, SkinTan, TraditionalSandal, true);
            monk.leftArm = Limb("Left Arm", root, new Vector3(-0.24f, 1.40f, 0), SaffronMonkRobe, SkinTan, TraditionalSandal, false);
            monk.rightArm = Limb("Right Arm", root, new Vector3(0.24f, 1.40f, 0), SkinTan, SkinTan, TraditionalSandal, false);

            var head = Group("Head", root, new Vector3(0, 1.56f, 0));
            monk.head = head;
            Part("Neck", head, new Vector3(0, -0.055f, 0), new Vector3(0.11f, 0.10f, 0.11f), SkinTan);
            Part("Shaved Head", head, new Vector3(0, 0.12f, 0.015f), new Vector3(0.23f, 0.30f, 0.23f), ShavedHead, PrimitiveType.Sphere);
            Part("Serene Face", head, new Vector3(0, 0.11f, 0.13f), new Vector3(0.038f, 0.06f, 0.04f), SkinTan, PrimitiveType.Sphere);
            for (int side = -1; side <= 1; side += 2)
            {
                Part("Ear", head, new Vector3(side * 0.12f, 0.11f, 0), new Vector3(0.045f, 0.085f, 0.038f), SkinTan, PrimitiveType.Sphere);
                Part("Eye", head, new Vector3(side * 0.045f, 0.14f, 0.118f), new Vector3(0.024f, 0.010f, 0.012f), DarkHair, PrimitiveType.Sphere);
            }

            Merge(head);

            var col = root.gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.76f; col.radius = 0.28f; col.center = Vector3.up * 0.88f;
            return monk;
        }

        // ================================================================
        // 4. ACHAR (CEREMONIAL MASTER OF CEREMONIES)
        // ================================================================
        public static WeddingGuest KhmerAchar(Transform root, string name)
        {
            var achar = root.gameObject.AddComponent<WeddingGuest>();
            achar.displayName = name;
            achar.role = "Achar (Master of Ceremonies)";
            achar.greeting = "Everything is prepared according to ancient Khmer custom. The auspicious hour has arrived.";
            achar.weddingAnswer = "We shall begin with Gaat Sah (haircutting), followed by Sompeas Ptem (wrist-tying).";
            achar.helpAnswer = "Ensure the fruit offerings and betel trays are placed respectfully on the woven mats.";

            var suit = Group("Achar White Ceremonial Suit", root, Vector3.zero);
            Part("White Formal Jacket", suit, new Vector3(0, 1.24f, 0), new Vector3(0.40f, 0.32f, 0.25f), AcharWhiteCeremonial);
            Part("White Waist", suit, new Vector3(0, 0.90f, 0), new Vector3(0.34f, 0.18f, 0.23f), AcharWhiteCeremonial);
            var scarf = Part("White Ceremonial Scarf", suit, new Vector3(0, 1.35f, 0.08f), new Vector3(0.24f, 0.32f, 0.06f), AcharWhiteCeremonial, PrimitiveType.Cube);
            scarf.localEulerAngles = new Vector3(0, 0, 5);
            var tray = Part("Offering Bowl", suit, new Vector3(0, 1.05f, 0.25f), new Vector3(0.22f, 0.08f, 0.22f), GoldJewelry, PrimitiveType.Cylinder);
            Part("Flower Petals", tray, new Vector3(0, 0.05f, 0), new Vector3(0.18f, 0.04f, 0.18f), JasmineFlower, PrimitiveType.Sphere);

            Merge(suit);

            Limb("Left Leg", root, new Vector3(-0.10f, 0.82f, 0), AcharWhiteCeremonial, AcharWhiteCeremonial, TraditionalSandal, true);
            Limb("Right Leg", root, new Vector3(0.10f, 0.82f, 0), AcharWhiteCeremonial, AcharWhiteCeremonial, TraditionalSandal, true);
            achar.leftArm = Limb("Left Arm", root, new Vector3(-0.25f, 1.44f, 0), AcharWhiteCeremonial, SkinElder, TraditionalSandal, false);
            achar.rightArm = Limb("Right Arm", root, new Vector3(0.25f, 1.44f, 0), AcharWhiteCeremonial, SkinElder, TraditionalSandal, false);

            var head = Group("Head", root, new Vector3(0, 1.58f, 0));
            achar.head = head;
            Part("Neck", head, new Vector3(0, -0.055f, 0), new Vector3(0.11f, 0.09f, 0.11f), SkinElder);
            Part("Face", head, new Vector3(0, 0.11f, 0.015f), new Vector3(0.22f, 0.28f, 0.23f), SkinElder, PrimitiveType.Sphere);
            Part("Gray Hair", head, new Vector3(0, 0.21f, -0.01f), new Vector3(0.23f, 0.15f, 0.23f), GrayHair, PrimitiveType.Sphere);
            Part("Nose", head, new Vector3(0, 0.10f, 0.13f), new Vector3(0.04f, 0.06f, 0.04f), SkinElder, PrimitiveType.Sphere);
            for (int side = -1; side <= 1; side += 2)
            {
                Part("Ear", head, new Vector3(side * 0.12f, 0.11f, 0), new Vector3(0.04f, 0.075f, 0.035f), SkinElder, PrimitiveType.Sphere);
                Part("Eye", head, new Vector3(side * 0.045f, 0.14f, 0.118f), new Vector3(0.024f, 0.012f, 0.012f), DarkHair, PrimitiveType.Sphere);
            }

            Merge(head);

            var col = root.gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.76f; col.radius = 0.28f; col.center = Vector3.up * 0.88f;
            return achar;
        }

        // ================================================================
        // 5. CAMBODIAN WEDDING GUESTS (MALE & FEMALE VARIATIONS)
        // ================================================================

        public static WeddingGuest KhmerGuestFemale(Transform root, string name, string role, Material blouseMat, Material skirtMat, string greeting, string wedding, string help)
        {
            var guest = root.gameObject.AddComponent<WeddingGuest>();
            guest.displayName = name;
            guest.role = role;
            guest.greeting = greeting;
            guest.weddingAnswer = wedding;
            guest.helpAnswer = help;

            var torso = Group("Khmer Female Silk Attire", root, Vector3.zero);
            Part("Silk Blouse", torso, new Vector3(0, 1.20f, 0), new Vector3(0.36f, 0.28f, 0.23f), blouseMat);
            var shawl = Part("Silk Scarf", torso, new Vector3(-0.08f, 1.25f, 0.06f), new Vector3(0.12f, 0.30f, 0.06f), KhmerGoldSilk, PrimitiveType.Cube);
            shawl.localEulerAngles = new Vector3(0, 10, -20);
            Part("Sampot Skirt", torso, new Vector3(0, 0.72f, 0), new Vector3(0.35f, 0.44f, 0.25f), skirtMat);

            Merge(torso);

            Limb("Left Leg", root, new Vector3(-0.09f, 0.80f, 0), skirtMat, skirtMat, TraditionalSandal, true);
            Limb("Right Leg", root, new Vector3(0.09f, 0.80f, 0), skirtMat, skirtMat, TraditionalSandal, true);
            guest.leftArm = Limb("Left Arm", root, new Vector3(-0.23f, 1.42f, 0), blouseMat, SkinFair, TraditionalSandal, false);
            guest.rightArm = Limb("Right Arm", root, new Vector3(0.23f, 1.42f, 0), blouseMat, SkinFair, TraditionalSandal, false);

            var head = Group("Head", root, new Vector3(0, 1.54f, 0));
            guest.head = head;
            Part("Neck", head, new Vector3(0, -0.055f, 0), new Vector3(0.10f, 0.09f, 0.10f), SkinFair);
            Part("Face", head, new Vector3(0, 0.11f, 0.015f), new Vector3(0.21f, 0.27f, 0.22f), SkinFair, PrimitiveType.Sphere);
            Part("Hair Bun", head, new Vector3(0, 0.21f, -0.02f), new Vector3(0.22f, 0.16f, 0.22f), DarkHair, PrimitiveType.Sphere);
            Part("Nose", head, new Vector3(0, 0.10f, 0.125f), new Vector3(0.035f, 0.055f, 0.038f), SkinFair, PrimitiveType.Sphere);
            for (int side = -1; side <= 1; side += 2)
            {
                Part("Earring", head, new Vector3(side * 0.11f, 0.08f, 0), Vector3.one * 0.018f, GoldJewelry, PrimitiveType.Sphere);
                Part("Eye", head, new Vector3(side * 0.042f, 0.135f, 0.112f), new Vector3(0.024f, 0.011f, 0.012f), DarkHair, PrimitiveType.Sphere);
            }

            Merge(head);

            var col = root.gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.70f; col.radius = 0.27f; col.center = Vector3.up * 0.85f;
            return guest;
        }

        public static WeddingGuest KhmerGuestMale(Transform root, string name, string role, Material shirtMat, bool hasKrama, string greeting, string wedding, string help)
        {
            var guest = root.gameObject.AddComponent<WeddingGuest>();
            guest.displayName = name;
            guest.role = role;
            guest.greeting = greeting;
            guest.weddingAnswer = wedding;
            guest.helpAnswer = help;

            var torso = Group("Formal Shirt and Trousers", root, Vector3.zero);
            Part("Formal Shirt", torso, new Vector3(0, 1.23f, 0), new Vector3(0.40f, 0.29f, 0.25f), shirtMat);
            Part("Waist", torso, new Vector3(0, 0.91f, 0), new Vector3(0.33f, 0.15f, 0.22f), CharcoalTrousers);

            if (hasKrama)
            {
                var krama = Part("Krama Scarf", torso, new Vector3(-0.06f, 1.25f, 0.08f), new Vector3(0.14f, 0.32f, 0.07f), CambodianKramaRed, PrimitiveType.Cube);
                krama.localEulerAngles = new Vector3(0, 10, -22);
            }

            Merge(torso);

            Limb("Left Leg", root, new Vector3(-0.105f, 0.88f, 0), CharcoalTrousers, CharcoalTrousers, FormalBlackShoe, true);
            Limb("Right Leg", root, new Vector3(0.105f, 0.88f, 0), CharcoalTrousers, CharcoalTrousers, FormalBlackShoe, true);
            guest.leftArm = Limb("Left Arm", root, new Vector3(-0.25f, 1.47f, 0), shirtMat, SkinTan, FormalBlackShoe, false);
            guest.rightArm = Limb("Right Arm", root, new Vector3(0.25f, 1.47f, 0), shirtMat, SkinTan, FormalBlackShoe, false);

            var head = Group("Head", root, new Vector3(0, 1.58f, 0));
            guest.head = head;
            Part("Neck", head, new Vector3(0, -0.055f, 0), new Vector3(0.11f, 0.10f, 0.11f), SkinTan);
            Part("Face", head, new Vector3(0, 0.12f, 0.015f), new Vector3(0.23f, 0.30f, 0.24f), SkinTan, PrimitiveType.Sphere);
            Part("Hair", head, new Vector3(0, 0.215f, -0.01f), new Vector3(0.236f, 0.15f, 0.235f), DarkHair, PrimitiveType.Sphere);
            Part("Nose", head, new Vector3(0, 0.11f, 0.135f), new Vector3(0.04f, 0.065f, 0.045f), SkinTan, PrimitiveType.Sphere);
            for (int side = -1; side <= 1; side += 2)
            {
                Part("Eye", head, new Vector3(side * 0.047f, 0.148f, 0.122f), new Vector3(0.027f, 0.012f, 0.012f), DarkHair, PrimitiveType.Sphere);
            }

            Merge(head);

            var col = root.gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.78f; col.radius = 0.29f; col.center = Vector3.up * 0.89f;
            return guest;
        }

        public static WeddingGuest KhmerCook(Transform root, string name)
        {
            var cook = root.gameObject.AddComponent<WeddingGuest>();
            cook.displayName = name;
            cook.role = "Village Banquet Cook";
            cook.greeting = "The beef Lok Lak and Samlor Kari are simmering! We've prepared enough for the whole village.";
            cook.weddingAnswer = "Feeding a hundred guests takes six wood stoves and three hours of continuous stirring.";
            cook.helpAnswer = "Careful around the boiling soup pots behind the stilt house.";

            var torso = Group("Cook Attire", root, Vector3.zero);
            Part("Light Cotton Shirt", torso, new Vector3(0, 1.22f, 0), new Vector3(0.40f, 0.28f, 0.26f), KhmerIvorySilk);
            Part("Rolled Trousers", torso, new Vector3(0, 0.90f, 0), new Vector3(0.33f, 0.15f, 0.23f), CharcoalTrousers);
            Part("Krama Apron", torso, new Vector3(0, 0.85f, 0.08f), new Vector3(0.36f, 0.24f, 0.08f), CambodianKramaBlue, PrimitiveType.Cube);

            Merge(torso);

            Limb("Left Leg", root, new Vector3(-0.10f, 0.85f, 0), CharcoalTrousers, SkinTan, TraditionalSandal, true);
            Limb("Right Leg", root, new Vector3(0.10f, 0.85f, 0), CharcoalTrousers, SkinTan, TraditionalSandal, true);
            cook.leftArm = Limb("Left Arm", root, new Vector3(-0.25f, 1.45f, 0), KhmerIvorySilk, SkinTan, TraditionalSandal, false);
            cook.rightArm = Limb("Right Arm", root, new Vector3(0.25f, 1.45f, 0), KhmerIvorySilk, SkinTan, TraditionalSandal, false);

            var head = Group("Head", root, new Vector3(0, 1.56f, 0));
            cook.head = head;
            Part("Neck", head, new Vector3(0, -0.055f, 0), new Vector3(0.11f, 0.10f, 0.11f), SkinTan);
            Part("Face", head, new Vector3(0, 0.12f, 0.015f), new Vector3(0.23f, 0.29f, 0.23f), SkinTan, PrimitiveType.Sphere);
            Part("Headband Krama", head, new Vector3(0, 0.18f, 0.02f), new Vector3(0.245f, 0.055f, 0.245f), CambodianKramaBlue, PrimitiveType.Cylinder);
            Part("Hair", head, new Vector3(0, 0.22f, -0.01f), new Vector3(0.235f, 0.14f, 0.235f), DarkHair, PrimitiveType.Sphere);

            Merge(head);

            var col = root.gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.76f; col.radius = 0.29f; col.center = Vector3.up * 0.88f;
            return cook;
        }

        // ================================================================
        // 6. BACKWARD COMPATIBLE GUEST & PLAYER HELPERS
        // ================================================================

        public static WeddingGuest Guest(Transform root, string name, Color shirtColor)
        {
            var mat = Mat(name + " Shirt", shirtColor);
            return KhmerGuestMale(root, name, "Wedding Guest", mat, false,
                "Congratulations on your wedding day!",
                "The wedding marquee looks wonderful.",
                "Feel free to look around and greet the relatives.");
        }

        public static FirstPersonBody Player(EnvironmentCamera camera, Transform root)
        {
            var light = camera.GetComponent<ForestExploration>()?.flashlight;
            if (light) light.transform.SetParent(camera.transform, true);
            var oldHands = camera.transform.Find("First person hands"); if (oldHands) Object.DestroyImmediate(oldHands.gameObject);
            var oldAnimatedHands = camera.transform.Find("First Person Animated Hands"); if (oldAnimatedHands) Object.DestroyImmediate(oldAnimatedHands.gameObject);

            var body = camera.GetComponent<FirstPersonBody>(); if (!body) body = camera.gameObject.AddComponent<FirstPersonBody>();
            
            // Full 3D humanoid character body for the Bride (Bopha)
            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var b in allObjects)
            {
                if (b != null && b.name == "Player visible body") Object.DestroyImmediate(b);
            }

            body.lowerBody = Group("Player visible body", root, Vector3.zero);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ChapterOne/Characters/Mother/Female_Adult_01.fbx");
            if (prefab != null)
            {
                var humanInstance = Object.Instantiate(prefab, body.lowerBody);
                humanInstance.name = "Bride Full 3D Human Character";
                humanInstance.transform.localPosition = Vector3.zero;
                humanInstance.transform.localRotation = Quaternion.identity;

                // Setup silk morning attire materials
                var smr = humanInstance.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    var brideBodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/BrideMorningBody.mat");
                    if (!brideBodyMat)
                    {
                        brideBodyMat = new Material(Shader.Find("Standard"));
                        var bodyTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_body_color.tga");
                        var bodyNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_body_normal.tga");
                        if (bodyTex) brideBodyMat.mainTexture = bodyTex;
                        if (bodyNorm) { brideBodyMat.SetTexture("_BumpMap", bodyNorm); brideBodyMat.EnableKeyword("_NORMALMAP"); }
                        brideBodyMat.color = new Color(0.96f, 0.92f, 0.88f);
                        AssetDatabase.CreateAsset(brideBodyMat, "Assets/ChapterOne/Gameplay/Materials/BrideMorningBody.mat");
                    }

                    var brideHeadMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/BrideHead.mat");
                    if (!brideHeadMat)
                    {
                        brideHeadMat = new Material(Shader.Find("Standard"));
                        var headTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_head_color.tga");
                        var headNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChapterOne/Characters/Mother/f001_head_normal.tga");
                        if (headTex) brideHeadMat.mainTexture = headTex;
                        if (headNorm) { brideHeadMat.SetTexture("_BumpMap", headNorm); brideHeadMat.EnableKeyword("_NORMALMAP"); }
                        brideHeadMat.color = new Color(0.96f, 0.92f, 0.88f);
                        AssetDatabase.CreateAsset(brideHeadMat, "Assets/ChapterOne/Gameplay/Materials/BrideHead.mat");
                    }

                    smr.sharedMaterials = new Material[] { brideBodyMat, brideHeadMat, brideHeadMat };
                }

                // Attach wedding jewelry: delicate gold bangle on right wrist
                Transform rWrist = FindChildRecursive(humanInstance.transform, "Bip01 R Hand");
                if (rWrist)
                {
                    var bangle = Part("Bridal Gold Bangle", rWrist, Vector3.zero, new Vector3(0.062f, 0.022f, 0.062f), GoldJewelry, PrimitiveType.Cylinder);
                    bangle.localEulerAngles = new Vector3(90, 0, 0);
                }
            }

            body.explorer = camera.GetComponent<ForestExploration>();
            return body;
        }

        static Transform Hand(Transform parent, string name, bool right, out Transform[] fingers)
        {
            var hand = Group(name, parent, Vector3.zero);
            Part("Palm", hand, Vector3.zero, new Vector3(0.09f, 0.035f, 0.10f), SkinFair, PrimitiveType.Sphere);
            fingers = new Transform[4];
            for (int i = 0; i < 4; i++)
            {
                var finger = Group("Finger " + (i + 1), hand, new Vector3((i - 1.5f) * 0.021f, 0, 0.04f)); fingers[i] = finger;
                float length = i == 3 ? 0.052f : i == 0 ? 0.062f : 0.072f;
                Part("Finger", finger, new Vector3(0, 0, length * 0.45f), new Vector3(0.017f, 0.016f, length), SkinFair, PrimitiveType.Sphere);
                Part("Fingertip", finger, new Vector3(0, -0.008f, length * 0.88f), new Vector3(0.016f, 0.02f, 0.022f), SkinFair, PrimitiveType.Sphere);
                Merge(finger); finger.localEulerAngles = new Vector3(right ? -72 : 18, 0, 0);
            }
            var thumb = Part("Thumb", hand, new Vector3(right ? -0.049f : 0.049f, -0.003f, 0.005f), new Vector3(0.025f, 0.023f, 0.064f), SkinFair, PrimitiveType.Sphere);
            thumb.localEulerAngles = new Vector3(22, right ? -35 : 35, 0); return hand;
        }

        static void Merge(Transform root)
        {
            ModelCombiner.CombineModel(root, false);
            var f = root.GetComponent<MeshFilter>();
            if (!f) return;
            string id = root.name.Replace(" ", "_") + "_" + root.GetInstanceID();
            AssetDatabase.CreateAsset(f.sharedMesh, AssetDatabase.GenerateUniqueAssetPath(MeshFolder + id + ".asset"));
        }
    }
}
