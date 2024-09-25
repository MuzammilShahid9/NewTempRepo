Shader "Particles/Additive_SubUV"
{
  Properties
  {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
    _Wid ("Wid", float) = 4
    _Hei ("Hei", float) = 4
    _Tile ("Tile", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "PreviewType" = "Plane"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "PreviewType" = "Plane"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZWrite Off
      Cull Off
      Blend SrcAlpha One
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float4 _TintColor;
      uniform float _Wid;
      uniform float _Hei;
      uniform float _Tile;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 color :COLOR0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 color :COLOR0;
          float2 texcoord :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 color :COLOR0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.color = in_v.color;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float2 u_xlat0_d;
      float4 u_xlat16_0;
      float4 u_xlat10_0;
      float2 u_xlat1_d;
      float4 u_xlat16_1;
      float2 u_xlat2;
      float u_xlat3;
      float roundEven(float x)
      {
          float y = floor((x + 0.5));
          return (((y - x)==0.5))?((floor((0.5 * y)) * 2)):(y);
      }
      
      float2 roundEven(float2 a)
      {
          a.x = round(a.x);
          a.y = round(a.y);
          return a;
      }
      
      float3 roundEven(float3 a)
      {
          a.x = round(a.x);
          a.y = round(a.y);
          a.z = round(a.z);
          return a;
      }
      
      float4 roundEven(float4 a)
      {
          a.x = round(a.x);
          a.y = round(a.y);
          a.z = round(a.z);
          a.w = round(a.w);
          return a;
      }
      
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = round(_Tile);
          u_xlat1_d.x = (float(1) / _Wid);
          u_xlat3 = (u_xlat0_d.x * u_xlat1_d.x);
          u_xlat3 = floor(u_xlat3);
          u_xlat2.x = (((-_Wid) * u_xlat3) + u_xlat0_d.x);
          u_xlat0_d.y = (u_xlat3 + (-in_f.texcoord.y));
          u_xlat0_d.x = in_f.texcoord.x;
          u_xlat2.y = 1;
          u_xlat0_d.xy = (u_xlat0_d.xy + u_xlat2.xy);
          u_xlat1_d.y = (float(1) / (-_Hei));
          u_xlat0_d.xy = (u_xlat0_d.xy * u_xlat1_d.xy);
          u_xlat10_0 = tex2D(_MainTex, u_xlat0_d.xy);
          u_xlat16_1 = (in_f.color + in_f.color);
          u_xlat16_1 = (u_xlat16_1 * _TintColor);
          u_xlat16_0 = (u_xlat10_0 * u_xlat16_1);
          out_f.color.w = u_xlat16_0.w;
          out_f.color.w = clamp(out_f.color.w, 0, 1);
          out_f.color.xyz = u_xlat16_0.xyz;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
