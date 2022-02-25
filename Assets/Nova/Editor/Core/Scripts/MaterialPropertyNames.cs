// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
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
        public const string BaseMapRotation = "_BaseMapRotation";
        public const string BaseMapRotationCoord = "_BaseMapRotationCoord";
        public const string BaseMapRotationOffsets = "_BaseMapRotationOffsets";
        public const string BaseMapMirrorSampling = "_BaseMapMirrorSampling";

        // Tint Color
        public const string TintAreaMode = "_TintAreaMode";
        public const string TintColorMode = "_TintColorMode";
        public const string TintColor = "_TintColor";
        public const string TintMap = "_TintMap";
        public const string TintMap3D = "_TintMap3D";
        public const string TintMap3DProgress = "_TintMap3DProgress";
        public const string TintMap3DProgressCoord = "_TintMap3DProgressCoord";
        public const string TintMapSliceCount = "_TintMapSliceCount";
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
        public const string FlowIntensity = "_FlowIntensity";
        public const string FlowIntensityCoord = "_FlowIntensityCoord";
        public const string FlowMapTarget = "_FlowMapTarget";

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
        public const string AlphaTransitionMapSliceCount = "_AlphaTransitionMapSliceCount";
        public const string AlphaTransitionProgress = "_AlphaTransitionProgress";
        public const string AlphaTransitionProgressCoord = "_AlphaTransitionProgressCoord";
        public const string DissolveSharpness = "_DissolveSharpness";

        // Emission
        public const string EmissionAreaType = "_EmissionAreaType";
        public const string EmissionMapMode = "_EmissionMapMode";
        public const string EmissionMap = "_EmissionMap";
        public const string EmissionMap2DArray = "_EmissionMap2DArray";
        public const string EmissionMap3D = "_EmissionMap3D";
        public const string EmissionMapProgress = "_EmissionMapProgress";
        public const string EmissionMapProgressCoord = "_EmissionMapProgressCoord";
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
    }
}