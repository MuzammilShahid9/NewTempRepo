Shader "Custom/FlowLightShader"
{
  Properties
  {
    [HideInInspector] _MainTex ("Base (RGB)", 2D) = "white" {}
    _FlashColor ("Flash Color", Color) = (1,1,1,1)
    _Angle ("Flash Angle", Range(0, 180)) = 45
    _Width ("Flash Width", Range(0, 1)) = 0.2
    _LoopTime ("Loop Time", float) = 0.5
    _Interval ("Time Interval", float) = 1.5
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 200
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 200
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      //uniform float4 _Time;
      uniform float4 _FlashColor;
      uniform float _Angle;
      uniform float _Width;
      uniform float _LoopTime;
      uniform float _Interval;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float u_xlat6;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord2.xyz = ((conv_mxt4x4_3(unity_ObjectToWorld).xyz * in_v.vertex.www) + u_xlat0.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          u_xlat0.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.texcoord.xy = u_xlat0.xy;
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord1.xyz = normalize(u_xlat0.xyz);
          out_v.texcoord2.w = 0;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float u_xlat0_d;
      float4 u_xlat1_d;
      float u_xlat2;
      float u_xlat3;
      int u_xlatb3;
      float u_xlat6_d;
      int u_xlatb6;
      float u_xlat9;
      int u_xlatb9;
      float trunc(float x)
      {
          return (sign(x) * floor(abs(x)));
      }
      
      float2 trunc(float2 x)
      {
          return (sign(x) * floor(abs(x)));
      }
      
      float3 trunc(float3 x)
      {
          return (sign(x) * floor(abs(x)));
      }
      
      float4 trunc(float4 x)
      {
          return (sign(x) * floor(abs(x)));
      }
      
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = (_LoopTime + _Interval);
          u_xlat3 = (_Time.y / u_xlat0_d);
          u_xlat3 = trunc(u_xlat3);
          u_xlat0_d = (((-u_xlat3) * u_xlat0_d) + _Time.y);
          u_xlat0_d = (u_xlat0_d + (-_Interval));
          u_xlat0_d = (u_xlat0_d / _LoopTime);
          u_xlat3 = (_Angle * 0.0174444001);
          u_xlat1_d.x = sin(u_xlat3);
          u_xlat2 = cos(u_xlat3);
          u_xlat3 = (u_xlat1_d.x / u_xlat2);
          u_xlat3 = (float(1) / u_xlat3);
          u_xlat6_d = (u_xlat3 + 1);
          u_xlatb9 = (0<u_xlat3);
          u_xlat6_d = (u_xlatb9)?(u_xlat6_d):(1);
          u_xlat9 = (u_xlatb9)?(0):(u_xlat3);
          u_xlat3 = ((in_f.texcoord.y * u_xlat3) + in_f.texcoord.x);
          u_xlat6_d = ((-u_xlat9) + u_xlat6_d);
          u_xlat0_d = ((u_xlat0_d * u_xlat6_d) + u_xlat9);
          u_xlatb6 = (u_xlat3<u_xlat0_d);
          u_xlat9 = (u_xlat0_d + (-_Width));
          u_xlat0_d = (u_xlat0_d + u_xlat9);
          u_xlatb9 = (u_xlat9<u_xlat3);
          u_xlat0_d = ((u_xlat3 * 2) + (-u_xlat0_d));
          u_xlat0_d = (abs(u_xlat0_d) / _Width);
          u_xlat0_d = ((-u_xlat0_d) + 1);
          u_xlatb3 = (u_xlatb6 && u_xlatb9);
          u_xlat0_d = (u_xlatb3)?(u_xlat0_d):(float(0));
          u_xlat1_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat1_d.xyz = ((_FlashColor.xyz * float3(u_xlat0_d, u_xlat0_d, u_xlat0_d)) + u_xlat1_d.xyz);
          out_f.color = u_xlat1_d;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
