#ifndef NOVA_PARTICLESUBERLIT_INCLUDED
#define NOVA_PARTICLESUBERLIT_INCLUDED

#include "ParticlesUberUnlit.hlsl"

// Input attributes for lit.
struct AttributesLit
{
    Attributes attributesUnlit;
    #ifdef _NORMALMAP
    float4 tangentOS : TANGENT;
    #endif
};

// Output data from the vertex shader.
struct VaryingsLit
{
    Varyings varyingsUnlit;
    float4 positionWS : COLOR1;
    #ifdef _NORMALMAP
    float4 tangentWS : TANGENT;
    float3 binormalWS : BINORMAL;
    #endif
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord : COLOR2;
    #endif
    float3 vertexSH    : COLOR3; // SH
    
};
inline void InitializeFragmentInputLit(in out VaryingsLit input)
{
    InitializeFragmentInput(input.varyingsUnlit);
    #ifdef _NORMALMAP
    input.binormalWS = normalize(input.binormalWS);
    input.tangentWS = normalize(input.tangentWS);
    #endif
}


#endif
