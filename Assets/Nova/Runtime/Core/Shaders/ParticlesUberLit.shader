Shader "Nova/Particles/UberLit"
{
    Properties
    {
        // Render Settings
        _RenderType("Render Type", Float) = 2.0
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _TransparentBlendMode("Blend Mode", Float) = 0.0
        _Cull("Cull", Float) = 2.0
        _QueueOffset("Queue Offset", Float) = 0.0
        _VertexAlphaMode("Vertex Alpha Mode", Float) = 0.0
        _BlendSrc("Blend Src", Float) = 1.0
        _BlendDst("Blend Dst", Float) = 0.0
        _ZWrite("ZWrite", Float) = 1.0
        _ZWriteOverride("ZWrite Override", Float) = -1.0
        _ZTest("ZTest", Float) = 4.0
        _LitWorkflowMode("Lit Workflow Mode",Float) = 0.0
        _LitReceiveShadows("Lit Receive Shadows", Float) = 0.0
        _SpecularHighlights("Specular Highlights", Float) = 0.0
        _EnvironmentReflections("Environment Reflections", Float) = 1.0

        // Surface Maps
        _NormalMap("Normal Map", 2D) = "" {}
        _NormalMap2DArray("Normal Map 2D Array", 2DArray) = "" {}
        _NormalMap3D("Normal Map 3D", 3D) = "" {}
        _NormalMapBumpScale("Normal Map Bump Scale", Float) = 1.0
        _SpecularMap("Specular Map", 2D) = "" {}
        _SpecularMap2DArray("Specular Map 2D Array", 2DArray) = "" {}
        _SpecularMap3D("Specluar Map 3D", 3D) = "" {}
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularMapChannelsX("Specular Map Channes X", Float) = 0.0
        _MetallicMap("Metallic Map", 2D) = "" {}
        _MetallicMap2DArray("Metallic Map 2D Array", 2DArray) = "" {}
        _MetallicMap3D("Metallic Map 3D", 3D) = "" {}
        [Gamma]_Metallic("Metallic", Range( 0.0, 1.0)) = 1.0
        _MetallicMapChannelsX("Metallic Map Channes X", Float) = 0.0
        _SmoothnessMap("Smoothness Map", 2D) = "" {}
        _SmoothnessMap2DArray("Smoothness Map 2D Array", 2DArray) = "" {}
        _SmoothnessMap3D("Smoothness Map 3D", 3D) = "" {}
        _Smoothness("Smoothness", Range( 0.0, 1.0)) = 1.0
        _SmoothnessMapChannelsX("Smoothness Map Channes X", Float) = 3.0
        // Base Map
        _BaseMapMode("Base Map Mode", Float) = 0.0
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _BaseMap2DArray("Base Map 2D Array", 2DArray) = "" {}
        _BaseMap3D("Base Map 3D", 3D) = "" {}
        _BaseMapProgress("Base Map Progress", Range(0.0, 1.0)) = 0.0
        _BaseMapProgressCoord("Base Map Progress Coord", Float) = 0.0
        _BaseMapSliceCount("Base Map Slice Count", Float) = 4.0
        _BaseMapOffsetXCoord("Base Map Offset X Coord", Float) = 0.0
        _BaseMapOffsetYCoord("Base Map Offset Y Coord", Float) = 0.0
        _BaseMapRotation("Base Map Rotation", Range(0.0, 1.0)) = 0.0
        _BaseMapRotationCoord("Base Map Rotation Coord", Float) = 0.0
        _BaseMapRotationOffsets("Base Map Rotation Offsets", Vector) = (0.0, 0.0, 0.0, 0.0)
        _BaseMapMirrorSampling("Base Map Mirror Sampling", Float) = 0.0

        // Tint Color
        _TintAreaMode("Tint Area Mode", Float) = 0.0
        _TintColorMode("Tint Color Mode", Float) = 0.0
        [HDR] _TintColor("Tint Color", Color) = (1, 1, 1, 1)
        _TintMap("Tint Map", 2D) = "white" {}
        _TintMap3D("Tint Map 3D", 3D) = "" {}
        _TintMap3DProgress("Tint Map 3D Progress", Range(0, 1)) = 0.0
        _TintMap3DProgressCoord("Tint Map 3D Progress Coord", Float) = 0.0
        _TintMapSliceCount("Base Map Slice Count", Float) = 4.0
        _TintMapOffsetXCoord("Tint Map Offset X Coord", Float) = 0.0
        _TintMapOffsetYCoord("Tint Map Offset Y Coord", Float) = 0.0
        _TintBlendRate("Tint Blend Rate", Range(0.0, 1.0)) = 1.0
        _TintBlendRateCoord("Tint Blend Rate Coord", Float) = 0.0
        _TintRimProgress("Tint Rim Progress", Range(0.0, 1.0)) = 0.5
        _TintRimProgressCoord("Tint Rim Progress Coord", Float) = 0.0
        _TintRimSharpness("Tint Rim Sharpness", Range(0.0, 1.0)) = 0.5
        _TintRimSharpnessCoord("Tint Rim Sharpness Coord", Float) = 0.0
        _InverseTintRim("Inverse Tint Rim", Float) = 0.0

        // Flow Map
        _FlowMap("Flow Map", 2D) = "grey" {}
        _FlowMapOffsetXCoord("Flow Map Offset X Coord", Float) = 0.0
        _FlowMapOffsetYCoord("Flow Map Offset Y Coord", Float) = 0.0
        _FlowMapChannelsX("Flow Map Channes X", Float) = 0.0
        _FlowMapChannelsY("Flow Map Channes Y", Float) = 1.0
        _FlowIntensity("Flow Intensity", Float) = 1.0
        _FlowIntensityCoord("Flow Intensity Coord", Float) = 0.0
        _FlowMapTarget("Flow Map Target", Float) = 1.0

        // Parallax Map
        _ParallaxMapMode("Emission Map Mode", Float) = 0.0
        _ParallaxMap("Parallax Map", 2D) = "" {}
        _ParallaxMap2DArray("Parallax Map 2D Array", 2DArray) = "" {}
        _ParallaxMap3D("Parallax Map 3D", 3D) = "" {}
        _ParallaxMapProgress("Parallax Map Progress", Range(0.0, 1.0)) = 0.0
        _ParallaxMapProgressCoord("Parallax Map Progress Coord", Float) = 0.0
        _ParallaxMapOffsetXCoord("Parallax Map Offset X Coord", Float) = 0.0
        _ParallaxMapOffsetYCoord("Parallax Map Offset Y Coord", Float) = 0.0
        _ParallaxMapSliceCount("Parallax Map Slice Count", Float) = 4.0
        _ParallaxMapChannel("Parallax Map Channel", Float) = 0.0
        _ParallaxStrength("Parallax Strength", Range(0.0, 1.0)) = 0.3
        _ParallaxMapTarget("Parallax Map Target", Float) = 1.0

        // Color Correction
        _ColorCorrectionMode("Alpha Transition Progress Coord", Float) = 0.0
        _GradientMap("Gradient Map", 2D) = "white" {}

        // Alpha Transition
        _AlphaTransitionMode("Alpha Transition Mode", Float) = 0.0
        _AlphaTransitionMapMode("Alpha Transition Map Mode", Float) = 0.0
        _AlphaTransitionMap("Alpha Transition Map", 2D) = "white" {}
        _AlphaTransitionMap2DArray("Alpha Transition Map 2D Array", 2DArray) = "" {}
        _AlphaTransitionMap3D("Alpha Transition Map 3D", 3D) = "" {}
        _AlphaTransitionMapProgress("Alpha　Transition Map Progress", Range(0.0, 1.0)) = 0.0
        _AlphaTransitionMapProgressCoord("Alpha　Transition Map Progress Coord", Float) = 0.0
        _AlphaTransitionMapOffsetXCoord("Alpha Transition Map Offset X Coord", Float) = 0.0
        _AlphaTransitionMapOffsetYCoord("Alpha Transition Map Offset Y Coord", Float) = 0.0
        _AlphaTransitionMapChannelsX("Alpha Transition Map Channes X", Float) = 0.0
        _AlphaTransitionMapSliceCount("Alpha Transition Map Slice Count", Float) = 4.0
        _AlphaTransitionProgress("Alpha Transition Progress", Range(0.0, 1.0)) = 0.0
        _AlphaTransitionProgressCoord("Alpha Transition Progress Coord", Float) = 0.0
        _DissolveSharpness("Dissolve Sharpness", Range(0.0, 1.0)) = 0.5
        _AlphaTransitionSecondTextureBlendMode("Alpha Transition Second Texture BlendMode", Float) = 0.0
        _AlphaTransitionMapSecondTexture("Alpha Transition Second Texture Map", 2D) = "white" {}
        _AlphaTransitionMapSecondTexture2DArray("Alpha Transition Second Texture Map 2D Array", 2DArray) = "" {}
        _AlphaTransitionMapSecondTexture3D("Alpha Transition Second Texture Map 3D", 3D) = "" {}
        _AlphaTransitionMapSecondTextureProgress("Alpha　Transition Map Second Texture  Progress", Range(0.0, 1.0)) = 0.0
        _AlphaTransitionMapSecondTextureProgressCoord("Alpha　Transition Map Second Texture  Progress Coord", Float) = 0.0
        _AlphaTransitionMapSecondTextureOffsetXCoord("Alpha Transition Second Texture Map Offset X Coord", Float) = 0.0
        _AlphaTransitionMapSecondTextureOffsetYCoord("Alpha Transition Second Texture Map Offset Y Coord", Float) = 0.0
        _AlphaTransitionMapSecondTextureChannelsX("Alpha Transition Second Texture Map Channes X", Float) = 0.0
        _AlphaTransitionMapSecondTextureSliceCount("Alpha Transition Map Second Texture Slice Count", Float) = 4.0
        _AlphaTransitionProgressSecondTexture("Alpha Transition Second Texture Progress", Range(0.0, 1.0)) = 0.0
        _AlphaTransitionProgressCoordSecondTexture("Alpha Transition Second Texture Progress Coord", Float) = 0.0
        _DissolveSharpnessSecondTexture("Dissolve Sharpness", Range(0.0, 1.0)) = 0.5

        // Emission
        _EmissionAreaType("Emission Area Type", Float) = 0.0
        _EmissionMapMode("Emission Map Mode", Float) = 0.0
        _EmissionMap("Emission Map", 2D) = "black" {}
        _EmissionMap2DArray("Emission Map 2D Array", 2DArray) = "" {}
        _EmissionMap3D("Emission Map 3D", 3D) = "" {}
        _EmissionMapProgress("Emission Map Progress", Range(0.0, 1.0)) = 0.0
        _EmissionMapProgressCoord("Emission Map Progress Coord", Float) = 0.0
        _EmissionMapOffsetXCoord("Emission Map Offset X Coord", Float) = 0.0
        _EmissionMapOffsetYCoord("Emission Map Offset Y Coord", Float) = 0.0
        _EmissionMapChannelsX("Emission Map Channels X", Float) = 0.0
        _EmissionMapSliceCount("Alpha Transition Map Slice Count", Float) = 4.0
        _EmissionColorType("Emission Color Type", Float) = 0.0
        [HDR]_EmissionColor("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionColorRamp("Emission Color Ramp", 2D) = "black" {}
        _EmissionIntensity("Emission Intensity", Float) = 1.0
        _EmissionIntensityCoord("Emission Intensity Coord", Float) = 0.0
        _KeepEdgeTransparency("Keep Edge Transparency", Float) = 1.0

        // Transparency
        _RimTransparencyEnabled("Rim Transparency Enabled", Float) = 0.0
        _RimTransparencyProgress("Rim Transparency Progress", Range(0.0, 1.0)) = 0.5
        _RimTransparencyProgressCoord("Rim Transparency Progress Coord", Float) = 0.0
        _RimTransparencySharpness("Rim Transparency Sharpness", Range(0.0, 1.0)) = 0.5
        _RimTransparencySharpnessCoord("Rim Transparency Sharpness Coord", Float) = 0.0
        _InverseRimTransparency("Inverse Rim Transparency", Float) = 0.0
        _LuminanceTransparencyEnabled("Luminance Transparency Enabled", Float) = 0.0
        _LuminanceTransparencyProgress("Luminance Transparency Progress", Range(0.0, 1.0)) = 0.5
        _LuminanceTransparencyProgressCoord("Luminance Transparency Progress Coord", Float) = 0.0
        _LuminanceTransparencySharpness("Luminance Transparency Sharpness", Range(0.0, 1.0)) = 0.5
        _LuminanceTransparencySharpnessCoord("Luminance Transparency Sharpness Coord", Float) = 0.0
        _InverseLuminanceTransparency("Inverse Luminance Transparency", Float) = 0.0
        _SoftParticlesEnabled("Soft Particles Enabled", Float) = 0.0
        _SoftParticlesIntensity("Soft Particles Intensity", Float) = 1.0
        _DepthFadeEnabled("Depth Fade Enabled", Float) = 0.0
        _DepthFadeNear("Depth Fade Near", Float) = 1.0
        _DepthFadeFar("Depth Fade Far", Float) = 10.0
        _DepthFadeWidth("Depth Fade Width", Float) = 1.0

        // Vertex Deformation
        _VertexDeformationEnabled ("Vertex Deformation Enabled", Float) = 0
        _VertexDeformationMap ("Vertex Deformation Map", 2D) = "white" {}
        _VertexDeformationMapOffsetXCoord("VertexDeformation Map Offset X Coord", Float) = 0.0
        _VertexDeformationMapOffsetYCoord("VertexDeformation Map Offset Y Coord", Float) = 0.0
        _VertexDeformationMapChannel("VertexDeformation Map Channel", Float) = 0.0
        _VertexDeformationIntensity("VertexDeformation Intensity", Float) = 0.1
        _VertexDeformationIntensityCoord("VertexDeformation Intensity Coord", Float) = 0.0
        _VertexDeformationBaseValue("Vertex Deformation Base Value", Range(0.0, 1.0)) = 0

        // Shadow Caster
        _ShadowCasterEnabled("Shadow Caster", Float) = 0
        _ShadowCasterApplyVertexDeformation("Shadow Caster Vertex Deformation Enabled", Float) = 0
        _ShadowCasterAlphaTestEnabled("Shadow Caster Alpha Test Enabled", Float) = 0
        _ShadowCasterAlphaCutoff("Shadow Caster Alpha Test Cutoff", Range(0.0, 1.0)) = 0.5
        _ShadowCasterAlphaAffectedByTintColor("Shadow Caster Alpha Effect By Tint Color", Float) = 0
        _ShadowCasterAlphaAffectedByFlowMap("Shadow Caster Alpha Effect By Flow Map", Float) = 0
        _ShadowCasterAlphaAffectedByAlphaTransitionMap("Shadow Caster Alpha Effect By Alpha Transition Map", Float) = 0
        _ShadowCasterAlphaAffectedByTransparencyLuminance("Shadow Caster Alpha Effect By Transparency Luminance", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "PerformanceChecks" = "False"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            Blend [_BlendSrc] [_BlendDst]
            ZWrite[_ZWrite]
            Cull[_Cull]
            ColorMask RGB
            Lighting Off
            ZTest [_ZTest]

            HLSLPROGRAM

            #include "Config.hlsl"
            
            #pragma vertex vertLit
            #pragma fragment fragLit
            #pragma target 3.5
            
            // Unity Defined
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma instancing_options procedural:ParticleInstancingSetup
            #pragma require 2darray

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            // Render Settings
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #pragma dynamic_branch_local_fragment _ALPHAMODULATE_ENABLED
            #else
            #pragma shader_feature_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #pragma shader_feature_local_fragment _ALPHAMODULATE_ENABLED
            #endif
            
            #pragma shader_feature_local_fragment _ALPHATEST_ENABLED

            #pragma shader_feature_local _RECEIVE_SHADOWS_ENABLED
            #pragma shader_feature_local _SPECULAR_HIGHLIGHTS_ENABLED
            #pragma shader_feature_local _ENVIRONMENT_REFLECTIONS_ENABLED
            #pragma shader_feature_local _SPECULAR_SETUP

            // Surface maps
            #pragma shader_feature_local _NORMAL_MAP_ENABLED
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _METALLIC_MAP_ENABLED
            #pragma dynamic_branch_local_fragment _SMOOTHNESS_MAP_ENABLED
            #pragma dynamic_branch_local_fragment _SPECULAR_MAP_ENABLED
            #else
            #pragma shader_feature_local_fragment _METALLIC_MAP_ENABLED
            #pragma shader_feature_local_fragment _SMOOTHNESS_MAP_ENABLED
            #pragma shader_feature_local_fragment _SPECULAR_MAP_ENABLED
            #endif
            
            // Base Map
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma dynamic_branch_local_vertex _BASE_MAP_ROTATION_ENABLED
            #else
            #pragma shader_feature_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma shader_feature_local_vertex _BASE_MAP_ROTATION_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ _BASE_SAMPLER_STATE_POINT_MIRROR _BASE_SAMPLER_STATE_LINEAR_MIRROR _BASE_SAMPLER_STATE_TRILINEAR_MIRROR

            // Tint Color
            #pragma shader_feature_local _ _TINT_AREA_ALL _TINT_AREA_RIM // Cannot be converted to Dynamic Branch."
            #pragma shader_feature_local _ _TINT_COLOR_ENABLED _TINT_MAP_ENABLED _TINT_MAP_3D_ENABLED // Cannot be converted to Dynamic Branch."

            // Flow Map(// Cannot be converted to Dynamic Branch.)
            #pragma shader_feature_local _FLOW_MAP_ENABLED // Obsolete, but retained for compatibility.
            #pragma shader_feature_local _FLOW_MAP_TARGET_BASE
            #pragma shader_feature_local _FLOW_MAP_TARGET_TINT
            #pragma shader_feature_local _FLOW_MAP_TARGET_EMISSION
            #pragma shader_feature_local _FLOW_MAP_TARGET_ALPHA_TRANSITION

            // Parallax Map
            #pragma shader_feature_local _PARALLAX_MAP_TARGET_BASE // Cannot be converted to Dynamic Branch.
            #pragma shader_feature_local _PARALLAX_MAP_TARGET_TINT // Cannot be converted to Dynamic Branch.
            #pragma shader_feature_local _PARALLAX_MAP_TARGET_EMISSION // Cannot be converted to Dynamic Branch.
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _PARALLAX_MAP_MODE_2D _PARALLAX_MAP_MODE_2D_ARRAY _PARALLAX_MAP_MODE_3D
            #else
            #pragma shader_feature_local _PARALLAX_MAP_MODE_2D _PARALLAX_MAP_MODE_2D_ARRAY _PARALLAX_MAP_MODE_3D
            #endif
            
            // Color Correction
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED
            #else
            #pragma shader_feature_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED
            #endif

            // Alpha Transition
            #pragma shader_feature_local _ _FADE_TRANSITION_ENABLED _DISSOLVE_TRANSITION_ENABLED
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #endif
            #pragma shader_feature_local _ _ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE _ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY

            // Emission
            #pragma shader_feature_local _ _EMISSION_AREA_ALL _EMISSION_AREA_MAP _EMISSION_AREA_ALPHA

            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            #pragma dynamic_branch_local_fragment _EMISSION_COLOR_COLOR _EMISSION_COLOR_BASECOLOR _EMISSION_COLOR_MAP
            #else
            #pragma shader_feature_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            #pragma shader_feature_local_fragment _ _EMISSION_COLOR_COLOR _EMISSION_COLOR_BASECOLOR _EMISSION_COLOR_MAP
            #endif

            // Transparency
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _TRANSPARENCY_BY_LUMINANCE 
            #else
            #pragma shader_feature_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #endif

            #pragma shader_feature_local _TRANSPARENCY_BY_RIM
            #pragma shader_feature_local _SOFT_PARTICLES_ENABLED
            #pragma shader_feature_local _DEPTH_FADE_ENABLED

            // Vertex Deformation
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_vertex _VERTEX_DEFORMATION_ENABLED
            #else
            #pragma shader_feature_local_vertex _VERTEX_DEFORMATION_ENABLED
            #endif

            #include "ParticlesUberLitForward.hlsl"
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }

            BlendOp Add
            Blend One Zero
            ZWrite On
            Cull Off

            HLSLPROGRAM

            #include "Config.hlsl"
            
            #pragma vertex vertEditor
            #pragma fragment fragSceneHighlight
            #pragma target 3.5

            // Unity Defined
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup
            #pragma require 2darray

            // Render Settings
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #pragma dynamic_branch_local_fragment _ALPHAMODULATE_ENABLED
            #else
            #pragma shader_feature_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #pragma shader_feature_local_fragment _ALPHAMODULATE_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ALPHATEST_ENABLED

            // Base Map
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma dynamic_branch_local_vertex _BASE_MAP_ROTATION_ENABLED
            #else
            #pragma shader_feature_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma shader_feature_local_vertex _BASE_MAP_ROTATION_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ _BASE_SAMPLER_STATE_POINT_MIRROR _BASE_SAMPLER_STATE_LINEAR_MIRROR _BASE_SAMPLER_STATE_TRILINEAR_MIRROR


            // Tint Color
            #pragma shader_feature_local _ _TINT_AREA_ALL _TINT_AREA_RIM
            #pragma shader_feature_local _ _TINT_COLOR_ENABLED _TINT_MAP_ENABLED _TINT_MAP_3D_ENABLED

            // Flow Map
            #pragma shader_feature_local _FLOW_MAP_ENABLED // Obsolete, but retained for compatibility.
            #pragma shader_feature_local _FLOW_MAP_TARGET_BASE
            #pragma shader_feature_local _FLOW_MAP_TARGET_TINT
            #pragma shader_feature_local _FLOW_MAP_TARGET_EMISSION
            #pragma shader_feature_local _FLOW_MAP_TARGET_ALPHA_TRANSITION

            // Parallax Map
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_BASE
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_TINT
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_EMISSION
            // #pragma shader_feature_local _PARALLAX_MAP_MODE_2D _PARALLAX_MAP_MODE_2D_ARRAY _PARALLAX_MAP_MODE_3D

            // Color Correction
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED
            #else
            #pragma shader_feature_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED
            #endif

            // Alpha Transition
            #pragma shader_feature_local _ _FADE_TRANSITION_ENABLED _DISSOLVE_TRANSITION_ENABLED
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #endif
            #pragma shader_feature_local _ _ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE _ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY

            // Emission
            //#pragma shader_feature_local _ _EMISSION_AREA_ALL _EMISSION_AREA_MAP _EMISSION_AREA_ALPHA
            //#pragma shader_feature_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            //#pragma shader_feature_local _ _EMISSION_COLOR_COLOR _EMISSION_COLOR_BASECOLOR _EMISSION_COLOR_MAP

            // Transparency
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #else
            #pragma shader_feature_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #endif

            #pragma shader_feature_local _TRANSPARENCY_BY_RIM
            #pragma shader_feature_local _SOFT_PARTICLES_ENABLED
            #pragma shader_feature_local _DEPTH_FADE_ENABLED

            // Vertex Deformation
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_vertex _VERTEX_DEFORMATION_ENABLED
            #else
            #pragma shader_feature_local_vertex _ _VERTEX_DEFORMATION_ENABLED
            #endif

            #ifdef ENABLE_DYNAMIC_BRANCH
            #define _METALLIC_MAP_ENABLED 0
            #define _SMOOTHNESS_MAP_ENABLED 0
            #define _SPECULAR_MAP_ENABLED 0
            #define _PARALLAX_MAP_MODE_2D 0
            #define _PARALLAX_MAP_MODE_2D_ARRAY 0
            #define _PARALLAX_MAP_MODE_3D 0
            #define _EMISSION_COLOR_COLOR 0
            #define _EMISSION_COLOR_BASECOLOR 0
            #define _EMISSION_COLOR_MAP 0
            #define _EMISSION_MAP_MODE_2D 0
            #define _EMISSION_MAP_MODE_2D_ARRAY 0
            #define _EMISSION_MAP_MODE_3D 0
            #endif
            
            // When LightMode is SceneSelectionPass, the shaders are the same as in the Unlit version,
            // so there is no problem.
            #include "ParticlesUberUnlitEditor.hlsl"
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "Picking"
            }

            BlendOp Add
            Blend One Zero
            ZWrite On
            Cull Off

            HLSLPROGRAM

            #include "Config.hlsl"
            
            #pragma vertex vertEditor
            #pragma fragment fragScenePicking
            #pragma target 3.5

            // Unity Defined
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup
            #pragma require 2darray

            // Render Settings
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #pragma dynamic_branch_local_fragment _ALPHAMODULATE_ENABLED
            #else
            #pragma shader_feature_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #pragma shader_feature_local_fragment _ALPHAMODULATE_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ALPHATEST_ENABLED

            // Base Map
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma dynamic_branch_local_vertex _BASE_MAP_ROTATION_ENABLED
            #else
            #pragma shader_feature_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma shader_feature_local_vertex _BASE_MAP_ROTATION_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ _BASE_SAMPLER_STATE_POINT_MIRROR _BASE_SAMPLER_STATE_LINEAR_MIRROR _BASE_SAMPLER_STATE_TRILINEAR_MIRROR

            // Tint Color
            #pragma shader_feature_local _ _TINT_AREA_ALL _TINT_AREA_RIM
            #pragma shader_feature_local _ _TINT_COLOR_ENABLED _TINT_MAP_ENABLED _TINT_MAP_3D_ENABLED

            // Flow Map
            #pragma shader_feature_local _FLOW_MAP_ENABLED  // Obsolete, but retained for compatibility.
            #pragma shader_feature_local _FLOW_MAP_TARGET_BASE
            #pragma shader_feature_local _FLOW_MAP_TARGET_TINT
            #pragma shader_feature_local _FLOW_MAP_TARGET_EMISSION
            #pragma shader_feature_local _FLOW_MAP_TARGET_ALPHA_TRANSITION

            // Parallax Map
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_BASE
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_TINT
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_EMISSION
            // #pragma shader_feature_local _PARALLAX_MAP_MODE_2D _PARALLAX_MAP_MODE_2D_ARRAY _PARALLAX_MAP_MODE_3D

            // Color Correction
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED
            #else
            #pragma shader_feature_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED
            #endif

            // Alpha Transition
            #pragma shader_feature_local _ _FADE_TRANSITION_ENABLED _DISSOLVE_TRANSITION_ENABLED
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #endif
            #pragma shader_feature_local _ _ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE _ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY

            // Emission
            //#pragma shader_feature_local _ _EMISSION_AREA_ALL _EMISSION_AREA_MAP _EMISSION_AREA_ALPHA
            //#pragma shader_feature_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            //#pragma shader_feature_local _ _EMISSION_COLOR_COLOR _EMISSION_COLOR_BASECOLOR _EMISSION_COLOR_MAP

            // Transparency
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #else
            #pragma shader_feature_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #endif

            #pragma shader_feature_local _TRANSPARENCY_BY_RIM
            #pragma shader_feature_local _SOFT_PARTICLES_ENABLED
            #pragma shader_feature_local _DEPTH_FADE_ENABLED

            // Vertex Deformation
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_vertex _VERTEX_DEFORMATION_ENABLED
            #else
            #pragma shader_feature_local_vertex _ _VERTEX_DEFORMATION_ENABLED
            #endif

            #ifdef ENABLE_DYNAMIC_BRANCH
            #define _METALLIC_MAP_ENABLED 0
            #define _SMOOTHNESS_MAP_ENABLED 0
            #define _SPECULAR_MAP_ENABLED 0
            #define _PARALLAX_MAP_MODE_2D 0
            #define _PARALLAX_MAP_MODE_2D_ARRAY 0
            #define _PARALLAX_MAP_MODE_3D 0
            #define _EMISSION_COLOR_COLOR 0
            #define _EMISSION_COLOR_BASECOLOR 0
            #define _EMISSION_COLOR_MAP 0
            #define _EMISSION_MAP_MODE_2D 0
            #define _EMISSION_MAP_MODE_2D_ARRAY 0
            #define _EMISSION_MAP_MODE_3D 0
            #endif
            
            // When LightMode is Picking, the shaders are the same as in the Unlit version,
            // so there is no problem.
            #include "ParticlesUberUnlitEditor.hlsl"
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite On
            Cull[_Cull]
            ColorMask RGB
            Lighting Off
            ZTest LEqual

            HLSLPROGRAM

            #include "Config.hlsl"
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            // Unity Defined
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup
            #pragma require 2darray

            // Render Settings
            // NOTE : Not need in DepthNormals pass.
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #else
            #pragma shader_feature_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #endif
            // #pragma shader_feature_local_fragment _ALPHAMODULATE_ENABLED
            
            #pragma shader_feature_local _ALPHATEST_ENABLED

            // Base Map
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma dynamic_branch_local_vertex _BASE_MAP_ROTATION_ENABLED
            #else
            #pragma shader_feature_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma shader_feature_local_vertex _BASE_MAP_ROTATION_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ _BASE_SAMPLER_STATE_POINT_MIRROR _BASE_SAMPLER_STATE_LINEAR_MIRROR _BASE_SAMPLER_STATE_TRILINEAR_MIRROR

            // Surface maps
            #pragma shader_feature_local _NORMAL_MAP_ENABLED

            // Tint Color
            #pragma shader_feature_local _ _TINT_AREA_ALL _TINT_AREA_RIM
            #pragma shader_feature_local _ _TINT_COLOR_ENABLED _TINT_MAP_ENABLED _TINT_MAP_3D_ENABLED

            // Flow Map
            #pragma shader_feature_local _FLOW_MAP_ENABLED // Obsolete, but retained for compatibility.
            #pragma shader_feature_local _FLOW_MAP_TARGET_BASE
            #pragma shader_feature_local _FLOW_MAP_TARGET_TINT
            #pragma shader_feature_local _FLOW_MAP_TARGET_EMISSION
            #pragma shader_feature_local _FLOW_MAP_TARGET_ALPHA_TRANSITION

            // Parallax Map
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_BASE
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_TINT
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_EMISSION
            // #pragma shader_feature_local _PARALLAX_MAP_MODE_2D _PARALLAX_MAP_MODE_2D_ARRAY _PARALLAX_MAP_MODE_3D

            // NOTE : Not need in DepthNormals pass.
            // Color Correction
            // #pragma shader_feature_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED

            // Alpha Transition
            #pragma shader_feature_local _ _FADE_TRANSITION_ENABLED _DISSOLVE_TRANSITION_ENABLED
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #endif
            #pragma shader_feature_local _ _ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE _ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY

            // _EMISSION_AREA_ALPHA keyword affects the depth value, so disable all other keywords.
            // #pragma shader_feature_local _ _EMISSION_AREA_ALL _EMISSION_AREA_MAP _EMISSION_AREA_ALPHA
            #pragma shader_feature_local_fragment _ _EMISSION_AREA_ALPHA
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            #endif
            
            // Transparency
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #else
            #pragma shader_feature_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #endif

            #pragma shader_feature_local _TRANSPARENCY_BY_RIM
            #pragma shader_feature_local _SOFT_PARTICLES_ENABLED
            #pragma shader_feature_local _DEPTH_FADE_ENABLED

            // Vertex Deformation
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_vertex _VERTEX_DEFORMATION_ENABLED
            #else
            #pragma shader_feature_local_vertex _ _VERTEX_DEFORMATION_ENABLED
            #endif

            #ifdef ENABLE_DYNAMIC_BRANCH
            #define _ALPHAMODULATE_ENABLED 0
            #define _METALLIC_MAP_ENABLED 0
            #define _SMOOTHNESS_MAP_ENABLED 0
            #define _SPECULAR_MAP_ENABLED 0
            #define _PARALLAX_MAP_MODE_2D 0
            #define _PARALLAX_MAP_MODE_2D_ARRAY 0
            #define _PARALLAX_MAP_MODE_3D 0
            #define _GREYSCALE_ENABLED 0
            #define _GRADIENT_MAP_ENABLED 0
            #define _EMISSION_COLOR_COLOR 0
            #define _EMISSION_COLOR_BASECOLOR 0
            #define _EMISSION_COLOR_MAP 0
            #endif
            #include "ParticlesUberDepthNormals.hlsl"
            ENDHLSL
        }
        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            Cull[_Cull]
            ColorMask RGB
            Lighting Off
            ZTest LEqual

            HLSLPROGRAM

            #include "Config.hlsl"
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            // Unity Defined
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup
            #pragma require 2darray

            // Render Settings
            // NOTE : Not need in DepthNormals pass.
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #else
            #pragma shader_feature_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #endif
            // #pragma shader_feature_local_fragment _ALPHAMODULATE_ENABLED
            #pragma shader_feature_local _ALPHATEST_ENABLED

            // Base Map
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma dynamic_branch_local_vertex _BASE_MAP_ROTATION_ENABLED
            #else
            #pragma shader_feature_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma shader_feature_local_vertex _BASE_MAP_ROTATION_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ _BASE_SAMPLER_STATE_POINT_MIRROR _BASE_SAMPLER_STATE_LINEAR_MIRROR _BASE_SAMPLER_STATE_TRILINEAR_MIRROR

            // Tint Color
            #pragma shader_feature_local _ _TINT_AREA_ALL _TINT_AREA_RIM
            #pragma shader_feature_local _ _TINT_COLOR_ENABLED _TINT_MAP_ENABLED _TINT_MAP_3D_ENABLED

            // Flow Map
            #pragma shader_feature_local _FLOW_MAP_ENABLED // Obsolete, but retained for compatibility.
            #pragma shader_feature_local _FLOW_MAP_TARGET_BASE
            #pragma shader_feature_local _FLOW_MAP_TARGET_TINT
            #pragma shader_feature_local _FLOW_MAP_TARGET_EMISSION
            #pragma shader_feature_local _FLOW_MAP_TARGET_ALPHA_TRANSITION

            // Parallax Map
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_BASE
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_TINT
            // #pragma shader_feature_local _PARALLAX_MAP_TARGET_EMISSION
            // #pragma shader_feature_local _PARALLAX_MAP_MODE_2D _PARALLAX_MAP_MODE_2D_ARRAY _PARALLAX_MAP_MODE_3D

            // NOTE : Not need in DepthNormals pass.
            // Color Correction
            // #pragma shader_feature_local_fragment _ _GREYSCALE_ENABLED _GRADIENT_MAP_ENABLED

            // Alpha Transition
            #pragma shader_feature_local _ _FADE_TRANSITION_ENABLED _DISSOLVE_TRANSITION_ENABLED
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #endif
            #pragma shader_feature_local _ _ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE _ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY

            // _EMISSION_AREA_ALPHA keyword affects the depth value, so disable all other keywords.
            // #pragma shader_feature_local _ _EMISSION_AREA_ALL _EMISSION_AREA_MAP _EMISSION_AREA_ALPHA
            #pragma shader_feature_local_fragment _ _EMISSION_AREA_ALPHA
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _EMISSION_MAP_MODE_2D _EMISSION_MAP_MODE_2D_ARRAY _EMISSION_MAP_MODE_3D
            #endif
            
            // Transparency
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #else
            #pragma shader_feature_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #endif

            #pragma shader_feature_local _TRANSPARENCY_BY_RIM
            #pragma shader_feature_local _SOFT_PARTICLES_ENABLED
            #pragma shader_feature_local _DEPTH_FADE_ENABLED

            // Vertex Deformation
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_vertex _VERTEX_DEFORMATION_ENABLED
            #else
            #pragma shader_feature_local_vertex _ _VERTEX_DEFORMATION_ENABLED
            #endif

            #ifdef ENABLE_DYNAMIC_BRANCH
            #define _ALPHAMODULATE_ENABLED 0
            #define _METALLIC_MAP_ENABLED 0
            #define _SMOOTHNESS_MAP_ENABLED 0
            #define _SPECULAR_MAP_ENABLED 0
            #define _PARALLAX_MAP_MODE_2D 0
            #define _PARALLAX_MAP_MODE_2D_ARRAY 0
            #define _PARALLAX_MAP_MODE_3D 0
            #define _GREYSCALE_ENABLED 0
            #define _GRADIENT_MAP_ENABLED 0
            #define _EMISSION_COLOR_COLOR 0
            #define _EMISSION_COLOR_BASECOLOR 0
            #define _EMISSION_COLOR_MAP 0
            #endif
            
            // When LightMode is DepthOnly, the shaders are the same as in the Unlit version,
            // so there is no problem.
            #include "ParticlesUberDepthOnly.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM

            #include "Config.hlsl"
            
            #pragma target 3.5

            // Unity Defined
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup
            #pragma require 2darray

            // Render Settings
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #else
            #pragma shader_feature_local_fragment _VERTEX_ALPHA_AS_TRANSITION_PROGRESS
            #endif
            
            // Base Map
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma dynamic_branch_local_vertex _BASE_MAP_ROTATION_ENABLED
            #else
            #pragma shader_feature_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
            #pragma shader_feature_local_vertex _BASE_MAP_ROTATION_ENABLED
            #endif
            #pragma shader_feature_local_fragment _ _BASE_SAMPLER_STATE_POINT_MIRROR _BASE_SAMPLER_STATE_LINEAR_MIRROR _BASE_SAMPLER_STATE_TRILINEAR_MIRROR

            // Tint Color
            // If only _TINT_AREA_ALL is used, fragment is sufficient.
            #pragma shader_feature_local_fragment  _ _TINT_AREA_ALL
            #pragma shader_feature_local _ _TINT_COLOR_ENABLED _TINT_MAP_ENABLED _TINT_MAP_3D_ENABLED

            // Flow Map
            #pragma shader_feature_local _FLOW_MAP_TARGET_BASE
            #pragma shader_feature_local _FLOW_MAP_TARGET_TINT
            #pragma shader_feature_local _FLOW_MAP_TARGET_ALPHA_TRANSITION

            // Alpha Transition
            #pragma shader_feature_local _ _FADE_TRANSITION_ENABLED _DISSOLVE_TRANSITION_ENABLED
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #else
            #pragma shader_feature_local _ALPHA_TRANSITION_MAP_MODE_2D _ALPHA_TRANSITION_MAP_MODE_2D_ARRAY _ALPHA_TRANSITION_MAP_MODE_3D
            #endif
            #pragma shader_feature_local _ _ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE _ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY

            // Transparency
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #else
            #pragma shader_feature_local_fragment _TRANSPARENCY_BY_LUMINANCE
            #endif

            // Vertex Deformation
            #ifdef ENABLE_DYNAMIC_BRANCH
            #pragma dynamic_branch_local_vertex _VERTEX_DEFORMATION_ENABLED
            #else
            #pragma shader_feature_local_vertex _ _VERTEX_DEFORMATION_ENABLED
            #endif

            // Shadow Caster
            #pragma shader_feature_local _SHADOW_CASTER_ALPHA_TEST_ENABLED

            // -------------------------------------
            // Universal Pipeline keywords
            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #ifdef ENABLE_DYNAMIC_BRANCH
            #define _ALPHAMODULATE_ENABLED 0
            #define _METALLIC_MAP_ENABLED 0
            #define _SMOOTHNESS_MAP_ENABLED 0
            #define _SPECULAR_MAP_ENABLED 0
            #define _PARALLAX_MAP_MODE_2D 0
            #define _PARALLAX_MAP_MODE_2D_ARRAY 0
            #define _PARALLAX_MAP_MODE_3D 0
            #define _EMISSION_COLOR_COLOR 0
            #define _EMISSION_COLOR_BASECOLOR 0
            #define _EMISSION_COLOR_MAP 0
            #define _GREYSCALE_ENABLED 0
            #define _GRADIENT_MAP_ENABLED 0
            #define _EMISSION_MAP_MODE_2D 0
            #define _EMISSION_MAP_MODE_2D_ARRAY 0
            #define _EMISSION_MAP_MODE_3D 0
            #endif
            
            #include "ParticlesUberShadowCaster.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "Nova.Editor.Core.Scripts.ParticlesUberLitGUI"
}
