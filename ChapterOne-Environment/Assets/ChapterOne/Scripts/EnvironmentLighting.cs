using UnityEngine;
using UnityEngine.Rendering;

namespace ThatNight.ChapterOne
{
    public sealed class EnvironmentLighting:MonoBehaviour
    {
        public Light key;public Light[] practicals;public Material sky;public bool daylight;
        void Start(){Apply(daylight);}
        public void Apply(bool day)
        {
            daylight=day;RenderSettings.skybox=sky;sky.SetFloat("_Day",day?1:0);
            RenderSettings.ambientMode=AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=day?new Color(.43f,.49f,.55f):new Color(.20f,.24f,.32f);
            RenderSettings.ambientEquatorColor=day?new Color(.3f,.32f,.3f):new Color(.14f,.15f,.17f);
            RenderSettings.ambientGroundColor=day?new Color(.15f,.14f,.11f):new Color(.09f,.075f,.06f);
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Exponential;RenderSettings.fogDensity=day?.009f:.014f;
            RenderSettings.fogColor=day?new Color(.42f,.48f,.49f):new Color(.025f,.035f,.045f);
            key.color=day?new Color(1,.9f,.75f):new Color(.49f,.62f,.85f);key.intensity=day?1.1f:.45f;
            key.transform.rotation=Quaternion.Euler(day?48:38,day?-35:-130,0);RenderSettings.sun=key;
            foreach(var lamp in practicals)if(lamp)lamp.enabled=true;
            RenderSettings.reflectionIntensity=day?.5f:.2f;
        }
    }
}
