// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// MakeupMirrorStation.cs — Bridal Dressing Table Mirror & Jewelry Box
//
// Interactive makeup station at the bridal dressing table mirror.
// After washing face, bride prepares her wedding makeup and reflects on Mom.
// ============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class MakeupMirrorStation : WorldInteractable
    {
        public static MakeupMirrorStation Instance { get; private set; }

        bool isPreparing = false;

        public override float Reach => 2.2f;
        public override bool CanUse => ChapterOneDirector.Instance != null && 
                                       ChapterOneDirector.Instance.currentScene == GameScene.Scene05_Preparation && 
                                       FaceWashStation.Instance != null &&
                                       FaceWashStation.Instance.washCount >= FaceWashStation.RequiredWashes &&
                                       !isPreparing;

        public override string Prompt => "[Left Click] Apply Wedding Makeup";

        void Awake()
        {
            Instance = this;
        }

        public override void Use(PlayerInteraction player)
        {
            if (isPreparing) return;
            isPreparing = true;
            if (ChapterOneDirector.Instance != null)
            {
                ChapterOneDirector.Instance.OnPreparationStarted();
            }
        }
    }
}
