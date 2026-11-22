// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// GroomNPC.cs — The Groom (Bride's Husband)
//
// Head-tracking NPC. Moves between ceremony, reception, and bedroom.
// ============================================================================

using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum GroomPosition
    {
        Ceremony,    // Scene 06: at the wedding ceremony
        Reception,   // Scene 07: at the evening reception
        Bedroom,     // Scene 08: retires with bride
    }

    [ExecuteAlways]
    public sealed class GroomNPC : WorldInteractable
    {
        [Header("Character")]
        public Transform head;
        public GroomPosition currentPosition = GroomPosition.Ceremony;

        Transform playerCam;
        readonly NpcLookPose lookPose = new NpcLookPose();

        public override float Reach => 2.4f;
        public override string Prompt => "Talk to Groom";

        [Header("Bones & Posing")]
        public Transform lUpperArm;
        public Transform rUpperArm;
        public Transform lForearm;
        public Transform rForearm;

        void Start()
        {
            var cam = Camera.main;
            if (cam) playerCam = cam.transform;

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

            // Natural arm resting pose down beside hips
            float breath = Application.isPlaying ? Mathf.Sin(Time.time * 1.6f + 1f) * 1.1f : 0f;
            if (lUpperArm) lUpperArm.localEulerAngles = new Vector3(5.35f + breath, 277.5f, 354.74f);
            if (rUpperArm) rUpperArm.localEulerAngles = new Vector3(354.65f - breath, 82.5f, 354.74f);
            if (lForearm) lForearm.localEulerAngles = new Vector3(1.08f, 352.0f, 2.98f);
            if (rForearm) rForearm.localEulerAngles = new Vector3(358.92f, 8.0f, 2.98f);

            if (!Application.isPlaying) return;
            if (!head) head = FindBone(transform, "Bip01 Head");
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
                case GameScene.Scene06_Ceremony:
                    director.OnCeremonyInteraction();
                    break;
                case GameScene.Scene07_Reception:
                    director.OnReceptionInteraction();
                    break;
            }
        }

        public void MoveToPosition(GroomPosition pos)
        {
            currentPosition = pos;
            switch (pos)
            {
                case GroomPosition.Ceremony:
                    SetWorldPosition(new Vector3(0.5f, SiteLayout.HillElevation + 0.1f, -1.5f),
                                     Quaternion.Euler(0, 180, 0));
                    break;
                case GroomPosition.Reception:
                    SetWorldPosition(new Vector3(2, SiteLayout.HillElevation + 0.1f, 1),
                                     Quaternion.Euler(0, 200, 0));
                    break;
                case GroomPosition.Bedroom:
                    Vector3 bedPos = SiteLayout.HousePoint(new Vector3(-2.5f, 2.5f, 4.5f));
                    SetWorldPosition(bedPos, SiteLayout.HouseRotation * Quaternion.Euler(0, 160, 0));
                    break;
            }
        }

        void SetWorldPosition(Vector3 pos, Quaternion rot)
        {
            if (Physics.Raycast(pos + Vector3.up * 0.8f, Vector3.down, out var hit, 4f))
                pos = hit.point + Vector3.up * 0.02f;

            transform.SetPositionAndRotation(pos, rot);
        }
    }
}
