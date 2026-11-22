Shader "ThatNight/Invisible"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest-50" "RenderType"="None" "IgnoreProjector"="True" }
        Pass
        {
            ZWrite Off
            ColorMask 0
            Cull Back
        }
    }
}
