#ifndef NOVA_PARTICLESUBERUNLIT_INCLUDED
#define NOVA_PARTICLESUBERUNLIT_INCLUDED

#include "ParticlesUber.hlsl"


struct Attributes
{
    float4 positionOS : POSITION;
    float4 color : COLOR;
    float3 normalOS : NORMAL;
    float2 texcoord : TEXCOORD0;
    #ifndef NOVA_PARTICLE_INSTANCING_ENABLED
    INPUT_CUSTOM_COORD(1, 2)
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float4 color : COLOR0;
    INPUT_CUSTOM_COORD(0, 1)
    float4 baseMapUVAndProgresses : TEXCOORD2; // xy: BaseMap UV, z: Base Map Progress, w: Tint Map Progress
    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_TINT) || defined(_FLOW_MAP_TARGET_EMISSION) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION) || defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    float4 flowTransitionUVs : TEXCOORD3; // xy: FlowMap UV, zw: TransitionMap UV
    #endif
    #if defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED) || defined(_EMISSION_AREA_MAP)
    float4 tintEmissionUV : TEXCOORD4; // xy: TintMap UV, zw: EmissionMap UV
    #endif
    float3 transitionEmissionProgresses : TEXCOORD5;
    // x: TransitionMap Progress, y: EmissionMap Progress, z: Fog Factor
    #ifdef FRAGMENT_USE_VIEW_DIR_WS
    float3 viewDirWS : TEXCOORD6;
    #endif
    #ifdef FRAGMENT_USE_NORMAL_WS
    float3 normalWS : TEXCOORD7;
    #endif
    #ifdef USE_PROJECTED_POSITION
    float4 projectedPosition : TEXCOORD8;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

inline void InitializeVertexOutput(in Attributes input, in out Varyings output, out float3 positionWS)
{
    positionWS = 0;
    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    output.color = GetParticleColor(input.color);
    #ifdef FRAGMENT_USE_VIEW_DIR_WS
    positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.viewDirWS = GetWorldSpaceViewDir(positionWS);
    #endif
    #ifdef FRAGMENT_USE_NORMAL_WS
    output.normalWS = TransformObjectToWorldNormal(input.normalOS, true);
    #endif

    #ifdef USE_PROJECTED_POSITION
    output.projectedPosition = ComputeScreenPos(output.positionHCS);
    #endif
}

inline void InitializeFragmentInput(in out Varyings input)
{
    #ifdef FRAGMENT_USE_NORMAL_WS
    input.normalWS = normalize(input.normalWS);
    #endif

    #ifdef FRAGMENT_USE_VIEW_DIR_WS
    input.viewDirWS = normalize(input.viewDirWS);
    #endif
}

Varyings vertUnlit(Attributes input, out float3 positionWS, uniform bool useEmission, uniform bool useFog)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    SETUP_VERTEX;
    SETUP_CUSTOM_COORD(input)
    TRANSFER_CUSTOM_COORD(input, output);

    InitializeVertexOutput(input, output, positionWS);

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
    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_TINT) || defined(_FLOW_MAP_TARGET_EMISSION) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
    output.flowTransitionUVs.xy = TRANSFORM_TEX(input.texcoord.xy, _FlowMap);
    output.flowTransitionUVs.x += GET_CUSTOM_COORD(_FlowMapOffsetXCoord);
    output.flowTransitionUVs.y += GET_CUSTOM_COORD(_FlowMapOffsetYCoord);
    #endif

    // Transition Map UV
    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    output.flowTransitionUVs.zw = TRANSFORM_ALPHA_TRANSITION_MAP(input.texcoord.xy);
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

    if (useEmission)
    {
        // Emission Map UV
        #ifdef _EMISSION_AREA_MAP
        output.tintEmissionUV.zw = TRANSFORM_EMISSION_MAP(input.texcoord.xy);
        output.tintEmissionUV.z += GET_CUSTOM_COORD(_EmissionMapOffsetXCoord)
        output.tintEmissionUV.w += GET_CUSTOM_COORD(_EmissionMapOffsetYCoord)
        #endif

        // Emission Map Progress
        #ifdef _EMISSION_MAP_MODE_2D_ARRAY
        float emissionMapProgress = _EmissionMapProgress + GET_CUSTOM_COORD(_EmissionMapProgressCoord);
        output.transitionEmissionProgresses.y = FlipBookProgress(emissionMapProgress, _EmissionMapSliceCount);
        #elif _EMISSION_MAP_MODE_3D
        float emissionMapProgress = _EmissionMapProgress + GET_CUSTOM_COORD(_EmissionMapProgressCoord);
        output.transitionEmissionProgresses.y = FlipBookBlendingProgress(emissionMapProgress, _EmissionMapSliceCount);
        #endif
    }
    if (useFog)
    {
        //Fog
        output.transitionEmissionProgresses.z = ComputeFogFactor(output.positionHCS.z);
    }
    return output;
}

