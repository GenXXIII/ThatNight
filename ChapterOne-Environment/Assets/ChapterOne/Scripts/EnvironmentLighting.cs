using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne
{
    public enum AtmosphereStage
    {
        Dawn4AM,              // Scene 01: 4:00 AM — Quiet pre-dawn
        MorningPeaceful,      // Scene 02-04: Morning — Peaceful daylight & preparations
        AfternoonCelebration, // Scene 05: Afternoon — Wedding ceremony & feast
        EveningWarm,          // Scene 06: Evening — Warm string lights & celebration
        NightHorror           // Scene 07-09: Night — Disappearance & supernatural horror
    }

    public sealed class EnvironmentLighting : MonoBehaviour
    {
        public Light key;
        public Light[] practicals;
        public Material sky;
        public bool daylight;
        public AtmosphereStage currentStage = AtmosphereStage.NightHorror;
        [Header("Forest night visibility")]
        [Tooltip("Exponential fog: .055 leaves about 25% visibility at 25 metres and 6% at 50 metres.")]
        [Range(.02f,.09f)] public float nightFogDensity = .055f;
        [Range(0,.5f)] public float nightMoonIntensity = .18f;

        void Start()
        {
            ApplyStage(currentStage);
        }

        public void Apply(bool day)
        {
            ApplyStage(day ? AtmosphereStage.AfternoonCelebration : AtmosphereStage.EveningWarm);
        }

        public void ApplyStage(AtmosphereStage stage)
        {
            currentStage = stage;
            daylight = (stage == AtmosphereStage.MorningPeaceful || stage == AtmosphereStage.AfternoonCelebration);

            if (sky != null)
            {
                RenderSettings.skybox = sky;
                float dayParam = stage switch
                {
                    AtmosphereStage.Dawn4AM => 0.15f,
                    AtmosphereStage.MorningPeaceful => 0.85f,
                    AtmosphereStage.AfternoonCelebration => 1.0f,
                    AtmosphereStage.EveningWarm => 0.35f,
                    AtmosphereStage.NightHorror => 0.0f,
                    _ => 0.5f
                };
                sky.SetFloat("_Day", dayParam);
            }

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;

            switch (stage)
            {
                case AtmosphereStage.Dawn4AM:
                    // 4:00 AM: Deep indigo pre-dawn, quiet village ambience
                    RenderSettings.ambientSkyColor = new Color(.10f, .14f, .22f);
                    RenderSettings.ambientEquatorColor = new Color(.07f, .09f, .13f);
                    RenderSettings.ambientGroundColor = new Color(.04f, .04f, .05f);
                    RenderSettings.fogColor = new Color(.03f, .045f, .07f);
                    RenderSettings.fogDensity = .016f;
                    RenderSettings.reflectionIntensity = .15f;

                    if (key != null)
                    {
                        key.color = new Color(.35f, .45f, .65f);
                        key.intensity = .25f;
                        key.transform.rotation = Quaternion.Euler(16, -145, 0);
                        key.shadowStrength = .4f;
                    }
                    SetPracticals(true, 0.45f);
                    break;

                case AtmosphereStage.MorningPeaceful:
                    // Morning: Fresh early sunlight, soft golden rays, clear preparations
                    RenderSettings.ambientSkyColor = new Color(.42f, .48f, .55f);
                    RenderSettings.ambientEquatorColor = new Color(.30f, .32f, .30f);
                    RenderSettings.ambientGroundColor = new Color(.15f, .14f, .11f);
                    RenderSettings.fogColor = new Color(.48f, .52f, .55f);
                    RenderSettings.fogDensity = .008f;
                    RenderSettings.reflectionIntensity = .45f;

                    if (key != null)
                    {
                        key.color = new Color(1.0f, .92f, .78f);
                        key.intensity = 1.05f;
                        key.transform.rotation = Quaternion.Euler(32, -45, 0);
                        key.shadowStrength = .65f;
                    }
                    SetPracticals(false, 0f);
                    break;

                case AtmosphereStage.AfternoonCelebration:
                    // Afternoon: High tropical sun, vibrant wedding colors, celebration
                    RenderSettings.ambientSkyColor = new Color(.48f, .54f, .60f);
                    RenderSettings.ambientEquatorColor = new Color(.35f, .36f, .32f);
                    RenderSettings.ambientGroundColor = new Color(.18f, .16f, .12f);
                    RenderSettings.fogColor = new Color(.52f, .56f, .58f);
                    RenderSettings.fogDensity = .005f;
                    RenderSettings.reflectionIntensity = .55f;

                    if (key != null)
                    {
                        key.color = new Color(1.0f, .97f, .86f);
                        key.intensity = 1.30f;
                        key.transform.rotation = Quaternion.Euler(58, -30, 0);
                        key.shadowStrength = .70f;
                    }
                    SetPracticals(false, 0f);
                    break;

                case AtmosphereStage.EveningWarm:
                    // Evening: Golden-violet twilight, warm tungsten bulbs illuminating marquee
                    RenderSettings.ambientSkyColor = new Color(.24f, .22f, .32f);
                    RenderSettings.ambientEquatorColor = new Color(.18f, .16f, .19f);
                    RenderSettings.ambientGroundColor = new Color(.10f, .08f, .07f);
                    RenderSettings.fogColor = new Color(.14f, .10f, .14f);
                    RenderSettings.fogDensity = .011f;
                    RenderSettings.reflectionIntensity = .25f;

                    if (key != null)
                    {
                        key.color = new Color(1.0f, .65f, .40f);
                        key.intensity = .55f;
                        key.transform.rotation = Quaternion.Euler(14, -80, 0);
                        key.shadowStrength = .60f;
                    }
                    SetPracticals(true, 1.0f);
                    break;

                case AtmosphereStage.NightHorror:
                    // Blair Witch & The Forest Horror: Silvery-blue moonlight, deep contrast, atmospheric fog
                    RenderSettings.ambientSkyColor = new Color(.055f, .085f, .09f);
                    RenderSettings.ambientEquatorColor = new Color(.025f, .04f, .035f);
                    RenderSettings.ambientGroundColor = new Color(.012f, .018f, .014f);
                    RenderSettings.fogColor = new Color(.018f, .028f, .03f);
                    RenderSettings.fogDensity = nightFogDensity;
                    RenderSettings.reflectionIntensity = .06f;

                    if (key != null)
                    {
                        key.color = new Color(.58f, .81f, .86f);
                        key.intensity = nightMoonIntensity;
                        key.transform.rotation = Quaternion.Euler(32, -48, 0);
                        key.shadowStrength = .82f;
                    }
                    SetPracticals(true, 0.40f);
                    break;
            }

            if (key != null) RenderSettings.sun = key;
        }

        private void SetPracticals(bool enabled, float intensityMultiplier)
        {
            if (practicals == null) return;
            foreach (var lamp in practicals)
            {
                if (lamp == null) continue;
                lamp.enabled = enabled;
                if (enabled) lamp.intensity = Mathf.Max(0.5f, lamp.range * 0.4f * intensityMultiplier);
            }
        }
    }
}
