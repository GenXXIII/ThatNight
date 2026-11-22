Shader "ThatNight/ForestBark" {
 Properties { _MainTex("Bark",2D)="white"{} _Color("Bark tint",Color)=(.38,.42,.34,1) }
 SubShader {
  Tags {"RenderType"="Opaque"}
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows addshadow
  #pragma target 3.0
  sampler2D _MainTex;fixed4 _Color;
  struct Input {float3 worldPos;float3 worldNormal;};
  void surf(Input i,inout SurfaceOutputStandard o) {
   float3 weights=pow(abs(i.worldNormal),4);weights/=max(dot(weights,1),.001);
   float3 p=i.worldPos*.42;
   float3 bark=tex2D(_MainTex,p.zy).rgb*weights.x+tex2D(_MainTex,p.xz).rgb*weights.y+tex2D(_MainTex,p.xy).rgb*weights.z;
   o.Albedo=bark*_Color.rgb;o.Smoothness=.16;o.Metallic=0;o.Occlusion=.88;o.Alpha=1;
  }
  ENDCG
 } Fallback "Diffuse"
}
