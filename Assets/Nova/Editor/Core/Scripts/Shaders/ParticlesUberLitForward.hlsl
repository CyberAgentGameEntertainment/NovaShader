#ifndef NOVA_PARTICLESUBERLITFORWARD_INCLUDED
#define NOVA_PARTICLESUBERLITFORWARD_INCLUDED

#define FRAGMENT_USE_NORMAL_WS
#define FRAGMENT_USE_VIEW_DIR_WS
// todo If _SPECULARHIGHLIGHTS_OFF is defined, specular highlight is disabled.
#define _SPECULARHIGHLIGHTS_OFF
// todo If _ENVIRONMENTREFLECTIONS_OFF is defined, environment reflections are desabled。
#define _ENVIRONMENTREFLECTIONS_OFF
// todo If _NORMALMAP is defind, normal map is used.
#define _NORMALMAP
// todo If REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR is defined, uv coords of shadow map is calculated in vertex shader.
#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
// todo If MAIN_LIGHT_CALCULATE_SHADOWS is defined, shadow are cast on the objects.
#ifdef _MAIN_LIGHT_CALCULATE_SHADOWS
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
    #ifdef _NORMALMAP
    output.tangentWS.xyz = TransformObjectToWorldDir(input.tangentOS.xyz, true);
    output.tangentWS.w = input.tangentOS.w;
    output.binormalWS = cross(output.varyingsUnlit.normalWS, output.tangentWS) * input.tangentOS.w;
    #endif
    
    half3 vertexLight = VertexLighting(output.positionWS, output.varyingsUnlit.normalWS);
    half fogFactor = ComputeFogFactor(output.varyingsUnlit.positionHCS.z);
    output.positionWS.w = fogFactor;
    
    OUTPUT_SH(output.varyingsUnlit.normalWS.xyz, output.vertexSH);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowCoord = TransformWorldToShadowCoord(output.positionWS.xyz);
    #endif
    
    return output;
}
half GetMetallic( float3 uvw )
{
    half4 metallic = SAMPLE_METALLIC_MAP(uvw.xy, uvw.z);
    return metallic[(int)_MetallicMapChannelsX.x];;
}
half GetSmoothness( float3 uvw )
{
    const half4 smoothness = SAMPLE_SMOOTHNESS_MAP(uvw.xy, uvw.z);
    return smoothness[(int)_SmoothnessMapChannelsX.x];
}
half4 fragLit(VaryingsLit input) : SV_Target
{
    Varyings inputUnlit = input.varyingsUnlit; 
    half4 color = frag(inputUnlit);
    SurfaceData surfaceData = (SurfaceData)0;
    // OK
    surfaceData.albedo = color.xyz;
    // TODO Survey the SampleNormalTS Function in Particels.hlsl in URP package.
    surfaceData.normalTS = SAMPLE_NORMAL_MAP(inputUnlit.baseMapUVAndProgresses.xy, inputUnlit.baseMapUVAndProgresses.z);
    // TODO Specular Workflow is not implemented. 
    // OK
    surfaceData.metallic = GetMetallic(inputUnlit.baseMapUVAndProgresses.xyz);
    // OK
    surfaceData.specular = half3( 0, 0, 0);
    surfaceData.smoothness = GetSmoothness(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.alpha = color.a;
    // The values of clearCoatMask,clearCoatSmoothness and occlusion is referenced from ParticlesLitInput.hlsl in UPR Package. 
    surfaceData.clearCoatMask = 0;
    surfaceData.clearCoatSmoothness = 1;
    surfaceData.occlusion = 1;
    // todo : What to do emission?
    surfaceData.emission = 0;
    
    InputData inputData = (InputData)0;
    // OK
    inputData.positionWS = input.positionWS.xyz;
    #ifdef _NORMALMAP
    // OK
    inputData.normalWS = TransformTangentToWorld(
        surfaceData.normalTS,
        half3x3(
            input.tangentWS.xyz,
            input.binormalWS.xyz,
            inputUnlit.normalWS.xyz));
    
    #else
    inputData.normalWS = inputUnlit.normalWS.xyz;
    #endif
    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    // OK
    inputData.viewDirectionWS = inputUnlit.viewDirWS;
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    // OK
    inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    // todo : Not implemented yet.
    inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS.xyz);
    #else
    inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif
    inputData.fogCoord = input.positionWS.w;
    // todo : What to do vertexLighting? 
    
    inputData.bakedGI = SampleSHPixel(input.vertexSH, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(inputUnlit.positionHCS);
    // The values of shadowMask and vertexLighting are referenced from ParticlesLitForwardPass.hlsl in UPR Package.
    inputData.shadowMask = half4(1, 1, 1, 1);
    inputData.vertexLighting = half3(0, 0, 0);
    
    color = UniversalFragmentPBR(inputData, surfaceData);
    return color;
}

#endif