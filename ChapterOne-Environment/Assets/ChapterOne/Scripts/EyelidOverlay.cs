using System;
using System.Collections;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    /// <summary>
    /// Photorealistic double-eyelid waking blink and blur overlay.
    /// Provides cinematic eyelid opening transitions for Scene 01 -> Scene 02.
    /// Fully procedural and robust across all rendering pipelines.
    /// </summary>
    public sealed class EyelidOverlay : MonoBehaviour
    {
        public static EyelidOverlay Instance { get; private set; }

        [Range(0f, 1f)] public float openAmount = 1f;
        [Range(0f, 1f)] public float drowsiness = 0f;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);
            openAmount = 1f;
            drowsiness = 0f;
        }

        /// <summary>
        /// Plays the classic drowsy waking up double-blink sequence:
        /// Eyes closed -> first flutter -> brief close -> second wider open -> gentle blink -> full waking focus.
        /// </summary>
        public IEnumerator PlayWakingSequence(Action onHalfAwake = null, Action onFullyAwake = null)
        {
            openAmount = 0f;
            drowsiness = 1.0f;
            yield return new WaitForSeconds(0.6f);

            // 1. First peek open (drowsy, squinting into morning light)
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 1.5f;
                openAmount = Mathf.Lerp(0f, 0.42f, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }

            yield return new WaitForSeconds(0.4f);

            // 2. Heavy lids flutter shut
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.6f;
                openAmount = Mathf.Lerp(0.42f, 0.08f, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);
            onHalfAwake?.Invoke();

            // 3. Second open, wider, taking in the room
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 1.3f;
                openAmount = Mathf.Lerp(0.08f, 0.82f, Mathf.SmoothStep(0, 1, t));
                drowsiness = Mathf.Lerp(1.0f, 0.25f, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);

            // 4. Quick natural clarifying blink
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.3f;
                openAmount = Mathf.Lerp(0.82f, 0.40f, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.45f;
                openAmount = Mathf.Lerp(0.40f, 1.0f, Mathf.SmoothStep(0, 1, t));
                drowsiness = Mathf.Lerp(0.25f, 0f, t);
                yield return null;
            }

            openAmount = 1.0f;
            drowsiness = 0f;
            onFullyAwake?.Invoke();
        }

        void OnGUI()
        {
            if (openAmount >= 0.995f) return;

            float w = Screen.width;
            float h = Screen.height;
            float halfH = h * 0.5f;

            // When openAmount == 0: eyelids completely closed (solid black)
            // When openAmount == 1: eyelids completely open
            float closedH = halfH * Mathf.Clamp01(1.0f - openAmount);

            GUI.color = Color.black;
            // Upper eyelid
            GUI.DrawTexture(new Rect(0, 0, w, closedH), Texture2D.whiteTexture);
            // Lower eyelid
            GUI.DrawTexture(new Rect(0, h - closedH, w, closedH), Texture2D.whiteTexture);

            // Feathered soft lash gradient band
            float lashFeather = Mathf.Min(24f, halfH * 0.15f);
            if (closedH > 0 && closedH < halfH)
            {
                GUI.color = new Color(0, 0, 0, 0.65f);
                GUI.DrawTexture(new Rect(0, closedH, w, lashFeather), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, h - closedH - lashFeather, w, lashFeather), Texture2D.whiteTexture);
            }

            // Drowsiness edge vignette
            if (drowsiness > 0.05f)
            {
                GUI.color = new Color(0, 0, 0, drowsiness * 0.4f);
                float vigW = w * 0.08f * drowsiness;
                GUI.DrawTexture(new Rect(0, closedH, vigW, h - closedH * 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(w - vigW, closedH, vigW, h - closedH * 2), Texture2D.whiteTexture);
            }

            GUI.color = Color.white;
        }
    }
}
