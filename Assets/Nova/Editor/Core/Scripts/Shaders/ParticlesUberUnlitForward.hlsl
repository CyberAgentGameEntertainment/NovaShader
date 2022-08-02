#ifndef NOVA_PARTICLESUBERUNLITFORWARD_INCLUDED
#define NOVA_PARTICLESUBERUNLITFORWARD_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ParticlesUberUnlit.hlsl"

Varyings vert(Attributes input)
{
    return vertUnlit(input);
}
half4 frag(Varyings input) : SV_Target
{
    return fragUnlit(input);
}

#endif