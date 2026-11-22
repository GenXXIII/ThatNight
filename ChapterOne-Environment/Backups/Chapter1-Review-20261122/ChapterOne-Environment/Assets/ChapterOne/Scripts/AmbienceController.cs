// ============================================================================
// THAT NIGHT — Chapter 1: The Wedding
// AmbienceController.cs — Environmental Audio Manager
//
// Manages ambient soundscapes for each phase of Chapter 1:
// Dawn, Morning, Ceremony, Reception, Horror.
// 2006 rural Cambodia — Battambang countryside.
// ============================================================================

using UnityEngine;

namespace ThatNight.ChapterOne
{
    public enum AmbienceScene
    {
        Silent,
        Dawn,       // 4:00 AM — crickets, distant rooster, pre-dawn stillness
        Morning,    // Early morning — birds, light breeze, household sounds
        Ceremony,   // Afternoon — Phleng Kar music, crowd murmur, celebration
        Reception,  // Evening — festive ambience, music, talking, laughter
        Horror      // Night — silence, distant insects, unsettling atmosphere
    }

    public sealed class AmbienceController : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource baseLayer;    // Primary ambient loop
        public AudioSource musicLayer;   // Music or secondary loop

        [Header("Clips")]
        public AudioClip dawnAmbience;
        public AudioClip morningBirds;
        public AudioClip ceremonyMusic;
        public AudioClip receptionMusic;
        public AudioClip horrorAmbience;

        AmbienceScene current = AmbienceScene.Silent;
        float targetBaseVolume;
        float targetMusicVolume;

        void Start()
        {
            if (!baseLayer)
            {
                var go1 = new GameObject("Ambience Base").transform;
                go1.SetParent(transform, false);
                baseLayer = go1.gameObject.AddComponent<AudioSource>();
                baseLayer.loop = true;
                baseLayer.spatialBlend = 0;
                baseLayer.playOnAwake = false;
            }

            if (!musicLayer)
            {
                var go2 = new GameObject("Ambience Music").transform;
                go2.SetParent(transform, false);
                musicLayer = go2.gameObject.AddComponent<AudioSource>();
                musicLayer.loop = true;
                musicLayer.spatialBlend = 0;
                musicLayer.playOnAwake = false;
            }
        }

        void Update()
        {
            // Smooth volume transitions
            if (baseLayer)
                baseLayer.volume = Mathf.MoveTowards(baseLayer.volume, targetBaseVolume, Time.deltaTime * 0.5f);
            if (musicLayer)
                musicLayer.volume = Mathf.MoveTowards(musicLayer.volume, targetMusicVolume, Time.deltaTime * 0.5f);
        }

        public void SetScene(AmbienceScene scene)
        {
            if (scene == current) return;
            current = scene;

            switch (scene)
            {
                case AmbienceScene.Silent:
                    targetBaseVolume = 0;
                    targetMusicVolume = 0;
                    break;

                case AmbienceScene.Dawn:
                    PlayBase(dawnAmbience, 0.15f);
                    StopMusic();
                    break;

                case AmbienceScene.Morning:
                    PlayBase(morningBirds, 0.2f);
                    StopMusic();
                    break;

                case AmbienceScene.Ceremony:
                    PlayBase(null, 0);
                    PlayMusic(ceremonyMusic, 0.25f);
                    break;

                case AmbienceScene.Reception:
                    PlayBase(null, 0);
                    PlayMusic(receptionMusic, 0.3f);
                    break;

                case AmbienceScene.Horror:
                    PlayBase(horrorAmbience, 0.12f);
                    StopMusic();
                    break;
            }
        }

        void PlayBase(AudioClip clip, float volume)
        {
            if (!baseLayer) return;
            if (clip)
            {
                if (baseLayer.clip != clip) { baseLayer.clip = clip; baseLayer.Play(); }
                targetBaseVolume = volume;
            }
            else
            {
                targetBaseVolume = 0;
            }
        }

        void PlayMusic(AudioClip clip, float volume)
        {
            if (!musicLayer) return;
            if (clip)
            {
                if (musicLayer.clip != clip) { musicLayer.clip = clip; musicLayer.Play(); }
                targetMusicVolume = volume;
            }
            else
            {
                StopMusic();
            }
        }

        void StopMusic()
        {
            targetMusicVolume = 0;
        }
    }
}
