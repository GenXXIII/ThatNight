Shader "ThatNight/CameraGrade" {
 Properties {_MainTex("Scene",2D)="white"{} _Horror("Forest horror grade",Range(0,1))=0}
 SubShader {Cull Off ZWrite Off ZTest Always
 Pass {
 CGPROGRAM
 #pragma vertex vert_img
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _MainTex_TexelSize;float _Horror;
 fixed4 frag(v2f_img i):SV_Target {
  float3 c=tex2D(_MainTex,i.uv).rgb;
  float2 texel=_MainTex_TexelSize.xy*3;
  float3 bloom=0;
  bloom+=max(0,tex2D(_MainTex,i.uv+float2(-texel.x,-texel.y)).rgb-.8);
  bloom+=max(0,tex2D(_MainTex,i.uv+float2( texel.x,-texel.y)).rgb-.8);
  bloom+=max(0,tex2D(_MainTex,i.uv+float2(-texel.x, texel.y)).rgb-.8);
  bloom+=max(0,tex2D(_MainTex,i.uv+float2( texel.x, texel.y)).rgb-.8);
  c+=bloom*.07;c=c/(1+c*.14);
  float luminance=dot(c,float3(.2126,.7152,.0722));
  float3 horror=lerp(luminance.xxx,c,.76);
  horror*=float3(.87,1.015,1.035);
  c=lerp(c,horror,_Horror);
  float2 uv=i.uv*2-1;c*=1-lerp(.13,.19,_Horror)*dot(uv,uv);
  return float4(c,1);
 }
 ENDCG
 }}
}
