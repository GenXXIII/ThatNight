// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// CircleSkipUI.cs — Circular Hold-Space Skip Indicator
//
// Shows a sleek Space key icon with a circular progress ring around it.
// When holding Space, the ring sweeps 0° to 360° to skip cinematics and dialogues.
// ============================================================================

using UnityEngine;

namespace ThatNight.ChapterOne
{
    public static class CircleSkipUI
    {
        static Texture2D _ringTex;
        static float _lastFill = -1f;

        public static Texture2D GetRingTexture(float fillAmount)
        {
            if (_ringTex == null)
            {
                _ringTex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
                _ringTex.filterMode = FilterMode.Bilinear;
                _lastFill = -1f;
            }

            float fillSnapped = Mathf.Round(fillAmount * 60f) / 60f;
            if (Mathf.Approximately(fillSnapped, _lastFill)) return _ringTex;
            _lastFill = fillSnapped;

            Vector2 center = new Vector2(31.5f, 31.5f);
            float rOuter = 29f;
            float rInner = 22f;

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    Vector2 p = new Vector2(x, y);
                    float dist = Vector2.Distance(p, center);
                    if (dist >= rInner - 1.2f && dist <= rOuter + 1.2f)
                    {
                        float angle = Mathf.Atan2(p.x - center.x, p.y - center.y);
                        if (angle < 0) angle += Mathf.PI * 2f;
                        float normAngle = angle / (Mathf.PI * 2f);

                        float ringAlpha = 1f;
                        if (dist < rInner) ringAlpha = Mathf.Clamp01(dist - (rInner - 1.2f));
                        else if (dist > rOuter) ringAlpha = Mathf.Clamp01((rOuter + 1.2f) - dist);

                        if (normAngle <= fillSnapped && fillSnapped > 0.01f)
                        {
                            // Active glowing gold progress ring
                            _ringTex.SetPixel(x, y, new Color(0.96f, 0.78f, 0.22f, 0.95f * ringAlpha));
                        }
                        else
                        {
                            // Subtle track ring
                            _ringTex.SetPixel(x, y, new Color(0.20f, 0.20f, 0.20f, 0.55f * ringAlpha));
                        }
                    }
                    else
                    {
                        _ringTex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            _ringTex.Apply();
            return _ringTex;
        }

        public static void Draw(float x, float y, float progress, string label = "Hold Space to Skip")
        {
            float ringSize = 34f;
            float pillW = 185f, pillH = 38f;

            GUI.color = new Color(0.04f, 0.04f, 0.04f, 0.65f);
            GUI.DrawTexture(new Rect(x - 6f, y - 2f, pillW, pillH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var tex = GetRingTexture(progress);
            GUI.DrawTexture(new Rect(x, y, ringSize, ringSize), tex);

            var keyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            keyStyle.normal.textColor = progress > 0.05f ? new Color(1f, 0.95f, 0.7f) : new Color(0.85f, 0.85f, 0.85f, 0.9f);
            GUI.Label(new Rect(x, y - 1f, ringSize, ringSize), "␣", keyStyle);

            var lblStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft
            };
            lblStyle.normal.textColor = progress > 0.05f ? new Color(0.96f, 0.82f, 0.35f) : new Color(0.85f, 0.85f, 0.85f, 0.85f);
            GUI.Label(new Rect(x + ringSize + 8f, y, 140f, ringSize), label, lblStyle);
        }
    }
}
