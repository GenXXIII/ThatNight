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
        readonly NpcLookPose lookPose = new NpcLookPose();

        public override float Reach => 1.35f;
        public override string Prompt => "Talk to Mother";
        public override bool CanUse => ChapterOneDirector.Instance &&
            (ChapterOneDirector.Instance.currentScene == GameScene.Scene04_LivingRoom ||
             ChapterOneDirector.Instance.currentScene == GameScene.Scene06_Ceremony ||
             ChapterOneDirector.Instance.currentScene == GameScene.Scene07_Reception);

        [Header("Bones & Posing")]
        public Transform lUpperArm;
        public Transform rUpperArm;
        public Transform lForearm;
        public Transform rForearm;

        bool bonesSearched;

        void Start()
        {
            var cam = Camera.main;
            if (cam) playerCam = cam.transform;
            CacheBones();
        }

        void CacheBones()
        {
            if (bonesSearched) return;
            bonesSearched = true;
            if (!head) head = FindBone(transform, "Bip01 Head");
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
            if (!bonesSearched) CacheBones();

            // 1. Natural Human Arm Resting Posture & Gentle Idle Breathing
            float breath = Application.isPlaying ? Mathf.Sin(Time.time * 1.8f) * 1.2f : 0f;
            if (lUpperArm) lUpperArm.localEulerAngles = new Vector3(5.35f + breath, 277.5f, 354.74f);
            if (rUpperArm) rUpperArm.localEulerAngles = new Vector3(354.65f - breath, 82.5f, 354.74f);
            if (lForearm) lForearm.localEulerAngles = new Vector3(1.08f, 352.0f, 2.98f);
            if (rForearm) rForearm.localEulerAngles = new Vector3(358.92f, 8.0f, 2.98f);

            // 2. Natural head tracking — Mother looks at her daughter when nearby
            if (!Application.isPlaying) return;
            if (!playerCam) { var cam = Camera.main; if (cam) playerCam = cam.transform; }
            if (!head || !playerCam) return;

            lookPose.UpdatePose(transform, head, playerCam);
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
                    SetWorldPosition(new Vector3(-4.000f, 23.700f, 24.500f),
                                     Quaternion.Euler(0f, 217.2f, 0f));
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
