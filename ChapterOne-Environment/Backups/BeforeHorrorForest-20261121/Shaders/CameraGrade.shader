Shader "ThatNight/CameraGrade" {
 Properties {_MainTex("Scene",2D)="white"{}}
 SubShader {Cull Off ZWrite Off ZTest Always
 Pass {
 CGPROGRAM
 #pragma vertex vert_img
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _MainTex_TexelSize;
 fixed4 frag(v2f_img i):SV_Target {
  float3 c=tex2D(_MainTex,i.uv).rgb;
  float2 texel=_MainTex_TexelSize.xy*3;
  float3 bloom=0;
  bloom+=max(0,tex2D(_MainTex,i.uv+float2(-texel.x,-texel.y)).rgb-.8);
  bloom+=max(0,tex2D(_MainTex,i.uv+float2( texel.x,-texel.y)).rgb-.8);
  bloom+=max(0,tex2D(_MainTex,i.uv+float2(-texel.x, texel.y)).rgb-.8);
  bloom+=max(0,tex2D(_MainTex,i.uv+float2( texel.x, texel.y)).rgb-.8);
  c+=bloom*.07;c=c/(1+c*.14);
  float2 uv=i.uv*2-1;c*=1-.13*dot(uv,uv);
  return float4(c,1);
 }
 ENDCG
 }}
}
