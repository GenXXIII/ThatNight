using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed partial class ChapterOneDirector
    {
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

            if (eyelids)
            {
                bool fullyDone = false;
                yield return eyelids.PlayWakingSequence(
                    onHalfAwake: () => 
                    { 
                        objective = "Wedding Day — 4:00 AM";
                        // Ringtone begins ringing only as the player opens their eyes!
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

            // Bride murmurs groggily as she opens her eyes (glance left and right slightly from the pillow)
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

            if (explorer != null)
            {
                explorer.LookClamped = false;
                explorer.ControlsLocked = true;
            }

            // Bride rises from bed (camera sits up and rises to standing, but body remains hidden so no glitch is visible)
            yield return RiseFromBed();

            // Once standing upright on the wooden floor beside the bed, activate body and unlock player controls
            isStandingUp = true;
            UnlockPlayer();

            // 1. First tutorial prompt: Look around (glance/look around for 2 seconds)
            // Keep movement locked briefly so player focuses on looking around first
            if (explorer != null) explorer.MovementLocked = true;
            objective = "Look around";

            float lookActiveTime = 0f;
            float lookSafety = 0f;
            while (lookActiveTime < 2.0f && lookSafety < 10.0f)
            {
                lookSafety += Time.deltaTime;
                float mouseDelta = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));
                if (mouseDelta > 0.02f)
                {
                    lookActiveTime += Time.deltaTime;
                }
                yield return null;
            }

            // Enable movement for directional WASD walk tutorials
            if (explorer != null) explorer.MovementLocked = false;

            // 2. Move Forward (W) - 1 second
            objective = "Move Forward (W)";
            float fwdTime = 0f;
            float fwdSafety = 0f;
            while (fwdTime < 1.0f && fwdSafety < 12.0f)
            {
                fwdSafety += Time.deltaTime;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetAxisRaw("Vertical") > 0.2f)
                {
                    fwdTime += Time.deltaTime;
                }
                yield return null;
            }

            // 3. Move Backward (S) - 1 second
            objective = "Move Backward (S)";
            float backTime = 0f;
            float backSafety = 0f;
            while (backTime < 1.0f && backSafety < 12.0f)
            {
                backSafety += Time.deltaTime;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") < -0.2f)
                {
                    backTime += Time.deltaTime;
                }
                yield return null;
            }

            // 4. Move Left (A) - 1 second
            objective = "Move Left (A)";
            float leftTime = 0f;
            float leftSafety = 0f;
            while (leftTime < 1.0f && leftSafety < 12.0f)
            {
                leftSafety += Time.deltaTime;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetAxisRaw("Horizontal") < -0.2f)
                {
                    leftTime += Time.deltaTime;
                }
                yield return null;
            }

            // 5. Move Right (D) - 1 second
            objective = "Move Right (D)";
            float rightTime = 0f;
            float rightSafety = 0f;
            while (rightTime < 1.0f && rightSafety < 12.0f)
            {
                rightSafety += Time.deltaTime;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetAxisRaw("Horizontal") > 0.2f)
                {
                    rightTime += Time.deltaTime;
                }
                yield return null;
            }

            hasMovedAround = true;

            // 6. Now transition to the phone call task
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
            objective = "Meet Mother in the living room";
            if (ambience) ambience.SetScene(AmbienceScene.Morning);

            // Wait until player approaches the open bedroom door
            var doors = FindObjectsByType<HingedOpening>(FindObjectsSortMode.None);
            HingedOpening door = null;
            foreach (var d in doors)
            {
                if (d.triggerDirectorOnOpen) { door = d; break; }
            }
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

                // Smooth cinematic walk towards Mother (2.0s)
                float dur = 2.0f;
                for (float t = 0; t < dur; t += Time.deltaTime)
                {
                    float smooth = Mathf.SmoothStep(0f, 1f, t / dur);
                    cam.transform.position = Vector3.Lerp(startP, targetP, smooth);
                    cam.transform.rotation = Quaternion.Slerp(startR, targetR, smooth);
                    yield return null;
                }
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
            var doors = FindObjectsByType<HingedOpening>(FindObjectsSortMode.None);
            HingedOpening door = null;
            foreach (var d in doors)
            {
                if (d.triggerDirectorOnOpen) { door = d; break; }
            }
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

            GameObject dressingTable = null;
            foreach (var go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go.name == "Bridal dressing table mirror and jewelry casket" && go.transform.position.x < 0)
                {
                    dressingTable = go;
                    break;
                }
            }
            if (dressingTable == null) dressingTable = GameObject.Find("Bridal dressing table mirror and jewelry casket");
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
