// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// ChapterOneNarrative.cs — Dialogue System & Subtitle Controller
//
// Contains all authored dialogue for Chapter 1 and handles subtitle display
// with speaker identification, color-coded names, and timed progression.
// ============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThatNight.ChapterOne
{
    // ── Speaker identification ─────────────────────────────────────────────
    public enum DialogueSpeaker
    {
        Bride,
        Mother,
        Brother,
        Groom,
        Guest
    }

    // ── Single dialogue line ───────────────────────────────────────────────
    [Serializable]
    public struct DialogueLine
    {
        public DialogueSpeaker speaker;
        public string speakerName;
        public string text;
        public float duration;
        public AudioClip audioClip;

        public DialogueLine(DialogueSpeaker speaker, string name, string text,
                            float duration = 3.5f, AudioClip clip = null)
        {
            this.speaker = speaker;
            this.speakerName = name;
            this.text = text;
            this.duration = duration;
            this.audioClip = clip;
        }
    }

    // ── Narrative controller ───────────────────────────────────────────────
    public sealed class ChapterOneNarrative : MonoBehaviour
    {
        public static ChapterOneNarrative Instance { get; private set; }

        // ── State ──────────────────────────────────────────────────────────
        public bool ShowSubtitles { get; set; } = true;
        public DialogueLine CurrentLine { get; private set; }
        public bool IsPlaying { get; private set; }

        Coroutine activeSequence;
        float lineAlpha; // For fade in/out of subtitles
        readonly Queue<(List<DialogueLine> lines, Action complete)> pending = new();
        AudioSource voice;
        Transform motherSpeaker, groomSpeaker;
        bool advanceLine;
        bool skipAll;
        int playbackGeneration;
        float spaceHoldTimer;
        const float SpaceHoldToSkipAll = 2.0f;

        void Update()
        {
            if (IsPlaying)
            {
                // Clicking Space does NOT skip: only holding Space skips the sequence!
                if (Input.GetKey(KeyCode.Space))
                {
                    spaceHoldTimer += Time.deltaTime;
                    if (spaceHoldTimer >= SpaceHoldToSkipAll)
                    {
                        skipAll = true;
                        spaceHoldTimer = 0f;
                    }
                }
                else
                {
                    spaceHoldTimer = 0f;
                }
            }
            else
            {
                spaceHoldTimer = 0f;
            }
        }

        void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) { Destroy(this); return; }
        }

        // ================================================================
        // PLAY DIALOGUE
        // ================================================================

        /// <summary>Play a sequence and invoke onComplete when finished.</summary>
        public void PlaySequence(List<DialogueLine> lines, Action onComplete = null)
        {
            if (lines == null || lines.Count == 0) { onComplete?.Invoke(); return; }
            pending.Enqueue((new List<DialogueLine>(lines), onComplete));
            if (IsPlaying) return;
            IsPlaying = true;
            activeSequence = StartCoroutine(RunQueue());
        }

        /// <summary>Play a sequence as a coroutine you can yield on.</summary>
        public IEnumerator PlayAndWait(List<DialogueLine> lines)
        {
            bool done = false;
            int generation = playbackGeneration;
            PlaySequence(lines, () => done = true);
            yield return new WaitUntil(() => done || generation != playbackGeneration);
        }

        IEnumerator RunQueue()
        {
            while (pending.Count > 0)
            {
                var request = pending.Dequeue();
                yield return RunSequence(request.lines, request.complete);
            }
            IsPlaying = false;
            activeSequence = null;
        }

        void PlayVoice(DialogueLine line)
        {
            if (!voice)
            {
                var source = new GameObject("Dialogue voice");
                source.transform.SetParent(transform, false);
                voice = source.AddComponent<AudioSource>();
                voice.playOnAwake = false;
                voice.minDistance = 1f;
                voice.maxDistance = 14f;
                voice.rolloffMode = AudioRolloffMode.Linear;
            }
            voice.Stop();
            if (!motherSpeaker)
            {
                var actor = FindFirstObjectByType<MotherNPC>();
                if (actor) motherSpeaker = actor.head ? actor.head : actor.transform;
            }
            if (!groomSpeaker)
            {
                var actor = FindFirstObjectByType<GroomNPC>();
                if (actor) groomSpeaker = actor.head ? actor.head : actor.transform;
            }
            Transform speaker = line.speaker == DialogueSpeaker.Mother ? motherSpeaker :
                line.speaker == DialogueSpeaker.Groom ? groomSpeaker : null;
            voice.spatialBlend = speaker ? 1f : 0f;
            voice.transform.SetParent(speaker ? speaker : transform, false);
            voice.transform.localPosition = Vector3.zero;
            voice.clip = line.audioClip;
            if (voice.clip) voice.Play();
        }

        void OnDisable()
        {
            StopAllCoroutines();
            pending.Clear();
            if (voice) voice.Stop();
            activeSequence = null;
            CurrentLine = default;
            IsPlaying = false;
            lineAlpha = 0;
            playbackGeneration++;
        }

        IEnumerator RunSequence(List<DialogueLine> lines, Action onComplete)
        {
            IsPlaying = true;
            skipAll = false;

            // Lock walking movement during dialogue so player cannot wander off
            var explorer = Camera.main ? Camera.main.GetComponent<ForestExploration>() : null;
            if (explorer) explorer.MovementLocked = true;

            try
            {
                foreach (var line in lines)
                {
                    if (skipAll) break;

                    CurrentLine = line;
                    advanceLine = false;
                    PlayVoice(line);

                    // Fade in subtitle
                    lineAlpha = 0;
                    float fadeIn = 0;
                    while (fadeIn < 1f && !skipAll && !advanceLine)
                    {
                        fadeIn += Time.deltaTime / 0.15f;
                        lineAlpha = Mathf.Clamp01(fadeIn);
                        yield return null;
                    }
                    lineAlpha = 1f;

                    // Hold for duration minus fade times
                    float readingTime = Mathf.Max(line.duration, (line.text?.Length ?? 0) / 17f + 0.5f);
                    float duration = line.audioClip ? Mathf.Max(readingTime, line.audioClip.length) : readingTime;
                    float hold = Mathf.Max(0.1f, duration - 0.3f);
                    for (float elapsed = 0; elapsed < hold; elapsed += Time.deltaTime)
                    {
                        if (skipAll) break;
                        if (advanceLine && elapsed >= 0.12f) break;
                        yield return null;
                    }

                    // Fade out subtitle
                    float fadeOut = 0;
                    while (fadeOut < 1f && !skipAll)
                    {
                        fadeOut += Time.deltaTime / 0.15f;
                        lineAlpha = 1f - Mathf.Clamp01(fadeOut);
                        yield return null;
                    }
                    lineAlpha = 0;
                    if (voice) voice.Stop();
                }
            }
            finally
            {
                if (voice) voice.Stop();
                lineAlpha = 0;
                CurrentLine = default;
                bool phoneHeld = (ChapterOneDirector.Instance != null && ChapterOneDirector.Instance.nokia != null && ChapterOneDirector.Instance.nokia.isHeld);
                if (explorer && !phoneHeld) explorer.MovementLocked = false;
                onComplete?.Invoke();
            }
        }

        // ================================================================
        // SUBTITLE RENDERING
        // ================================================================

        void OnGUI()
        {
            if (!ShowSubtitles || !IsPlaying || string.IsNullOrEmpty(CurrentLine.text))
                return;

            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            var prev = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);

            float sw = Screen.width / scale;
            float sh = Screen.height / scale;
            float boxW = 850, boxH = 72;
            float x = (sw - boxW) * 0.5f;
            float y = sh - 115;

            // Dialogue banner: ONLY displays chat (Speaker and spoken line), NO instrument controls
            float alpha = lineAlpha;

            // Background bar
            GUI.color = new Color(0, 0, 0, 0.65f * alpha);
            GUI.DrawTexture(new Rect(x - 20, y - 12, boxW + 40, boxH + 24), Texture2D.whiteTexture);

            // Speaker name (color-coded)
            Color speakerColor = GetSpeakerColor(CurrentLine.speaker);
            speakerColor.a = alpha;

            var nameStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            nameStyle.normal.textColor = speakerColor;

            GUI.color = Color.white;
            GUI.Label(new Rect(x, y, boxW, 24), CurrentLine.speakerName.ToUpperInvariant(), nameStyle);

            // Dialogue text
            var textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            textStyle.normal.textColor = new Color(0.95f, 0.94f, 0.88f, alpha);
            GUI.Label(new Rect(x, y + 26, boxW, 46), $"\"{CurrentLine.text}\"", textStyle);

            // Circular Hold Space Skip indicator in bottom-right corner
            float progress = Mathf.Clamp01(spaceHoldTimer / SpaceHoldToSkipAll);
            CircleSkipUI.Draw(sw - 200f, sh - 52f, progress, "Hold Space to Skip");

            GUI.matrix = prev;
            GUI.color = Color.white;
        }

        static Color GetSpeakerColor(DialogueSpeaker speaker) => speaker switch
        {
            DialogueSpeaker.Bride        => new Color(0.92f, 0.82f, 0.45f),  // Gold
            DialogueSpeaker.Mother  => new Color(0.95f, 0.70f, 0.65f),  // Warm rose
            DialogueSpeaker.Brother => new Color(0.55f, 0.78f, 0.95f),  // Soft sky blue
            DialogueSpeaker.Groom   => new Color(0.65f, 0.90f, 0.70f),  // Soft green
            DialogueSpeaker.Guest        => new Color(0.85f, 0.85f, 0.85f),  // Light grey
            _                            => Color.white
        };

        // ================================================================
        // ALL CHAPTER 1 DIALOGUE DATA
        // ================================================================

        public static List<DialogueLine> Scene01_WakingDialogue() => new()
        {
            new(DialogueSpeaker.Bride, "Bride", "The phone... already?", 2.5f),
            new(DialogueSpeaker.Bride, "Bride", "Brother?", 2.0f)
        };

        public static List<DialogueLine> Scene02_MorningLines() => new()
        {
            new(DialogueSpeaker.Mother, "Mother", "You're awake already?", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Yes, Mom. I'm coming.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "Today is your big day. Don't stay in bed too long.", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "I know, Mom.", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Come downstairs when you're ready. We still have so much to do.", 4.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Okay!", 2.0f),
        };

        public static List<DialogueLine> Scene03_LivingAreaLines() => new()
        {
            new(DialogueSpeaker.Mother, "Mother", "There you are, my daughter... Look at you on your wedding morning.", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Mom, why are you crying? Please don't be sad.", 3.0f),
            new(DialogueSpeaker.Mother, "Mother", "I'm not sad, sweetheart... Just, time passed so fast. It feels like yesterday you were just a little girl running through the yard.", 4.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "I will always be your little girl, Mom. I'm not leaving you. I will come home and visit every week.", 4.0f),
            new(DialogueSpeaker.Mother, "Mother", "You have your own family now, Bopha. But hearing you say that makes my heart so warm.", 4.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Smile for me, Mom. Today is our happiest celebration. Don't be sad.", 3.5f),
            new(DialogueSpeaker.Mother, "Mother", "You're right, my sweet child. Today is for smiles and joy. Go to the kitchen, wash your face with cool water to wake up, and then come to the mirror table so we can prepare your makeup.", 5.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Okay, Mom. I love you so much.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "I love you too, my daughter. I'll head downstairs to welcome the wedding guests.", 3.5f),
        };

        public static List<DialogueLine> Scene04_PhoneCallLines() => new()
        {
            new(DialogueSpeaker.Bride,   "Bopha",    "Hello?", 2.0f),
            new(DialogueSpeaker.Brother, "Brother",  "Hey, little sister. Are you awake?", 2.5f),
            new(DialogueSpeaker.Bride,   "Bopha",    "Brother! I've been waiting for your call! Where are you?", 3.0f),
            new(DialogueSpeaker.Brother, "Brother",  "Happy wedding day, Bopha.", 2.5f),
            new(DialogueSpeaker.Bride,   "Bopha",    "That's all? You aren't even here yet! You promised you'd be here before the ceremony!", 3.5f),
            new(DialogueSpeaker.Brother, "Brother",  "I'm sorry, Bopha... I wanted to be there. More than anything.", 3.0f),
            new(DialogueSpeaker.Bride,   "Bopha",    "Then why aren't you here? Mom and everyone have been asking for you.", 3.5f),
            new(DialogueSpeaker.Brother, "Brother",  "Something came up with my work in the city. I can't leave right now, but I promise I'll come see you as soon as I can.", 4.5f),
            new(DialogueSpeaker.Bride,   "Bopha",    "You always have something urgent. It doesn't feel right having my wedding without my big brother.", 4.0f),
            new(DialogueSpeaker.Brother, "Brother",  "I know I keep disappointing you. Don't let me ruin your day. Smile, okay? You're going to be a beautiful bride.", 4.5f),
            new(DialogueSpeaker.Bride,   "Bopha",    "I wish you were here, brother.", 2.5f),
            new(DialogueSpeaker.Brother, "Brother",  "I wish I was too. Take care of yourself, little sister... and congratulations.", 3.5f),
        };

        public static List<DialogueLine> Scene05_PreparationLines() => new()
        {
            new(DialogueSpeaker.Mother, "Mother", "Come here, sweetheart. Let's get you ready.", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Mom, is my dress ready?", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "Everything is ready. I checked it twice.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "You're more nervous than I am.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "Of course I am. My daughter is getting married today.", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "You're making it sound like I'm leaving forever.", 3.5f),
            new(DialogueSpeaker.Mother, "Mother", "You're starting a new life. That's different.", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "I'll still come home and visit you.", 3.0f),
            new(DialogueSpeaker.Mother, "Mother", "You better. I don't want you forgetting your mother.", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Never.", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Look at you. My little girl is growing up.", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Mom, don't cry.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "I'm not crying.", 2.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "You are!", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "I'm just happy, sweetheart.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "I love you, Mom.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "I love you too.", 2.5f),
        };

        public static List<DialogueLine> Scene06_CeremonyLines() => new()
        {
            new(DialogueSpeaker.Mother, "Mother", "Are you ready?", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "I think so.", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Don't be nervous. Just smile.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "You're staying with me, right?", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "Of course. I'm right here.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Thank you, Mom.", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Now go. Everyone is waiting.", 3.0f),
            new(DialogueSpeaker.Groom,  "Groom",  "You look beautiful.", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Thank you.", 2.0f),
            new(DialogueSpeaker.Groom,  "Groom",  "Are you nervous?", 2.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "A little. What about you?", 2.5f),
            new(DialogueSpeaker.Groom,  "Groom",  "Honestly? Very.", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "I thought you would be confident.", 2.5f),
            new(DialogueSpeaker.Groom,  "Groom",  "Not when I'm standing in front of you.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Haha. You're silly.", 2.0f),
            new(DialogueSpeaker.Groom,  "Groom",  "But I'm happy.", 2.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Me too.", 2.0f),
        };

        public static List<DialogueLine> Scene07_ReceptionLines() => new()
        {
            new(DialogueSpeaker.Mother, "Mother", "Are you enjoying yourself?", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Yes, Mom. It's been wonderful.", 3.0f),
            new(DialogueSpeaker.Mother, "Mother", "I'm glad.", 2.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "I wish everyone could be here.", 3.0f),
            new(DialogueSpeaker.Mother, "Mother", "I know who you're thinking about.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "He really wanted to come.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "I'm sure he did. Don't be sad today.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "I know.", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Today is your day. Enjoy every moment.", 3.5f),
            new(DialogueSpeaker.Groom,  "Groom",  "You've been smiling all evening.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "It's my wedding. I'm allowed to.", 3.0f),
            new(DialogueSpeaker.Groom,  "Groom",  "You look tired, though.", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "A little.", 2.0f),
            new(DialogueSpeaker.Groom,  "Groom",  "It's been a long day. Shall we go inside?", 3.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Okay. Just let me say goodbye to Mom.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Mom, we're going inside now.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "Alright, sweetheart. Get some rest.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "You should rest too.", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "I will. Don't worry about me.", 3.0f),
            new(DialogueSpeaker.Bride,  "Bride",  "Goodnight, Mom.", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Goodnight, my daughter.", 2.5f),
        };

        public static List<DialogueLine> Scene08_DisappearanceLines() => new()
        {
            // Phase 1: Quiet moment
            new(DialogueSpeaker.Groom,  "Groom",  "Finally, some quiet.", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "It's been a long day.", 2.5f),
            new(DialogueSpeaker.Groom,  "Groom",  "A good one, though.", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "The best.", 2.0f),
            // Phase 2: Disturbance
            new(DialogueSpeaker.Groom,  "Groom",  "Did you hear that?", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Hear what?", 2.0f),
            new(DialogueSpeaker.Groom,  "Groom",  "I thought someone was outside.", 2.5f),
            new(DialogueSpeaker.Bride,  "Bride",  "Maybe it's just the guests.", 2.5f),
            new(DialogueSpeaker.Groom,  "Groom",  "Wait...", 1.5f),
            // Phase 4: Guest reactions after blackout
            new(DialogueSpeaker.Guest,  "Guest",  "What was that?!", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "That came from their room!", 2.5f),
            new(DialogueSpeaker.Guest,  "Guest",  "Open the door!", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Where is my daughter?!", 2.5f),
            new(DialogueSpeaker.Mother, "Mother", "Sweetheart?!", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "Please answer me!", 2.5f),
            new(DialogueSpeaker.Guest,  "Guest",  "They're not here!", 2.0f),
            new(DialogueSpeaker.Mother, "Mother", "No... no, this can't be happening...", 3.5f),
        };
    }
}
