// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// ChapterOneDirectorBuilder.cs — Master Chapter 1 Scene Builder
//
// Cleanly configures and wires all Chapter 1 systems, characters, audio,
// and gameplay triggers in the Unity scene for an authentic 2006 Khmer Wedding.
// ============================================================================

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThatNight.ChapterOne.Editor
{
    public static class ChapterOneDirectorBuilder
    {
        const string RootName = "12 CHAPTER ONE STORY DIRECTOR";

        [MenuItem("THAT NIGHT/Chapter 1/Setup Complete Chapter 1 Flow & Characters")]
        public static void Setup()
        {
            if (EditorApplication.isPlaying)
                throw new System.InvalidOperationException("Stop Play before setting up Chapter 1.");

            var camera = Object.FindFirstObjectByType<EnvironmentCamera>();
            if (!camera)
                throw new System.InvalidOperationException("Environment Camera not found in the scene.");

            CharacterArtBuilder.Prepare();

            // 1. Ensure First-Person interactions
            camera.tag = "MainCamera";
            var player = camera.GetComponent<PlayerInteraction>();
            if (!player) player = camera.gameObject.AddComponent<PlayerInteraction>();
            player.explorer = camera.GetComponent<ForestExploration>();
            GameplayOpeningBuilder.Apply();

            // First-Person Animated Hands & Eyelid Overlay
            FirstPersonHandsBuilder.Build(camera.GetComponent<Camera>());
            if (!camera.GetComponent<EyelidOverlay>())
                camera.gameObject.AddComponent<EyelidOverlay>();

            // Ambience Controller
            var ambience = camera.GetComponent<AmbienceController>();
            if (!ambience)
                ambience = camera.gameObject.AddComponent<AmbienceController>();

            // 2. Setup or clean Chapter 1 Root
            var existing = GameObject.Find(RootName);
            if (existing)
            {
                // Clean existing characters inside root to avoid duplicates
                var oldMother = existing.transform.Find("Mother - Biological Mother");
                if (oldMother) Object.DestroyImmediate(oldMother.gameObject);
                var oldGroom = existing.transform.Find("Groom - Husband");
                if (oldGroom) Object.DestroyImmediate(oldGroom.gameObject);
                var oldGuests = existing.transform.Find("Khmer Wedding Community");
                if (oldGuests) Object.DestroyImmediate(oldGuests.gameObject);
            }
            Transform root = existing ? existing.transform : new GameObject(RootName).transform;

            // Full 3D humanoid body for the Bride player
            player.body = CharacterArtBuilder.Player(camera, root);

            // 3. Mother of the Bride (Biological Mother) in Living Room - standing naturally in open living room facing bedroom door
            Vector3 motherLocalPos = new Vector3(-2.2f, 2.5f, -1.2f);
            Vector3 motherWorldPos = SiteLayout.HousePoint(motherLocalPos);
            var motherObj = CharacterArtBuilder.Group("Mother - Biological Mother", root, motherWorldPos);
            motherObj.rotation = SiteLayout.HouseRotation * Quaternion.Euler(0, -42f, 0);
            var mother = CharacterArtBuilder.KhmerMother(motherObj);

            // 4. The Groom (Husband) in Wedding Marquee / Ceremony Area
            Vector3 groomPos = new Vector3(0.5f, SiteLayout.HillElevation + 0.1f, -1.5f);
            var groomObj = CharacterArtBuilder.Group("Groom - Husband", root, groomPos);
            groomObj.rotation = Quaternion.Euler(0, 180f, 0);
            var groom = CharacterArtBuilder.KhmerGroom(groomObj);

            // 5. Authentic 2006 Khmer Wedding Community (Monks, Elders, Guests, Cooks)
            var community = CharacterArtBuilder.Group("Khmer Wedding Community", root, Vector3.zero);

            // A. Venerable Buddhist Monk in traditional ceremony blessing bay
            var monkObj = CharacterArtBuilder.Group("Venerable Monk - Bhikkhu Saman", community, new Vector3(4.5f, SiteLayout.HillElevation + 0.1f, -2.0f));
            monkObj.rotation = Quaternion.Euler(0, -90f, 0);
            CharacterArtBuilder.BuddhistMonk(monkObj, "Venerable Monk");

            // B. Achar (Master of Ceremonies)
            var acharObj = CharacterArtBuilder.Group("Achar - Om Seng (Ceremony Master)", community, new Vector3(2.5f, SiteLayout.HillElevation + 0.1f, -1.8f));
            acharObj.rotation = Quaternion.Euler(0, 160f, 0);
            CharacterArtBuilder.KhmerAchar(acharObj, "Achar Om Seng");

            // C. Auntie Navy in Royal Purple Silk (Marquee Seating)
            var auntNavy = CharacterArtBuilder.Group("Auntie Navy - Relative", community, new Vector3(-5.2f, SiteLayout.HillElevation + 0.1f, -6.5f));
            auntNavy.rotation = Quaternion.Euler(0, 45f, 0);
            CharacterArtBuilder.KhmerGuestFemale(auntNavy, "Auntie Navy", "Bride's Aunt",
                CharacterArtBuilder.KhmerRoyalPurple, CharacterArtBuilder.KhmerCrimsonSilk,
                "Sweetheart! You look as radiant as an Apsara today.",
                "Your mother has worked so hard preparing the silk and offerings.",
                "Have some fruit and cold tea before the ceremony begins.");

            // D. Auntie Sophal in Emerald Silk (Marquee Seating)
            var auntSophal = CharacterArtBuilder.Group("Auntie Sophal - Village Elder", community, new Vector3(-7.5f, SiteLayout.HillElevation + 0.1f, -10.0f));
            auntSophal.rotation = Quaternion.Euler(0, 70f, 0);
            CharacterArtBuilder.KhmerGuestFemale(auntSophal, "Auntie Sophal", "Village Elder",
                CharacterArtBuilder.KhmerEmeraldSilk, CharacterArtBuilder.KhmerGoldSilk,
                "Congratulations to the whole family! A blessed day for the village.",
                "The wedding marquee is the most beautiful we have seen this dry season.",
                "May your marriage bring five blessings: long life, beauty, happiness, strength, and wisdom.");

            // E. Uncle Chhay in Formal Linen & Red Krama (Marquee Entrance)
            var uncleChhay = CharacterArtBuilder.Group("Uncle Chhay - Family Elder", community, new Vector3(1.5f, SiteLayout.HillElevation + 0.1f, -12.5f));
            uncleChhay.rotation = Quaternion.Euler(0, 195f, 0);
            CharacterArtBuilder.KhmerGuestMale(uncleChhay, "Uncle Chhay", "Family Elder",
                CharacterArtBuilder.KhmerIvorySilk, true,
                "Welcome, nieces and nephews! Today we celebrate our family.",
                "The groom's family has arrived with generous fruit and cake offerings.",
                "I will oversee the guest seating and greeting at the front arch.");

            // F. Cousin Bopha in Lotus Pink (Bridesmaid / Helper)
            var cousinBopha = CharacterArtBuilder.Group("Bopha - Cousin & Bridesmaid", community, new Vector3(-1.5f, SiteLayout.HillElevation + 0.1f, 3.5f));
            cousinBopha.rotation = Quaternion.Euler(0, -35f, 0);
            CharacterArtBuilder.KhmerGuestFemale(cousinBopha, "Bopha", "Cousin & Bridesmaid",
                CharacterArtBuilder.KhmerLotusSilk, CharacterArtBuilder.KhmerGoldSilk,
                "Everything is so exciting! We are almost ready for the procession.",
                "Your wedding dress and jewelry are the most gorgeous in the province.",
                "Call me if you need help fixing your hair or adjusting your Sbai.");

            // G. Banquet Cooks at Outdoor Catering Station (behind house / marquee)
            var cook1 = CharacterArtBuilder.Group("Cook Seyha - Feast Chef", community, new Vector3(14.5f, SiteLayout.HillElevation + 0.1f, 15.0f));
            cook1.rotation = Quaternion.Euler(0, 220f, 0);
            CharacterArtBuilder.KhmerCook(cook1, "Cook Seyha");

            var cook2 = CharacterArtBuilder.Group("Cook Makara - Assistant Chef", community, new Vector3(16.2f, SiteLayout.HillElevation + 0.1f, 18.5f));
            cook2.rotation = Quaternion.Euler(0, 175f, 0);
            CharacterArtBuilder.KhmerCook(cook2, "Cook Makara");

            // 6. Setup Master ChapterOneDirector on Camera
            var director = camera.GetComponent<ChapterOneDirector>();
            if (!director) director = camera.gameObject.AddComponent<ChapterOneDirector>();

            director.envCamera = camera;
            director.explorer = camera.GetComponent<ForestExploration>();
            director.player = player;
            director.lighting = camera.lighting;
            director.narrative = camera.GetComponent<ChapterOneNarrative>();
            director.eyelids = camera.GetComponent<EyelidOverlay>();
            director.ambience = ambience;
            director.mother = mother;
            director.groom = groom;

            // 6. Setup Nokia Phone in the Bridal Bedroom beside the Bed
            // Remove any secondary / duplicate Nokia phone in other rooms
            var allPhones = Object.FindObjectsByType<NokiaHandPhone>(FindObjectsSortMode.None);
            NokiaHandPhone activePhone = null;

            // Bedside nightstand position in Bridal Bedroom 1 (right next to bed/pillow)
            Vector3 bedsidePhonePos = new Vector3(-8.60f, 24.16f, 23.90f);
            Quaternion bedsidePhoneRot = Quaternion.Euler(0, 140f, 0);

            if (allPhones.Length > 0)
            {
                activePhone = allPhones[0];
                for (int i = 1; i < allPhones.Length; i++)
                {
                    Object.DestroyImmediate(allPhones[i].gameObject);
                }
            }
            else
            {
                var phoneObj = new GameObject("2006 classic button mobile phone");
                activePhone = phoneObj.AddComponent<NokiaHandPhone>();
            }

            activePhone.name = "2006 classic button mobile phone (Bridal Bedroom)";
            activePhone.transform.SetPositionAndRotation(bedsidePhonePos, bedsidePhoneRot);

            // Ensure a generous interaction collider on the Nokia phone
            var phoneCol = activePhone.GetComponent<BoxCollider>();
            if (!phoneCol) phoneCol = activePhone.gameObject.AddComponent<BoxCollider>();
            phoneCol.size = new Vector3(0.35f, 0.25f, 0.35f);
            phoneCol.center = Vector3.zero;

            director.nokia = activePhone;

            // Setup Nokia audio clips
            if (activePhone)
            {
                activePhone.ringtoneClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NokiaRingtone2006.wav");
                activePhone.vibrationClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NokiaVibration.wav");
                activePhone.callEndedClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NokiaCallEnded.wav");
            }

            // Setup Director SFX clips
            director.clipRooster = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/DistantRooster.wav");
            director.clipClockTick = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/ClockTick.wav");
            director.clipRumble = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/DeepRumble.wav");
            director.clipScream = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/Scream.wav");

            // Setup AmbienceController clips
            ambience.dawnAmbience = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Forest/Audio/ForestWind.wav");
            ambience.horrorAmbience = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Forest/Audio/NightInsects.wav");

            // 7. Align Bride in Bedroom at 4:00 AM Dawn
            Vector3 bedroomPos = SiteLayout.HousePoint(new Vector3(-3.45f, 2.5f, 4.2f));
            Quaternion bedroomRot = SiteLayout.HouseRotation * Quaternion.Euler(0, 160f, 0);

            if (player.arrival != null)
            {
                player.arrival.SetPositionAndRotation(bedroomPos, bedroomRot);
            }
            camera.transform.SetPositionAndRotation(bedroomPos, bedroomRot);
            camera.lighting.ApplyStage(AtmosphereStage.Dawn4AM);

            // Mark bridal bedroom door to trigger director
            foreach (var hinge in Object.FindObjectsByType<HingedOpening>(FindObjectsSortMode.None))
            {
                if (hinge.displayName != null && hinge.displayName.ToLowerInvariant().Contains("bridal"))
                {
                    hinge.triggerDirectorOnOpen = true;
                }
            }

            // 8. Solid Doors & Window Security Bars
            SetupWindowsAndDoors();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();

            Debug.Log("CHAPTER_ONE_KHMER_WEDDING_SETUP_COMPLETE: All characters, authentic 2006 Khmer silks, Monks, Achar, Mother, Groom, Guests, Nokia, and Director wired successfully!");
        }

        static void SetupWindowsAndDoors()
        {
            var house = GameObject.Find("01 HOUSE - raised timber home");
            if (!house) return;

            var darkWood = AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Materials/Dark hardwood.mat")
                        ?? AssetDatabase.LoadAssetAtPath<Material>("Assets/ChapterOne/Gameplay/Materials/DarkWood.mat");

            // 1. Bedroom doors: Ensure clean single door leaf (NO duplicate backing panel) and tightly closed
            foreach (var dName in new[] { "Bedroom door bridal", "Bedroom door family" })
            {
                var door = GameObject.Find(dName);
                if (door)
                {
                    door.transform.localRotation = Quaternion.identity;
                    var hinge = door.GetComponent<HingedOpening>();
                    if (hinge)
                    {
                        hinge.closedAngle = 0f;
                        // Swings INWARD into bedroom (+Z): -90 for bridal (leaf extends +X), +90 for family (leaf extends -X)
                        hinge.openAngle = (dName == "Bedroom door bridal") ? -90f : 90f;
                        hinge.isOpen = false;
                    }

                    // Remove any duplicate backing panel so only the authored carved door exists
                    var oldBacking = door.transform.Find("Solid Door Backing Panel");
                    if (oldBacking) Object.DestroyImmediate(oldBacking.gameObject);
                }
            }

            // 2. Windows: Clean up any duplicate procedural bar objects (bars are already baked into wall mesh)
            foreach (Transform wall in house.transform)
            {
                if (!wall.name.Contains("Vertical plank wall")) continue;
                for (int winIdx = -1; winIdx <= 1; winIdx += 2)
                {
                    string barsName = "Window " + winIdx + " Security Bars";
                    var existingBars = wall.Find(barsName);
                    if (existingBars) Object.DestroyImmediate(existingBars.gameObject);
                }
            }

            // 3. Ensure Front Entrance Doors have solid lintel, top cover transom canopy, and overlapping center astragal batten
            var frontDoors = GameObject.Find("Double wooden entrance doors");
            if (frontDoors)
            {
                var existingTopCover = frontDoors.transform.Find("Entrance Top Cover Transom Canopy");
                if (existingTopCover) Object.DestroyImmediate(existingTopCover.gameObject);

                var topCover = GameObject.CreatePrimitive(PrimitiveType.Cube);
                topCover.name = "Entrance Top Cover Transom Canopy";
                topCover.transform.SetParent(frontDoors.transform, false);
                topCover.transform.localPosition = new Vector3(0f, 2.50f, 0f);
                topCover.transform.localScale = new Vector3(1.28f, 0.48f, 0.18f);
                topCover.GetComponent<Renderer>().sharedMaterial = darkWood;

                if (!frontDoors.transform.Find("Entrance Door Frame Lintel"))
                {
                    var lintel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    lintel.name = "Entrance Door Frame Lintel";
                    lintel.transform.SetParent(frontDoors.transform, false);
                    lintel.transform.localPosition = new Vector3(0f, 2.30f, 0f);
                    lintel.transform.localScale = new Vector3(1.28f, 0.08f, 0.16f);
                    lintel.GetComponent<Renderer>().sharedMaterial = darkWood;

                    var jambL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    jambL.name = "Entrance Door Frame Jamb Left";
                    jambL.transform.SetParent(frontDoors.transform, false);
                    jambL.transform.localPosition = new Vector3(-0.58f, 1.15f, 0f);
                    jambL.transform.localScale = new Vector3(0.08f, 2.26f, 0.14f);
                    jambL.GetComponent<Renderer>().sharedMaterial = darkWood;

                    var jambR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    jambR.name = "Entrance Door Frame Jamb Right";
                    jambR.transform.SetParent(frontDoors.transform, false);
                    jambR.transform.localPosition = new Vector3(0.58f, 1.15f, 0f);
                    jambR.transform.localScale = new Vector3(0.08f, 2.26f, 0.14f);
                    jambR.GetComponent<Renderer>().sharedMaterial = darkWood;

                    var sill = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    sill.name = "Entrance Door Threshold Sill";
                    sill.transform.SetParent(frontDoors.transform, false);
                    sill.transform.localPosition = new Vector3(0f, 0.04f, 0f);
                    sill.transform.localScale = new Vector3(1.28f, 0.08f, 0.18f);
                    sill.GetComponent<Renderer>().sharedMaterial = darkWood;

                    var canopy = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    canopy.name = "Entrance Door Top Canopy Moulding";
                    canopy.transform.SetParent(frontDoors.transform, false);
                    canopy.transform.localPosition = new Vector3(0f, 2.36f, -0.06f);
                    canopy.transform.localScale = new Vector3(1.32f, 0.06f, 0.22f);
                    canopy.GetComponent<Renderer>().sharedMaterial = darkWood;
                }

                var leftD = frontDoors.transform.Find("Entrance door left");
                if (leftD)
                {
                    leftD.localPosition = new Vector3(-0.54f, 1.18f, 0f);
                    leftD.localRotation = Quaternion.identity;
                    var hingeL = leftD.GetComponent<HingedOpening>();
                    // Left door swings INWARD into living room (+Z)
                    if (hingeL) { hingeL.closedAngle = 0f; hingeL.openAngle = -90f; hingeL.isOpen = false; }

                    var oldPL = leftD.Find("Solid Door Panel Left");
                    if (oldPL) Object.DestroyImmediate(oldPL.gameObject);

                    var pL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    pL.name = "Solid Door Panel Left";
                    pL.transform.SetParent(leftD, false);
                    pL.transform.localPosition = new Vector3(0.275f, 0f, 0f);
                    pL.transform.localScale = new Vector3(0.56f, 2.20f, 0.05f);
                    pL.GetComponent<Renderer>().sharedMaterial = darkWood;
                }

                var rightD = frontDoors.transform.Find("Entrance door right");
                if (rightD)
                {
                    rightD.localPosition = new Vector3(0.54f, 1.18f, 0f);
                    rightD.localRotation = Quaternion.identity;
                    var hingeR = rightD.GetComponent<HingedOpening>();
                    // Right door swings INWARD into living room (+Z)
                    if (hingeR) { hingeR.closedAngle = 0f; hingeR.openAngle = 90f; hingeR.isOpen = false; }

                    var oldPR = rightD.Find("Solid Door Panel Right");
                    if (oldPR) Object.DestroyImmediate(oldPR.gameObject);

                    var pR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    pR.name = "Solid Door Panel Right";
                    pR.transform.SetParent(rightD, false);
                    pR.transform.localPosition = new Vector3(-0.275f, 0f, 0f);
                    pR.transform.localScale = new Vector3(0.56f, 2.20f, 0.05f);
                    pR.GetComponent<Renderer>().sharedMaterial = darkWood;

                    var oldBatten = rightD.Find("Center Overlapping Astragal Batten");
                    if (oldBatten) Object.DestroyImmediate(oldBatten.gameObject);

                    var batten = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    batten.name = "Center Overlapping Astragal Batten";
                    batten.transform.SetParent(rightD, false);
                    batten.transform.localPosition = new Vector3(-0.55f, 0f, -0.015f);
                    batten.transform.localScale = new Vector3(0.08f, 2.22f, 0.045f);
                    batten.GetComponent<Renderer>().sharedMaterial = darkWood;
                }
            }
        }
    }
}
