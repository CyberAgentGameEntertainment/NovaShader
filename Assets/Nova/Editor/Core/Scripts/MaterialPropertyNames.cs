// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

namespace Nova.Editor.Core.Scripts
{
    public static class MaterialPropertyNames
    {
        // Render Settings
        public const string RenderType = "_RenderType";
        public const string Cutoff = "_Cutoff";
        public const string TransparentBlendMode = "_TransparentBlendMode";
        public const string Cull = "_Cull";
        public const string QueueOffset = "_QueueOffset";
        public const string VertexAlphaMode = "_VertexAlphaMode";
        public const string BlendSrc = "_BlendSrc";
        public const string BlendDst = "_BlendDst";
        public const string ZWrite = "_ZWrite";
        public const string ZWriteOverride = "_ZWriteOverride";
        public const string ZTest = "_ZTest";
        public const string LitWorkflowMode = "_LitWorkflowMode";
        public const string LitReceiveShadows = "_LitReceiveShadows";
        public const string SpecularHighlights = "_SpecularHighlights";
        public const string EnvironmentReflections = "_EnvironmentReflections";

        // Surface Maps
        public const string NormalMap = "_NormalMap";
        public const string NormalMap2DArray = "_NormalMap2DArray";
        public const string NormalMap3D = "_NormalMap3D";
        public const string NormalMapBumpScale = "_NormalMapBumpScale";
        public const string SpecularMap = "_SpecularMap";
        public const string SpecularMap2DArray = "_SpecularMap2DArray";
        public const string SpecularMap3D = "_SpecularMap3D";
        public const string Specular = "_SpecularColor";

        public const string MetallicMap = "_MetallicMap";
        public const string MetallicMap2DArray = "_MetallicMap2DArray";
        public const string MetallicMap3D = "_MetallicMap3D";
        public const string Metallic = "_Metallic";
        public const string MetallicMapChannelsX = "_MetallicMapChannelsX";
        public const string SmoothnessMap = "_SmoothnessMap";
        public const string SmoothnessMap2DArray = "_SmoothnessMap2DArray";
        public const string SmoothnessMap3D = "_SmoothnessMap3D";
        public const string Smoothness = "_Smoothness";
        public const string SmoothnessMapChannelsX = "_SmoothnessMapChannelsX";

        // Base Color
        public const string BaseMapMode = "_BaseMapMode";
        public const string BaseMap = "_BaseMap";
        public const string BaseMap2DArray = "_BaseMap2DArray";
        public const string BaseMap3D = "_BaseMap3D";
        public const string BaseMapProgress = "_BaseMapProgress";
        public const string BaseMapProgressCoord = "_BaseMapProgressCoord";
        public const string BaseMapSliceCount = "_BaseMapSliceCount";
        public const string BaseMapOffsetXCoord = "_BaseMapOffsetXCoord";
        public const string BaseMapOffsetYCoord = "_BaseMapOffsetYCoord";
        public const string BaseMapChannelsX = "_BaseMapChannelsX";
        public const string BaseMapChannelsY = "_BaseMapChannelsY";
        public const string BaseMapRotation = "_BaseMapRotation";
        public const string BaseMapRotationCoord = "_BaseMapRotationCoord";
        public const string BaseMapRotationOffsets = "_BaseMapRotationOffsets";
        public const string BaseMapMirrorSampling = "_BaseMapMirrorSampling";
        public const string BaseMapUnpackNormal = "_BaseMapUnpackNormal";

        // Tint Color
        public const string TintAreaMode = "_TintAreaMode";
        public const string TintColorMode = "_TintColorMode";
        public const string TintColor = "_TintColor";
        public const string TintMap = "_TintMap";
        public const string TintMap3D = "_TintMap3D";
        public const string TintMap3DProgress = "_TintMap3DProgress";
        public const string TintMap3DProgressCoord = "_TintMap3DProgressCoord";
        public const string TintMapSliceCount = "_TintMapSliceCount";
        public const string TintMapOffsetXCoord = "_TintMapOffsetXCoord";
        public const string TintMapOffsetYCoord = "_TintMapOffsetYCoord";
        public const string TintMapBlendRate = "_TintBlendRate";
        public const string TintMapBlendRateCoord = "_TintBlendRateCoord";
        public const string TintRimProgress = "_TintRimProgress";
        public const string TintRimProgressCoord = "_TintRimProgressCoord";
        public const string TintRimSharpness = "_TintRimSharpness";
        public const string TintRimSharpnessCoord = "_TintRimSharpnessCoord";
        public const string InverseTintRim = "_InverseTintRim";

