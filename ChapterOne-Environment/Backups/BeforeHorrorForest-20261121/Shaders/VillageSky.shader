Shader "ThatNight/VillageSky" {
 Properties {_Day("Day",Range(0,1))=0}
 SubShader {
  Tags {"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"}
  Cull Off ZWrite Off
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   struct appdata {float4 vertex:POSITION;};
   struct v2f {float4 pos:SV_POSITION;float3 dir:TEXCOORD0;};
   float _Day;
   v2f vert(appdata v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.dir=v.vertex.xyz;return o;}
   float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
   float noise(float2 p){float2 i=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash(i),hash(i+float2(1,0)),f.x),lerp(hash(i+float2(0,1)),hash(i+1),f.x),f.y);}
   fixed4 frag(v2f i):SV_Target {
    float3 d=normalize(i.dir);float h=saturate(d.y);
    float3 night=lerp(float3(.028,.039,.047),float3(.012,.025,.05),pow(h,.35));
    float3 day=lerp(float3(.48,.57,.59),float3(.13,.29,.47),pow(h,.4));
    float2 sky=d.xz/(max(.12,d.y)+.5)*3;
    float cloud=noise(sky)*.5+noise(sky*2.7)*.3+noise(sky*7.1)*.2;
    cloud=smoothstep(.52,.72,cloud)*smoothstep(0,.2,h);
    float3 color=lerp(night,day,_Day)+cloud*lerp(float3(.023,.03,.035),float3(.24,.24,.22),_Day);
    float moon=smoothstep(.99975,.9999,dot(d,normalize(float3(.35,.49,.6))));
    float halo=pow(saturate(dot(d,normalize(float3(.35,.49,.6)))),250)*.07;
    color+=(moon*.85+halo)*(1-_Day)*(1-cloud*.85);
    return float4(color,1);
   }
   ENDCG
  }
 }
}
