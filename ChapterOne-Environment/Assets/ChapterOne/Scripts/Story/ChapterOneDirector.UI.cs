using System;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed partial class ChapterOneDirector
    {
        static Texture2D leftVignetteTex;
        static Texture2D _markerDiamondTex;

        // Cached GUIStyles (initialized once — zero per-frame OnGUI GC allocations!)
        GUIStyle uiButtonStyle;
        GUIStyle uiKhmerHeadingStyle;
        GUIStyle uiTitleShadowStyle;
        GUIStyle uiTitleStyle;
        GUIStyle uiChapterStyle;
        GUIStyle uiSettingStyle;
        GUIStyle uiObjectiveStyle;
        GUIStyle uiHintStyle;
        GUIStyle uiTaglineStyle;
        GUIStyle uiCardTitleStyle;
        GUIStyle uiCardChapterStyle;

        void EnsureGuiStyles()
        {
            if (uiButtonStyle != null) return;

            uiButtonStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            uiKhmerHeadingStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            uiKhmerHeadingStyle.normal.textColor = new Color(0.88f, 0.74f, 0.38f, 0.95f);

            uiTitleShadowStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 48,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            uiTitleShadowStyle.normal.textColor = new Color(0.38f, 0.05f, 0.05f, 0.85f);

            uiTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 48,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            uiTitleStyle.normal.textColor = new Color(0.98f, 0.96f, 0.92f, 1f);

            uiChapterStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            uiChapterStyle.normal.textColor = new Color(0.82f, 0.78f, 0.70f, 0.92f);

            uiSettingStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.UpperLeft
            };
            uiSettingStyle.normal.textColor = new Color(0.68f, 0.60f, 0.48f, 0.80f);

            uiObjectiveStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            uiObjectiveStyle.normal.textColor = new Color(1f, 0.92f, 0.45f, 1f);

            uiHintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.UpperLeft
            };
            uiHintStyle.normal.textColor = new Color(0.95f, 0.80f, 0.35f, 0.90f);

            uiTaglineStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal
            };
            uiTaglineStyle.normal.textColor = new Color(0.72f, 0.22f, 0.22f, 0.95f);

            uiCardTitleStyle = new GUIStyle(uiTaglineStyle)
            {
                fontSize = 56,
                fontStyle = FontStyle.Bold
            };
            uiCardTitleStyle.normal.textColor = new Color(0.95f, 0.94f, 0.90f, 1f);

            uiCardChapterStyle = new GUIStyle(uiTaglineStyle);
            uiCardChapterStyle.normal.textColor = new Color(0.85f, 0.75f, 0.55f, 0.85f);
        }

        static bool ContainsCI(string source, string value)
        {
            return !string.IsNullOrEmpty(source) && source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static Texture2D GetLeftVignetteTex()
        {
            if (leftVignetteTex) return leftVignetteTex;
            leftVignetteTex = new Texture2D(128, 1, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            Color[] cols = new Color[128];
            for (int i = 0; i < 128; i++)
            {
                float u = (float)i / 127f;
                float alpha = Mathf.Pow(1f - u, 1.8f) * 0.88f;
                cols[i] = new Color(0.02f, 0.02f, 0.02f, alpha);
            }
            leftVignetteTex.SetPixels(cols);
            leftVignetteTex.Apply(false, true);
            return leftVignetteTex;
        }

        bool DrawCustomMenuButton(Rect rect, string text, bool isHover)
        {
            GUI.color = isHover ? new Color(0.24f, 0.17f, 0.08f, 0.88f) : new Color(0.05f, 0.04f, 0.04f, 0.70f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            GUI.color = isHover ? new Color(0.96f, 0.84f, 0.44f, 0.95f) : new Color(0.68f, 0.54f, 0.28f, 0.45f);
            float bw = isHover ? 1.5f : 1.0f;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, bw), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - bw, rect.width, bw), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, bw, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - bw, rect.y, bw, rect.height), Texture2D.whiteTexture);

            if (isHover)
            {
                const float c = 5f;
                GUI.color = new Color(1f, 0.90f, 0.55f, 1f);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.y - 2, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.y - 2, 2, c), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax - c + 2, rect.y - 2, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax, rect.y - 2, 2, c), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.yMax, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x - 2, rect.yMax - c + 2, 2, c), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax - c + 2, rect.yMax, c, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax, rect.yMax - c + 2, 2, c), Texture2D.whiteTexture);
            }
            GUI.color = Color.white;

            uiButtonStyle.normal.textColor = isHover ? new Color(1f, 0.95f, 0.78f, 1f) : new Color(0.88f, 0.86f, 0.82f, 0.90f);
            GUI.Label(rect, text, uiButtonStyle);

            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        void OnGUI()
        {
            EnsureGuiStyles();
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);

            if (shadowScareFlash > 0.005f)
            {
                GUI.color = new Color(0.06f, 0.01f, 0.01f, Mathf.Clamp01(shadowScareFlash) * 0.65f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }

            if (fade > 0.005f)
            {
                GUI.color = new Color(0, 0, 0, Mathf.Clamp01(fade));
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }

            if (showIntroMenu && !menuTransitioning)
            {
                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
                float sw = Screen.width / scale, sh = Screen.height / scale;

                float barH = sh * 0.055f;
                GUI.color = new Color(0.02f, 0.02f, 0.02f, 0.96f);
                GUI.DrawTexture(new Rect(0, 0, sw, barH), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, sh - barH, sw, barH), Texture2D.whiteTexture);
                GUI.color = new Color(0.68f, 0.54f, 0.28f, 0.35f);
                GUI.DrawTexture(new Rect(0, barH - 1, sw, 1), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, sh - barH, sw, 1), Texture2D.whiteTexture);
                GUI.color = Color.white;

                GUI.DrawTexture(new Rect(0, barH, 560, sh - barH * 2), GetLeftVignetteTex());

                const float leftX = 85f;
                float topY = sh * 0.22f;

                GUI.Label(new Rect(leftX, topY, 440, 26), "✦   យ  ប់  នោះ   ✦", uiKhmerHeadingStyle);
                GUI.Label(new Rect(leftX + 2, topY + 28, 440, 56), "THAT NIGHT", uiTitleShadowStyle);
                GUI.Label(new Rect(leftX, topY + 26, 440, 56), "THAT NIGHT", uiTitleStyle);
                GUI.Label(new Rect(leftX, topY + 84, 440, 22), "CHAPTER ONE: THE WEDDING · បុណ្យអាពាហ៍ពិពាហ៍", uiChapterStyle);
                GUI.Label(new Rect(leftX, topY + 106, 440, 20), "Battambang Countryside, Cambodia — April 2006", uiSettingStyle);

                GUI.color = new Color(0.72f, 0.58f, 0.30f, 0.40f);
                GUI.DrawTexture(new Rect(leftX, topY + 134, 340, 1), Texture2D.whiteTexture);
                GUI.color = Color.white;

                float btnY = topY + 155f;
                const float btnW = 320f, btnH = 46f, spacing = 58f;
                Vector2 mousePos = Event.current != null ? Event.current.mousePosition : Vector2.zero;

                Rect rContinue = new Rect(leftX, btnY, btnW, btnH);
                if (DrawCustomMenuButton(rContinue, "▶    CONTINUE", rContinue.Contains(mousePos)))
                {
                    StartFromIntroMenu();
                }

                Rect rNewGame = new Rect(leftX, btnY + spacing, btnW, btnH);
                if (DrawCustomMenuButton(rNewGame, "✦    NEW GAME", rNewGame.Contains(mousePos)))
                {
                    StartFromIntroMenu();
                }

                Rect rQuit = new Rect(leftX, btnY + spacing * 2, btnW, btnH);
                if (DrawCustomMenuButton(rQuit, "✕    QUIT", rQuit.Contains(mousePos)))
                {
                    Application.Quit();
                }

                GUI.matrix = prev;
            }

            if (!titleCard && !string.IsNullOrEmpty(objective) && fade < 0.5f && objectiveVisible)
            {
                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);

                string labelText = "◆  " + objective;
                GUI.color = new Color(0, 0, 0, 0.92f);
                GUI.Label(new Rect(34, 30, 800, 32), labelText, uiObjectiveStyle);
                GUI.Label(new Rect(33, 29, 800, 32), labelText, uiObjectiveStyle);
                GUI.color = new Color(1f, 0.92f, 0.45f, 1f);
                GUI.Label(new Rect(32, 28, 800, 32), labelText, uiObjectiveStyle);
                GUI.color = Color.white;

                string firstTimeHint = GetObjectiveHint();
                if (!string.IsNullOrEmpty(firstTimeHint))
                {
                    GUI.Label(new Rect(34, 58, 600, 24), firstTimeHint, uiHintStyle);
                }

                GUI.matrix = prev;
                DrawActiveTaskMarker();
            }

            if (titleCard)
            {
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
                float sw = Screen.width / scale, sh = Screen.height / scale;

                GUI.color = Color.white;
                GUI.Label(new Rect(0, sh * 0.34f, sw, 30), "A CAMBODIAN SUPERNATURAL HORROR GAME", uiTaglineStyle);
                GUI.Label(new Rect(0, sh * 0.43f, sw, 80), "THAT NIGHT", uiCardTitleStyle);
                GUI.Label(new Rect(0, sh * 0.56f, sw, 30), "CHAPTER ONE — THE WEDDING", uiCardChapterStyle);

                GUI.matrix = prev;
            }
        }

        string GetObjectiveHint()
        {
            if (objective == "Look around") return "Hint: Move Mouse to look around";
            if (!hasPickedUpFlashlightOnce && ContainsCI(objective, "flashlight"))
                return "Hint: Look down and [Left Click] to pick up the Flashlight";
            if (ContainsCI(objective, "crying"))
                return "Hint: Use [W, A, S, D] to walk and [F] to toggle Flashlight";
            if (!hasPickedUpPhoneOnce && (ContainsCI(objective, "phone") || ContainsCI(objective, "call")))
                return "Hint: [Left Click] Pick up Phone at mirror table";
            if (!hasOpenedDoorOnce && ContainsCI(objective, "door"))
                return "Hint: Hold [Left Click] to Open Bedroom Door";
            return null;
        }

        static Texture2D GetMarkerDiamondTex()
        {
            if (_markerDiamondTex != null) return _markerDiamondTex;
            const int sz = 32;
            _markerDiamondTex = new Texture2D(sz, sz, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear
            };
            Color[] pixels = new Color[sz * sz];
            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    float d = Mathf.Abs(x - 16f) + Mathf.Abs(y - 16f);
                    Color col;
                    if (d <= 11f)
                    {
                        if (d >= 9f) col = new Color(0.08f, 0.05f, 0.01f, 1.0f);
                        else if (d <= 4f) col = Color.white;
                        else col = new Color(1f, 0.84f, 0.18f, 1.0f);
                    }
                    else if (d <= 13f) col = new Color(0f, 0f, 0f, 0.80f);
                    else col = Color.clear;
                    pixels[y * sz + x] = col;
                }
            }
            _markerDiamondTex.SetPixels(pixels);
            _markerDiamondTex.Apply(false, true);
            return _markerDiamondTex;
        }

        void DrawActiveTaskMarker()
        {
            var cam = MainCam;
            if (!cam || string.IsNullOrEmpty(objective)) return;

            Transform target = null;
            Vector3? explicitWorldPt = null;
            float yOffset = 0.25f;

            if (ContainsCI(objective, "flashlight"))
            {
                if (forestFlashlight != null && !forestFlashlight.IsPickedUp)
                {
                    target = forestFlashlight.transform;
                    yOffset = 0.22f;
                }
            }
            else if (ContainsCI(objective, "phone") || ContainsCI(objective, "call"))
            {
                if (nokia != null) target = nokia.transform;
            }
            else if (ContainsCI(objective, "door"))
            {
                if (bridalDoor != null && !bridalDoor.isOpen)
                {
                    target = bridalDoor.transform;
                    explicitWorldPt = bridalDoor.transform.TransformPoint(bridalDoor.handleLocal);
                }
            }
            else if (ContainsCI(objective, "mother"))
            {
                if (mother != null && mother.gameObject.activeSelf)
                {
                    target = mother.head != null ? mother.head : mother.transform;
                    yOffset = 0.35f;
                }
            }
            else if (ContainsCI(objective, "wash") || ContainsCI(objective, "kitchen"))
            {
                if (FaceWashStation.Instance != null && FaceWashStation.Instance.gameObject.activeSelf)
                {
                    target = FaceWashStation.Instance.transform;
                    yOffset = 0.85f;
                }
            }
            else if (ContainsCI(objective, "makeup") || ContainsCI(objective, "mirror"))
            {
                if (MakeupMirrorStation.Instance != null)
                {
                    target = MakeupMirrorStation.Instance.transform;
                    yOffset = 0.50f;
                }
            }

            if (target == null && !explicitWorldPt.HasValue) return;

            Vector3 worldPt = explicitWorldPt ?? (target.position + Vector3.up * yOffset);
            Vector3 screenPt = cam.WorldToScreenPoint(worldPt);

            if (screenPt.z > 0.4f && screenPt.z < 45f)
            {
                float sx = screenPt.x;
                float sy = Screen.height - screenPt.y;
                float dist = Vector3.Distance(cam.transform.position, worldPt);
                float alpha = Mathf.Clamp01((42f - dist) / 10f);
                float pulse = 0.90f + 0.10f * Mathf.Sin(Time.time * 5f);
                float mSize = Mathf.Clamp(30f * pulse, 26f, 34f);

                GUI.color = new Color(1f, 1f, 1f, Mathf.Max(0.75f, alpha * pulse));
                GUI.DrawTexture(new Rect(sx - mSize * 0.5f, sy - mSize * 0.5f, mSize, mSize), GetMarkerDiamondTex());
                GUI.color = Color.white;
            }
        }
    }
}
