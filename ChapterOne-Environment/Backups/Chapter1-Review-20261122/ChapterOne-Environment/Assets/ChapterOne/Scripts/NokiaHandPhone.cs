// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// NokiaHandPhone.cs — 2006 Classic Nokia Candybar Phone
//
// Interactive Nokia phone on the tea table. Rings with spatial audio,
// picked up to first-person view with green LCD display, plays
// the emotional phone conversation with Cousin Opharn.
// ============================================================================

using System;
using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum PhoneState { Idle, Ringing, InCall, Ended }

    public sealed class NokiaHandPhone : WorldInteractable
    {
        [Header("Audio")]
        public AudioSource ringSource;
        public AudioClip ringtoneClip;
        public AudioClip vibrationClip;
        public AudioClip callEndedClip;

        [Header("Visual")]
        public Light keypadBacklight;

        [Header("State")]
        public PhoneState currentState = PhoneState.Idle;
        public string callerName = "Brother";

        float callTimer;
        bool isHeld;
        Transform originalParent;
        Vector3 originalPosition;
        Quaternion originalRotation;
        Coroutine ringLoop;

        public override float Reach => 3.2f;
        public override string Prompt => currentState switch
        {
            PhoneState.Ringing => "Answer Phone",
            PhoneState.InCall  => "Listening...",
            _                  => "Inspect Phone"
        };

        void Start()
        {
            // If parented to the dressing table, ensure it rests cleanly on the tabletop
            if (transform.parent != null && transform.parent.name.Contains("dressing table"))
            {
                transform.localPosition = new Vector3(-0.06f, 0.725f, -0.06f);
                transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
            }

            originalParent = transform.parent;
            originalPosition = transform.position;
            originalRotation = transform.rotation;

            if (!ringSource)
            {
                ringSource = gameObject.AddComponent<AudioSource>();
                ringSource.spatialBlend = 1.0f;
                ringSource.minDistance = 0.5f;
                ringSource.maxDistance = 15f;
                ringSource.rolloffMode = AudioRolloffMode.Linear;
            }

            // Try loading audio from Resources
            if (!ringtoneClip) ringtoneClip = Resources.Load<AudioClip>("NokiaRingtone2006");
            if (!vibrationClip) vibrationClip = Resources.Load<AudioClip>("NokiaVibration");
            if (!callEndedClip) callEndedClip = Resources.Load<AudioClip>("NokiaCallEnded");
        }

        void Update()
        {
            if (currentState == PhoneState.InCall) callTimer += Time.deltaTime;
        }

        // ── Trigger incoming call ──────────────────────────────────────────
        public void TriggerIncomingCall()
        {
            if (currentState == PhoneState.InCall) return;
            currentState = PhoneState.Ringing;
            if (ringLoop != null) StopCoroutine(ringLoop);
            ringLoop = StartCoroutine(RingLoop());
        }

        IEnumerator RingLoop()
        {
            while (currentState == PhoneState.Ringing)
            {
                if (ringtoneClip && ringSource)
                {
                    ringSource.clip = ringtoneClip;
                    ringSource.loop = true;
                    ringSource.volume = 0.85f;
                    ringSource.Play();
                }

                float elapsed = 0;
                while (elapsed < 3.5f && currentState == PhoneState.Ringing)
                {
                    if (keypadBacklight)
                    {
                        keypadBacklight.enabled = true;
                        // Soft, atmospheric pulse for retro green LCD (not blinding)
                        keypadBacklight.intensity = 0.12f + Mathf.PingPong(elapsed * 2.5f, 0.18f);
                    }
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                yield return null;
            }
        }

        // ── Interaction ────────────────────────────────────────────────────
        public override void Use(PlayerInteraction player)
        {
            if (currentState == PhoneState.Ringing || currentState == PhoneState.Idle)
                player.ReachAndAct(transform.position, () => AnswerPhone(player));
            else if (currentState == PhoneState.Ended && isHeld)
                player.ReachAndAct(transform.position, () => PutDownPhone());
        }

        void AnswerPhone(PlayerInteraction player)
        {
            currentState = PhoneState.InCall;
            callTimer = 0;
            if (ringLoop != null) StopCoroutine(ringLoop);
            if (ringSource && ringSource.isPlaying) ringSource.Stop();
            if (keypadBacklight) keypadBacklight.intensity = 0.20f;

            isHeld = true;

            // Lock walking movement but allow subtle mouse glance look around
            var explorer = Camera.main ? Camera.main.GetComponent<ForestExploration>() : null;
            if (explorer)
            {
                explorer.MovementLocked = true;
                explorer.LookClamped = true;
                explorer.ClampCenterYaw = explorer.transform.eulerAngles.y;
                explorer.ClampYawRange = 40f;
            }

            StartCoroutine(LiftPhoneToView(StartDialogue));
        }

        IEnumerator LiftPhoneToView(Action onLifted)
        {
            var cam = Camera.main;
            if (cam)
            {
                Vector3 startPos = transform.position;
                Quaternion startRot = transform.rotation;
                transform.SetParent(cam.transform, true);
                Vector3 targetLocalPos = new Vector3(0.24f, -0.22f, 0.45f);
                Quaternion targetLocalRot = Quaternion.Euler(70f, -25f, 15f);

                for (float t = 0; t < 1f; t += Time.deltaTime / 0.45f)
                {
                    float smooth = Mathf.SmoothStep(0f, 1f, t);
                    transform.localPosition = Vector3.Lerp(cam.transform.InverseTransformPoint(startPos), targetLocalPos, smooth);
                    transform.localRotation = Quaternion.Slerp(Quaternion.Inverse(cam.transform.rotation) * startRot, targetLocalRot, smooth);
                    yield return null;
                }
                transform.localPosition = targetLocalPos;
                transform.localRotation = targetLocalRot;
            }
            onLifted?.Invoke();
        }

        void StartDialogue()
        {
            var narrative = ChapterOneNarrative.Instance;
            if (narrative != null)
            {
                narrative.PlaySequence(ChapterOneNarrative.Scene04_PhoneCallLines(), OnCallFinished);
            }
            else
            {
                StartCoroutine(FallbackCall());
            }
        }

        IEnumerator FallbackCall()
        {
            yield return new WaitForSeconds(18f);
            OnCallFinished();
        }

        void OnCallFinished()
        {
            currentState = PhoneState.Ended;
            if (ringSource && callEndedClip)
                ringSource.PlayOneShot(callEndedClip, 0.7f);

            StartCoroutine(AutoPutDown());
        }

        IEnumerator AutoPutDown()
        {
            yield return new WaitForSeconds(2.0f);
            PutDownPhone();
        }

        void PutDownPhone()
        {
            if (!isHeld) return;
            isHeld = false;
            if (keypadBacklight) keypadBacklight.intensity = 0;
            currentState = PhoneState.Idle;

            // Release movement lock
            var explorer = Camera.main ? Camera.main.GetComponent<ForestExploration>() : null;
            if (explorer)
            {
                explorer.MovementLocked = false;
                explorer.LookClamped = false;
            }

            StartCoroutine(LowerPhoneToTable());
        }

        IEnumerator LowerPhoneToTable()
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            transform.SetParent(originalParent, true);

            for (float t = 0; t < 1f; t += Time.deltaTime / 0.45f)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t);
                transform.position = Vector3.Lerp(startPos, originalPosition, smooth);
                transform.rotation = Quaternion.Slerp(startRot, originalRotation, smooth);
                yield return null;
            }
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            ChapterOneDirector.Instance?.OnPhoneCallComplete();
        }

        // ── Nokia LCD HUD ──────────────────────────────────────────────────
        void OnGUI()
        {
            if (!isHeld) return;

            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            var prev = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);

            float sw = Screen.width / scale, sh = Screen.height / scale;
            float w = 240, h = 130;
            float x = sw - w - 40, y = sh - h - 40;

            // Dark bezel
            GUI.color = new Color(0.12f, 0.14f, 0.15f, 0.95f);
            GUI.DrawTexture(new Rect(x - 8, y - 8, w + 16, h + 16), Texture2D.whiteTexture);

            // Green LCD
            GUI.color = new Color(0.38f, 0.58f, 0.32f, 0.98f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);

            var textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            textStyle.normal.textColor = new Color(0.12f, 0.22f, 0.10f);

            var statusStyle = new GUIStyle(textStyle)
            {
                fontSize = 11, alignment = TextAnchor.MiddleLeft
            };

            GUI.color = Color.white;

            // Status bar
            GUI.Label(new Rect(x + 10, y + 6, 80, 20), "▮▮▮▮  3G", statusStyle);
            statusStyle.alignment = TextAnchor.MiddleRight;
            GUI.Label(new Rect(x + w - 70, y + 6, 60, 20), "[||||]", statusStyle);

            // Content
            if (currentState == PhoneState.Ringing)
            {
                GUI.Label(new Rect(x, y + 35, w, 22), "INCOMING CALL", textStyle);
                textStyle.fontSize = 18;
                GUI.Label(new Rect(x, y + 60, w, 28), callerName, textStyle);
            }
            else if (currentState == PhoneState.InCall)
            {
                GUI.Label(new Rect(x, y + 32, w, 22), callerName, textStyle);
                textStyle.fontSize = 20;
                int min = (int)(callTimer / 60);
                int sec = (int)(callTimer % 60);
                GUI.Label(new Rect(x, y + 58, w, 28), $"{min:00}:{sec:00}", textStyle);

                var footerStyle = new GUIStyle(statusStyle)
                { alignment = TextAnchor.MiddleCenter, fontSize = 11 };
                GUI.Label(new Rect(x, y + 95, w, 20), "Options              End", footerStyle);
            }
            else if (currentState == PhoneState.Ended)
            {
                textStyle.fontSize = 17;
                GUI.Label(new Rect(x, y + 45, w, 28), "CALL ENDED", textStyle);
            }

            GUI.matrix = prev;
        }
    }
}
