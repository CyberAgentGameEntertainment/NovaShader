#ifndef NOVA_PARTICLESUBERLITFORWARD_INCLUDED
#define NOVA_PARTICLESUBERLITFORWARD_INCLUDED

//////////////////////////////////////////
// Define symbols for URP Functions.
//////////////////////////////////////////

#ifndef _SPECULAR_HIGHLIGHTS_ENABLED
#define _SPECULARHIGHLIGHTS_OFF
#endif

#ifndef _ENVIRONMENT_REFLECTIONS_ENABLED
#define _ENVIRONMENTREFLECTIONS_OFF
#endif

#ifdef _NORMAL_MAP_ENABLED
#define _NORMALMAP
#endif

#ifndef _RECEIVE_SHADOWS_ENABLED
#define _RECEIVE_SHADOWS_OFF
#endif

#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR

//////////////////////////////////////////
// Define symbols for UberUnlit.
//////////////////////////////////////////
#define FRAGMENT_USE_NORMAL_WS
#define FRAGMENT_USE_VIEW_DIR_WS

//////////////////////////////////////////
// Include files
//////////////////////////////////////////
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "ParticlesUberLit.hlsl"

//////////////////////////////////////////
// Functions
//////////////////////////////////////////

/**
 * \brief
 *  Get Metallic value.
 * \param uvw
 *  If metallic map is Texture2D, uv.xy is used.\n
 *  But if it is TextureArray or Texture 3D, uv.xyz is is used.
 * \return
 *  If _METALLIC_MAP_ENABLED is defined, \n
 *  the returned value is multiplied the sampled value from metallic map by _Metallic.\n
 *  It isn't defined, _Metallic property value is returned.\n
 *  But if _SPECULAR_SETUP is defined, returned value is always 1.
 */
half GetMetallic(float3 uvw)
{
    #ifdef _SPECULAR_SETUP
    return 1;
    #else

#ifdef ENABLE_DYNAMIC_BRANCH    
    if(_METALLIC_MAP_ENABLED)
    {
        half4 metallic = SAMPLE_METALLIC_MAP(uvw.xy, uvw.z);
        return metallic[(int)_MetallicMapChannelsX.x] * _Metallic;
    }else
    {
        return _Metallic;
    }
#else 
    #ifdef _METALLIC_MAP_ENABLED
    half4 metallic = SAMPLE_METALLIC_MAP(uvw.xy, uvw.z);
    return metallic[(int)_MetallicMapChannelsX.x] * _Metallic;
    #else
    return _Metallic;
    #endif
#endif
    
    #endif
}

/**
 * \brief
 *  Get Smoothness Value.
 * \param uvw
 *  If smoothness map is Texture2D, uv.xy is used.<br/>
 *  But if it is TextureArray or Texture 3D, uv.xyz is is used.
 * \return
 *   If _SMOOTHNESS_MAP_ENABLED is defined, \n
 *  the returned value is multiplied the sampled value from smooth map by _Smoothness.\n
 *  It isn't defined, _Smoothness property value is returned.\n
 */
half GetSmoothness(float3 uvw)
{
#ifdef ENABLE_DYNAMIC_BRANCH
    if(_SMOOTHNESS_MAP_ENABLED)
    {
        const half4 smoothness = SAMPLE_SMOOTHNESS_MAP(uvw.xy, uvw.z);
        // The reason for multiplying _Smoothness is because it was done in URP's build-in shaders.
        return smoothness[(int)_SmoothnessMapChannelsX.x] * _Smoothness;
    }else
    {
        return _Smoothness; 
    }
#else
    #ifdef _SMOOTHNESS_MAP_ENABLED
    const half4 smoothness = SAMPLE_SMOOTHNESS_MAP(uvw.xy, uvw.z);
    // The reason for multiplying _Smoothness is because it was done in URP's build-in shaders.
    return smoothness[(int)_SmoothnessMapChannelsX.x] * _Smoothness;
    #else
    return _Smoothness;
    #endif
#endif
}

/**
 * \brief
 *  Get Specular Color.
 * \param uvw
 *  If specular map is Texture2D, uv.xy is used.\n
 * But if it is TextureArray or Texture 3D, uv.xyz is is used.
 * \return
 *  If _SPECULAR_SETUP isn't defined, returned value is always half3( 0, 0, 0 ).\n
 *  If it is defined and _SPECULAR_MAP_ENABLED is defined,
 *  the returned value is  multiplied the sampled value from specular map by _SpecularColor.\n
 *  If _SPECULAR_MAP_ENABLED isn't defined, the returned value is _SpecularColor. 
 */

half3 GetSpecular(float3 uvw)
{
#ifdef _SPECULAR_SETUP
    #ifdef ENABLE_DYNAMIC_BRANCH
        if(_SPECULAR_MAP_ENABLED)
        {
            const half4 specular = SAMPLE_SPECULAR_MAP(uvw.xy, uvw.z);
            return specular.xyz * _SpecularColor.xyz;
        }else
        {
            return _SpecularColor.xyz;    
        }
    #else
        #ifdef _SPECULAR_MAP_ENABLED
        const half4 specular = SAMPLE_SPECULAR_MAP(uvw.xy, uvw.z);
        return specular.xyz * _SpecularColor.xyz;
        #else
        return _SpecularColor.xyz;
        #endif
    #endif
#else
    return half3(0, 0, 0);
#endif
}

