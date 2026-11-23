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
    public sealed partial class ChapterOneDirector : MonoBehaviour
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
        public bool skipCurrentShot = false;
        public float cinematicSpaceTimer = 0f;
        public const float CinematicHoldToSkip = 2.0f;

        public bool hasMovedAround = false;
        public bool hasPickedUpPhoneOnce = false;
        public bool hasOpenedDoorOnce = false;
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
            if (PreviewMode.SkipStory) { enabled = false; return; }
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
                if (skipCurrentShot)
                {
                    skipCurrentShot = false;
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

        void Update()
        {
            if (currentScene == GameScene.Scene01_Opening && !showIntroMenu)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    cinematicSpaceTimer += Time.deltaTime;
                    if (cinematicSpaceTimer >= CinematicHoldToSkip)
                    {
                        skipOpening = true;
                        cinematicSpaceTimer = 0f;
                    }
                }
                else
                {
                    cinematicSpaceTimer = 0f;
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

    }
}
