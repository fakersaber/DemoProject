Shader "Custom/HotWave"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseTex("Noise",2D) = "while" {}
        _LuminosityAmount("LuminosityAmount",Range(0.0,1.0)) = 1.0
        _DistortTimeFactor("DistortTimeFactor",Range(0,1)) = 1
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform sampler2D _MainTex;
            uniform sampler2D _NoiseTex;
            fixed _LuminosityAmount;
            float _DistortTimeFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 noise = tex2D(_NoiseTex,i.uv - _Time.xy * _DistortTimeFactor);
                float2 offset = noise.xy * _LuminosityAmount;
                return tex2D(_MainTex, i.uv + offset);
            }
            ENDCG
        }
    }
}
