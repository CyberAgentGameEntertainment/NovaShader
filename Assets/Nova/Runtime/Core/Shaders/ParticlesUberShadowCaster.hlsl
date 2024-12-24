#ifndef NOVA_PARTICLESUBER_SHADOW_CASTER_INCLUDED
#define NOVA_PARTICLESUBER_SHADOW_CASTER_INCLUDED

#include "ParticlesUber.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

// Shadow Casting Light geometric parameters. These variables are used when applying the shadow Normal Bias and are set by UnityEngine.Rendering.Universal.ShadowUtils.SetupShadowCasterConstantBuffer in com.unity.render-pipelines.universal/Runtime/ShadowUtils.cs
// For Directional lights, _LightDirection is used when applying shadow Normal Bias.
// For Spot lights and Point lights, _LightPosition is used to compute the actual light direction because it is different at each shadow caster geometry vertex.
float3 _LightDirection;
float3 _LightPosition;

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
    #if defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED)
    float2 tintUV : TEXCOORD4; // xy: TintMap UV, zw: EmissionMap UV
    #endif
    float transitionProgress : TEXCOORD5;
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    float4 flowTransitionSecondUVs : TEXCOORD6; // xy: FlowMap UV, zw: TransitionMap UV
    float transitionProgressSecond : TEXCOORD7;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

float4 GetShadowPositionHClip(Attributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

    #if _CASTING_PUNCTUAL_LIGHT_SHADOW
    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
    #else
    float3 lightDirectionWS = _LightDirection;
    #endif

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

    #if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else
    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #endif

    return positionCS;
}

Varyings ShadowPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    SETUP_VERTEX;
    SETUP_CUSTOM_COORD(input)
    TRANSFER_CUSTOM_COORD(input, output);

    // Vertex Deformation
    #ifdef _VERTEX_DEFORMATION_ENABLED
    if (_ShadowCasterApplyVertexDeformation)
    {
        float2 vertexDeformationUVs = TRANSFORM_TEX(input.texcoord.xy, _VertexDeformationMap);
        vertexDeformationUVs.x += GET_CUSTOM_COORD(_VertexDeformationMapOffsetXCoord);
        vertexDeformationUVs.y += GET_CUSTOM_COORD(_VertexDeformationMapOffsetYCoord);
        float vertexDeformationIntensity = _VertexDeformationIntensity + GET_CUSTOM_COORD(_VertexDeformationIntensityCoord);
        vertexDeformationIntensity = GetVertexDeformationIntensity(
            _VertexDeformationMap, sampler_VertexDeformationMap,
            vertexDeformationIntensity,
            vertexDeformationUVs,
            _VertexDeformationMapChannel,
            _VertexDeformationBaseValue);
        input.positionOS.xyz += normalize(input.normalOS) * vertexDeformationIntensity;
    }
    #endif

    output.positionHCS = GetShadowPositionHClip(input);

    #ifdef _SHADOW_CASTER_ALPHA_TEST_ENABLED
    output.color = GetParticleColor(input.color);

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
    output.tintUV = TRANSFORM_TINT_MAP(input.texcoord.xy);
    output.tintUV.x += GET_CUSTOM_COORD(_TintMapOffsetXCoord);
    output.tintUV.y += GET_CUSTOM_COORD(_TintMapOffsetYCoord);
    #endif

    // Tint Map Progress
    #ifdef _TINT_MAP_3D_ENABLED
    output.baseMapUVAndProgresses.w = _TintMap3DProgress + GET_CUSTOM_COORD(_TintMap3DProgressCoord);
    output.baseMapUVAndProgresses.w = TintMapProgress(output.baseMapUVAndProgresses.w);
    #endif

    // Flow Map UV
    #if defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_TINT) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
    output.flowTransitionUVs.xy = TRANSFORM_TEX(input.texcoord.xy, _FlowMap);
    output.flowTransitionUVs.x += GET_CUSTOM_COORD(_FlowMapOffsetXCoord);
    output.flowTransitionUVs.y += GET_CUSTOM_COORD(_FlowMapOffsetYCoord);
    #endif

    // Transition Map UV
    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    output.flowTransitionUVs.zw = TRANSFORM_ALPHA_TRANSITION_MAP(input.texcoord.xy);
    output.flowTransitionUVs.z += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetXCoord)
    output.flowTransitionUVs.w += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetYCoord)
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    output.flowTransitionSecondUVs.zw = TRANSFORM_ALPHA_TRANSITION_MAP_SECOND(input.texcoord.xy);
    output.flowTransitionSecondUVs.z += GET_CUSTOM_COORD(_AlphaTransitionMapSecondTextureOffsetXCoord)
    output.flowTransitionSecondUVs.w += GET_CUSTOM_COORD(_AlphaTransitionMapSecondTextureOffsetYCoord)
    #endif
    #endif

    // Transition Map Progress
    #if defined(_ALPHA_TRANSITION_MAP_MODE_2D_ARRAY) || defined(_ALPHA_TRANSITION_MAP_MODE_3D)
    float transitionMapProgress = _AlphaTransitionMapProgress + GET_CUSTOM_COORD(_AlphaTransitionMapProgressCoord);
    float sliceCount = _AlphaTransitionMapSliceCount;
    #ifdef _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
    output.transitionProgress = FlipBookProgress(transitionMapProgress, sliceCount);
    #elif _ALPHA_TRANSITION_MAP_MODE_3D
    output.transitionProgress = FlipBookBlendingProgress(transitionMapProgress, sliceCount);
    #endif

    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    float transitionMapProgressSecond = _AlphaTransitionMapSecondTextureProgress + GET_CUSTOM_COORD(_AlphaTransitionMapSecondTextureProgressCoord);
    float sliceCountSecond = _AlphaTransitionMapSecondTextureSliceCount;
    #ifdef _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
    output.transitionProgressSecond = FlipBookProgress(transitionMapProgressSecond, sliceCountSecond);
    #elif _ALPHA_TRANSITION_MAP_MODE_3D
    output.transitionProgressSecond = FlipBookBlendingProgress(transitionMapProgressSecond, sliceCountSecond);
    #endif
    #endif
    #endif
    #endif
    return output;
}

