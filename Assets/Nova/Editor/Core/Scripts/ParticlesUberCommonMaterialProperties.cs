// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using Codice.CM.Interfaces;
using Nova.Editor.Foundation.Scripts;
using NUnit.Framework.Internal;
using UnityEditor;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    public class ParticlesUberCommonMaterialProperties
    {
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
        }
        internal void Initialize(MaterialEditor editor, MaterialProperty[] properties)
        {
            
            var prefsKeyPrefix = $"{GetType().Namespace}.{GetType().Name}.";
            var renderSettingsFoldoutKey = $"{prefsKeyPrefix}{nameof(RenderSettingsFoldout)}";
            var baseMapFoldoutKey = $"{prefsKeyPrefix}{nameof(BaseMapFoldout)}";
            var tintColorFoldoutKey = $"{prefsKeyPrefix}{nameof(TintColorFoldout)}";
            var flowMapFoldoutKey = $"{prefsKeyPrefix}{nameof(FlowMapFoldout)}";
            var colorCorrectionFoldoutKey = $"{prefsKeyPrefix}{nameof(ColorCorrectionFoldout)}";
            var transparencyFoldoutKey = $"{prefsKeyPrefix}{nameof(TransparencyFoldout)}";
            var alphaTransitionFoldoutKey = $"{prefsKeyPrefix}{nameof(AlphaTransitionFoldout)}";
            var emissionFoldoutKey = $"{prefsKeyPrefix}{nameof(EmissionFoldout)}";

            RenderSettingsFoldout = new BoolEditorPrefsProperty(renderSettingsFoldoutKey, true);
            BaseMapFoldout = new BoolEditorPrefsProperty(baseMapFoldoutKey, true);
            TintColorFoldout = new BoolEditorPrefsProperty(tintColorFoldoutKey, true);
            FlowMapFoldout = new BoolEditorPrefsProperty(flowMapFoldoutKey, true);
            ColorCorrectionFoldout = new BoolEditorPrefsProperty(colorCorrectionFoldoutKey, true);
            TransparencyFoldout = new BoolEditorPrefsProperty(transparencyFoldoutKey, true);
            AlphaTransitionFoldout = new BoolEditorPrefsProperty(alphaTransitionFoldoutKey, true);
            EmissionFoldout = new BoolEditorPrefsProperty(emissionFoldoutKey, true);

        }

        #region Foldout Properties

        internal BoolEditorPrefsProperty AlphaTransitionFoldout { get; private set; }
        internal BoolEditorPrefsProperty BaseMapFoldout { get; private set; }
        internal BoolEditorPrefsProperty TintColorFoldout { get; private set; }
        internal BoolEditorPrefsProperty FlowMapFoldout { get; private set; }
        internal BoolEditorPrefsProperty ColorCorrectionFoldout { get; private set; }
        internal BoolEditorPrefsProperty EmissionFoldout { get; private set; }
        internal BoolEditorPrefsProperty RenderSettingsFoldout { get; private set; }
        internal BoolEditorPrefsProperty TransparencyFoldout { get; private set; }
        

        #endregion

        #region Render Settings Material Properties

        internal ParticlesGUI.Property RenderTypeProp { get;  } = new ParticlesGUI.Property(PropertyNames.RenderType);
        internal ParticlesGUI.Property CutoffProp { get; } = new ParticlesGUI.Property(PropertyNames.Cutoff);

        internal ParticlesGUI.Property TransparentBlendModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TransparentBlendMode);
        internal ParticlesGUI.Property CullProp { get; } = new ParticlesGUI.Property(PropertyNames.Cull);
        internal ParticlesGUI.Property QueueOffsetProp { get; } = new ParticlesGUI.Property(PropertyNames.QueueOffset);
        internal ParticlesGUI.Property VertexAlphaModeProp { get; } = new ParticlesGUI.Property(PropertyNames.VertexAlphaMode);

        internal ParticlesGUI.Property BlendDstProp { get; } = new ParticlesGUI.Property(PropertyNames.BlendDst);
        internal ParticlesGUI.Property BlendSrcProp { get; } = new ParticlesGUI.Property(PropertyNames.BlendSrc);
        internal ParticlesGUI.Property ZWriteProp { get; } = new ParticlesGUI.Property(PropertyNames.ZWrite);

        #endregion

        #region Base Map Material Properties

        internal ParticlesGUI.Property BaseMapModeProp { get; } = new ParticlesGUI.Property(PropertyNames.BaseMapMode);
        internal ParticlesGUI.Property BaseMapProp { get; } = new ParticlesGUI.Property(PropertyNames.BaseMap);
        internal ParticlesGUI.Property BaseMap2DArrayProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMap2DArray);
        internal ParticlesGUI.Property BaseMap3DProp { get; } = new ParticlesGUI.Property(PropertyNames.BaseMap3D);
        internal ParticlesGUI.Property BaseMapProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapProgress);

        internal ParticlesGUI.Property BaseMapProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapProgressCoord);

        internal ParticlesGUI.Property BaseMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapSliceCount);

        internal ParticlesGUI.Property BaseColorProp { get; } = new ParticlesGUI.Property(PropertyNames.TintColor);

        internal ParticlesGUI.Property BaseMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapOffsetXCoord);

        internal ParticlesGUI.Property BaseMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapOffsetYCoord);

        internal ParticlesGUI.Property BaseMapRotationProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapRotation);

        internal ParticlesGUI.Property BaseMapRotationCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapRotationCoord);

        internal ParticlesGUI.Property BaseMapRotationOffsetsProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapRotationOffsets);

        internal ParticlesGUI.Property BaseMapMirrorSamplingProp { get; } =
            new ParticlesGUI.Property(PropertyNames.BaseMapMirrorSampling);

        #endregion

        #region Tint Color Material Properties

        internal ParticlesGUI.Property TintAreaModeProp { get; } = new ParticlesGUI.Property(PropertyNames.TintAreaMode);

        internal ParticlesGUI.Property TintColorModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintColorMode);

        internal ParticlesGUI.Property TintMapProp { get; } = new ParticlesGUI.Property(PropertyNames.TintMap);
        internal ParticlesGUI.Property TintMap3DProp { get; } = new ParticlesGUI.Property(PropertyNames.TintMap3D);

        internal ParticlesGUI.Property TintMap3DProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMap3DProgress);

        internal ParticlesGUI.Property TintMap3DProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMap3DProgressCoord);

        internal ParticlesGUI.Property TintMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMapSliceCount);

        internal ParticlesGUI.Property TintMapBlendRateProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMapBlendRate);

        internal ParticlesGUI.Property TintMapBlendRateCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintMapBlendRateCoord);

        internal ParticlesGUI.Property TintRimProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimProgress);

        internal ParticlesGUI.Property TintRimProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimProgressCoord);

        internal ParticlesGUI.Property TintRimSharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimSharpness);

        internal ParticlesGUI.Property TintRimSharpnessCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.TintRimSharpnessCoord);

        internal ParticlesGUI.Property InverseTintRimProp { get; } =
            new ParticlesGUI.Property(PropertyNames.InverseTintRim);

        #endregion

        #region Flow Map Material Properties

        internal ParticlesGUI.Property FlowMapProp { get; } = new ParticlesGUI.Property(PropertyNames.FlowMap);

        internal ParticlesGUI.Property FlowMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapOffsetXCoord);

        internal ParticlesGUI.Property FlowMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapOffsetYCoord);

        internal ParticlesGUI.Property FlowMapChannelsXProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapChannelsX);

        internal ParticlesGUI.Property FlowMapChannelsYProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapChannelsY);

        internal ParticlesGUI.Property FlowIntensityProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowIntensity);

        internal ParticlesGUI.Property FlowIntensityCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowIntensityCoord);

        internal ParticlesGUI.Property FlowMapTargetProp { get; } =
            new ParticlesGUI.Property(PropertyNames.FlowMapTarget);

        #endregion

        #region Color Correction Matrial Propreties

        internal ParticlesGUI.Property ColorCorrectionModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.ColorCorrectionMode);

        internal ParticlesGUI.Property GradientMapProp { get; } = new ParticlesGUI.Property(PropertyNames.GradientMap);

        #endregion

        #region Alpha Transition Material Properties

        internal ParticlesGUI.Property AlphaTransitionModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMode);

        internal ParticlesGUI.Property AlphaTransitionMapModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapMode);

        internal ParticlesGUI.Property AlphaTransitionMapProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMap);

        internal ParticlesGUI.Property AlphaTransitionMap2DArrayProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMap2DArray);

        internal ParticlesGUI.Property AlphaTransitionMap3DProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMap3D);

        internal ParticlesGUI.Property AlphaTransitionMapProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapProgress);

        internal ParticlesGUI.Property AlphaTransitionMapProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapProgressCoord);

        internal ParticlesGUI.Property AlphaTransitionMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapOffsetXCoord);

        internal ParticlesGUI.Property AlphaTransitionMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapOffsetYCoord);

        internal ParticlesGUI.Property AlphaTransitionMapChannelsXProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapChannelsX);

        internal ParticlesGUI.Property AlphaTransitionMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionMapSliceCount);

        internal ParticlesGUI.Property AlphaTransitionProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionProgress);

        internal ParticlesGUI.Property AlphaTransitionProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.AlphaTransitionProgressCoord);

        internal ParticlesGUI.Property DissolveSharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DissolveSharpness);

        #endregion

        #region Emission Material Properties

        internal ParticlesGUI.Property EmissionAreaTypeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionAreaType);

        internal ParticlesGUI.Property EmissionMapModeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapMode);

        internal ParticlesGUI.Property EmissionMapProp { get; } = new ParticlesGUI.Property(PropertyNames.EmissionMap);

        internal ParticlesGUI.Property EmissionMap2DArrayProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMap2DArray);

        internal ParticlesGUI.Property EmissionMap3DProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMap3D);

        internal ParticlesGUI.Property EmissionMapProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapProgress);

        internal ParticlesGUI.Property EmissionMapProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapProgressCoord);

        internal ParticlesGUI.Property EmissionMapOffsetXCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapOffsetXCoord);

        internal ParticlesGUI.Property EmissionMapOffsetYCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapOffsetYCoord);

        internal ParticlesGUI.Property EmissionMapChannelsXProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapChannelsX);

        internal ParticlesGUI.Property EmissionMapSliceCountProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionMapSliceCount);

        internal ParticlesGUI.Property EmissionColorTypeProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionColorType);

        internal ParticlesGUI.Property EmissionColorProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionColor);

        internal ParticlesGUI.Property EmissionColorRampProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionColorRamp);

        internal ParticlesGUI.Property EmissionIntensityProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionIntensity);

        internal ParticlesGUI.Property EmissionIntensityCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.EmissionIntensityCoord);

        internal ParticlesGUI.Property KeepEdgeTransparencyProp { get; } =
            new ParticlesGUI.Property(PropertyNames.KeepEdgeTransparency);

        #endregion

        #region Transparency Material Properties

        internal ParticlesGUI.Property RimTransparencyEnabledProp{get;} =
            new ParticlesGUI.Property(PropertyNames.RimTransparencyEnabled);

        internal ParticlesGUI.Property RimTransparencyProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencyProgress);

        internal ParticlesGUI.Property RimTransparencyProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencyProgressCoord);

        internal ParticlesGUI.Property RimTransparencySharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencySharpness);

        internal ParticlesGUI.Property RimTransparencySharpnessCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.RimTransparencySharpnessCoord);

        internal ParticlesGUI.Property InverseRimTransparencyProp { get; } =
            new ParticlesGUI.Property(PropertyNames.InverseRimTransparency);

        internal ParticlesGUI.Property LuminanceTransparencyEnabledProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencyEnabled);

        internal ParticlesGUI.Property LuminanceTransparencyProgressProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencyProgress);

        internal ParticlesGUI.Property LuminanceTransparencyProgressCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencyProgressCoord);

        internal ParticlesGUI.Property LuminanceTransparencySharpnessProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencySharpness);

        internal ParticlesGUI.Property LuminanceTransparencySharpnessCoordProp { get; } =
            new ParticlesGUI.Property(PropertyNames.LuminanceTransparencySharpnessCoord);

        internal ParticlesGUI.Property InverseLuminanceTransparencyProp { get; } =
            new ParticlesGUI.Property(PropertyNames.InverseLuminanceTransparency);

        internal ParticlesGUI.Property SoftParticlesEnabledProp { get; } =
            new ParticlesGUI.Property(PropertyNames.SoftParticlesEnabled);

        internal ParticlesGUI.Property SoftParticlesIntensityProp { get; } =
            new ParticlesGUI.Property(PropertyNames.SoftParticlesIntensity);

        internal ParticlesGUI.Property DepthFadeEnabledProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DepthFadeEnabled);

        internal ParticlesGUI.Property DepthFadeNearProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DepthFadeNear);

        internal ParticlesGUI.Property
            DepthFadeFarProp { get; } = new ParticlesGUI.Property(PropertyNames.DepthFadeFar);

        internal ParticlesGUI.Property DepthFadeWidthProp { get; } =
            new ParticlesGUI.Property(PropertyNames.DepthFadeWidth);

        #endregion

       
    }
}