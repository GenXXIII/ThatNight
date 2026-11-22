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
        Transform lClavicle, rClavicle;
        Transform leftLeg, rightLeg;
        Transform leftCalf, rightCalf;
        Transform rUpperArm, rForearm, rHandBone;
        Transform lUpperArm, lForearm, lHandBone;

        // Public properties for external test & review references
        public Transform leftHand => lHandBone != null ? lHandBone : transform;
        public Transform rightHand => rHandBone != null ? rHandBone : transform;

        List<Transform> rFingers = new List<Transform>();
        List<Quaternion> rFingerBases = new List<Quaternion>();

        Quaternion leftLegBase = Quaternion.identity;
        Quaternion rightLegBase = Quaternion.identity;
        Quaternion leftCalfBase = Quaternion.identity;
        Quaternion rightCalfBase = Quaternion.identity;
        Quaternion rUpperBase = Quaternion.identity;
        Quaternion rForeBase = Quaternion.identity;
        Quaternion rHandBase = Quaternion.identity;
        Quaternion lUpperBase = Quaternion.identity;
        Quaternion lForeBase = Quaternion.identity;
        Quaternion lHandBase = Quaternion.identity;

        bool bonesInitialized = false;
        float stride, speed;
        Vector3 lastPosition;

        // Interaction hand reach & grab animation
        public float interactProgress = 0f;
        Coroutine interactCoroutine;

        // Door grip & slow open hold [E]
        public bool isGrippingDoor = false;
        public Vector3 doorGripTarget;
        float doorGripBlend = 0f;

        // Phone hold pose
        public bool isHoldingPhone = false;
        float phoneHoldBlend = 0f;

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
                if (t.name.Contains("NeckPlug"))
                {
                    t.gameObject.SetActive(false);
                }
                else if (t.name == "Bip01 Head") headBone = t;
                else if (t.name == "Bip01 Neck") neckBone = t;
                else if (t.name == "Bip01 Spine2") spine2Bone = t;
                else if (t.name == "Bip01 L Clavicle") lClavicle = t;
                else if (t.name == "Bip01 R Clavicle") rClavicle = t;
                else if (t.name == "Bip01 L Thigh") leftLeg = t;
                else if (t.name == "Bip01 R Thigh") rightLeg = t;
                else if (t.name == "Bip01 L Calf") leftCalf = t;
                else if (t.name == "Bip01 R Calf") rightCalf = t;
                else if (t.name == "Bip01 R UpperArm") rUpperArm = t;
                else if (t.name == "Bip01 R Forearm") rForearm = t;
                else if (t.name == "Bip01 R Hand") rHandBone = t;
                else if (t.name == "Bip01 L UpperArm") lUpperArm = t;
                else if (t.name == "Bip01 L Forearm") lForearm = t;
                else if (t.name == "Bip01 L Hand") lHandBone = t;
                else if (t.name.StartsWith("Bip01 R Finger"))
                {
                    rFingers.Add(t);
                }
            }

            // Reparent arms to Spine2 (chest) so Neck can be scaled to 0 without affecting arms
            if (spine2Bone)
            {
                if (lClavicle && lClavicle.parent != spine2Bone) lClavicle.SetParent(spine2Bone, true);
                if (rClavicle && rClavicle.parent != spine2Bone) rClavicle.SetParent(spine2Bone, true);
            }

            // Permanently collapse neck and head so views down show only belly to legs
            if (neckBone) neckBone.localScale = Vector3.zero;
            if (headBone) headBone.localScale = Vector3.zero;

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

        // ── Door Continuous Grip Control ──────────────────────────────────
        public void StartDoorGrip(Vector3 handleWorld)
        {
            isGrippingDoor = true;
            doorGripTarget = handleWorld;
        }

        public void UpdateDoorGrip(Vector3 handleWorld)
        {
            doorGripTarget = handleWorld;
        }

        public void ReleaseDoorGrip()
        {
            isGrippingDoor = false;
        }

        // ── Phone Holding Posture ─────────────────────────────────────────
        public void StartHoldingPhone()
        {
            isHoldingPhone = true;
        }

        public void ReleaseHoldingPhone()
        {
            isHoldingPhone = false;
        }

        // ── Realistic Pickup & Interaction Hand ───────────────────────────
        public void PlayInteractHand(Action onPeak = null)
        {
            if (interactCoroutine != null) StopCoroutine(interactCoroutine);
            interactCoroutine = StartCoroutine(AnimateInteractHand(onPeak));
        }

        IEnumerator AnimateInteractHand(Action onPeak)
        {
            // 1. Hand reaches towards target, fingers open slightly (0.18s)
            float reachDuration = 0.18f;
            for (float t = 0; t < reachDuration; t += Time.deltaTime)
            {
                interactProgress = Mathf.SmoothStep(0f, 1f, t / reachDuration);
                yield return null;
            }
            interactProgress = 1f;

            // 2. Hand wraps around target / handle in realistic grip & triggers action
            onPeak?.Invoke();
            yield return new WaitForSeconds(0.10f);

            // 3. Smoothly retracts or lifts back (0.24s)
            float retractDuration = 0.24f;
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
                    // 1. Sitting up on mattress: Hips firmly on mattress, thighs bent forward at 75 deg resting on silk sheet
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
                    // 2. Realistic leg swing: Body swivels toward the left bed rail, thighs rotate over the edge, calves swing down
                    float swing = Mathf.SmoothStep(0f, 1f, (phase - 0.40f) / 0.38f);
                    lowerBody.position = new Vector3(camPos.x, camPos.y - 1.25f + swing * 0.10f, camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, Mathf.Lerp(105f, 82f, swing), 0f);

                    // Thighs remain bent at sitting angle, calves bend down 85 degrees over the wooden bed frame
                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(0f, 0f, 75f);
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(0f, 0f, -75f);
                    if (leftCalf) leftCalf.localRotation = leftCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(-15f, -80f, swing));
                    if (rightCalf) rightCalf.localRotation = rightCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(15f, 80f, swing));
                }
                else
                {
                    // 3. Standing up onto the floorboards: Feet plant on floorboards, thighs and calves unbend to 0
                    float stand = Mathf.SmoothStep(0f, 1f, (phase - 0.78f) / 0.22f);
                    lowerBody.position = new Vector3(camPos.x, camPos.y - Mathf.Lerp(1.15f, 1.58f, stand), camPos.z);
                    lowerBody.rotation = Quaternion.Euler(0f, 82f, 0f);

                    if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(75f, 0f, stand));
                    if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(-75f, 0f, stand));
                    if (leftCalf) leftCalf.localRotation = leftCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(-80f, 0f, stand));
                    if (rightCalf) rightCalf.localRotation = rightCalfBase * Quaternion.Euler(0f, 0f, Mathf.Lerp(80f, 0f, stand));
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

            // Do NOT render first-person body during title menu, opening flyby, or bed wakeup.
            var dir = ChapterOneDirector.Instance;
            bool hideBody = dir != null && (dir.showIntroMenu || dir.menuTransitioning || dir.currentScene == GameScene.Scene01_Opening || dir.currentScene == GameScene.None || (dir.currentScene == GameScene.Scene02_WakingUpAndPhone && !dir.isStandingUp));

            if (hideBody)
            {
                if (lowerBody && lowerBody.gameObject.activeSelf) lowerBody.gameObject.SetActive(false);
                return;
            }

            // Bride's 3D body is active during bedroom and gameplay so looking down shows real person
            if (lowerBody)
            {
                if (!lowerBody.gameObject.activeSelf) lowerBody.gameObject.SetActive(true);
                Vector3 forwardFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                Vector3 bodyPos = (explorer && explorer.Body && explorer.Walking)
                    ? (explorer.Body.transform.position - forwardFlat * 0.14f)
                    : (transform.position - forwardFlat * 0.14f - Vector3.up * 1.58f);
                lowerBody.position = bodyPos;
                lowerBody.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            }

            if (!bonesInitialized) AutoFindBones();

            // Permanently collapse neck and head so views down show only belly to legs
            if (neckBone) neckBone.localScale = Vector3.zero;
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

            // Door grip blend animation
            if (isGrippingDoor)
                doorGripBlend = Mathf.MoveTowards(doorGripBlend, 1f, Time.deltaTime * 6f);
            else
                doorGripBlend = Mathf.MoveTowards(doorGripBlend, 0f, Time.deltaTime * 5f);

            // Phone hold blend animation
            if (isHoldingPhone)
                phoneHoldBlend = Mathf.MoveTowards(phoneHoldBlend, 1f, Time.deltaTime * 6f);
            else
                phoneHoldBlend = Mathf.MoveTowards(phoneHoldBlend, 0f, Time.deltaTime * 5f);

            // Leg stride animation
            if (leftLeg) leftLeg.localRotation = leftLegBase * Quaternion.Euler(gait, 0, 0);
            if (rightLeg) rightLeg.localRotation = rightLegBase * Quaternion.Euler(-gait, 0, 0);

            // Left arm idle & walking swing
            if (lUpperArm)
            {
                float leftSwing = walking ? (gait * 0.65f) : (Mathf.Sin(Time.time * 1.6f) * 1.2f);
                lUpperArm.localRotation = lUpperBase * Quaternion.Euler(leftSwing, 0, 0);
            }

            // Right arm: blend between walking/resting, continuous door grip, and pickup reach
            if (rUpperArm)
            {
                float rightSwing = walking ? (-gait * 0.65f) : (Mathf.Sin(Time.time * 1.6f) * -1.2f);
                Quaternion idleUpperRot = rUpperBase * Quaternion.Euler(rightSwing, 0, 0);
                Quaternion idleForeRot = rForeBase;
                Quaternion idleHandRot = rHandBase;
                Quaternion idleWorldRot = rUpperArm.parent ? rUpperArm.parent.rotation * idleUpperRot : idleUpperRot;

                if (doorGripBlend > 0.001f)
                {
                    // Door grip pose: arm reaches up toward handle, hand clearly in view holding handle/ring
                    Vector3 gripPoint = transform.position + transform.forward * 0.50f + transform.right * 0.14f - transform.up * 0.10f;
                    if (doorGripTarget != Vector3.zero)
                    {
                        Vector3 dirToTarget = (doorGripTarget - transform.position).normalized;
                        gripPoint = transform.position + dirToTarget * 0.52f + transform.right * 0.08f - transform.up * 0.08f;
                    }
                    Vector3 dirToHandle = (gripPoint - rUpperArm.position).normalized;
                    Quaternion gripUpperRot = Quaternion.LookRotation(dirToHandle, transform.up) * Quaternion.Euler(10f, 82f, -10f);
                    Quaternion gripForeRot = Quaternion.Euler(0, 0, 46f);
                    Quaternion gripHandRot = idleHandRot * Quaternion.Euler(6f, -10f, 14f);

                    rUpperArm.rotation = Quaternion.Slerp(idleWorldRot, gripUpperRot, doorGripBlend);
                    if (rForearm) rForearm.localRotation = Quaternion.Slerp(idleForeRot, gripForeRot, doorGripBlend);
                    if (rHandBone) rHandBone.localRotation = Quaternion.Slerp(idleHandRot, gripHandRot, doorGripBlend);
                    ApplyFingerCurl(doorGripBlend, 14f, 18f, 8f, 10f);
                }
                else if (phoneHoldBlend > 0.001f)
                {
                    // Realistic phone holding pose in front of view: hand gently cradles phone
                    Vector3 phonePoint = transform.position + transform.forward * 0.42f + transform.right * 0.16f - transform.up * 0.16f;
                    Vector3 dirToPhone = (phonePoint - rUpperArm.position).normalized;
                    Quaternion phoneUpperRot = Quaternion.LookRotation(dirToPhone, transform.up) * Quaternion.Euler(15f, 78f, -12f);
                    Quaternion phoneForeRot = Quaternion.Euler(0, 0, 58f);
                    Quaternion phoneHandRot = idleHandRot * Quaternion.Euler(10f, -14f, 12f);

                    rUpperArm.rotation = Quaternion.Slerp(idleWorldRot, phoneUpperRot, phoneHoldBlend);
                    if (rForearm) rForearm.localRotation = Quaternion.Slerp(idleForeRot, phoneForeRot, phoneHoldBlend);
                    if (rHandBone) rHandBone.localRotation = Quaternion.Slerp(idleHandRot, phoneHandRot, phoneHoldBlend);
                    ApplyFingerCurl(phoneHoldBlend, 12f, 16f, 8f, 10f);
                }
                else if (interactProgress > 0.001f)
                {
                    // Target pickup reach pose: hand extends and fingers naturally grasp item (no claw)
                    Vector3 targetPoint = transform.position + transform.forward * 0.52f + transform.right * 0.14f - transform.up * 0.06f;
                    if (ReachTarget != Vector3.zero)
                    {
                        Vector3 dirToTarget = (ReachTarget - transform.position).normalized;
                        targetPoint = transform.position + dirToTarget * 0.52f + transform.right * 0.06f - transform.up * 0.06f;
                    }
                    Vector3 dirToReach = (targetPoint - rUpperArm.position).normalized;
                    Quaternion reachUpperRot = Quaternion.LookRotation(dirToReach, transform.up) * Quaternion.Euler(10f, 82f, -10f);
                    Quaternion reachForeRot = Quaternion.Euler(0, 0, 42f);
                    Quaternion reachHandRot = idleHandRot * Quaternion.Euler(6f, -10f, 12f);

                    rUpperArm.rotation = Quaternion.Slerp(idleWorldRot, reachUpperRot, interactProgress);
                    if (rForearm) rForearm.localRotation = Quaternion.Slerp(idleForeRot, reachForeRot, interactProgress);
                    if (rHandBone) rHandBone.localRotation = Quaternion.Slerp(idleHandRot, reachHandRot, interactProgress);
                    ApplyFingerCurl(interactProgress, 12f, 14f, 6f, 8f);
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

        void ApplyFingerCurl(float blend, float baseAngle, float midAngle, float tipAngle, float thumbSpread)
        {
            for (int i = 0; i < rFingers.Count; i++)
            {
                if (!rFingers[i] || i >= rFingerBases.Count) continue;
                string fName = rFingers[i].name;

                if (fName.Contains("Finger0")) // Thumb
                {
                    if (fName.EndsWith("2"))
                        rFingers[i].localRotation = rFingerBases[i] * Quaternion.Euler(0, 0, 6f * blend);
                    else if (fName.EndsWith("1"))
                        rFingers[i].localRotation = rFingerBases[i] * Quaternion.Euler(4f * blend, -thumbSpread * 0.5f * blend, 8f * blend);
                    else
                        rFingers[i].localRotation = rFingerBases[i] * Quaternion.Euler(6f * blend, -thumbSpread * blend, 10f * blend);
                }
                else // Fingers 1-4 (Index, Middle, Ring, Pinky)
                {
                    if (fName.EndsWith("2")) // Fingertip
                        rFingers[i].localRotation = rFingerBases[i] * Quaternion.Euler(0, 0, tipAngle * blend);
                    else if (fName.EndsWith("1")) // Middle knuckle
                        rFingers[i].localRotation = rFingerBases[i] * Quaternion.Euler(0, 0, midAngle * blend);
                    else // Base knuckle
                        rFingers[i].localRotation = rFingerBases[i] * Quaternion.Euler(0, 0, baseAngle * blend);
                }
            }
        }
    }
}
