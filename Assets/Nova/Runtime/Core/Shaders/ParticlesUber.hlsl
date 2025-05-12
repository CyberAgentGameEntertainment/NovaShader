#ifndef NOVA_PARTICLESUBER_INCLUDED
#define NOVA_PARTICLESUBER_INCLUDED

#include "Particles.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);
TEXTURE2D_ARRAY(_BaseMap2DArray);
SAMPLER(sampler_BaseMap2DArray);
TEXTURE3D(_BaseMap3D);
SAMPLER(sampler_BaseMap3D);
TEXTURE2D(_TintMap);
SAMPLER(sampler_TintMap);
TEXTURE3D(_TintMap3D);
SAMPLER(sampler_TintMap3D);
TEXTURE2D(_FlowMap);
SAMPLER(sampler_FlowMap);
TEXTURE2D(_AlphaTransitionMap);
SAMPLER(sampler_AlphaTransitionMap);
TEXTURE2D(_AlphaTransitionMapSecondTexture);
SAMPLER(sampler_AlphaTransitionMapSecondTexture);
TEXTURE2D_ARRAY(_AlphaTransitionMap2DArray);
SAMPLER(sampler_AlphaTransitionMap2DArray);
TEXTURE2D_ARRAY(_AlphaTransitionMapSecondTexture2DArray);
SAMPLER(sampler_AlphaTransitionMapSecondTexture2DArray);
TEXTURE3D(_AlphaTransitionMap3D);
SAMPLER(sampler_AlphaTransitionMap3D);
TEXTURE3D(_AlphaTransitionMapSecondTexture3D);
SAMPLER(sampler_AlphaTransitionMapSecondTexture3D);
TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);
TEXTURE2D_ARRAY(_EmissionMap2DArray);
SAMPLER(sampler_EmissionMap2DArray);
TEXTURE3D(_EmissionMap3D);
SAMPLER(sampler_EmissionMap3D);
TEXTURE2D(_EmissionColorRamp);
SAMPLER(sampler_EmissionColorRamp);
TEXTURE2D(_GradientMap);
SAMPLER(sampler_GradientMap);
TEXTURE2D(_VertexDeformationMap);
SAMPLER(sampler_VertexDeformationMap);
// Normal map
TEXTURE2D(_NormalMap);
SAMPLER(sampler_NormalMap);
TEXTURE2D_ARRAY(_NormalMap2DArray);
SAMPLER(sampler_NormalMap2DArray);
TEXTURE3D(_NormalMap3D);
SAMPLER(sampler_NormalMap3D);
// Parallax Map
TEXTURE2D(_ParallaxMap);
SAMPLER(sampler_ParallaxMap);
TEXTURE2D_ARRAY(_ParallaxMap2DArray);
SAMPLER(sampler_ParallaxMap2DArray);
TEXTURE3D(_ParallaxMap3D);
SAMPLER(sampler_ParallaxMap3D);
// Specular Map
TEXTURE2D(_SpecularMap);
SAMPLER(sampler_SpecularMap);
TEXTURE2D_ARRAY(_SpecularMap2DArray);
SAMPLER(sampler_SpecularMap2DArray);
TEXTURE3D(_SpecularMap3D);
SAMPLER(sampler_SpecularMap3D);
// Metallic Map
TEXTURE2D(_MetallicMap);
SAMPLER(sampler_MetallicMap);
TEXTURE2D_ARRAY(_MetallicMap2DArray);
SAMPLER(sampler_MetallicMap2DArray);
TEXTURE3D(_MetallicMap3D);
SAMPLER(sampler_MetallicMap3D);
// Smoothness Map
TEXTURE2D(_SmoothnessMap);
SAMPLER(sampler_SmoothnessMap);
TEXTURE2D_ARRAY(_SmoothnessMap2DArray);
SAMPLER(sampler_SmoothnessMap2DArray);
TEXTURE3D(_SmoothnessMap3D);
SAMPLER(sampler_SmoothnessMap3D);

CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    float4 _BaseMap2DArray_ST;
    float4 _BaseMap3D_ST;
    float _BaseMapProgress;
    float _BaseMapProgressCoord;
    float _BaseMapSliceCount;
    DECLARE_CUSTOM_COORD(_BaseMapOffsetXCoord);
    DECLARE_CUSTOM_COORD(_BaseMapOffsetYCoord);
    float _BaseMapRotation;
    DECLARE_CUSTOM_COORD(_BaseMapRotationCoord);
    float4 _BaseMapRotationOffsets;

    half4 _TintColor;
    float4 _TintMap_ST;
    float4 _TintMap3D_ST;
    float _TintMap3DProgress;
    DECLARE_CUSTOM_COORD(_TintMap3DProgressCoord);
    float _TintMapSliceCount;
    DECLARE_CUSTOM_COORD(_TintMapOffsetXCoord);
    DECLARE_CUSTOM_COORD(_TintMapOffsetYCoord);
    float _TintBlendRate;
    float _TintBlendRateCoord;
    float _TintRimProgress;
    float _TintRimProgressCoord;
    float _TintRimSharpness;
    float _TintRimSharpnessCoord;
    float _InverseTintRim;

    float4 _FlowMap_ST;
    DECLARE_CUSTOM_COORD(_FlowMapOffsetXCoord);
    DECLARE_CUSTOM_COORD(_FlowMapOffsetYCoord);
    half _FlowMapChannelsX;
    half _FlowMapChannelsY;
    float _FlowIntensity;
    DECLARE_CUSTOM_COORD(_FlowIntensityCoord);

    float4 _AlphaTransitionMap_ST;
    float4 _AlphaTransitionMap2DArray_ST;
    float4 _AlphaTransitionMap3D_ST;
    float4 _AlphaTransitionMapSecondTexture_ST;
    float4 _AlphaTransitionMapSecondTexture2DArray_ST;
    float4 _AlphaTransitionMapSecondTexture3D_ST;

    float _AlphaTransitionMapProgress;
    DECLARE_CUSTOM_COORD(_AlphaTransitionMapProgressCoord);
    float _AlphaTransitionMapSecondTextureProgress;
    DECLARE_CUSTOM_COORD(_AlphaTransitionMapSecondTextureProgressCoord);
    float _AlphaTransitionMapSliceCount;
    float _AlphaTransitionMapSecondTextureSliceCount;
    DECLARE_CUSTOM_COORD(_AlphaTransitionMapOffsetXCoord);
    DECLARE_CUSTOM_COORD(_AlphaTransitionMapOffsetYCoord);
    DECLARE_CUSTOM_COORD(_AlphaTransitionMapSecondTextureOffsetXCoord);
    DECLARE_CUSTOM_COORD(_AlphaTransitionMapSecondTextureOffsetYCoord);
    half _AlphaTransitionMapChannelsX;
    float _DissolveSharpness;
    half _AlphaTransitionMapSecondTextureChannelsX;
    float _AlphaTransitionProgress;
    DECLARE_CUSTOM_COORD(_AlphaTransitionProgressCoord);
    float _AlphaTransitionProgressSecondTexture;
    DECLARE_CUSTOM_COORD(_AlphaTransitionProgressCoordSecondTexture);
    float _DissolveSharpnessSecondTexture;

    float4 _EmissionMap_ST;
    float4 _EmissionMap2DArray_ST;
    float4 _EmissionMap3D_ST;
    float _EmissionMapProgress;
    DECLARE_CUSTOM_COORD(_EmissionMapProgressCoord);
    float _EmissionMapSliceCount;
    DECLARE_CUSTOM_COORD(_EmissionMapOffsetXCoord);
    DECLARE_CUSTOM_COORD(_EmissionMapOffsetYCoord);
    half _EmissionMapChannelsX;

    float3 _EmissionColor;
    float _EmissionIntensity;
    DECLARE_CUSTOM_COORD(_EmissionIntensityCoord);
    float _KeepEdgeTransparency;

    float _Cutoff;
    float _Greyscale;
    float _InverseRimTransparency;
    float _RimTransparencyProgress;
    DECLARE_CUSTOM_COORD(_RimTransparencyProgressCoord);
    float _RimTransparencySharpness;
    DECLARE_CUSTOM_COORD(_RimTransparencySharpnessCoord);
    float _InverseLuminanceTransparency;
    float _LuminanceTransparencyProgress;
    DECLARE_CUSTOM_COORD(_LuminanceTransparencyProgressCoord);
    float _LuminanceTransparencySharpness;
    DECLARE_CUSTOM_COORD(_LuminanceTransparencySharpnessCoord);

    float4 _VertexDeformationMap_ST;
    DECLARE_CUSTOM_COORD(_VertexDeformationMapOffsetXCoord);
    DECLARE_CUSTOM_COORD(_VertexDeformationMapOffsetYCoord);
    half _VertexDeformationMapChannel;
    float _VertexDeformationIntensity;
    DECLARE_CUSTOM_COORD(_VertexDeformationIntensityCoord);
    float _VertexDeformationBaseValue;

    float _SoftParticlesIntensity;
    float _DepthFadeNear;
    float _DepthFadeFar;
    float _DepthFadeWidth;
    // Normal map
    half _NormalMapBumpScale;
    // Parallax Map
    half _ParallaxMapChannel;
    half _ParallaxStrength;
    float4 _ParallaxMap_ST;
    float4 _ParallaxMap2DArray_ST;
    float4 _ParallaxMap3D_ST;
    float _ParallaxMapProgress;
    DECLARE_CUSTOM_COORD(_ParallaxMapProgressCoord);
    float _ParallaxMapSliceCount;
    DECLARE_CUSTOM_COORD(_ParallaxMapOffsetXCoord);
    DECLARE_CUSTOM_COORD(_ParallaxMapOffsetYCoord);
    // Specular Map
    half4 _SpecularColor;
    // Metallic Map
    half _MetallicMapChannelsX;
    float _Metallic;
    // Smoothness Map
    half _SmoothnessMapChannelsX;
    float _Smoothness;
    // Shadow Caster
    half _ShadowCasterEnabled;
    half _ShadowCasterApplyVertexDeformation;
    half _ShadowCasterAlphaTestEnabled;
    half _ShadowCasterAlphaCutoff;
    half _ShadowCasterAlphaAffectedByTintColor;
    half _ShadowCasterAlphaAffectedByFlowMap;
    half _ShadowCasterAlphaAffectedByAlphaTransitionMap;
    half _ShadowCasterAlphaAffectedByTransparencyLuminance;

