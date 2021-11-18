#ifndef CT_PARTICLESUBERUNLIT_INCLUDED
#define CT_PARTICLESUBERUNLIT_INCLUDED

#include "ParticlesUber.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float4 color : COLOR;
    float3 normalOS : NORMAL;
    float2 texcoord : TEXCOORD0;
#ifndef CT_PARTICLE_INSTANCING_ENABLED
    INPUT_CUSTOM_COORD(1, 2)
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float4 color : COLOR;
    INPUT_CUSTOM_COORD(0, 1)
    float4 baseMapUVAndProgresses : TEXCOORD2; // xy: BaseMap UV, z: Base Map Progress, w: Tint Map Progress
    float4 flowTransitionUVs : TEXCOORD3; // xy: FlowMap UV, zw: TransitionMap UV
    float4 tintEmissionUV : TEXCOORD4; // xy: TintMap UV, zw: EmissionMap UV
    float3 transitionEmissionProgresses : TEXCOORD5; // x: TransitionMap Progress, y: EmissionMap Progress, z: Fog Factor
#ifdef FRAGMENT_USE_VIEW_DIR_WS
    float3 viewDirWS : TEXCOORD6;
#endif
#ifdef FRAGMENT_USE_NORMAL_WS
    float3 normalWS : TEXCOORD7;
#endif
#ifdef USE_PROJECTED_POSITION
    float4 projectedPosition : TEXCOORD8;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

inline void InitializeVertexOutput(in Attributes input, in out Varyings output)
{
    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    output.color = GetParticleColor(input.color);;
    #ifdef FRAGMENT_USE_VIEW_DIR_WS
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.viewDirWS = GetWorldSpaceViewDir(positionWS);
    #endif
    #ifdef FRAGMENT_USE_NORMAL_WS
    output.normalWS = TransformObjectToWorldNormal(input.normalOS, true);
    #endif

    #ifdef USE_PROJECTED_POSITION
    output.projectedPosition = ComputeScreenPos(output.positionHCS);
    #endif
}

inline void InitializeFragmentInput(in out Varyings input)
{
#ifdef FRAGMENT_USE_NORMAL_WS
    input.normalWS = normalize(input.normalWS);
#endif
    
#ifdef FRAGMENT_USE_VIEW_DIR_WS
    input.viewDirWS = normalize(input.viewDirWS);
#endif
}

#endif