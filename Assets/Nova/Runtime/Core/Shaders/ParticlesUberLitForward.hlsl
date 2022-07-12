#ifndef NOVA_PARTICLESUBERLITFORWARD_INCLUDED
#define NOVA_PARTICLESUBERLITFORWARD_INCLUDED

#define FRAGMENT_USE_NORMAL_WS
#define FRAGMENT_USE_VIEW_DIR_WS
#define _SPECULARHIGHLIGHTS_OFF         // todo これのオンオフでスペキュラのハイライトが変わる。
#define _ENVIRONMENTREFLECTIONS_OFF     // todo これのオンオフで環境光の反射が変わる。

#include "ParticlesUberUnlitForward.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "ParticlesUberLit.hlsl"

VaryingsLit vertLit(AttributesLit input)
{
    VaryingsLit output = (VaryingsLit)0;
    output.varyingsUnlit = vert(input.attributesUnlit);

    output.positionWS = TransformObjectToWorld(input.attributesUnlit.positionOS.xyz);
    output.tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz, true);
    
    half sign = input.tangentOS.w * unity_WorldTransformParams.w;
    output.binormalWS = cross(output.varyingsUnlit.normalWS, output.tangentWS) * sign;
    
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
    surfaceData.albedo = color.xyz;
    surfaceData.metallic = 0.5;
    surfaceData.normalTS = SAMPLE_NORMAL_MAP(inputUnlit.baseMapUVAndProgresses.xy, inputUnlit.baseMapUVAndProgresses.z);
    surfaceData.alpha = color.a;
    surfaceData.metallic = GetMetallic(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.smoothness = GetSmoothness(inputUnlit.baseMapUVAndProgresses.xyz);
    surfaceData.clearCoatMask = 0;
    surfaceData.clearCoatSmoothness = 0;
    surfaceData.occlusion = 0;
    surfaceData.emission = 0;
    InputData inputData = (InputData)0;
    inputData.positionWS = input.positionWS;
    inputData.normalWS = TransformTangentToWorld(
        surfaceData.normalTS,
        half3x3(input.tangentWS.xyz, input.binormalWS.xyz, inputUnlit.normalWS.xyz));
    inputData.viewDirectionWS = inputUnlit.viewDirWS;
    
    // todo : What to do vertexLighting? 
    inputData.vertexLighting = half3(0, 0, 0);
    inputData.normalizedScreenSpaceUV = 
    color = UniversalFragmentPBR(inputData, surfaceData);
    return color;
}

#endif