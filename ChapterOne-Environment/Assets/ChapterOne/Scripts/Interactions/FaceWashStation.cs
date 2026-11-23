// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// FaceWashStation.cs — Traditional Khmer Kitchen Water Basin
//
// Interactive wash station in the outdoor kitchen.
// Player washes face 3 times with cool well water to wake up for the wedding.
// ============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class FaceWashStation : WorldInteractable
    {
        public static FaceWashStation Instance { get; private set; }

        public int washCount = 0;
        public const int RequiredWashes = 3;
        bool isWashing = false;

        public override float Reach => 2.2f;
        public override bool CanUse => ChapterOneDirector.Instance != null && 
                                       ChapterOneDirector.Instance.currentScene == GameScene.Scene05_Preparation && 
                                       washCount < RequiredWashes && !isWashing;

        public override string Prompt => washCount < RequiredWashes 
            ? $"[Left Click] Wash Face ({washCount}/{RequiredWashes})" 
            : "Face Washed Clean";

        void Awake()
        {
            Instance = this;
        }

        public override void Use(PlayerInteraction player)
        {
            if (isWashing || washCount >= RequiredWashes) return;
            StartCoroutine(PerformWash(player));
        }

        IEnumerator PerformWash(PlayerInteraction player)
        {
            isWashing = true;
            washCount++;

            // Brief eyelid blink / wipe splash effect
            var eyelids = FindFirstObjectByType<EyelidOverlay>();
            if (eyelids != null)
            {
                eyelids.Blink(0.40f);
            }

            // Update Director objective
            if (ChapterOneDirector.Instance != null)
            {
                ChapterOneDirector.Instance.objective = $"Go to the kitchen to wash your face ({washCount}/{RequiredWashes})";
            }

            yield return new WaitForSeconds(0.45f);

            // Monologue thought based on wash count
            string thought = washCount switch
            {
                1 => "The cold well water shocks the sleep from my eyes...",
                2 => "Mom stayed up all night preparing for today... I will make her proud.",
                3 => "My face is awake and refreshed. Now I must return to the bedroom mirror to do my makeup.",
                _ => ""
            };

            var narrative = ChapterOneNarrative.Instance;
            if (narrative != null && !string.IsNullOrEmpty(thought))
            {
                yield return narrative.PlayAndWait(new List<DialogueLine> {
                    new DialogueLine(DialogueSpeaker.Bride, "Bride", thought, 3.2f)
                });
            }

            isWashing = false;

            if (washCount >= RequiredWashes && ChapterOneDirector.Instance != null)
            {
                ChapterOneDirector.Instance.OnFaceWashComplete();
            }
        }
    }
}
