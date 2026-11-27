using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed partial class ChapterOneDirector
    {
        float GroundHeightAt(float x, float z, float fallbackY = 20.88f)
        {
            if (Physics.Raycast(new Vector3(x, 80f, z), Vector3.down, out var hit, 140f, ~0, QueryTriggerInteraction.Ignore))
                return hit.point.y;
            return fallbackY;
        }

        IEnumerator AutoWalkCameraTo(Vector3 targetEyePos, Vector3 lookAtWorld, float duration)
        {
            var cam = MainCam;
            if (!cam) yield break;

            Vector3 startPos = cam.transform.position;
            Quaternion startRot = cam.transform.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float u = Mathf.Clamp01(elapsed / duration);
                float smooth = Mathf.SmoothStep(0f, 1f, u);
                Vector3 pos = Vector3.Lerp(startPos, targetEyePos, smooth);
                // Natural footsteps head-bob during auto-approach
                float bob = Mathf.Sin(elapsed * 8.5f) * 0.018f * (1f - u * 0.6f);
                pos.y += bob;

                Vector3 lookDir = lookAtWorld - pos;
                Quaternion targetRot = lookDir.sqrMagnitude > 0.01f ? Quaternion.LookRotation(lookDir.normalized) : startRot;
                Quaternion rot = Quaternion.Slerp(startRot, targetRot, smooth);

                if (explorer != null && explorer.Walking)
                    explorer.MoveRigAndCamera(pos, rot, seated: false);
                else
                    cam.transform.SetPositionAndRotation(pos, rot);

                yield return null;
            }

            Vector3 finalDir = (lookAtWorld - targetEyePos).normalized;
            Quaternion finalRot = finalDir.sqrMagnitude > 0.01f ? Quaternion.LookRotation(finalDir) : startRot;
            TeleportPlayer(targetEyePos, finalRot, seated: false);
        }

        // Winding left-right forest trail far away from BOTH the Big Tree and the Wedding House.
        // Wedding House sits at (0, 22) -> over 1,350m away and completely blocked by the corridor.
        // Big Tree sits at (-880, 1020) -> ~760m of winding left-and-right S-curves through dense jungle.
        public static readonly Vector2[] ForestNightmareTrail =
        {
            new(-1240f, 560f),   // 0: Unconscious wake-up clearing deep in western jungle
            new(-1145f, 625f),   // 1: Bend 1 — curves RIGHT (East-Northeast)
            new(-1215f, 710f),   // 2: Bend 2 — curves LEFT (Northwest)
            new(-1055f, 775f),   // 3: Bend 3 — curves RIGHT (East-Northeast)
            new(-1110f, 865f),   // 4: Bend 4 — curves LEFT (Northwest)
            new(-935f,  910f),   // 5: Bend 5 — curves RIGHT toward Sacred Glade
            new(-866.5f, 954f),  // 6: Big Tree auto-walk cinematic trigger (open glade bay)
            new(-864.2f, 973.0f) // 7: Stop position behind the crying woman's back
        };

        public static readonly Vector3 SacredTreeGhostPos = new(-863.5f, 20.88f, 976.5f);

        // ================================================================
        // SCENE 00 — OPENING NIGHTMARE IN THE HORROR FOREST
        // 1. Bride wakes unconscious deep in the forest (far from Big Tree & Wedding House),
        //    glances around confused, and hears someone crying in the distance.
        // 2. Stands up -> Task: Pick up the flashlight into her hand.
        // 3. Walks following the crying voice along a winding left-right forest trail
        //    (corridor prevents direct straight shortcut & blocks returning to the wedding house).
        // 4. Reaches the Big Tree -> auto-walk opening cinematic to behind the crying woman
        //    (Bride sees ONLY her back).
        // 5. Bride asks why she is crying -> Ghost relives her love, promises, betrayal,
        //    suffering, and snapping anger (sympathy turns to fear).
        // 6. Ghost slowly turns around with an unnatural circling neck, stalks in a scary pose
        //    with her shadowed face clearly visible, until filling the entire screen -> smash cut!
        // ================================================================

        IEnumerator Scene00_ForestNightmareSequence()
        {
            currentScene = GameScene.Scene00_ForestNightmare;
            isStandingUp = false;
            hasPickedUpFlashlightOnce = false;
            shadowScareFlash = 0f;
            LockPlayer();
            SetAtmosphere(AtmosphereStage.NightHorror);
            RenderSettings.fogDensity = 0.032f;
            if (ambience) ambience.SetScene(AmbienceScene.Horror);

            if (forestNightmareRoot) forestNightmareRoot.gameObject.SetActive(true);

            Vector2 spawnXZ = ForestNightmareTrail[0];
            float spawnGroundY = GroundHeightAt(spawnXZ.x, spawnXZ.y);
            Vector2 bend1XZ = ForestNightmareTrail[1];
            Vector3 toBend1 = new Vector3(bend1XZ.x - spawnXZ.x, 0f, bend1XZ.y - spawnXZ.y).normalized;
            float wakeYaw = Mathf.Atan2(toBend1.x, toBend1.z) * Mathf.Rad2Deg;

            // Place Ghost Woman in the open moonlit glade bay beneath the colossal Sacred World Tree
            // facing North-Northeast (into the trunk) so the Bride from the South sees ONLY her back!
            Vector3 ghostWorldPos = new Vector3(SacredTreeGhostPos.x, GroundHeightAt(SacredTreeGhostPos.x, SacredTreeGhostPos.z), SacredTreeGhostPos.z);
            if (ghostWoman)
            {
                ghostWoman.transform.SetPositionAndRotation(ghostWorldPos, Quaternion.Euler(0f, 12f, 0f));
                if (clipWomanCrying && !ghostWoman.cryingClip) ghostWoman.cryingClip = clipWomanCrying;
                if (clipNeckCrack && !ghostWoman.neckCrackClip) ghostWoman.neckCrackClip = clipNeckCrack;
                if (clipScream && !ghostWoman.screamClip) ghostWoman.screamClip = clipScream;
                ghostWoman.SetState(GhostAnimState.Crying);
                ghostWoman.SetCryingAudioActive(false);
            }

            // Place the Flashlight on the forest floor ~0.95m in front-right of where the Bride wakes up
            Vector3 rightDir = Vector3.Cross(Vector3.up, toBend1).normalized;
            Vector3 flashXZ = new Vector3(spawnXZ.x, 0f, spawnXZ.y) + toBend1 * 0.95f + rightDir * 0.28f;
            float flashGroundY = GroundHeightAt(flashXZ.x, flashXZ.z, spawnGroundY);
            Vector3 flashWorldPos = new Vector3(flashXZ.x, flashGroundY + 0.045f, flashXZ.z);
            Quaternion flashWorldRot = Quaternion.Euler(0f, wakeYaw + 32f, 90f);
            if (forestFlashlight)
            {
                forestFlashlight.ResetToGround(flashWorldPos, flashWorldRot);
            }

            // Configure forest corridor: locked to wake-up clearing until flashlight is picked up
            if (explorer != null)
            {
                explorer.RestrictToCompound = false;
                explorer.FlashlightUnlocked = false;
                if (explorer.flashlight) explorer.flashlight.enabled = false;
                explorer.WalkSpeedMultiplier = 2.6f; // Brisk urgent forest walk across long winding trail
                explorer.SetTrailCorridor(ForestNightmareTrail, halfWidth: 24f, maxDistance: 10f);
            }

            // Soft dark-adapted moonlit fill on camera so ground & flashlight glint are readable before pickup
            Light dreamEyeFill = null;
            var fillCam = MainCam;
            if (fillCam)
            {
                var fillObj = new GameObject("Nightmare Eye Fill");
                fillObj.transform.SetParent(fillCam.transform, false);
                fillObj.transform.localPosition = new Vector3(0f, 0.1f, 0.2f);
                dreamEyeFill = fillObj.AddComponent<Light>();
                dreamEyeFill.type = LightType.Point;
                dreamEyeFill.color = new Color(0.32f, 0.44f, 0.62f);
                dreamEyeFill.range = 12f;
                dreamEyeFill.intensity = 0.42f;
                dreamEyeFill.shadows = LightShadows.None;
            }

            // ------------------------------------------------------------
            // PHASE 1: UNCONSCIOUS ON FOREST FLOOR -> GLANCE CONFUSED & HEAR CRYING
            // ------------------------------------------------------------
            if (explorer != null) explorer.CinematicOverride = true;
            Vector3 lyingPos = new Vector3(spawnXZ.x, spawnGroundY + 0.30f, spawnXZ.y);
            Quaternion lyingRot = Quaternion.Euler(-12f, wakeYaw, -14f);
            var cam = MainCam;
            if (cam) cam.transform.SetPositionAndRotation(lyingPos, lyingRot);
            TeleportPlayer(lyingPos, lyingRot);

            fade = 0f;
            objective = "";

            // Crying voice echoes from ahead along the first bend as she wakes up!
            Vector3 initialCryPos = new Vector3(spawnXZ.x, spawnGroundY + 1.4f, spawnXZ.y) + toBend1 * 28f;
            if (ghostWoman)
            {
                ghostWoman.SetCryBeaconWorldPosition(initialCryPos, 0.48f);
            }

            if (eyelids)
            {
                bool awake = false;
                yield return eyelids.PlayWakingSequence(
                    onHalfAwake: () =>
                    {
                        if (ghostWoman) ghostWoman.SetCryingAudioActive(true, 0.48f);
                    },
                    onFullyAwake: () => { awake = true; }
                );
                while (!awake) yield return null;
            }
            else
            {
                yield return FadeFromBlack(1.4f);
                if (ghostWoman) ghostWoman.SetCryingAudioActive(true, 0.48f);
            }

            // Confused glancing around left and right from the forest floor while hearing the crying voice
            Vector3 proppedPos = new Vector3(spawnXZ.x, spawnGroundY + 0.58f, spawnXZ.y);
            Quaternion glanceLeft  = Quaternion.Euler(-4f, wakeYaw - 48f, -5f);
            Quaternion glanceRight = Quaternion.Euler(-2f, wakeYaw + 54f, 4f);
            Quaternion glanceBackL = Quaternion.Euler(4f,  wakeYaw - 28f, -2f);
            Quaternion lookTowardFlash = Quaternion.LookRotation((flashWorldPos - new Vector3(spawnXZ.x, spawnGroundY + 1.54f, spawnXZ.y)).normalized);

            yield return GlideCamera(lyingPos, proppedPos, lyingRot, glanceLeft, 1.15f);
            yield return new WaitForSeconds(0.20f);
            yield return GlideCamera(proppedPos, proppedPos, glanceLeft, glanceRight, 1.30f);
            yield return new WaitForSeconds(0.18f);
            yield return GlideCamera(proppedPos, proppedPos, glanceRight, glanceBackL, 0.95f);

            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_ForestWakeLines());
            }

            // Stand up smoothly from the forest floor looking toward the flashlight
            Vector3 standPos = new Vector3(spawnXZ.x, spawnGroundY + 1.54f, spawnXZ.y);
            Quaternion curRot = MainCam ? MainCam.transform.rotation : glanceBackL;
            yield return GlideCamera(proppedPos, standPos, curRot, lookTowardFlash, 1.25f);
            if (explorer != null) explorer.CinematicOverride = false;
            TeleportPlayer(standPos, lookTowardFlash);

            // ------------------------------------------------------------
            // PHASE 2: TASK 1 — PICK UP FLASHLIGHT TO HAND
            // ------------------------------------------------------------
            isStandingUp = true;
            UnlockPlayer();
            if (explorer != null)
            {
                explorer.RestrictToCompound = false;
                explorer.WalkSpeedMultiplier = 2.6f;
                explorer.SetTrailCorridor(ForestNightmareTrail, halfWidth: 16f, maxDistance: 10f);
            }

            objective = "Pick up the flashlight";
            if (forestFlashlight != null)
            {
                yield return new WaitUntil(() => forestFlashlight == null || forestFlashlight.IsPickedUp || currentScene != GameScene.Scene00_ForestNightmare);
            }

            if (currentScene != GameScene.Scene00_ForestNightmare)
            {
                if (dreamEyeFill) Destroy(dreamEyeFill.gameObject);
                yield break;
            }

            hasPickedUpFlashlightOnce = true;
            if (dreamEyeFill) dreamEyeFill.intensity = 0.22f;

            if (explorer != null)
            {
                explorer.FlashlightUnlocked = true;
                explorer.SetTrailCorridor(ForestNightmareTrail, halfWidth: 24f, maxDistance: float.PositiveInfinity);
            }

            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_AfterFlashlightLines());
            }

            // ------------------------------------------------------------
            // PHASE 3: TASK 2 — FOLLOW CRYING VOICE ALONG WINDING LEFT-RIGHT PATH
            // ------------------------------------------------------------
            objective = "Follow the crying voice through the forest";
            float totalLen = explorer != null ? Mathf.Max(1f, explorer.TrailTotalLength) : 760f;
            float triggerDist = Vector2.Distance(ForestNightmareTrail[ForestNightmareTrail.Length - 2], ForestNightmareTrail[ForestNightmareTrail.Length - 1]);
            float triggerProgress = Mathf.Max(totalLen - triggerDist - 16f, totalLen * 0.88f);
            bool playedMidTrailLine = false;
            bool remarkedGiantTree = false;

            while (currentScene == GameScene.Scene00_ForestNightmare)
            {
                cam = MainCam;
                if (cam)
                {
                    Vector3 cur = cam.transform.position;
                    Vector2 cur2 = new(cur.x, cur.z);
                    float prog = explorer != null ? explorer.GetTrailProgress(cur2) : 0f;
                    if (prog > 6f) hasMovedAround = true;

                    float distToGhost = Vector2.Distance(cur2, new Vector2(ghostWorldPos.x, ghostWorldPos.z));

                    // Guide the player around every left/right bend using the 3D crying voice
                    if (ghostWoman)
                    {
                        if (distToGhost <= 90f)
                        {
                            ghostWoman.LockCryBeaconToBody(0.95f);
                            if (!remarkedGiantTree)
                            {
                                remarkedGiantTree = true;
                                if (narrative) yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_NearTreeCryingLines());
                            }
                        }
                        else if (explorer != null)
                        {
                            Vector2 ahead2 = explorer.SampleTrailAtDistance(prog + 36f);
                            float aheadY = GroundHeightAt(ahead2.x, ahead2.y, spawnGroundY) + 1.35f;
                            float vol = Mathf.Lerp(0.46f, 0.92f, Mathf.Clamp01(prog / Mathf.Max(1f, triggerProgress)));
                            ghostWoman.SetCryBeaconWorldPosition(new Vector3(ahead2.x, aheadY, ahead2.y), vol);
                        }
                    }

                    if (!playedMidTrailLine && prog >= totalLen * 0.45f)
                    {
                        playedMidTrailLine = true;
                        if (narrative) yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_ForestMidTrailLines());
                    }

                    if (prog >= triggerProgress || distToGhost <= 28f)
                    {
                        break;
                    }
                }
                yield return null;
            }

            // ------------------------------------------------------------
            // PHASE 3: BIG TREE AUTO-WALK CINEMATIC TO BEHIND CRYING WOMAN (OPEN GLADE BAY)
            // ------------------------------------------------------------
            SequenceBusy = true;
            try
            {
                LockPlayer();
                objective = "";
                if (ghostWoman)
                {
                    ghostWoman.LockCryBeaconToBody(0.96f);
                    ghostWoman.SetState(GhostAnimState.Crying);
                }

                // Smooth cinematic auto-walk through open glade bay behind the crying woman's back
                Vector3 ghostBackLook = ghostWorldPos + Vector3.up * 0.92f;
                Vector2 wpEntry = ForestNightmareTrail[ForestNightmareTrail.Length - 2];
                Vector2 wpStop  = ForestNightmareTrail[ForestNightmareTrail.Length - 1];

                Vector3 curEye = MainCam ? MainCam.transform.position : new Vector3(wpEntry.x, spawnGroundY + 1.54f, wpEntry.y);
                Vector3 midGladePos = new Vector3(wpEntry.x, GroundHeightAt(wpEntry.x, wpEntry.y) + 1.54f, wpEntry.y);
                Vector3 stopBehindPos = new Vector3(wpStop.x, GroundHeightAt(wpStop.x, wpStop.y) + 1.54f, wpStop.y);

                if (curEye.z < wpEntry.y - 2f)
                {
                    yield return AutoWalkCameraTo(midGladePos, ghostBackLook, 3.0f);
                }
                yield return AutoWalkCameraTo(stopBehindPos, ghostBackLook, 3.2f);

                cam = MainCam;
                float baseYaw = cam ? cam.transform.eulerAngles.y : 0f;
                if (explorer != null)
                {
                    explorer.MovementLocked = true;
                    explorer.ControlsLocked = false;
                    explorer.LookClamped = true;
                    explorer.ClampCenterYaw = baseYaw;
                    explorer.ClampYawRange = 18f;
                    explorer.GlanceLocked = false;
                }

                // ------------------------------------------------------------
                // PHASE 5: BRIDE ASKS -> GHOST RELIVES HER PAIN (ONLY HER BACK VISIBLE)
                // Emotional progression: Love -> Promises -> Betrayal -> Suffering -> Anger
                // ------------------------------------------------------------
                if (narrative) yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_BrideAsksGhostLines());

                if (ghostWoman) ghostWoman.SetState(GhostAnimState.Storytelling);

                // Step slightly closer at the end of Story 1
                if (narrative) yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_GhostStory_LoveAndPromises());
                Vector3 s1Pos = Vector3.Lerp(stopBehindPos, new Vector3(ghostWorldPos.x, stopBehindPos.y, ghostWorldPos.z), 0.16f);
                yield return AutoWalkCameraTo(s1Pos, ghostBackLook, 0.9f);

                // Stage 2: Betrayal & loss (Bride follows at end of story part)
                if (narrative) yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_GhostStory_BetrayalAndSuffering());
                Vector3 s2Pos = Vector3.Lerp(stopBehindPos, new Vector3(ghostWorldPos.x, stopBehindPos.y, ghostWorldPos.z), 0.30f);
                yield return AutoWalkCameraTo(s2Pos, ghostBackLook, 0.9f);

                // Stage 5: Crying stops completely -> sadness becomes pure hatred & anger
                if (ghostWoman) ghostWoman.SetState(GhostAnimState.AngerBuilding);
                if (sfx && clipRumble) sfx.PlayOneShot(clipRumble, 0.80f);
                if (forestFlashlight) forestFlashlight.SetHorrorFlicker(0.38f);
                yield return new WaitForSeconds(0.55f);

                if (narrative) yield return narrative.PlayAndWait(ChapterOneNarrative.Scene00_GhostStory_AngerAndSnap());

                // ------------------------------------------------------------
                // PHASE 6: SLOW TURN + CIRCLE NECK + SCARY SHADOW WALK TO FULL SCREEN
                // ------------------------------------------------------------
                yield return new WaitForSeconds(0.25f);
                if (explorer != null)
                {
                    // Prevent ForestExploration.Update() from fighting our horror camera tracking!
                    explorer.CinematicOverride = true;
                    explorer.LookClamped = false;
                    explorer.ControlsLocked = true;
                    explorer.GlanceLocked = true;
                }
                if (forestFlashlight) forestFlashlight.SetHorrorFlicker(0.75f);

                Vector3 brideEyePos = MainCam ? MainCam.transform.position : stopBehindPos;
                if (ghostWoman)
                {
                    ghostWoman.SetState(GhostAnimState.AngryTurnAndNeckCircle, brideEyePos);
                }

                // Watch in frozen terror as she slowly turns around while twisting/circling her neck!
                float turnWatch = 0f;
                while (turnWatch < 2.75f)
                {
                    turnWatch += Time.deltaTime;
                    if (MainCam && ghostWoman)
                    {
                        Vector3 lookPt = ghostWoman.GetHeadWorldPosition();
                        Quaternion targetR = Quaternion.LookRotation((lookPt - MainCam.transform.position).normalized);
                        MainCam.transform.rotation = Quaternion.Slerp(MainCam.transform.rotation, targetR, Time.deltaTime * 5f);
                    }
                    shake = Mathf.Lerp(0.004f, 0.014f, turnWatch / 2.75f);
                    yield return null;
                }

                // Terrifying pose & stalking walk straight at the Bride until her shadow face fills the screen!
                bool fullScreenReached = false;
                if (forestFlashlight) forestFlashlight.SetHorrorFlicker(1.0f);
                if (ghostWoman)
                {
                    ghostWoman.SetState(GhostAnimState.ScaryWalk, brideEyePos, () => fullScreenReached = true);
                }

                float stalkSafety = 0f;
                while (!fullScreenReached && stalkSafety < 10.5f)
                {
                    stalkSafety += Time.deltaTime;
                    if (MainCam && ghostWoman)
                    {
                        Vector3 facePt = ghostWoman.GetHeadWorldPosition();
                        float dist = Vector3.Distance(MainCam.transform.position, facePt);
                        Quaternion targetR = Quaternion.LookRotation((facePt - MainCam.transform.position).normalized);
                        MainCam.transform.rotation = Quaternion.Slerp(MainCam.transform.rotation, targetR, Time.deltaTime * 10f);
                        shake = Mathf.Lerp(0.008f, 0.055f, Mathf.InverseLerp(6.5f, 0.45f, dist));
                    }
                    yield return null;
                }

                // Split-second terrifying full-screen shadow close-up before instant smash-cut!
                shadowScareFlash = 1.0f;
                shake = 0.09f;
                yield return new WaitForSeconds(0.18f);

                // INSTANT SMASH-CUT TO PURE BLACK
                shadowScareFlash = 0f;
                shake = 0f;
                fade = 1.0f;
                objective = "";
                if (dreamEyeFill) Destroy(dreamEyeFill.gameObject);
                if (ghostWoman) ghostWoman.SetState(GhostAnimState.Hidden);
                if (forestFlashlight) forestFlashlight.HideAfterNightmare(explorer);
                if (ambience) ambience.SetScene(AmbienceScene.Silent);

                yield return new WaitForSeconds(0.85f);

                if (forestNightmareRoot) forestNightmareRoot.gameObject.SetActive(false);
                if (explorer != null)
                {
                    explorer.CinematicOverride = false;
                    explorer.GlanceLocked = false;
                    explorer.LookClamped = false;
                    explorer.ClearTrailCorridor();
                    explorer.RestrictToCompound = true;
                    explorer.FlashlightUnlocked = false;
                    explorer.WalkSpeedMultiplier = 1.0f;
                }
            }
            finally
            {
                shadowScareFlash = 0f;
                shake = 0f;
                if (explorer != null) explorer.CinematicOverride = false;
                if (dreamEyeFill) Destroy(dreamEyeFill.gameObject);
                SequenceBusy = false;
            }
        }

        // ================================================================
        // SCENE 02 — TERRIFIED WAKE UP IN BED -> RINGTONE + MOM SHOUTS OUTSIDE -> ANSWER PHONE
        // ================================================================

        IEnumerator Scene02_WakeUpToPhoneCall(bool fromNightmare = false)
        {
            currentScene = GameScene.Scene02_WakingUpAndPhone;
            isStandingUp = false;

            SetAtmosphere(AtmosphereStage.Dawn4AM);
            if (ambience) ambience.SetScene(AmbienceScene.Dawn);
            if (explorer != null)
            {
                explorer.CinematicOverride = true;
                explorer.ClearTrailCorridor();
                explorer.RestrictToCompound = true;
                explorer.FlashlightUnlocked = false;
                if (explorer.flashlight) explorer.flashlight.enabled = false;
                explorer.WalkSpeedMultiplier = 1.0f;
            }
            if (forestNightmareRoot) forestNightmareRoot.gameObject.SetActive(false);
            if (ghostWoman) ghostWoman.SetState(GhostAnimState.Hidden);
            if (forestFlashlight) forestFlashlight.HideAfterNightmare(explorer);
            if (mother)
            {
                mother.gameObject.SetActive(true);
                mother.MoveToPosition(MotherPosition.LivingRoom);
            }

            // Camera rests on the bridal bed pillow
            Vector3 pillowPos = new Vector3(-9.79f, 24.55f, 25.05f);
            Quaternion pillowRot = Quaternion.Euler(-6f, 138f, 0f);
            var cam = MainCam;
            if (cam) cam.transform.SetPositionAndRotation(pillowPos, pillowRot);
            TeleportPlayer(pillowPos, pillowRot);

            fade = 0f;

            if (fromNightmare)
            {
                // Phone is ALREADY ringing loudly as she jolts awake terrified in bed!
                if (nokia) nokia.TriggerIncomingCall();
                if (sfx && clipShockGasp) sfx.PlayOneShot(clipShockGasp, 0.95f);
                StartCoroutine(ShakeCamera(0.45f, 0.03f));

                if (eyelids)
                {
                    bool shockDone = false;
                    yield return eyelids.PlayShockWakeSequence(
                        onEyesOpen: () => { objective = "Wedding Day — 4:00 AM"; },
                        onFullyAwake: () => { shockDone = true; }
                    );
                    while (!shockDone) yield return null;
                }
                else
                {
                    yield return FadeFromBlack(0.45f);
                    objective = "Wedding Day — 4:00 AM";
                }

                if (explorer != null)
                {
                    explorer.CinematicOverride = false;
                    explorer.SetLookDirection(138f, -6f);
                    explorer.ClampCenterYaw = 138f;
                    explorer.ClampYawRange = 30f;
                    explorer.LookClamped = true;
                    explorer.GlanceLocked = false;
                    explorer.MovementLocked = true;
                    explorer.ControlsLocked = false;
                }

                // Terrified gasp -> Mom shouts loudly from outside because phone has been ringing a long time -> Bride answers Mom!
                if (narrative)
                {
                    yield return narrative.PlayAndWait(ChapterOneNarrative.Scene02_TerrifiedWakeAndMomShout());
                }
            }
            else
            {
                if (eyelids)
                {
                    bool fullyDone = false;
                    yield return eyelids.PlayWakingSequence(
                        onHalfAwake: () =>
                        {
                            objective = "Wedding Day — 4:00 AM";
                            if (nokia) nokia.TriggerIncomingCall();
                        },
                        onFullyAwake: () => { fullyDone = true; }
                    );
                    while (!fullyDone) yield return null;
                }
                else
                {
                    yield return FadeFromBlack(1.6f);
                    objective = "Wedding Day — 4:00 AM";
                    if (nokia) nokia.TriggerIncomingCall();
                }

                if (explorer != null)
                {
                    explorer.SetLookDirection(138f, -6f);
                    explorer.ClampCenterYaw = 138f;
                    explorer.ClampYawRange = 25f;
                    explorer.LookClamped = true;
                    explorer.GlanceLocked = false;
                    explorer.MovementLocked = true;
                    explorer.ControlsLocked = false;
                }

                if (narrative)
                {
                    yield return narrative.PlayAndWait(ChapterOneNarrative.Scene01_WakingDialogue());
                }
            }

            if (explorer != null)
            {
                explorer.LookClamped = false;
                explorer.ControlsLocked = true;
            }

            // Bride rises from bed and goes straight to answer the ringing phone
            yield return RiseFromBed();

            isStandingUp = true;
            UnlockPlayer();

            if (!fromNightmare)
            {
                if (explorer != null) explorer.MovementLocked = true;
                objective = "Look around";

                float lookActiveTime = 0f;
                float lookSafety = 0f;
                while (lookActiveTime < 2.0f && lookSafety < 10.0f)
                {
                    lookSafety += Time.deltaTime;
                    float mouseDelta = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));
                    if (mouseDelta > 0.02f) lookActiveTime += Time.deltaTime;
                    yield return null;
                }

                if (explorer != null) explorer.MovementLocked = false;
            }

            hasMovedAround = true;
            objective = "Answer Brother's call at the bedroom mirror table";
            yield return new WaitUntil(() => currentScene != GameScene.Scene02_WakingUpAndPhone);
        }

        IEnumerator RiseFromBed()
        {
            var cam = MainCam;
            if (!cam) yield break;

            // Lock ForestExploration camera override so rig never snaps camera back during rise!
            if (explorer != null) explorer.CinematicOverride = true;
            try
            {
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
            finally
            {
                if (explorer != null) explorer.CinematicOverride = false;
            }
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
            if (bridalDoor && bridalDoor.isOpen) OnBedroomDoorOpened();
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
            objective = "Meet Mother in the living room";
            if (ambience) ambience.SetScene(AmbienceScene.Morning);

            // Wait until player approaches the open bedroom door
            HingedOpening door = bridalDoor;
            Vector3 doorPos = door ? door.transform.position : new Vector3(-7.22f, 23.70f, 21.08f);

            bool autoTriggered = false;
            while (!autoTriggered && currentScene == GameScene.Scene04_LivingRoom)
            {
                var cam = MainCam;
                if (cam != null && (door == null || door.isOpen))
                {
                    float distToDoor = Vector3.Distance(new Vector3(cam.transform.position.x, 0, cam.transform.position.z), new Vector3(doorPos.x, 0, doorPos.z));
                    float distToMother = mother != null ? Vector3.Distance(cam.transform.position, mother.transform.position) : 99f;

                    // When player walks near the open doorway or steps into the hall: auto-trigger conversation!
                    if (distToDoor < 2.2f || distToMother < 2.5f || cam.transform.position.x > -6.8f)
                    {
                        autoTriggered = true;
                        break;
                    }
                }
                yield return null;
            }

            if (currentScene == GameScene.Scene04_LivingRoom)
            {
                yield return PlayLivingRoomCinematicAndDialogue();
            }
        }

        public void OnMotherTalkedLivingRoom()
        {
            if (currentScene == GameScene.Scene04_LivingRoom)
            {
                BeginSequence(PlayLivingRoomCinematicAndDialogue());
            }
        }

        IEnumerator PlayLivingRoomCinematicAndDialogue()
        {
            objective = "Mother is speaking with you";
            LockPlayer();

            var cam = MainCam;
            if (cam != null && mother != null)
            {
                Vector3 startP = cam.transform.position;
                Quaternion startR = cam.transform.rotation;

                Vector3 targetP = mother.transform.position + mother.transform.forward * 1.35f;
                targetP.y = 25.24f; // Standing eye height
                Vector3 lookTarget = mother.head ? mother.head.position : (mother.transform.position + Vector3.up * 1.55f);
                Quaternion targetR = Quaternion.LookRotation((lookTarget - targetP).normalized);

                // Smooth cinematic walk towards Mother (2.0s) with CinematicOverride so rig never fights camera
                if (explorer != null) explorer.CinematicOverride = true;
                float dur = 2.0f;
                for (float t = 0; t < dur; t += Time.deltaTime)
                {
                    float smooth = Mathf.SmoothStep(0f, 1f, t / dur);
                    cam.transform.position = Vector3.Lerp(startP, targetP, smooth);
                    cam.transform.rotation = Quaternion.Slerp(startR, targetR, smooth);
                    yield return null;
                }
                if (explorer != null) explorer.CinematicOverride = false;
                TeleportPlayer(targetP, targetR);

                // Enable glance left and right during dialogue so player can look at Mother and surroundings
                float centerYaw = targetR.eulerAngles.y;
                if (explorer != null)
                {
                    explorer.MovementLocked = true;
                    explorer.ControlsLocked = false;
                    explorer.LookClamped = true;
                    explorer.ClampCenterYaw = centerYaw;
                    explorer.ClampYawRange = 25f;
                    explorer.GlanceLocked = false;
                    explorer.SetLookDirection(centerYaw, 0f);
                }
            }

            // Automatic dialogue with Mother (no click required)
            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene03_LivingAreaLines());
            }

            // After conversation ends, Mother disappears to downstairs wedding preparations
            if (mother) mother.gameObject.SetActive(false);

            if (explorer != null)
            {
                explorer.LookClamped = false;
                explorer.MovementLocked = false;
            }
            UnlockPlayer();

            currentScene = GameScene.Scene05_Preparation;
            objective = "Go to the kitchen to wash your face (0/3)";
        }

        public void OnFaceWashComplete()
        {
            objective = "Return to bedroom mirror table to do makeup";
            StartCoroutine(WaitForReturnToBedroom());
        }

        IEnumerator WaitForReturnToBedroom()
        {
            HingedOpening door = bridalDoor;
            Vector3 doorway = new Vector3(-6.75f, 25.24f, 21.10f);

            bool entered = false;
            while (!entered && currentScene == GameScene.Scene05_Preparation)
            {
                var cam = MainCam;
                if (cam != null)
                {
                    // Must be upstairs in the house (Y > 24.5m) to prevent triggering from downstairs/outside
                    if (cam.transform.position.y > 24.5f)
                    {
                        float distToDoor = Vector2.Distance(
                            new Vector2(cam.transform.position.x, cam.transform.position.z),
                            new Vector2(doorway.x, doorway.z)
                        );

                        // If approaching within 2.0m outside, automatically open the door
                        if (distToDoor < 2.0f && door != null && !door.isOpen)
                        {
                            door.Open();
                        }

                        // Trigger only when player reaches the actual entrance of the bedroom door!
                        // Inside bedroom is Z >= 21.20f, threshold is distToDoor < 0.95f
                        bool atDoorway = (distToDoor < 0.95f) || 
                                         (cam.transform.position.z >= 21.20f && cam.transform.position.x >= -7.4f && cam.transform.position.x <= -6.1f);

                        if (atDoorway)
                        {
                            entered = true;
                            break;
                        }
                    }
                }
                yield return null;
            }

            if (currentScene == GameScene.Scene05_Preparation)
            {
                // Ensure the door is open so the path into the bedroom is clear
                if (door != null && !door.isOpen) door.Open();
                BeginSequence(PlayPreparationSequence());
            }
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
            objective = "Preparing bridal makeup with Mother";
            LockPlayer();

            GameObject dressingTable = preparationStation ? preparationStation.gameObject
                : (MakeupMirrorStation.Instance ? MakeupMirrorStation.Instance.gameObject
                : GameObject.Find("Bridal dressing table mirror and jewelry casket"));
            var cam = MainCam;

            // Ensure dressing chair exists in front of the mirror table
            if (MakeupMirrorStation.Instance != null)
            {
                MakeupMirrorStation.Instance.EnsureChair();
            }

            if (dressingTable != null && cam != null)
            {
                // Seated on the wooden chair directly at the dressing table
                Vector3 seatPos = new Vector3(-4.534f, 24.650f, 23.796f);

                // Position Mother to the LEFT of the player beside the dressing table mirror facing the bride
                Vector3 motherPos = new Vector3(-4.000f, 23.700f, 24.500f);
                if (mother != null)
                {
                    mother.gameObject.SetActive(true);
                    mother.transform.position = motherPos;
                    Vector3 toBride = (seatPos - motherPos);
                    toBride.y = 0;
                    mother.transform.rotation = Quaternion.LookRotation(toBride.normalized);
                }

                // Mother's face position
                Vector3 motherFaceTarget = motherPos + Vector3.up * 1.55f;

                // Aim directly at Mother's face so she is clearly seen in front of the bride
                Vector3 lookAtMotherDir = (motherFaceTarget - seatPos).normalized;
                Quaternion seatRot = Quaternion.LookRotation(lookAtMotherDir);

                Vector3 startP = cam.transform.position;
                Quaternion startR = cam.transform.rotation;

                // Waypoint navigation through bedroom door into chair so no wall clipping occurs
                Vector3 wpDoorway = new Vector3(-6.75f, 25.24f, 21.40f); // Just inside the doorway
                Vector3 wpChairFront = new Vector3(-4.534f, 25.24f, 23.796f); // In front of chair standing

                // Smooth cinematic walk from the bedroom doorway over to the chair and sit down
                float dur = 2.6f;
                for (float t = 0; t < dur; t += Time.deltaTime)
                {
                    float u = Mathf.Clamp01(t / dur);
                    float smooth = Mathf.SmoothStep(0f, 1f, u);

                    // Interpolate position along natural path from door entrance -> chair front -> sit down
                    Vector3 currentP;
                    if (smooth < 0.40f)
                    {
                        float pSeg = smooth / 0.40f;
                        currentP = Vector3.Lerp(startP, wpDoorway, pSeg);
                    }
                    else if (smooth < 0.75f)
                    {
                        float pSeg = (smooth - 0.40f) / 0.35f;
                        currentP = Vector3.Lerp(wpDoorway, wpChairFront, pSeg);
                    }
                    else
                    {
                        float pSeg = (smooth - 0.75f) / 0.25f;
                        currentP = Vector3.Lerp(wpChairFront, seatPos, pSeg);
                    }

                    Quaternion currentR = Quaternion.Slerp(startR, seatRot, smooth);

                    if (explorer != null)
                    {
                        explorer.MoveRigAndCamera(currentP, currentR, seated: smooth >= 0.75f);
                    }
                    else
                    {
                        cam.transform.SetPositionAndRotation(currentP, currentR);
                    }

                    yield return null;
                }

                TeleportPlayer(seatPos, seatRot, seated: true);

                // Enable gentle glance centered between Mother (37 deg) and Mirror (72 deg)
                // Midpoint is ~54.5 deg, range +/- 25 deg covers both Mother and the dressing mirror!
                float centerSeatYaw = 54.5f;
                float faceMotherYaw = seatRot.eulerAngles.y;
                float faceMotherPitch = Mathf.DeltaAngle(0, seatRot.eulerAngles.x);

                if (explorer != null)
                {
                    explorer.MovementLocked = true;
                    explorer.ControlsLocked = true;
                    explorer.LookClamped = true;
                    explorer.ClampCenterYaw = centerSeatYaw;
                    explorer.ClampYawRange = 25f;
                    explorer.GlanceLocked = false;
                    // Face Mother directly at the start of dialogue!
                    explorer.SetLookDirection(faceMotherYaw, faceMotherPitch);
                }
            }

            // Play the emotional and encouraging conversation with Mother at the mirror table
            if (narrative)
            {
                yield return narrative.PlayAndWait(ChapterOneNarrative.Scene05_PreparationLines());
            }

            // All scenes after Mother's makeup conversation are removed — conclude Chapter 1 demo cleanly
            LockPlayer();
            yield return FadeToBlack(1.5f);
            objective = "";
            titleCard = true;
            yield return new WaitForSeconds(6.0f);
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

    }
}
