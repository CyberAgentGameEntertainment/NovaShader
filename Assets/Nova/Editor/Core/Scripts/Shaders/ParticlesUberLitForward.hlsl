#ifndef NOVA_PARTICLESUBERLITFORWARD_INCLUDED
#define NOVA_PARTICLESUBERLITFORWARD_INCLUDED

#define FRAGMENT_USE_NORMAL_WS
#define FRAGMENT_USE_VIEW_DIR_WS

#ifndef _SPECULAR_HIGHLIGHTS_ENABLED
// This symbol has been defined for URP Functions.
// todo : Complete the test that symbol goes from on to off.
//        But we have not tested the effectiveness of the symbols.
#define _SPECULARHIGHLIGHTS_OFF
#endif

#ifndef _ENVIRONMENT_REFLECTIONS_ENABLED
// This symbol has been defined for URP Functions.
#define _ENVIRONMENTREFLECTIONS_OFF
#endif 

// todo If REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR is defined, uv coords of shadow map is calculated in vertex shader.
#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR

#ifdef _NORMAL_MAP_ENABLED
// This symbol has been defined for URP Functions.
#define _NORMAL_MAP
#endif
#ifdef _RECEIVE_SHADOWS_ENABLED
// This symbol has been defined for URP Functions.
#define MAIN_LIGHT_CALCULATE_SHADOWS
#endif


#include "ParticlesUberUnlitForward.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "ParticlesUberLit.hlsl"

VaryingsLit vertLit(AttributesLit input)
{
    VaryingsLit output = (VaryingsLit)0;
    output.varyingsUnlit = vert(input.attributesUnlit);

    output.positionWS.xyz = TransformObjectToWorld(input.attributesUnlit.positionOS.xyz);

    // Calculate tanget and binormal
    #ifdef _NORMAL_MAP_ENABLED
    output.tangentWS.xyz = TransformObjectToWorldDir(input.tangentOS.xyz, true);
    output.tangentWS.w = input.tangentOS.w;
    output.binormalWS = cross(output.varyingsUnlit.normalWS, output.tangentWS) * input.tangentOS.w;
    #endif
    
    // todo : vertexLight is not used in ParticlesLitForwardPass.hlsl.
    // half3 vertexLight = VertexLighting(output.positionWS, output.varyingsUnlit.normalWS);
    half fogFactor = ComputeFogFactor(output.varyingsUnlit.positionHCS.z);
    output.positionWS.w = fogFactor;
    
    OUTPUT_SH(output.varyingsUnlit.normalWS.xyz, output.vertexSH);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(_RECEIVE_SHADOWS_ENABLED)
    output.shadowCoord = TransformWorldToShadowCoord(output.positionWS.xyz);
    #endif
    
    return output;
}
half GetMetallic( float3 uvw )
{
    #ifdef _SPECULAR_SETUP
    return 0;
    #else
    #ifdef _METALLIC_MAP_ENABLED
    half4 metallic = SAMPLE_METALLIC_MAP(uvw.xy, uvw.z);
    return metallic[(int)_MetallicMapChannelsX.x];
    #else
    return _Metallic;
    #endif
    #endif
}
half GetSmoothness( float3 uvw )
{
    #ifdef _SMOOTHNESS_MAP_ENABLED
    const half4 smoothness = SAMPLE_SMOOTHNESS_MAP(uvw.xy, uvw.z);
    return smoothness[(int)_SmoothnessMapChannelsX.x];
    #else
    return _Smoothness;
    #endif
}
half3 GetSpecular(float3 uvw)
{
    #ifdef _SPECULAR_SETUP
    const half4 specular = SAMPLE_SPECULAR_MAP(uvw.xy, uvw.z);
    return specular.xyz;
    #else
    return half3(0, 0, 0);
    #endif
}
#ifdef _RECEIVE_SHADOWS_ENABLED
float4 GetShadowCoord( VaryingsLit input )
{
    float4 shadowCoord = float4(0, 0, 0, 0);
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    shadowCoord = input.shadowCoord;
    #else
    shadowCoord = TransformWorldToShadowCoord(input.positionWS.xyz);
    #endif
    return shadowCoord;
}
#define GET_SHADOW_COORD( shadowCoord, input ) shadowCoord = GetShadowCoord(input)
#else
#define GET_SHADOW_COORD( shadowCoord, input )
#endif

float3 GetNormalWS(SurfaceData surfaceData, VaryingsLit input)
{
    float3 normalWS;
    #ifdef _NORMAL_MAP_ENABLED
    normalWS = TransformTangentToWorld(
        surfaceData.normalTS,
        half3x3(
            input.tangentWS.xyz,
            input.binormalWS.xyz,
            input.varyingsUnlit.normalWS.xyz));
    
    #else
    normalWS = input.varyingsUnlit.normalWS.xyz;
    #endif
    normalWS = NormalizeNormalPerPixel(normalWS);
    return normalWS;
}

void InitializeSurfaceData( out SurfaceData surfaceData, VaryingsLit input, half4 albedoColor)
{
    surfaceData = (SurfaceData)0;
    Varyings inputUnlit = input.varyingsUnlit;
    surfaceData.albedo = albedoColor.xyz;
    // TODO Survey the SampleNormalTS Function in Particels.hlsl in URP package.
    surfaceData.normalTS = SAMPLE_NORMAL_MAP(inputUnlit.baseMapUVAndProgresses.xy, inputUnlit.baseMapUVAndProgresses.z);
    surfaceData.metallic = GetMetallic(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.specular = GetSpecular(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.smoothness = GetSmoothness(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.alpha = albedoColor.a;
    // todo : What to do emission?
    surfaceData.emission = 0;
    // The values of clearCoatMask,clearCoatSmoothness and occlusion is referenced from ParticlesLitInput.hlsl in UPR Package. 
    surfaceData.clearCoatMask = 0;
    surfaceData.clearCoatSmoothness = 1;
    surfaceData.occlusion = 1;
}
void InitializeInputData(out InputData inputData, SurfaceData surfaceData, VaryingsLit input)
{
    inputData = (InputData)0;
    Varyings inputUnlit = input.varyingsUnlit;
    inputData.positionWS = input.positionWS.xyz;
    inputData.normalWS = GetNormalWS(surfaceData, input);
    inputData.viewDirectionWS = inputUnlit.viewDirWS;
    GET_SHADOW_COORD(inputData.shadowCoord, input );
    inputData.fogCoord = input.positionWS.w;
    inputData.bakedGI = SampleSHPixel(input.vertexSH, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(inputUnlit.positionHCS);
    // The values of shadowMask and vertexLighting are referenced from ParticlesLitForwardPass.hlsl in UPR Package.
    inputData.shadowMask = half4(1, 1, 1, 1);
    inputData.vertexLighting = half3(0, 0, 0);
}
half4 fragLit(VaryingsLit input) : SV_Target
{
    SurfaceData surfaceData;
    InitializeSurfaceData(surfaceData, input, frag(input.varyingsUnlit));
    InputData inputData;
    InitializeInputData(inputData, surfaceData, input);
    
    return UniversalFragmentPBR(inputData, surfaceData);
}

#endif