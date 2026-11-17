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
  float3 c=tex2D(_MainTex,i.uv).rgb;float3 bloom=0;
  for(int j=0;j<4;j++) {float2 o=float2(j%2==0?-1:1,j<2?-1:1)*_MainTex_TexelSize.xy*3;bloom+=max(0,tex2D(_MainTex,i.uv+o).rgb-.8);}
  c+=bloom*.07;c=c/(1+c*.14);
  float2 uv=i.uv*2-1;c*=1-.13*dot(uv,uv);
  return float4(c,1);
 }
 ENDCG
 }}
}
