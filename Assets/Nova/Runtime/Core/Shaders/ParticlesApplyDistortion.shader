Shader "Hidden/Nova/Particles/ApplyDistortion"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #if UNITY_VERSION >= 202200
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #endif

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ScreenSpaceUvTexture);
            SAMPLER(sampler_ScreenSpaceUvTexture);
            half4 _TintColor;

            #if UNITY_VERSION < 202200
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

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }
            #endif

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                #if UNITY_VERSION >= 202200
                const float2 uv = IN.texcoord;
                #else
                const float2 uv = IN.uv;
                #endif
                half2 dist = SAMPLE_TEXTURE2D(_ScreenSpaceUvTexture, sampler_ScreenSpaceUvTexture, uv).xy;
                dist = dist.xy * 2.0 - 1.0;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + dist);
                return col;
            }
            ENDHLSL
        }
    }
}
