#ifndef NOVA_PARTICLESDISTORTIONFORWARD_INCLUDED
#define NOVA_PARTICLESDISTORTIONFORWARD_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ParticlesDistortion.hlsl"

Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    SETUP_VERTEX;
    SETUP_CUSTOM_COORD(input)
    TRANSFER_CUSTOM_COORD(input, output);

    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    output.projectedPosition = ComputeScreenPos(output.positionHCS);

    float2 baseMapUv = input.texcoord.xy;
    if (_BaseMapRotation > 0.0 || _BaseMapRotationCoord > 0.0)
    {
        half angle = _BaseMapRotation + GET_CUSTOM_COORD(_BaseMapRotationCoord);
        baseMapUv = RotateUV(baseMapUv, angle * PI * 2, _BaseMapRotationOffsets.xy);
    }

    baseMapUv.xy = TRANSFORM_TEX(baseMapUv, _BaseMap);
    baseMapUv.x += GET_CUSTOM_COORD(_BaseMapOffsetXCoord);
    baseMapUv.y += GET_CUSTOM_COORD(_BaseMapOffsetYCoord);
    output.baseUv.xy = baseMapUv;

    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
    float2 flowMapUv = input.texcoord.xy;
    if (_FlowMapRotation > 0.0 || _FlowMapRotationCoord > 0.0)
    {
        half flowAngle = _FlowMapRotation + GET_CUSTOM_COORD(_FlowMapRotationCoord);
        flowMapUv = RotateUV(flowMapUv, flowAngle * PI * 2, _FlowMapRotationOffsets.xy);
    }
    flowMapUv = TRANSFORM_TEX(flowMapUv, _FlowMap);
    flowMapUv.x += GET_CUSTOM_COORD(_FlowMapOffsetXCoord);
    flowMapUv.y += GET_CUSTOM_COORD(_FlowMapOffsetYCoord);
    output.flowTransitionUVs.xy = flowMapUv;
    #endif

    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    float2 alphaTransitionMapUv = input.texcoord.xy;
    if (_AlphaTransitionMapRotation > 0.0 || _AlphaTransitionMapRotationCoord > 0.0)
    {
        half alphaAngle = _AlphaTransitionMapRotation + GET_CUSTOM_COORD(_AlphaTransitionMapRotationCoord);
        alphaTransitionMapUv = RotateUV(alphaTransitionMapUv, alphaAngle * PI * 2, _AlphaTransitionMapRotationOffsets.xy);
    }
    alphaTransitionMapUv = TRANSFORM_TEX(alphaTransitionMapUv, _AlphaTransitionMap);
    alphaTransitionMapUv.x += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetXCoord);
    alphaTransitionMapUv.y += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetYCoord);
    output.flowTransitionUVs.zw = alphaTransitionMapUv;
    #endif

    return output;
}

half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    SETUP_FRAGMENT;
    SETUP_CUSTOM_COORD(input);

    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
    half4 flowMapUvOffsetSrc = SAMPLE_TEXTURE2D(_FlowMap, sampler_FlowMap, input.flowTransitionUVs.xy); 
    half2 flowMapUvOffset;
    flowMapUvOffset.x = flowMapUvOffsetSrc[(uint)_FlowMapChannelsX];
    flowMapUvOffset.y = flowMapUvOffsetSrc[(uint)_FlowMapChannelsY];
    flowMapUvOffset = flowMapUvOffset * 2 - 1;
    flowMapUvOffset *= _FlowIntensity + GET_CUSTOM_COORD(_FlowIntensityCoord);
    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE)
    input.baseUv.xy += flowMapUvOffset;
    #endif
    #ifdef _FLOW_MAP_TARGET_ALPHA_TRANSITION
    input.flowTransitionUVs.zw += flowMapUvOffset;
    #endif
    #endif

    SamplerState baseMapSamplerState;
    #ifdef BASE_SAMPLER_STATE_OVERRIDE_ENABLED
    baseMapSamplerState = BASE_SAMPLER_STATE_NAME;
    #else
    baseMapSamplerState = sampler_BaseMap;
    #endif
    half4 distortionSrc = SAMPLE_TEXTURE2D(_BaseMap, baseMapSamplerState, input.baseUv.xy);
    if (_BaseMapUnpackNormal)
    {
        // [???] => [-1, 1]
        half3 unpackedNormal = UnpackNormal(distortionSrc);
        // [-1, 1] => [0, 1]
        distortionSrc = half4((unpackedNormal + 1) * 0.5f, 1);
    }

    half2 distortion;
    distortion.x = distortionSrc[(uint)_BaseMapChannelsX];
    distortion.y = distortionSrc[(uint)_BaseMapChannelsY];
    distortion = distortion * 2.0 - 1.0;

    half2 distortionIntensity;
    if (_DistortionIntensityMode > 0.5) // XY mode
    {
        distortionIntensity = half2(
            _DistortionIntensityX + GET_CUSTOM_COORD(_DistortionIntensityXCoord),
            _DistortionIntensityY + GET_CUSTOM_COORD(_DistortionIntensityYCoord)
        );
    }
    else // Single mode
    {
        half intensity = _DistortionIntensity + GET_CUSTOM_COORD(_DistortionIntensityCoord);
        distortionIntensity = half2(intensity, intensity);
    }
    distortion *= 0.1 * distortionIntensity;

    #if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
    half transitionAlpha = SAMPLE_TEXTURE2D(_AlphaTransitionMap, sampler_AlphaTransitionMap, input.flowTransitionUVs.zw)[_AlphaTransitionMapChannelsX];
    half progress = _AlphaTransitionProgress + GET_CUSTOM_COORD(_AlphaTransitionProgressCoord);
    #ifdef _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
    progress += 1.0 - input.color.a;
    #endif
    progress = min(1.0, progress);

    #ifdef _FADE_TRANSITION_ENABLED
    progress = (progress * 2 - 1) * -1;
    transitionAlpha += progress;
    transitionAlpha = saturate(transitionAlpha);
    #elif _DISSOLVE_TRANSITION_ENABLED
    half dissolveWidth = lerp(0.5, 0.0001, _DissolveSharpness);
    progress = lerp(-dissolveWidth, 1.0 + dissolveWidth, progress);
    transitionAlpha = smoothstep(progress - dissolveWidth, progress + dissolveWidth, transitionAlpha);
    #endif
    distortion *= transitionAlpha;
    #endif

    #ifdef _SOFT_PARTICLES_ENABLED
    distortion *= SoftParticles(input.projectedPosition, _SoftParticlesIntensity);
    #endif

    #ifdef _DEPTH_FADE_ENABLED
    distortion *= DepthFade(_DepthFadeNear, _DepthFadeFar, _DepthFadeWidth, input.projectedPosition);
    #endif

    return half4(distortion, 0, 1);
}

#endif
