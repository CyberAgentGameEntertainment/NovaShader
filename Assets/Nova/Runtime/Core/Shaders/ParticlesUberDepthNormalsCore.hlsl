/**
* \brief This header include common processing of DepthOnly and DepthNormals pass.
*/
#ifndef NOVA_PARTICLESUBERDEPTHNORMALSCORE_INCLUDED
#define NOVA_PARTICLESUBERDEPTHNORMALSCORE_INCLUDED


// If defined _ALPHATEST_ENABLED or  _NORMAL_MAP_ENABLED, base map uv is enabled.
#if defined( _ALPHATEST_ENABLED ) || defined(_NORMAL_MAP_ENABLED)
#define _USE_BASE_MAP_UV
#endif

#if !defined( _ALPHATEST_ENABLED )
#undef _FLOW_MAP_TARGET_TINT
#undef _FLOW_MAP_TARGET_EMISSION
#undef _FLOW_MAP_TARGET_ALPHA_TRANSITION
#endif

#if defined( _ALPHATEST_ENABLED )
#if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_TINT) || defined(_FLOW_MAP_TARGET_EMISSION) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
        #define _USE_FLOW_MAP
#endif
#if defined(_FADE_TRANSITION_ENABLED) || defined(_DISSOLVE_TRANSITION_ENABLED)
        #define _USE_TRANSITION_MAP
#endif
#elif defined(_USE_BASE_MAP_UV)
#if defined(_FLOW_MAP_ENABLED)
        #define _USE_FLOW_MAP
#endif
#endif

#if defined( _ALPHATEST_ENABLED ) || defined(_USE_FLOW_MAP) || defined(_USE_BASE_MAP_UV)
#define _USE_CUSTOM_COORD
#endif


#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ParticlesUberUnlit.hlsl"

/**
 * \brief Input attribute of vertex shader.
 * \details This structure is used by DepthOnly pass and DepthNormalsPass.
 */
struct AttributesDrawDepth
{
    float4 positionOS : POSITION;
    #ifdef FRAGMENT_USE_NORMAL_WS
    float3 normalOS : NORMAL;
    #ifdef _NORMAL_MAP_ENABLED
    float4 tangentOS : TANGENT;
    #endif
    #endif
    #ifdef _USE_BASE_MAP_UV
    float2 texcoord : TEXCOORD0;
    #endif
    #ifndef NOVA_PARTICLE_INSTANCING_ENABLED
    #ifdef _USE_CUSTOM_COORD
    INPUT_CUSTOM_COORD(1, 2)
    #endif
    #endif
    #ifdef _ALPHATEST_ENABLED // This attributes is not used for opaque objects.
    float4 color : COLOR;

    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/**
 * \brief Input data of the pixel shader and output data from the vertex shader.
 * \brief This structure is used by DepthOnly pass and DepthNormalsPass.
 */
struct VaryingsDrawDepth
{
    float4 positionHCS : SV_POSITION;
    #ifdef FRAGMENT_USE_NORMAL_WS
    float3 normalWS : NORMAL;
    #ifdef _NORMAL_MAP_ENABLED
    float4 tangentWS : TANGENT;
    float3 binormalWS : BINORMAL;
    #endif
    #endif

    #ifdef _USE_CUSTOM_COORD
    INPUT_CUSTOM_COORD(0, 1)
    #endif

    #ifdef _USE_BASE_MAP_UV
    float4 baseMapUVAndProgresses : TEXCOORD2; // xy: BaseMap UV, z: Base Map Progress, w: Tint Map Progress
    #endif
    #if defined( _USE_FLOW_MAP ) || defined(_USE_TRANSITION_MAP)
    float4 flowTransitionUVs : TEXCOORD3; // xy: FlowMap UV, zw: TransitionMap UV
    #endif

    #ifdef _ALPHATEST_ENABLED // This attributes is not used for opaque objects.
    float4 color : COLOR;
    float4 tintEmissionUV : TEXCOORD4; // xy: TintMap UV, zw: EmissionMap UV
    float3 transitionEmissionProgresses : TEXCOORD5;
    // x: TransitionMap Progress, y: EmissionMap Progress, z: Fog Factor
    #ifdef FRAGMENT_USE_VIEW_DIR_WS
    float3 viewDirWS : TEXCOORD6;
    #endif

