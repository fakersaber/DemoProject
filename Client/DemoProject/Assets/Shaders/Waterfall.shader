Shader "Custom/WaterfallShader"
{
    Properties
    {
        _MainTex ("MatinTex",2D) = "white" {}  
        _NoiseTex ("Noise texture", 2D) = "white" {}
        _DisplGuide("Displacement guide", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _DisplAmount("Displacement amount", float) = 0
        [HDR]_ColorBottomDark("Color bottom dark", color) = (1,1,1,1)
        [HDR]_ColorTopDark("Color top dark", color) = (1,1,1,1)
        [HDR]_ColorBottomLight("Color bottom light", color) = (1,1,1,1)
        [HDR]_ColorTopLight("Color top light", color) = (1,1,1,1)
        _BottomFoamThreshold("Bottom foam threshold", Range(0,1)) = 0.1
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
    ENDHLSL

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "LightweightPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Lightweight2D" }
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float4  color       : COLOR;
                float2	uv          : TEXCOORD0;
                float2	lightingUV  : TEXCOORD1;
                float2  noiseUV     : TEXCOORD2;
                float2  displUV     : TEXCOORD3;
            };

            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/2D/Include/LightingUtility.hlsl"

            // TEXTURE2D(_MainTex);
            // SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            //half4 _MainTex_ST;
            half4 _NormalMap_ST;
  
   
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            sampler2D _DisplGuide;
            float4 _DisplGuide_ST;
            half4 _ColorBottomDark;
            half4 _ColorTopDark;
            half4 _ColorBottomLight;
            half4 _ColorTopLight;
            half _DisplAmount;
            half _BottomFoamThreshold;
             
            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _NoiseTex);
                o.displUV = TRANSFORM_TEX(v.uv, _DisplGuide);
                float4 clipVertex = o.positionCS / o.positionCS.w;
                o.lightingUV = ComputeScreenPos(clipVertex).xy;

                #if UNITY_UV_STARTS_AT_TOP
                o.lightingUV.y = 1.0 - o.lightingUV.y;
                #endif

                o.color = v.color;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                //half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                half2 displ = tex2D(_DisplGuide, i.displUV + _Time.y / 5).xy;
                displ = ((displ * 2) - 1) * _DisplAmount;   
                half noise = tex2D(_NoiseTex, float2(i.noiseUV.x, i.noiseUV.y + _Time.y / 5) + displ).x;
                noise = round(noise * 5.0) / 5.0;   
                half4 col = lerp(lerp(_ColorBottomDark, _ColorTopDark, i.uv.y), lerp(_ColorBottomLight, _ColorTopLight, i.uv.y), noise);
                col = lerp(half4(1,1,1,1), col, step(_BottomFoamThreshold, i.uv.y + displ.y));
                col = CombinedShapeLightShared(col, mask, i.lightingUV);
                col.a = tex2D(_MainTex, i.uv).a;
                return col;
            }
            ENDHLSL
        }

        // Pass
        // {
        //     Tags { "LightMode" = "NormalsRendering"}
        //     HLSLPROGRAM
        //     #pragma prefer_hlslcc gles
        //     #pragma vertex NormalsRenderingVertex
        //     #pragma fragment NormalsRenderingFragment

        //     struct Attributes
        //     {
        //         float3 positionOS   : POSITION;
        //         float4 color		: COLOR;
        //         float2 uv			: TEXCOORD0;
        //     };

        //     struct Varyings
        //     {
        //         float4  positionCS		: SV_POSITION;
        //         float4  color			: COLOR;
        //         float2	uv				: TEXCOORD0;
        //         float3  normalWS		: TEXCOORD1;
        //         float3  tangentWS		: TEXCOORD2;
        //         float3  bitangentWS		: TEXCOORD3;
        //     };

        //     TEXTURE2D(_MainTex);
        //     SAMPLER(sampler_MainTex);
        //     TEXTURE2D(_NormalMap);
        //     SAMPLER(sampler_NormalMap);
        //     float4 _NormalMap_ST;  // Is this the right way to do this?

        //     Varyings NormalsRenderingVertex(Attributes attributes)
        //     {
        //         Varyings o = (Varyings)0;

        //         o.positionCS = TransformObjectToHClip(attributes.positionOS);
        //         #if UNITY_UV_STARTS_AT_TOP
        //             o.positionCS.y = -o.positionCS.y;
        //         #endif
        //         o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
        //         o.uv = attributes.uv;
        //         o.color = attributes.color;
        //         o.normalWS = TransformObjectToWorldDir(float3(0, 0, 1));
        //         o.tangentWS = TransformObjectToWorldDir(float3(1, 0, 0));
        //         o.bitangentWS = TransformObjectToWorldDir(float3(0, 1, 0));
        //         return o;
        //     }

        //     #include "Packages/com.unity.render-pipelines.lightweight/Shaders/2D/Include/NormalsRenderingShared.hlsl"

        //     float4 NormalsRenderingFragment(Varyings i) : SV_Target
        //     {
        //         float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        //         float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));
        //         return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, -i.normalWS.xyz);
        //     }
        //     ENDHLSL
        // }
    }

        // Pass
        // {
        //     Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }


        //     CGPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment frag
        //     // make fog work
        //     #pragma multi_compile_fog
             
        //     #include "UnityCG.cginc"
 
        //     struct appdata
        //     {
        //         float4 vertex : POSITION;
        //         float2 uv : TEXCOORD0;
        //     };
 
        //     struct v2f
        //     {
        //         float4 vertex : SV_POSITION;
        //         float2 uv : TEXCOORD0;
        //         float2 noiseUV : TEXCOORD1;
        //         float2 displUV : TEXCOORD2;
        //         UNITY_FOG_COORDS(3)
        //     };
 
        //     uniform sampler2D _MainTex;
        //     float4 _MainTex_ST;
        //     sampler2D _NoiseTex;
        //     float4 _NoiseTex_ST;
        //     sampler2D _DisplGuide;
        //     float4 _DisplGuide_ST;
        //     fixed4 _ColorBottomDark;
        //     fixed4 _ColorTopDark;
        //     fixed4 _ColorBottomLight;
        //     fixed4 _ColorTopLight;
        //     half _DisplAmount;
        //     half _BottomFoamThreshold;
             
        //     v2f vert (appdata v)
        //     {
        //         v2f o;
        //         o.vertex = UnityObjectToClipPos(v.vertex);
        //         o.noiseUV = TRANSFORM_TEX(v.uv, _NoiseTex);
        //         o.displUV = TRANSFORM_TEX(v.uv, _DisplGuide);
        //         o.uv = v.uv;
        //         UNITY_TRANSFER_FOG(o,o.vertex);
        //         return o;
        //     }
             
        //     fixed4 frag (v2f i) : SV_Target
        //     {
        //         half2 displ = tex2D(_DisplGuide, i.displUV + _Time.y / 5).xy;
        //         displ = ((displ * 2) - 1) * _DisplAmount;   
        //         half noise = tex2D(_NoiseTex, float2(i.noiseUV.x, i.noiseUV.y + _Time.y / 5) + displ).x;
        //         noise = round(noise * 5.0) / 5.0;   
        //         fixed4 col = lerp(lerp(_ColorBottomDark, _ColorTopDark, i.uv.y), lerp(_ColorBottomLight, _ColorTopLight, i.uv.y), noise);
        //         col = lerp(fixed4(1,1,1,1), col, step(_BottomFoamThreshold, i.uv.y + displ.y));
        //         fixed4 SampColor = tex2D(_MainTex,i.uv);
        //         clip(SampColor.a - 0.1);
        //         // half result = step(SampColor.a, 0.1);
        //         // fixed4 color = result * SampColor + col * (1-result);
        //         UNITY_APPLY_FOG(i.fogCoord, col);
        //         return col;
        //     }
        //     ENDCG
        // }
    Fallback "Hidden/Sprite-Fallback"
}