Shader "Jingyu/niuqu_dongtai"
{
  Properties
  {
    _NoiseTex ("Noise Texture (RG)", 2D) = "white" {}
    _MainTex ("Alpha (A)", 2D) = "white" {}
    _HeatTime ("Heat Time", Range(0, 1.5)) = 1
    _HeatForce ("Heat Force", Range(0, 0.1)) = 0.1
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent+1"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
      }
      ZClip Off
      ZWrite Off
      Cull Off
      Stencil
      { 
        Ref 0
        ReadMask 0
        WriteMask 0
        Pass Keep
        Fail Keep
        ZFail Keep
        PassFront Keep
        FailFront Keep
        ZFailFront Keep
        PassBack Keep
        FailBack Keep
        ZFailBack Keep
      } 
      // m_ProgramMask = 0
      
    } // end phase
    Pass // ind: 2, name: BASE
    {
      Name "BASE"
      Tags
      { 
        "LIGHTMODE" = "ALWAYS"
        "QUEUE" = "Transparent+1"
        "RenderType" = "Transparent"
      }
      ZWrite Off
      Cull Off
      Blend SrcAlpha OneMinusSrcAlpha
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
      //uniform float4 _Time;
      uniform float _HeatForce;
      uniform float _HeatTime;
      uniform sampler2D _NoiseTex;
      uniform sampler2D _GrabTexture;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 texcoord :TEXCOORD0;
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
          u_xlat0 = UnityObjectToClipPos(in_v.vertex);
          out_v.vertex = u_xlat0;
          u_xlat0.xy = (u_xlat0.ww + u_xlat0.xy);
          out_v.texcoord.zw = u_xlat0.zw;
          out_v.texcoord.xy = (u_xlat0.xy * float2(0.5, 0.5));
          out_v.texcoord1.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float2 u_xlat0_d;
      float3 u_xlat10_0;
      float2 u_xlat16_1;
      float4 u_xlat10_1;
      float2 u_xlat4;
      float2 u_xlat10_4;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = ((_Time.xz * float2(float2(_HeatTime, _HeatTime))) + in_f.texcoord1.xy);
          u_xlat10_0.xy = tex2D(_NoiseTex, u_xlat0_d.xy).xy;
          u_xlat4.xy = (((-_Time.yx) * float2(float2(_HeatTime, _HeatTime))) + in_f.texcoord1.xy);
          u_xlat10_4.xy = tex2D(_NoiseTex, u_xlat4.xy).xy;
          u_xlat16_1.xy = (u_xlat10_4.xy + u_xlat10_0.xy);
          u_xlat16_1.xy = (u_xlat16_1.xy + float2(-1, (-1)));
          u_xlat0_d.xy = ((u_xlat16_1.xy * float2(_HeatForce, _HeatForce)) + in_f.texcoord.xy);
          u_xlat0_d.xy = (u_xlat0_d.xy / in_f.texcoord.ww);
          u_xlat10_0.xyz = tex2D(_GrabTexture, u_xlat0_d.xy).xyz;
          u_xlat10_1 = tex2D(_MainTex, in_f.texcoord1.xy);
          out_f.color.xyz = (u_xlat10_0.xyz * u_xlat10_1.xyz);
          out_f.color.w = u_xlat10_1.w;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent+1"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: BASE
    {
      Name "BASE"
      Tags
      { 
        "QUEUE" = "Transparent+1"
        "RenderType" = "Transparent"
      }
      ZWrite Off
      Cull Off
      Fog
      { 
        Mode  Off
      } 
      Blend DstColor Zero
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
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.color = float4(0, 0, 0, 1);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.vertex = UnityObjectToClipPos(float4(in_v.vertex, 0));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat10_0;
      int u_xlatb1;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat10_0 = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlatb1 = (0.00999999978>=u_xlat10_0.w);
          out_f.color = u_xlat10_0;
          if(((int(u_xlatb1) * (-1))!=0))
          {
              discard;
          }
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
