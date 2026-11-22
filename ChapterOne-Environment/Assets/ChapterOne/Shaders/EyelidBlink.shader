Shader "ThatNight/EyelidBlink"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Open ("Open Amount", Range(0, 1)) = 1.0
        _Drowsiness ("Drowsiness Blur", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay+100" "RenderType"="Transparent" }
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Open;
            float _Drowsiness;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;

                // When fully open, render nothing (transparent)
                if (_Open >= 0.999) return fixed4(0, 0, 0, 0);

                // Realistic human eye aperture curve (arch highest in center, narrower at corners)
                float arch = sin(saturate(uv.x) * 3.14159265);
                float curvature = (1.0 - _Open) * 0.16 * arch;

                // Center of eye vertical opening (0.5)
                float topEyelid = 0.5 + (_Open * 0.54) + curvature;
                float bottomEyelid = 0.5 - (_Open * 0.54) - curvature;

                // Soft feathered shadow at the lash line
                float feather = 0.045 + (1.0 - _Open) * 0.08;

                float alpha = 0.0;
                if (uv.y > topEyelid - feather)
                {
                    alpha = max(alpha, smoothstep(topEyelid - feather, topEyelid, uv.y));
                }
                if (uv.y < bottomEyelid + feather)
                {
                    alpha = max(alpha, smoothstep(bottomEyelid + feather, bottomEyelid, uv.y));
                }

                // Add subtle vignetting around the edges when drowsy
                float2 dist = (uv - 0.5) * 2.0;
                float vig = dot(dist, dist) * _Drowsiness * 0.4;
                alpha = saturate(alpha + vig);

                return fixed4(0.015, 0.01, 0.01, alpha);
            }
            ENDCG
        }
    }
}
