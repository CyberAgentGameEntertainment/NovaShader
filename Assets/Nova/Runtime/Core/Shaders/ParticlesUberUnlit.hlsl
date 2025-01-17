#ifndef NOVA_PARTICLESUBERUNLIT_INCLUDED
#define NOVA_PARTICLESUBERUNLIT_INCLUDED

#include "ParticlesUber.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float4 color : COLOR;
    float3 normalOS : NORMAL;
    #ifdef USE_PARALLAX_MAP
    float4 tangentOS : TANGENT;
    #endif
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
    #ifdef USE_PARALLAX_MAP
    float3 viewDirTS : TEXCOORD9;
    float3 parallaxMapUVAndProgress : TEXCOORD10;
    #endif
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    float4 flowTransitionSecondUVs : TEXCOORD11; // xy: FlowMap UV, zw: TransitionMap UV
    float2 transitionEmissionProgressesSecond : TEXCOORD12; // x: TransitionMap Progress, y: EmissionMap Progress
    #endif

    #ifdef UNITY_UI_CLIP_RECT
    float4 mask : TEXCOORD13;
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
    #ifdef USE_PARALLAX_MAP
    half4 tangentWS = 0;
    tangentWS.xyz = TransformObjectToWorldDir(input.tangentOS.xyz);
    tangentWS.w = input.tangentOS.w * GetOddNegativeScale();
    output.viewDirTS = GetViewDirectionTangentSpace(tangentWS, output.normalWS, output.viewDirWS);
    #endif

    #ifdef UNITY_UI_CLIP_RECT
    float2 pixelSize = output.positionHCS.w;
    pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
    output.mask = float4(input.positionOS.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));
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
    #ifdef USE_PARALLAX_MAP
    input.viewDirTS = normalize(input.viewDirTS);
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

    // Vertex Deformation
    #ifdef _VERTEX_DEFORMATION_ENABLED
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
    #endif

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
    output.tintEmissionUV.x += GET_CUSTOM_COORD(_TintMapOffsetXCoord);
    output.tintEmissionUV.y += GET_CUSTOM_COORD(_TintMapOffsetYCoord);
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

    // Parallax Map UV
    #if defined(USE_PARALLAX_MAP)
    output.parallaxMapUVAndProgress.xy = TRANSFORM_PARALLAX_MAP(input.texcoord.xy)
    output.parallaxMapUVAndProgress.x += GET_CUSTOM_COORD(_ParallaxMapOffsetXCoord);
    output.parallaxMapUVAndProgress.y += GET_CUSTOM_COORD(_ParallaxMapOffsetYCoord);
    float parallaxMapProgress = _ParallaxMapProgress + GET_CUSTOM_COORD(_ParallaxMapProgressCoord);
    output.parallaxMapUVAndProgress.z = FlipBookProgress(parallaxMapProgress, _ParallaxMapSliceCount);
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
    output.transitionEmissionProgresses.x = FlipBookProgress(transitionMapProgress, sliceCount);
    #elif _ALPHA_TRANSITION_MAP_MODE_3D
    output.transitionEmissionProgresses.x = FlipBookBlendingProgress(transitionMapProgress, sliceCount);
    #endif

    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    float transitionMapProgressSecond = _AlphaTransitionMapSecondTextureProgress + GET_CUSTOM_COORD(_AlphaTransitionMapSecondTextureProgressCoord);
    float sliceCountSecond = _AlphaTransitionMapSecondTextureSliceCount;
    #ifdef _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
    output.transitionEmissionProgressesSecond.x = FlipBookProgress(transitionMapProgressSecond, sliceCountSecond);
    #elif _ALPHA_TRANSITION_MAP_MODE_3D
    output.transitionEmissionProgressesSecond.x = FlipBookBlendingProgress(transitionMapProgressSecond, sliceCountSecond);
    #endif
    #endif
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

half4 fragUnlit(in out Varyings input, uniform bool useEmission)
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
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    input.flowTransitionSecondUVs.zw += flowMapUvOffset;
    #endif
    #endif
    #endif

    #ifdef USE_PARALLAX_MAP
    // Remap _ParallaxStrength to the valid range of parallax scale
    // The valid range for parallax scale is usually between 0 and 0.1
    half parallaxScale = lerp(0, 0.1, _ParallaxStrength);
    half2 parallaxOffset = GetParallaxMappingUVOffset(input.parallaxMapUVAndProgress.xy, input.parallaxMapUVAndProgress.z, _ParallaxMapChannel, parallaxScale, input.viewDirTS);

    #if defined(_PARALLAX_MAP_TARGET_BASE)
    input.baseMapUVAndProgresses.xy += parallaxOffset;
    #endif

    #if defined(_PARALLAX_MAP_TARGET_TINT) && (defined(_TINT_MAP_ENABLED) || defined(_TINT_MAP_3D_ENABLED))
    input.tintEmissionUV.xy += parallaxOffset;
    #endif

    #if defined(_PARALLAX_MAP_TARGET_EMISSION) && defined(_EMISSION_AREA_MAP)
    input.tintEmissionUV.zw += parallaxOffset;
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
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    half alphaTransitionProgressSecondTexture = _AlphaTransitionProgressSecondTexture + GET_CUSTOM_COORD(_AlphaTransitionProgressCoordSecondTexture);
    ModulateAlphaTransitionProgress(alphaTransitionProgressSecondTexture, input.color.a);
    color.a *= GetTransitionAlpha(input.flowTransitionUVs.zw, input.transitionEmissionProgresses.x, alphaTransitionProgress, input.flowTransitionSecondUVs.zw, input.transitionEmissionProgressesSecond.x, alphaTransitionProgressSecondTexture);
    #else
    color.a *= GetTransitionAlpha(input.flowTransitionUVs.zw, input.transitionEmissionProgresses.x, alphaTransitionProgress);
    #endif
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


    #ifdef UNITY_UI_CLIP_RECT
    half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
    color.a *= m.x * m.y;  
    #endif

    AlphaClip(color.a, _Cutoff);
    color.rgb = ApplyAlpha(color.rgb, color.a);

    return color;
}
#endif
