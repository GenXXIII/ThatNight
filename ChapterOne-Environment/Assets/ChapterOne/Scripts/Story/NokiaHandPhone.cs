// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// NokiaHandPhone.cs — 2006 Classic Nokia Candybar Phone
//
// Interactive Nokia phone firmly anchored to the bedroom mirror table.
// Step 1: Click [E] to pick up and inspect incoming call from Brother,
//         shows LCD caller and thought dialogue "It is really Brother...".
// Step 2: Ringtone continues playing.
//         - [Esc] puts phone back on table without hanging up (continues ringing).
//         - [E] answers the call and begins the conversation with Brother.
// ============================================================================

using System;
using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum PhoneState { Idle, Ringing, InspectingCall, InCall, Ended }

    public sealed class NokiaHandPhone : WorldInteractable
    {
        [Header("Audio")]
        public AudioSource ringSource;
        public AudioClip ringtoneClip;
        public AudioClip vibrationClip;
        public AudioClip callEndedClip;

        [Header("Visual")]
        public Light keypadBacklight;
        public Renderer screenRenderer;

        [Header("State")]
        public PhoneState currentState = PhoneState.Idle;
        public string callerName = "Brother";
        public string thoughtText = null;

        float callTimer;
        bool isHeld;
        Transform originalParent;
        Vector3 originalPosition;
        Quaternion originalRotation;
        Coroutine ringLoop;

        public override float Reach => 2.2f; // Comfortable reach from in front of mirror table
        public override bool CanUse => currentState == PhoneState.Ringing || (currentState == PhoneState.InCall && isHeld);
        public override string Prompt => currentState switch
        {
            PhoneState.Ringing        => "[Left Click] Pick up Phone",
            PhoneState.InspectingCall => "[Left Click] Answer Call  •  [Right Click] Put Back",
            PhoneState.InCall         => "In Call",
            PhoneState.Ended          => "[Right Click] Put back on table",
            _                         => "Inspect Phone"
        };

        [Header("3D LCD Screen")]
        GameObject lcdRoot;
        Material lcdMat;
        TextMesh lcdHeader;
        TextMesh lcdBody;

        void Awake()
        {
            EnsureTablePosition();
            EnsureLcdScreen();
        }

        void Start()
        {
            EnsureTablePosition();
            EnsureLcdScreen();

            var mc = GetComponent<MeshCollider>();
            if (mc) Destroy(mc);

            var box = GetComponent<BoxCollider>();
            if (!box) box = gameObject.AddComponent<BoxCollider>();
            box.enabled = true;
            box.size = new Vector3(0.14f, 0.08f, 0.22f);
            box.center = new Vector3(0f, 0.035f, 0f);

            #if UNITY_EDITOR
            if (!ringtoneClip) ringtoneClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NokiaRingtone2006.wav");
            if (!vibrationClip) vibrationClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NokiaVibration.wav");
            if (!callEndedClip) callEndedClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ChapterOne/Audio/NokiaCallEnded.wav");
            #endif

            if (!ringtoneClip) ringtoneClip = Resources.Load<AudioClip>("NokiaRingtone2006");
            if (!vibrationClip) vibrationClip = Resources.Load<AudioClip>("NokiaVibration");
            if (!callEndedClip) callEndedClip = Resources.Load<AudioClip>("NokiaCallEnded");

            if (!ringSource)
            {
                ringSource = GetComponent<AudioSource>();
                if (!ringSource) ringSource = gameObject.AddComponent<AudioSource>();
            }
            ringSource.spatialBlend = 0.4f;
            ringSource.minDistance = 2.0f;
            ringSource.maxDistance = 30f;
            ringSource.volume = 1.0f;
            ringSource.rolloffMode = AudioRolloffMode.Linear;
            ringSource.clip = ringtoneClip;

            if (!keypadBacklight) keypadBacklight = GetComponentInChildren<Light>(true);
            if (keypadBacklight)
            {
                keypadBacklight.transform.localPosition = new Vector3(0f, 0.04f, 0f);
                keypadBacklight.range = 0.85f;
                keypadBacklight.intensity = 0f;
                keypadBacklight.enabled = false;
                keypadBacklight.color = new Color(0.35f, 0.95f, 0.40f);
            }

            if (!screenRenderer)
            {
                foreach (var r in GetComponentsInChildren<Renderer>(true))
                {
                    if (r.gameObject.name.Contains("LCD") || r.gameObject.name.Contains("Screen") || r.gameObject.name.Contains("phone") || (r.sharedMaterial != null && r.sharedMaterial.name.Contains("LCD")))
                    {
                        screenRenderer = r;
                        break;
                    }
                }
            }
            SetScreenGlow(0f);
        }

        public void EnsureTablePosition()
        {
            if (transform.parent == null || !transform.parent.name.Contains("dressing table"))
            {
                var tables = FindObjectsByType<Transform>(FindObjectsSortMode.None);
                foreach (var t in tables)
                {
                    if (t.name.Contains("Bridal dressing table") && t.position.x < 0)
                    {
                        transform.SetParent(t, false);
                        transform.localPosition = new Vector3(-0.06f, 0.742f, -0.06f);
                        transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
                        break;
                    }
                }
            }
            originalParent = transform.parent;
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        public void EnsureLcdScreen()
        {
            if (lcdRoot == null)
            {
                var existing = transform.Find("Nokia 3D LCD Display");
                if (existing != null)
                {
                    lcdRoot = existing.gameObject;
                }
                else
                {
                    lcdRoot = new GameObject("Nokia 3D LCD Display");
                    lcdRoot.transform.SetParent(transform, false);
                    lcdRoot.transform.localPosition = new Vector3(0f, 0.0205f, 0.018f);
                    lcdRoot.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.name = "LCD Backlight Quad";
                    quad.transform.SetParent(lcdRoot.transform, false);
                    quad.transform.localPosition = Vector3.zero;
                    quad.transform.localRotation = Quaternion.identity;
                    quad.transform.localScale = new Vector3(0.040f, 0.030f, 1f);
                    var col = quad.GetComponent<Collider>();
                    if (col != null) Destroy(col);

                    var shader = Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
                    lcdMat = new Material(shader);
                    lcdMat.color = new Color(0.40f, 0.65f, 0.35f);
                    quad.GetComponent<MeshRenderer>().sharedMaterial = lcdMat;

                    var bodyGo = new GameObject("Body");
                    bodyGo.transform.SetParent(lcdRoot.transform, false);
                    bodyGo.transform.localPosition = Vector3.zero;
                    bodyGo.transform.localRotation = Quaternion.identity;
                    bodyGo.transform.localScale = Vector3.one * 0.00075f;
                    lcdBody = bodyGo.AddComponent<TextMesh>();
                    lcdBody.text = "INCOMING CALL\nBrother";
                    lcdBody.fontSize = 44;
                    lcdBody.fontStyle = FontStyle.Bold;
                    lcdBody.alignment = TextAlignment.Center;
                    lcdBody.anchor = TextAnchor.MiddleCenter;
                    lcdBody.color = new Color(0.05f, 0.12f, 0.04f);
                }
            }

            if (!lcdMat && lcdRoot != null)
            {
                var mr = lcdRoot.GetComponentInChildren<MeshRenderer>();
                if (mr != null) lcdMat = mr.material;
            }

            // Unconditionally remove any duplicate or extraneous text meshes so only single clean, big text appears
            if (lcdRoot != null)
            {
                for (int i = lcdRoot.transform.childCount - 1; i >= 0; i--)
                {
                    var child = lcdRoot.transform.GetChild(i);
                    if (child.name != "Body" && child.name != "LCD Backlight Quad")
                    {
                        if (Application.isPlaying) Destroy(child.gameObject);
                        else DestroyImmediate(child.gameObject);
                    }
                }
                lcdHeader = null;
            }

            if (!lcdBody && lcdRoot != null)
            {
                var b = lcdRoot.transform.Find("Body");
                if (b != null) lcdBody = b.GetComponent<TextMesh>();
            }

            if (lcdBody != null)
            {
                lcdBody.fontSize = 44;
                lcdBody.fontStyle = FontStyle.Bold;
                lcdBody.alignment = TextAlignment.Center;
                lcdBody.anchor = TextAnchor.MiddleCenter;
                lcdBody.color = new Color(0.05f, 0.12f, 0.04f);
                lcdBody.transform.localPosition = Vector3.zero;
                lcdBody.transform.localScale = Vector3.one * 0.00075f;
            }
        }

        void Update3DLcdScreen()
        {
            EnsureLcdScreen();
            if (lcdBody == null) return;

            int min = (int)(callTimer / 60);
            int sec = (int)(callTimer % 60);
            string timeStr = $"{min:00}:{sec:00}";

            if (currentState == PhoneState.Ringing || currentState == PhoneState.InspectingCall)
            {
                lcdBody.gameObject.SetActive(true);
                lcdBody.text = "INCOMING CALL\n" + callerName;
            }
            else if (currentState == PhoneState.InCall)
            {
                lcdBody.gameObject.SetActive(true);
                lcdBody.text = "CALL\n" + callerName + "\n" + timeStr;
            }
            else if (currentState == PhoneState.Ended)
            {
                lcdBody.gameObject.SetActive(true);
                lcdBody.text = "HUNG UP\n" + callerName + "\n" + timeStr;
            }
            else
            {
                if (lcdBody) lcdBody.gameObject.SetActive(false);
            }
        }

        float[] audioSamples = new float[64];

        void Update()
        {
            Update3DLcdScreen();

            // When holding the phone, keep the screen and keypad light ON continuously until put back on table!
            if (isHeld)
            {
                if (keypadBacklight)
                {
                    keypadBacklight.enabled = true;
                    keypadBacklight.intensity = 0.45f;
                }
                SetScreenGlow(1.2f);
                if (lcdMat != null)
                {
                    lcdMat.color = new Color(0.42f, 0.75f, 0.38f);
                    if (lcdBody) lcdBody.color = new Color(0.05f, 0.12f, 0.04f);
                }
            }
            else
            {
                // When on the table:
                if (currentState == PhoneState.Ringing)
                {
                    UpdateRingtoneLighting();
                }
                else
                {
                    // Ringing ended or call finished: turn lights completely OFF on table
                    if (keypadBacklight && keypadBacklight.enabled)
                    {
                        keypadBacklight.intensity = 0f;
                        keypadBacklight.enabled = false;
                    }
                    SetScreenGlow(0f);
                    if (lcdMat != null) lcdMat.color = new Color(0.18f, 0.28f, 0.15f);
                }
            }

            if (currentState == PhoneState.InspectingCall)
            {
                // Right click (or Esc) puts phone back on table without answering
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    PutBackOnTable();
                }
                // Left click ONLY answers call (pure mouse focus)
                else if (Input.GetMouseButtonDown(0))
                {
                    AnswerCall();
                }
            }
            else if (currentState == PhoneState.InCall)
            {
                callTimer += Time.deltaTime;
                // Right click is explicitly DISABLED during active call!
            }
            else if (currentState == PhoneState.Ended && isHeld)
            {
                // Only Right-Click (or Esc) puts the phone back after call ends
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    PutDownPhone();
                }
            }
        }

        void UpdateRingtoneLighting()
        {
            if (ringSource != null && ringSource.isPlaying)
            {
                ringSource.GetOutputData(audioSamples, 0);
                float sum = 0f;
                for (int i = 0; i < audioSamples.Length; i++)
                {
                    sum += Mathf.Abs(audioSamples[i]);
                }
                float rms = sum / audioSamples.Length;
                float loudness = Mathf.Clamp01(rms * 12f);

                if (loudness > 0.03f)
                {
                    float lightIntensity = Mathf.Clamp(loudness, 0.35f, 1.2f);
                    if (keypadBacklight)
                    {
                        keypadBacklight.enabled = true;
                        keypadBacklight.intensity = lightIntensity * 1.5f;
                    }
                    SetScreenGlow(lightIntensity * 4.5f);

                    if (lcdMat != null)
                    {
                        lcdMat.color = new Color(0.42f, 0.75f, 0.38f) * (0.85f + lightIntensity * 0.35f);
                        if (lcdHeader) lcdHeader.color = new Color(0.08f, 0.16f, 0.06f);
                        if (lcdBody) lcdBody.color = new Color(0.08f, 0.16f, 0.06f);
                    }
                }
                else
                {
                    // Silence / pause between music notes -> completely off!
                    if (keypadBacklight)
                    {
                        keypadBacklight.intensity = 0f;
                        keypadBacklight.enabled = false;
                    }
                    SetScreenGlow(0f);
                    if (lcdMat != null)
                    {
                        lcdMat.color = new Color(0.18f, 0.28f, 0.15f);
                        if (lcdHeader) lcdHeader.color = new Color(0.12f, 0.20f, 0.10f, 0.4f);
                        if (lcdBody) lcdBody.color = new Color(0.12f, 0.20f, 0.10f, 0.4f);
                    }
                }
            }
            else
            {
                // Ringtone stopped / vibration pause -> completely off!
                if (keypadBacklight)
                {
                    keypadBacklight.intensity = 0f;
                    keypadBacklight.enabled = false;
                }
                SetScreenGlow(0f);
                if (lcdMat != null)
                {
                    lcdMat.color = new Color(0.18f, 0.28f, 0.15f);
                    if (lcdHeader) lcdHeader.color = new Color(0.12f, 0.20f, 0.10f, 0.4f);
                    if (lcdBody) lcdBody.color = new Color(0.12f, 0.20f, 0.10f, 0.4f);
                }
            }
        }

        // ── Trigger incoming call ──────────────────────────────────────────
        public void TriggerIncomingCall()
        {
            EnsureTablePosition();
            if (currentState == PhoneState.InCall || currentState == PhoneState.InspectingCall) return;
            currentState = PhoneState.Ringing;
            if (ringLoop != null) StopCoroutine(ringLoop);
            ringLoop = StartCoroutine(RingLoop());
        }

        IEnumerator RingLoop()
        {
            while (currentState == PhoneState.Ringing || currentState == PhoneState.InspectingCall)
            {
                if (ringtoneClip && ringSource)
                {
                    ringSource.clip = ringtoneClip;
                    ringSource.loop = true;
                    ringSource.volume = 1.0f;
                    ringSource.Play();
                }

                float elapsed = 0;
                while (elapsed < 3.8f && (currentState == PhoneState.Ringing || currentState == PhoneState.InspectingCall))
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (ringSource && (currentState == PhoneState.Ringing || currentState == PhoneState.InspectingCall))
                {
                    ringSource.Stop();
                }

                // Turn lights completely off during vibration pause
                if (keypadBacklight)
                {
                    keypadBacklight.intensity = 0f;
                    keypadBacklight.enabled = false;
                }
                SetScreenGlow(0f);

                float vibrationWait = 0;
                while (vibrationWait < 1.0f && (currentState == PhoneState.Ringing || currentState == PhoneState.InspectingCall))
                {
                    vibrationWait += Time.deltaTime;
                    yield return null;
                }
            }
        }

        void SetScreenGlow(float intensity)
        {
            if (!screenRenderer) return;
            var mats = screenRenderer.materials;
            Color glow = new Color(0.35f, 0.95f, 0.35f) * intensity;
            if (mats.Length > 4 && mats[4] != null)
            {
                mats[4].SetColor("_EmissionColor", glow);
                if (intensity > 0.01f) mats[4].EnableKeyword("_EMISSION");
                else mats[4].DisableKeyword("_EMISSION");
            }
            if (mats.Length > 8 && mats[8] != null)
            {
                mats[8].SetColor("_EmissionColor", glow * 0.4f);
                if (intensity > 0.01f) mats[8].EnableKeyword("_EMISSION");
                else mats[8].DisableKeyword("_EMISSION");
            }
            screenRenderer.materials = mats;
        }

        // ── Step 1: Pick up phone to inspect incoming call ─────────────────
        public override void Use(PlayerInteraction player)
        {
            if (currentState == PhoneState.Ringing)
            {
                InspectCall(player);
            }
            else if (currentState == PhoneState.Ended && isHeld)
            {
                PutDownPhone();
            }
        }

        public void InspectCall(PlayerInteraction player)
        {
            if (currentState != PhoneState.Ringing) return;
            currentState = PhoneState.InspectingCall;
            isHeld = true;
            thoughtText = "It is really Brother...";

            var explorer = Camera.main ? Camera.main.GetComponent<ForestExploration>() : null;
            if (explorer)
            {
                explorer.MovementLocked = true;
                explorer.LookClamped = false;
                explorer.ClampCenterYaw = explorer.transform.eulerAngles.y;
                explorer.ClampYawRange = 40f;
            }

            if (keypadBacklight)
            {
                keypadBacklight.enabled = true;
                keypadBacklight.intensity = 0.22f;
            }
            SetScreenGlow(0.65f);

            StartCoroutine(LiftPhoneToView(null));
        }

        IEnumerator LiftPhoneToView(Action onLifted)
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            Transform camTrans = Camera.main ? Camera.main.transform : null;
            if (camTrans != null)
            {
                transform.SetParent(camTrans, true);
                Vector3 targetLocalPos = new Vector3(0f, -0.04f, 0.23f);
                Quaternion targetLocalRot = Quaternion.Euler(-90f, 0f, 0f);

                float liftDur = 0.35f;
                for (float t = 0; t < 1f; t += Time.deltaTime / liftDur)
                {
                    float smooth = Mathf.SmoothStep(0f, 1f, t);
                    transform.localPosition = Vector3.Lerp(camTrans.InverseTransformPoint(startPos), targetLocalPos, smooth);
                    transform.localRotation = Quaternion.Slerp(Quaternion.Inverse(camTrans.rotation) * startRot, targetLocalRot, smooth);
                    yield return null;
                }
                transform.localPosition = targetLocalPos;
                transform.localRotation = targetLocalRot;
            }
            onLifted?.Invoke();
        }

        // ── Option A: Put phone back on table without hanging up ───────────
        void PutBackOnTable()
        {
            if (!isHeld) return;
            isHeld = false;
            currentState = PhoneState.Ringing;
            thoughtText = null;

            var explorer = Camera.main ? Camera.main.GetComponent<ForestExploration>() : null;
            if (explorer)
            {
                explorer.MovementLocked = false;
                explorer.LookClamped = false;
            }

            // Ringtone continues playing on table!
            StartCoroutine(LowerPhoneToTable(false));
        }

        // ── Option B: Answer the call ──────────────────────────────────────
        void AnswerCall()
        {
            if (currentState != PhoneState.InspectingCall) return;
            currentState = PhoneState.InCall;
            callTimer = 0;
            thoughtText = null;

            if (ringLoop != null) StopCoroutine(ringLoop);
            if (ringSource && ringSource.isPlaying) ringSource.Stop();

            StartDialogue();
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
            if (keypadBacklight)
            {
                keypadBacklight.intensity = 0f;
                keypadBacklight.enabled = false;
            }
            SetScreenGlow(0f);

            if (ringSource && callEndedClip)
                ringSource.PlayOneShot(callEndedClip, 0.7f);

            // Do not auto put down: wait for player to click Right Mouse Button!
        }

        void PutDownPhone()
        {
            if (!isHeld) return;
            isHeld = false;
            if (keypadBacklight)
            {
                keypadBacklight.intensity = 0f;
                keypadBacklight.enabled = false;
            }
            SetScreenGlow(0f);
            currentState = PhoneState.Ended;

            var explorer = Camera.main ? Camera.main.GetComponent<ForestExploration>() : null;
            if (explorer)
            {
                explorer.MovementLocked = false;
                explorer.LookClamped = false;
            }

            StartCoroutine(LowerPhoneToTable(true));
        }

        IEnumerator LowerPhoneToTable(bool isCallCompleted)
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            transform.SetParent(originalParent, true);

            for (float t = 0; t < 1f; t += Time.deltaTime / 0.40f)
            {
                float smooth = Mathf.SmoothStep(0f, 1f, t);
                transform.position = Vector3.Lerp(startPos, originalPosition, smooth);
                transform.rotation = Quaternion.Slerp(startRot, originalRotation, smooth);
                yield return null;
            }
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            if (isCallCompleted)
            {
                ChapterOneDirector.Instance?.OnPhoneCallComplete();
            }
        }

        // ── Nokia LCD HUD & Thought Subtitle ──────────────────────────────
        void OnGUI()
        {
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            var prev = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);

            float sw = Screen.width / scale, sh = Screen.height / scale;

            // Render thought monologue subtitle at bottom center
            if (!string.IsNullOrEmpty(thoughtText))
            {
                float tw = 440, th = 40;
                float tx = (sw - tw) * 0.5f, ty = sh - 145f;
                GUI.color = new Color(0.04f, 0.05f, 0.04f, 0.88f);
                GUI.DrawTexture(new Rect(tx - 12, ty - 6, tw + 24, th + 12), Texture2D.whiteTexture);

                var thoughtStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 17,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter
                };
                thoughtStyle.normal.textColor = new Color(0.95f, 0.90f, 0.75f);
                GUI.color = Color.white;
                GUI.Label(new Rect(tx, ty, tw, th), "“" + thoughtText + "”", thoughtStyle);
            }

            // Clean mouse action prompt pill at bottom center (no edge box; interface is 100% on the 3D phone screen)
            if (isHeld)
            {
                if (currentState == PhoneState.InspectingCall)
                {
                    float pillW = 420f, pillH = 34f;
                    float px = (sw - pillW) * 0.5f, py = sh - 75f;
                    GUI.color = new Color(0.04f, 0.05f, 0.04f, 0.85f);
                    GUI.DrawTexture(new Rect(px - 10, py - 4, pillW + 20, pillH + 8), Texture2D.whiteTexture);
                    GUI.color = Color.white;

                    var promptStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 15,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter
                    };
                    promptStyle.normal.textColor = new Color(0.96f, 0.92f, 0.75f);
                    GUI.Label(new Rect(px, py, pillW, pillH), "[Left Click] Answer Call   •   [Right Click] Put Back", promptStyle);
                }
                else if (currentState == PhoneState.Ended)
                {
                    float pillW = 320f, pillH = 34f;
                    float px = (sw - pillW) * 0.5f, py = sh - 75f;
                    GUI.color = new Color(0.05f, 0.04f, 0.04f, 0.85f);
                    GUI.DrawTexture(new Rect(px - 10, py - 4, pillW + 20, pillH + 8), Texture2D.whiteTexture);
                    GUI.color = Color.white;

                    var promptStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 15,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter
                    };
                    promptStyle.normal.textColor = new Color(1f, 0.90f, 0.70f);
                    GUI.Label(new Rect(px, py, pillW, pillH), "[Right Click] Put back on table", promptStyle);
                }
            }

            GUI.matrix = prev;
        }
    }
}