half4 ShadowPassFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    SETUP_FRAGMENT;
    SETUP_CUSTOM_COORD(input);

    #ifdef _SHADOW_CASTER_ALPHA_TEST_ENABLED
    // Flow Map
    #if defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_TINT) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
    if (_ShadowCasterAlphaAffectedByFlowMap)
    {
        half intensity = _FlowIntensity + GET_CUSTOM_COORD(_FlowIntensityCoord);
        half2 flowMapUvOffset = GetFlowMapUvOffset(_FlowMap, sampler_FlowMap, intensity, input.flowTransitionUVs.xy,
                                                   _FlowMapChannelsX, _FlowMapChannelsY);
    #if defined(_FLOW_MAP_TARGET_BASE)
        input.baseMapUVAndProgresses.xy += flowMapUvOffset;
    #endif

    #ifdef _FLOW_MAP_TARGET_TINT
    #if defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED)
        input.tintUV += flowMapUvOffset;
    #endif
    #endif
    #ifdef _FLOW_MAP_TARGET_ALPHA_TRANSITION
        input.flowTransitionUVs.zw += flowMapUvOffset;
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
        input.flowTransitionSecondUVs.zw += flowMapUvOffset;
    #endif
    #endif
    }
    #endif

    // Base Color
    half4 color = SAMPLE_BASE_MAP(input.baseMapUVAndProgresses.xy, input.baseMapUVAndProgresses.z);

    // Tint Color
    #if defined(_TINT_AREA_ALL)
    if (_ShadowCasterAlphaAffectedByTintColor)
    {
        half tintBlendRate = _TintBlendRate + GET_CUSTOM_COORD(_TintBlendRateCoord);
    #if defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED)
        ApplyTintColor(color, input.tintUV, input.baseMapUVAndProgresses.w, tintBlendRate);
    #else
        ApplyTintColor(color, half2( 0, 0 ), input.baseMapUVAndProgresses.w, tintBlendRate);
    #endif
    }
    #endif

    // Alpha Transition
    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    if (_ShadowCasterAlphaAffectedByAlphaTransitionMap)
    {
        half alphaTransitionProgress = _AlphaTransitionProgress + GET_CUSTOM_COORD(_AlphaTransitionProgressCoord);
        ModulateAlphaTransitionProgress(alphaTransitionProgress, input.color.a);
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
        half alphaTransitionProgressSecondTexture = _AlphaTransitionProgressSecondTexture + GET_CUSTOM_COORD(_AlphaTransitionProgressCoordSecondTexture);
        ModulateAlphaTransitionProgress(alphaTransitionProgressSecondTexture, input.color.a);
        color.a *= GetTransitionAlpha(input.flowTransitionUVs.zw, input.transitionProgress, alphaTransitionProgress, input.flowTransitionSecondUVs.zw, input.transitionProgressSecond, alphaTransitionProgressSecondTexture);
    #else
        color.a *= GetTransitionAlpha(input.flowTransitionUVs.zw, input.transitionProgress, alphaTransitionProgress);
    #endif
    }
    #endif

    // Vertex Color
    ApplyVertexColor(color, input.color);

    // Luminance Transparency
    #ifdef _TRANSPARENCY_BY_LUMINANCE
    if (_ShadowCasterAlphaAffectedByTransparencyLuminance)
    {
        half luminanceTransparencyProgress = _LuminanceTransparencyProgress + GET_CUSTOM_COORD(_LuminanceTransparencyProgressCoord);
        half luminanceTransparencySharpness = _LuminanceTransparencySharpness + GET_CUSTOM_COORD(_LuminanceTransparencySharpnessCoord);
        ApplyLuminanceTransparency(color, luminanceTransparencyProgress, luminanceTransparencySharpness);
    }
    #endif

    clip(color.a - _ShadowCasterAlphaCutoff);
    #endif

    return 0;
}

#endif