        // Flow Map
        public const string FlowMap = "_FlowMap";
        public const string FlowMapOffsetXCoord = "_FlowMapOffsetXCoord";
        public const string FlowMapOffsetYCoord = "_FlowMapOffsetYCoord";
        public const string FlowMapChannelsX = "_FlowMapChannelsX";
        public const string FlowMapChannelsY = "_FlowMapChannelsY";
        public const string FlowIntensity = "_FlowIntensity";
        public const string FlowIntensityCoord = "_FlowIntensityCoord";
        public const string FlowMapTarget = "_FlowMapTarget";

        // Parallax Map
        public const string ParallaxMapMode = "_ParallaxMapMode";
        public const string ParallaxMap = "_ParallaxMap";
        public const string ParallaxMap2DArray = "_ParallaxMap2DArray";
        public const string ParallaxMap3D = "_ParallaxMap3D";
        public const string ParallaxMapProgress = "_ParallaxMapProgress";
        public const string ParallaxMapProgressCoord = "_ParallaxMapProgressCoord";
        public const string ParallaxMapOffsetXCoord = "_ParallaxMapOffsetXCoord";
        public const string ParallaxMapOffsetYCoord = "_ParallaxMapOffsetYCoord";
        public const string ParallaxMapSliceCount = "_ParallaxMapSliceCount";
        public const string ParallaxMapChannel = "_ParallaxMapChannel";
        public const string ParallaxStrength = "_ParallaxStrength";
        public const string ParallaxMapTarget = "_ParallaxMapTarget";

        // Color Correction
        public const string ColorCorrectionMode = "_ColorCorrectionMode";
        public const string GradientMap = "_GradientMap";

        // Alpha Transition
        public const string AlphaTransitionMode = "_AlphaTransitionMode";
        public const string AlphaTransitionMapMode = "_AlphaTransitionMapMode";
        public const string AlphaTransitionMap = "_AlphaTransitionMap";
        public const string AlphaTransitionMap2DArray = "_AlphaTransitionMap2DArray";
        public const string AlphaTransitionMap3D = "_AlphaTransitionMap3D";
        public const string AlphaTransitionMapProgress = "_AlphaTransitionMapProgress";
        public const string AlphaTransitionMapProgressCoord = "_AlphaTransitionMapProgressCoord";
        public const string AlphaTransitionMapOffsetXCoord = "_AlphaTransitionMapOffsetXCoord";
        public const string AlphaTransitionMapOffsetYCoord = "_AlphaTransitionMapOffsetYCoord";
        public const string AlphaTransitionMapChannelsX = "_AlphaTransitionMapChannelsX";
        public const string AlphaTransitionMapSliceCount = "_AlphaTransitionMapSliceCount";
        public const string AlphaTransitionSecondTextureBlendMode = "_AlphaTransitionSecondTextureBlendMode";
        public const string AlphaTransitionProgress = "_AlphaTransitionProgress";
        public const string AlphaTransitionProgressCoord = "_AlphaTransitionProgressCoord";
        public const string DissolveSharpness = "_DissolveSharpness";
        public const string AlphaTransitionMapSecondTexture = "_AlphaTransitionMapSecondTexture";
        public const string AlphaTransitionMapSecondTexture2DArray = "_AlphaTransitionMapSecondTexture2DArray";
        public const string AlphaTransitionMapSecondTexture3D = "_AlphaTransitionMapSecondTexture3D";
        public const string AlphaTransitionMapSecondTextureProgress = "_AlphaTransitionMapSecondTextureProgress";

        public const string AlphaTransitionMapSecondTextureProgressCoord =
            "_AlphaTransitionMapSecondTextureProgressCoord";

        public const string AlphaTransitionMapSecondTextureOffsetXCoord =
            "_AlphaTransitionMapSecondTextureOffsetXCoord";

        public const string AlphaTransitionMapSecondTextureOffsetYCoord =
            "_AlphaTransitionMapSecondTextureOffsetYCoord";

        public const string AlphaTransitionMapSecondTextureChannelsX = "_AlphaTransitionMapSecondTextureChannelsX";
        public const string AlphaTransitionMapSecondTextureSliceCount = "_AlphaTransitionMapSecondTextureSliceCount";
        public const string AlphaTransitionProgressSecondTexture = "_AlphaTransitionProgressSecondTexture";
        public const string AlphaTransitionProgressCoordSecondTexture = "_AlphaTransitionProgressCoordSecondTexture";
        public const string DissolveSharpnessSecondTexture = "_DissolveSharpnessSecondTexture";

