#ifndef NOVA_PARTICLESUBERLIT_INCLUDED
#define NOVA_PARTICLESUBERLIT_INCLUDED

#include "ParticlesUberUnlit.hlsl"

// Input attributes for lit.
struct AttributesLit
{
    Attributes attributesUnlit;
    float4 tangentOS : TANGENT;
};

// Output data from the vertex shader.
struct VaryingsLit
{
    Varyings varyingsUnlit;
    float3 positionWS : COLOR1;
    float3 tangentWS : TANGENT;
    float3 binormalWS : BINORMAL;
    
};
inline void InitializeFragmentInputLit(in out VaryingsLit input)
{
    InitializeFragmentInput(input.varyingsUnlit);
    input.binormalWS = normalize(input.binormalWS);
    input.tangentWS = normalize(input.tangentWS);
}


#endif
