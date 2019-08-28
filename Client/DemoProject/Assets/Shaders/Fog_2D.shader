Shader "Custom/Fog"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Noise texture", 2D) = "white" {}
		_Color ("Fog Color", Color) = (1,1,1,1)
		_FogIntensity ("Fog Intensity", Range(0,1)) = 0.2
		_FogMass ("Fog Mass",Range(0,1)) = 0.1
	}
	SubShader
	{
		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always 
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _NoiseTex;
			float3 _Color;
			float _FogIntensity;
			float _FogMass;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex,i.uv);
				half Noise = tex2D(_NoiseTex,i.uv + _Time.x * 2).x;
				fixed3 FinalColor = lerp(col.rgb, _Color.rgb, Noise *_FogMass);
				col.rgb = lerp(col.rgb, FinalColor, Noise * _FogIntensity);
				return col;
			}
			ENDCG
		}
	}
}