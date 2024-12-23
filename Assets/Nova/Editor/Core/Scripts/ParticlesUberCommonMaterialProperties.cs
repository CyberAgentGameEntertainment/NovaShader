// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    internal class ParticlesUberCommonMaterialProperties
    {
        public ParticlesUberCommonMaterialProperties(MaterialProperty[] properties)
        {
            var prefsKeyPrefix = $"{GetType().Namespace}.{GetType().Name}.";
            var renderSettingsFoldoutKey = $"{prefsKeyPrefix}{nameof(RenderSettingsFoldout)}";
            var baseMapFoldoutKey = $"{prefsKeyPrefix}{nameof(BaseMapFoldout)}";
            var tintColorFoldoutKey = $"{prefsKeyPrefix}{nameof(TintColorFoldout)}";
            var flowMapFoldoutKey = $"{prefsKeyPrefix}{nameof(FlowMapFoldout)}";
            var parallaxMapFoldoutKey = $"{prefsKeyPrefix}{nameof(ParallaxMapFoldout)}";
            var colorCorrectionFoldoutKey = $"{prefsKeyPrefix}{nameof(ColorCorrectionFoldout)}";
            var transparencyFoldoutKey = $"{prefsKeyPrefix}{nameof(TransparencyFoldout)}";
            var alphaTransitionFoldoutKey = $"{prefsKeyPrefix}{nameof(AlphaTransitionFoldout)}";
            var emissionFoldoutKey = $"{prefsKeyPrefix}{nameof(EmissionFoldout)}";
            var vertexDeformationFoldoutKey = $"{prefsKeyPrefix}{nameof(VertexDeformationFoldout)}";
            var shadowCasterFoldoutKey = $"{prefsKeyPrefix}{nameof(ShadowCasterFoldout)}";

            RenderSettingsFoldout = new BoolEditorPrefsProperty(renderSettingsFoldoutKey, true);
            BaseMapFoldout = new BoolEditorPrefsProperty(baseMapFoldoutKey, true);
            TintColorFoldout = new BoolEditorPrefsProperty(tintColorFoldoutKey, true);
            FlowMapFoldout = new BoolEditorPrefsProperty(flowMapFoldoutKey, true);
            ParallaxMapFoldout = new BoolEditorPrefsProperty(parallaxMapFoldoutKey, true);
            ColorCorrectionFoldout = new BoolEditorPrefsProperty(colorCorrectionFoldoutKey, true);
            TransparencyFoldout = new BoolEditorPrefsProperty(transparencyFoldoutKey, true);
            AlphaTransitionFoldout = new BoolEditorPrefsProperty(alphaTransitionFoldoutKey, true);
            EmissionFoldout = new BoolEditorPrefsProperty(emissionFoldoutKey, true);
            VertexDeformationFoldout = new BoolEditorPrefsProperty(vertexDeformationFoldoutKey, true);
            ShadowCasterFoldout = new BoolEditorPrefsProperty(shadowCasterFoldoutKey, true);

            Setup(properties);
        }

        public void Setup(MaterialProperty[] properties)
        {
            // Render Settings
            RenderTypeProp.Setup(properties);
            CutoffProp.Setup(properties);
            TransparentBlendModeProp.Setup(properties);
            CullProp.Setup(properties);
            QueueOffsetProp.Setup(properties);
            VertexAlphaModeProp.Setup(properties);
            BlendSrcProp.Setup(properties);
            BlendDstProp.Setup(properties);
            ZWriteProp.Setup(properties);
            ZWriteOverrideProp.Setup(properties);
            ZTestProp.Setup(properties);

            // Base Color
            BaseMapModeProp.Setup(properties);
            BaseMapProp.Setup(properties);
            BaseMap2DArrayProp.Setup(properties);
            BaseMap3DProp.Setup(properties);
            BaseMapProgressProp.Setup(properties);
            BaseMapProgressCoordProp.Setup(properties);
            BaseMapSliceCountProp.Setup(properties);
            BaseColorProp.Setup(properties);
            BaseMapOffsetXCoordProp.Setup(properties);
            BaseMapOffsetYCoordProp.Setup(properties);
            BaseMapRotationProp.Setup(properties);
            BaseMapRotationCoordProp.Setup(properties);
            BaseMapRotationOffsetsProp.Setup(properties);
            BaseMapMirrorSamplingProp.Setup(properties);

            // Tint Color
            TintAreaModeProp.Setup(properties);
            TintColorModeProp.Setup(properties);
            TintMapProp.Setup(properties);
            TintMap3DProp.Setup(properties);
            TintMap3DProgressProp.Setup(properties);
            TintMap3DProgressCoordProp.Setup(properties);
            TintMapSliceCountProp.Setup(properties);
            TintMapOffsetXCoordProp.Setup(properties);
            TintMapOffsetYCoordProp.Setup(properties);
            TintMapBlendRateProp.Setup(properties);
            TintMapBlendRateCoordProp.Setup(properties);
            TintRimProgressProp.Setup(properties);
            TintRimProgressCoordProp.Setup(properties);
            TintRimSharpnessProp.Setup(properties);
            TintRimSharpnessCoordProp.Setup(properties);
            InverseTintRimProp.Setup(properties);

            // Flow Map
            FlowMapProp.Setup(properties);
            FlowMapOffsetXCoordProp.Setup(properties);
            FlowMapOffsetYCoordProp.Setup(properties);
            FlowMapChannelsXProp.Setup(properties);
            FlowMapChannelsYProp.Setup(properties);
            FlowIntensityProp.Setup(properties);
            FlowIntensityCoordProp.Setup(properties);
            FlowMapTargetProp.Setup(properties);

            // Parallax Map
            ParallaxMapModeProp.Setup(properties);
            ParallaxMapProp.Setup(properties);
            ParallaxMap2DArrayProp.Setup(properties);
            ParallaxMap3DProp.Setup(properties);
            ParallaxMapProgressProp.Setup(properties);
            ParallaxMapProgressCoordProp.Setup(properties);
            ParallaxMapOffsetXCoordProp.Setup(properties);
            ParallaxMapOffsetYCoordProp.Setup(properties);
            ParallaxMapSliceCountProp.Setup(properties);
            ParallaxMapChannel.Setup(properties);
            ParallaxStrengthProp.Setup(properties);
            ParallaxMapTargetProp.Setup(properties);

            // Color Correction
            ColorCorrectionModeProp.Setup(properties);
            GradientMapProp.Setup(properties);

            // Alpha Transition
            AlphaTransitionModeProp.Setup(properties);
            AlphaTransitionMapModeProp.Setup(properties);
            AlphaTransitionMapProp.Setup(properties);
            AlphaTransitionMap2DArrayProp.Setup(properties);
            AlphaTransitionMap3DProp.Setup(properties);
            AlphaTransitionMapProgressProp.Setup(properties);
            AlphaTransitionMapProgressCoordProp.Setup(properties);
            AlphaTransitionMapOffsetXCoordProp.Setup(properties);
            AlphaTransitionMapOffsetYCoordProp.Setup(properties);
            AlphaTransitionMapChannelsXProp.Setup(properties);
            AlphaTransitionMapSliceCountProp.Setup(properties);
            AlphaTransitionProgressProp.Setup(properties);
            AlphaTransitionProgressCoordProp.Setup(properties);
            DissolveSharpnessProp.Setup(properties);
            AlphaTransitionSecondTextureBlendModeProp.Setup(properties);
            AlphaTransitionMapSecondTextureProp.Setup(properties);
            AlphaTransitionMapSecondTexture2DArrayProp.Setup(properties);
            AlphaTransitionMapSecondTexture3DProp.Setup(properties);
            AlphaTransitionMapSecondTextureProgressProp.Setup(properties);
            AlphaTransitionMapSecondTextureProgressCoordProp.Setup(properties);
            AlphaTransitionMapSecondTextureOffsetXCoordProp.Setup(properties);
            AlphaTransitionMapSecondTextureOffsetYCoordProp.Setup(properties);
            AlphaTransitionMapSecondTextureChannelsXProp.Setup(properties);
            AlphaTransitionMapSecondTextureSliceCountProp.Setup(properties);
            AlphaTransitionProgressSecondTextureProp.Setup(properties);
            AlphaTransitionProgressCoordSecondTextureProp.Setup(properties);
            DissolveSharpnessSecondTextureProp.Setup(properties);

            // Emission
            EmissionAreaTypeProp.Setup(properties);
            EmissionMapModeProp.Setup(properties);
            EmissionMapProp.Setup(properties);
            EmissionMap2DArrayProp.Setup(properties);
            EmissionMap3DProp.Setup(properties);
            EmissionMapProgressProp.Setup(properties);
            EmissionMapProgressCoordProp.Setup(properties);
            EmissionMapOffsetXCoordProp.Setup(properties);
            EmissionMapOffsetYCoordProp.Setup(properties);
            EmissionMapChannelsXProp.Setup(properties);
            EmissionMapSliceCountProp.Setup(properties);
            EmissionColorTypeProp.Setup(properties);
            EmissionColorProp.Setup(properties);
            EmissionColorRampProp.Setup(properties);
            EmissionIntensityProp.Setup(properties);
            EmissionIntensityCoordProp.Setup(properties);
            KeepEdgeTransparencyProp.Setup(properties);

            // Transparency
            RimTransparencyEnabledProp.Setup(properties);
            RimTransparencyProgressProp.Setup(properties);
            RimTransparencyProgressCoordProp.Setup(properties);
            RimTransparencySharpnessProp.Setup(properties);
            RimTransparencySharpnessCoordProp.Setup(properties);
            InverseRimTransparencyProp.Setup(properties);
            LuminanceTransparencyEnabledProp.Setup(properties);
            LuminanceTransparencyProgressProp.Setup(properties);
            LuminanceTransparencyProgressCoordProp.Setup(properties);
            LuminanceTransparencySharpnessProp.Setup(properties);
            LuminanceTransparencySharpnessCoordProp.Setup(properties);
            InverseLuminanceTransparencyProp.Setup(properties);
            SoftParticlesEnabledProp.Setup(properties);
            SoftParticlesIntensityProp.Setup(properties);
            DepthFadeEnabledProp.Setup(properties);
            DepthFadeNearProp.Setup(properties);
            DepthFadeFarProp.Setup(properties);
            DepthFadeWidthProp.Setup(properties);

            // Vertex Deformation
            VertexDeformationMapProp.Setup(properties);
            VertexDeformationMapOffsetXCoordProp.Setup(properties);
            VertexDeformationMapOffsetYCoordProp.Setup(properties);
            VertexDeformationMapChannelProp.Setup(properties);
            VertexDeformationIntensityProp.Setup(properties);
            VertexDeformationIntensityCoordProp.Setup(properties);
            VertexDeformationBaseValueProp.Setup(properties);

            // Shadow Caster
            ShadowCasterEnabledProp.Setup(properties);
            ShadowCasterApplyVertexDeformationProp.Setup(properties);
            ShadowCasterAlphaTestEnabledProp.Setup(properties);
            ShadowCasterAlphaCutoffProp.Setup(properties);
            ShadowCasterAlphaAffectedByTintColorProp.Setup(properties);
            ShadowCasterAlphaAffectedByFlowMapProp.Setup(properties);
            ShadowCasterAlphaAffectedByAlphaTransitionMapProp.Setup(properties);
            ShadowCasterAlphaAffectedByTransparencyLuminanceProp.Setup(properties);
        }

        #region Foldout Properties

        public BoolEditorPrefsProperty AlphaTransitionFoldout { get; private set; }
        public BoolEditorPrefsProperty BaseMapFoldout { get; private set; }
        public BoolEditorPrefsProperty TintColorFoldout { get; private set; }
        public BoolEditorPrefsProperty FlowMapFoldout { get; private set; }
        public BoolEditorPrefsProperty ParallaxMapFoldout { get; private set; }
        public BoolEditorPrefsProperty ColorCorrectionFoldout { get; private set; }
        public BoolEditorPrefsProperty EmissionFoldout { get; private set; }
        public BoolEditorPrefsProperty RenderSettingsFoldout { get; private set; }
        public BoolEditorPrefsProperty TransparencyFoldout { get; private set; }
        public BoolEditorPrefsProperty VertexDeformationFoldout { get; private set; }
        public BoolEditorPrefsProperty ShadowCasterFoldout { get; private set; }

        #endregion

        #region Render Settings Material Properties

        public ParticlesGUI.Property RenderTypeProp { get; } = new(PropertyNames.RenderType);
        public ParticlesGUI.Property CutoffProp { get; } = new(PropertyNames.Cutoff);

        public ParticlesGUI.Property TransparentBlendModeProp { get; } = new(PropertyNames.TransparentBlendMode);

        public ParticlesGUI.Property CullProp { get; } = new(PropertyNames.Cull);
        public ParticlesGUI.Property QueueOffsetProp { get; } = new(PropertyNames.QueueOffset);

        public ParticlesGUI.Property VertexAlphaModeProp { get; } = new(PropertyNames.VertexAlphaMode);

        public ParticlesGUI.Property BlendDstProp { get; } = new(PropertyNames.BlendDst);
        public ParticlesGUI.Property BlendSrcProp { get; } = new(PropertyNames.BlendSrc);
        public ParticlesGUI.Property ZWriteProp { get; } = new(PropertyNames.ZWrite);

        public ParticlesGUI.Property ZWriteOverrideProp { get; } = new(PropertyNames.ZWriteOverride);

        public ParticlesGUI.Property ZTestProp { get; } = new(PropertyNames.ZTest);

        #endregion

        #region Base Map Material Properties

        public ParticlesGUI.Property BaseMapModeProp { get; } = new(PropertyNames.BaseMapMode);
        public ParticlesGUI.Property BaseMapProp { get; } = new(PropertyNames.BaseMap);

        public ParticlesGUI.Property BaseMap2DArrayProp { get; } = new(PropertyNames.BaseMap2DArray);

        public ParticlesGUI.Property BaseMap3DProp { get; } = new(PropertyNames.BaseMap3D);

        public ParticlesGUI.Property BaseMapProgressProp { get; } = new(PropertyNames.BaseMapProgress);

        public ParticlesGUI.Property BaseMapProgressCoordProp { get; } = new(PropertyNames.BaseMapProgressCoord);

        public ParticlesGUI.Property BaseMapSliceCountProp { get; } = new(PropertyNames.BaseMapSliceCount);

        public ParticlesGUI.Property BaseColorProp { get; } = new(PropertyNames.TintColor);

        public ParticlesGUI.Property BaseMapOffsetXCoordProp { get; } = new(PropertyNames.BaseMapOffsetXCoord);

        public ParticlesGUI.Property BaseMapOffsetYCoordProp { get; } = new(PropertyNames.BaseMapOffsetYCoord);

        public ParticlesGUI.Property BaseMapRotationProp { get; } = new(PropertyNames.BaseMapRotation);

        public ParticlesGUI.Property BaseMapRotationCoordProp { get; } = new(PropertyNames.BaseMapRotationCoord);

        public ParticlesGUI.Property BaseMapRotationOffsetsProp { get; } = new(PropertyNames.BaseMapRotationOffsets);

        public ParticlesGUI.Property BaseMapMirrorSamplingProp { get; } = new(PropertyNames.BaseMapMirrorSampling);

        #endregion

        #region Tint Color Material Properties

        public ParticlesGUI.Property TintAreaModeProp { get; } = new(PropertyNames.TintAreaMode);

        public ParticlesGUI.Property TintColorModeProp { get; } = new(PropertyNames.TintColorMode);

        public ParticlesGUI.Property TintMapProp { get; } = new(PropertyNames.TintMap);
        public ParticlesGUI.Property TintMap3DProp { get; } = new(PropertyNames.TintMap3D);

        public ParticlesGUI.Property TintMap3DProgressProp { get; } = new(PropertyNames.TintMap3DProgress);

        public ParticlesGUI.Property TintMap3DProgressCoordProp { get; } = new(PropertyNames.TintMap3DProgressCoord);

        public ParticlesGUI.Property TintMapSliceCountProp { get; } = new(PropertyNames.TintMapSliceCount);

        public ParticlesGUI.Property TintMapOffsetXCoordProp { get; } = new(PropertyNames.TintMapOffsetXCoord);

        public ParticlesGUI.Property TintMapOffsetYCoordProp { get; } = new(PropertyNames.TintMapOffsetYCoord);

        public ParticlesGUI.Property TintMapBlendRateProp { get; } = new(PropertyNames.TintMapBlendRate);

        public ParticlesGUI.Property TintMapBlendRateCoordProp { get; } = new(PropertyNames.TintMapBlendRateCoord);

        public ParticlesGUI.Property TintRimProgressProp { get; } = new(PropertyNames.TintRimProgress);

        public ParticlesGUI.Property TintRimProgressCoordProp { get; } = new(PropertyNames.TintRimProgressCoord);

        public ParticlesGUI.Property TintRimSharpnessProp { get; } = new(PropertyNames.TintRimSharpness);

        public ParticlesGUI.Property TintRimSharpnessCoordProp { get; } = new(PropertyNames.TintRimSharpnessCoord);

        public ParticlesGUI.Property InverseTintRimProp { get; } = new(PropertyNames.InverseTintRim);

        #endregion

        #region Flow Map Material Properties

        public ParticlesGUI.Property FlowMapProp { get; } = new(PropertyNames.FlowMap);

        public ParticlesGUI.Property FlowMapOffsetXCoordProp { get; } = new(PropertyNames.FlowMapOffsetXCoord);

        public ParticlesGUI.Property FlowMapOffsetYCoordProp { get; } = new(PropertyNames.FlowMapOffsetYCoord);

        public ParticlesGUI.Property FlowMapChannelsXProp { get; } = new(PropertyNames.FlowMapChannelsX);

        public ParticlesGUI.Property FlowMapChannelsYProp { get; } = new(PropertyNames.FlowMapChannelsY);

        public ParticlesGUI.Property FlowIntensityProp { get; } = new(PropertyNames.FlowIntensity);

        public ParticlesGUI.Property FlowIntensityCoordProp { get; } = new(PropertyNames.FlowIntensityCoord);

        public ParticlesGUI.Property FlowMapTargetProp { get; } = new(PropertyNames.FlowMapTarget);

        #endregion

        #region Paralax Map Material Properties

        public ParticlesGUI.Property ParallaxMapModeProp { get; } = new(PropertyNames.ParallaxMapMode);
        public ParticlesGUI.Property ParallaxMapProp { get; } = new(PropertyNames.ParallaxMap);
        public ParticlesGUI.Property ParallaxMap2DArrayProp { get; } = new(PropertyNames.ParallaxMap2DArray);
        public ParticlesGUI.Property ParallaxMap3DProp { get; } = new(PropertyNames.ParallaxMap3D);
        public ParticlesGUI.Property ParallaxMapProgressProp { get; } = new(PropertyNames.ParallaxMapProgress);

        public ParticlesGUI.Property ParallaxMapProgressCoordProp { get; } =
            new(PropertyNames.ParallaxMapProgressCoord);

        public ParticlesGUI.Property ParallaxMapOffsetXCoordProp { get; } = new(PropertyNames.ParallaxMapOffsetXCoord);
        public ParticlesGUI.Property ParallaxMapOffsetYCoordProp { get; } = new(PropertyNames.ParallaxMapOffsetYCoord);
        public ParticlesGUI.Property ParallaxMapSliceCountProp { get; } = new(PropertyNames.ParallaxMapSliceCount);
        public ParticlesGUI.Property ParallaxMapChannel { get; } = new(PropertyNames.ParallaxMapChannel);
        public ParticlesGUI.Property ParallaxStrengthProp { get; } = new(PropertyNames.ParallaxStrength);
        public ParticlesGUI.Property ParallaxMapTargetProp { get; } = new(PropertyNames.ParallaxMapTarget);

        #endregion

        #region Color Correction Matrial Propreties

        public ParticlesGUI.Property ColorCorrectionModeProp { get; } = new(PropertyNames.ColorCorrectionMode);

        public ParticlesGUI.Property GradientMapProp { get; } = new(PropertyNames.GradientMap);

        #endregion

        #region Alpha Transition Material Properties

        public ParticlesGUI.Property AlphaTransitionModeProp { get; } = new(PropertyNames.AlphaTransitionMode);

        public ParticlesGUI.Property AlphaTransitionMapModeProp { get; } = new(PropertyNames.AlphaTransitionMapMode);

        public ParticlesGUI.Property AlphaTransitionMapProp { get; } = new(PropertyNames.AlphaTransitionMap);

        public ParticlesGUI.Property AlphaTransitionMap2DArrayProp { get; } =
            new(PropertyNames.AlphaTransitionMap2DArray);

        public ParticlesGUI.Property AlphaTransitionMap3DProp { get; } = new(PropertyNames.AlphaTransitionMap3D);

        public ParticlesGUI.Property AlphaTransitionMapProgressProp { get; } =
            new(PropertyNames.AlphaTransitionMapProgress);

        public ParticlesGUI.Property AlphaTransitionMapProgressCoordProp { get; } =
            new(PropertyNames.AlphaTransitionMapProgressCoord);

        public ParticlesGUI.Property AlphaTransitionMapOffsetXCoordProp { get; } =
            new(PropertyNames.AlphaTransitionMapOffsetXCoord);

        public ParticlesGUI.Property AlphaTransitionMapOffsetYCoordProp { get; } =
            new(PropertyNames.AlphaTransitionMapOffsetYCoord);

        public ParticlesGUI.Property AlphaTransitionMapChannelsXProp { get; } =
            new(PropertyNames.AlphaTransitionMapChannelsX);

        public ParticlesGUI.Property AlphaTransitionMapSliceCountProp { get; } =
            new(PropertyNames.AlphaTransitionMapSliceCount);

        public ParticlesGUI.Property DissolveSharpnessProp { get; } = new(PropertyNames.DissolveSharpness);

        public ParticlesGUI.Property AlphaTransitionSecondTextureBlendModeProp { get; } =
            new(PropertyNames.AlphaTransitionSecondTextureBlendMode);

        public ParticlesGUI.Property AlphaTransitionMapSecondTextureProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTexture);

        public ParticlesGUI.Property AlphaTransitionMapSecondTexture2DArrayProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTexture2DArray);

        public ParticlesGUI.Property AlphaTransitionMapSecondTexture3DProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTexture3D);

        public ParticlesGUI.Property AlphaTransitionMapSecondTextureProgressProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTextureProgress);

        public ParticlesGUI.Property AlphaTransitionMapSecondTextureProgressCoordProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTextureProgressCoord);

        public ParticlesGUI.Property AlphaTransitionMapSecondTextureOffsetXCoordProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTextureOffsetXCoord);

        public ParticlesGUI.Property AlphaTransitionMapSecondTextureOffsetYCoordProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTextureOffsetYCoord);

        public ParticlesGUI.Property AlphaTransitionMapSecondTextureChannelsXProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTextureChannelsX);

        public ParticlesGUI.Property AlphaTransitionMapSecondTextureSliceCountProp { get; } =
            new(PropertyNames.AlphaTransitionMapSecondTextureSliceCount);

        public ParticlesGUI.Property AlphaTransitionProgressProp { get; } = new(PropertyNames.AlphaTransitionProgress);

        public ParticlesGUI.Property AlphaTransitionProgressCoordProp { get; } =
            new(PropertyNames.AlphaTransitionProgressCoord);

        public ParticlesGUI.Property AlphaTransitionProgressSecondTextureProp { get; } =
            new(PropertyNames.AlphaTransitionProgressSecondTexture);

        public ParticlesGUI.Property AlphaTransitionProgressCoordSecondTextureProp { get; } =
            new(PropertyNames.AlphaTransitionProgressCoordSecondTexture);

        public ParticlesGUI.Property DissolveSharpnessSecondTextureProp { get; } =
            new(PropertyNames.DissolveSharpnessSecondTexture);

        #endregion

        #region Emission Material Properties

        public ParticlesGUI.Property EmissionAreaTypeProp { get; } = new(PropertyNames.EmissionAreaType);

        public ParticlesGUI.Property EmissionMapModeProp { get; } = new(PropertyNames.EmissionMapMode);

        public ParticlesGUI.Property EmissionMapProp { get; } = new(PropertyNames.EmissionMap);

        public ParticlesGUI.Property EmissionMap2DArrayProp { get; } = new(PropertyNames.EmissionMap2DArray);

        public ParticlesGUI.Property EmissionMap3DProp { get; } = new(PropertyNames.EmissionMap3D);

        public ParticlesGUI.Property EmissionMapProgressProp { get; } = new(PropertyNames.EmissionMapProgress);

        public ParticlesGUI.Property EmissionMapProgressCoordProp { get; } =
            new(PropertyNames.EmissionMapProgressCoord);

        public ParticlesGUI.Property EmissionMapOffsetXCoordProp { get; } = new(PropertyNames.EmissionMapOffsetXCoord);

        public ParticlesGUI.Property EmissionMapOffsetYCoordProp { get; } = new(PropertyNames.EmissionMapOffsetYCoord);

        public ParticlesGUI.Property EmissionMapChannelsXProp { get; } = new(PropertyNames.EmissionMapChannelsX);

        public ParticlesGUI.Property EmissionMapSliceCountProp { get; } = new(PropertyNames.EmissionMapSliceCount);

        public ParticlesGUI.Property EmissionColorTypeProp { get; } = new(PropertyNames.EmissionColorType);

        public ParticlesGUI.Property EmissionColorProp { get; } = new(PropertyNames.EmissionColor);

        public ParticlesGUI.Property EmissionColorRampProp { get; } = new(PropertyNames.EmissionColorRamp);

        public ParticlesGUI.Property EmissionIntensityProp { get; } = new(PropertyNames.EmissionIntensity);

        public ParticlesGUI.Property EmissionIntensityCoordProp { get; } = new(PropertyNames.EmissionIntensityCoord);

        public ParticlesGUI.Property KeepEdgeTransparencyProp { get; } = new(PropertyNames.KeepEdgeTransparency);

        #endregion

        #region Transparency Material Properties

        public ParticlesGUI.Property RimTransparencyEnabledProp { get; } = new(PropertyNames.RimTransparencyEnabled);

        public ParticlesGUI.Property RimTransparencyProgressProp { get; } = new(PropertyNames.RimTransparencyProgress);

        public ParticlesGUI.Property RimTransparencyProgressCoordProp { get; } =
            new(PropertyNames.RimTransparencyProgressCoord);

        public ParticlesGUI.Property RimTransparencySharpnessProp { get; } =
            new(PropertyNames.RimTransparencySharpness);

        public ParticlesGUI.Property RimTransparencySharpnessCoordProp { get; } =
            new(PropertyNames.RimTransparencySharpnessCoord);

        public ParticlesGUI.Property InverseRimTransparencyProp { get; } = new(PropertyNames.InverseRimTransparency);

        public ParticlesGUI.Property LuminanceTransparencyEnabledProp { get; } =
            new(PropertyNames.LuminanceTransparencyEnabled);

        public ParticlesGUI.Property LuminanceTransparencyProgressProp { get; } =
            new(PropertyNames.LuminanceTransparencyProgress);

        public ParticlesGUI.Property LuminanceTransparencyProgressCoordProp { get; } =
            new(PropertyNames.LuminanceTransparencyProgressCoord);

        public ParticlesGUI.Property LuminanceTransparencySharpnessProp { get; } =
            new(PropertyNames.LuminanceTransparencySharpness);

        public ParticlesGUI.Property LuminanceTransparencySharpnessCoordProp { get; } =
            new(PropertyNames.LuminanceTransparencySharpnessCoord);

        public ParticlesGUI.Property InverseLuminanceTransparencyProp { get; } =
            new(PropertyNames.InverseLuminanceTransparency);

        public ParticlesGUI.Property SoftParticlesEnabledProp { get; } = new(PropertyNames.SoftParticlesEnabled);

        public ParticlesGUI.Property SoftParticlesIntensityProp { get; } = new(PropertyNames.SoftParticlesIntensity);

        public ParticlesGUI.Property DepthFadeEnabledProp { get; } = new(PropertyNames.DepthFadeEnabled);

        public ParticlesGUI.Property DepthFadeNearProp { get; } = new(PropertyNames.DepthFadeNear);

        public ParticlesGUI.Property
            DepthFadeFarProp { get; } = new(PropertyNames.DepthFadeFar);

        public ParticlesGUI.Property DepthFadeWidthProp { get; } = new(PropertyNames.DepthFadeWidth);

        #endregion

        #region VertexDeformation Map Material Properties

        public ParticlesGUI.Property VertexDeformationMapProp { get; } = new(PropertyNames.VertexDeformationMap);

        public ParticlesGUI.Property VertexDeformationMapOffsetXCoordProp { get; } =
            new(PropertyNames.VertexDeformationMapOffsetXCoord);

        public ParticlesGUI.Property VertexDeformationMapOffsetYCoordProp { get; } =
            new(PropertyNames.VertexDeformationMapOffsetYCoord);

        public ParticlesGUI.Property VertexDeformationMapChannelProp { get; } =
            new(PropertyNames.VertexDeformationMapChannel);

        public ParticlesGUI.Property VertexDeformationIntensityProp { get; } =
            new(PropertyNames.VertexDeformationIntensity);

        public ParticlesGUI.Property VertexDeformationBaseValueProp { get; } =
            new(PropertyNames.VertexDeformationBaseValue);

        public ParticlesGUI.Property VertexDeformationIntensityCoordProp { get; } =
            new(PropertyNames.VertexDeformationIntensityCoord);

        #endregion

        #region Shadow Caster Material Properties

        public ParticlesGUI.Property ShadowCasterEnabledProp { get; } = new(PropertyNames.ShadowCasterEnabled);

        public ParticlesGUI.Property ShadowCasterApplyVertexDeformationProp { get; } =
            new(PropertyNames.ShadowCasterApplyVertexDeformation);

        public ParticlesGUI.Property ShadowCasterAlphaTestEnabledProp { get; } =
            new(PropertyNames.ShadowCasterAlphaTestEnabled);

        public ParticlesGUI.Property ShadowCasterAlphaCutoffProp { get; } = new(PropertyNames.ShadowCasterAlphaCutoff);

        public ParticlesGUI.Property ShadowCasterAlphaAffectedByTintColorProp { get; } =
            new(PropertyNames.ShadowCasterAlphaAffectedByTintColor);

        public ParticlesGUI.Property ShadowCasterAlphaAffectedByFlowMapProp { get; } =
            new(PropertyNames.ShadowCasterAlphaAffectedByFlowMap);

        public ParticlesGUI.Property ShadowCasterAlphaAffectedByAlphaTransitionMapProp { get; } =
            new(PropertyNames.ShadowCasterAlphaAffectedByAlphaTransitionMap);

        public ParticlesGUI.Property ShadowCasterAlphaAffectedByTransparencyLuminanceProp { get; } =
            new(PropertyNames.ShadowCasterAlphaAffectedByTransparencyLuminance);

        #endregion
    }
}
