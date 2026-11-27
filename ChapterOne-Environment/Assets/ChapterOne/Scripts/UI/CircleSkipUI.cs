// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// CircleSkipUI.cs — Circular Hold-Space Skip Indicator
//
// Shows a sleek Space key icon with a circular progress ring around it.
// Uses cached GUIStyles and Color32 pixel buffer for zero per-frame GC.
// ============================================================================

using UnityEngine;

namespace ThatNight.ChapterOne
{
    public static class CircleSkipUI
    {
        const int TexSize = 64;
        static Texture2D _ringTex;
        static Color32[] _pixels;
        static float _lastFill = -1f;
        static GUIStyle _keyStyle;
        static GUIStyle _lblStyle;

        public static Texture2D GetRingTexture(float fillAmount)
        {
            if (_ringTex == null)
            {
                _ringTex = new Texture2D(TexSize, TexSize, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Bilinear
                };
                _pixels = new Color32[TexSize * TexSize];
                _lastFill = -1f;
            }

            float fillSnapped = Mathf.Round(fillAmount * 60f) / 60f;
            if (Mathf.Approximately(fillSnapped, _lastFill)) return _ringTex;
            _lastFill = fillSnapped;

            const float center = 31.5f;
            const float rOuter = 29f;
            const float rInner = 22f;
            const float minR = rInner - 1.2f;
            const float maxR = rOuter + 1.2f;
            const float minRSq = minR * minR;
            const float maxRSq = maxR * maxR;
            const float invTwoPi = 1f / (Mathf.PI * 2f);
            bool hasFill = fillSnapped > 0.01f;

            for (int y = 0; y < TexSize; y++)
            {
                float dy = y - center;
                int row = y * TexSize;
                for (int x = 0; x < TexSize; x++)
                {
                    float dx = x - center;
                    float distSq = dx * dx + dy * dy;
                    if (distSq >= minRSq && distSq <= maxRSq)
                    {
                        float dist = Mathf.Sqrt(distSq);
                        float angle = Mathf.Atan2(dx, dy);
                        if (angle < 0f) angle += Mathf.PI * 2f;
                        float normAngle = angle * invTwoPi;

                        float ringAlpha = 1f;
                        if (dist < rInner) ringAlpha = Mathf.Clamp01(dist - minR);
                        else if (dist > rOuter) ringAlpha = Mathf.Clamp01(maxR - dist);

                        if (hasFill && normAngle <= fillSnapped)
                        {
                            _pixels[row + x] = new Color32(245, 199, 56, (byte)(242f * ringAlpha));
                        }
                        else
                        {
                            _pixels[row + x] = new Color32(51, 51, 51, (byte)(140f * ringAlpha));
                        }
                    }
                    else
                    {
                        _pixels[row + x] = new Color32(0, 0, 0, 0);
                    }
                }
            }
            _ringTex.SetPixels32(_pixels);
            _ringTex.Apply(false, false);
            return _ringTex;
        }

        public static void Draw(float x, float y, float progress, string label = "Hold Space to Skip")
        {
            if (_keyStyle == null)
            {
                _keyStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                _lblStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            const float ringSize = 34f;
            const float pillW = 185f;
            const float pillH = 38f;

            GUI.color = new Color(0.04f, 0.04f, 0.04f, 0.65f);
            GUI.DrawTexture(new Rect(x - 6f, y - 2f, pillW, pillH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.DrawTexture(new Rect(x, y, ringSize, ringSize), GetRingTexture(progress));

            bool active = progress > 0.05f;
            _keyStyle.normal.textColor = active ? new Color(1f, 0.95f, 0.7f) : new Color(0.85f, 0.85f, 0.85f, 0.9f);
            GUI.Label(new Rect(x, y - 1f, ringSize, ringSize), "␣", _keyStyle);

            _lblStyle.normal.textColor = active ? new Color(0.96f, 0.82f, 0.35f) : new Color(0.85f, 0.85f, 0.85f, 0.85f);
            GUI.Label(new Rect(x + ringSize + 8f, y, 140f, ringSize), label, _lblStyle);
        }
    }
}
