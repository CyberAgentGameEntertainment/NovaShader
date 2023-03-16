// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    internal class ParticlesUberCommonMaterialProperties
    {
        public ParticlesUberCommonMaterialProperties(MaterialEditor editor, MaterialProperty[] properties)
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
            ParallaxScaleProp.Setup(properties);
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

        #endregion

        #region Render Settings Material Properties

        public ParticlesGUI.Property RenderTypeProp { get; } = new ParticlesGUI.Property(PropertyNames.RenderType);
        public ParticlesGUI.Property CutoffProp { get; } = new ParticlesGUI.Property(PropertyNames.Cutoff);

        public ParticlesGUI.Property TransparentBlendModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TransparentBlendMode);

        public ParticlesGUI.Property CullProp { get; } = new ParticlesGUI.Property(PropertyNames.Cull);
        public ParticlesGUI.Property QueueOffsetProp { get; } = new ParticlesGUI.Property(PropertyNames.QueueOffset);

        public ParticlesGUI.Property VertexAlphaModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.VertexAlphaMode);

        public ParticlesGUI.Property BlendDstProp { get; } = new ParticlesGUI.Property(PropertyNames.BlendDst);
        public ParticlesGUI.Property BlendSrcProp { get; } = new ParticlesGUI.Property(PropertyNames.BlendSrc);
        public ParticlesGUI.Property ZWriteProp { get; } = new ParticlesGUI.Property(PropertyNames.ZWrite);

        #endregion

        #region Base Map Material Properties

        public ParticlesGUI.Property BaseMapModeProp { get; } = new ParticlesGUI.Property(PropertyNames.BaseMapMode);
        public ParticlesGUI.Property BaseMapProp { get; } = new ParticlesGUI.Property(PropertyNames.BaseMap);

        public ParticlesGUI.Property BaseMap2DArrayProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMap2DArray);

        public ParticlesGUI.Property BaseMap3DProp { get; } = new ParticlesGUI.Property(PropertyNames.BaseMap3D);

        public ParticlesGUI.Property BaseMapProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapProgress);

        public ParticlesGUI.Property BaseMapProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapProgressCoord);

        public ParticlesGUI.Property BaseMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapSliceCount);

        public ParticlesGUI.Property BaseColorProp { get; } = new ParticlesGUI.Property(PropertyNames.TintColor);

        public ParticlesGUI.Property BaseMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapOffsetXCoord);

        public ParticlesGUI.Property BaseMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapOffsetYCoord);

        public ParticlesGUI.Property BaseMapRotationProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapRotation);

        public ParticlesGUI.Property BaseMapRotationCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapRotationCoord);

        public ParticlesGUI.Property BaseMapRotationOffsetsProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapRotationOffsets);

        public ParticlesGUI.Property BaseMapMirrorSamplingProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapMirrorSampling);

        #endregion

        #region Tint Color Material Properties

        public ParticlesGUI.Property TintAreaModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintAreaMode);

        public ParticlesGUI.Property TintColorModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintColorMode);

        public ParticlesGUI.Property TintMapProp { get; } = new ParticlesGUI.Property(PropertyNames.TintMap);
        public ParticlesGUI.Property TintMap3DProp { get; } = new ParticlesGUI.Property(PropertyNames.TintMap3D);

        public ParticlesGUI.Property TintMap3DProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMap3DProgress);

        public ParticlesGUI.Property TintMap3DProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMap3DProgressCoord);

        public ParticlesGUI.Property TintMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMapSliceCount);

        public ParticlesGUI.Property TintMapBlendRateProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMapBlendRate);

        public ParticlesGUI.Property TintMapBlendRateCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMapBlendRateCoord);

        public ParticlesGUI.Property TintRimProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimProgress);

        public ParticlesGUI.Property TintRimProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimProgressCoord);

        public ParticlesGUI.Property TintRimSharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimSharpness);

        public ParticlesGUI.Property TintRimSharpnessCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimSharpnessCoord);

        public ParticlesGUI.Property InverseTintRimProp { get; } =
            new ParticlesGUI.Property(PropertyNames.InverseTintRim);

        #endregion

        #region Flow Map Material Properties

        public ParticlesGUI.Property FlowMapProp { get; } = new ParticlesGUI.Property(PropertyNames.FlowMap);

        public ParticlesGUI.Property FlowMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapOffsetXCoord);

        public ParticlesGUI.Property FlowMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapOffsetYCoord);

        public ParticlesGUI.Property FlowMapChannelsXProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapChannelsX);

        public ParticlesGUI.Property FlowMapChannelsYProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapChannelsY);

        public ParticlesGUI.Property FlowIntensityProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowIntensity);

        public ParticlesGUI.Property FlowIntensityCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowIntensityCoord);

        public ParticlesGUI.Property FlowMapTargetProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapTarget);

        #endregion
        
        #region Paralax Map Material Properties

        public ParticlesGUI.Property ParallaxMapModeProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapMode);
        public ParticlesGUI.Property ParallaxMapProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMap);
        public ParticlesGUI.Property ParallaxMap2DArrayProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMap2DArray);
        public ParticlesGUI.Property ParallaxMap3DProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMap3D);
        public ParticlesGUI.Property ParallaxMapProgressProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapProgress);
        public ParticlesGUI.Property ParallaxMapProgressCoordProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapProgressCoord);
        public ParticlesGUI.Property ParallaxMapOffsetXCoordProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapOffsetXCoord);
        public ParticlesGUI.Property ParallaxMapOffsetYCoordProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapOffsetYCoord);
        public ParticlesGUI.Property ParallaxMapSliceCountProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapSliceCount);
        public ParticlesGUI.Property ParallaxMapChannel { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapChannel);
        public ParticlesGUI.Property ParallaxScaleProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxScale);
        public ParticlesGUI.Property ParallaxMapTargetProp { get; } = new ParticlesGUI.Property(PropertyNames.ParallaxMapTarget);

        #endregion

        #region Color Correction Matrial Propreties

        public ParticlesGUI.Property ColorCorrectionModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.ColorCorrectionMode);

        public ParticlesGUI.Property GradientMapProp { get; } = new ParticlesGUI.Property(PropertyNames.GradientMap);

        #endregion

        #region Alpha Transition Material Properties

        public ParticlesGUI.Property AlphaTransitionModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMode);

        public ParticlesGUI.Property AlphaTransitionMapModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapMode);

        public ParticlesGUI.Property AlphaTransitionMapProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMap);

        public ParticlesGUI.Property AlphaTransitionMap2DArrayProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMap2DArray);

        public ParticlesGUI.Property AlphaTransitionMap3DProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMap3D);

        public ParticlesGUI.Property AlphaTransitionMapProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapProgress);

        public ParticlesGUI.Property AlphaTransitionMapProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapProgressCoord);

        public ParticlesGUI.Property AlphaTransitionMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapOffsetXCoord);

        public ParticlesGUI.Property AlphaTransitionMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapOffsetYCoord);

        public ParticlesGUI.Property AlphaTransitionMapChannelsXProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapChannelsX);

        public ParticlesGUI.Property AlphaTransitionMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapSliceCount);

        public ParticlesGUI.Property AlphaTransitionProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionProgress);

        public ParticlesGUI.Property AlphaTransitionProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionProgressCoord);

        public ParticlesGUI.Property DissolveSharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DissolveSharpness);

        #endregion

        #region Emission Material Properties

        public ParticlesGUI.Property EmissionAreaTypeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionAreaType);

        public ParticlesGUI.Property EmissionMapModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapMode);

        public ParticlesGUI.Property EmissionMapProp { get; } = new ParticlesGUI.Property(PropertyNames.EmissionMap);

        public ParticlesGUI.Property EmissionMap2DArrayProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMap2DArray);

        public ParticlesGUI.Property EmissionMap3DProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMap3D);

        public ParticlesGUI.Property EmissionMapProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapProgress);

        public ParticlesGUI.Property EmissionMapProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapProgressCoord);

        public ParticlesGUI.Property EmissionMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapOffsetXCoord);

        public ParticlesGUI.Property EmissionMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapOffsetYCoord);

        public ParticlesGUI.Property EmissionMapChannelsXProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapChannelsX);

        public ParticlesGUI.Property EmissionMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapSliceCount);

        public ParticlesGUI.Property EmissionColorTypeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionColorType);

        public ParticlesGUI.Property EmissionColorProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionColor);

        public ParticlesGUI.Property EmissionColorRampProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionColorRamp);

        public ParticlesGUI.Property EmissionIntensityProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionIntensity);

        public ParticlesGUI.Property EmissionIntensityCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionIntensityCoord);

        public ParticlesGUI.Property KeepEdgeTransparencyProp { get; } =
            new ParticlesGUI.Property(PropertyNames.KeepEdgeTransparency);

        #endregion

        #region Transparency Material Properties

        public ParticlesGUI.Property RimTransparencyEnabledProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencyEnabled);

        public ParticlesGUI.Property RimTransparencyProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencyProgress);

        public ParticlesGUI.Property RimTransparencyProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencyProgressCoord);

        public ParticlesGUI.Property RimTransparencySharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencySharpness);

        public ParticlesGUI.Property RimTransparencySharpnessCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencySharpnessCoord);

        public ParticlesGUI.Property InverseRimTransparencyProp { get; } =
            new ParticlesGUI.Property(PropertyNames.InverseRimTransparency);

        public ParticlesGUI.Property LuminanceTransparencyEnabledProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencyEnabled);

        public ParticlesGUI.Property LuminanceTransparencyProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencyProgress);

        public ParticlesGUI.Property LuminanceTransparencyProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencyProgressCoord);

        public ParticlesGUI.Property LuminanceTransparencySharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencySharpness);

        public ParticlesGUI.Property LuminanceTransparencySharpnessCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencySharpnessCoord);

        public ParticlesGUI.Property InverseLuminanceTransparencyProp { get; } =
            new ParticlesGUI.Property(PropertyNames.InverseLuminanceTransparency);

        public ParticlesGUI.Property SoftParticlesEnabledProp { get; } =
            new ParticlesGUI.Property(PropertyNames.SoftParticlesEnabled);

        public ParticlesGUI.Property SoftParticlesIntensityProp { get; } =
            new ParticlesGUI.Property(PropertyNames.SoftParticlesIntensity);

        public ParticlesGUI.Property DepthFadeEnabledProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DepthFadeEnabled);

        public ParticlesGUI.Property DepthFadeNearProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DepthFadeNear);

        public ParticlesGUI.Property
            DepthFadeFarProp { get; } = new ParticlesGUI.Property(PropertyNames.DepthFadeFar);

        public ParticlesGUI.Property DepthFadeWidthProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DepthFadeWidth);

        #endregion
        
        #region VertexDeformation Map Material Properties

        public ParticlesGUI.Property VertexDeformationMapProp { get; } = new ParticlesGUI.Property(PropertyNames.VertexDeformationMap);

        public ParticlesGUI.Property VertexDeformationMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.VertexDeformationMapOffsetXCoord);

        public ParticlesGUI.Property VertexDeformationMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.VertexDeformationMapOffsetYCoord);

        public ParticlesGUI.Property VertexDeformationMapChannelProp { get; } =
            new ParticlesGUI.Property(PropertyNames.VertexDeformationMapChannel);

        public ParticlesGUI.Property VertexDeformationIntensityProp { get; } =
            new ParticlesGUI.Property(PropertyNames.VertexDeformationIntensity);

        public ParticlesGUI.Property VertexDeformationIntensityCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.VertexDeformationIntensityCoord);

        #endregion
    }
}
