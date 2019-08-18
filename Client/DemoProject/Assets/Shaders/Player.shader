Shader "Custom/2D/Sprite-Lit-Default"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
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
            };

            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _MainTex_ST;
            half4 _NormalMap_ST;

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
                half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                return CombinedShapeLightShared(main, mask, i.lightingUV);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
                float3  normalWS		: TEXCOORD1;
                float3  tangentWS		: TEXCOORD2;
                float3  bitangentWS		: TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;  // Is this the right way to do this?

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                #if UNITY_UV_STARTS_AT_TOP
                    o.positionCS.y = -o.positionCS.y;
                #endif
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.uv = attributes.uv;
                o.color = attributes.color;
                o.normalWS = TransformObjectToWorldDir(float3(0, 0, 1));
                o.tangentWS = TransformObjectToWorldDir(float3(1, 0, 0));
                o.bitangentWS = TransformObjectToWorldDir(float3(0, 1, 0));
                return o;
            }

            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            float4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));
                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, -i.normalWS.xyz);
            }
            ENDHLSL
        }
        Pass
        {
            Tags { "LightMode" = "LightweightForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.uv = attributes.uv;
                o.color = attributes.color;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return mainTex;
            }
            ENDHLSL
        }

        // Pass
        // {
        //     //采样周边像素alpha取平均值，给外部加发光效果(1-col.a可避免内部发光) 
        //     Cull back
        //     Lighting Off
        //     ZWrite Off
        //     Offset -1, -1
        //     Fog { Mode Off }
        //     Blend SrcAlpha OneMinusSrcAlpha
            
        //     CGPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment frag
        //     #include "UnityCG.cginc"

        //     sampler2D _MainTex;
        //     float4 _Color;
        //     float _Factor;
        //     float _SampleRange;
        //     float2 _TexSize;
        //     float2 _SampleInterval;

        //     struct appdata_t
        //     {
        //         float4 vertex : POSITION;
        //         float2 texcoord : TEXCOORD0;
        //     };
    
        //     struct v2f
        //     {
        //         float4 vertex : SV_POSITION;
        //         half2 uv : TEXCOORD0;
        //     };

        //     v2f vert(appdata_t v)
        //     {
        //         v2f o;
        //         o.vertex = UnityObjectToClipPos(v.vertex);
        //         o.uv = v.texcoord;
        //         return o;
        //     }

        //     fixed4 frag(v2f i) : SV_Target
        //     {
        //         int range = (int)_SampleRange;
        //         float radiusX = _SampleInterval.x / _TexSize.x;
        //         float radiusY = _SampleInterval.y / _TexSize.y;
        //         float inner = 0;
        //         //float outter = 0;
        //         int count = 0;
        //         for (int k = -range; k <= range; ++k)
        //         {
        //             for (int j = -range; j <= range; ++j)
        //             {
        //                 float4 m = tex2D(_MainTex, float2(i.uv.x + k * radiusX , i.uv.y + j * radiusY));
        //                 //outter += 1 - m.a;
        //                 inner += m.a;
        //                 count += 1;
        //             }
        //         }
        //         inner /= count;
        //         //outter /= count;
        //         fixed4 col = tex2D(_MainTex, i.uv);
        //         col.rgb = col.rgb + (1-col.a) * _Factor * _Color.a * _Color.rgb;
        //         col.a = max(col.a, inner);
        //         return col;
        //     }
        //     ENDCG
        // }
    }
    Fallback "Hidden/Sprite-Fallback"
}
