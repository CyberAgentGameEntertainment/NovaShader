#ifndef NOVA_PARTICLESUBERLIT_INCLUDED
#define NOVA_PARTICLESUBERLIT_INCLUDED

#include "ParticlesUberUnlit.hlsl"

/**
 * \brief Vertex shader input attributes for UberLit.
 */
struct AttributesLit
{
    Attributes attributesUnlit;
    #ifdef _NORMAL_MAP_ENABLED
    float4 tangentOS : TANGENT;
    #endif
};

/**
 * \brief Output data structure from the vertex shader.
 */
struct VaryingsLit
{
    Varyings varyingsUnlit;
    float4 positionWS : COLOR1;
    #ifdef _NORMAL_MAP_ENABLED
    float4 tangentWS : TANGENT;
    float3 binormalWS : BINORMAL;
    #endif
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(_RECEIVE_SHADOWS_ENABLED)
    float4 shadowCoord : COLOR2;
    #endif
    float3 vertexSH : COLOR3; // SH
};

/**
 * \brief Initialize the input data of fragment shader.
 * \details This function will call initialize function of UberUnlit.\n
 * After, it will execute initialize proprietary process of UberLit.
 * \param[in,out] input InputData from the vertex shader.
 */
inline void InitializeFragmentInputLit(in out VaryingsLit input)
{
    InitializeFragmentInput(input.varyingsUnlit);
    #ifdef _NORMAL_MAP_ENABLED
    input.binormalWS = normalize(input.binormalWS);
    input.tangentWS = normalize(input.tangentWS);
    #endif
}


#endif