#ifndef NOVA_PARTICLESUBERLIT_INCLUDED
#define NOVA_PARTICLESUBERLIT_INCLUDED

#include "ParticlesUberUnlit.hlsl"

// StableRandom access is handled through varyingsUnlit.stableRandomX
// No need to override GET_STABLE_RANDOM_X macro as it uses Unlit's implementation

/**
 * \brief Vertex shader input attributes for UberLit.
 */
struct AttributesLit
{
    Attributes attributesUnlit;
    #ifdef _NORMAL_MAP_ENABLED
    float4 tangentOS : TANGENT;
    #endif
    #ifndef NOVA_PARTICLE_INSTANCING_ENABLED
    // For Random Row Selection, StableRandom.x value is obtained through:
    // 1. GPU Instancing: instanceData.stableRandom.x (automatic)
    // 2. Non-GPU Instancing: Custom Coord or Unity's automatic StableRandom mapping (via Vertex Streams)
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
    float4 probeOcclusion : TEXCOORD14;
    #endif
};

#endif
