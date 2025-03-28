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
    float3 positionWS : COLOR1;
    #ifdef _NORMAL_MAP_ENABLED
    float4 tangentWS : TANGENT;
    float3 binormalWS : BINORMAL;
    #endif
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(_RECEIVE_SHADOWS_ENABLED)
    float4 shadowCoord : COLOR2;
    #endif
    half3 vertexSH : COLOR3; // SH

    #ifdef USE_APV_PROBE_OCCLUSION
    float4 probeOcclusion  : TEXCOORD9;
    #endif
};

#endif