CBUFFER_END

#ifdef UNITY_UI_CLIP_RECT
    float4 _ClipRect;
    float _UIMaskSoftnessX;
    float _UIMaskSoftnessY;
#endif

// Returns the sampler state of the base map.
SamplerState GetBaseMapSamplerState()
{
    #ifdef BASE_SAMPLER_STATE_OVERRIDE_ENABLED
    return BASE_SAMPLER_STATE_NAME;
    #else
    #ifdef _BASE_MAP_MODE_2D
    return sampler_BaseMap;
    #elif _BASE_MAP_MODE_2D_ARRAY
    return sampler_BaseMap2DArray;
    #elif _BASE_MAP_MODE_3D
    return sampler_BaseMap3D;
    #endif
    #endif
}

SamplerState GetNormalMapSamplerState()
{
    #ifdef BASE_SAMPLER_STATE_OVERRIDE_ENABLED
    return BASE_SAMPLER_STATE_NAME;
    #else
    #ifdef _BASE_MAP_MODE_2D
    return sampler_NormalMap;
    #elif _BASE_MAP_MODE_2D_ARRAY
    return sampler_NormalMap2DArray;
    #elif _BASE_MAP_MODE_3D
    return sampler_NormalMap3D;
    #endif
    #endif
}

SamplerState GetParallaxMapSamplerState()
{
    #ifdef _PARALLAX_MAP_MODE_2D
    return sampler_ParallaxMap;
    #elif _PARALLAX_MAP_MODE_2D_ARRAY
    return sampler_ParallaxMap2DArray;
    #elif _PARALLAX_MAP_MODE_3D
    return sampler_ParallaxMap3D;
    #endif
}

SamplerState GetMetallicMapSamplerState()
{
    #ifdef BASE_SAMPLER_STATE_OVERRIDE_ENABLED
    return BASE_SAMPLER_STATE_NAME;
    #else
    #ifdef _BASE_MAP_MODE_2D
    return sampler_MetallicMap;
    #elif _BASE_MAP_MODE_2D_ARRAY
    return sampler_MetallicMap2DArray;
    #elif _BASE_MAP_MODE_3D
    return sampler_MetallicMap3D;
    #endif
    #endif
}

SamplerState GetSmoothnessMapSamplerState()
{
    #ifdef BASE_SAMPLER_STATE_OVERRIDE_ENABLED
    return BASE_SAMPLER_STATE_NAME;
    #else
    #ifdef _BASE_MAP_MODE_2D
    return sampler_SmoothnessMap;
    #elif _BASE_MAP_MODE_2D_ARRAY
    return sampler_SmoothnessMap2DArray;
    #elif _BASE_MAP_MODE_3D
    return sampler_SmoothnessMap3D;
    #endif
    #endif
}

