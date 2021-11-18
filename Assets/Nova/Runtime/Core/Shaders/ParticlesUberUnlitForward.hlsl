#ifndef CT_PARTICLESUBERUNLITFORWARD_INCLUDED
#define CT_PARTICLESUBERUNLITFORWARD_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ParticlesUberUnlit.hlsl"

Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    SETUP_VERTEX;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    SETUP_CUSTOM_COORD(input)
    TRANSFER_CUSTOM_COORD(input, output);
    InitializeVertexOutput(input, output);

    // Base Map UV
    float2 baseMapUv = input.texcoord.xy;
#ifdef _BASE_MAP_ROTATION_ENABLED
    half angle = _BaseMapRotation + GET_CUSTOM_COORD(_BaseMapRotationCoord)
    baseMapUv = RotateUV(baseMapUv, angle * PI * 2, _BaseMapRotationOffsets.xy);
#endif
    baseMapUv = TRANSFORM_BASE_MAP(baseMapUv);
    baseMapUv.x += GET_CUSTOM_COORD(_BaseMapOffsetXCoord);
    baseMapUv.y += GET_CUSTOM_COORD(_BaseMapOffsetYCoord);
    output.baseMapUVAndProgresses.xy = baseMapUv;

    // Base Map Progress
#ifdef _BASE_MAP_MODE_2D_ARRAY
    float baseMapProgress = _BaseMapProgress + GET_CUSTOM_COORD(_BaseMapProgressCoord);
    output.baseMapUVAndProgresses.z = FlipBookProgress(baseMapProgress, _BaseMapSliceCount);
#elif _BASE_MAP_MODE_3D
    float baseMapProgress = _BaseMapProgress + GET_CUSTOM_COORD(_BaseMapProgressCoord);
    output.baseMapUVAndProgresses.z = FlipBookBlendingProgress(baseMapProgress, _BaseMapSliceCount);
#endif

    // Tint Map UV
#if defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED)
    output.tintEmissionUV.xy = TRANSFORM_TINT_MAP(input.texcoord.xy);
#endif

    // Tint Map Progress
#ifdef _TINT_MAP_3D_ENABLED
    output.baseMapUVAndProgresses.w = _TintMap3DProgress + GET_CUSTOM_COORD(_TintMap3DProgressCoord);
    output.baseMapUVAndProgresses.w = TintMapProgress(output.baseMapUVAndProgresses.w);
#endif

    // Flow Map UV
#ifdef _FLOW_MAP_ENABLED
    output.flowTransitionUVs.xy = TRANSFORM_TEX(input.texcoord.xy, _FlowMap);
    output.flowTransitionUVs.x += GET_CUSTOM_COORD(_FlowMapOffsetXCoord);
    output.flowTransitionUVs.y += GET_CUSTOM_COORD(_FlowMapOffsetYCoord);
#endif

    // Transition Map UV
#if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    output.flowTransitionUVs.zw = TRANSFORM_TEX(input.texcoord.xy, _AlphaTransitionMap);
    output.flowTransitionUVs.z += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetXCoord)
    output.flowTransitionUVs.w += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetYCoord)
#endif
    
    // Transition Map Progress
#ifdef _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
    float transitionMapProgress = _AlphaTransitionMapProgress + GET_CUSTOM_COORD(_AlphaTransitionMapProgressCoord);
    output.transitionEmissionProgresses.x = FlipBookProgress(transitionMapProgress, _AlphaTransitionMapSliceCount);
#elif _ALPHA_TRANSITION_MAP_MODE_3D
    float transitionMapProgress = _AlphaTransitionMapProgress + GET_CUSTOM_COORD(_AlphaTransitionMapProgressCoord);
    output.transitionEmissionProgresses.x = FlipBookBlendingProgress(transitionMapProgress, _AlphaTransitionMapSliceCount);
#endif

    // Emission Map UV
#ifdef _EMISSION_AREA_MAP
    output.tintEmissionUV.zw = TRANSFORM_TEX(input.texcoord.xy, _EmissionMap);
#endif

    // Emission Map Progress
#ifdef _EMISSION_MAP_MODE_2D_ARRAY
    float emissionMapProgress = _EmissionMapProgress + GET_CUSTOM_COORD(_EmissionMapProgressCoord);
    output.transitionEmissionProgresses.y = FlipBookProgress(emissionMapProgress, _EmissionMapSliceCount);
