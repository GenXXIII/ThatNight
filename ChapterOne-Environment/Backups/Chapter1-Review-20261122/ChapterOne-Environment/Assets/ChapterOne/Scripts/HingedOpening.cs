// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// HingedOpening.cs — Doors, Windows, Wardrobes
//
// Opens/closes on interaction with hand-reach animation.
// Optionally triggers Director events (e.g. bedroom door → scene advance).
// ============================================================================

using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class HingedOpening : WorldInteractable
    {
        [Header("Opening")]
        public float closedAngle;
        public float openAngle = 95;
        public bool isOpen = true;

        [Header("Interaction")]
        public Vector3 handleLocal;
        public bool triggerDirectorOnOpen;  // For the bridal bedroom door

        public bool Moving { get; private set; }

        public override string Prompt => (isOpen ? "Close " : "Open ") + displayName;

        public override void Use(PlayerInteraction player)
        {
            if (Moving) return;
            player.ReachAndAct(transform.TransformPoint(handleLocal), () => StartCoroutine(Swing()));
        }

        IEnumerator Swing()
        {
            Moving = true;
            bool next = !isOpen;

            Quaternion start = transform.localRotation;
            Quaternion goal = Quaternion.Euler(0, next ? openAngle : closedAngle, 0);

            for (float t = 0; t < 1; t += Time.deltaTime / 0.65f)
            {
                transform.localRotation = Quaternion.Slerp(start, goal, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
            transform.localRotation = goal;
            isOpen = next;
            Moving = false;

            // Notify Director when bridal bedroom door opens
            if (triggerDirectorOnOpen && isOpen && ChapterOneDirector.Instance != null)
            {
                ChapterOneDirector.Instance.OnBedroomDoorOpened();
            }
        }
    }
}
