Shader "Custom/EnergyBar"
{
    Properties
    {
        _MainTex ("MatinTex",2D) = "white" {}  
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"} 
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            uniform fixed4 _Color;  
            uniform sampler2D _MainTex;


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
                return SampColor;
            }
            ENDCG
        }
    }
}