half4 fragUnlit(in out Varyings input, uniform bool useEmission, uniform bool useFog)
{
    UNITY_SETUP_INSTANCE_ID(input);
    SETUP_FRAGMENT;
    SETUP_CUSTOM_COORD(input);
    InitializeFragmentInput(input);

    #if defined(_TRANSPARENCY_BY_RIM) || defined(_TINT_AREA_RIM)
    half rim = 1.0 - abs(dot(input.normalWS, input.viewDirWS));
    #endif

    // Flow Map
    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_TINT) || defined(_FLOW_MAP_TARGET_EMISSION) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
    half intensity = _FlowIntensity + GET_CUSTOM_COORD(_FlowIntensityCoord);
    half2 flowMapUvOffset = GetFlowMapUvOffset(_FlowMap, sampler_FlowMap, intensity, input.flowTransitionUVs.xy,
                                               _FlowMapChannelsX, _FlowMapChannelsY);
    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE)
    input.baseMapUVAndProgresses.xy += flowMapUvOffset;
    #endif

    #ifdef _FLOW_MAP_TARGET_TINT
    #if defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED)
    input.tintEmissionUV.xy += flowMapUvOffset;
    #endif
    #endif
    #if defined(_FLOW_MAP_TARGET_EMISSION) && defined(_EMISSION_AREA_MAP)
    input.tintEmissionUV.zw += flowMapUvOffset;
    #endif
    #ifdef _FLOW_MAP_TARGET_ALPHA_TRANSITION
    input.flowTransitionUVs.zw += flowMapUvOffset;
    #endif
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
    #if defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED)
    ApplyTintColor(color, input.tintEmissionUV.xy, input.baseMapUVAndProgresses.w, tintBlendRate);
    #else
    ApplyTintColor(color, half2( 0, 0 ), input.baseMapUVAndProgresses.w, tintBlendRate);
    #endif
    #endif

    // Color Correction
    ApplyColorCorrection(color.rgb);

    // Alpha Transition
    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    half alphaTransitionProgress = _AlphaTransitionProgress + GET_CUSTOM_COORD(_AlphaTransitionProgressCoord);
    ModulateAlphaTransitionProgress(alphaTransitionProgress, input.color.a);
    color.a *= GetTransitionAlpha(alphaTransitionProgress, input.flowTransitionUVs.zw, input.transitionEmissionProgresses.x, _AlphaTransitionMapChannelsX);
    #endif

    // Vertex Color
    ApplyVertexColor(color, input.color);

    // Emission
    if (useEmission)
    {
        half emissionIntensity = _EmissionIntensity + GET_CUSTOM_COORD(_EmissionIntensityCoord);
        #ifdef _EMISSION_AREA_MAP
        ApplyEmissionColor(color, input.tintEmissionUV.zw, emissionIntensity, input.transitionEmissionProgresses.y,
                           _EmissionMapChannelsX);
        #else
        ApplyEmissionColor(color, half2(0, 0), emissionIntensity, input.transitionEmissionProgresses.y,
                           _EmissionMapChannelsX);
        #endif
    }
    if (useFog)
    {
        // Fog
        half fogFactor = input.transitionEmissionProgresses.z;
        color.rgb = MixFog(color.rgb, fogFactor);
    }
    // Rim Transparency
    #ifdef _TRANSPARENCY_BY_RIM
    half rimTransparencyProgress = _RimTransparencyProgress + GET_CUSTOM_COORD(_RimTransparencyProgressCoord);
    half rimTransparencySharpness = _RimTransparencySharpness + GET_CUSTOM_COORD(_RimTransparencySharpnessCoord);
    ApplyRimTransparency(color, 1.0 - rim, rimTransparencyProgress, rimTransparencySharpness);
    #endif

    // Luminance Transparency
    #ifdef _TRANSPARENCY_BY_LUMINANCE
    half luminanceTransparencyProgress = _LuminanceTransparencyProgress + GET_CUSTOM_COORD(_LuminanceTransparencyProgressCoord);
    half luminanceTransparencySharpness = _LuminanceTransparencySharpness + GET_CUSTOM_COORD(_LuminanceTransparencySharpnessCoord);
    ApplyLuminanceTransparency(color, luminanceTransparencyProgress, luminanceTransparencySharpness);
    #endif

    // Soft Particle
    #ifdef _SOFT_PARTICLES_ENABLED
    ApplySoftParticles(color, input.projectedPosition);
    #endif

    // Depth Fade
    #ifdef _DEPTH_FADE_ENABLED
    ApplyDepthFade(color, input.projectedPosition);
    #endif

    AlphaClip(color.a, _Cutoff);
    color.rgb = ApplyAlpha(color.rgb, color.a);
    return color;
}
#endif