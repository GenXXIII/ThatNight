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
        Transform leftLeg, rightLeg;
        Transform rUpperArm, rForearm, rHandBone;
        Transform lUpperArm, lForearm, lHandBone;

        // Public properties for external test & review references
        public Transform leftHand => lHandBone != null ? lHandBone : transform;
        public Transform rightHand => rHandBone != null ? rHandBone : transform;

        List<Transform> rFingers = new List<Transform>();
        List<Quaternion> rFingerBases = new List<Quaternion>();

        Quaternion leftLegBase = Quaternion.identity;
        Quaternion rightLegBase = Quaternion.identity;
        Quaternion rUpperBase = Quaternion.identity;
        Quaternion rForeBase = Quaternion.identity;
        Quaternion rHandBase = Quaternion.identity;
        Quaternion lUpperBase = Quaternion.identity;
        Quaternion lForeBase = Quaternion.identity;
        Quaternion lHandBase = Quaternion.identity;

        bool bonesInitialized = false;
        float stride, speed;
        Vector3 lastPosition;

        // Interaction hand reach animation
        public float interactProgress = 0f;
        Coroutine interactCoroutine;

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
            rFingers.Clear();
            rFingerBases.Clear();

            foreach (var t in lowerBody.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "Bip01 Head") headBone = t;
                else if (t.name == "Bip01 L Thigh") leftLeg = t;
                else if (t.name == "Bip01 R Thigh") rightLeg = t;
                else if (t.name == "Bip01 R UpperArm") rUpperArm = t;
                else if (t.name == "Bip01 R Forearm") rForearm = t;
                else if (t.name == "Bip01 R Hand") rHandBone = t;
                else if (t.name == "Bip01 L UpperArm") lUpperArm = t;
                else if (t.name == "Bip01 L Forearm") lForearm = t;
                else if (t.name == "Bip01 L Hand") lHandBone = t;
                else if (t.name.StartsWith("Bip01 R Finger") && t.name.Length <= 15)
                {
                    rFingers.Add(t);
                }
            }

            if (leftLeg) leftLegBase = leftLeg.localRotation;
            if (rightLeg) rightLegBase = rightLeg.localRotation;
            if (rUpperArm) rUpperBase = rUpperArm.localRotation;
            if (rForearm) rForeBase = rForearm.localRotation;
            if (rHandBone) rHandBase = rHandBone.localRotation;
            if (lUpperArm) lUpperBase = lUpperArm.localRotation;
            if (lForearm) lForeBase = lForearm.localRotation;
            if (lHandBone) lHandBase = lHandBone.localRotation;

            foreach (var f in rFingers)
            {
                rFingerBases.Add(f.localRotation);
            }

            bonesInitialized = true;
        }

        void OnEnable()
        {
            lastPosition = transform.position;
            if (lowerBody) lowerBody.gameObject.SetActive(false);
            AutoFindBones();
        }

        public void PlayInteractHand(Action onPeak = null)
        {
            if (interactCoroutine != null) StopCoroutine(interactCoroutine);
            interactCoroutine = StartCoroutine(AnimateInteractHand(onPeak));
        }

        IEnumerator AnimateInteractHand(Action onPeak)
        {
            // Reach forward towards interactable / center view over 0.20s
            float reachDuration = 0.20f;
            for (float t = 0; t < reachDuration; t += Time.deltaTime)
            {
                interactProgress = Mathf.SmoothStep(0f, 1f, t / reachDuration);
                yield return null;
            }
            interactProgress = 1f;

            // Trigger the interaction action at peak reach
            onPeak?.Invoke();

            // Hold briefly at peak (0.08s) so player sees hand touching handle / item
            yield return new WaitForSeconds(0.08f);

            // Retract back to side resting position over 0.22s
            float retractDuration = 0.22f;
            for (float t = 0; t < retractDuration; t += Time.deltaTime)
            {
                interactProgress = Mathf.SmoothStep(1f, 0f, t / retractDuration);
                yield return null;
            }
            interactProgress = 0f;
            interactCoroutine = null;
        }

        public void EndBedWakeup()
        {
            bedWakeupPhase = -1f;
        }

        public void UpdateBedWakeup(float phase, Vector3 camPos)
        {
            bedWakeupPhase = phase;
            if (!bonesInitialized) AutoFindBones();
            if (headBone) headBone.localScale = Vector3.zero;

            if (lowerBody)
            {
                if (!lowerBody.gameObject.activeSelf) lowerBody.gameObject.SetActive(true);

                if (phase < 0.40f)
                {
                    // 1. Sitting up on mattress, legs resting forward on silk bedsheets
                    lowerBody.position = new Vector3(camPos.x, camPos.y - 1.50f, camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, 105f, 0f);
                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(-12f, 0f, 0f);
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(-12f, 0f, 0f);
                }
                else if (phase < 0.78f)
                {
                    // 2. Swinging legs over the left side of the mattress down toward floorboards
                    float swing = Mathf.SmoothStep(0f, 1f, (phase - 0.40f) / 0.38f);
                    lowerBody.position = new Vector3(camPos.x, camPos.y - 1.50f, camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, Mathf.Lerp(105f, 82f, swing), 0f);
                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(Mathf.Lerp(-12f, 75f, swing), -12f * swing, 0f);
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(Mathf.Lerp(-12f, 70f, swing), 12f * swing, 0f);
                }
                else
                {
                    // 3. Standing up onto the floorboards
                    float stand = Mathf.SmoothStep(0f, 1f, (phase - 0.78f) / 0.22f);
                    lowerBody.position = new Vector3(camPos.x, camPos.y - 1.55f, camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, 82f, 0f);
                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(Mathf.Lerp(75f, 0f, stand), 0f, 0f);
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(Mathf.Lerp(70f, 0f, stand), 0f, 0f);
                }

                // Arms rest naturally beside the body on the mattress / side
                if (lUpperArm) lUpperArm.localRotation = lUpperBase;
                if (rUpperArm) rUpperArm.localRotation = rUpperBase;
            }
        }

        void LateUpdate()
        {
            // If bed wakeup sequence is active, it has full control over bone kinematics
            if (bedWakeupPhase >= 0f) return;

            // Do NOT render first-person body during title menu or opening flyby cinematic.
            var dir = ChapterOneDirector.Instance;
            bool hideBody = dir != null && (dir.showIntroMenu || dir.menuTransitioning || dir.currentScene == GameScene.Scene01_Opening || dir.currentScene == GameScene.None);

            if (hideBody)
            {
                if (lowerBody && lowerBody.gameObject.activeSelf) lowerBody.gameObject.SetActive(false);
                return;
            }

            // Bride's 3D body is active during bedroom and gameplay so looking down shows real person
            if (lowerBody)
            {
                if (!lowerBody.gameObject.activeSelf) lowerBody.gameObject.SetActive(true);
                Vector3 bodyPos = (explorer && explorer.Body && explorer.Walking) ? explorer.Body.transform.position : (transform.position - Vector3.up * 1.55f);
                lowerBody.position = bodyPos;
                lowerBody.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            }

            if (!bonesInitialized) AutoFindBones();

            // Permanently hide head/mouth/lips so they NEVER appear in front of the camera view
            if (headBone) headBone.localScale = Vector3.zero;

            bool walking = explorer && explorer.Walking && !explorer.ControlsLocked;
            float gait = 0f;

            if (walking)
            {
                Vector3 difference = transform.position - lastPosition;
                difference.y = 0;
                speed = Mathf.Lerp(speed, Mathf.Clamp(difference.magnitude / Mathf.Max(Time.deltaTime, .001f), 0, 4.5f), Time.deltaTime * 10);
                lastPosition = transform.position;
                stride += speed * Time.deltaTime * 3.5f;
                gait = Mathf.Sin(stride) * Mathf.Clamp01(speed / 2.2f) * 22f;
            }
            else
            {
                lastPosition = transform.position;
                speed = Mathf.Lerp(speed, 0f, Time.deltaTime * 10);
            }

            // Leg stride animation
            if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(gait, 0, 0);
            if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(-gait, 0, 0);

            // Left arm idle & walking swing
            if (lUpperArm)
            {
                float leftSwing = walking ? (gait * 0.65f) : (Mathf.Sin(Time.time * 1.6f) * 1.2f);
                lUpperArm.localRotation = lUpperBase * Quaternion.Euler(leftSwing, 0, 0);
            }

            // Right arm: blend between walking/resting and interaction reach
            if (rUpperArm)
            {
                float rightSwing = walking ? (-gait * 0.65f) : (Mathf.Sin(Time.time * 1.6f) * -1.2f);
                Quaternion idleUpperRot = rUpperBase * Quaternion.Euler(rightSwing, 0, 0);
                Quaternion idleForeRot = rForeBase;
                Quaternion idleHandRot = rHandBase;

                if (interactProgress > 0.001f)
                {
                    // Target reach pose in front of camera
                    Vector3 targetPoint = transform.position + transform.forward * 0.55f + transform.right * 0.15f - transform.up * 0.02f;
                    Vector3 dirToTarget = (targetPoint - rUpperArm.position).normalized;
                    Quaternion reachUpperRot = Quaternion.LookRotation(dirToTarget, transform.up) * Quaternion.Euler(0, 90, 0);
                    Quaternion reachForeRot = Quaternion.Euler(0, 0, 45);
                    Quaternion reachHandRot = Quaternion.Euler(15, -15, 0);

                    rUpperArm.rotation = Quaternion.Slerp(idleUpperRot, reachUpperRot, interactProgress);
                    if (rForearm) rForearm.localRotation = Quaternion.Slerp(idleForeRot, reachForeRot, interactProgress);
                    if (rHandBone) rHandBone.localRotation = Quaternion.Slerp(idleHandRot, reachHandRot, interactProgress);

                    // Finger grip animation on interact
                    for (int i = 0; i < rFingers.Count; i++)
                    {
                        if (rFingers[i] && i < rFingerBases.Count)
                        {
                            rFingers[i].localRotation = rFingerBases[i] * Quaternion.Euler(0, 0, 22f * interactProgress);
                        }
                    }
                }
                else
                {
                    rUpperArm.localRotation = idleUpperRot;
                    if (rForearm) rForearm.localRotation = idleForeRot;
                    if (rHandBone) rHandBone.localRotation = idleHandRot;

                    for (int i = 0; i < rFingers.Count; i++)
                    {
                        if (rFingers[i] && i < rFingerBases.Count)
                        {
                            rFingers[i].localRotation = rFingerBases[i];
                        }
                    }
                }
            }
        }
    }
}
