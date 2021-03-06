﻿Shader "Custom/HealthBar"
{
    Properties
    {
        _MainTex ("MatinTex",2D) = "white" {}  
        _HealthColor ("HealthColor",Color) = (0,1,0,1)
        _DamageColor ("DamageColor",Color) = (1,0,0,1)
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"} //"RenderPipeline" = "LightweightPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        //Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            uniform fixed4 _Color;  
            uniform sampler2D _MainTex;
            float4 _MainTex_ST;
            uniform float _CurHealth;
            fixed4 _HealthColor;
            fixed4 _DamageColor;


            float MappingHealth(float curHealth){
                return curHealth * 0.95 + 0.025;
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 SampColor = tex2D(_MainTex,i.uv);
                half healthResult = step(i.uv.x,MappingHealth(_CurHealth));
                fixed4 healthColor = fixed4(_HealthColor.rgb,SampColor.a) * healthResult + fixed4(_DamageColor.rgb,SampColor.a) * (1-healthResult);
                half result = step(SampColor.a, 0.4);
                fixed4 color = (1 - result) * SampColor + healthColor * result;
                return color;
            }
            ENDCG
        }
    }
}

