// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// ShadowGhostWoman.cs — Crying Shadow Apparition at the Sacred Tree
//
// Starts beneath the colossal Sacred Tree with ONLY her back visible to the
// Bride while weeping and reliving her tragic betrayal. When her sadness turns
// into fury, crying stops, she slowly turns around with an unnatural circling
// neck, and stalks toward the Bride in a terrifying shadow pose — her dark
// shadowed face clearly silhouetted — until she fills the entire screen.
// ============================================================================

using System;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum GhostAnimState
    {
        Hidden,
        Crying,
        Storytelling,
        AngerBuilding,
        AngryTurnAndNeckCircle,
        ScaryWalk,
        FullScreenLunge
    }

    public sealed class ShadowGhostWoman : MonoBehaviour
    {
        [Header("Audio")]
        public AudioSource crySource;
        public AudioSource sfxSource;
        public AudioClip cryingClip;
        public AudioClip neckCrackClip;
        public AudioClip screamClip;

        [Header("Visuals & Lighting")]
        public Transform modelRoot;
        public Light backRimLight;
        public Light faceSilhouetteLight;

        [Header("Skeleton Bones")]
        public Transform pelvis;
        public Transform spine;
        public Transform spine1;
        public Transform spine2;
        public Transform neck;
        public Transform head;
        public Transform lUpperArm;
        public Transform rUpperArm;
        public Transform lForearm;
        public Transform rForearm;
        public Transform lThigh;
        public Transform rThigh;
        public Transform lCalf;
        public Transform rCalf;

        public GhostAnimState State { get; private set; } = GhostAnimState.Hidden;

        Vector3 pelvisRestPos;
        Quaternion pelvisRestRot;
        Quaternion spineRest, spine1Rest, spine2Rest, neckRest, headRest;
        Quaternion lArmRest, rArmRest, lForeRest, rForeRest;
        Quaternion lThighRest, rThighRest, lCalfRest, rCalfRest;
        bool bonesCached;

        float stateTimer;
        Quaternion turnStartRot;
        Quaternion turnTargetRot;
        Vector3 stalkTarget;
        float walkSpeed;
        float lungeSpeed;
        Action onFullScreenImpact;
        bool impactFired;
        bool audioCryingEnabled;

        Transform cryBeaconTransform;

        void Awake()
        {
            CacheBones();
            ApplyShadowMaterials();
            SetupAudio();
        }

        public void CacheBones()
        {
            if (bonesCached) return;
            Transform searchRoot = modelRoot ? modelRoot : transform;

            if (!pelvis) pelvis = FindBone(searchRoot, "Bip01");
            if (!spine) spine = FindBone(searchRoot, "Bip01 Spine");
            if (!spine1) spine1 = FindBone(searchRoot, "Bip01 Spine1");
            if (!spine2) spine2 = FindBone(searchRoot, "Bip01 Spine2");
            if (!neck) neck = FindBone(searchRoot, "Bip01 Neck");
            if (!head) head = FindBone(searchRoot, "Bip01 Head");
            if (!lUpperArm) lUpperArm = FindBone(searchRoot, "Bip01 L UpperArm");
            if (!rUpperArm) rUpperArm = FindBone(searchRoot, "Bip01 R UpperArm");
            if (!lForearm) lForearm = FindBone(searchRoot, "Bip01 L Forearm");
            if (!rForearm) rForearm = FindBone(searchRoot, "Bip01 R Forearm");
            if (!lThigh) lThigh = FindBone(searchRoot, "Bip01 L Thigh");
            if (!rThigh) rThigh = FindBone(searchRoot, "Bip01 R Thigh");
            if (!lCalf) lCalf = FindBone(searchRoot, "Bip01 L Calf");
            if (!rCalf) rCalf = FindBone(searchRoot, "Bip01 R Calf");

            if (pelvis) { pelvisRestPos = pelvis.localPosition; pelvisRestRot = pelvis.localRotation; }
            if (spine) spineRest = spine.localRotation;
            if (spine1) spine1Rest = spine1.localRotation;
            if (spine2) spine2Rest = spine2.localRotation;
            if (neck) neckRest = neck.localRotation;
            if (head) headRest = head.localRotation;
            if (lUpperArm) lArmRest = lUpperArm.localRotation;
            if (rUpperArm) rArmRest = rUpperArm.localRotation;
            if (lForearm) lForeRest = lForearm.localRotation;
            if (rForearm) rForeRest = rForearm.localRotation;
            if (lThigh) lThighRest = lThigh.localRotation;
            if (rThigh) rThighRest = rThigh.localRotation;
            if (lCalf) lCalfRest = lCalf.localRotation;
            if (rCalf) rCalfRest = rCalf.localRotation;

            bonesCached = true;
        }

        public void ApplyShadowMaterials()
        {
            var unlitShader = Shader.Find("Unlit/Color");
            var standardShader = Shader.Find("Standard");
            if (!unlitShader && !standardShader) return;

            foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                var mats = smr.sharedMaterials;
                if (mats == null || mats.Length == 0) continue;

                // Pure unlit pitch-dark shadow silhouette: even under direct flashlight beam,
                // her face & body remain obscured as a living shadow ("show like shadow").
                var shadowMat = new Material(unlitShader ? unlitShader : standardShader)
                {
                    name = "ShadowGhostSilhouette_Runtime",
                    color = new Color(0.01f, 0.01f, 0.014f, 1f)
                };
                if (!unlitShader)
                {
                    shadowMat.SetFloat("_Glossiness", 0f);
                    shadowMat.SetFloat("_Metallic", 0f);
                    shadowMat.SetFloat("_SpecularHighlights", 0f);
                }

                var unlitCutout = Shader.Find("Unlit/Transparent Cutout");
                for (int i = 0; i < mats.Length; i++)
                {
                    if (i == 2 && mats[i] != null && mats[i].mainTexture != null)
                    {
                        var hairShadow = new Material(unlitCutout ? unlitCutout : mats[i].shader)
                        {
                            name = "ShadowGhostHair_Runtime",
                            mainTexture = mats[i].mainTexture,
                            color = new Color(0.01f, 0.01f, 0.014f, 1f)
                        };
                        if (hairShadow.HasProperty("_Cutoff")) hairShadow.SetFloat("_Cutoff", 0.45f);
                        if (hairShadow.HasProperty("_Glossiness")) hairShadow.SetFloat("_Glossiness", 0f);
                        if (hairShadow.HasProperty("_SpecularHighlights")) hairShadow.SetFloat("_SpecularHighlights", 0f);
                        mats[i] = hairShadow;
                    }
                    else
                    {
                        mats[i] = shadowMat;
                    }
                }
                smr.sharedMaterials = mats;
                smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                smr.receiveShadows = false;
            }
        }

        void SetupAudio()
        {
            if (!crySource)
            {
                var go = new GameObject("Ghost Cry Audio Beacon");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.up * 1.15f;
                crySource = go.AddComponent<AudioSource>();
            }
            cryBeaconTransform = crySource.transform;
            crySource.clip = cryingClip;
            crySource.loop = true;
            crySource.playOnAwake = false;
            crySource.spatialBlend = 1.0f;
            crySource.minDistance = 4f;
            crySource.maxDistance = 120f;
            crySource.rolloffMode = AudioRolloffMode.Custom;
            crySource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(
                new Keyframe(0f, 1.0f),
                new Keyframe(14f, 0.90f),
                new Keyframe(38f, 0.55f),
                new Keyframe(72f, 0.25f),
                new Keyframe(120f, 0.0f)
            ));

            if (!sfxSource)
            {
                var go2 = new GameObject("Ghost SFX Audio");
                go2.transform.SetParent(transform, false);
                go2.transform.localPosition = Vector3.up * 1.45f;
                sfxSource = go2.AddComponent<AudioSource>();
            }
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0.45f;
            sfxSource.minDistance = 2f;
            sfxSource.maxDistance = 40f;
        }

        /// <summary>
        /// Moves the 3D crying audio source to guide the player around each winding forest bend,
        /// then snaps it back onto the Ghost Woman at the Sacred Tree.
        /// </summary>
        public void SetCryBeaconWorldPosition(Vector3 worldPos, float volume = 0.92f)
        {
            if (!crySource) SetupAudio();
            cryBeaconTransform.position = worldPos;
            crySource.volume = volume;
        }

        public void LockCryBeaconToBody(float volume = 0.92f)
        {
            if (!crySource) SetupAudio();
            cryBeaconTransform.localPosition = Vector3.up * 1.15f;
            crySource.volume = volume;
        }

        public void SetCryingAudioActive(bool active, float volume = 0.92f)
        {
            audioCryingEnabled = active;
            if (!crySource) SetupAudio();
            if (active)
            {
                if (cryingClip && crySource.clip != cryingClip) crySource.clip = cryingClip;
                crySource.volume = volume;
                if (crySource.clip && !crySource.isPlaying) crySource.Play();
            }
            else
            {
                if (crySource.isPlaying) crySource.Stop();
            }
        }

        public void SetState(GhostAnimState next, Vector3 targetPos = default, Action impactCallback = null)
        {
            CacheBones();
            State = next;
            stateTimer = 0f;
            impactFired = false;

            if (next == GhostAnimState.Hidden)
            {
                SetCryingAudioActive(false);
                if (modelRoot) modelRoot.gameObject.SetActive(false);
                if (backRimLight) backRimLight.enabled = false;
                if (faceSilhouetteLight) faceSilhouetteLight.enabled = false;
                return;
            }

            if (modelRoot) modelRoot.gameObject.SetActive(true);
            if (backRimLight) backRimLight.enabled = true;

            switch (next)
            {
                case GhostAnimState.Crying:
                    if (faceSilhouetteLight) faceSilhouetteLight.enabled = false;
                    SetCryingAudioActive(audioCryingEnabled);
                    break;

                case GhostAnimState.Storytelling:
                    if (faceSilhouetteLight) faceSilhouetteLight.enabled = false;
                    if (crySource) crySource.volume = 0.28f;
                    break;

                case GhostAnimState.AngerBuilding:
                    // Crying stops completely as grief turns to cold hatred (still facing away)
                    SetCryingAudioActive(false);
                    if (faceSilhouetteLight) faceSilhouetteLight.enabled = false;
                    break;

                case GhostAnimState.AngryTurnAndNeckCircle:
                    SetCryingAudioActive(false);
                    turnStartRot = transform.rotation;
                    Vector3 toBride = targetPos - transform.position;
                    toBride.y = 0f;
                    turnTargetRot = toBride.sqrMagnitude > 0.01f
                        ? Quaternion.LookRotation(toBride.normalized)
                        : transform.rotation * Quaternion.Euler(0f, 180f, 0f);
                    if (faceSilhouetteLight) faceSilhouetteLight.enabled = true;
                    if (sfxSource && neckCrackClip) sfxSource.PlayOneShot(neckCrackClip, 1.0f);
                    break;

                case GhostAnimState.ScaryWalk:
                    SetCryingAudioActive(false);
                    stalkTarget = targetPos;
                    walkSpeed = 1.15f;
                    onFullScreenImpact = impactCallback;
                    if (faceSilhouetteLight) faceSilhouetteLight.enabled = true;
                    break;

                case GhostAnimState.FullScreenLunge:
                    SetCryingAudioActive(false);
                    stalkTarget = targetPos;
                    lungeSpeed = 6.2f;
                    onFullScreenImpact = impactCallback;
                    if (faceSilhouetteLight) faceSilhouetteLight.enabled = true;
                    if (sfxSource && screamClip) sfxSource.PlayOneShot(screamClip, 1.0f);
                    break;
            }
        }

        public Vector3 GetHeadWorldPosition()
        {
            if (head) return head.position;
            return transform.position + Vector3.up * 1.52f;
        }

        void Update()
        {
            if (State == GhostAnimState.Hidden) return;
            stateTimer += Time.deltaTime;

            if (State == GhostAnimState.AngryTurnAndNeckCircle)
            {
                // Slowly rise & turn around toward the Bride between t=0.55s and t=2.65s
                float turnU = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((stateTimer - 0.55f) / 2.1f));
                transform.rotation = Quaternion.Slerp(turnStartRot, turnTargetRot, turnU);
            }
            else if (State == GhostAnimState.ScaryWalk)
            {
                Vector3 toTarget = stalkTarget - transform.position;
                toTarget.y = 0f;
                float dist = toTarget.magnitude;
                if (dist > 0.05f)
                {
                    Quaternion look = Quaternion.LookRotation(toTarget.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 6f);
                    // Jerky, creeping horror walk that steadily builds speed as she gets close
                    float lurch = 0.60f + 0.72f * Mathf.Abs(Mathf.Sin(stateTimer * 4.8f));
                    float paceBoost = Mathf.Lerp(1.0f, 1.65f, Mathf.InverseLerp(6.0f, 1.4f, dist));
                    transform.position += toTarget.normalized * (walkSpeed * lurch * paceBoost * Time.deltaTime);
                }

                // When very close, transition seamlessly into the final lunge that fills the screen
                if (!impactFired && dist <= 1.35f)
                {
                    SetState(GhostAnimState.FullScreenLunge, stalkTarget, onFullScreenImpact);
                }
            }
            else if (State == GhostAnimState.FullScreenLunge)
            {
                Vector3 toTarget = stalkTarget - transform.position;
                toTarget.y = 0f;
                float dist = toTarget.magnitude;
                if (dist > 0.05f)
                {
                    transform.rotation = Quaternion.LookRotation(toTarget.normalized);
                    transform.position += toTarget.normalized * (lungeSpeed * Time.deltaTime);
                }
                // Stop right in front of the camera so her dark shadowed face fills the full screen!
                if (!impactFired && dist <= 0.42f)
                {
                    impactFired = true;
                    onFullScreenImpact?.Invoke();
                }
            }
        }

        void LateUpdate()
        {
            if (!bonesCached || State == GhostAnimState.Hidden) return;
            ApplyPose();
        }

        void ApplyPose()
        {
            float t = stateTimer;
            switch (State)
            {
                case GhostAnimState.Crying:
                {
                    // Kneeling low facing the giant tree, hands raised over bowed head, shoulders sobbing
                    float sob = Mathf.Sin(t * 7.2f) * 0.5f + Mathf.Sin(t * 3.1f) * 0.5f;
                    SetPelvisOffset(new Vector3(0f, -0.48f, 0f), 10f, 0f, 0f);
                    SetBone(spine, spineRest, 24f + sob * 2.5f, 0f, 0f);
                    SetBone(spine1, spine1Rest, 18f + sob * 2.0f, 0f, 0f);
                    SetBone(spine2, spine2Rest, 16f, 0f, 0f);
                    SetBone(neck, neckRest, 22f, 0f, 0f);
                    SetBone(head, headRest, 30f + sob * 4f, Mathf.Sin(t * 2.1f) * 4f, 0f);

                    SetArmPose(lUpperArm, lArmRest, -72f, 18f, -10f);
                    SetArmPose(rUpperArm, rArmRest, -72f, -18f, 10f);
                    SetBone(lForearm, lForeRest, -106f, 0f, 0f);
                    SetBone(rForearm, rForeRest, -106f, 0f, 0f);

                    SetBone(lThigh, lThighRest, -68f, -3f, 0f);
                    SetBone(rThigh, rThighRest, -68f, 3f, 0f);
                    SetBone(lCalf, lCalfRest, 154f, 0f, 0f);
                    SetBone(rCalf, rCalfRest, 154f, 0f, 0f);
                    break;
                }

                case GhostAnimState.Storytelling:
                {
                    // Still kneeling/bowed in shadow, gently swaying and trembling while speaking
                    float sway = Mathf.Sin(t * 2.0f);
                    float tremble = Mathf.Sin(t * 8.2f) * 1.4f;
                    SetPelvisOffset(new Vector3(0f, -0.44f, 0f), 8f, 0f, 0f);
                    SetBone(spine, spineRest, 18f + tremble * 0.6f, sway * 3.5f, 0f);
                    SetBone(spine1, spine1Rest, 14f, 0f, 0f);
                    SetBone(spine2, spine2Rest, 12f, sway * 2.5f, 0f);
                    SetBone(neck, neckRest, 16f, 0f, 0f);
                    SetBone(head, headRest, 20f + tremble, sway * 6f, sway * 4f);

                    SetArmPose(lUpperArm, lArmRest, -42f, 12f, -12f);
                    SetArmPose(rUpperArm, rArmRest, -42f, -12f, 12f);
                    SetBone(lForearm, lForeRest, -74f, 0f, 0f);
                    SetBone(rForearm, rForeRest, -74f, 0f, 0f);

                    SetBone(lThigh, lThighRest, -66f, -3f, 0f);
                    SetBone(rThigh, rThighRest, -66f, 3f, 0f);
                    SetBone(lCalf, lCalfRest, 150f, 0f, 0f);
                    SetBone(rCalf, rCalfRest, 150f, 0f, 0f);
                    break;
                }

                case GhostAnimState.AngerBuilding:
                {
                    // Crying stops — she slowly straightens up, trembling with suppressed rage, STILL back turned
                    float rise = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / 2.2f));
                    float shiver = Mathf.Sin(t * 26f) * (1.2f + rise * 2.5f);
                    SetPelvisOffset(new Vector3(0f, Mathf.Lerp(-0.26f, -0.02f, rise), 0f), Mathf.Lerp(6f, -2f, rise), 0f, 0f);
                    SetBone(spine, spineRest, Mathf.Lerp(16f, 2f, rise), 0f, shiver * 0.35f);
                    SetBone(spine1, spine1Rest, Mathf.Lerp(12f, 4f, rise), 0f, 0f);
                    SetBone(spine2, spine2Rest, Mathf.Lerp(10f, 4f, rise), 0f, -shiver * 0.35f);
                    SetBone(neck, neckRest, Mathf.Lerp(15f, -4f, rise), 0f, shiver * 0.8f);
                    SetBone(head, headRest, Mathf.Lerp(18f, -6f, rise), 0f, shiver);

                    SetArmPose(lUpperArm, lArmRest, Mathf.Lerp(-32f, 4f, rise), 0f, -8f);
                    SetArmPose(rUpperArm, rArmRest, Mathf.Lerp(-32f, 4f, rise), 0f, 8f);
                    SetBone(lForearm, lForeRest, Mathf.Lerp(-64f, -18f, rise), 0f, 0f);
                    SetBone(rForearm, rForeRest, Mathf.Lerp(-64f, -18f, rise), 0f, 0f);

                    SetBone(lThigh, lThighRest, Mathf.Lerp(-50f, 0f, rise), 0f, 0f);
                    SetBone(rThigh, rThighRest, Mathf.Lerp(-50f, 0f, rise), 0f, 0f);
                    SetBone(lCalf, lCalfRest, Mathf.Lerp(84f, 4f, rise), 0f, 0f);
                    SetBone(rCalf, rCalfRest, Mathf.Lerp(84f, 4f, rise), 0f, 0f);
                    break;
                }

                case GhostAnimState.AngryTurnAndNeckCircle:
                {
                    // Standing tall, slowly turning toward the Bride while her neck/head circles unnaturally 360+ deg!
                    float twitch = Mathf.Sin(t * 28f) * 3.5f;
                    SetPelvisOffset(new Vector3(0f, -0.02f, 0f), -4f, 0f, 0f);
                    SetBone(lThigh, lThighRest, 0f, 0f, 0f);
                    SetBone(rThigh, rThighRest, 0f, 0f, 0f);
                    SetBone(lCalf, lCalfRest, 4f, 0f, 0f);
                    SetBone(rCalf, rCalfRest, 4f, 0f, 0f);

                    SetBone(spine, spineRest, -4f, 0f, twitch * 0.4f);
                    SetBone(spine1, spine1Rest, 4f, 0f, 0f);
                    SetBone(spine2, spine2Rest, 6f, 0f, -twitch * 0.4f);

                    // Rigid eerie arms hanging slightly away from sides with crooked elbows
                    SetArmPose(lUpperArm, lArmRest, -10f, 6f, -14f);
                    SetArmPose(rUpperArm, rArmRest, -10f, -6f, 14f);
                    SetBone(lForearm, lForeRest, -32f, 0f, 0f);
                    SetBone(rForearm, rForeRest, -32f, 0f, 0f);

                    // Eerie circular neck rotation (continuous circling twist during the turn!)
                    float circleU = Mathf.Clamp01(t / 2.6f);
                    float angle = circleU * 450f + Mathf.Sin(t * 22f) * 6f;
                    float rad = angle * Mathf.Deg2Rad;
                    float neckTiltX = Mathf.Cos(rad) * 36f;
                    float neckRollZ = Mathf.Sin(rad) * 44f;
                    SetBone(neck, neckRest, neckTiltX * 0.65f, Mathf.Sin(rad) * 26f, neckRollZ * 0.65f);
                    SetBone(head, headRest, neckTiltX * 0.50f, Mathf.Sin(rad) * 20f, neckRollZ * 0.55f);
                    break;
                }

                case GhostAnimState.ScaryWalk:
                {
                    // Distorted stalking horror walk with continuous eerie circling/crooked neck & clawing arms
                    float cycle = t * 4.8f;
                    float stride = Mathf.Sin(cycle);
                    float bob = Mathf.Abs(Mathf.Cos(cycle)) * 0.06f;
                    float lurchZ = Mathf.Sin(cycle * 0.5f) * 14f;

                    SetPelvisOffset(new Vector3(0f, -0.06f + bob, 0f), 8f, stride * 6f, lurchZ * 0.45f);
                    SetBone(spine, spineRest, 16f, stride * 8f, lurchZ);
                    SetBone(spine1, spine1Rest, 10f, -stride * 6f, -lurchZ * 0.55f);
                    SetBone(spine2, spine2Rest, 8f, 0f, Mathf.Sin(t * 17f) * 4f);

                    // Slow, disturbing circular head/neck roll + crooked tilt while walking straight at Bride
                    float rollAngle = t * 185f * Mathf.Deg2Rad;
                    float nPitch = -10f + Mathf.Cos(rollAngle) * 22f;
                    float nRoll = 34f + Mathf.Sin(rollAngle) * 28f + Mathf.Sin(t * 21f) * 6f;
                    SetBone(neck, neckRest, nPitch * 0.6f, 8f, nRoll * 0.65f);
                    SetBone(head, headRest, 10f + nPitch * 0.4f, -8f, nRoll * 0.55f);

                    // Crooked horror pose arms reaching toward Bride
                    SetArmPose(lUpperArm, lArmRest, -48f + stride * 18f, 16f, -26f);
                    SetArmPose(rUpperArm, rArmRest, -54f - stride * 18f, -16f, 26f);
                    SetBone(lForearm, lForeRest, -54f + Mathf.Sin(t * 9f) * 8f, 0f, 0f);
                    SetBone(rForearm, rForeRest, -48f - Mathf.Cos(t * 9f) * 8f, 0f, 0f);

                    // Uneven dragging horror stride
                    SetBone(lThigh, lThighRest, -stride * 28f - 8f, 0f, 0f);
                    SetBone(rThigh, rThighRest, stride * 28f - 8f, 0f, 0f);
                    SetBone(lCalf, lCalfRest, Mathf.Max(4f, stride * 42f + 16f), 0f, 0f);
                    SetBone(rCalf, rCalfRest, Mathf.Max(4f, -stride * 42f + 16f), 0f, 0f);
                    break;
                }

                case GhostAnimState.FullScreenLunge:
                {
                    // Terrifying final surge right into the camera lens so her dark shadow face fills the screen
                    float cycle = t * 18f;
                    float stride = Mathf.Sin(cycle);
                    SetPelvisOffset(new Vector3(0f, -0.04f + Mathf.Abs(stride) * 0.05f, 0f), 10f, 0f, 0f);
                    SetBone(spine, spineRest, 16f, stride * 6f, 0f);
                    SetBone(spine1, spine1Rest, 8f, 0f, 0f);
                    SetBone(spine2, spine2Rest, 6f, 0f, 0f);
                    SetBone(neck, neckRest, -12f, 0f, Mathf.Sin(t * 32f) * 16f);
                    SetBone(head, headRest, -8f, 0f, Mathf.Cos(t * 28f) * 14f);

                    SetArmPose(lUpperArm, lArmRest, -86f, 12f, -10f);
                    SetArmPose(rUpperArm, rArmRest, -86f, -12f, 10f);
                    SetBone(lForearm, lForeRest, -18f, 0f, 0f);
                    SetBone(rForearm, rForeRest, -18f, 0f, 0f);

                    SetBone(lThigh, lThighRest, -stride * 44f, 0f, 0f);
                    SetBone(rThigh, rThighRest, stride * 44f, 0f, 0f);
                    SetBone(lCalf, lCalfRest, Mathf.Max(6f, stride * 56f + 18f), 0f, 0f);
                    SetBone(rCalf, rCalfRest, Mathf.Max(6f, -stride * 56f + 18f), 0f, 0f);
                    break;
                }
            }
        }

        void SetPelvisOffset(Vector3 rootSpaceOffset, float pitchDeg, float yawDeg, float rollDeg)
        {
            if (!pelvis || !pelvis.parent) return;
            pelvis.localPosition = pelvisRestPos + pelvis.parent.InverseTransformVector(transform.TransformVector(rootSpaceOffset));
            SetBone(pelvis, pelvisRestRot, pitchDeg, yawDeg, rollDeg);
        }

        void SetArmPose(Transform arm, Quaternion restLocal, float pitchDeg, float yawDeg, float rollDeg)
        {
            if (!arm || !arm.parent) return;
            Vector3 euler = arm == lUpperArm ? new Vector3(5.35f, 277.5f, 354.74f) : new Vector3(354.65f, 82.5f, 354.74f);
            Quaternion armsDown = Quaternion.Euler(euler);
            SetBone(arm, armsDown, pitchDeg, yawDeg, rollDeg);
        }

        void SetBone(Transform bone, Quaternion restLocal, float pitchRightDeg, float yawUpDeg, float rollFwdDeg)
        {
            if (!bone || !bone.parent) return;
            Quaternion worldDelta = Quaternion.AngleAxis(yawUpDeg, transform.up)
                                  * Quaternion.AngleAxis(pitchRightDeg, transform.right)
                                  * Quaternion.AngleAxis(rollFwdDeg, transform.forward);
            Quaternion localDelta = Quaternion.Inverse(bone.parent.rotation) * worldDelta * bone.parent.rotation;
            bone.localRotation = localDelta * restLocal;
        }

        static Transform FindBone(Transform root, string name)
        {
            if (!root) return null;
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                var found = FindBone(root.GetChild(i), name);
                if (found) return found;
            }
            return null;
        }
    }
}
