Shader "Hidden/Nova/Particles/ApplyDistortion"
{
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ScreenSpaceUvTexture);
            SAMPLER(sampler_ScreenSpaceUvTexture);
            half4 _TintColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {                
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                half2 dist = SAMPLE_TEXTURE2D(_ScreenSpaceUvTexture, sampler_ScreenSpaceUvTexture, IN.uv).xy;
                dist = dist.xy * 2.0 - 1.0;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + dist);
                return col;
            }
            ENDHLSL
        }
    }
}