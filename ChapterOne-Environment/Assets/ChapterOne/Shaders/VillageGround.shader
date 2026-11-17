Shader "ThatNight/VillageGround" {
 Properties {
  _Color("Earth tint",Color)=(0.76,0.69,0.57,1)
  _MainTex("Packed earth",2D)="white"{}
  _BumpMap("Earth normal",2D)="bump"{}
  _GrassTex("Leaf litter and grass",2D)="white"{}
  _GrassBump("Grass normal",2D)="bump"{}
  _TileScale("World texture scale",Float)=0.4
 }
 SubShader {
  Tags {"RenderType"="Opaque"}
  LOD 250
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows addshadow
  #pragma target 3.0
  #include "UnityStandardUtils.cginc"
  sampler2D _MainTex,_BumpMap,_GrassTex,_GrassBump;
  fixed4 _Color;half _TileScale;
  // Vertex channels: local shade, vegetation, moisture, foot/vehicle wear.
  struct Input {float3 worldPos;fixed4 color:COLOR;};
  void surf(Input IN,inout SurfaceOutputStandard o) {
   float2 uv=IN.worldPos.xz*_TileScale;
   fixed3 soil=tex2D(_MainTex,uv).rgb;
   fixed3 broad=tex2D(_MainTex,uv*.137+float2(.23,.61)).rgb;
   half detail=dot(broad,half3(.299,.587,.114));
   soil*=lerp(.83,1.13,detail);
   soil=lerp(soil,dot(soil,half3(.299,.587,.114)).xxx,.22)*_Color.rgb;
   fixed3 grass=tex2D(_GrassTex,uv*.83).rgb*half3(.53,.62,.40);
   half blend=saturate(IN.color.g+(detail-.5)*.18);
   fixed3 albedo=lerp(soil,grass,blend);
   albedo*=lerp(1,.67,IN.color.b);
   albedo*=lerp(1,1.12,IN.color.a);
   o.Albedo=albedo*lerp(.75,1,IN.color.r);
   o.Normal=normalize(lerp(UnpackScaleNormal(tex2D(_BumpMap,uv),.65),UnpackScaleNormal(tex2D(_GrassBump,uv*.83),.8),blend));
   o.Smoothness=lerp(.06,.25,IN.color.b);o.Metallic=0;
   o.Occlusion=lerp(.62,1,IN.color.r);o.Alpha=1;
  }
  ENDCG
 }
 Fallback "Diffuse"
}
