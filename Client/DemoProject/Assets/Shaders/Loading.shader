Shader "Custom/Loading" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _Center ("Center", vector) = (0.5, 0.5, 0, 0)
        //_Radius ("Radius", Range(0,0.707)) = 0.707
        // _Alpha("Alpha", float) = 1.0
        // _Fade ("Fade Rate", float) = 0.99
        // _Interval ("Interval Time", float) = 3
    }
    SubShader {
        LOD 100

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }
            Offset -1, -1
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMaterial AmbientAndDiffuse
            

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
                        
            #include "UnityCG.cginc"            

            sampler2D _MainTex;
            float2 _Center;
            float _Radius;
            //float _Alpha;
            //float _Fade;
            //float _Interval;
            
            struct v2f 
            {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord;
                return o;
            }
            
            half4 frag (v2f i) : COLOR
            {                
                half4 texcol = tex2D (_MainTex, i.uv);
                //float dis = sqrt(pow(i.uv - _Center.xy,2));
                float dis = distance(i.uv, _Center.xy);
                half result = step(dis, _Radius);
                texcol.a = result * texcol.a + (1 - result) * 0;


                
                return texcol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}