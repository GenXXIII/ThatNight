using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    public sealed partial class ChapterOneDirector
    {
        static Texture2D leftVignetteTex;
        static Texture2D GetLeftVignetteTex()
        {
            if (leftVignetteTex) return leftVignetteTex;
            leftVignetteTex = new Texture2D(128, 1, TextureFormat.RGBA32, false);
            leftVignetteTex.wrapMode = TextureWrapMode.Clamp;
            Color[] cols = new Color[128];
            for (int i = 0; i < 128; i++)
            {
                float u = (float)i / 127f;
                float alpha = Mathf.Pow(1f - u, 1.8f) * 0.88f;
                cols[i] = new Color(0.02f, 0.02f, 0.02f, alpha);
            }
            leftVignetteTex.SetPixels(cols);
            leftVignetteTex.Apply();
            return leftVignetteTex;
        }

        bool DrawCustomMenuButton(Rect rect, string text, bool isHover)
        {
            // Glass slate background
            GUI.color = isHover ? new Color(0.24f, 0.17f, 0.08f, 0.88f) : new Color(0.05f, 0.04f, 0.04f, 0.70f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            // 1px Gold border
            GUI.color = isHover ? new Color(0.96f, 0.84f, 0.44f, 0.95f) : new Color(0.68f, 0.54f, 0.28f, 0.45f);
            float bw = isHover ? 1.5f : 1.0f;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, bw), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - bw, rect.width, bw), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, bw, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - bw, rect.y, bw, rect.height), Texture2D.whiteTexture);

            // Corner ornamental notches when hovered
            if (isHover)
            {
                float c = 5f;
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

            // Text
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = isHover ? new Color(1f, 0.95f, 0.78f, 1f) : new Color(0.88f, 0.86f, 0.82f, 0.90f);
            GUI.Label(rect, text, style);

            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        void OnGUI()
        {
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);

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

                // 1. Cinemascope widescreen letterbox bars top & bottom
                float barH = sh * 0.055f;
                GUI.color = new Color(0.02f, 0.02f, 0.02f, 0.96f);
                GUI.DrawTexture(new Rect(0, 0, sw, barH), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, sh - barH, sw, barH), Texture2D.whiteTexture);
                // Subtle gold accent along letterbox edges
                GUI.color = new Color(0.68f, 0.54f, 0.28f, 0.35f);
                GUI.DrawTexture(new Rect(0, barH - 1, sw, 1), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, sh - barH, sw, 1), Texture2D.whiteTexture);
                GUI.color = Color.white;

                // 2. Smooth cinematic gradient vignette on the left (seamless fade into 3D world!)
                GUI.DrawTexture(new Rect(0, barH, 560, sh - barH * 2), GetLeftVignetteTex());

                float leftX = 85;
                float topY = sh * 0.22f;

                // 3. Khmer ornate heading in warm luminous gold
                var khmerHeadingStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 17,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                khmerHeadingStyle.normal.textColor = new Color(0.88f, 0.74f, 0.38f, 0.95f);
                GUI.Label(new Rect(leftX, topY, 440, 26), "✦   យ  ប់  នោះ   ✦", khmerHeadingStyle);

                // 4. Main title in monumental lettering with horror crimson shadow
                var titleShadowStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 48,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                titleShadowStyle.normal.textColor = new Color(0.38f, 0.05f, 0.05f, 0.85f);
                GUI.Label(new Rect(leftX + 2, topY + 28, 440, 56), "THAT NIGHT", titleShadowStyle);

                var titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 48,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                titleStyle.normal.textColor = new Color(0.98f, 0.96f, 0.92f, 1f);
                GUI.Label(new Rect(leftX, topY + 26, 440, 56), "THAT NIGHT", titleStyle);

                // 5. Chapter name & setting
                var chapterStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                chapterStyle.normal.textColor = new Color(0.82f, 0.78f, 0.70f, 0.92f);
                GUI.Label(new Rect(leftX, topY + 84, 440, 22), "CHAPTER ONE: THE WEDDING · បុណ្យអាពាហ៍ពិពាហ៍", chapterStyle);

                var settingStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.UpperLeft
                };
                settingStyle.normal.textColor = new Color(0.68f, 0.60f, 0.48f, 0.80f);
                GUI.Label(new Rect(leftX, topY + 106, 440, 20), "Battambang Countryside, Cambodia — April 2006", settingStyle);

                // Fine gold rule with diamond center
                GUI.color = new Color(0.72f, 0.58f, 0.30f, 0.40f);
                GUI.DrawTexture(new Rect(leftX, topY + 134, 340, 1), Texture2D.whiteTexture);
                GUI.color = Color.white;

                // 6. Interactive Custom Styled Buttons
                float btnY = topY + 155;
                float btnW = 320;
                float btnH = 46;
                float spacing = 58;

                Vector2 mousePos = Event.current != null ? Event.current.mousePosition : Vector2.zero;

                Rect rContinue = new Rect(leftX, btnY, btnW, btnH);
                bool hoverContinue = rContinue.Contains(mousePos);
                if (DrawCustomMenuButton(rContinue, "▶    CONTINUE", hoverContinue))
                {
                    StartFromIntroMenu();
                }

                Rect rNewGame = new Rect(leftX, btnY + spacing, btnW, btnH);
                bool hoverNewGame = rNewGame.Contains(mousePos);
                if (DrawCustomMenuButton(rNewGame, "✦    NEW GAME", hoverNewGame))
                {
                    StartFromIntroMenu();
                }

                Rect rQuit = new Rect(leftX, btnY + spacing * 2, btnW, btnH);
                bool hoverQuit = rQuit.Contains(mousePos);
                if (DrawCustomMenuButton(rQuit, "✕    QUIT", hoverQuit))
                {
                    Application.Quit();
                }

                GUI.matrix = prev;
            }

            // On-screen skip hint during Opening flyby cinematic with Circular Progress Ring around Space icon
            if (currentScene == GameScene.Scene01_Opening && !showIntroMenu && !menuTransitioning && fade < 0.85f)
            {
                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
                float sw = Screen.width / scale, sh = Screen.height / scale;

                float prog = Mathf.Clamp01(cinematicSpaceTimer / CinematicHoldToSkip);
                CircleSkipUI.Draw(sw - 200f, sh - 52f, prog, "Hold Space to Skip");

                GUI.matrix = prev;
            }

            if (!titleCard && !string.IsNullOrEmpty(objective) && fade < 0.5f && objectiveVisible)
            {
                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);

                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 17,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                style.normal.textColor = new Color(0.92f, 0.88f, 0.72f, 0.95f);

                GUI.color = new Color(0, 0, 0, 0.6f);
                GUI.Label(new Rect(34, 30, 800, 32), "◆  " + objective, style);
                GUI.color = new Color(0.92f, 0.88f, 0.72f, 0.95f);
                GUI.Label(new Rect(32, 28, 800, 32), "◆  " + objective, style);
                GUI.color = Color.white;

                // First-time instruction hints shown cleanly under objective
                string firstTimeHint = null;
                if (!hasPickedUpPhoneOnce && (objective.ToLower().Contains("phone") || objective.ToLower().Contains("call")))
                {
                    firstTimeHint = "Hint: [Left Click] Pick up Phone at mirror table";
                }
                else if (!hasOpenedDoorOnce && objective.ToLower().Contains("door"))
                {
                    firstTimeHint = "Hint: Hold [Left Click] to Open Bedroom Door";
                }

                if (!string.IsNullOrEmpty(firstTimeHint))
                {
                    var hintStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 13,
                        fontStyle = FontStyle.Italic,
                        alignment = TextAnchor.UpperLeft
                    };
                    hintStyle.normal.textColor = new Color(0.95f, 0.80f, 0.35f, 0.90f);
                    GUI.Label(new Rect(34, 58, 600, 24), firstTimeHint, hintStyle);
                }

                GUI.matrix = prev;

                // Draw floating diamond sign / marker above active task target in world
                DrawActiveTaskMarker();
            }

            if (titleCard)
            {
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

                var prev = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
                float sw = Screen.width / scale, sh = Screen.height / scale;

                var taglineStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 18,
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Normal
                };
                taglineStyle.normal.textColor = new Color(0.72f, 0.22f, 0.22f, 0.95f);

                var titleStyle = new GUIStyle(taglineStyle)
                {
                    fontSize = 56,
                    fontStyle = FontStyle.Bold
                };
                titleStyle.normal.textColor = new Color(0.95f, 0.94f, 0.90f, 1f);

                var chapterStyle = new GUIStyle(taglineStyle);
                chapterStyle.normal.textColor = new Color(0.85f, 0.75f, 0.55f, 0.85f);

                GUI.color = Color.white;
                GUI.Label(new Rect(0, sh * 0.34f, sw, 30), "A CAMBODIAN SUPERNATURAL HORROR GAME", taglineStyle);
                GUI.Label(new Rect(0, sh * 0.43f, sw, 80), "THAT NIGHT", titleStyle);
                GUI.Label(new Rect(0, sh * 0.56f, sw, 30), "CHAPTER ONE — THE WEDDING", chapterStyle);

                GUI.matrix = prev;
            }
        }

        static Texture2D _markerDiamondTex;

        Texture2D GetMarkerDiamondTex()
        {
            if (_markerDiamondTex != null) return _markerDiamondTex;
            int sz = 32;
            _markerDiamondTex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
            _markerDiamondTex.filterMode = FilterMode.Bilinear;
            Vector2 c = new Vector2(16, 16);
            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    float d = Mathf.Abs(x - c.x) + Mathf.Abs(y - c.y);
                    if (d <= 10f)
                    {
                        if (d >= 8f) _markerDiamondTex.SetPixel(x, y, new Color(0.95f, 0.85f, 0.40f, 0.95f));
                        else _markerDiamondTex.SetPixel(x, y, new Color(1f, 0.95f, 0.70f, 0.85f));
                    }
                    else if (d <= 12f)
                    {
                        _markerDiamondTex.SetPixel(x, y, new Color(0.1f, 0.08f, 0.02f, 0.5f));
                    }
                    else
                    {
                        _markerDiamondTex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            _markerDiamondTex.Apply();
            return _markerDiamondTex;
        }

        void DrawActiveTaskMarker()
        {
            var cam = MainCam;
            if (!cam || string.IsNullOrEmpty(objective)) return;

            Transform target = null;
            Vector3? explicitWorldPt = null;
            float yOffset = 0.25f;

            string objLower = objective.ToLower();
            if (objLower.Contains("phone") || objLower.Contains("call"))
            {
                if (nokia != null) target = nokia.transform;
            }
            else if (objLower.Contains("door"))
            {
                var doors = FindObjectsByType<HingedOpening>(FindObjectsSortMode.None);
                foreach (var d in doors)
                {
                    if (d.triggerDirectorOnOpen && !d.isOpen)
                    {
                        target = d.transform;
                        // Position marker exactly on the door panel near the handle
                        explicitWorldPt = d.transform.TransformPoint(d.handleLocal);
                        break;
                    }
                }
            }
            else if (objLower.Contains("mother"))
            {
                if (mother != null && mother.gameObject.activeSelf)
                {
                    target = mother.head != null ? mother.head : mother.transform;
                    yOffset = 0.35f;
                }
            }
            else if (objLower.Contains("wash") || objLower.Contains("kitchen"))
            {
                if (FaceWashStation.Instance != null && FaceWashStation.Instance.gameObject.activeSelf)
                {
                    target = FaceWashStation.Instance.transform;
                    yOffset = 0.85f; // Sits directly on top of the earthen water jar
                }
            }
            else if (objLower.Contains("makeup") || objLower.Contains("mirror"))
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

            if (screenPt.z > 0.4f && screenPt.z < 25f)
            {
                float sx = screenPt.x;
                float sy = Screen.height - screenPt.y;

                float dist = Vector3.Distance(cam.transform.position, worldPt);
                float alpha = Mathf.Clamp01((20f - dist) / 5f);
                float pulse = 0.85f + 0.15f * Mathf.Sin(Time.time * 4f);
                float mSize = Mathf.Clamp(24f * pulse, 18f, 28f);

                GUI.color = new Color(1f, 1f, 1f, alpha * pulse);
                GUI.DrawTexture(new Rect(sx - mSize * 0.5f, sy - mSize * 0.5f, mSize, mSize), GetMarkerDiamondTex());
                GUI.color = Color.white;
            }
        }
    }
}