#elif _EMISSION_MAP_MODE_3D
    float emissionMapProgress = _EmissionMapProgress + GET_CUSTOM_COORD(_EmissionMapProgressCoord);
    output.transitionEmissionProgresses.y = FlipBookBlendingProgress(emissionMapProgress, _EmissionMapSliceCount);
#endif
    
    //Fog
    output.transitionEmissionProgresses.z = ComputeFogFactor(output.positionHCS.z);
    
    return output;
}

half4 frag(Varyings input) : SV_Target
{
    SETUP_FRAGMENT;
    UNITY_SETUP_INSTANCE_ID(input);
    SETUP_CUSTOM_COORD(input);
    InitializeFragmentInput(input);

#if defined(_TRANSPARENCY_BY_RIM) || defined(_TINT_AREA_RIM)
    half rim = 1.0 - abs(dot(input.normalWS, input.viewDirWS));
#endif

    // Flow Map
#ifdef _FLOW_MAP_ENABLED
    half intensity = _FlowIntensity + GET_CUSTOM_COORD(_FlowIntensityCoord);
    ApplyFlowMap(_FlowMap, sampler_FlowMap, intensity, input.flowTransitionUVs.xy, input.baseMapUVAndProgresses.xy);
#endif

    // Base Color
    half4 color = SAMPLE_BASE_MAP(input.baseMapUVAndProgresses.xy, input.baseMapUVAndProgresses.z);

    // Tint Color
#if defined(_TINT_AREA_ALL) || defined(_TINT_AREA_RIM)
    half tintBlendRate = _TintBlendRate + GET_CUSTOM_COORD(_TintBlendRateCoord);
#ifdef _TINT_AREA_RIM
    half tintRimProgress = _TintRimProgress + GET_CUSTOM_COORD(_TintRimProgressCoord);
    half tintRimSharpness = _TintRimSharpness + GET_CUSTOM_COORD(_TintRimSharpnessCoord);
    rim = GetRimValue(rim, tintRimProgress, tintRimSharpness, _InverseTintRim);
    tintBlendRate *= _TintBlendRate * rim;
#endif
    ApplyTintColor(color, input.tintEmissionUV.xy, input.baseMapUVAndProgresses.w, tintBlendRate);
#endif
    
    // Color Correction
    ApplyColorCorrection(color.rgb);
    
    // Alpha Transition
#if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    half alphaTransitionProgress = _AlphaTransitionProgress + GET_CUSTOM_COORD(_AlphaTransitionProgressCoord);
    ModulateAlphaTransitionProgress(alphaTransitionProgress, input.color.a);
    color.a *= GetTransitionAlpha(alphaTransitionProgress, input.flowTransitionUVs.zw, input.transitionEmissionProgresses.x);
#endif
    
    // Vertex Color
    ApplyVertexColor(color, input.color);

    // Emission
    half emissionIntensity = _EmissionIntensity + GET_CUSTOM_COORD(_EmissionIntensityCoord);
    ApplyEmissionColor(color, input.tintEmissionUV.zw, emissionIntensity, input.transitionEmissionProgresses.y);

    // Fog
    half fogFactor = input.transitionEmissionProgresses.z;
    color.rgb = MixFog(color.rgb, fogFactor);
    
    // Rim Transparency
#if _TRANSPARENCY_BY_RIM
    half rimTransparencyProgress = _RimTransparencyProgress + GET_CUSTOM_COORD(_RimTransparencyProgressCoord);
    half rimTransparencySharpness = _RimTransparencySharpness + GET_CUSTOM_COORD(_RimTransparencySharpnessCoord);
    ApplyRimTransparency(color, 1.0 - rim, rimTransparencyProgress, rimTransparencySharpness);
#endif

    // Luminance Transparency
#if _TRANSPARENCY_BY_LUMINANCE
    half luminanceTransparencyProgress = _LuminanceTransparencyProgress + GET_CUSTOM_COORD(_LuminanceTransparencyProgressCoord);
    half luminanceTransparencySharpness = _LuminanceTransparencySharpness + GET_CUSTOM_COORD(_LuminanceTransparencySharpnessCoord);
    ApplyLuminanceTransparency(color, luminanceTransparencyProgress, luminanceTransparencySharpness);
#endif

    // Soft Particle
#if _SOFT_PARTICLES_ENABLED
    ApplySoftParticles(color, input.projectedPosition);
#endif

    // Depth Fade
#if _DEPTH_FADE_ENABLED
    ApplyDepthFade(color, input.projectedPosition);
#endif
    
    color.rgb = AlphaModulate(color.rgb, color.a);
    AlphaClip(color.a, _Cutoff);
    return color;
}

#endif