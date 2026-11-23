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

            objective = "Move around the bedroom";

            // Wait for player to move around first
            Vector3 startP = player != null ? player.transform.position : (MainCam ? MainCam.transform.position : Vector3.zero);
            float moveWait = 0f;
            while (!hasMovedAround && moveWait < 5.0f)
            {
                moveWait += Time.deltaTime;
                Vector3 curP = player != null ? player.transform.position : (MainCam ? MainCam.transform.position : Vector3.zero);
                if (Vector3.Distance(new Vector3(curP.x, 0, curP.z), new Vector3(startP.x, 0, startP.z)) > 0.7f)
                {
                    hasMovedAround = true;
                }
                yield return null;
            }
            hasMovedAround = true;

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

            // Mother disappears (walks away / goes downstairs to wedding preparations)
            if (mother) mother.gameObject.SetActive(false);

            currentScene = GameScene.Scene05_Preparation;
            objective = "Go to the kitchen to wash your face (0/3)";
        }

        public void OnFaceWashComplete()
        {
            objective = "Return to bedroom mirror table to do makeup";
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
            objective = "Applying bridal makeup";
            LockPlayer();
            yield return FadeToBlack(0.6f);

            var dressingTable = GameObject.Find("Bridal dressing table mirror and jewelry casket");
            if (dressingTable != null && MainCam)
            {
                Vector3 seatPos = dressingTable.transform.position - dressingTable.transform.forward * 0.70f + Vector3.up * 0.50f;
                Quaternion seatRot = Quaternion.LookRotation(dressingTable.transform.position - seatPos);
                TeleportPlayer(seatPos, seatRot);
            }

            // Position Mother beside the dressing table mirror for the emotional conversation
            if (mother != null && dressingTable != null)
            {
                mother.gameObject.SetActive(true);
                mother.transform.position = dressingTable.transform.position + dressingTable.transform.right * 0.70f - dressingTable.transform.forward * 0.20f;
                mother.transform.rotation = Quaternion.LookRotation(-dressingTable.transform.right);
            }

            yield return FadeFromBlack(0.6f);

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
