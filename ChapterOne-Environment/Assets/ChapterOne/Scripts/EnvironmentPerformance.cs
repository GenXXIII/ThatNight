using UnityEngine;

namespace ThatNight.ChapterOne
{
    /// <summary>
    /// Runtime performance policy for the generated environment.
    /// Keeps visual layout decisions out of the camera controller.
    /// </summary>
    public static class EnvironmentPerformance
    {
        public const int DistantEnvironmentLayer = 8;
        public const float DistantEnvironmentCullDistance = 700f;

        public static void Apply(Camera camera)
        {
            if (camera == null) return;

            var layerCullDistances = camera.layerCullDistances;
            layerCullDistances[DistantEnvironmentLayer] = DistantEnvironmentCullDistance;
            camera.layerCullDistances = layerCullDistances;
            camera.layerCullSpherical = true;
            camera.useOcclusionCulling = true;

            // One directional light and the two intentionally shadowed practicals
            // remain pixel lights. Other practicals are rendered per vertex.
            QualitySettings.pixelLightCount = 3;
            QualitySettings.shadowDistance = 34f;
            QualitySettings.shadowCascades = 2;
            QualitySettings.lodBias = 0.85f;
            QualitySettings.streamingMipmapsActive = true;
        }
    }
}