        // Emission
        public const string EmissionAreaType = "_EmissionAreaType";
        public const string EmissionMapMode = "_EmissionMapMode";
        public const string EmissionMap = "_EmissionMap";
        public const string EmissionMap2DArray = "_EmissionMap2DArray";
        public const string EmissionMap3D = "_EmissionMap3D";
        public const string EmissionMapProgress = "_EmissionMapProgress";
        public const string EmissionMapProgressCoord = "_EmissionMapProgressCoord";
        public const string EmissionMapOffsetXCoord = "_EmissionMapOffsetXCoord";
        public const string EmissionMapOffsetYCoord = "_EmissionMapOffsetYCoord";
        public const string EmissionMapChannelsX = "_EmissionMapChannelsX";
        public const string EmissionMapSliceCount = "_EmissionMapSliceCount";
        public const string EmissionColorType = "_EmissionColorType";
        public const string EmissionColor = "_EmissionColor";
        public const string EmissionColorRamp = "_EmissionColorRamp";
        public const string EmissionIntensity = "_EmissionIntensity";
        public const string EmissionIntensityCoord = "_EmissionIntensityCoord";
        public const string KeepEdgeTransparency = "_KeepEdgeTransparency";

        // Transparency
        public const string RimTransparencyEnabled = "_RimTransparencyEnabled";
        public const string RimTransparencyProgress = "_RimTransparencyProgress";
        public const string RimTransparencyProgressCoord = "_RimTransparencyProgressCoord";
        public const string RimTransparencySharpness = "_RimTransparencySharpness";
        public const string RimTransparencySharpnessCoord = "_RimTransparencySharpnessCoord";
        public const string InverseRimTransparency = "_InverseRimTransparency";
        public const string LuminanceTransparencyEnabled = "_LuminanceTransparencyEnabled";
        public const string LuminanceTransparencyProgress = "_LuminanceTransparencyProgress";
        public const string LuminanceTransparencyProgressCoord = "_LuminanceTransparencyProgressCoord";
        public const string LuminanceTransparencySharpness = "_LuminanceTransparencySharpness";
        public const string LuminanceTransparencySharpnessCoord = "_LuminanceTransparencySharpnessCoord";
        public const string InverseLuminanceTransparency = "_InverseLuminanceTransparency";
        public const string SoftParticlesEnabled = "_SoftParticlesEnabled";
        public const string SoftParticlesIntensity = "_SoftParticlesIntensity";
        public const string DepthFadeEnabled = "_DepthFadeEnabled";
        public const string DepthFadeNear = "_DepthFadeNear";
        public const string DepthFadeFar = "_DepthFadeFar";
        public const string DepthFadeWidth = "_DepthFadeWidth";

        // Distortion
        public const string DistortionIntensity = "_DistortionIntensity";
        public const string DistortionIntensityCoord = "_DistortionIntensityCoord";

        // Vertex Deformation
        public const string VertexDeformationMap = "_VertexDeformationMap";
        public const string VertexDeformationMapOffsetXCoord = "_VertexDeformationMapOffsetXCoord";
        public const string VertexDeformationMapOffsetYCoord = "_VertexDeformationMapOffsetYCoord";
        public const string VertexDeformationMapChannel = "_VertexDeformationMapChannel";
        public const string VertexDeformationIntensity = "_VertexDeformationIntensity";
        public const string VertexDeformationIntensityCoord = "_VertexDeformationIntensityCoord";
        public const string VertexDeformationBaseValue = "_VertexDeformationBaseValue";

        // Shadow Caster
        public const string ShadowCasterEnabled = "_ShadowCasterEnabled";
        public const string ShadowCasterApplyVertexDeformation = "_ShadowCasterApplyVertexDeformation";
        public const string ShadowCasterAlphaTestEnabled = "_ShadowCasterAlphaTestEnabled";
        public const string ShadowCasterAlphaCutoff = "_ShadowCasterAlphaCutoff";
        public const string ShadowCasterAlphaAffectedByTintColor = "_ShadowCasterAlphaAffectedByTintColor";
        public const string ShadowCasterAlphaAffectedByFlowMap = "_ShadowCasterAlphaAffectedByFlowMap";

        public const string ShadowCasterAlphaAffectedByAlphaTransitionMap =
            "_ShadowCasterAlphaAffectedByAlphaTransitionMap";

        public const string ShadowCasterAlphaAffectedByTransparencyLuminance =
            "_ShadowCasterAlphaAffectedByTransparencyLuminance";
    }
}