    #ifdef USE_PROJECTED_POSITION
    float4 projectedPosition : TEXCOORD7;
    #endif
    #endif

    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    float4 flowTransitionSecondUVs : TEXCOORD8; // xy: FlowMap UV, zw: TransitionMap UV
    float2 transitionEmissionProgressesSecond : TEXCOORD9; // x: TransitionMap Progress, y: EmissionMap Progress
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

/**
 * \brief Initialize output data from vertex shader for DepthOnly and DepthNormals pass.
 * \param[in] input Input attributes to vertex shader.
 * \param[in,out] output Output data from vertex shader.
 */
inline void InitializeVertexOutputDrawDepth(in AttributesDrawDepth input, in out VaryingsDrawDepth output)
{
    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    #ifdef FRAGMENT_USE_NORMAL_WS
    output.normalWS = TransformObjectToWorldNormal(input.normalOS, true);
    #ifdef _NORMAL_MAP_ENABLED
    CalculateTangetAndBinormalInWorldSpace(output.tangentWS, output.binormalWS,
        output.normalWS, input.tangentOS );
    #endif
    #endif

    #ifdef _ALPHATEST_ENABLED // This code is not used for opaque objects.
    output.color = GetParticleColor(input.color);
    #ifdef FRAGMENT_USE_VIEW_DIR_WS
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.viewDirWS = GetWorldSpaceViewDir(positionWS);
    #endif

    #ifdef USE_PROJECTED_POSITION
    output.projectedPosition = ComputeScreenPos(output.positionHCS);
    #endif
    #endif
}

/**
 * \brief Initialize input data of fragment shader for DepthOnly and DepthNormalsPass.
 * \param input Input data from vertex shader.
 */
inline void InitializeFragmentInputDrawDepth(in out VaryingsDrawDepth input)
{
    #ifdef FRAGMENT_USE_NORMAL_WS
    input.normalWS = normalize(input.normalWS);
    #endif

    #ifdef _ALPHATEST_ENABLED // This code is not used for opaque objects.
    #ifdef FRAGMENT_USE_VIEW_DIR_WS
    input.viewDirWS = normalize(input.viewDirWS);
    #endif
    #endif
}

/**
 * \brief Vertex shader entry point.
 * \param input Input attribute.
 * \return Return data of VaryingsDrawDepth Structure.
 */
VaryingsDrawDepth vert(AttributesDrawDepth input)
{
    VaryingsDrawDepth output = (VaryingsDrawDepth)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    SETUP_VERTEX;
    #ifdef _USE_CUSTOM_COORD // This code is not used for opaque objects.
    SETUP_CUSTOM_COORD(input)
    TRANSFER_CUSTOM_COORD(input, output);
    #endif
    InitializeVertexOutputDrawDepth(input, output);

    #ifdef _USE_BASE_MAP_UV
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
    #endif

    // Flow Map UV
    #ifdef _USE_FLOW_MAP
    output.flowTransitionUVs.xy = TRANSFORM_TEX(input.texcoord.xy, _FlowMap);
    output.flowTransitionUVs.x += GET_CUSTOM_COORD(_FlowMapOffsetXCoord);
    output.flowTransitionUVs.y += GET_CUSTOM_COORD(_FlowMapOffsetYCoord);
    #endif

    #ifdef _USE_TRANSITION_MAP
    // Transition Map UV
    output.flowTransitionUVs.zw = TRANSFORM_ALPHA_TRANSITION_MAP(input.texcoord.xy);
    output.flowTransitionUVs.z += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetXCoord)
    output.flowTransitionUVs.w += GET_CUSTOM_COORD(_AlphaTransitionMapOffsetYCoord)
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
    output.flowTransitionSecondUVs.zw = TRANSFORM_ALPHA_TRANSITION_MAP_SECOND(input.texcoord.xy);
    output.flowTransitionSecondUVs.z += GET_CUSTOM_COORD(_AlphaTransitionMapSecondTextureOffsetXCoord)
    output.flowTransitionSecondUVs.w += GET_CUSTOM_COORD(_AlphaTransitionMapSecondTextureOffsetYCoord)
    #endif
    #endif