SamplerState GetSpecularMapSamplerState()
{
    #ifdef BASE_SAMPLER_STATE_OVERRIDE_ENABLED
    return BASE_SAMPLER_STATE_NAME;
    #else
    #ifdef _BASE_MAP_MODE_2D
    return sampler_SpecularMap;
    #elif _BASE_MAP_MODE_2D_ARRAY
    return sampler_SpecularMap2DArray;
    #elif _BASE_MAP_MODE_3D
    return sampler_SpecularMap3D;
    #endif
    #endif
}

// Returns the sampler state of the alpha transition map.
SamplerState GetAlphaTransitionMapSamplerState()
{
    #ifdef _ALPHA_TRANSITION_MAP_MODE_2D
    return sampler_AlphaTransitionMap;
    #elif _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
    return sampler_AlphaTransitionMap2DArray;
    #elif _ALPHA_TRANSITION_MAP_MODE_3D
    return sampler_AlphaTransitionMap3D;
    #endif
}

// Returns the sampler state of the alpha emission map.
SamplerState GetEmissionMapSamplerState()
{
    #ifdef _EMISSION_MAP_MODE_2D
    return sampler_EmissionMap;
    #elif _EMISSION_MAP_MODE_2D_ARRAY
    return sampler_EmissionMap2DArray;
    #elif _EMISSION_MAP_MODE_3D
    return sampler_EmissionMap3D;
    #endif
}

// Transforms the base map UV by the scale/bias property
#ifdef _BASE_MAP_MODE_2D
#define TRANSFORM_BASE_MAP(texcoord) TRANSFORM_TEX(texcoord, _BaseMap);
#elif _BASE_MAP_MODE_2D_ARRAY
#define TRANSFORM_BASE_MAP(texcoord) TRANSFORM_TEX(texcoord, _BaseMap2DArray);
#elif _BASE_MAP_MODE_3D
#define TRANSFORM_BASE_MAP(texcoord) TRANSFORM_TEX(texcoord, _BaseMap3D);
#else
#define TRANSFORM_BASE_MAP(texcoord) texcoord
#endif

// Sample the base map.
#ifdef _BASE_MAP_MODE_2D
#define SAMPLE_BASE_MAP(uv, progress) SAMPLE_TEXTURE2D(_BaseMap, GetBaseMapSamplerState(), uv);
#elif _BASE_MAP_MODE_2D_ARRAY
#define SAMPLE_BASE_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_BaseMap2DArray, GetBaseMapSamplerState(), uv, progress);
#elif _BASE_MAP_MODE_3D
#define SAMPLE_BASE_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_BaseMap3D, GetBaseMapSamplerState(), float3(uv, progress), 0);
#else
#define SAMPLE_BASE_MAP(uv, progress) half4(0, 0, 0, 1)
#endif

// Transforms the tint map UV by the scale/bias property
#ifdef _TINT_MAP_ENABLED
#define TRANSFORM_TINT_MAP(texcoord) TRANSFORM_TEX(texcoord, _TintMap);
#elif _TINT_MAP_3D_ENABLED
#define TRANSFORM_TINT_MAP(texcoord) TRANSFORM_TEX(texcoord, _TintMap3D);
#endif

// Transforms the alpha transition map UV by the scale/bias property
#ifdef _ALPHA_TRANSITION_MAP_MODE_2D
#define TRANSFORM_ALPHA_TRANSITION_MAP(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMap);
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    #define TRANSFORM_ALPHA_TRANSITION_MAP_SECOND(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMapSecondTexture);
#endif
#elif _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
#define TRANSFORM_ALPHA_TRANSITION_MAP(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMap2DArray);
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    #define TRANSFORM_ALPHA_TRANSITION_MAP_SECOND(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMapSecondTexture2DArray);
#endif
#elif _ALPHA_TRANSITION_MAP_MODE_3D
#define TRANSFORM_ALPHA_TRANSITION_MAP(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMap3D);
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    #define TRANSFORM_ALPHA_TRANSITION_MAP_SECOND(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMapSecondTexture3D);
#endif
#endif

