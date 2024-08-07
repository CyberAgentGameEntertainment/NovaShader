#ifndef NOVA_PARTICLESUBERUNLITEDITOR_INCLUDED
#define NOVA_PARTICLESUBERUNLITEDITOR_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ParticlesUberUnlit.hlsl"

float _ObjectId;
float _PassValue;
float4 _SelectionID;

Varyings vertEditor(Attributes input)
{
    float3 positionWS;
    return vertUnlit(input, positionWS, false, false);
}

half4 fragSceneHighlight(Varyings input) : SV_Target
{
    fragUnlit(input, false);
    return float4(_ObjectId, _PassValue, 1, 1);
}

half4 fragScenePicking(Varyings input) : SV_Target
{
    fragUnlit(input, false);
    return _SelectionID;
}

#endif
