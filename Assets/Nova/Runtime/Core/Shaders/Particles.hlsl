#ifndef NOVA_PARTICLES_INCLUDED
#define NOVA_PARTICLES_INCLUDED

#include "ParticlesInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

#if defined(_PARALLAX_MAP_TARGET_BASE) || defined(_PARALLAX_MAP_TARGET_TINT) || defined(_PARALLAX_MAP_TARGET_EMISSION)
#define USE_PARALLAX_MAP
#endif

#if defined(_TRANSPARENCY_BY_RIM) || defined(_TINT_AREA_RIM) || defined(USE_PARALLAX_MAP)
#define FRAGMENT_USE_VIEW_DIR_WS
#endif
#if defined(_TRANSPARENCY_BY_RIM) || defined(_TINT_AREA_RIM) || defined(DEPTH_NORMALS_PASS) || defined(USE_PARALLAX_MAP)
#define FRAGMENT_USE_NORMAL_WS
#endif

#if defined(DEPTH_ONLY_PASS)
#ifndef _ALPHATEST_ENABLED
#undef FRAGMENT_USE_NORMAL_WS // This symbol is not necessary when drawing opaque objects.
#endif
#endif

#if defined(DEPTH_ONLY_PASS) || defined(DEPTH_NORMALS_PASS)
#ifndef _ALPHATEST_ENABLED
// These symbols are not necessary when drawing opaque objects.
#undef FRAGMENT_USE_VIEW_DIR_WS 
#undef _TRANSPARENCY_BY_RIM
#undef _TINT_AREA_RIM
#endif
#endif

#if defined(_SOFT_PARTICLES_ENABLED) || defined(_DEPTH_FADE_ENABLED)
#define USE_PROJECTED_POSITION
#endif

// Setup for instancing.
#ifdef NOVA_PARTICLE_INSTANCING_ENABLED
#define SETUP_VERTEX NOVA_PARTICLE_INSTANCE_DATA instanceData = unity_ParticleInstanceData[unity_InstanceID];
#define SETUP_FRAGMENT NOVA_PARTICLE_INSTANCE_DATA instanceData = unity_ParticleInstanceData[unity_InstanceID];
#else
#define SETUP_VERTEX
#define SETUP_FRAGMENT
#endif

// Custom Coords
#define DECLARE_CUSTOM_COORD(propertyName) uniform half propertyName;
#define INPUT_CUSTOM_COORD(index1, index2) float4 customCoord1: TEXCOORD##index1; \
    float4 customCoord2: TEXCOORD##index2;

#ifdef NOVA_PARTICLE_INSTANCING_ENABLED
#define SETUP_CUSTOM_COORD(input) float4 customCoords[] = \
{ \
float4(0.0, 0.0, 0.0, 0.0), \
instanceData.customCoord1, \
instanceData.customCoord2 \
};
#else
#define SETUP_CUSTOM_COORD(input) float4 customCoords[] = \
{ \
float4(0.0, 0.0, 0.0, 0.0), \
input.customCoord1, \
input.customCoord2 \
};
#endif

#ifdef NOVA_PARTICLE_INSTANCING_ENABLED
#define TRANSFER_CUSTOM_COORD(input, output) output.customCoord1 = instanceData.customCoord1; \
output.customCoord2 = instanceData.customCoord2;
#else
#define TRANSFER_CUSTOM_COORD(input, output) output.customCoord1 = input.customCoord1; \
output.customCoord2 = input.customCoord2;
#endif
#define GET_CUSTOM_COORD(propertyName) customCoords[(uint)propertyName % 10][(uint)propertyName / 10];
#define GET_CUSTOM_COORD_DIRECT(coordIndex, swizzleIndex) customCoords[coordIndex][swizzleIndex];

// Base Map Sampler State Override
#if defined(_BASE_SAMPLER_STATE_POINT_MIRROR) || defined(_BASE_SAMPLER_STATE_LINEAR_MIRROR) || defined(_BASE_SAMPLER_STATE_TRILINEAR_MIRROR)
#define BASE_SAMPLER_STATE_OVERRIDE_ENABLED
#endif
#ifdef BASE_SAMPLER_STATE_OVERRIDE_ENABLED
#ifdef _BASE_SAMPLER_STATE_POINT_MIRROR
#define BASE_SAMPLER_STATE_NAME sampler_point_mirror
#elif _BASE_SAMPLER_STATE_LINEAR_MIRROR
#define BASE_SAMPLER_STATE_NAME sampler_linear_mirror
#elif _BASE_SAMPLER_STATE_TRILINEAR_MIRROR
#define BASE_SAMPLER_STATE_NAME sampler_trilinear_mirror
#endif
SamplerState BASE_SAMPLER_STATE_NAME;
#endif