// Transforms the alpha transition map UV by the scale/bias property
#ifdef _EMISSION_MAP_MODE_2D
#define TRANSFORM_EMISSION_MAP(texcoord) TRANSFORM_TEX(texcoord, _EmissionMap);
#elif _EMISSION_MAP_MODE_2D_ARRAY
#define TRANSFORM_EMISSION_MAP(texcoord) TRANSFORM_TEX(texcoord, _EmissionMap2DArray);
#elif _EMISSION_MAP_MODE_3D
#define TRANSFORM_EMISSION_MAP(texcoord) TRANSFORM_TEX(texcoord, _EmissionMap3D);
#endif

// Sample the normal map.
#ifdef _BASE_MAP_MODE_2D
#define SAMPLE_NORMAL_MAP(uv, progress, scale) UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, GetNormalMapSamplerState(), uv), scale);
#elif _BASE_MAP_MODE_2D_ARRAY
#define SAMPLE_NORMAL_MAP(uv, progress, scale) UnpackNormalScale(SAMPLE_TEXTURE2D_ARRAY(_NormalMap2DArray, GetNormalMapSamplerState(), uv, progress), scale);
#elif _BASE_MAP_MODE_3D
#define SAMPLE_NORMAL_MAP(uv, progress, scale) UnpackNormalScale(SAMPLE_TEXTURE3D_LOD(_NormalMap3D, GetNormalMapSamplerState(), float3(uv, progress), 0), scale);
#endif

// Sample the parallax map.
#ifdef _PARALLAX_MAP_MODE_2D
#define SAMPLE_PARALLAX_MAP(uv, progress) SAMPLE_TEXTURE2D(_ParallaxMap, GetParallaxMapSamplerState(), uv);
#define TRANSFORM_PARALLAX_MAP(texcoord) TRANSFORM_TEX(texcoord, _ParallaxMap);
#elif _PARALLAX_MAP_MODE_2D_ARRAY
#define SAMPLE_PARALLAX_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_ParallaxMap2DArray, GetParallaxMapSamplerState(), uv, progress);
#define TRANSFORM_PARALLAX_MAP(texcoord) TRANSFORM_TEX(texcoord, _ParallaxMap2DArray);
#elif _PARALLAX_MAP_MODE_3D
#define SAMPLE_PARALLAX_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_ParallaxMap3D, GetParallaxMapSamplerState(), float3(uv, progress), 0);
#define TRANSFORM_PARALLAX_MAP(texcoord) TRANSFORM_TEX(texcoord, _ParallaxMap3D);
#endif

// Sample the metallic map.
#ifdef _BASE_MAP_MODE_2D
#define SAMPLE_METALLIC_MAP(uv, progress) SAMPLE_TEXTURE2D(_MetallicMap, GetMetallicMapSamplerState(), uv);
#elif _BASE_MAP_MODE_2D_ARRAY
#define SAMPLE_METALLIC_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_MetallicMap2DArray, GetMetallicMapSamplerState(), uv, progress);
#elif _BASE_MAP_MODE_3D
#define SAMPLE_METALLIC_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_MetallicMap3D, GetMetallicMapSamplerState(), float3(uv, progress), 0);
#endif

// Sample the smoothness map.
#ifdef _BASE_MAP_MODE_2D
#define SAMPLE_SMOOTHNESS_MAP(uv, progress) SAMPLE_TEXTURE2D(_SmoothnessMap, GetSmoothnessMapSamplerState(), uv);
#elif _BASE_MAP_MODE_2D_ARRAY
#define SAMPLE_SMOOTHNESS_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_SmoothnessMap2DArray, GetSmoothnessMapSamplerState(), uv, progress);
#elif _BASE_MAP_MODE_3D
#define SAMPLE_SMOOTHNESS_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_SmoothnessMap3D, GetSmoothnessMapSamplerState(), float3(uv, progress), 0);
#endif

// Sample the specular map.
#ifdef _BASE_MAP_MODE_2D
#define SAMPLE_SPECULAR_MAP(uv, progress) SAMPLE_TEXTURE2D(_SpecularMap, GetSpecularMapSamplerState(), uv);
#elif _BASE_MAP_MODE_2D_ARRAY
#define SAMPLE_SPECULAR_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_SpecularMap2DArray, GetSpecularMapSamplerState(), uv, progress);
#elif _BASE_MAP_MODE_3D
#define SAMPLE_SPECULAR_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_SpecularMap3D, GetSpecularMapSamplerState(), float3(uv, progress), 0);
#endif

