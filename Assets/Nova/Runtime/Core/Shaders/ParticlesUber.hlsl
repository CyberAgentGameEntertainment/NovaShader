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
TEXTURE2D_ARRAY(_AlphaTransitionMap2DArray);
SAMPLER(sampler_AlphaTransitionMap2DArray);
TEXTURE3D(_AlphaTransitionMap3D);
SAMPLER(sampler_AlphaTransitionMap3D);
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
DECLARE_CUSTOM_COORD(_FlowMapChannelsX);
DECLARE_CUSTOM_COORD(_FlowMapChannelsY);
float _FlowIntensity;
DECLARE_CUSTOM_COORD(_FlowIntensityCoord);

float4 _AlphaTransitionMap_ST;
float4 _AlphaTransitionMap2DArray_ST;
float4 _AlphaTransitionMap3D_ST;
float _AlphaTransitionMapProgress;
DECLARE_CUSTOM_COORD(_AlphaTransitionMapProgressCoord);
float _AlphaTransitionMapSliceCount;
DECLARE_CUSTOM_COORD(_AlphaTransitionMapOffsetXCoord);
DECLARE_CUSTOM_COORD(_AlphaTransitionMapOffsetYCoord);
DECLARE_CUSTOM_COORD(_AlphaTransitionMapChannelsX);
float _AlphaTransitionProgress;
DECLARE_CUSTOM_COORD(_AlphaTransitionProgressCoord);
float _DissolveSharpness;

float4 _EmissionMap_ST;
float4 _EmissionMap2DArray_ST;
float4 _EmissionMap3D_ST;
float _EmissionMapProgress;
DECLARE_CUSTOM_COORD(_EmissionMapProgressCoord);
float _EmissionMapSliceCount;
DECLARE_CUSTOM_COORD(_EmissionMapOffsetXCoord);
DECLARE_CUSTOM_COORD(_EmissionMapOffsetYCoord);
DECLARE_CUSTOM_COORD(_EmissionMapChannelsX);

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

float _SoftParticlesIntensity;
float _DepthFadeNear;
float _DepthFadeFar;
float _DepthFadeWidth;

#ifdef _LIT_ENABLED

float _WorkflowMode;
float _ReceiveShadows;

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
#endif

// Sample the base map.
#ifdef _BASE_MAP_MODE_2D
#define SAMPLE_BASE_MAP(uv, progress) SAMPLE_TEXTURE2D(_BaseMap, GetBaseMapSamplerState(), uv);
#elif _BASE_MAP_MODE_2D_ARRAY
#define SAMPLE_BASE_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_BaseMap2DArray, GetBaseMapSamplerState(), uv, progress);
#elif _BASE_MAP_MODE_3D
#define SAMPLE_BASE_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_BaseMap3D, GetBaseMapSamplerState(), float3(uv, progress), 0);
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
#elif _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
#define TRANSFORM_ALPHA_TRANSITION_MAP(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMap2DArray);
#elif _ALPHA_TRANSITION_MAP_MODE_3D
#define TRANSFORM_ALPHA_TRANSITION_MAP(texcoord) TRANSFORM_TEX(texcoord, _AlphaTransitionMap3D);
#endif

// Transforms the alpha transition map UV by the scale/bias property
#ifdef _EMISSION_MAP_MODE_2D
#define TRANSFORM_EMISSION_MAP(texcoord) TRANSFORM_TEX(texcoord, _EmissionMap);
#elif _EMISSION_MAP_MODE_2D_ARRAY
#define TRANSFORM_EMISSION_MAP(texcoord) TRANSFORM_TEX(texcoord, _EmissionMap2DArray);
#elif _EMISSION_MAP_MODE_3D
#define TRANSFORM_EMISSION_MAP(texcoord) TRANSFORM_TEX(texcoord, _EmissionMap3D);
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
    color *= lerp(1, tintColor, saturate(blendRate));
}

// Apply the color correction.
void ApplyColorCorrection(in out float3 color)
{
    #if _GREYSCALE_ENABLED
    color.rgb = Luminance(color.rgb);
    #elif _GRADIENT_MAP_ENABLED
    color.rgb = SAMPLE_TEXTURE2D(_GradientMap, sampler_GradientMap, half2(Luminance(color.rgb), 0.5)).rgb;
    #endif
}

// Sample the alpha transition map.
#ifdef _ALPHA_TRANSITION_MAP_MODE_2D
#define SAMPLE_ALPHA_TRANSITION_MAP(uv, progress) SAMPLE_TEXTURE2D(_AlphaTransitionMap, sampler_AlphaTransitionMap, uv);
#elif _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
#define SAMPLE_ALPHA_TRANSITION_MAP(uv, progress) SAMPLE_TEXTURE2D_ARRAY(_AlphaTransitionMap2DArray, sampler_AlphaTransitionMap2DArray, uv, progress);
#elif _ALPHA_TRANSITION_MAP_MODE_3D
#define SAMPLE_ALPHA_TRANSITION_MAP(uv, progress) SAMPLE_TEXTURE3D_LOD(_AlphaTransitionMap3D, sampler_AlphaTransitionMap3D, float3(uv, progress), 0);
#endif

void ModulateAlphaTransitionProgress(in out half progress, half vertexAlpha)
{
    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    #if _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
    progress += 1.0 - vertexAlpha;
    #endif
    progress = min(1.0, progress);
    #endif
}

// Returns alpha value by the alpha transition.
half GetTransitionAlpha(half transitionProgress, half2 transitionMapUv, half transitionMapProgress, half transitionMapChannelsX)
{
    half4 map = SAMPLE_ALPHA_TRANSITION_MAP(transitionMapUv, transitionMapProgress);
    half transitionAlpha = map[(uint)transitionMapChannelsX];
    #ifdef _FADE_TRANSITION_ENABLED
    transitionProgress = (transitionProgress * 2 - 1) * -1;
    transitionAlpha += transitionProgress;
    transitionAlpha = saturate(transitionAlpha);
    #elif _DISSOLVE_TRANSITION_ENABLED
    half dissolveWidth = lerp(0.5, 0.0001, _DissolveSharpness);
    transitionProgress = lerp(-dissolveWidth, 1.0 + dissolveWidth, transitionProgress);
    transitionAlpha = smoothstep(transitionProgress - dissolveWidth, transitionProgress + dissolveWidth, transitionAlpha);
    #endif
    return transitionAlpha;
}

// Apply the vertex color.
inline void ApplyVertexColor(in out half4 color, in half4 vertexColor)
{
    #if _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
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
inline void ApplyEmissionColor(in out half4 color, half2 emissionMapUv, float intensity, half emissionMapProgress, half emissionChannelsX)
{
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
    emissionIntensity = step(0.0001, emissionMapValue);
    emissionColorRampU = emissionMapValue;
    #endif
    #elif _EMISSION_AREA_ALPHA
    emissionIntensity = step(0.0001, 1.0 - color.a);
    emissionColorRampU = color.a;
    color.a = _KeepEdgeTransparency >= 0.5f ? color.a : step(0.0001, color.a);
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
    #if _TRANSPARENCY_BY_RIM
    rim = GetRimValue(rim, progress, sharpness, _InverseRimTransparency);
    color.a *= rim;
    #endif
}

inline void ApplyLuminanceTransparency(in out half4 color, half progress, half sharpness)
{
    #if _TRANSPARENCY_BY_LUMINANCE
    half luminance = Luminance(color.rgb);
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

#endif
