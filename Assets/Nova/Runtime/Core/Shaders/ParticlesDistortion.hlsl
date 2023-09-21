#ifndef NOVA_PARTICLESDISTORTION_INCLUDED
#define NOVA_PARTICLESDISTORTION_INCLUDED

#include "Particles.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD0;
    #ifndef NOVA_PARTICLE_INSTANCING_ENABLED
    INPUT_CUSTOM_COORD(1, 2)
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    INPUT_CUSTOM_COORD(0, 1)
    float2 baseUv : TEXCOORD2;
    float4 flowTransitionUVs : TEXCOORD3; // xy: FlowMap UV, zw: TransitionMap UV
    float4 projectedPosition: TEXCOORD4;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);
TEXTURE2D(_FlowMap);
SAMPLER(sampler_FlowMap);
TEXTURE2D(_AlphaTransitionMap);
SAMPLER(sampler_AlphaTransitionMap);

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
DECLARE_CUSTOM_COORD(_BaseMapOffsetXCoord);
DECLARE_CUSTOM_COORD(_BaseMapOffsetYCoord);
half _BaseMapChannelsX;
half _BaseMapChannelsY;

float _BaseMapRotation;
DECLARE_CUSTOM_COORD(_BaseMapRotationCoord);
float4 _BaseMapRotationOffsets;
float _DistortionIntensity;
float _DistortionIntensityCoord;

float4 _FlowMap_ST;
DECLARE_CUSTOM_COORD(_FlowMapOffsetXCoord);
DECLARE_CUSTOM_COORD(_FlowMapOffsetYCoord);
half _FlowMapChannelsX;
half _FlowMapChannelsY;

float _FlowIntensity;
DECLARE_CUSTOM_COORD(_FlowIntensityCoord);

float4 _AlphaTransitionMap_ST;
DECLARE_CUSTOM_COORD(_AlphaTransitionMapOffsetXCoord);
DECLARE_CUSTOM_COORD(_AlphaTransitionMapOffsetYCoord);
half _AlphaTransitionMapChannelsX;

float _AlphaTransitionProgress;
DECLARE_CUSTOM_COORD(_AlphaTransitionProgressCoord);
float _DissolveSharpness;

float _SoftParticlesIntensity;
float _DepthFadeNear;
float _DepthFadeFar;
float _DepthFadeWidth;
CBUFFER_END

#endif