// Returns the progress of the 2DArray/3d tint map.
half TintMapProgress(in half progress)
{
    half offset = 1.0 / _TintMapSliceCount * 0.5;
    progress += offset;
    return progress;
}

// Sample the tint map.
#ifdef _TINT_MAP_ENABLED
#define SAMPLE_TINT_MAP(uv, progress) SAMPLE_TEXTURE2D(_TintMap, sampler_TintMap, uv);
#elif _TINT_MAP_3D_ENABLED
#define SAMPLE_TINT_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_TintMap3D, sampler_TintMap3D, half3(uv, progress), 0);
#endif

// Apply the tint color.
inline void ApplyTintColor(in out half4 color, half2 uv, half progress, half blendRate)
{
    half4 tintColor;
    #ifdef _TINT_COLOR_ENABLED
    tintColor = _TintColor;
    #elif defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED)
    tintColor = SAMPLE_TINT_MAP(uv, progress);
    #endif
    color *= lerp(half4(1, 1, 1, 1), tintColor, saturate(blendRate));
}

// Apply the color correction.
void ApplyColorCorrection(in out float3 color)
{
    #ifdef _GREYSCALE_ENABLED
    color.rgb = GetLuminance(color.rgb);
    #elif _GRADIENT_MAP_ENABLED
    color.rgb = SAMPLE_TEXTURE2D(_GradientMap, sampler_GradientMap, half2(GetLuminance(color.rgb), 0.5)).rgb;
    #endif
}

// Sample the alpha transition map.
#ifdef _ALPHA_TRANSITION_MAP_MODE_2D
#define SAMPLE_ALPHA_TRANSITION_MAP(uv, progress) SAMPLE_TEXTURE2D(_AlphaTransitionMap, sampler_AlphaTransitionMap, uv);
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    #define SAMPLE_ALPHA_TRANSITION_MAP_SECOND(uv, progress) SAMPLE_TEXTURE2D(_AlphaTransitionMapSecondTexture, sampler_AlphaTransitionMapSecondTexture, uv);
#endif
#elif _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
#define SAMPLE_ALPHA_TRANSITION_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_AlphaTransitionMap2DArray, sampler_AlphaTransitionMap2DArray, uv, progress);
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    #define SAMPLE_ALPHA_TRANSITION_MAP_SECOND(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_AlphaTransitionMapSecondTexture2DArray, sampler_AlphaTransitionMapSecondTexture2DArray, uv, progress);
#endif
#elif _ALPHA_TRANSITION_MAP_MODE_3D
#define SAMPLE_ALPHA_TRANSITION_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_AlphaTransitionMap3D, sampler_AlphaTransitionMap3D, float3(uv, progress), 0);
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    #define SAMPLE_ALPHA_TRANSITION_MAP_SECOND(uv, progress) SAMPLE_TEXTURE3D_LOD(_AlphaTransitionMapSecondTexture3D, sampler_AlphaTransitionMapSecondTexture3D, float3(uv, progress), 0);
#endif
#endif

#ifndef SAMPLE_ALPHA_TRANSITION_MAP
#define SAMPLE_ALPHA_TRANSITION_MAP(uv, progress) half4(0, 0, 0, 1)
#endif

void ModulateAlphaTransitionProgress(in out half progress, half vertexAlpha)
{
    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    #ifdef _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
    progress += 1.0 - vertexAlpha;
    #endif
    progress = min(1.0, progress);
    #endif
}

half ApplyAlphaTransitionProgress(in out half transitionAlpha, half transitionProgress, float dissolveSharpness)
{
    #ifdef _FADE_TRANSITION_ENABLED
    transitionProgress = (transitionProgress * 2 - 1) * -1;
    transitionAlpha += transitionProgress;
    transitionAlpha = saturate(transitionAlpha);
    #elif _DISSOLVE_TRANSITION_ENABLED
    half dissolveWidth = lerp(0.5, 0.0001, dissolveSharpness);
    transitionProgress = lerp(-dissolveWidth, 1.0 + dissolveWidth, transitionProgress);
    transitionAlpha = smoothstep(transitionProgress - dissolveWidth, transitionProgress + dissolveWidth, transitionAlpha);
    #endif
    return transitionAlpha;
}