#ifdef _RECEIVE_SHADOWS_ENABLED
/**
 * \brief
 *  Get shadow map uv coords.
 * \param input
 *  It has been outputted from vertex shader.
 * \return
 *  If defined REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR, the returned value has been calculated by vertex shader.
 *  But if it isn't defined, the returned value is calculated by this function.
 */
float4 GetShadowCoord(VaryingsLit input)
{
    float4 shadowCoord = float4(0, 0, 0, 0);
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    shadowCoord = input.shadowCoord;
#else
    shadowCoord = TransformWorldToShadowCoord(input.positionWS);
#endif
    return shadowCoord;
}

/**
 * \brief
 *  Get Shadow Coord macro.\n
 *  Please don't use GetShadowCoord(), use this instead.
 * \param[out]  shadowCoord  output shadowCoord. 
 * \param[in]   input       It has been outputted from vertex shader.
 */
#define GET_SHADOW_COORD( shadowCoord, input ) shadowCoord = GetShadowCoord(input)
#else
#define GET_SHADOW_COORD( shadowCoord, input )
#endif

/**
 * \brief Initialize SurfaceData for UniversalFragmentPBR function.
 * \param [out] surfaceData     Outputs surface data.
 * \param [in]  input           It has been calculated from vertex shader.
 * \param [in]  albedoColor     Albedo color of surface.
 */
void InitializeSurfaceData(out SurfaceData surfaceData, VaryingsLit input, half4 albedoColor)
{
    surfaceData = (SurfaceData)0;
    Varyings inputUnlit = input.varyingsUnlit;
    surfaceData.albedo = albedoColor.xyz;
    surfaceData.normalTS = SAMPLE_NORMAL_MAP(inputUnlit.baseMapUVAndProgresses.xy, inputUnlit.baseMapUVAndProgresses.z,
                                             _NormalMapBumpScale);
    surfaceData.metallic = GetMetallic(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.specular = GetSpecular(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.smoothness = GetSmoothness(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.alpha = albedoColor.a;
    surfaceData.emission = 0;
    // The values of clearCoatMask,clearCoatSmoothness and occlusion is referenced from ParticlesLitInput.hlsl in UPR Package. 
    surfaceData.clearCoatMask = 0;
    surfaceData.clearCoatSmoothness = 0;
    surfaceData.occlusion = 1;
}

/**
 * \brief Initialize InputData for UniversalFragmentPBR function.
 * \param[out]  inputData       Outputs input data.
 * \param[in]   surfaceData     It must be calculated by InitializeSurfaceData().
 * \param[in]   input           It has been calculated from vertex shader.
 */
void InitializeInputData(out InputData inputData, SurfaceData surfaceData, VaryingsLit input)
{
    inputData = (InputData)0;
    Varyings inputUnlit = input.varyingsUnlit;
    inputData.positionWS = input.positionWS;

    inputData.normalWS = GET_NORMAL_WS(surfaceData.normalTS, input.tangentWS,
                                       input.binormalWS, input.varyingsUnlit.normalWS);

    inputData.viewDirectionWS = SafeNormalize(inputUnlit.viewDirWS);
    GET_SHADOW_COORD(inputData.shadowCoord, input);
    inputData.fogCoord = inputUnlit.transitionEmissionProgresses.z;
    inputData.bakedGI = SampleSHPixel(input.vertexSH, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(inputUnlit.positionHCS);
    // The values of shadowMask and vertexLighting are referenced from ParticlesLitForwardPass.hlsl in UPR Package.
    inputData.shadowMask = half4(1, 1, 1, 1);
    inputData.vertexLighting = half3(0, 0, 0);
}

/**
 * \brief Vertex shader entry point.
 */
VaryingsLit vertLit(AttributesLit input)
{
    VaryingsLit output = (VaryingsLit)0;
    output.varyingsUnlit = vertUnlit(input.attributesUnlit, output.positionWS, true, true);

    // Calculate tangent and binormal.
    #ifdef _NORMAL_MAP_ENABLED
    CalculateTangetAndBinormalInWorldSpace(output.tangentWS, output.binormalWS, output.varyingsUnlit.normalWS,
                                           input.tangentOS);
    #endif

    // TODO: vertexLight is not used in ParticlesLitForwardPass.hlsl. What about with NOVA Shader?
    // half3 vertexLight = VertexLighting(output.positionWS, output.varyingsUnlit.normalWS);

    // Fog Code was already computed by vertUnlit().
    // So this code was deleted .
    // half fogFactor = ComputeFogFactor(output.varyingsUnlit.positionHCS.z);
    // output.positionWS.w = fogFactor;

    OUTPUT_SH(output.varyingsUnlit.normalWS.xyz, output.vertexSH);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(_RECEIVE_SHADOWS_ENABLED)
    output.shadowCoord = TransformWorldToShadowCoord(output.positionWS);
    #endif

    return output;
}

/**
 * \brief Fragment shader entry point. 
 */
half4 fragLit(VaryingsLit input) : SV_Target
{
    half4 albedoColor = fragUnlit(input.varyingsUnlit, true);

    SurfaceData surfaceData;
    InitializeSurfaceData(surfaceData, input, albedoColor);

    InputData inputData;
    InitializeInputData(inputData, surfaceData, input);

    half4 color = UniversalFragmentPBR(inputData, surfaceData);
    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
}

#endif
