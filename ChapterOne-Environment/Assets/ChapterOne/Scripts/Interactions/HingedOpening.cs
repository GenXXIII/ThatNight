// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// HingedOpening.cs — Doors, Windows, Wardrobes
//
// Interactive doors with tactile grab [E], hold to open/close [Hold E],
// snappy response at 58 deg/sec, 70% auto-complete threshold with auto-release,
// spring-back if released under 70%, and [Esc] to exit grab.
// Bridal bedroom door stays locked/hidden until Brother's phone call completes.
// ============================================================================

using System;
using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class HingedOpening : WorldInteractable
    {
        [Header("Opening")]
        public float closedAngle = 0f;
        public float openAngle = 95f;
        public bool isOpen = false;
        public float currentAngle = 0f;
        public float openSpeed = 40f; // Weighty, realistic door swing (~2.2s for 90 deg)

        [Header("Auto Close")]
        public bool enableAutoClose = true;
        public float autoCloseDelay = 60f; // Automatically closes after 1 minute (60s) of being open
        float openTimer = 0f;

        [Header("Interaction")]
        public Vector3 handleLocal = new Vector3(0.85f, 1.05f, 0.05f);
        public bool triggerDirectorOnOpen; // For the bridal bedroom door

        [Header("Double Door Link")]
        public HingedOpening partnerDoor;
        bool isSyncingPartner = false;

        public bool isGrabbed { get; private set; }
        public bool isBeingHeld => isGrabbed; // Compatibility alias
        public bool Moving { get; private set; }

        public float startAngle { get; private set; }
        public float targetAngle { get; private set; }

        Coroutine moveCoroutine;

        public override float Reach => 2.2f; // Comfortable reach (0m to 2.2m) when interacting with doors/wardrobes

        public override bool CanUse
        {
            get
            {
                // Bridal bedroom door remains locked and non-interactable until Brother's call finishes!
                if (triggerDirectorOnOpen || (displayName != null && displayName.ToLower().Contains("bridal")))
                {
                    if (ChapterOneDirector.Instance != null && (int)ChapterOneDirector.Instance.currentScene < (int)GameScene.Scene03_MotherCallsDoor)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override string Prompt => isGrabbed
            ? ("Holding to " + (isOpen ? "Close " : "Open ") + displayName)
            : ("Hold [Left Click] to " + (isOpen ? "Close " : "Open ") + displayName);

        void Start()
        {
            float y = transform.localEulerAngles.y;
            if (y > 180f) y -= 360f;
            currentAngle = y;

            float distOpen = Mathf.Abs(currentAngle - openAngle);
            float distClosed = Mathf.Abs(currentAngle - closedAngle);
            isOpen = distOpen < distClosed;
        }

        void Update()
        {
            // Auto-close after 1 minute (60s) of being open
            if (enableAutoClose && isOpen && !isGrabbed && !Moving)
            {
                openTimer += Time.deltaTime;
                if (openTimer >= autoCloseDelay)
                {
                    openTimer = 0f;
                    Close();
                }
            }
            else
            {
                openTimer = 0f;
            }
        }

        public void Close()
        {
            if (!isOpen || Moving || isGrabbed) return;
            startAngle = currentAngle;
            targetAngle = closedAngle;
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(AutoGlide(targetAngle, false, true));

            if (partnerDoor != null && !isSyncingPartner && partnerDoor.isOpen)
            {
                isSyncingPartner = true;
                partnerDoor.Close();
                isSyncingPartner = false;
            }
        }

        public void Open()
        {
            if (isOpen || Moving || isGrabbed) return;
            startAngle = currentAngle;
            targetAngle = openAngle;
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(AutoGlide(targetAngle, true, true));

            if (partnerDoor != null && !isSyncingPartner && !partnerDoor.isOpen)
            {
                isSyncingPartner = true;
                partnerDoor.Open();
                isSyncingPartner = false;
            }
        }

        public void Grab(PlayerInteraction player)
        {
            if (Moving) return;
            if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }

            isGrabbed = true;
            openTimer = 0f;
            startAngle = currentAngle;
            targetAngle = isOpen ? closedAngle : openAngle;
        }

        public void HoldMove(PlayerInteraction player, float deltaTime)
        {
            if (!isGrabbed || Moving) return;

            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, openSpeed * deltaTime);
            transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);

            float totalDist = Mathf.Abs(targetAngle - startAngle);
            float currentDist = Mathf.Abs(currentAngle - startAngle);
            float progress = totalDist > 0.01f ? (currentDist / totalDist) : 1f;

            // 40% Threshold auto-complete and auto-release
            if (progress >= 0.40f)
            {
                TriggerAutoFinish(player);
            }
        }

        void TriggerAutoFinish(PlayerInteraction player)
        {
            isGrabbed = false;
            if (player != null)
            {
                player.OnDoorAutoReleased();
            }

            bool goalIsOpen = (targetAngle == openAngle);
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(AutoGlide(targetAngle, goalIsOpen, true));

            if (partnerDoor != null && !isSyncingPartner)
            {
                isSyncingPartner = true;
                if (partnerDoor.isOpen != goalIsOpen)
                {
                    partnerDoor.targetAngle = goalIsOpen ? partnerDoor.openAngle : partnerDoor.closedAngle;
                    if (partnerDoor.moveCoroutine != null) partnerDoor.StopCoroutine(partnerDoor.moveCoroutine);
                    partnerDoor.moveCoroutine = partnerDoor.StartCoroutine(partnerDoor.AutoGlide(partnerDoor.targetAngle, goalIsOpen, true));
                }
                isSyncingPartner = false;
            }
        }

        public void OnHoldReleased()
        {
            if (!isGrabbed || Moving) return;

            float totalDist = Mathf.Abs(targetAngle - startAngle);
            float currentDist = Mathf.Abs(currentAngle - startAngle);
            float progress = totalDist > 0.01f ? (currentDist / totalDist) : 1f;

            if (progress >= 0.40f)
            {
                bool goalIsOpen = (targetAngle == openAngle);
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(AutoGlide(targetAngle, goalIsOpen, true));
            }
            else if (currentDist > 0.1f)
            {
                bool goalIsOpen = (startAngle == openAngle);
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(AutoGlide(startAngle, goalIsOpen, false));
            }
            isGrabbed = false;
        }

        public void ExitGrab(PlayerInteraction player)
        {
            if (!isGrabbed) return;
            isGrabbed = false;

            if (player != null)
            {
                player.ReleaseGrabMovementLock();
            }

            float totalDist = Mathf.Abs(targetAngle - startAngle);
            float currentDist = Mathf.Abs(currentAngle - startAngle);
            float progress = totalDist > 0.01f ? (currentDist / totalDist) : 1f;

            if (progress < 0.40f && currentDist > 0.1f)
            {
                bool goalIsOpen = (startAngle == openAngle);
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(AutoGlide(startAngle, goalIsOpen, false));
            }
        }

        public void Ungrab(PlayerInteraction player)
        {
            ExitGrab(player);
        }

        IEnumerator AutoGlide(float goalAngle, bool finalIsOpen, bool isCompleting)
        {
            Moving = true;
            Quaternion startRot = transform.localRotation;
            Quaternion endRot = Quaternion.Euler(0f, goalAngle, 0f);

            float dist = Mathf.Abs(goalAngle - currentAngle);
            float duration = Mathf.Clamp(dist / Mathf.Max(openSpeed, 30f), 0.15f, 1.8f);

            for (float t = 0; t < 1f; t += Time.deltaTime / duration)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t);
                currentAngle = Mathf.Lerp(currentAngle, goalAngle, smooth);
                transform.localRotation = Quaternion.Slerp(startRot, endRot, smooth);

                if (isCompleting && triggerDirectorOnOpen && Mathf.Abs(currentAngle) >= Mathf.Abs(openAngle) * 0.60f)
                {
                    CheckDirectorTrigger();
                }

                yield return null;
            }

            currentAngle = goalAngle;
            transform.localRotation = endRot;
            isOpen = finalIsOpen;
            Moving = false;
            moveCoroutine = null;

            if (isCompleting && isOpen && triggerDirectorOnOpen)
            {
                CheckDirectorTrigger();
            }
        }

        public override void Use(PlayerInteraction player)
        {
            if (Moving) return;
            if (isGrabbed)
            {
                ExitGrab(player);
                return;
            }
            startAngle = currentAngle;
            targetAngle = isOpen ? closedAngle : openAngle;
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(AutoGlide(targetAngle, !isOpen, true));

            if (partnerDoor != null && !isSyncingPartner)
            {
                isSyncingPartner = true;
                if (partnerDoor.isOpen != !isOpen)
                {
                    partnerDoor.Use(player);
                }
                isSyncingPartner = false;
            }
        }

        void CheckDirectorTrigger()
        {
            if (triggerDirectorOnOpen && ChapterOneDirector.Instance != null)
            {
                ChapterOneDirector.Instance.OnBedroomDoorOpened();
            }
        }
    }
}
