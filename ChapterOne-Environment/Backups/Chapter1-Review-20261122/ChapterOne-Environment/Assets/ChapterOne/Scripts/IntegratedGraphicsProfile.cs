using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne
{
    [DefaultExecutionOrder(100)]
    public sealed class IntegratedGraphicsProfile : MonoBehaviour
    {
        public bool lowPower=true;
        Camera view;
        EnvironmentLighting lighting;
        ForestDetailCell[] details;
        ForestFoliageCell[] foliage;
        AtmosphereStage previous;
        void Start()
        {
            view=GetComponent<Camera>();lighting=GetComponent<EnvironmentCamera>().lighting;
            details=FindObjectsByType<ForestDetailCell>(FindObjectsSortMode.None);
            foliage=FindObjectsByType<ForestFoliageCell>(FindObjectsSortMode.None);
            Apply();
        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F6)){lowPower=!lowPower;Apply();}
            if(lighting&&previous!=lighting.currentStage)ApplyDistances();
        }
        public void Apply()
        {
            QualitySettings.vSyncCount=0;Application.targetFrameRate=30;
            QualitySettings.antiAliasing=0;QualitySettings.pixelLightCount=lowPower?2:3;
            QualitySettings.shadows=lowPower?ShadowQuality.HardOnly:ShadowQuality.All;
            QualitySettings.shadowResolution=lowPower?ShadowResolution.Low:ShadowResolution.Medium;
            QualitySettings.shadowCascades=lowPower?0:2;QualitySettings.shadowDistance=lowPower?18:34;
            QualitySettings.lodBias=lowPower?.6f:.85f;
            QualitySettings.realtimeReflectionProbes=false;QualitySettings.streamingMipmapsActive=true;
            view.allowHDR=false;view.allowMSAA=false;
            if(!Application.isEditor)Screen.SetResolution(lowPower?960:1280,lowPower?540:720,FullScreenMode.Windowed);
            var light=GetComponent<ForestExploration>().flashlight;
            if(light){light.shadows=lowPower?LightShadows.Hard:LightShadows.Soft;light.shadowResolution=lowPower?LightShadowResolution.Low:LightShadowResolution.Medium;}
            ApplyDistances();
        }
        void ApplyDistances()
        {
            previous=lighting.currentStage;bool night=previous==AtmosphereStage.NightHorror;
            var distances=view.layerCullDistances;
            distances[EnvironmentPerformance.DistantEnvironmentLayer]=night?95:lowPower?360:700;
            view.layerCullDistances=distances;view.layerCullSpherical=true;
            // Thick night fog masks distant geometry. Day retains the authored skyline.
            view.farClipPlane=night?125:lowPower?900:2200;
            foreach(var cell in details)if(cell)cell.visibleDistance=night?110:lowPower?145:205;
            foreach(var cell in foliage)if(cell)cell.distance=night?45:lowPower?80:180;
        }
    }
}
