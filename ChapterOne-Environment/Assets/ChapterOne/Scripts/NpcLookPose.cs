using UnityEngine;

namespace ThatNight.ChapterOne
{
    // Keeps the imported skeleton's natural head posture and smoothly rotates around the vertical axis
    // to face the player without breaking or snapping the neck.
    public sealed class NpcLookPose
    {
        Transform bone;
        Quaternion restLocal;

        public void UpdatePose(Transform root, Transform head, Transform target)
        {
            if (!head) return;
            if (bone != head)
            {
                bone = head;
                restLocal = head.localRotation;
            }

            Quaternion desired = restLocal;
            if (target)
            {
                Vector3 toTarget = target.position - head.position;
                toTarget.y = 0; // Lock to horizontal plane to avoid neck pitch bending/snapping
                if (toTarget.sqrMagnitude > 0.05f && toTarget.sqrMagnitude < 25f)
                {
                    // Calculate yaw relative to root's facing
                    Vector3 localDir = root.InverseTransformDirection(toTarget.normalized);
                    if (localDir.z > 0.1f) // Player is generally in front of NPC
                    {
                        float yaw = Mathf.Clamp(Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg, -40f, 40f);
                        // Rotate around bone's vertical up axis by yaw
                        desired = restLocal * Quaternion.Euler(0f, yaw, 0f);
                    }
                }
            }

            head.localRotation = Quaternion.Slerp(head.localRotation, desired, 1f - Mathf.Exp(-4f * Time.deltaTime));
        }
    }
}
