#ifndef NOVA_PARTICLESUBERUNLITFORWARD_INCLUDED
#define NOVA_PARTICLESUBERUNLITFORWARD_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#ifdef DEBUG_DISPLAY // for backward compatibility
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Unlit.hlsl"
#endif
#include "ParticlesUberUnlit.hlsl"

void InitializeInputData(out InputData inputData, Varyings input)
{
    inputData = (InputData)0;

    // InputData is only used for DebugDisplay purposes in Unlit.
    // This is implemented with reference to Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl.
    inputData.positionWS = half3(0, 0, 0);
    inputData.normalWS = half3(0, 0, 1);
    inputData.viewDirectionWS = half3(0, 0, 1);
    inputData.shadowCoord = 0;
    inputData.fogCoord = input.transitionEmissionProgresses.z;
    inputData.vertexLighting = half3(0, 0, 0);
    inputData.bakedGI = half3(0, 0, 0);
    inputData.normalizedScreenSpaceUV = 0;
    inputData.shadowMask = half4(1, 1, 1, 1);
}

Varyings vert(Attributes input)
{
    float3 positionWS;
    return vertUnlit(input, positionWS, true, true);
}

half4 frag(Varyings input) : SV_Target
{
    InputData inputData;
    InitializeInputData(inputData, input);

    half4 color = fragUnlit(input, true);
    #ifdef DEBUG_DISPLAY  // for backward compatibility
    color = UniversalFragmentUnlit(inputData, color.rgb, color.a);
    #endif
    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
}

#endif