half4 GetParticleColor(half4 color)
{
    #if defined(NOVA_PARTICLE_INSTANCING_ENABLED)
    NOVA_PARTICLE_INSTANCE_DATA data = unity_ParticleInstanceData[unity_InstanceID];
    color = lerp(half4(1.0, 1.0, 1.0, 1.0), color, unity_ParticleUseMeshColors);
    color *= UnpackFromR8G8B8A8(data.color);
    #endif
    return color;
}

float2 RotateUV(float2 uv, half angle, half2 offsets)
{
    half angleCos = cos(angle);
    half angleSin = sin(angle);
    half2x2 rotateMatrix = half2x2(angleCos, -angleSin, angleSin, angleCos);
    half2 UvOffsets = 0.5 - offsets;
    return mul(uv - UvOffsets, rotateMatrix) + UvOffsets;
}

// Adjust the albedo according to the blending.
half3 ApplyAlpha(half3 albedo, half alpha)
{
    #if defined(_ALPHAMODULATE_ENABLED)
    // In multiply, albedo needs to be white if the alpha is zero.
    return lerp(half3(1.0h, 1.0h, 1.0h), albedo, alpha);
    #endif
    return albedo;
}

// Returns the soft particles value.
float SoftParticles(float4 projection, half intensity)
{
    float2 depthTextureUv = UnityStereoTransformScreenSpaceTex(projection.xy / projection.w);
    float sceneDepthCS = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, depthTextureUv).r;
    float sceneDepth = LinearEyeDepth(sceneDepthCS, _ZBufferParams);
    float depth = LinearEyeDepth(projection.z / projection.w, _ZBufferParams);
    half inverseIntensity = 1.0 / intensity;
    return saturate((sceneDepth - depth) * inverseIntensity);
}

// Returns the luminance.
// The function name suffered from Unity's built-in function. 
float GetLuminance(float3 color)
{
    return dot(color.rgb, float3(0.298912, 0.586611, 0.114478));
}

// Returns the depth fade value.
half DepthFade(float near, float far, float width, float4 projection)
{
    float depth = LinearEyeDepth(projection.z / projection.w, _ZBufferParams);
    float length = min(depth - near, far - depth);
    return saturate(length / width);
}

// Apply alpha clipping.
void AlphaClip(real alpha, real cutoff, real offset = 0.0h)
{
    #ifdef _ALPHATEST_ENABLED
    clip(alpha - cutoff + offset);
    #endif
}

// Get UV offset values by flow map.
half2 GetFlowMapUvOffset(TEXTURE2D_PARAM(flowMap, sampler_flowMap),
                         in float intensity, in float2 flowMapUv, in half flowMapChannlesX, in half flowMapChannelsY)
{
    #if defined(_FLOW_MAP_ENABLED) || defined(_FLOW_MAP_TARGET_BASE) || defined(_FLOW_MAP_TARGET_TINT) || defined(_FLOW_MAP_TARGET_EMISSION) || defined(_FLOW_MAP_TARGET_ALPHA_TRANSITION)
    half4 flowSrc = SAMPLE_TEXTURE2D(flowMap, sampler_flowMap, flowMapUv);
    half2 flow;
    flow.x = flowSrc[(uint)flowMapChannlesX];
    flow.y = flowSrc[(uint)flowMapChannelsY];
    flow = flow * 2 - 1;
    flow *= intensity;
    return flow;
    #endif
}

// Returns the progress for flip-book.
half FlipBookProgress(in half progress, in half sliceCount)
{
    float result = progress;
    result = clamp(result, 0, 0.999);
    result = frac(result);
    result *= sliceCount;
    result -= 0.5;
    return result;
}

// Returns the progress for flip-book blending.
half FlipBookBlendingProgress(in half progress, in half sliceCount)
{
    float result = progress;
    float baseMapProgressOffset = 1.0 / sliceCount * 0.5;
    result = clamp(result, 0, 1.0);
    result = lerp(baseMapProgressOffset, 1.0 - baseMapProgressOffset, result);
    return result;
}

// Get vertex deformation intensity by vertex deformation map
half GetVertexDeformationIntensity(
    TEXTURE2D_PARAM(vertexDeformationMap, sampler_vertexDeformationMap),
    in float intensity,
    in float2 uv,
    in half mapChannel,
    in float baseValue)
{
    #if defined(_VERTEX_DEFORMATION_ENABLED)
    half4 vertexDeformation = SAMPLE_TEXTURE2D_LOD(vertexDeformationMap, sampler_vertexDeformationMap, uv, 0);
    float mapIntensity = vertexDeformation[(uint)mapChannel] - baseValue;
    mapIntensity *= intensity;
    return mapIntensity;
    #endif
}

#endif
