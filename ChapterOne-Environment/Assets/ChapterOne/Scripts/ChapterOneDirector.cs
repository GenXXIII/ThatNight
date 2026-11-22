// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// ChapterOneDirector.cs — Master Sequence Controller
//
// Sequence Flow:
// 1. 4:00 AM Dawn glide into bridal bedroom onto sleeping bride.
// 2. Bride wakes to the phone ringing on the bedroom mirror table.
// 3. Bride rises and answers Brother's call.
// 4. After the call, Mother calls from the living room; the bride goes to meet her.
// 5. The bride returns to the mirror table for wedding preparation.
// 7. Wedding preparations at dressing table -> Ceremony -> Reception -> Horror disappearance.
// ============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum GameScene
    {
        None,
        Scene01_Opening,
        Scene02_WakingUpAndPhone,   // Waking and Brother's call at the bedroom mirror table
        Scene03_MotherCallsDoor,    // Mother calls, bride opens bedroom door
        Scene04_LivingRoom,         // Meet Mother in living room
        Scene05_Preparation,        // Dressing table with Mother
        Scene06_Ceremony,           // Traditional ceremony with Monk & Groom
        Scene07_Reception,          // Evening celebration
        Scene08_Disappearance,      // Horror climax in bedroom
        Scene09_Ending              // Final aerial shot & title drop
    }

    [DefaultExecutionOrder(-10)]
    public sealed class ChapterOneDirector : MonoBehaviour
    {
        public static ChapterOneDirector Instance { get; private set; }

        [Header("Current State")]
        public GameScene currentScene = GameScene.None;
        public string objective = "";

        [Header("Systems")]
        public EnvironmentCamera envCamera;
        public ForestExploration explorer;
        public PlayerInteraction player;
        public EnvironmentLighting lighting;
        public ChapterOneNarrative narrative;
        public EyelidOverlay eyelids;
        public AmbienceController ambience;

        [Header("Characters & Objects")]
        public MotherNPC mother;
        public GroomNPC groom;
        public NokiaHandPhone nokia;
        public WeddingPreparationStation preparationStation;

        [Header("Audio")]
        public AudioSource sfx;
        public AudioClip clipRooster;
        public AudioClip clipClockTick;
        public AudioClip clipRumble;
        public AudioClip clipScream;
        public AudioClip clipDoorBurst;

        [Header("Title / Intro Menu")]
        public bool showIntroMenu = true;
        public bool menuTransitioning = false;

        float fade;
        float shake;
        bool titleCard;
        bool objectiveVisible = true;
        bool skipOpening = false;
        public bool isStandingUp = false;
        public bool SequenceBusy { get; private set; }

        void BeginSequence(IEnumerator sequence)
        {
            if (SequenceBusy || (narrative && narrative.IsPlaying)) return;
            SequenceBusy = true;
            StartCoroutine(RunExclusive(sequence));
        }

        IEnumerator RunExclusive(IEnumerator sequence)
        {
            try { yield return sequence; }
            finally { SequenceBusy = false; }
        }

        static readonly Vector3 BedLocalPos = new(-3.45f, 2.5f, 4.2f);

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            DiscoverSystems();
            if (currentScene == GameScene.None)
                currentScene = GameScene.Scene01_Opening;

            StartCoroutine(RunChapter());
        }

        void DiscoverSystems()
        {
            if (!envCamera)  envCamera  = FindFirstObjectByType<EnvironmentCamera>();
            if (!explorer)   explorer   = envCamera ? envCamera.GetComponent<ForestExploration>() : null;
            if (!player)     player     = envCamera ? envCamera.GetComponent<PlayerInteraction>() : null;
            if (!lighting)   lighting   = envCamera ? envCamera.lighting : FindFirstObjectByType<EnvironmentLighting>();
            if (!narrative)  narrative  = FindFirstObjectByType<ChapterOneNarrative>();
            if (!narrative && envCamera) narrative = envCamera.gameObject.AddComponent<ChapterOneNarrative>();
            if (!eyelids)    eyelids    = FindFirstObjectByType<EyelidOverlay>();
            if (!ambience)   ambience   = FindFirstObjectByType<AmbienceController>();
            if (!mother)     mother     = FindFirstObjectByType<MotherNPC>();
            if (!groom)      groom      = FindFirstObjectByType<GroomNPC>();
            if (!nokia)      nokia      = FindFirstObjectByType<NokiaHandPhone>();
            if (!preparationStation) preparationStation = FindFirstObjectByType<WeddingPreparationStation>();
            if (!sfx) { sfx = gameObject.AddComponent<AudioSource>(); sfx.spatialBlend = 0f; }
        }

        void LockPlayer()
        {
            if (explorer) { explorer.ControlsLocked = true; if (explorer.Walking) explorer.SetWalking(false); }
        }

        void UnlockPlayer()
        {
            if (explorer)
            {
                explorer.ControlsLocked = false;
                explorer.MovementLocked = false;
                if (!explorer.Walking) explorer.SetWalking(true);
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void TeleportPlayer(Vector3 worldPos, Quaternion worldRot)
        {
            var cam = MainCam;
            if (cam) cam.transform.SetPositionAndRotation(worldPos, worldRot);
            if (player) player.transform.SetPositionAndRotation(worldPos, worldRot);
            if (explorer != null)
            {
                bool wasWalking = explorer.Walking;
                if (wasWalking) explorer.SetWalking(false);
                if (cam) cam.transform.SetPositionAndRotation(worldPos, worldRot);
                if (wasWalking) explorer.SetWalking(true);
            }
        }

        Camera MainCam => Camera.main ?? (envCamera ? envCamera.GetComponent<Camera>() : null);

        IEnumerator FadeToBlack(float duration = 1.5f)
        {
            float t = 0;
            while (t < 1f) { t += Time.deltaTime / duration; fade = Mathf.Lerp(0, 1, t); yield return null; }
            fade = 1f;
        }

        IEnumerator FadeFromBlack(float duration = 1.5f)
        {
            float t = 0;
            while (t < 1f) { t += Time.deltaTime / duration; fade = Mathf.Lerp(1, 0, t); yield return null; }
            fade = 0f;
        }

        IEnumerator ShakeCamera(float duration, float magnitude)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                shake = magnitude * (1f - elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            shake = 0;
        }

        IEnumerator GlideCamera(Vector3 from, Vector3 to, Quaternion fromRot, Quaternion toRot, float duration)
        {
            var cam = MainCam;
            if (!cam) yield break;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float s = Mathf.SmoothStep(0, 1, t);
                cam.transform.position = Vector3.Lerp(from, to, s);
                cam.transform.rotation = Quaternion.Slerp(fromRot, toRot, s);
                yield return null;
            }
            cam.transform.SetPositionAndRotation(to, toRot);
        }

        // Constant-velocity cruising camera motion (steady 10 km/h) with seamless overlapping cross-fades
        IEnumerator GlideCameraCruising(Vector3 from, Vector3 to, Quaternion fromRot, Quaternion toRot, float duration, float fadeInTime, float fadeOutTime)
        {
            var cam = MainCam;
            if (!cam) yield break;

            float t = 0f;
            while (t < 1f)
            {
                if (skipOpening)
                {
                    fade = 1f;
                    yield break;
                }
                t += Time.deltaTime / duration;
                float currentT = Mathf.Clamp01(t);

                // Constant linear cruising speed — never drops to 0 km/h during transition!
                cam.transform.position = Vector3.Lerp(from, to, currentT);
                cam.transform.rotation = Quaternion.Slerp(fromRot, toRot, currentT);

                float timeRemaining = (1f - currentT) * duration;
                float timePassed = currentT * duration;

                if (timePassed < fadeInTime && fadeInTime > 0.01f)
                {
                    fade = Mathf.Clamp01(1f - (timePassed / fadeInTime));
                }
                else if (timeRemaining < fadeOutTime && fadeOutTime > 0.01f)
                {
                    fade = Mathf.Clamp01(1f - (timeRemaining / fadeOutTime));
                }
                else
                {
                    fade = 0f;
                }

                yield return null;
            }

            cam.transform.SetPositionAndRotation(to, toRot);
            fade = (fadeOutTime > 0.01f) ? 1f : 0f;
        }

        IEnumerator GlideCameraPath(Vector3[] positions, Quaternion[] rotations, float[] durations)
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                yield return GlideCamera(
                    positions[i], positions[i + 1],
                    rotations[i], rotations[i + 1],
                    durations.Length > i ? durations[i] : 3.5f
                );
            }
        }

        void SetAtmosphere(AtmosphereStage stage)
        {
            if (lighting) lighting.ApplyStage(stage);
        }

        // ================================================================
        // MASTER CHAPTER FLOW
        // ================================================================

        Coroutine menuTourCoroutine = null;

        struct ScenicView
        {
            public Vector3 startPos;
            public Vector3 endPos;
            public Quaternion startRot;
            public Quaternion endRot;
            public float duration;

            public ScenicView(Vector3 sp, Vector3 ep, Quaternion sr, Quaternion er, float dur)
            {
                startPos = sp; endPos = ep; startRot = sr; endRot = er; duration = dur;
            }
        }

        IEnumerator RunChapter()
        {
            yield return new WaitForSeconds(0.1f);

            if (showIntroMenu)
            {
                currentScene = GameScene.Scene01_Opening;
                LockPlayer();
                SetAtmosphere(AtmosphereStage.Dawn4AM);

                if (ambience) ambience.SetScene(AmbienceScene.Dawn);
                fade = 0f;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // Start the scenic wedding tour loop in the background of the intro menu
                menuTourCoroutine = StartCoroutine(IntroMenuScenicTourLoop());

                while (showIntroMenu) yield return null;
            }
            else
            {
                switch (currentScene)
                {
                    case GameScene.Scene01_Opening:
                        yield return Scene01_OpeningCinematic();
                        yield return Scene02_WakeUpToPhoneCall();
                        break;
                    default:
                        SetAtmosphere(AtmosphereStage.AfternoonCelebration);
                        UnlockPlayer();
                        break;
                }
            }
        }

        IEnumerator IntroMenuScenicTourLoop()
        {
            // 5 Atmospheric scenic tour viewpoints across the wedding grounds:
            // 1. Front Wedding Arch & Red Carpet
            // 2. Left Banquet Marquee & Festive Dining Tables
            // 3. Wrist-Tying Mandap & Ceremony Stage
            // 4. Outdoor Traditional Khmer Kitchen
            // 5. High Aerial Overview of the Estate
            ScenicView[] views = new ScenicView[]
            {
                // 1. Red Carpet Aisle looking towards the illuminated Stilt House & Marquee
                // Start at Z = -11.0m (comfortably past the entrance arch canopy at Z = -14.0m, zero clipping!)
                new ScenicView(
                    new Vector3(-0.5f, 22.80f, -11.0f), new Vector3(-0.5f, 22.95f, -4.5f),
                    Quaternion.Euler(4f, 0f, 0f), Quaternion.Euler(5f, 0f, 0f), 7.5f),
                // 2. Left Banquet Marquee Tables
                new ScenicView(
                    new Vector3(-13.0f, 23.2f, -7.0f), new Vector3(-7.5f, 23.2f, 2.0f),
                    Quaternion.Euler(5f, 38f, 0f), Quaternion.Euler(6f, 48f, 0f), 7.0f),
                // 3. Ceremony Stage & Wrist-Tying Mandap
                new ScenicView(
                    new Vector3(6.5f, 23.4f, -4.5f), new Vector3(4.0f, 23.5f, -2.5f),
                    Quaternion.Euler(8f, -45f, 0f), Quaternion.Euler(9f, -30f, 0f), 6.5f),
                // 4. Outdoor Khmer Kitchen
                new ScenicView(
                    new Vector3(14.5f, 23.2f, 7.0f), new Vector3(12.5f, 23.2f, 13.0f),
                    Quaternion.Euler(6f, -15f, 0f), Quaternion.Euler(7f, -5f, 0f), 6.5f),
                // 5. High Aerial Overview of Estate
                new ScenicView(
                    new Vector3(32.0f, 38.0f, -38.0f), new Vector3(12.0f, 35.0f, -42.0f),
                    Quaternion.Euler(34f, -40f, 0f), Quaternion.Euler(32f, -15f, 0f), 7.5f)
            };

            var cam = MainCam;
            int viewIdx = 0;

            while (showIntroMenu && !menuTransitioning)
            {
                var v = views[viewIdx];
                if (cam) cam.transform.SetPositionAndRotation(v.startPos, v.startRot);

                // Constant cruising glide (continuous steady speed, overlapping cross-fades while in motion)
                float elapsed = 0f;
                float crossTime = 0.7f;
                while (elapsed < v.duration && showIntroMenu && !menuTransitioning)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Clamp01(elapsed / v.duration);

                    if (cam)
                    {
                        cam.transform.position = Vector3.Lerp(v.startPos, v.endPos, progress);
                        cam.transform.rotation = Quaternion.Slerp(v.startRot, v.endRot, progress);
                    }

                    float timeLeft = v.duration - elapsed;
                    if (elapsed < crossTime)
                    {
                        fade = Mathf.Clamp01(1f - (elapsed / crossTime)) * 0.85f;
                    }
                    else if (timeLeft < crossTime)
                    {
                        fade = Mathf.Clamp01(1f - (timeLeft / crossTime)) * 0.85f;
                    }
                    else
                    {
                        fade = 0f;
                    }

                    yield return null;
                }

                viewIdx = (viewIdx + 1) % views.Length;
            }
        }

        public void StartFromIntroMenu()
        {
            if (menuTransitioning) return;
            if (menuTourCoroutine != null) StopCoroutine(menuTourCoroutine);
            StartCoroutine(IntroMenuTransition());
        }

        IEnumerator IntroMenuTransition()
        {
            menuTransitioning = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (sfx && clipClockTick) sfx.PlayOneShot(clipClockTick, 0.7f);

            var cam = MainCam;
            Vector3 startPos = cam ? cam.transform.position : new Vector3(32.0f, 38.0f, -38.0f);
            Quaternion startRot = cam ? cam.transform.rotation : Quaternion.Euler(34f, -40f, 0f);
            Vector3 zoomTarget = startPos + (cam ? cam.transform.forward : Vector3.forward) * 6.0f;

            float duration = 2.0f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float smoothT = Mathf.SmoothStep(0, 1, t);
                if (cam) cam.transform.position = Vector3.Lerp(startPos, zoomTarget, smoothT);
                fade = Mathf.Clamp01(t * 1.25f);
                yield return null;
            }
            fade = 1f;
            showIntroMenu = false;
            menuTransitioning = false;

            yield return new WaitForSeconds(0.4f);

            yield return Scene01_OpeningCinematic();
            yield return Scene02_WakeUpToPhoneCall();
        }

        // ================================================================
        // SCENE 01 — OPENING CINEMATIC (4:00 AM)
        // 3 Exact Cinematic Shots matching reference photographs:
        // 1. Top Aerial Overview Curving to the Left (Photo 1)
        // 2. Left Side of Wedding Marquee Moving Straight (Photo 2)
        // 3. Front Entrance Arch Moving Straight Forward along Red Carpet (Photo 3)
        // ================================================================

        IEnumerator Scene01_OpeningCinematic()
        {
            currentScene = GameScene.Scene01_Opening;
            LockPlayer();
            SetAtmosphere(AtmosphereStage.Dawn4AM);
            fade = 1f;
            objective = "";

            var cam = MainCam;
            if (ambience) ambience.SetScene(AmbienceScene.Dawn);

            // ================================================================
            // CLIP 1: TOP AERIAL OVERVIEW CURVING TO THE LEFT (PHOTO 1)
            // Cruises at steady 10 km/h with seamless overlapping cross-fade
            // ================================================================
            Vector3 shot1StartPos = new Vector3(32.0f, 38.0f, -38.0f);
            Quaternion shot1StartRot = Quaternion.Euler(34f, -42f, 0f);
            Vector3 shot1EndPos = new Vector3(10.0f, 35.0f, -44.0f);
            Quaternion shot1EndRot = Quaternion.Euler(32f, -14f, 0f);

            if (cam) cam.transform.SetPositionAndRotation(shot1StartPos, shot1StartRot);
            if (sfx && clipRooster) sfx.PlayOneShot(clipRooster, 0.6f);

            objective = "Battambang Countryside — April 2006";
            yield return GlideCameraCruising(shot1StartPos, shot1EndPos, shot1StartRot, shot1EndRot, 4.5f, fadeInTime: 0.8f, fadeOutTime: 0.5f);
            if (skipOpening)
            {
                fade = 1f;
                Vector3 pPos = new Vector3(-9.79f, 24.55f, 25.05f);
                Quaternion pRot = Quaternion.Euler(-6f, 138f, 0f);
                if (cam) cam.transform.SetPositionAndRotation(pPos, pRot);
                yield break;
            }

            // ================================================================
            // CLIP 3: FRONT ENTRANCE RED CARPET STRAIGHT FORWARD (PHOTO 3)
            // Immediately starts cruising forward down the aisle at steady 10 km/h
            // ================================================================
            Vector3 shot3StartPos = new Vector3(-0.5f, 22.8f, -11.0f);
            Quaternion shot3StartRot = Quaternion.Euler(4f, 0f, 0f);
            Vector3 shot3EndPos = new Vector3(-0.5f, 23.2f, -3.5f);
            Quaternion shot3EndRot = Quaternion.Euler(6f, 0f, 0f);

            if (cam) cam.transform.SetPositionAndRotation(shot3StartPos, shot3StartRot);
            objective = "Chapter 1: The Wedding";
            yield return GlideCameraCruising(shot3StartPos, shot3EndPos, shot3StartRot, shot3EndRot, 4.8f, fadeInTime: 0.5f, fadeOutTime: 1.0f);

            // Arrive in darkness resting on the bridal bed pillow before Scene 02 begins
            Vector3 pillowPos = new Vector3(-9.79f, 24.55f, 25.05f);
            Quaternion pillowRot = Quaternion.Euler(-6f, 138f, 0f);
            if (cam) cam.transform.SetPositionAndRotation(pillowPos, pillowRot);
        }

        // ================================================================
        // SCENE 02 — WAKING UP BECAUSE PHONE IS RINGING BESIDE BED
        // ================================================================

        IEnumerator Scene02_WakeUpToPhoneCall()
        {
            currentScene = GameScene.Scene02_WakingUpAndPhone;
            isStandingUp = false; // Keeps 3D body completely hidden during bed wake-up so no glitch is visible!

            // Camera rests on the bridal bed pillow
            Vector3 pillowPos = new Vector3(-9.79f, 24.55f, 25.05f);
            Quaternion pillowRot = Quaternion.Euler(-6f, 138f, 0f);
            var cam = MainCam;
            if (cam) cam.transform.SetPositionAndRotation(pillowPos, pillowRot);
            TeleportPlayer(pillowPos, pillowRot);

            // Crucial: Clear director fullscreen fade quad so camera and eyelids become visible!
            fade = 0f;

            if (nokia) nokia.TriggerIncomingCall();
            if (eyelids)
            {
                bool fullyDone = false;
                yield return eyelids.PlayWakingSequence(
                    onHalfAwake: () => { objective = "Wedding Day — 4:00 AM"; },
                    onFullyAwake: () => { fullyDone = true; }
                );
                while (!fullyDone) yield return null;
            }
            else
            {
                yield return FadeFromBlack(1.6f);
                objective = "Wedding Day — 4:00 AM";
            }

            // Bride murmurs groggily as she opens her eyes
            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene01_WakingDialogue());
            }

            // Bride rises from bed (camera sits up and rises to standing, but body remains hidden so no glitch is visible)
            yield return RiseFromBed();

            // Once standing upright on the wooden floor beside the bed, activate body and unlock player controls
            isStandingUp = true;
            UnlockPlayer();
            objective = "Answer Brother's call at the bedroom mirror table";
            yield return new WaitUntil(() => currentScene != GameScene.Scene02_WakingUpAndPhone);
        }

        IEnumerator RiseFromBed()
        {
            var cam = MainCam;
            if (!cam) yield break;

            Vector3 pillowPos = new Vector3(-9.79f, 24.55f, 25.05f);
            Quaternion pillowRot = Quaternion.Euler(-6f, 138f, 0f);

            // 1. Sitting up on mattress, looking forward across bedroom (1.4s)
            Vector3 sitPos = new Vector3(-9.15f, 24.85f, 24.75f);
            Quaternion sitRot = Quaternion.Euler(15f, 105f, 0f);

            float dur1 = 1.4f;
            for (float t = 0; t < dur1; t += Time.deltaTime)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t / dur1);
                Vector3 p = Vector3.Lerp(pillowPos, sitPos, smooth);
                Quaternion r = Quaternion.Slerp(pillowRot, sitRot, smooth);
                cam.transform.SetPositionAndRotation(p, r);
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);

            // 2. Turning to the edge of the bed toward the floor (1.3s)
            Vector3 edgePos = new Vector3(-8.20f, 24.95f, 24.45f);
            Quaternion edgeRot = Quaternion.Euler(12f, 82f, 0f);

            float dur2 = 1.3f;
            for (float t = 0; t < dur2; t += Time.deltaTime)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t / dur2);
                Vector3 p = Vector3.Lerp(sitPos, edgePos, smooth);
                Quaternion r = Quaternion.Slerp(sitRot, edgeRot, smooth);
                cam.transform.SetPositionAndRotation(p, r);
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);

            // 3. Standing up onto the floorboards facing the dressing table (1.5s)
            Vector3 standPos = new Vector3(-7.40f, 25.32f, 24.20f);
            Quaternion standRot = Quaternion.Euler(4f, 82f, 0f);

            float dur3 = 1.5f;
            for (float t = 0; t < dur3; t += Time.deltaTime)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t / dur3);
                Vector3 p = Vector3.Lerp(edgePos, standPos, smooth);
                Quaternion r = Quaternion.Slerp(edgeRot, standRot, smooth);
                cam.transform.SetPositionAndRotation(p, r);
                yield return null;
            }

            TeleportPlayer(standPos, standRot);
        }

        // ================================================================
        // SCENE 03 — AFTER PHONE CALL: MOTHER CALLS FROM OUTSIDE
        // ================================================================

        public void OnPhoneCallComplete()
        {
            if (currentScene != GameScene.Scene02_WakingUpAndPhone) return;
            BeginSequence(MotherCallsMorningSequence());
        }

        IEnumerator MotherCallsMorningSequence()
        {
            currentScene = GameScene.Scene03_MotherCallsDoor;
            objective = "";

            yield return new WaitForSeconds(1.2f);

            // Mother calls out from the living room
            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene02_MorningLines());
            }

            objective = "Open the bedroom door and meet Mother";

            // A door opened during Mother's call must not strand the objective.
            foreach (var door in FindObjectsByType<HingedOpening>(FindObjectsSortMode.None))
                if (door.triggerDirectorOnOpen && door.isOpen) { OnBedroomDoorOpened(); break; }
        }

        // Called by HingedOpening when bridal bedroom door is pushed open
        public void OnBedroomDoorOpened()
        {
            if (currentScene == GameScene.Scene03_MotherCallsDoor)
            {
                StartCoroutine(Scene04_LivingRoomSequence());
            }
        }

        // ================================================================
        // SCENE 04 — LIVING ROOM: MEET MOTHER
        // ================================================================

        IEnumerator Scene04_LivingRoomSequence()
        {
            currentScene = GameScene.Scene04_LivingRoom;
            objective = "Find Mother in the living room";
            if (ambience) ambience.SetScene(AmbienceScene.Morning);

            // Wait for player to interact with Mother
            yield return new WaitUntil(() => currentScene != GameScene.Scene04_LivingRoom);
        }

        public void OnMotherTalkedLivingRoom()
        {
            if (currentScene == GameScene.Scene04_LivingRoom)
            {
                BeginSequence(PlayLivingRoomDialogue());
            }
        }

        IEnumerator PlayLivingRoomDialogue()
        {
            objective = "Talk with Mother";
            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene03_LivingAreaLines());
            }

            currentScene = GameScene.Scene05_Preparation;
            objective = "Return to the bedroom mirror table to prepare for the wedding with Mother";
        }

        // ================================================================
        // SCENE 05 — WEDDING PREPARATION (Mother & Daughter at Mirror)
        // ================================================================

        public void OnPreparationStarted()
        {
            if (currentScene == GameScene.Scene05_Preparation)
            {
                BeginSequence(PlayPreparationSequence());
            }
        }

        IEnumerator PlayPreparationSequence()
        {
            objective = "Prepare bridal attire with Mother";
            // Stage the conversation at the actual dressing station, hiding the reposition.
            LockPlayer();
            yield return FadeToBlack(0.6f);
            if (mother && preparationStation)
            {
                Vector3 seat = MainCam.transform.position;
                Vector3 side = MainCam.transform.right;
                Vector3 position = seat + side * 0.8f - MainCam.transform.forward * 0.35f;
                position.y = mother.transform.position.y;
                Vector3 direction = preparationStation.transform.position - position;
                direction.y = 0;
                mother.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
                mother.currentPosition = MotherPosition.Bedroom;
            }
            yield return FadeFromBlack(0.6f);
            UnlockPlayer();

            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene05_PreparationLines());
            }

            // Montage complete -> transition to ceremony
            yield return TransitionToCeremony();
        }

        IEnumerator TransitionToCeremony()
        {
            objective = "The wedding ceremony awaits...";
            LockPlayer();

            yield return FadeToBlack(1.2f);

            currentScene = GameScene.Scene06_Ceremony;
            SetAtmosphere(AtmosphereStage.AfternoonCelebration);
            if (ambience) ambience.SetScene(AmbienceScene.Ceremony);

            // Teleport outside to wedding marquee entrance
            Vector3 ceremonyPos = new(0, SiteLayout.HillElevation + 1.72f, -17);
            TeleportPlayer(ceremonyPos, Quaternion.Euler(0, 0, 0));

            // Move Mother & Groom to ceremony positions
            if (mother) mother.MoveToPosition(MotherPosition.Ceremony);
            if (groom) groom.MoveToPosition(GroomPosition.Ceremony);

            yield return new WaitForSeconds(0.5f);
            yield return FadeFromBlack(1.2f);

            objective = "Join the wedding ceremony & greet your groom";
            UnlockPlayer();

        }

        // ================================================================
        // SCENE 06 — WEDDING CEREMONY
        // ================================================================

        public void OnCeremonyInteraction()
        {
            if (currentScene == GameScene.Scene06_Ceremony)
            {
                BeginSequence(PlayCeremonySequence());
            }
        }

        IEnumerator PlayCeremonySequence()
        {
            objective = "Traditional Khmer Wedding Ceremony";
            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene06_CeremonyLines());
            }

            yield return TransitionToReception();
        }

        IEnumerator TransitionToReception()
        {
            currentScene = GameScene.Scene07_Reception;
            objective = "Enjoy the evening celebration";
            LockPlayer();

            yield return FadeToBlack(1.0f);

            SetAtmosphere(AtmosphereStage.EveningWarm);
            if (ambience) ambience.SetScene(AmbienceScene.Reception);

            if (mother) mother.MoveToPosition(MotherPosition.Reception);
            if (groom) groom.MoveToPosition(GroomPosition.Reception);

            yield return new WaitForSeconds(0.5f);
            yield return FadeFromBlack(1.0f);

            UnlockPlayer();

        }

        // ================================================================
        // SCENE 07 — WEDDING RECEPTION
        // ================================================================

        public void OnReceptionInteraction()
        {
            if (currentScene == GameScene.Scene07_Reception)
            {
                BeginSequence(PlayReceptionSequence());
            }
        }

        IEnumerator PlayReceptionSequence()
        {
            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene07_ReceptionLines());
            }

            yield return TransitionToHorror();
        }

        // ================================================================
        // SCENE 08 — THE DISAPPEARANCE (Horror Climax)
        // ================================================================

        IEnumerator TransitionToHorror()
        {
            currentScene = GameScene.Scene08_Disappearance;
            objective = "Retire to bridal bedroom with Groom";
            LockPlayer();

            yield return FadeToBlack(1.0f);

            SetAtmosphere(AtmosphereStage.NightHorror);
            if (ambience) ambience.SetScene(AmbienceScene.Horror);

            // Move bride and groom inside bridal bedroom
            Vector3 bedPos = SiteLayout.HousePoint(BedLocalPos);
            Quaternion bedRot = SiteLayout.HouseRotation * Quaternion.Euler(0, 160, 0);
            TeleportPlayer(bedPos, bedRot);
            if (groom) groom.MoveToPosition(GroomPosition.Bedroom);

            yield return new WaitForSeconds(0.5f);
            yield return FadeFromBlack(1.0f);

            yield return PlayDisappearanceClimax();
        }

        IEnumerator PlayDisappearanceClimax()
        {
            objective = "";

            var lines = ChapterOneNarrative.Scene08_DisappearanceLines();

            // "Finally, some quiet." ... "The best."
            if (narrative)
            {
                yield return narrative.PlayAndWait(lines.GetRange(0, 4));
            }

            yield return new WaitForSeconds(3.0f);

            // Anomaly: deep rumble from beneath floorboards
            if (sfx && clipRumble) sfx.PlayOneShot(clipRumble, 0.8f);
            StartCoroutine(ShakeCamera(4.0f, 0.04f));

            // "Did you hear that?" ... "Wait..."
            if (narrative)
            {
                yield return narrative.PlayAndWait(lines.GetRange(4, 5));
            }

            yield return new WaitForSeconds(2.5f);

            // Supernatural shockwave + scream + instant blackout
            StartCoroutine(ShakeCamera(1.5f, 0.15f));
            if (sfx && clipScream) sfx.PlayOneShot(clipScream, 1.0f);

            yield return new WaitForSeconds(0.6f);

            // INSTANT BLACKOUT
            fade = 1.0f;
            objective = "";

            // Both spouses have disappeared before the external empty-room reveal.
            if (groom) groom.gameObject.SetActive(false);
            if (player && player.body)
            {
                player.body.enabled = false;
                if (player.body.lowerBody) player.body.lowerBody.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(2.5f);

            // Panicked guest & mother reactions outside
            if (narrative)
            {
                yield return narrative.PlayAndWait(lines.GetRange(9, lines.Count - 9));
            }

            yield return new WaitForSeconds(3.0f);

            yield return Scene09_EndingCinematic();
        }

        // ================================================================
        // SCENE 09 — FINAL CINEMATIC & TITLE REVEAL
        // ================================================================

        IEnumerator Scene09_EndingCinematic()
        {
            currentScene = GameScene.Scene09_Ending;
            fade = 1.0f;

            yield return new WaitForSeconds(1.5f);

            var cam = MainCam;
            if (cam)
            {
                Vector3 bedroomDoorPos = SiteLayout.HousePoint(new Vector3(-2.5f, 2.5f, 2.0f));
                Quaternion bedroomDoorRot = SiteLayout.HouseRotation * Quaternion.Euler(5, -160, 0);
                cam.transform.SetPositionAndRotation(bedroomDoorPos, bedroomDoorRot);
            }

            yield return FadeFromBlack(1.5f);
            yield return new WaitForSeconds(3.0f);

            if (cam)
            {
                Vector3[] endingPath = new Vector3[]
                {
                    cam.transform.position,
                    SiteLayout.HousePoint(new Vector3(-0.8f, 2.5f, 0.5f)),
                    SiteLayout.HousePoint(new Vector3(0, 2.5f, -2.0f)),
                    new(0, SiteLayout.HillElevation + 3f, 12),
                    new(0, SiteLayout.HillElevation + 15f, 5),
                    new(0, SiteLayout.HillElevation + 55f, -10),
                };
                Quaternion[] endingRots = new Quaternion[]
                {
                    cam.transform.rotation,
                    SiteLayout.HouseRotation * Quaternion.Euler(0, 180, 0),
                    SiteLayout.HouseRotation * Quaternion.Euler(5, 180, 0),
                    Quaternion.Euler(10, 180, 0),
                    Quaternion.Euler(35, 180, 0),
                    Quaternion.Euler(55, 180, 0),
                };
                float[] endingDurations = new float[] { 3f, 3f, 3.5f, 4f, 5f };

                yield return GlideCameraPath(endingPath, endingRots, endingDurations);
            }

            yield return new WaitForSeconds(3.0f);
            yield return FadeToBlack(2.0f);
            yield return new WaitForSeconds(1.5f);

            titleCard = true;
        }

        void Update()
        {
            if (currentScene == GameScene.Scene01_Opening && !showIntroMenu)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
                {
                    skipOpening = true;
                }
            }
        }

        void LateUpdate()
        {
            if (shake > 0.001f && MainCam)
            {
                MainCam.transform.localPosition += UnityEngine.Random.insideUnitSphere * shake;
            }
        }

        static Texture2D leftVignetteTex;
        static Texture2D GetLeftVignetteTex()
        {
            if (leftVignetteTex) return leftVignetteTex;
            leftVignetteTex = new Texture2D(128, 1, TextureFormat.RGBA32, false);
            leftVignetteTex.wrapMode = TextureWrapMode.Clamp;
            Color[] cols = new Color[128];
            for (int i = 0; i < 128; i++)
            {
                float u = (float)i / 127f;
                float alpha = Mathf.Pow(1f - u, 1.8f) * 0.88f;
                cols[i] = new Color(0.02f, 0.02f, 0.02f, alpha);
            }
            leftVignetteTex.SetPixels(cols);
            leftVignetteTex.Apply();
            return leftVignetteTex;
        }

        bool DrawCustomMenuButton(Rect rect, string text, bool isHover)
        {
            // Glass slate background
            GUI.color = isHover ? new Color(0.24f, 0.17f, 0.08f, 0.88f) : new Color(0.05f, 0.04f, 0.04f, 0.70f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            // 1px Gold border
            GUI.color = isHover ? new Color(0.96f, 0.84f, 0.44f, 0.95f) : new Color(0.68f, 0.54f, 0.28f, 0.45f);
            float bw = isHover ? 1.5f : 1.0f;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, bw), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - bw, rect.width, bw), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, bw, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - bw, rect.y, bw, rect.height), Texture2D.whiteTexture);

            // Corner ornamental notches when hovered
            if (isHover)
            {
                float c = 5f;
                GUI.color = new Color(1f, 0.90f, 0.55f, 1f);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.y - 2, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.y - 2, 2, c), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax - c + 2, rect.y - 2, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax, rect.y - 2, 2, c), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.yMax, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.yMax - c + 2, 2, c), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax - c + 2, rect.yMax, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax, rect.yMax - c + 2, 2, c), Texture2D.whiteTexture);
            }
            GUI.color = Color.white;

            // Text
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = isHover ? new Color(1f, 0.95f, 0.78f, 1f) : new Color(0.88f, 0.86f, 0.82f, 0.90f);
            GUI.Label(rect, text, style);

            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        void OnGUI()
        {
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);

            if (fade > 0.005f)
            {
                GUI.color = new Color(0, 0, 0, Mathf.Clamp01(fade));
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }

            if (showIntroMenu && !menuTransitioning)
            {
                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
                float sw = Screen.width / scale, sh = Screen.height / scale;

                // 1. Cinemascope widescreen letterbox bars top & bottom
                float barH = sh * 0.055f;
                GUI.color = new Color(0.02f, 0.02f, 0.02f, 0.96f);
                GUI.DrawTexture(new Rect(0, 0, sw, barH), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, sh - barH, sw, barH), Texture2D.whiteTexture);
                // Subtle gold accent along letterbox edges
                GUI.color = new Color(0.68f, 0.54f, 0.28f, 0.35f);
                GUI.DrawTexture(new Rect(0, barH - 1, sw, 1), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, sh - barH, sw, 1), Texture2D.whiteTexture);
                GUI.color = Color.white;

                // 2. Smooth cinematic gradient vignette on the left (seamless fade into 3D world!)
                GUI.DrawTexture(new Rect(0, barH, 560, sh - barH * 2), GetLeftVignetteTex());

                float leftX = 85;
                float topY = sh * 0.22f;

                // 3. Khmer ornate heading in warm luminous gold
                var khmerHeadingStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 17,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                khmerHeadingStyle.normal.textColor = new Color(0.88f, 0.74f, 0.38f, 0.95f);
                GUI.Label(new Rect(leftX, topY, 440, 26), "✦   យ  ប់  នោះ   ✦", khmerHeadingStyle);

                // 4. Main title in monumental lettering with horror crimson shadow
                var titleShadowStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 48,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                titleShadowStyle.normal.textColor = new Color(0.38f, 0.05f, 0.05f, 0.85f);
                GUI.Label(new Rect(leftX + 2, topY + 28, 440, 56), "THAT NIGHT", titleShadowStyle);

                var titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 48,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                titleStyle.normal.textColor = new Color(0.98f, 0.96f, 0.92f, 1f);
                GUI.Label(new Rect(leftX, topY + 26, 440, 56), "THAT NIGHT", titleStyle);

                // 5. Chapter name & setting
                var chapterStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                chapterStyle.normal.textColor = new Color(0.82f, 0.78f, 0.70f, 0.92f);
                GUI.Label(new Rect(leftX, topY + 84, 440, 22), "CHAPTER ONE: THE WEDDING · បុណ្យអាពាហ៍ពិពាហ៍", chapterStyle);

                var settingStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.UpperLeft
                };
                settingStyle.normal.textColor = new Color(0.68f, 0.60f, 0.48f, 0.80f);
                GUI.Label(new Rect(leftX, topY + 106, 440, 20), "Battambang Countryside, Cambodia — April 2006", settingStyle);

                // Fine gold rule with diamond center
                GUI.color = new Color(0.72f, 0.58f, 0.30f, 0.40f);
                GUI.DrawTexture(new Rect(leftX, topY + 134, 340, 1), Texture2D.whiteTexture);
                GUI.color = Color.white;

                // 6. Interactive Custom Styled Buttons
                float btnY = topY + 155;
                float btnW = 320;
                float btnH = 46;
                float spacing = 58;

                Vector2 mousePos = Event.current != null ? Event.current.mousePosition : Vector2.zero;

                Rect rContinue = new Rect(leftX, btnY, btnW, btnH);
                bool hoverContinue = rContinue.Contains(mousePos);
                if (DrawCustomMenuButton(rContinue, "▶    CONTINUE", hoverContinue))
                {
                    StartFromIntroMenu();
                }

                Rect rNewGame = new Rect(leftX, btnY + spacing, btnW, btnH);
                bool hoverNewGame = rNewGame.Contains(mousePos);
                if (DrawCustomMenuButton(rNewGame, "✦    NEW GAME", hoverNewGame))
                {
                    StartFromIntroMenu();
                }

                Rect rQuit = new Rect(leftX, btnY + spacing * 2, btnW, btnH);
                bool hoverQuit = rQuit.Contains(mousePos);
                if (DrawCustomMenuButton(rQuit, "✕    QUIT", hoverQuit))
                {
                    Application.Quit();
                }

                GUI.matrix = prev;
            }

            // On-screen skip hint during Opening flyby cinematic
            if (currentScene == GameScene.Scene01_Opening && !showIntroMenu && !menuTransitioning && fade < 0.85f)
            {
                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
                float sw = Screen.width / scale, sh = Screen.height / scale;

                var skipStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.MiddleRight
                };
                skipStyle.normal.textColor = new Color(0.85f, 0.82f, 0.75f, 0.70f);

                GUI.color = new Color(0f, 0f, 0f, 0.40f);
                GUI.DrawTexture(new Rect(sw - 230, sh - 46, 196, 28), Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.Label(new Rect(sw - 235, sh - 46, 190, 28), "[Space] Skip Opening", skipStyle);

                GUI.matrix = prev;
            }

            if (!titleCard && !string.IsNullOrEmpty(objective) && fade < 0.5f && objectiveVisible)
            {
                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);

                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 17,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                style.normal.textColor = new Color(0.92f, 0.88f, 0.72f, 0.85f);

                GUI.color = new Color(0, 0, 0, 0.5f);
                GUI.Label(new Rect(34, 30, 800, 32), "◆  " + objective, style);
                GUI.color = new Color(0.92f, 0.88f, 0.72f, 0.85f);
                GUI.Label(new Rect(32, 28, 800, 32), "◆  " + objective, style);
                GUI.color = Color.white;

                GUI.matrix = prev;
            }

            if (titleCard)
            {
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
                float sw = Screen.width / scale, sh = Screen.height / scale;

                var taglineStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 18,
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Normal
                };
                taglineStyle.normal.textColor = new Color(0.72f, 0.22f, 0.22f, 0.95f);

                var titleStyle = new GUIStyle(taglineStyle)
                {
                    fontSize = 56,
                    fontStyle = FontStyle.Bold
                };
                titleStyle.normal.textColor = new Color(0.95f, 0.94f, 0.90f, 1f);

                var chapterStyle = new GUIStyle(taglineStyle);
                chapterStyle.normal.textColor = new Color(0.85f, 0.75f, 0.55f, 0.85f);

                GUI.color = Color.white;
                GUI.Label(new Rect(0, sh * 0.34f, sw, 30), "A CAMBODIAN SUPERNATURAL HORROR GAME", taglineStyle);
                GUI.Label(new Rect(0, sh * 0.43f, sw, 80), "THAT NIGHT", titleStyle);
                GUI.Label(new Rect(0, sh * 0.56f, sw, 30), "CHAPTER ONE — THE WEDDING", chapterStyle);

                GUI.matrix = prev;
            }
        }
    }
}