    #ifdef _ALPHATEST_ENABLED // This code is not used for opaque objects.

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

    // NOTE : Not need in DepthNormals pass.
    //Fog
    // output.transitionEmissionProgresses.z = ComputeFogFactor(output.positionHCS.z);
    #endif

    return output;
}

/**
 * \brief Fragment shader entry point.
 * \param input Input data from vertex shader.
 * \return If DEPTH_NORMALS_PASS is defined, this function return normal of pixel,\n
 * but it isn't defined it return zero.
 */
half4 frag(VaryingsDrawDepth input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    SETUP_FRAGMENT;

    #ifdef _USE_CUSTOM_COORD
    SETUP_CUSTOM_COORD(input);
    #endif

    InitializeFragmentInputDrawDepth(input);

    #if defined(_TRANSPARENCY_BY_RIM) || defined(_TINT_AREA_RIM)
    half rim = 1.0 - abs(dot(input.normalWS, input.viewDirWS));
    #endif

    // Flow map
    #if defined( _USE_FLOW_MAP)
    half intensity = _FlowIntensity + GET_CUSTOM_COORD(_FlowIntensityCoord);
    half2 flowMapUvOffset = GetFlowMapUvOffset(_FlowMap, sampler_FlowMap, intensity, input.flowTransitionUVs.xy, _FlowMapChannelsX, _FlowMapChannelsY);
    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE)
        input.baseMapUVAndProgresses.xy += flowMapUvOffset;
    #endif
    #ifdef _FLOW_MAP_TARGET_TINT
        input.tintEmissionUV.xy += flowMapUvOffset;
    #endif
    #ifdef _FLOW_MAP_TARGET_EMISSION
        input.tintEmissionUV.zw += flowMapUvOffset;
    #endif
    #ifdef _FLOW_MAP_TARGET_ALPHA_TRANSITION
        input.flowTransitionUVs.zw += flowMapUvOffset;
    #if defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE) || defined(_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY)
        input.flowTransitionSecondUVs.zw += flowMapUvOffset;
    #endif
    #endif
    #endif

    #ifdef _ALPHATEST_ENABLED // This code is not used for opaque objects.
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

    // Alpha Transition
    #ifdef _USE_TRANSITION_MAP
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

    // NOTE : Not need in DepthNormals pass.
    // Color Correction
    // ApplyColorCorrection(color.rgb);

    // Vertex Color
    ApplyVertexColor(color, input.color);

    // Emission
    half emissionIntensity = _EmissionIntensity + GET_CUSTOM_COORD(_EmissionIntensityCoord);
    ApplyEmissionColor(color, input.tintEmissionUV.zw, emissionIntensity, input.transitionEmissionProgresses.y, _EmissionMapChannelsX);

    // NOTE : Not need in DepthNormals pass.
    // Fog
    // half fogFactor = input.transitionEmissionProgresses.z;
    // color.rgb = MixFog(color.rgb, fogFactor);

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

    // NOTE : Not need in DepthNormals pass.
    // color.rgb = ApplyAlpha(color.rgb, color.a);
    #endif
    #ifdef DEPTH_NORMALS_PASS
    float3 normalWS = input.normalWS;
    #ifdef _NORMAL_MAP_ENABLED
    float3 normalTS = SAMPLE_NORMAL_MAP(input.baseMapUVAndProgresses.xy, input.baseMapUVAndProgresses.z,
                                             _NormalMapBumpScale);
    normalWS = GetNormalWS(normalTS, input.tangentWS, input.binormalWS, input.normalWS );
    #endif
    return half4(NormalizeNormalPerPixel(normalWS), 0.0);
    #else
    return half4(0.0, 0.0, 0.0, 0.0);
    #endif
}

#endif
