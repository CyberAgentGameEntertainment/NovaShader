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
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ScreenSpaceUvTexture);
            SAMPLER(sampler_ScreenSpaceUvTexture);
            half4 _TintColor;

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                const float2 uv = IN.texcoord;
                half2 dist = SAMPLE_TEXTURE2D(_ScreenSpaceUvTexture, sampler_ScreenSpaceUvTexture, uv).xy;
                dist = dist.xy * 2.0 - 1.0;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + dist);
                return col;
            }
            ENDHLSL
        }
    }
}