half GetTransitionAlphaImpl(half4 map, uint channel, half transitionProgress, float dissolveSharpness)
{
    half transitionAlpha = map[channel];
    return ApplyAlphaTransitionProgress(transitionAlpha, transitionProgress, dissolveSharpness);
}

#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
half GetTransitionAlpha(half2 transitionMapUv, half transitionMapProgress, half transitionProgress, half2 transitionMapSecondUv, half transitionMapProgressSecond, half transitionProgressSecond)
{
    half4 mainTexMap = SAMPLE_ALPHA_TRANSITION_MAP(transitionMapUv, transitionMapProgress);
    half4 secondTexMap = SAMPLE_ALPHA_TRANSITION_MAP_SECOND(transitionMapSecondUv, transitionMapProgressSecond);
    half mainTexAlpha = GetTransitionAlphaImpl(mainTexMap, (uint)_AlphaTransitionMapChannelsX, transitionProgress, _DissolveSharpness);
    half secondTexAlpha = GetTransitionAlphaImpl(secondTexMap, (uint)_AlphaTransitionMapSecondTextureChannelsX, transitionProgressSecond, _DissolveSharpnessSecondTexture);
    
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE)
    mainTexAlpha = (mainTexAlpha + secondTexAlpha) * 0.5;
#endif
#if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    mainTexAlpha = mainTexAlpha * secondTexAlpha;
#endif

    return mainTexAlpha;
}
#else
half GetTransitionAlpha(half2 transitionMapUv, half transitionMapProgress, half transitionProgress)
{
    half4 mainTexMap = SAMPLE_ALPHA_TRANSITION_MAP(transitionMapUv, transitionMapProgress);
    return GetTransitionAlphaImpl(mainTexMap, (uint)_AlphaTransitionMapChannelsX, transitionProgress,
                                  _DissolveSharpness);
}
#endif

// Apply the vertex color.
inline void ApplyVertexColor(in out half4 color, in half4 vertexColor)
{
    #ifdef _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
    color.rgb *= vertexColor.rgb;
    #else
    color *= vertexColor;
    #endif
}

// Sample the emission map.
#ifdef _EMISSION_MAP_MODE_2D
#define SAMPLE_EMISSION_MAP(uv, progress) SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv);
#elif _EMISSION_MAP_MODE_2D_ARRAY
#define SAMPLE_EMISSION_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_EmissionMap2DArray, sampler_EmissionMap2DArray, uv, progress);
#elif _EMISSION_MAP_MODE_3D
#define SAMPLE_EMISSION_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_EmissionMap3D, sampler_EmissionMap3D, float3(uv, progress), 0);
#endif

// Apply the emission color.
inline void ApplyEmissionColor(in out half4 color, half2 emissionMapUv, float intensity, half emissionMapProgress,
                               half emissionChannelsX)
{
    // Texture compression may introduce an error of 1/256, so it's advisable to allow for some margin
    const half tex_comp_err_margin = 0.004;

    half emissionIntensity = 0;
    half emissionColorRampU = 0;
    #ifdef _EMISSION_AREA_ALL
    emissionIntensity = 1;
    #elif _EMISSION_AREA_MAP
    half4 emissiomMap = SAMPLE_EMISSION_MAP(emissionMapUv, emissionMapProgress);
    half emissionMapValue = emissiomMap[emissionChannelsX];
    #if defined(_EMISSION_COLOR_COLOR) || defined(_EMISSION_COLOR_BASECOLOR)
    emissionIntensity = emissionMapValue;
    #elif _EMISSION_COLOR_MAP
    emissionIntensity = step(tex_comp_err_margin, emissionMapValue);
    emissionColorRampU = emissionMapValue;
    #endif
    #elif _EMISSION_AREA_ALPHA
    emissionIntensity = step(tex_comp_err_margin, 1.0 - color.a);
    emissionColorRampU = color.a;
    color.a = _KeepEdgeTransparency >= 0.5f ? color.a : step(tex_comp_err_margin, color.a);
    #endif

    half3 emissionColor = 0;
    #ifdef _EMISSION_COLOR_COLOR
    emissionColor = _EmissionColor;
    #elif _EMISSION_COLOR_BASECOLOR
    emissionColor = color.rgb;
    #elif _EMISSION_COLOR_MAP
    emissionColor = SAMPLE_TEXTURE2D(_EmissionColorRamp, sampler_EmissionColorRamp, half2(emissionColorRampU, 0.5)).rgb;
    #endif

    emissionIntensity *= intensity;
    color.rgb += emissionColor * emissionIntensity;
}

