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

        public Transform dressingChair;
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

        void Start()
        {
            EnsureChair();
        }

        public void EnsureChair()
        {
            if (dressingChair != null) return;

            Transform table = transform;
            if (!table.name.Contains("Bridal dressing table"))
            {
                var found = GameObject.Find("Bridal dressing table mirror and jewelry casket");
                if (found != null) table = found.transform;
            }

            Transform existing = table.Find("Bridal Dressing Chair");
            if (existing != null)
            {
                dressingChair = existing;
                return;
            }

            // Create traditional Cambodian carved wooden chair in front of the mirror table
            GameObject chairGo = new GameObject("Bridal Dressing Chair");
            chairGo.transform.SetParent(table, false);

            // Positioned in front of the dressing table facing the mirror
            chairGo.transform.localPosition = new Vector3(0f, 0f, -0.62f);
            chairGo.transform.localRotation = Quaternion.identity;

            Material woodMat = null;
            var mr = table.GetComponent<MeshRenderer>();
            if (mr != null && mr.sharedMaterials != null && mr.sharedMaterials.Length > 0)
            {
                woodMat = mr.sharedMaterials[0];
            }
            if (woodMat == null)
            {
                var shader = Shader.Find("Standard");
                woodMat = new Material(shader);
                woodMat.color = new Color(0.28f, 0.17f, 0.10f); // Dark Cambodian teak wood
            }

            // 1. Seat Cushion / Board
            var seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "Seat";
            seat.transform.SetParent(chairGo.transform, false);
            seat.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            seat.transform.localScale = new Vector3(0.48f, 0.045f, 0.46f);
            if (seat.GetComponent<Renderer>()) seat.GetComponent<Renderer>().sharedMaterial = woodMat;

            // 2. Four Turned Wooden Legs
            Vector3[] legOffsets = new Vector3[]
            {
                new Vector3(-0.20f, 0.225f, -0.19f),
                new Vector3(0.20f, 0.225f, -0.19f),
                new Vector3(-0.20f, 0.225f, 0.19f),
                new Vector3(0.20f, 0.225f, 0.19f)
            };

            for (int i = 0; i < legOffsets.Length; i++)
            {
                var leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                leg.name = "Leg_" + i;
                leg.transform.SetParent(chairGo.transform, false);
                leg.transform.localPosition = legOffsets[i];
                leg.transform.localScale = new Vector3(0.045f, 0.225f, 0.045f);
                if (leg.GetComponent<Renderer>()) leg.GetComponent<Renderer>().sharedMaterial = woodMat;
                var col = leg.GetComponent<Collider>();
                if (col) Destroy(col);
            }

            // 3. Wooden Backrest Posts & Curved Crest Rail
            var postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postL.name = "BackPost_L";
            postL.transform.SetParent(chairGo.transform, false);
            postL.transform.localPosition = new Vector3(-0.20f, 0.70f, -0.20f);
            postL.transform.localScale = new Vector3(0.038f, 0.25f, 0.038f);
            if (postL.GetComponent<Renderer>()) postL.GetComponent<Renderer>().sharedMaterial = woodMat;
            var cL = postL.GetComponent<Collider>(); if (cL) Destroy(cL);

            var postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postR.name = "BackPost_R";
            postR.transform.SetParent(chairGo.transform, false);
            postR.transform.localPosition = new Vector3(0.20f, 0.70f, -0.20f);
            postR.transform.localScale = new Vector3(0.038f, 0.25f, 0.038f);
            if (postR.GetComponent<Renderer>()) postR.GetComponent<Renderer>().sharedMaterial = woodMat;
            var cR = postR.GetComponent<Collider>(); if (cR) Destroy(cR);

            // Crest Rail (top of backrest)
            var crest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crest.name = "CrestRail";
            crest.transform.SetParent(chairGo.transform, false);
            crest.transform.localPosition = new Vector3(0f, 0.94f, -0.20f);
            crest.transform.localScale = new Vector3(0.48f, 0.06f, 0.035f);
            if (crest.GetComponent<Renderer>()) crest.GetComponent<Renderer>().sharedMaterial = woodMat;
            var colCrest = crest.GetComponent<Collider>(); if (colCrest) Destroy(colCrest);

            // Backrest Center Slats
            for (int s = -2; s <= 2; s++)
            {
                var slat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slat.name = "Slat_" + s;
                slat.transform.SetParent(chairGo.transform, false);
                slat.transform.localPosition = new Vector3(s * 0.068f, 0.70f, -0.20f);
                slat.transform.localScale = new Vector3(0.024f, 0.42f, 0.015f);
                if (slat.GetComponent<Renderer>()) slat.GetComponent<Renderer>().sharedMaterial = woodMat;
                var colS = slat.GetComponent<Collider>(); if (colS) Destroy(colS);
            }

            // Lower Rungs / Stretchers
            var rungF = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rungF.name = "Rung_F";
            rungF.transform.SetParent(chairGo.transform, false);
            rungF.transform.localPosition = new Vector3(0f, 0.15f, 0.19f);
            rungF.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            rungF.transform.localScale = new Vector3(0.025f, 0.20f, 0.025f);
            if (rungF.GetComponent<Renderer>()) rungF.GetComponent<Renderer>().sharedMaterial = woodMat;
            var cF = rungF.GetComponent<Collider>(); if (cF) Destroy(cF);

            dressingChair = chairGo.transform;
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
