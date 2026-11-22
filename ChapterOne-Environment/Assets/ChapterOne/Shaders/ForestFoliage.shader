Shader "ThatNight/ForestFoliage" {
 Properties { _Color("Fern tint",Color)=(.24,.36,.23,1) _Wind("Wind movement",Range(0,.3))=.055 }
 SubShader {
  Tags {"RenderType"="Opaque"} Cull Off
  CGPROGRAM
  #pragma surface surf Standard vertex:vert addshadow fullforwardshadows
  #pragma target 3.0
  #pragma multi_compile_instancing
  fixed4 _Color; float _Wind;
  struct Input { fixed4 color:COLOR; float2 uv_MainTex; float facing:VFACE; };
  void vert(inout appdata_full v) {
   float3 w=mul(unity_ObjectToWorld,v.vertex).xyz;
   v.vertex.x+=sin(w.x*.27+w.z*.18+_Time.y*1.1)*_Wind*v.texcoord.y;
   v.vertex.z+=cos(w.z*.31+_Time.y*.8)*_Wind*.6*v.texcoord.y;
  }
  void surf(Input i,inout SurfaceOutputStandard o) {
   o.Albedo=_Color.rgb*i.color.rgb;
   o.Normal=float3(0,0,i.facing>=0?1:-1);
   o.Emission=0;
   o.Metallic=0; o.Smoothness=.2; o.Occlusion=.85; o.Alpha=1;
  }
  ENDCG
 } Fallback "Diffuse"
}
