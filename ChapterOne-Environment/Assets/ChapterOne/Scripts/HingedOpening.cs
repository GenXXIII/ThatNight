// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// HingedOpening.cs — Doors, Windows, Wardrobes
//
// Interactive doors with real human hand grip & continuous hold [E] opening.
// Player can hold [E] to slowly swing the door open to max, or tap [E].
// ============================================================================

using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class HingedOpening : WorldInteractable
    {
        [Header("Opening")]
        public float closedAngle;
        public float openAngle = 95f;
        public bool isOpen = false;
        public float currentAngle = 0f;
        public float openSpeed = 18f; // Slow, heavy solid teak door swing (18 deg/sec)

        [Header("Interaction")]
        public Vector3 handleLocal = new Vector3(0.85f, 1.05f, 0.05f);
        public bool triggerDirectorOnOpen;  // For the bridal bedroom door

        public bool isBeingHeld { get; private set; }
        public bool Moving { get; private set; }

        Coroutine finishCoroutine;

        public override float Reach => 2.20f; // Comfortable arm reach for doors & wardrobes
        public override string Prompt => (isOpen ? "Hold [E] to Close " : "Hold [E] to Open ") + displayName;

        void Start()
        {
            // Normalize initial angle
            float y = transform.localEulerAngles.y;
            if (y > 180f) y -= 360f;
            currentAngle = y;

            float distOpen = Mathf.Abs(currentAngle - openAngle);
            float distClosed = Mathf.Abs(currentAngle - closedAngle);
            isOpen = distOpen < distClosed;
        }

        public void OnStartHold(PlayerInteraction player)
        {
            if (Moving) return;
            if (finishCoroutine != null) { StopCoroutine(finishCoroutine); finishCoroutine = null; }

            isBeingHeld = true;
            Vector3 handleWorld = transform.TransformPoint(handleLocal);
            if (player.body != null)
            {
                player.body.StartDoorGrip(handleWorld);
            }
        }

        public void OnContinueHold(PlayerInteraction player, float deltaTime)
        {
            if (!isBeingHeld) return;

            float target = isOpen ? closedAngle : openAngle;
            currentAngle = Mathf.MoveTowards(currentAngle, target, openSpeed * deltaTime);
            transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);

            Vector3 handleWorld = transform.TransformPoint(handleLocal);
            if (player.body != null)
            {
                player.body.UpdateDoorGrip(handleWorld);
            }

            // Check if reached max limit
            if (Mathf.Abs(currentAngle - target) < 0.5f)
            {
                currentAngle = target;
                isOpen = !isOpen;
                isBeingHeld = false;
                if (player.body != null) player.body.ReleaseDoorGrip();

                CheckDirectorTrigger();
            }
            else if (triggerDirectorOnOpen && Mathf.Abs(currentAngle) >= Mathf.Abs(openAngle) * 0.60f)
            {
                CheckDirectorTrigger();
            }
        }

        public void OnReleaseHold(PlayerInteraction player)
        {
            if (!isBeingHeld) return;
            isBeingHeld = false;

            if (player.body != null)
            {
                player.body.ReleaseDoorGrip();
            }

            // If released before reaching max, it springs smoothly back to its starting position!
            float returnTarget = isOpen ? openAngle : closedAngle;
            if (finishCoroutine != null) StopCoroutine(finishCoroutine);
            finishCoroutine = StartCoroutine(SpringBack(returnTarget));
        }

        IEnumerator SpringBack(float target)
        {
            float start = currentAngle;
            float totalDist = Mathf.Abs(target - start);
            float duration = Mathf.Clamp(totalDist / (openSpeed * 1.5f), 0.2f, 1.8f);

            for (float t = 0; t < 1f; t += Time.deltaTime / duration)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t);
                currentAngle = Mathf.Lerp(start, target, smooth);
                transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
                yield return null;
            }
            currentAngle = target;
            transform.localRotation = Quaternion.Euler(0f, target, 0f);
        }

        public override void Use(PlayerInteraction player)
        {
            if (Moving || isBeingHeld) return;
            if (finishCoroutine != null) StopCoroutine(finishCoroutine);
            finishCoroutine = StartCoroutine(FinishSwing(!isOpen, player));
        }

        IEnumerator FinishSwing(bool toOpen, PlayerInteraction player = null)
        {
            Moving = true;
            float target = toOpen ? openAngle : closedAngle;
            Quaternion start = transform.localRotation;
            Quaternion goal = Quaternion.Euler(0f, target, 0f);

            float totalDist = Mathf.Abs(target - currentAngle);
            float duration = Mathf.Clamp(totalDist / openSpeed, 0.45f, 2.6f);

            if (player != null && player.body != null)
            {
                player.body.StartDoorGrip(transform.TransformPoint(handleLocal));
            }

            for (float t = 0; t < 1f; t += Time.deltaTime / duration)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t);
                transform.localRotation = Quaternion.Slerp(start, goal, smooth);
                currentAngle = Mathf.Lerp(currentAngle, target, smooth);
                if (player != null && player.body != null)
                {
                    player.body.UpdateDoorGrip(transform.TransformPoint(handleLocal));
                }
                yield return null;
            }

            transform.localRotation = goal;
            currentAngle = target;
            isOpen = toOpen;
            Moving = false;
            finishCoroutine = null;

            if (player != null && player.body != null)
            {
                player.body.ReleaseDoorGrip();
            }

            if (toOpen) CheckDirectorTrigger();
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
