#ifndef NOVA_SWICTHABLE_BRANCH_HLSL
#define NOVA_SWICTHABLE_BRANCH_HLSL

/// <summary>
/// This file contains the switchable branch functions.
/// </summary>
/// <remarks>
/// The function contained in this file returns a dynamic 0 or 1 if ENABLE_DYNAMIC_BRANCH is enabled.
/// Conversely, if it is disabled, it returns a static 0 or 1.
/// When returning a static value, the following branch is deleted by compiler optimization.
///
/// <code>
///     if(IsKeywordEnabled_VERTEX_DEFORMATION_ENABLED()){
///     }
/// </code>
/// 
/// </remarks>

int IsKeywordEnabled_VERTEX_DEFORMATION_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
        return _VERTEX_DEFORMATION_ENABLED ; 
    #else
        #ifdef _VERTEX_DEFORMATION_ENABLED
            return 1;
        #else
            return 0;
        #endif
    #endif
}

int IsKeywordEnabled_METALLIC_MAP_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
        return _METALLIC_MAP_ENABLED; 
    #else
        #ifdef _METALLIC_MAP_ENABLED
            return 1;
        #else
            return 0;
        #endif
    #endif
}

int IsKeywordEnabled_BASE_MAP_ROTATION_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
        return _BASE_MAP_ROTATION_ENABLED; 
    #else
        #ifdef _BASE_MAP_ROTATION_ENABLED
            return 1;
        #else
            return 0;
        #endif
    #endif
}
int IsKeywordEnabled_ALPHAMODULATE_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
        return _ALPHAMODULATE_ENABLED; 
    #else
        #ifdef _ALPHAMODULATE_ENABLED
            return 1;
        #else
            return 0;
        #endif
    #endif
}

int IsKeywordEnabled_VERTEX_ALPHA_AS_TRANSITION_PROGRESS()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
        return _VERTEX_ALPHA_AS_TRANSITION_PROGRESS; 
    #else
        #ifdef _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            return 1;
        #else
            return 0;
        #endif
    #endif
}
int IsKeywordEnabled_SMOOTHNESS_MAP_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _SMOOTHNESS_MAP_ENABLED; 
    #else
    #ifdef _SMOOTHNESS_MAP_ENABLED
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_SPECULAR_MAP_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _SPECULAR_MAP_ENABLED; 
    #else
    #ifdef _SPECULAR_MAP_ENABLED
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_BASE_MAP_MODE_2D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _BASE_MAP_MODE_2D; 
    #else
    #ifdef _BASE_MAP_MODE_2D
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_BASE_MAP_MODE_2D_ARRAY()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _BASE_MAP_MODE_2D_ARRAY; 
    #else
    #ifdef _BASE_MAP_MODE_2D_ARRAY
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_BASE_MAP_MODE_3D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _BASE_MAP_MODE_3D; 
    #else
    #ifdef _BASE_MAP_MODE_3D
    return 1;
    #else
    return 0;
    #endif
    #endif
}

int IsKeywordEnabled_PARALLAX_MAP_MODE_2D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _PARALLAX_MAP_MODE_2D; 
    #else
    #ifdef _PARALLAX_MAP_MODE_2D
    return 1;
    #else
    return 0;
    #endif
    #endif
}

int IsKeywordEnabled_EMISSION_COLOR_COLOR()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _EMISSION_COLOR_COLOR; 
    #else
    #ifdef _EMISSION_COLOR_COLOR
    return 1;
    #else
    return 0;
    #endif
    #endif
}

int IsKeywordEnabled_EMISSION_COLOR_BASECOLOR()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _EMISSION_COLOR_BASECOLOR; 
    #else
    #ifdef _EMISSION_COLOR_BASECOLOR
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_EMISSION_COLOR_MAP()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _EMISSION_COLOR_MAP; 
    #else
    #ifdef _EMISSION_COLOR_MAP
    return 1;
    #else
    return 0;
    #endif
    #endif
}

int IsKeywordEnabled_PARALLAX_MAP_MODE_2D_ARRAY()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _PARALLAX_MAP_MODE_2D_ARRAY; 
    #else
    #ifdef _PARALLAX_MAP_MODE_2D_ARRAY
    return 1;
    #else
    return 0;
    #endif
    #endif
}

int IsKeywordEnabled_PARALLAX_MAP_MODE_3D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _PARALLAX_MAP_MODE_3D; 
    #else
    #ifdef _PARALLAX_MAP_MODE_3D
    return 1;
    #else
    return 0;
    #endif
    #endif
}

int IsKeywordEnabled_GREYSCALE_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _GREYSCALE_ENABLED; 
    #else
    #ifdef _GREYSCALE_ENABLED
    return 1;
    #else
    return 0;
    #endif
    #endif
}

int IsKeywordEnabled_GRADIENT_MAP_ENABLED()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _GRADIENT_MAP_ENABLED; 
    #else
    #ifdef _GRADIENT_MAP_ENABLED
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_ALPHA_TRANSITION_MAP_MODE_2D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _ALPHA_TRANSITION_MAP_MODE_2D; 
    #else
    #ifdef _ALPHA_TRANSITION_MAP_MODE_2D
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_ALPHA_TRANSITION_MAP_MODE_2D_ARRAY()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY; 
    #else
    #ifdef _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_ALPHA_TRANSITION_MAP_MODE_3D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _ALPHA_TRANSITION_MAP_MODE_3D; 
    #else
    #ifdef _ALPHA_TRANSITION_MAP_MODE_3D
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_EMISSION_MAP_MODE_2D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _EMISSION_MAP_MODE_2D; 
    #else
    #ifdef _EMISSION_MAP_MODE_2D
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_EMISSION_MAP_MODE_2D_ARRAY()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _EMISSION_MAP_MODE_2D_ARRAY; 
    #else
    #ifdef _EMISSION_MAP_MODE_2D_ARRAY
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_EMISSION_MAP_MODE_3D()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _EMISSION_MAP_MODE_3D; 
    #else
    #ifdef _EMISSION_MAP_MODE_3D
    return 1;
    #else
    return 0;
    #endif
    #endif
}
int IsKeywordEnabled_TRANSPARENCY_BY_LUMINANCE()
{
    #ifdef ENABLE_DYNAMIC_BRANCH
    return _TRANSPARENCY_BY_LUMINANCE; 
    #else
    #ifdef _TRANSPARENCY_BY_LUMINANCE
    return 1;
    #else
    return 0;
    #endif
    #endif
}
#endif // NOVA_SWICTHABLE_BRANCH
