// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// MotherNPC.cs — Mother of the Bride (Biological Mother)
//
// STRICT CANON: This is the bride's BIOLOGICAL MOTHER, NOT an aunt.
// Tracks the player with head rotation. Moves between scene positions.
// ============================================================================

using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum MotherPosition
    {
        LivingRoom,  // Scene 03: organizing wedding preparations
        Bedroom,     // Scene 05: helping bride dress
        Ceremony,    // Scene 06: beside the bride at ceremony
        Reception,   // Scene 07: at the reception celebration
    }

    [ExecuteAlways]
    public sealed class MotherNPC : WorldInteractable
    {
        [Header("Character")]
        public Transform head;
        public MotherPosition currentPosition = MotherPosition.LivingRoom;

        Transform playerCam;

        public override float Reach => 2.4f;
        public override string Prompt => "Talk to Mother";

        [Header("Bones & Posing")]
        public Transform lUpperArm;
        public Transform rUpperArm;
        public Transform lForearm;
        public Transform rForearm;

        void Start()
        {
            var cam = Camera.main;
            if (cam) playerCam = cam.transform;

            // Find upper arm bones for natural human resting posture
            if (!lUpperArm) lUpperArm = FindBone(transform, "Bip01 L UpperArm");
            if (!rUpperArm) rUpperArm = FindBone(transform, "Bip01 R UpperArm");
            if (!lForearm) lForearm = FindBone(transform, "Bip01 L Forearm");
            if (!rForearm) rForearm = FindBone(transform, "Bip01 R Forearm");
        }

        static Transform FindBone(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var found = FindBone(parent.GetChild(i), name);
                if (found) return found;
            }
            return null;
        }

        void LateUpdate()
        {
            if (!lUpperArm) lUpperArm = FindBone(transform, "Bip01 L UpperArm");
            if (!rUpperArm) rUpperArm = FindBone(transform, "Bip01 R UpperArm");
            if (!lForearm) lForearm = FindBone(transform, "Bip01 L Forearm");
            if (!rForearm) rForearm = FindBone(transform, "Bip01 R Forearm");

            // 1. Natural Human Arm Resting Posture & Gentle Idle Breathing
            float breath = Application.isPlaying ? Mathf.Sin(Time.time * 1.8f) * 1.2f : 0f;
            // 277.5f (-82.5 deg) on left and 82.5 deg on right rests arms naturally down beside hips
            if (lUpperArm) lUpperArm.localEulerAngles = new Vector3(5.35f + breath, 277.5f, 354.74f);
            if (rUpperArm) rUpperArm.localEulerAngles = new Vector3(354.65f - breath, 82.5f, 354.74f);
            if (lForearm) lForearm.localEulerAngles = new Vector3(1.08f, 352.0f, 2.98f);
            if (rForearm) rForearm.localEulerAngles = new Vector3(358.92f, 8.0f, 2.98f);

            // 2. Natural head tracking — Mother looks at her daughter when nearby
            if (!Application.isPlaying) return;
            if (!head) head = FindBone(transform, "Bip01 Head");
            if (!head || !playerCam) return;

            float dist = Vector3.Distance(transform.position, playerCam.position);
            if (dist < 5.0f)
            {
                Vector3 dir = (playerCam.position - head.position).normalized;
                Vector3 local = transform.InverseTransformDirection(dir);
                if (local.z > 0.15f)
                {
                    Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
                    head.rotation = Quaternion.Slerp(head.rotation, target, Time.deltaTime * 3.5f);
                }
            }
        }

        public override void Use(PlayerInteraction player)
        {
            var director = ChapterOneDirector.Instance;
            if (!director) return;

            switch (director.currentScene)
            {
                case GameScene.Scene04_LivingRoom:
                    director.OnMotherTalkedLivingRoom();
                    break;
                case GameScene.Scene05_Preparation:
                    director.OnPreparationStarted();
                    break;
                case GameScene.Scene06_Ceremony:
                    director.OnCeremonyInteraction();
                    break;
                case GameScene.Scene07_Reception:
                    director.OnReceptionInteraction();
                    break;
            }
        }

        public void MoveToPosition(MotherPosition pos)
        {
            currentPosition = pos;
            switch (pos)
            {
                case MotherPosition.LivingRoom:
                    SetWorldPosition(SiteLayout.HousePoint(new Vector3(-2.2f, 2.5f, -1.2f)),
                                     SiteLayout.HouseRotation * Quaternion.Euler(0, -42f, 0));
                    break;
                case MotherPosition.Bedroom:
                    SetWorldPosition(SiteLayout.HousePoint(new Vector3(-2.5f, 2.5f, 4.0f)),
                                     SiteLayout.HouseRotation * Quaternion.Euler(0, 120, 0));
                    break;
                case MotherPosition.Ceremony:
                    SetWorldPosition(new Vector3(-2, SiteLayout.HillElevation + 0.1f, -1),
                                     Quaternion.Euler(0, 0, 0));
                    break;
                case MotherPosition.Reception:
                    SetWorldPosition(new Vector3(-3, SiteLayout.HillElevation + 0.1f, 2),
                                     Quaternion.Euler(0, 160, 0));
                    break;
            }
        }

        void SetWorldPosition(Vector3 pos, Quaternion rot)
        {
            // Snap feet to ground
            if (Physics.Raycast(pos + Vector3.up * 0.8f, Vector3.down, out var hit, 4f))
                pos = hit.point + Vector3.up * 0.02f;

            transform.SetPositionAndRotation(pos, rot);
        }
    }
}
