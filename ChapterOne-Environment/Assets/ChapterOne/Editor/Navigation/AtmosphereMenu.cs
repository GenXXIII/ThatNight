using UnityEditor;
using UnityEngine;

namespace ThatNight.ChapterOne.Editor
{
    public static class AtmosphereMenu
    {
        [MenuItem("THAT NIGHT/Atmosphere/1. 4-00 AM Dawn (Scene 1 Quiet)")]
        public static void SetAtmosphereDawn()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.Dawn4AM); Debug.Log("ATMOSPHERE: 4:00 AM Dawn (Quiet)"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/2. Morning Peaceful (Scene 2-4 Preparations)")]
        public static void SetAtmosphereMorning()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.MorningPeaceful); Debug.Log("ATMOSPHERE: Morning Peaceful"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/3. Afternoon Celebration (Scene 5 Ceremony)")]
        public static void SetAtmosphereAfternoon()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.AfternoonCelebration); Debug.Log("ATMOSPHERE: Afternoon Celebration"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/4. Evening Warm (Scene 6 Celebration)")]
        public static void SetAtmosphereEvening()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.EveningWarm); Debug.Log("ATMOSPHERE: Evening Warm"); }
        }

        [MenuItem("THAT NIGHT/Atmosphere/5. Night Horror (Scene 7-9 Climax)")]
        public static void SetAtmosphereNightHorror()
        {
            var lighting = UnityEngine.Object.FindAnyObjectByType<EnvironmentLighting>();
            if (lighting != null) { lighting.ApplyStage(AtmosphereStage.NightHorror); Debug.Log("ATMOSPHERE: Night Horror"); }
        }
    }
}

