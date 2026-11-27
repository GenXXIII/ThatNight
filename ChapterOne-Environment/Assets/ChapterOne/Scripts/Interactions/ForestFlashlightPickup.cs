// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// ForestFlashlightPickup.cs — Ground Flashlight Pickup for Forest Nightmare
//
// Starts resting on the dark forest floor beside the waking Bride.
// When picked up with Left Click, smoothly equips into the Bride's right hand
// (attached to Main Camera with organic walking sway) and switches the beam ON.
// ============================================================================

using System;
using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed class ForestFlashlightPickup : WorldInteractable
    {
        public Light groundGlintLight;
        public Light beamLight;
        public AudioClip clickClip;

        public bool IsPickedUp { get; private set; }
        public bool BeamOn { get; private set; }

        Transform restingParent;
        Vector3 restingPos;
        Quaternion restingRot;
        Vector3 restingScale;
        Collider[] colliders;

        Transform equippedCam;
        float swayPhase;
        float flickerAmount;

        static readonly Vector3 HandLocalPos = new(0.21f, -0.17f, 0.36f);
        static readonly Vector3 HandLocalEuler = new(3f, -5f, 0f);

        public override float Reach => 2.6f;
        public override bool CanUse => !IsPickedUp && gameObject.activeInHierarchy;
        public override string Prompt => "[Left Click] Pick up Flashlight";

        void Awake()
        {
            displayName = "Flashlight";
            restingParent = transform.parent;
            restingPos = transform.position;
            restingRot = transform.rotation;
            restingScale = transform.localScale;
            colliders = GetComponentsInChildren<Collider>(true);
        }

        public void ResetToGround(Vector3 worldPos, Quaternion worldRot)
        {
            StopAllCoroutines();
            IsPickedUp = false;
            BeamOn = false;
            equippedCam = null;
            flickerAmount = 0f;

            transform.SetParent(restingParent, false);
            transform.SetPositionAndRotation(worldPos, worldRot);
            transform.localScale = restingScale;
            restingPos = worldPos;
            restingRot = worldRot;

            if (colliders == null || colliders.Length == 0)
                colliders = GetComponentsInChildren<Collider>(true);
            foreach (var c in colliders) if (c) c.enabled = true;

            if (groundGlintLight) groundGlintLight.enabled = true;
            if (beamLight) beamLight.enabled = false;
            gameObject.SetActive(true);
        }

        public override void Use(PlayerInteraction player)
        {
            if (IsPickedUp) return;
            IsPickedUp = true;

            if (colliders == null || colliders.Length == 0)
                colliders = GetComponentsInChildren<Collider>(true);
            foreach (var c in colliders) if (c) c.enabled = false;

            if (groundGlintLight) groundGlintLight.enabled = false;

            var cam = Camera.main;
            if (!cam && player) cam = player.GetComponent<Camera>();
            equippedCam = cam ? cam.transform : null;

            StartCoroutine(EquipToHandRoutine(player));
        }

        IEnumerator EquipToHandRoutine(PlayerInteraction player)
        {
            if (equippedCam != null)
            {
                Vector3 startWorldPos = transform.position;
                Quaternion startWorldRot = transform.rotation;
                transform.SetParent(equippedCam, true);

                Vector3 startLocalPos = equippedCam.InverseTransformPoint(startWorldPos);
                Quaternion startLocalRot = Quaternion.Inverse(equippedCam.rotation) * startWorldRot;

                const float dur = 0.42f;
                for (float t = 0f; t < 1f; t += Time.deltaTime / dur)
                {
                    float s = Mathf.SmoothStep(0f, 1f, t);
                    transform.localPosition = Vector3.Lerp(startLocalPos, HandLocalPos, s);
                    transform.localRotation = Quaternion.Slerp(startLocalRot, Quaternion.Euler(HandLocalEuler), s);
                    yield return null;
                }
                transform.localPosition = HandLocalPos;
                transform.localRotation = Quaternion.Euler(HandLocalEuler);
            }

            SetBeamActive(true, player ? player.explorer : FindFirstObjectByType<ForestExploration>());
            ChapterOneDirector.Instance?.OnForestFlashlightPickedUp();
        }

        public void SetBeamActive(bool on, ForestExploration explorer = null)
        {
            BeamOn = on;
            // Use ONE single SpotLight so two flashlight beams never overlap or desync!
            if (beamLight != null && explorer != null && explorer.flashlight != null && explorer.flashlight != beamLight)
            {
                explorer.flashlight.enabled = false;
            }
            var lightToUse = beamLight ? beamLight : (explorer ? explorer.flashlight : null);
            if (lightToUse != null)
            {
                lightToUse.enabled = on;
                if (on)
                {
                    lightToUse.type = LightType.Spot;
                    lightToUse.color = new Color(1.0f, 0.92f, 0.80f);
                    lightToUse.intensity = 4.2f;
                    lightToUse.range = 32f;
                    lightToUse.spotAngle = 54f;
                    lightToUse.innerSpotAngle = 24f;
                    lightToUse.shadows = LightShadows.Soft;
                }
            }
        }

        public void SetHorrorFlicker(float intensity01)
        {
            flickerAmount = Mathf.Clamp01(intensity01);
        }

        public void HideAfterNightmare(ForestExploration explorer = null)
        {
            StopAllCoroutines();
            SetBeamActive(false, explorer);
            flickerAmount = 0f;
            if (groundGlintLight) groundGlintLight.enabled = false;
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (!IsPickedUp || equippedCam == null) return;

            var explorer = equippedCam.GetComponent<ForestExploration>();
            if (explorer && !explorer.ControlsLocked && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.L)))
            {
                SetBeamActive(!BeamOn, explorer);
            }

            // Gentle hand sway synced to movement / breathing
            float moveInput = 0f;
            if (explorer && !explorer.MovementLocked && !explorer.ControlsLocked)
            {
                moveInput = Mathf.Clamp01(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude);
            }
            swayPhase += Time.deltaTime * (moveInput > 0.1f ? 7.5f : 1.8f);
            float amp = moveInput > 0.1f ? 0.014f : 0.004f;
            Vector3 sway = new(Mathf.Cos(swayPhase * 0.5f) * amp, Mathf.Sin(swayPhase) * amp, 0f);

            // Subtle fear tremor during ghost climax
            if (flickerAmount > 0.01f)
            {
                sway += new Vector3(
                    (Mathf.PerlinNoise(Time.time * 24f, 1.3f) - 0.5f) * 0.018f * flickerAmount,
                    (Mathf.PerlinNoise(3.7f, Time.time * 24f) - 0.5f) * 0.018f * flickerAmount,
                    0f
                );

                var lightToUse = beamLight ? beamLight : (explorer ? explorer.flashlight : null);
                if (lightToUse && BeamOn)
                {
                    float n = Mathf.PerlinNoise(Time.time * 18f, 0.5f);
                    bool dip = n < 0.25f * flickerAmount;
                    lightToUse.intensity = dip ? 0.7f : Mathf.Lerp(4.2f, 2.6f, n * flickerAmount);
                }
            }

            transform.localPosition = HandLocalPos + sway;
        }
    }
}
