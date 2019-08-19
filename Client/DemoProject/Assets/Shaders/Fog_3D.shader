Shader "Custom/RainRippleFX"
{
	Properties
	{
		[HDR]_LightColor("Light Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_DFogDensity("DFog Density", Range(0,100)) = 1
		_VFogDensity("VFog Density", Range(0,20)) = 1
		_DFogColorFar("DFog Color Far", Color) = (1,1,1,1)
		_DFogColorNear("DFog Color Near", Color) = (1,1,1,1)
		_VFogColorLow("VFog Color Low", Color) = (1,1,1,1)
		_VFogColorHigh("VFog Color High", Color) = (1,1,1,1)
		_FogMass("Fog Mass", Range(0,100)) = 24
		_StartY("StartY", float) = 0
		_DFogHeight("DFogHeight", float) = 0
		_DFogMass("DFogMass", float) = 24

	}
	SubShader
	{
		Cull Off 
        ZWrite Off 
        ZTest Always
		Fog { Mode Off } 
        Blend Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature _ USE_LUT
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				
			};

			struct v2f
			{
				float4 frustumDir : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4x4 _FrustumDir;
			float _DFogDensity, _VFogDensity;
			half _StartY;
			half _FogMass, _DFogHeight, _DFogMass;
			fixed4 _DFogColorNear, _DFogColorFar, _VFogColorHigh, _VFogColorLow;
			float3 _CamPos;
			fixed4 _LightColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float2 uv = v.uv;

				int ix = (int)uv.x;
				int iy = (int)uv.y;
				o.frustumDir = _FrustumDir[ix + 2 * iy];

				o.uv = uv;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
								
				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));

				float linear01Depth = Linear01Depth(depth); 
				float linearEyeDepth = LinearEyeDepth(depth);

				float3 worldPos = _WorldSpaceCameraPos + linearEyeDepth * i.frustumDir.xyz;

				float fogDensityV = saturate(pow(exp((_StartY - worldPos.y) * 0.01), _FogMass) * _VFogDensity) * _VFogColorLow.a;
				
				float fogDensity = saturate((1-exp(-linear01Depth * _DFogDensity)) ); 
				fogDensity *= pow(fogDensity, lerp(2, 0, fogDensity));
				fogDensity *= saturate(pow(exp((_CamPos.y - worldPos.y + _DFogHeight) * 0.01), _DFogMass));
				_DFogColorFar.a = lerp(_DFogColorNear.a, _DFogColorFar.a, fogDensity);

				float3 RayDir = normalize(worldPos - _CamPos.xyz);
				float sunAmount = saturate(dot(RayDir, normalize(_WorldSpaceLightPos0.xyz)));
				sunAmount *= sunAmount;

				col.rgb = lerp(col.rgb, lerp(lerp(_DFogColorNear.rgb, _DFogColorFar.rgb, pow(saturate(linear01Depth),0.25)), _LightColor.rgb, sunAmount * _LightColor.a), fogDensity * _DFogColorFar.a);
				col.rgb = lerp(col.rgb, lerp(lerp(_VFogColorHigh.rgb, _VFogColorLow.rgb, fogDensityV * fogDensityV), _LightColor.rgb, sunAmount * _LightColor.a), fogDensityV);

				return col;

			}
			ENDCG
		}
	}
}