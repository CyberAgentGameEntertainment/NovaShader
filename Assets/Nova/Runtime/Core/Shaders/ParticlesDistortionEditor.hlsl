#ifndef NOVA_PARTICLESDISTORTIONEDITOR_INCLUDED
#define NOVA_PARTICLESDISTORTIONEDITOR_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ParticlesDistortionForward.hlsl"

float _ObjectId;
float _PassValue;
float4 _SelectionID;

Varyings vertEditor(Attributes input)
{
    return vert(input);
}

half4 fragSceneHighlight(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    
    // Call frag function to perform alpha test and other visibility checks
    frag(input);
    
    return float4(_ObjectId, _PassValue, 1, 1);
}

half4 fragScenePicking(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    
    // Call frag function to perform alpha test and other visibility checks
    frag(input);
    
    return _SelectionID;
}

#endif