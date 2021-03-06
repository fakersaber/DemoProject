﻿Shader "Custom/WaterFoamShader"
{
    Properties
    {
        _MainTex ("MatinTex",2D) = "white" {}  
        _NoiseTex ("Noise texture", 2D) = "white" {}
        _DisplGuide("Displacement guide", 2D) = "white" {}
        _DisplAmount("Displacement amount", float) = 0
        [HDR]_ColorBottomDark("Color bottom dark", color) = (1,1,1,1)
        [HDR]_ColorTopDark("Color top dark", color) = (1,1,1,1)
        [HDR]_ColorBottomLight("Color bottom light", color) = (1,1,1,1)
        [HDR]_ColorTopLight("Color top light", color) = (1,1,1,1)
        _BottomFoamThreshold("Bottom foam threshold", Range(0,1)) = 0.1
    }
    SubShader
    {   
        Tags { "RenderType"="Opaque" }
        LOD 100
 
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
             
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                float2 displUV : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
 
            uniform sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            sampler2D _DisplGuide;
            float4 _DisplGuide_ST;
            fixed4 _ColorBottomDark;
            fixed4 _ColorTopDark;
            fixed4 _ColorBottomLight;
            fixed4 _ColorTopLight;
            half _DisplAmount;
            half _BottomFoamThreshold;
             
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
             
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 SampColor = tex2D(_MainTex,i.uv);
                half result = step(SampColor.a, 0.1);
                fixed4 color = result * SampColor + fixed4(1,1,1,1) * (1-result);
                return color;
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}