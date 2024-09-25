Shader "Custom/Mask"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _MaskLayer ("Culling Mask", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0, 1)) = 0
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Transparent"
      }
      ZWrite Off
      Fog
      { 
        Mode  Off
      } 
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MaskLayer_ST;
      uniform float4 _MainTex_ST;
      uniform float _Cutoff;
      uniform sampler2D _MaskLayer;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float3 vertex :POSITION0;
          float3 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 color :COLOR0;
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
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
          out_v.color = float4(0, 0, 0, 1);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MaskLayer);
          out_v.texcoord1.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.vertex = UnityObjectToClipPos(float4(in_v.vertex, 0));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      int u_xlatb1;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.w = tex2D(_MaskLayer, in_f.texcoord.xy).w;
          u_xlatb1 = (u_xlat0_d.w<_Cutoff);
          if(((int(u_xlatb1) * (-1))!=0))
          {
              discard;
          }
          u_xlat0_d.xyz = tex2D(_MainTex, in_f.texcoord1.xy).xyz;
          out_f.color = u_xlat0_d;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
