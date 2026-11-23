using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    [DefaultExecutionOrder(50)]
    public sealed class FirstPersonBody : MonoBehaviour
    {
        public Transform lowerBody;
        public ForestExploration explorer;
        public Transform flashlightModel, holdSocket;
        public float ReachBlend { get; set; }
        public Vector3 ReachTarget { get; set; }
        public bool Holding { get; set; }

        // Bone references from the character model
        Transform headBone;
        Transform neckBone;
        Transform spine2Bone;
        Transform leftLeg, rightLeg;
        Transform leftCalf, rightCalf;
        List<Transform> disabledUpperBones = new List<Transform>();
        List<GameObject> disabledObjects = new List<GameObject>();

        public Transform leftHand => transform;
        public Transform rightHand => transform;

        Quaternion leftLegBase = Quaternion.identity;
        Quaternion rightLegBase = Quaternion.identity;
        Quaternion leftCalfBase = Quaternion.identity;
        Quaternion rightCalfBase = Quaternion.identity;

        bool bonesInitialized = false;
        float stride, speed;
        Vector3 lastPosition;

        // Bed wakeup animation phase (-1 = inactive, 0 to 1 = active)
        public float bedWakeupPhase = -1f;

        void Awake()
        {
            if (lowerBody)
            {
                lowerBody.gameObject.SetActive(false);
                AutoFindBones();
            }
        }

        public void AutoFindBones()
        {
            if (!lowerBody) return;

            disabledUpperBones.Clear();
            disabledObjects.Clear();

            foreach (var t in lowerBody.GetComponentsInChildren<Transform>(true))
            {
                string n = t.name;
                string lower = n.ToLower();

                if (n.Contains("NeckPlug") || lower.Contains("bangle") || lower.Contains("bracelet"))
                {
                    t.gameObject.SetActive(false);
                    disabledObjects.Add(t.gameObject);
                }
                else if (n == "Bip01 Head") headBone = t;
                else if (n == "Bip01 Neck") neckBone = t;
                else if (n == "Bip01 Spine2") spine2Bone = t;
                else if (n == "Bip01 L Thigh") leftLeg = t;
                else if (n == "Bip01 R Thigh") rightLeg = t;
                else if (n == "Bip01 L Calf") leftCalf = t;
                else if (n == "Bip01 R Calf") rightCalf = t;

                if (lower.Contains("arm") || lower.Contains("forearm") || lower.Contains("hand") || 
                    lower.Contains("finger") || lower.Contains("clavicle") || lower.Contains("head") || 
                    lower.Contains("neck") || lower.Contains("jaw") || lower.Contains("lip") || 
                    lower.Contains("eye") || lower.Contains("cheek") || lower.Contains("nose") || 
                    lower.Contains("caninus") || lower.Contains("masseter") || lower.Contains("eyebrow") ||
                    lower.Contains("mouth") || lower.Contains("tongue") || lower.Contains("spine1") || lower.Contains("spine2"))
                {
                    disabledUpperBones.Add(t);
                    t.localScale = Vector3.zero;
                }
            }

            EnforceUpperBodyCollapse();

            // Make head, lips, and hair submeshes 100% invisible so no facial mesh ever hovers in view
            var smr = lowerBody ? lowerBody.GetComponentInChildren<SkinnedMeshRenderer>(true) : null;
            if (smr != null && smr.sharedMaterials != null && smr.sharedMaterials.Length >= 3)
            {
                var invisibleShader = Shader.Find("ThatNight/Invisible");
                if (invisibleShader != null)
                {
                    var mats = smr.sharedMaterials;
                    var invis = new Material(invisibleShader);
                    mats[1] = invis;
                    mats[2] = invis;
                    smr.sharedMaterials = mats;
                }
            }

            if (leftLeg) leftLegBase = leftLeg.localRotation;
            if (rightLeg) rightLegBase = rightLeg.localRotation;
            if (leftCalf) leftCalfBase = leftCalf.localRotation;
            if (rightCalf) rightCalfBase = rightCalf.localRotation;

            bonesInitialized = true;
        }

        void OnEnable()
        {
            lastPosition = transform.position;
            if (lowerBody) lowerBody.gameObject.SetActive(false);
            AutoFindBones();
        }

        public void EndBedWakeup()
        {
            bedWakeupPhase = -1f;
        }

        void EnforceUpperBodyCollapse()
        {
            for (int i = 0; i < disabledUpperBones.Count; i++)
            {
                var b = disabledUpperBones[i];
                if (b != null) b.localScale = Vector3.zero;
            }
            for (int i = 0; i < disabledObjects.Count; i++)
            {
                var go = disabledObjects[i];
                if (go != null && go.activeSelf) go.SetActive(false);
            }
        }

        public void UpdateBedWakeup(float phase, Vector3 camPos)
        {
            bedWakeupPhase = phase;
            if (!bonesInitialized) AutoFindBones();
            EnforceUpperBodyCollapse();

            if (lowerBody)
            {
                if (!lowerBody.gameObject.activeSelf) lowerBody.gameObject.SetActive(true);

                if (phase < 0.40f)
                {
                    float t = Mathf.SmoothStep(0f, 1f, phase / 0.40f);
                    lowerBody.position = new Vector3(camPos.x, camPos.y - 1.15f, camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, 105f, 0f);
                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(0f, 0f, 75f * t);
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(0f, 0f, -75f * t);
                    if (leftCalf) leftCalf.localRotation = leftCalfBase * Quaternion.Euler(0f, 0f, -15f * t);
                    if (rightCalf) rightCalf.localRotation = rightCalfBase * Quaternion.Euler(0f, 0f, 15f * t);
                }
                else if (phase < 0.78f)
                {
                    float swing = Mathf.SmoothStep(0f, 1f, (phase - 0.40f) / 0.38f);
                    lowerBody.position = new Vector3(camPos.x, camPos.y - 1.25f + swing * 0.10f, camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, Mathf.Lerp(105f, 82f, swing), 0f);

                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(0f, 0f, 75f);
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(0f, 0f, -75f);
                    if (leftCalf) leftCalf.localRotation = leftCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(-15f, -80f, swing));
                    if (rightCalf) rightCalf.localRotation = rightCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(15f, 80f, swing));
                }
                else
                {
                    float stand = Mathf.SmoothStep(0f, 1f, (phase - 0.78f) / 0.22f);
                    lowerBody.position = new Vector3(camPos.x, camPos.y - Mathf.Lerp(1.15f, 1.58f, stand), camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, 82f, 0f);

                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(75f, 0f, stand));
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(-75f, 0f, stand));
                    if (leftCalf) leftCalf.localRotation = leftCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(-80f, 0f, stand));
                    if (rightCalf) rightCalf.localRotation = rightCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(80f, 0f, stand));
                }
            }
        }

        void LateUpdate()
        {
            if (lowerBody && lowerBody.gameObject.activeSelf)
            {
                lowerBody.gameObject.SetActive(false);
            }
            return;
        }
    }
}
