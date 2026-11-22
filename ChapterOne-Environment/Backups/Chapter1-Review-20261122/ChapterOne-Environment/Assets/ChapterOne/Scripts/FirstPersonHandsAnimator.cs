using System;
using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum HandPose
    {
        Idle,
        Walking,
        Reaching,
        HoldingPhone,
        WakingBed
    }

    /// <summary>
    /// AAA-style procedural First-Person Hands & Arms Animator.
    /// Drives realistic organic breathing sway, mouse-look drag/inertia,
    /// footstep cadence sway, door-push interaction, and phone hold animation.
    /// </summary>
    [DefaultExecutionOrder(60)]
    public sealed class FirstPersonHandsAnimator : MonoBehaviour
    {
        public static FirstPersonHandsAnimator Instance { get; private set; }

        [Header("Hierarchy Roots")]
        public Transform cameraTransform;
        public Transform handsRoot;
        public Transform leftArmRoot;
        public Transform rightArmRoot;

        [Header("Left Arm Joints")]
        public Transform leftShoulder;
        public Transform leftUpperArm;
        public Transform leftForearm;
        public Transform leftWrist;
        public Transform leftPalm;
        public Transform[] leftFingers;

        [Header("Right Arm Joints")]
        public Transform rightShoulder;
        public Transform rightUpperArm;
        public Transform rightForearm;
        public Transform rightWrist;
        public Transform rightPalm;
        public Transform[] rightFingers;
        public Transform phoneSocket;

        [Header("Materials / Accessories")]
        public Renderer leftGoldBangle;
        public Renderer rightGoldBangle;

        [Header("State")]
        public HandPose currentPose = HandPose.Idle;
        public bool isHoldingPhone = false;

        // Base resting local positions relative to camera - placed naturally in lower viewport
        // so player sees their hands and gold bridal bangles like a real person in first-person
        Vector3 leftRestLocal = new Vector3(-0.23f, -0.27f, 0.42f);
        Vector3 rightRestLocal = new Vector3(0.23f, -0.27f, 0.42f);
        Vector3 phoneHoldLocal = new Vector3(0.14f, -0.06f, 0.32f);

        // Procedural animation parameters
        Vector3 leftCurrentPos, rightCurrentPos;
        Quaternion leftCurrentRot, rightCurrentRot;
        Vector3 leftTargetPos, rightTargetPos;
        Quaternion leftTargetRot, rightTargetRot;

        // Inertia & drag
        Vector2 mouseDelta;
        Vector2 swayOffset;
        Vector2 swayVelocity;
        const float SwayAmount = 0.022f;
        const float SwaySmooth = 8.5f;

        // Walking / footstep bob
        float walkCycle;
        float walkSpeed;

        // Reach target
        bool isReaching;
        Vector3 reachWorldTarget;
        float reachProgress;
        bool reachWithRight;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);

            if (!cameraTransform) cameraTransform = transform;

            leftCurrentPos = leftRestLocal;
            rightCurrentPos = rightRestLocal;
            leftCurrentRot = Quaternion.Euler(14f, 6f, -4f);
            rightCurrentRot = Quaternion.Euler(14f, -6f, 4f);
        }

        void LateUpdate()
        {
            if (!cameraTransform || !handsRoot) return;

            // Only hide hands during title menu and opening flyby cinematic
            var dir = ChapterOneDirector.Instance;
            bool inMenuOrCinematic = dir != null && (dir.showIntroMenu || dir.menuTransitioning || dir.currentScene == GameScene.Scene01_Opening || dir.currentScene == GameScene.None);
            bool shouldShow = !inMenuOrCinematic;

            if (handsRoot.gameObject.activeSelf != shouldShow)
                handsRoot.gameObject.SetActive(shouldShow);

            if (!shouldShow) return;

            // 1. Calculate Mouse Look Sway / Inertia
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");
            mouseDelta = new Vector2(mouseX, mouseY);

            Vector2 targetSway = new Vector2(
                -Mathf.Clamp(mouseDelta.x * SwayAmount, -0.045f, 0.045f),
                -Mathf.Clamp(mouseDelta.y * SwayAmount * 0.7f, -0.035f, 0.035f)
            );
            swayOffset = Vector2.SmoothDamp(swayOffset, targetSway, ref swayVelocity, 1.0f / SwaySmooth);

            // 2. Calculate Walking Cadence & Stride Bob
            var explorer = cameraTransform.GetComponent<ForestExploration>();
            bool isWalking = explorer && explorer.Walking && !explorer.ControlsLocked;
            float moveInput = isWalking ? Mathf.Clamp01(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude) : 0f;
            walkSpeed = Mathf.Lerp(walkSpeed, moveInput, Time.deltaTime * 6f);

            walkCycle += walkSpeed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 7.5f : 4.8f);

            // Organic breathing wave (~12 breaths/minute)
            float breathe = Mathf.Sin(Time.time * 2.1f) * 0.0035f;
            float breatheSide = Mathf.Cos(Time.time * 1.05f) * 0.0018f;

            // Walking step bobbing and realistic arm swing (opposite arm forward-back motion)
            float walkBobY = Mathf.Sin(walkCycle * 2f) * 0.012f * walkSpeed;
            float walkBobX = Mathf.Cos(walkCycle) * 0.008f * walkSpeed;
            float armSwingLeftZ = Mathf.Sin(walkCycle) * 0.05f * walkSpeed;
            float armSwingRightZ = -Mathf.Sin(walkCycle) * 0.05f * walkSpeed;
            float armPitchLeft = Mathf.Sin(walkCycle) * 14f * walkSpeed;
            float armPitchRight = -Mathf.Sin(walkCycle) * 14f * walkSpeed;

            Vector3 proceduralOffset = new Vector3(
                swayOffset.x + breatheSide + walkBobX,
                swayOffset.y + breathe + walkBobY,
                0f
            );

            // 3. Pose Evaluation
            if (isHoldingPhone)
            {
                // Right hand holds phone close to eye/ear
                rightTargetPos = phoneHoldLocal + new Vector3(swayOffset.x * 0.3f, breathe * 0.8f, 0f);
                rightTargetRot = Quaternion.Euler(68f, -22f, 15f);

                // Left hand swings / rests naturally at side
                leftTargetPos = leftRestLocal + proceduralOffset + new Vector3(0f, 0f, armSwingLeftZ);
                leftTargetRot = Quaternion.Euler(14f + armPitchLeft, 6f, -4f);
            }
            else if (isReaching)
            {
                // Smooth IK reach toward world target (e.g. door handle or phone on table)
                Vector3 localReach = cameraTransform.InverseTransformPoint(reachWorldTarget);
                localReach = Vector3.ClampMagnitude(localReach, 0.85f);

                if (reachWithRight)
                {
                    rightTargetPos = Vector3.Lerp(rightRestLocal, localReach, Mathf.SmoothStep(0, 1, reachProgress));
                    rightTargetRot = Quaternion.Euler(20f, -8f, 0f);
                    leftTargetPos = leftRestLocal + proceduralOffset + new Vector3(0f, 0f, armSwingLeftZ);
                    leftTargetRot = Quaternion.Euler(14f + armPitchLeft, 6f, -4f);
                }
                else
                {
                    leftTargetPos = Vector3.Lerp(leftRestLocal, localReach, Mathf.SmoothStep(0, 1, reachProgress));
                    leftTargetRot = Quaternion.Euler(20f, 8f, 0f);
                    rightTargetPos = rightRestLocal + proceduralOffset + new Vector3(0f, 0f, armSwingRightZ);
                    rightTargetRot = Quaternion.Euler(14f + armPitchRight, -6f, 4f);
                }
            }
            else
            {
                // Natural idle / walking: both hands visible, swinging naturally with footsteps
                leftTargetPos = leftRestLocal + proceduralOffset + new Vector3(0f, 0f, armSwingLeftZ);
                rightTargetPos = rightRestLocal + proceduralOffset + new Vector3(0f, 0f, armSwingRightZ);

                leftTargetRot = Quaternion.Euler(14f + armPitchLeft, 6f, -4f);
                rightTargetRot = Quaternion.Euler(14f + armPitchRight, -6f, 4f);
            }

            // 4. Smooth Position and Rotation Damping
            leftCurrentPos = Vector3.Lerp(leftCurrentPos, leftTargetPos, Time.deltaTime * 12f);
            rightCurrentPos = Vector3.Lerp(rightCurrentPos, rightTargetPos, Time.deltaTime * 12f);
            leftCurrentRot = Quaternion.Slerp(leftCurrentRot, leftTargetRot, Time.deltaTime * 12f);
            rightCurrentRot = Quaternion.Slerp(rightCurrentRot, rightTargetRot, Time.deltaTime * 12f);

            // Apply transforms to arm roots
            if (leftArmRoot)
            {
                leftArmRoot.localPosition = leftCurrentPos;
                leftArmRoot.localRotation = leftCurrentRot;
            }
            if (rightArmRoot)
            {
                rightArmRoot.localPosition = rightCurrentPos;
                rightArmRoot.localRotation = rightCurrentRot;
            }

            // 5. Procedural Finger Curling
            ApplyFingerPoses();
        }

        void ApplyFingerPoses()
        {
            float curl = isHoldingPhone ? 65f : (isReaching ? 10f : 24f);

            if (rightFingers != null)
            {
                for (int i = 0; i < rightFingers.Length; i++)
                {
                    if (rightFingers[i] == null) continue;
                    float individualCurl = curl + (i * 3.5f);
                    if (isHoldingPhone) individualCurl += 15f; // Tighter grip on phone
                    rightFingers[i].localRotation = Quaternion.Euler(individualCurl, 0f, 0f);
                }
            }

            if (leftFingers != null)
            {
                for (int i = 0; i < leftFingers.Length; i++)
                {
                    if (leftFingers[i] == null) continue;
                    float individualCurl = (isReaching && !reachWithRight) ? 12f : 24f + (i * 3.5f);
                    leftFingers[i].localRotation = Quaternion.Euler(individualCurl, 0f, 0f);
                }
            }
        }

        /// <summary>
        /// Smoothly reaches hand out to touch or push a target (like a door handle),
        /// executes the action callback while touching, and returns to resting pose.
        /// </summary>
        public void ReachAndAct(Vector3 worldTarget, Action onReachAction, bool useRightHand = true)
        {
            StartCoroutine(DoReachSequence(worldTarget, onReachAction, useRightHand));
        }

        IEnumerator DoReachSequence(Vector3 worldTarget, Action onReachAction, bool useRightHand)
        {
            isReaching = true;
            reachWorldTarget = worldTarget;
            reachWithRight = useRightHand;
            reachProgress = 0f;

            // Reach forward
            while (reachProgress < 1f)
            {
                reachProgress += Time.deltaTime / 0.26f;
                yield return null;
            }
            reachProgress = 1f;

            // Execute action (e.g. door starts swinging inward)
            onReachAction?.Invoke();

            // Hand pushes forward slightly during swing
            yield return new WaitForSeconds(0.12f);

            // Retract hand back
            while (reachProgress > 0f)
            {
                reachProgress -= Time.deltaTime / 0.32f;
                yield return null;
            }
            reachProgress = 0f;
            isReaching = false;
        }

        /// <summary>
        /// Animates the right hand smoothly lifting the Nokia hand-phone up to ear/screen level.
        /// </summary>
        public void PickUpPhone(Transform phoneObject, Action onHeld)
        {
            StartCoroutine(DoPhonePickup(phoneObject, onHeld));
        }

        IEnumerator DoPhonePickup(Transform phoneObject, Action onHeld)
        {
            // Reach down to table
            isReaching = true;
            reachWithRight = true;
            reachWorldTarget = phoneObject.position;
            reachProgress = 0f;

            while (reachProgress < 1f)
            {
                reachProgress += Time.deltaTime / 0.35f;
                yield return null;
            }

            // Attach to phone socket
            if (phoneSocket != null)
            {
                phoneObject.SetParent(phoneSocket, true);
                phoneObject.localPosition = Vector3.zero;
                phoneObject.localRotation = Quaternion.identity;
            }

            isReaching = false;
            isHoldingPhone = true;
            onHeld?.Invoke();
        }

        /// <summary>
        /// Smoothly lowers the phone and places it back onto the table.
        /// </summary>
        public void PutDownPhone(Transform phoneObject, Vector3 tableWorldPos, Quaternion tableWorldRot, Action onPutDown)
        {
            StartCoroutine(DoPhonePutDown(phoneObject, tableWorldPos, tableWorldRot, onPutDown));
        }

        IEnumerator DoPhonePutDown(Transform phoneObject, Vector3 tableWorldPos, Quaternion tableWorldRot, Action onPutDown)
        {
            // Lower arm toward table
            isHoldingPhone = false;
            isReaching = true;
            reachWithRight = true;
            reachWorldTarget = tableWorldPos;
            reachProgress = 1f;

            while (reachProgress > 0f)
            {
                reachProgress -= Time.deltaTime / 0.4f;
                yield return null;
            }

            // Detach phone back to world
            phoneObject.SetParent(null, true);
            phoneObject.position = tableWorldPos;
            phoneObject.rotation = tableWorldRot;

            isReaching = false;
            onPutDown?.Invoke();
        }
    }
}
