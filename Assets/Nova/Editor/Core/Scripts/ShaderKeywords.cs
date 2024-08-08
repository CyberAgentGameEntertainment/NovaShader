// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace Nova.Editor.Core.Scripts
{
    public static class ShaderKeywords
    {
        // Render Settings
        public const string VertexAlphaAsTransitionProgress = "_VERTEX_ALPHA_AS_TRANSITION_PROGRESS";
        public const string AlphaModulateEnabled = "_ALPHAMODULATE_ENABLED";
        public const string AlphaTestEnabled = "_ALPHATEST_ENABLED";
        public const string ReceiveShadowsEnabled = "_RECEIVE_SHADOWS_ENABLED";
        public const string SpecularHightlightsEnabled = "_SPECULAR_HIGHLIGHTS_ENABLED";
        public const string EnvironmentReflectionsEnabled = "_ENVIRONMENT_REFLECTIONS_ENABLED";

        public const string SpecularSetup = "_SPECULAR_SETUP";

        // Base Color
        public const string BaseMapRotationEnabled = "_BASE_MAP_ROTATION_ENABLED";
        public const string BaseMapMode2D = "_BASE_MAP_MODE_2D";
        public const string BaseMapMode2DArray = "_BASE_MAP_MODE_2D_ARRAY";
        public const string BaseMapMode3D = "_BASE_MAP_MODE_3D";
        public const string BaseSamplerStatePointMirror = "_BASE_SAMPLER_STATE_POINT_MIRROR";
        public const string BaseSamplerStateLinearMirror = "_BASE_SAMPLER_STATE_LINEAR_MIRROR";
        public const string BaseSamplerStateTrilinearMirror = "_BASE_SAMPLER_STATE_TRILINEAR_MIRROR";

        // Surface maps
        public const string NormalMapEnabled = "_NORMAL_MAP_ENABLED";
        public const string MetallicMapEnabled = "_METALLIC_MAP_ENABLED";
        public const string SmoothnessMapEnabled = "_SMOOTHNESS_MAP_ENABLED";
        public const string SpecularMapEnabled = "_SPECULAR_MAP_ENABLED";

        // Tint Color
        public const string TintAreaAll = "_TINT_AREA_ALL";
        public const string TintAreaRim = "_TINT_AREA_RIM";
        public const string TintColorEnabled = "_TINT_COLOR_ENABLED";
        public const string TintMapEnabled = "_TINT_MAP_ENABLED";
        public const string TintMap3DEnabled = "_TINT_MAP_3D_ENABLED";
        public const string GreyscaleEnabled = "_GREYSCALE_ENABLED";
        public const string GradientMapEnabled = "_GRADIENT_MAP_ENABLED";
        public const string FlowMapEnabled = "_FLOW_MAP_ENABLED";
        public const string FlowMapTargetBase = "_FLOW_MAP_TARGET_BASE";
        public const string FlowMapTargetTint = "_FLOW_MAP_TARGET_TINT";
        public const string FlowMapTargetEmission = "_FLOW_MAP_TARGET_EMISSION";
        public const string FlowMapTargetAlphaTransition = "_FLOW_MAP_TARGET_ALPHA_TRANSITION";

        // Parallax Map
        public const string ParallaxMapTargetBase = "_PARALLAX_MAP_TARGET_BASE";
        public const string ParallaxMapTargetTint = "_PARALLAX_MAP_TARGET_TINT";
        public const string ParallaxMapTargetEmission = "_PARALLAX_MAP_TARGET_EMISSION";
        public const string ParallaxMapMode2D = "_PARALLAX_MAP_MODE_2D";
        public const string ParallaxMapMode2DArray = "_PARALLAX_MAP_MODE_2D_ARRAY";
        public const string ParallaxMapMode3D = "_PARALLAX_MAP_MODE_3D";

        // Alpha Transition
        public const string FadeTransitionEnabled = "_FADE_TRANSITION_ENABLED";
        public const string DissolveTransitionEnabled = "_DISSOLVE_TRANSITION_ENABLED";
        public const string AlphaTransitionMapMode2D = "_ALPHA_TRANSITION_MAP_MODE_2D";
        public const string AlphaTransitionMapMode2DArray = "_ALPHA_TRANSITION_MAP_MODE_2D_ARRAY";
        public const string AlphaTransitionMapMode3D = "_ALPHA_TRANSITION_MAP_MODE_3D";
        public const string AlphaTransitionBlendSecondTexAverage = "_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE";
        public const string AlphaTransitionBlendSecondTexMultiply = "_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY";

        // Emission
        public const string EmissionAreaAll = "_EMISSION_AREA_ALL";
        public const string EmissionAreaMap = "_EMISSION_AREA_MAP";
        public const string EmissionMapMode2D = "_EMISSION_MAP_MODE_2D";
        public const string EmissionMapMode2DArray = "_EMISSION_MAP_MODE_2D_ARRAY";
        public const string EmissionMapMode3D = "_EMISSION_MAP_MODE_3D";
        public const string EmissionAreaAlpha = "_EMISSION_AREA_ALPHA";
        public const string EmissionColorColor = "_EMISSION_COLOR_COLOR";
        public const string EmissionColorBaseColor = "_EMISSION_COLOR_BASECOLOR";
        public const string EmissionColorMap = "_EMISSION_COLOR_MAP";

        // Transparency
        public const string TransparencyByRim = "_TRANSPARENCY_BY_RIM";
        public const string TransparencyByLuminance = "_TRANSPARENCY_BY_LUMINANCE";
        public const string SoftParticlesEnabled = "_SOFT_PARTICLES_ENABLED";
        public const string DepthFadeEnabled = "_DEPTH_FADE_ENABLED";

        // Vertex Deformation
        public const string VertexDeformationEnabled = "_VERTEX_DEFORMATION_ENABLED";

        // Shadow Caster
        public const string ShadowCasterAlphaTestEnable = "_SHADOW_CASTER_ALPHA_TEST_ENABLED";

        public static string GetShaderKeyword(this BaseMapMode baseMapMode)
        {
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    return BaseMapMode2D;
                case BaseMapMode.FlipBook:
                    return BaseMapMode2DArray;
                case BaseMapMode.FlipBookBlending:
                    return BaseMapMode3D;
                default:
                    throw new ArgumentOutOfRangeException(nameof(baseMapMode), baseMapMode, null);
            }
        }

        public static string GetShaderKeyword(this AlphaTransitionMapMode alphaTransitionMapMode)
        {
            switch (alphaTransitionMapMode)
            {
                case AlphaTransitionMapMode.SingleTexture:
                    return AlphaTransitionMapMode2D;
                case AlphaTransitionMapMode.FlipBook:
                    return AlphaTransitionMapMode2DArray;
                case AlphaTransitionMapMode.FlipBookBlending:
                    return AlphaTransitionMapMode3D;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alphaTransitionMapMode), alphaTransitionMapMode, null);
            }
        }

        public static string GetShaderKeyword(this EmissionMapMode emissionMapMode)
        {
            switch (emissionMapMode)
            {
                case EmissionMapMode.SingleTexture:
                    return EmissionMapMode2D;
                case EmissionMapMode.FlipBook:
                    return EmissionMapMode2DArray;
                case EmissionMapMode.FlipBookBlending:
                    return EmissionMapMode3D;
                default:
                    throw new ArgumentOutOfRangeException(nameof(emissionMapMode), emissionMapMode, null);
            }
        }

        public static string GetShaderKeyword(this EmissionAreaType emissionAreaType)
        {
            switch (emissionAreaType)
            {
                case EmissionAreaType.None:
                    return string.Empty;
                case EmissionAreaType.All:
                    return EmissionAreaAll;
                case EmissionAreaType.ByTexture:
                    return EmissionAreaMap;
                case EmissionAreaType.Edge:
                    return EmissionAreaAlpha;
                default:
                    throw new ArgumentOutOfRangeException(nameof(emissionAreaType), emissionAreaType, null);
            }
        }

        public static string GetShaderKeyword(this TintAreaMode tintAreaMode)
        {
            switch (tintAreaMode)
            {
                case TintAreaMode.None:
                    return string.Empty;
                case TintAreaMode.All:
                    return TintAreaAll;
                case TintAreaMode.Rim:
                    return TintAreaRim;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tintAreaMode), tintAreaMode, null);
            }
        }

        public static string GetShaderKeyword(this TintColorMode tintColorMode)
        {
            switch (tintColorMode)
            {
                case TintColorMode.SingleColor:
                    return TintColorEnabled;
                case TintColorMode.Texture2D:
                    return TintMapEnabled;
                case TintColorMode.Texture3D:
                    return TintMap3DEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tintColorMode), tintColorMode, null);
            }
        }

        public static string GetShaderKeyword(this EmissionColorType emissionColorType)
        {
            switch (emissionColorType)
            {
                case EmissionColorType.Color:
                    return EmissionColorColor;
                case EmissionColorType.BaseColor:
                    return EmissionColorBaseColor;
                case EmissionColorType.GradiantMap:
                    return EmissionColorMap;
                default:
                    throw new ArgumentOutOfRangeException(nameof(emissionColorType), emissionColorType, null);
            }
        }

        public static string GetShaderKeyword(this ColorCorrectionMode colorCorrectionMode)
        {
            switch (colorCorrectionMode)
            {
                case ColorCorrectionMode.None:
                    return string.Empty;
                case ColorCorrectionMode.Greyscale:
                    return GreyscaleEnabled;
                case ColorCorrectionMode.GradientMap:
                    return GradientMapEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colorCorrectionMode), colorCorrectionMode, null);
            }
        }

        public static string GetShaderKeyword(this ParallaxMapMode parallaxMapMode)
        {
            switch (parallaxMapMode)
            {
                case ParallaxMapMode.SingleTexture:
                    return ParallaxMapMode2D;
                case ParallaxMapMode.FlipBook:
                    return ParallaxMapMode2DArray;
                case ParallaxMapMode.FlipBookBlending:
                    return ParallaxMapMode3D;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ParallaxMapMode), parallaxMapMode, null);
            }
        }
    }
}
