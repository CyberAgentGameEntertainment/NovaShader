// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     Common GUI for ParticleUberUnlit, ParticleUberLit classes.
    /// </summary>
    internal class ParticlesUberCommonGUI
    {
        # region private variable

        private const int RenderPriorityMax = 50;
        private const int RenderPriorityMin = -RenderPriorityMax;
        private MaterialEditor _editor;
        private ParticlesUberCommonMaterialProperties _commonMaterialProperties;

        # endregion

        # region internal method

        public void Setup(MaterialEditor editor, ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            _editor = editor;
            _commonMaterialProperties = commonMaterialProperties;
        }

        public void DrawRenderSettingsProperties()
        {
            DrawProperties(_commonMaterialProperties.RenderSettingsFoldout,
                "Render Settings", InternalDrawRenderSettingsProperties);
        }

        public void DrawBaseMapProperties()
        {
            DrawProperties(_commonMaterialProperties.BaseMapFoldout,
                "Base Map", InternalDrawBaseMapProperties);
        }

        public void DrawTintColorProperties()
        {
            DrawProperties(_commonMaterialProperties.TintColorFoldout,
                "Tint Color", InternalDrawTintColorProperties);
        }

        public void DrawFlowMapProperties()
        {
            DrawProperties(_commonMaterialProperties.FlowMapFoldout,
                "Flow Map", InternalDrawFlowMapProperties);
        }

        public void DrawColorCorrectionProperties()
        {
            DrawProperties(_commonMaterialProperties.ColorCorrectionFoldout,
                "Color Correction", InternalDrawColorCorrectionProperties);
        }

        public void DrawAlphaTransitionProperties()
        {
            DrawProperties(_commonMaterialProperties.AlphaTransitionFoldout,
                "Alpha Transition", InternalDrawAlphaTransitionProperties);
        }

        public void DrawEmissionProperties()
        {
            DrawProperties(_commonMaterialProperties.EmissionFoldout,
                "Emission", InternalDrawEmissionProperties);
        }

        public void DrawTransparencyProperties()
        {
            DrawProperties(_commonMaterialProperties.TransparencyFoldout,
                "Transparency", InternalDrawTransparencyProperties);
        }

        #endregion

        #region private method

        private void DrawProperties(BoolEditorPrefsProperty foldout, string categoryName, Action internalDrawFunction)
        {
            using (var foldoutScope = new MaterialEditorUtility.FoldoutHeaderScope(foldout.Value, categoryName))
            {
                if (foldoutScope.Foldout) internalDrawFunction();

                foldout.Value = foldoutScope.Foldout;
            }
        }

        private void InternalDrawRenderSettingsProperties()
        {
            var props = _commonMaterialProperties;

            MaterialEditorUtility.DrawEnumProperty<RenderType>(_editor, "Render Type", props.RenderTypeProp.Value);
            var renderType = (RenderType)props.RenderTypeProp.Value.floatValue;
            if (renderType == RenderType.Cutout)
                _editor.ShaderProperty(props.CutoffProp.Value, "Cutoff");
            else if (renderType == RenderType.Transparent)
                MaterialEditorUtility.DrawEnumProperty<TransparentBlendMode>(_editor, "Blend Mode",
                    props.TransparentBlendModeProp.Value);

            MaterialEditorUtility.DrawEnumProperty<RenderFace>(_editor, "Render Face", props.CullProp.Value);
            MaterialEditorUtility.DrawIntRangeProperty(_editor, "Render Priority", props.QueueOffsetProp.Value,
                RenderPriorityMin, RenderPriorityMax);
            MaterialEditorUtility.DrawEnumProperty<VertexAlphaMode>(_editor, "Vertex Alpha Mode",
                props.VertexAlphaModeProp.Value);
        }

        private void InternalDrawBaseMapProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawEnumProperty<BaseMapMode>(_editor, "Mode", props.BaseMapModeProp.Value);
            var baseMapMode = (BaseMapMode)props.BaseMapModeProp.Value.floatValue;
            MaterialProperty baseMapMaterialProp;
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    baseMapMaterialProp = props.BaseMapProp.Value;
                    break;
                case BaseMapMode.FlipBook:
                    baseMapMaterialProp = props.BaseMap2DArrayProp.Value;
                    break;
                case BaseMapMode.FlipBookBlending:
                    baseMapMaterialProp = props.BaseMap3DProp.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                MaterialEditorUtility.DrawTexture(_editor, baseMapMaterialProp, props.BaseMapOffsetXCoordProp.Value,
                    props.BaseMapOffsetYCoordProp.Value, null, null);

                if (changeCheckScope.changed)
                {
                    if (baseMapMode == BaseMapMode.FlipBook && props.BaseMap2DArrayProp.Value.textureValue != null)
                    {
                        var tex2DArray = (Texture2DArray)props.BaseMap2DArrayProp.Value.textureValue;
                        props.BaseMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                    }

                    if (baseMapMode == BaseMapMode.FlipBookBlending &&
                        props.BaseMap3DProp.Value.textureValue != null)
                    {
                        var tex3D = (Texture3D)props.BaseMap3DProp.Value.textureValue;
                        props.BaseMapSliceCountProp.Value.floatValue = tex3D.depth;
                    }
                }
            }

            MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Rotation",
                props.BaseMapRotationProp.Value, props.BaseMapRotationCoordProp.Value);
            using (new EditorGUI.IndentLevelScope())
            {
                MaterialEditorUtility.DrawVector2Property(_editor, "Offset", props.BaseMapRotationOffsetsProp.Value);
            }

            MaterialEditorUtility.DrawToggleProperty(_editor, "Mirror Sampling",
                props.BaseMapMirrorSamplingProp.Value);

            if (baseMapMode == BaseMapMode.FlipBook || baseMapMode == BaseMapMode.FlipBookBlending)
                MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Flip-Book Progress",
                    props.BaseMapProgressProp.Value, props.BaseMapProgressCoordProp.Value);
        }

        private void InternalDrawTintColorProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawEnumProperty<TintAreaMode>(_editor, "Mode", props.TintAreaModeProp.Value);
            var tintAreaMode = (TintAreaMode)props.TintAreaModeProp.Value.floatValue;
            if (tintAreaMode == TintAreaMode.None) return;

            if (tintAreaMode == TintAreaMode.Rim)
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(
                        _editor, "Progress", props.TintRimProgressProp.Value,
                        props.TintRimSharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Sharpness",
                        props.TintRimSharpnessProp.Value,
                        props.TintRimSharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawToggleProperty(_editor, "Inverse", props.InverseTintRimProp.Value);
                }

            MaterialEditorUtility.DrawEnumProperty<TintColorMode>(_editor, "Color Mode", props.TintColorModeProp.Value);
            var tintColorMode = (TintColorMode)props.TintColorModeProp.Value.floatValue;
            if (tintColorMode == TintColorMode.SingleColor)
            {
                _editor.ShaderProperty(props.BaseColorProp.Value, "Color");
            }
            else if (tintColorMode == TintColorMode.Texture2D)
            {
                MaterialEditorUtility.DrawTexture(_editor, props.TintMapProp.Value, true);
            }
            else if (tintColorMode == TintColorMode.Texture3D)
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTexture(_editor, props.TintMap3DProp.Value, true);

                    if (changeCheckScope.changed && props.TintMap3DProp.Value.textureValue != null)
                    {
                        var tex3D = (Texture3D)props.TintMap3DProp.Value.textureValue;
                        props.TintMapSliceCountProp.Value.floatValue = tex3D.depth;
                    }
                }

                MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Progress",
                    props.TintMap3DProgressProp.Value, props.TintMap3DProgressCoordProp.Value);
            }

            MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Blend Rate", props.TintMapBlendRateProp.Value,
                props.TintMapBlendRateCoordProp.Value);
        }

        private void InternalDrawFlowMapProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawTexture(_editor, props.FlowMapProp.Value,
                props.FlowMapOffsetXCoordProp.Value,
                props.FlowMapOffsetYCoordProp.Value,
                props.FlowMapChannelsXProp.Value, props.FlowMapChannelsYProp.Value);
            MaterialEditorUtility.DrawPropertyAndCustomCoord(
                _editor,
                "Intensity",
                props.FlowIntensityProp.Value,
                props.FlowIntensityCoordProp.Value);
            MaterialEditorUtility.DrawEnumFlagsProperty<FlowMapTarget>(
                _editor, "Targets", props.FlowMapTargetProp.Value);
        }

        private void InternalDrawColorCorrectionProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawEnumProperty<ColorCorrectionMode>(_editor, "Mode",
                props.ColorCorrectionModeProp.Value);
            var colorCorrectionMode = (ColorCorrectionMode)props.ColorCorrectionModeProp.Value.floatValue;
            if (colorCorrectionMode == ColorCorrectionMode.GradientMap)
                MaterialEditorUtility.DrawTexture(_editor, props.GradientMapProp.Value, false);
        }

        private void InternalDrawAlphaTransitionProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawEnumProperty<AlphaTransitionMode>(_editor, "Mode",
                props.AlphaTransitionModeProp.Value);
            var mode = (AlphaTransitionMode)props.AlphaTransitionModeProp.Value.floatValue;
            if (mode != AlphaTransitionMode.None)
            {
                MaterialEditorUtility.DrawEnumProperty<AlphaTransitionMapMode>(_editor, "Map Mode",
                    props.AlphaTransitionMapModeProp.Value);
                var alphaTransitionMapMode = (AlphaTransitionMapMode)props.AlphaTransitionMapModeProp.Value.floatValue;
                MaterialProperty alphaTransitionMapProp;
                switch (alphaTransitionMapMode)
                {
                    case AlphaTransitionMapMode.SingleTexture:
                        alphaTransitionMapProp = props.AlphaTransitionMapProp.Value;
                        break;
                    case AlphaTransitionMapMode.FlipBook:
                        alphaTransitionMapProp = props.AlphaTransitionMap2DArrayProp.Value;
                        break;
                    case AlphaTransitionMapMode.FlipBookBlending:
                        alphaTransitionMapProp = props.AlphaTransitionMap3DProp.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTexture(_editor, alphaTransitionMapProp,
                        props.AlphaTransitionMapOffsetXCoordProp.Value, props.AlphaTransitionMapOffsetYCoordProp.Value,
                        props.AlphaTransitionMapChannelsXProp.Value, null);

                    if (changeCheckScope.changed)
                    {
                        if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBook
                            && props.AlphaTransitionMap2DArrayProp.Value.textureValue != null)
                        {
                            var tex2DArray = (Texture2DArray)props.AlphaTransitionMap2DArrayProp.Value.textureValue;
                            props.AlphaTransitionMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                        }

                        if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBookBlending
                            && props.AlphaTransitionMap3DProp.Value.textureValue != null)
                        {
                            var tex3D = (Texture3D)props.AlphaTransitionMap3DProp.Value.textureValue;
                            props.AlphaTransitionMapSliceCountProp.Value.floatValue = tex3D.depth;
                        }
                    }
                }

                if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBook
                    || alphaTransitionMapMode == AlphaTransitionMapMode.FlipBookBlending)
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Flip-Book Progress",
                        props.AlphaTransitionMapProgressProp.Value, props.AlphaTransitionMapProgressCoordProp.Value);

                MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Transition Progress",
                    props.AlphaTransitionProgressProp.Value, props.AlphaTransitionProgressCoordProp.Value);
                if (mode == AlphaTransitionMode.Dissolve)
                    _editor.ShaderProperty(props.DissolveSharpnessProp.Value, "Edge Sharpness");
            }
        }

        private void InternalDrawEmissionProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawEnumProperty<EmissionAreaType>(_editor, "Mode",
                props.EmissionAreaTypeProp.Value);
            var areaType = (EmissionAreaType)props.EmissionAreaTypeProp.Value.floatValue;
            if (areaType != EmissionAreaType.None)
            {
                if (areaType == EmissionAreaType.All)
                {
                    MaterialEditorUtility.DrawEnumProperty<EmissionColorTypeForAllArea>(_editor, "Color Type",
                        props.EmissionColorTypeProp.Value);
                }
                else if (areaType == EmissionAreaType.ByTexture)
                {
                    MaterialEditorUtility.DrawEnumProperty<EmissionMapMode>(_editor, "Map Mode",
                        props.EmissionMapModeProp.Value);
                    var emissionMapMode = (EmissionMapMode)props.EmissionMapModeProp.Value.floatValue;
                    MaterialProperty emissionMapProp;
                    switch (emissionMapMode)
                    {
                        case EmissionMapMode.SingleTexture:
                            emissionMapProp = props.EmissionMapProp.Value;
                            break;
                        case EmissionMapMode.FlipBook:
                            emissionMapProp = props.EmissionMap2DArrayProp.Value;
                            break;
                        case EmissionMapMode.FlipBookBlending:
                            emissionMapProp = props.EmissionMap3DProp.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        MaterialEditorUtility.DrawTexture(_editor, emissionMapProp,
                            props.EmissionMapOffsetXCoordProp.Value,
                            props.EmissionMapOffsetYCoordProp.Value, props.EmissionMapChannelsXProp.Value, null);

                        if (changeCheckScope.changed)
                        {
                            if (emissionMapMode == EmissionMapMode.FlipBook
                                && props.EmissionMap2DArrayProp.Value.textureValue != null)
                            {
                                var tex2DArray = (Texture2DArray)props.EmissionMap2DArrayProp.Value.textureValue;
                                props.EmissionMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                            }

                            if (emissionMapMode == EmissionMapMode.FlipBookBlending
                                && props.EmissionMap3DProp.Value.textureValue != null)
                            {
                                var tex3D = (Texture3D)props.EmissionMap3DProp.Value.textureValue;
                                props.EmissionMapSliceCountProp.Value.floatValue = tex3D.depth;
                            }
                        }
                    }

                    if (emissionMapMode == EmissionMapMode.FlipBook
                        || emissionMapMode == EmissionMapMode.FlipBookBlending)
                        MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Flip-Book Progress",
                            props.EmissionMapProgressProp.Value, props.EmissionMapProgressCoordProp.Value);

                    MaterialEditorUtility.DrawEnumProperty<EmissionColorType>(_editor, "Color Type",
                        props.EmissionColorTypeProp.Value);
                }
                else if (areaType == EmissionAreaType.Edge)
                {
                    MaterialEditorUtility.DrawEnumProperty<EmissionColorType>(_editor, "Color Type",
                        props.EmissionColorTypeProp.Value);
                }

                var colorType = (EmissionColorType)props.EmissionColorTypeProp.Value.floatValue;
                if (colorType == EmissionColorType.Color)
                    _editor.ShaderProperty(props.EmissionColorProp.Value, "Color");
                else if (colorType == EmissionColorType.GradiantMap)
                    MaterialEditorUtility.DrawTexture(_editor, props.EmissionColorRampProp.Value, false);

                if (areaType == EmissionAreaType.Edge)
                    MaterialEditorUtility.DrawToggleProperty(_editor, "Keep Edge Transparency",
                        props.KeepEdgeTransparencyProp.Value);

                using (var ccs2 = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Intensity",
                        props.EmissionIntensityProp.Value, props.EmissionIntensityCoordProp.Value);
                    if (ccs2.changed)
                        props.EmissionIntensityProp.Value.floatValue =
                            Mathf.Max(0, props.EmissionIntensityProp.Value.floatValue);
                }
            }
        }

        private void InternalDrawTransparencyProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawToggleProperty(_editor, "Rim", props.RimTransparencyEnabledProp.Value);
            if (props.RimTransparencyEnabledProp.Value.floatValue > 0.5f)
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Progress",
                        props.RimTransparencyProgressProp.Value, props.RimTransparencyProgressCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Sharpness",
                        props.RimTransparencySharpnessProp.Value, props.RimTransparencySharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawToggleProperty(_editor, "Inverse",
                        props.InverseRimTransparencyProp.Value);
                }

            MaterialEditorUtility.DrawToggleProperty(_editor, "Luminance",
                props.LuminanceTransparencyEnabledProp.Value);
            if (props.LuminanceTransparencyEnabledProp.Value.floatValue > 0.5f)
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Progress",
                        props.LuminanceTransparencyProgressProp.Value,
                        props.LuminanceTransparencyProgressCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(_editor, "Sharpness",
                        props.LuminanceTransparencySharpnessProp.Value,
                        props.LuminanceTransparencySharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawToggleProperty(_editor, "Inverse",
                        props.InverseLuminanceTransparencyProp.Value);
                }

            MaterialEditorUtility.DrawToggleProperty(_editor, "Soft Particles", props.SoftParticlesEnabledProp.Value);
            var softParticlesEnabled = props.SoftParticlesEnabledProp.Value.floatValue >= 0.5f;
            if (softParticlesEnabled)
                using (new EditorGUI.IndentLevelScope())
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    _editor.ShaderProperty(props.SoftParticlesIntensityProp.Value, "Intensity");
                    if (changeCheckScope.changed)
                        props.SoftParticlesIntensityProp.Value.floatValue =
                            Mathf.Max(0, props.SoftParticlesIntensityProp.Value.floatValue);
                }

            MaterialEditorUtility.DrawToggleProperty(_editor, "Depth Fade", props.DepthFadeEnabledProp.Value);
            var depthFadeEnabled = props.DepthFadeEnabledProp.Value.floatValue >= 0.5f;
            if (depthFadeEnabled)
                using (new EditorGUI.IndentLevelScope())
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTwoFloatProperties("Distance", props.DepthFadeNearProp.Value, "Near",
                        props.DepthFadeFarProp.Value, "Far", _editor);
                    _editor.ShaderProperty(props.DepthFadeWidthProp.Value, "Width");
                }
        }

        #endregion
    }
}