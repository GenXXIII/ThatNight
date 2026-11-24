using System;
using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    [DefaultExecutionOrder(20)]
    public sealed class PlayerInteraction : MonoBehaviour
    {
        public FirstPersonBody body;
        public ForestExploration explorer;
        public Transform arrival;
        public PickupItem HeldItem { get; private set; }
        public WorldInteractable Focus { get; private set; }
        public bool Busy { get; private set; }
        public WeddingGuest Speaker { get; private set; }
        string speech;
        GUIStyle text, title, prompt, button;
        bool help = true;
        HingedOpening activeDoorGrab = null;

        // Procedural Reticle Textures (Open Hand, Closed Grab Hand, Dialogue Talk Cursor)
        static Texture2D _handOpenTexture;
        static Texture2D _handGrabTexture;
        static Texture2D _talkCursorTexture;

        public static Texture2D GetHandOpenTexture()
        {
            if (_handOpenTexture != null) return _handOpenTexture;
            string path = Application.dataPath + "/ChapterOne/Textures/Cursors/UnityHandOpen.png";
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                _handOpenTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                _handOpenTexture.filterMode = FilterMode.Bilinear;
                _handOpenTexture.LoadImage(bytes);
                return _handOpenTexture;
            }
            int size = 32;
            _handOpenTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            return _handOpenTexture;
        }

        public static Texture2D GetHandGrabTexture()
        {
            if (_handGrabTexture != null) return _handGrabTexture;
            string path = Application.dataPath + "/ChapterOne/Textures/Cursors/UnityHandGrab.png";
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                _handGrabTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                _handGrabTexture.filterMode = FilterMode.Bilinear;
                _handGrabTexture.LoadImage(bytes);
                return _handGrabTexture;
            }
            int size = 32;
            _handGrabTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            return _handGrabTexture;
        }

        public static Texture2D GetTalkCursorTexture()
        {
            if (_talkCursorTexture != null) return _talkCursorTexture;
            string path = Application.dataPath + "/ChapterOne/Textures/Cursors/UnityTalkCursor.png";
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                _talkCursorTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                _talkCursorTexture.filterMode = FilterMode.Bilinear;
                _talkCursorTexture.LoadImage(bytes);
                return _talkCursorTexture;
            }
            int size = 32;
            _talkCursorTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            return _talkCursorTexture;
        }

        static float DistCircle(Vector2 p, Vector2 center, float radius)
        {
            return Vector2.Distance(p, center) - radius;
        }

        static float DistCapsule(Vector2 p, Vector2 a, Vector2 b, float radius)
        {
            Vector2 pa = p - a, ba = b - a;
            float h = Mathf.Clamp01(Vector2.Dot(pa, ba) / Vector2.Dot(ba, ba));
            return Vector2.Distance(p, a + ba * h) - radius;
        }

        static Color EvalReticlePixel(float d)
        {
            if (d <= 0f) return Color.white;
            if (d <= 1.2f) return Color.Lerp(Color.white, new Color(0.05f, 0.05f, 0.05f, 0.95f), d / 1.2f);
            if (d <= 3.0f) return new Color(0.05f, 0.05f, 0.05f, 0.95f);
            if (d <= 4.5f) return new Color(0.05f, 0.05f, 0.05f, Mathf.Lerp(0.95f, 0f, (d - 3.0f) / 1.5f));
            return Color.clear;
        }

        void Start()
        {
            if (PreviewMode.EnvironmentPreview || PreviewMode.EnvironmentReview || PreviewMode.ForestPreview || ChapterOneDirector.Instance != null) return;
            if (!Array.Exists(Environment.GetCommandLineArgs(), x => x == "-forestPreview")) EnterGameplay();
        }

        public void EnterGameplay()
        {
            if (arrival == null)
            {
                var spawn = GameObject.Find("PlayerArrival") ?? GameObject.Find("02 ARRIVAL - bridal arrival vehicle");
                if (spawn) arrival = spawn.transform;
            }
            if (explorer && arrival != null)
            {
                explorer.SetWalking(false);
                transform.SetPositionAndRotation(arrival.position, arrival.rotation);
            }
            var envCam = GetComponent<EnvironmentCamera>();
            if (envCam && envCam.lighting) envCam.lighting.ApplyStage(AtmosphereStage.AfternoonCelebration);
            if (explorer) explorer.SetWalking(true);
        }

        public void OnDoorAutoReleased()
        {
            activeDoorGrab = null;
            ReleaseGrabMovementLock();
        }

        public void ReleaseGrabMovementLock()
        {
            if (explorer)
            {
                explorer.MovementLocked = false;
                explorer.GlanceLocked = false;
                explorer.LookClamped = false;
            }
        }

        void Update()
        {
            if (GameplayReview.IsRunning) return;
            if (Input.GetKeyDown(KeyCode.H)) help = !help;
            if (Input.GetKeyDown(KeyCode.F7) && !Busy) { CloseDialogue(); EnterGameplay(); }
            if (!explorer || !explorer.Walking) { CloseDialogue(); Focus = null; return; }
            if (ChapterOneNarrative.Instance && ChapterOneNarrative.Instance.IsPlaying)
            { Focus = null; return; }
            if (ChapterOneDirector.Instance && ChapterOneDirector.Instance.SequenceBusy)
            { Focus = null; return; }
            if (Speaker)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) CloseDialogue();
                if (Input.GetKeyDown(KeyCode.Alpha1)) Choose(0);
                if (Input.GetKeyDown(KeyCode.Alpha2)) Choose(1);
                return;
            }

            // Handle active door grab: Hold Left Mouse Button ONLY to swing, release to spring back
            if (activeDoorGrab != null)
            {
                Vector3 handleWorld = activeDoorGrab.transform.TransformPoint(activeDoorGrab.handleLocal);
                Vector3 playerFeet = (explorer && explorer.Body) ? explorer.Body.transform.position : (transform.position - Vector3.up * 1.54f);
                float distToHandle = Vector3.Distance(new Vector3(playerFeet.x, handleWorld.y, playerFeet.z), handleWorld);

                if (Input.GetKeyDown(KeyCode.Escape) || distToHandle > 2.6f)
                {
                    var door = activeDoorGrab;
                    activeDoorGrab = null;
                    door.ExitGrab(this);
                    ReleaseGrabMovementLock();
                    return;
                }

                if (Input.GetMouseButton(0) && Cursor.lockState == CursorLockMode.Locked)
                {
                    activeDoorGrab.HoldMove(this, Time.deltaTime);
                }
                else
                {
                    // Released mouse button before auto-completing: spring back!
                    var door = activeDoorGrab;
                    activeDoorGrab = null;
                    door.OnHoldReleased();
                    ReleaseGrabMovementLock();
                }
                return;
            }

            // Put back held item with [Esc], [Q], or [Right Click]
            if (HeldItem && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1)))
            {
                ReturnHeldItem();
                return;
            }

            if (Busy) return;

            // Direct point-specific raycast targeting (from 0m to 2.0m)
            const float maxInteractReach = 2.0f;
            Ray ray = new Ray(transform.position, transform.forward);
            WorldInteractable bestTarget = null;

            // 1. RaycastAll along line of sight so sub-meshes / tabletop don't block the interactable
            RaycastHit[] hits = Physics.RaycastAll(ray, maxInteractReach, ~(1 << 2), QueryTriggerInteraction.Ignore);
            if (hits != null && hits.Length > 0)
            {
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                foreach (var h in hits)
                {
                    var interactable = h.collider.GetComponentInParent<WorldInteractable>();
                    if (interactable != null && interactable.CanUse && h.distance <= interactable.Reach)
                    {
                        bestTarget = interactable;
                        break;
                    }
                }
            }

            // 2. Fallback gentle 0.08m spherecast to effortlessly target small objects (e.g. phone) from 0m to 2m
            if (bestTarget == null)
            {
                RaycastHit[] sHits = Physics.SphereCastAll(ray, 0.08f, maxInteractReach, ~(1 << 2), QueryTriggerInteraction.Ignore);
                if (sHits != null && sHits.Length > 0)
                {
                    Array.Sort(sHits, (a, b) => a.distance.CompareTo(b.distance));
                    foreach (var sh in sHits)
                    {
                        var interactable = sh.collider.GetComponentInParent<WorldInteractable>();
                        if (interactable != null && interactable.CanUse && sh.distance <= interactable.Reach)
                        {
                            bestTarget = interactable;
                            break;
                        }
                    }
                }
            }

            Focus = bestTarget;

            // WORLD INTERACTION FOCUSES ON LEFT MOUSE CLICK ONLY (Key E is reserved for decisions like answering phone)
            if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.Locked && Focus)
            {
                var target = Focus;
                if (target is HingedOpening door)
                {
                    if (ChapterOneDirector.Instance != null) ChapterOneDirector.Instance.hasOpenedDoorOnce = true;
                    activeDoorGrab = door;
                    door.Grab(this);
                    if (explorer)
                    {
                        // Enable player movement during hold open/close door
                        explorer.MovementLocked = false;
                    }
                }
                else if (target is NokiaHandPhone phone)
                {
                    if (ChapterOneDirector.Instance != null) ChapterOneDirector.Instance.hasPickedUpPhoneOnce = true;
                    phone.InspectCall(this);
                }
                else
                {
                    target.Use(this);
                }
            }
        }

        public void ReturnHeldItem()
        {
            if (!HeldItem) return;
            var item = HeldItem;
            ReachAndAct(transform.position, () =>
            {
                item.PutBack();
                HeldItem = null;
                if (body) body.Holding = false;
            });
        }

        public void ReachAndAct(Vector3 target, Action action)
        {
            if (Busy || Speaker) return;
            action?.Invoke();
        }

        public void Hold(PickupItem item)
        {
            HeldItem = item;
            if (body && body.holdSocket) item.Attach(body.holdSocket);
            if (body) body.Holding = true;
        }

        public void Talk(WeddingGuest guest)
        {
            Speaker = guest;
            speech = guest.greeting;
            if (explorer)
            {
                explorer.MovementLocked = true;
                explorer.LookClamped = true;
                explorer.ClampCenterYaw = explorer.transform.eulerAngles.y;
                explorer.ClampYawRange = 35f;
            }
        }

        public void Choose(int choice)
        {
            if (Speaker) speech = choice == 0 ? Speaker.weddingAnswer : Speaker.helpAnswer;
        }

        public void CloseDialogue()
        {
            if (!Speaker) return;
            Speaker = null;
            if (explorer)
            {
                explorer.ControlsLocked = false;
                explorer.MovementLocked = false;
                explorer.LookClamped = false;
            }
        }

        void LateUpdate()
        {
            if (HeldItem && body && body.holdSocket)
            {
                HeldItem.transform.position = body.holdSocket.position;
                HeldItem.transform.rotation = body.holdSocket.rotation;
            }
            if (explorer && explorer.Walking)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();
            Busy = false;
            if (activeDoorGrab != null)
            {
                activeDoorGrab.ExitGrab(this);
                activeDoorGrab = null;
            }
            ReleaseGrabMovementLock();
            if (explorer) explorer.ControlsLocked = false;
        }

        void OnGUI()
        {
            if (!explorer || !explorer.Walking) return;
            if (text == null)
            {
                text = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
                text.normal.textColor = new Color(.91f, .89f, .81f);
                title = new GUIStyle(text) { fontSize = 22, fontStyle = FontStyle.Bold };
                prompt = new GUIStyle(text) { alignment = TextAnchor.MiddleCenter };
                button = new GUIStyle(GUI.skin.button) { fontSize = 17, alignment = TextAnchor.MiddleLeft, padding = new RectOffset(15, 12, 8, 8) };
            }
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            var previous = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            float w = Screen.width / scale, h = Screen.height / scale;

            if (!Speaker)
            {
                const float cursorSize = 17f; // Subtle, neat Unity hand cursor size (17x17 pixels)
                float cx = w * 0.5f - cursorSize * 0.5f;
                float cy = h * 0.5f - cursorSize * 0.5f;

                if (activeDoorGrab != null)
                {
                    // While holding Left Mouse to open/close door: Small Grab Hand icon (16x16), NO text underneath
                    var grabTex = GetHandGrabTexture();
                    GUI.color = new Color(1f, 1f, 1f, 0.95f);
                    GUI.DrawTexture(new Rect(cx, cy, cursorSize, cursorSize), grabTex);
                    GUI.color = Color.white;
                }
                else if (Focus != null)
                {
                    // Hovering NPC/Speaker: Talk / Speech Bubble cursor. Hovering object/door: Open Hand cursor (16x16), NO text underneath
                    bool isTalkTarget = Focus is WeddingGuest || (Focus.displayName != null && (Focus.displayName.Contains("Mother") || Focus.displayName.Contains("Guest")));
                    Texture2D cursorTex = isTalkTarget ? GetTalkCursorTexture() : GetHandOpenTexture();

                    GUI.color = new Color(1f, 1f, 1f, 0.95f);
                    GUI.DrawTexture(new Rect(cx, cy, cursorSize, cursorSize), cursorTex);
                    GUI.color = Color.white;
                }
                else
                {
                    // Normal state (not pointing at an object): Subtle, clean 2x2 white dot
                    GUI.color = new Color(0.95f, 0.95f, 0.95f, 0.70f);
                    GUI.DrawTexture(new Rect(w * 0.5f - 1f, h * 0.5f - 1f, 2f, 2f), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }

                if (HeldItem)
                {
                    GUI.Label(new Rect(28, h - 75, 600, 35), HeldItem.displayName + "   •   [Right Click / Esc] Put back", text);
                }
            }
            else
            {
                float x = (w - 800) * .5f, y = h - 264;
                GUI.color = new Color(.025f, .035f, .033f, .96f);
                GUI.DrawTexture(new Rect(x, y, 800, 240), Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.Label(new Rect(x + 24, y + 18, 752, 32), Speaker.displayName + "  /  " + Speaker.role, title);
                GUI.Label(new Rect(x + 24, y + 58, 752, 70), speech, text);
                if (GUI.Button(new Rect(x + 24, y + 140, 365, 40), "1  About the wedding", button)) Choose(0);
                if (GUI.Button(new Rect(x + 411, y + 140, 365, 40), "2  Can you help me?", button)) Choose(1);
                if (GUI.Button(new Rect(x + 24, y + 190, 752, 32), "E / Esc   Leave conversation", button)) CloseDialogue();
            }
            GUI.matrix = previous;
        }
    }
}