// Returns the value defined by rim.
inline float GetRimValue(half rim, half progress, half sharpness, half inverse)
{
    if (inverse >= 0.5)
    {
        rim = 1.0 - rim;
    }
    progress = min(1.0, progress);
    half width = 1.0 - min(1.0, sharpness);
    half start = lerp(-width, 1.0, progress);
    half end = lerp(0.0, 1.0 + width, progress);
    rim = smoothstep(start, end, rim);
    return rim;
}

inline void ApplyRimTransparency(in out half4 color, half rim, half progress, half sharpness)
{
    #ifdef _TRANSPARENCY_BY_RIM
    rim = GetRimValue(rim, progress, sharpness, _InverseRimTransparency);
    color.a *= rim;
    #endif
}

inline void ApplyLuminanceTransparency(in out half4 color, half progress, half sharpness)
{
    #ifdef _TRANSPARENCY_BY_LUMINANCE
    half luminance = GetLuminance(color.rgb);
    if (_InverseLuminanceTransparency >= 0.5)
    {
        luminance = 1.0 - luminance;
    }
    progress = min(1.0, progress);
    half width = 1.0 - min(1.0, sharpness);
    half start = lerp(-width, 1.0, progress);
    half end = lerp(0.0, 1.0 + width, progress);
    luminance = smoothstep(start, end, luminance);
    color.a *= luminance;
    #endif
}

inline void ApplySoftParticles(in out half4 color, float4 projection)
{
    #ifdef _SOFT_PARTICLES_ENABLED
    color.a *= SoftParticles(projection, _SoftParticlesIntensity);
    #endif
}

inline void ApplyDepthFade(in out half4 color, float4 projection)
{
    #ifdef _DEPTH_FADE_ENABLED
    color.a *= DepthFade(_DepthFadeNear, _DepthFadeFar, _DepthFadeWidth, projection);
    #endif
}

inline void CalculateTangetAndBinormalInWorldSpace(out float4 tangentWS, out float3 binormalWS, float3 normalWS,
                                                   float4 tangentOS)
{
    tangentWS.xyz = TransformObjectToWorldDir(tangentOS.xyz, true);
    tangentWS.w = tangentOS.w;
    binormalWS = cross(normalWS, tangentWS.xyz) * tangentOS.w;
}

/**
 * \brief Get normal in world space.
 * \param normalTS Normal in tangent space.
 * \param tangentWS Tangent in world space.
 * \param binormalWS Binormal in world space.
 * \param normalWSPerVertex Normal in world space per vertex.
 * \return The returned value is normal in world space per pixel.\n
 */
float3 GetNormalWS(float3 normalTS, float3 tangentWS, float3 binormalWS, float3 normalWSPerVertex)
{
    float3 normalWS;
    normalWS = TransformTangentToWorld(
        normalTS,
        half3x3(
            tangentWS.xyz,
            binormalWS.xyz,
            normalWSPerVertex.xyz));
    normalWS = NormalizeNormalPerPixel(normalWS);
    return normalWS;
}
#ifdef _NORMAL_MAP_ENABLED
#define GET_NORMAL_WS( normalTS, tangentWS, binormalWS, normalWSPerVertex ) GetNormalWS(normalTS, tangentWS, binormalWS, normalWSPerVertex );
#else
#define GET_NORMAL_WS( normalTS, tangentWS, binormalWS, normalWSPerVertex ) NormalizeNormalPerPixel(normalWSPerVertex.xyz);
#endif
#endif

// Parallax Map
#ifdef USE_PARALLAX_MAP
inline half2 ParallaxOffset(in half height, in half scale, in half3 viewDirTS)
{
    // 参考: URP公式視差メソッド ParallaxOffset1Step(height, scale, viewDirTS)
    half scaledHeight = -height * scale;
    half3 view = normalize(viewDirTS);
    view.z += 0.42;
    half2 offset = view.xy / view.z * scaledHeight;
    return offset;
}

inline half2 GetParallaxMappingUVOffset(in half2 uv, in half progress, in half channel, in half scale, in half3 viewDirTS)
{
    half4 map = SAMPLE_PARALLAX_MAP(uv, progress);
    half height = map[(int)channel];
    half2 offset = ParallaxOffset(height, scale, viewDirTS);
    return offset;
}
#endif
