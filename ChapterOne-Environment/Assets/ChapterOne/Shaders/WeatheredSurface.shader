Shader "ThatNight/WeatheredSurface" {
 Properties {
  _Color("Tint",Color)=(1,1,1,1)
  _MainTex("Color",2D)="white"{}
  _BumpMap("Normal",2D)="bump"{}
  _BumpScale("Normal strength",Float)=0.65
  _OcclusionMap("Occlusion",2D)="white"{}
  _Glossiness("Smoothness",Range(0,1))=0.15
  _Metallic("Metalness",Range(0,1))=0
  _Desaturate("Base map desaturation",Range(0,1))=0
 }
 SubShader {
  Tags {"RenderType"="Opaque"}
  LOD 250
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows addshadow
  #pragma target 3.0
  #include "UnityStandardUtils.cginc"
  sampler2D _MainTex,_BumpMap,_OcclusionMap;
  half _Glossiness,_Metallic,_BumpScale,_Desaturate;fixed4 _Color;
  struct Input {float2 uv_MainTex;fixed4 color:COLOR;};
  void surf(Input IN,inout SurfaceOutputStandard o) {
   fixed4 c=tex2D(_MainTex,IN.uv_MainTex);
   c.rgb=lerp(c.rgb,dot(c.rgb,float3(.299,.587,.114)).xxx,_Desaturate)*_Color.rgb;
   o.Albedo=c.rgb*IN.color.rgb;
   o.Normal=UnpackScaleNormal(tex2D(_BumpMap,IN.uv_MainTex),_BumpScale);
   o.Metallic=_Metallic;o.Smoothness=_Glossiness;
   o.Occlusion=tex2D(_OcclusionMap,IN.uv_MainTex).g*IN.color.a;
   o.Alpha=1;
  }
  ENDCG
 }
 Fallback "Diffuse"
}
