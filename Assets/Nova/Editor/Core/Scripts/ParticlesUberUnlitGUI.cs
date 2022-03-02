// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using UnityEngine;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     GUI for a material assigned the ParticlesUberUnlit Shader.
    /// </summary>
    internal sealed class ParticlesUberUnlitGUI : ParticlesGUI
    {
        private const int RenderPriorityMax = 50;
        private const int RenderPriorityMin = -RenderPriorityMax;

        protected override void SetupProperties(MaterialProperty[] properties)
        {
            // Render Settings
            _renderTypeProp.Setup(properties);
            _cutoffProp.Setup(properties);
            _transparentBlendModeProp.Setup(properties);
            _cullProp.Setup(properties);
            _queueOffsetProp.Setup(properties);
            _vertexAlphaModeProp.Setup(properties);
            _blendSrcProp.Setup(properties);
            _blendDstProp.Setup(properties);
            _zWriteProp.Setup(properties);

            // Base Color
            _baseMapModeProp.Setup(properties);
            _baseMapProp.Setup(properties);
            _baseMap2DArrayProp.Setup(properties);
            _baseMap3DProp.Setup(properties);
            _baseMapProgressProp.Setup(properties);
            _baseMapProgressCoordProp.Setup(properties);
            _baseMapSliceCountProp.Setup(properties);
            _baseColorProp.Setup(properties);
            _baseMapOffsetXCoordProp.Setup(properties);
            _baseMapOffsetYCoordProp.Setup(properties);
            _baseMapRotationProp.Setup(properties);
            _baseMapRotationCoordProp.Setup(properties);
            _baseMapRotationOffsetsProp.Setup(properties);
            _baseMapMirrorSamplingProp.Setup(properties);

            // Tint Color
            _tintAreaModeProp.Setup(properties);
            _tintColorModeProp.Setup(properties);
            _tintMapProp.Setup(properties);
            _tintMap3DProp.Setup(properties);
            _tintMap3DProgressProp.Setup(properties);
            _tintMap3DProgressCoordProp.Setup(properties);
            _tintMapSliceCountProp.Setup(properties);
            _tintMapBlendRateProp.Setup(properties);
            _tintMapBlendRateCoordProp.Setup(properties);
            _tintRimProgressProp.Setup(properties);
            _tintRimProgressCoordProp.Setup(properties);
            _tintRimSharpnessProp.Setup(properties);
            _tintRimSharpnessCoordProp.Setup(properties);
            _inverseTintRimProp.Setup(properties);

            // Flow Map
            _flowMapProp.Setup(properties);
            _flowMapOffsetXCoordProp.Setup(properties);
            _flowMapOffsetYCoordProp.Setup(properties);
            _flowIntensityProp.Setup(properties);
            _flowIntensityCoordProp.Setup(properties);
            _flowMapTargetProp.Setup(properties);

            // Color Correction
            _colorCorrectionModeProp.Setup(properties);
            _gradientMapProp.Setup(properties);

            // Alpha Transition
            _alphaTransitionModeProp.Setup(properties);
            _alphaTransitionMapModeProp.Setup(properties);
            _alphaTransitionMapProp.Setup(properties);
            _alphaTransitionMap2DArrayProp.Setup(properties);
            _alphaTransitionMap3DProp.Setup(properties);
            _alphaTransitionMapProgressProp.Setup(properties);
            _alphaTransitionMapProgressCoordProp.Setup(properties);
            _alphaTransitionMapOffsetXCoordProp.Setup(properties);
            _alphaTransitionMapOffsetYCoordProp.Setup(properties);
            _alphaTransitionMapSliceCountProp.Setup(properties);
            _alphaTransitionProgressProp.Setup(properties);
            _alphaTransitionProgressCoordProp.Setup(properties);
            _dissolveSharpnessProp.Setup(properties);

            // Emission
            _emissionAreaTypeProp.Setup(properties);
            _emissionMapModeProp.Setup(properties);
            _emissionMapProp.Setup(properties);
            _emissionMap2DArrayProp.Setup(properties);
            _emissionMap3DProp.Setup(properties);
            _emissionMapProgressProp.Setup(properties);
            _emissionMapProgressCoordProp.Setup(properties);
            _emissionMapSliceCountProp.Setup(properties);
            _emissionColorTypeProp.Setup(properties);
            _emissionColorProp.Setup(properties);
            _emissionColorRampProp.Setup(properties);
            _emissionIntensityProp.Setup(properties);
            _emissionIntensityCoordProp.Setup(properties);
            _keepEdgeTransparencyProp.Setup(properties);

            // Transparency
            _rimTransparencyEnabledProp.Setup(properties);
            _rimTransparencyProgressProp.Setup(properties);
            _rimTransparencyProgressCoordProp.Setup(properties);
            _rimTransparencySharpnessProp.Setup(properties);
            _rimTransparencySharpnessCoordProp.Setup(properties);
            _inverseRimTransparencyProp.Setup(properties);
            _luminanceTransparencyEnabledProp.Setup(properties);
            _luminanceTransparencyProgressProp.Setup(properties);
            _luminanceTransparencyProgressCoordProp.Setup(properties);
            _luminanceTransparencySharpnessProp.Setup(properties);
            _luminanceTransparencySharpnessCoordProp.Setup(properties);
            _inverseLuminanceTransparencyProp.Setup(properties);
            _softParticlesEnabledProp.Setup(properties);
            _softParticlesIntensityProp.Setup(properties);
            _depthFadeEnabledProp.Setup(properties);
            _depthFadeNearProp.Setup(properties);
            _depthFadeFarProp.Setup(properties);
            _depthFadeWidthProp.Setup(properties);
        }

        protected override void Initialize(MaterialEditor editor, MaterialProperty[] properties)
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

        protected override void DrawGUI(MaterialEditor editor, MaterialProperty[] properties)
        {
            var headerStyle = new GUIStyle(EditorStyles.foldoutHeader);
            headerStyle.fontStyle = FontStyle.Bold;

            using (var foldoutScope =
                new MaterialEditorUtility.FoldoutHeaderScope(RenderSettingsFoldout.Value, "Render Settings"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawRenderSettingsProperties(editor, properties);
                }

                RenderSettingsFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope = new MaterialEditorUtility.FoldoutHeaderScope(BaseMapFoldout.Value, "Base Map"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawBaseMapProperties(editor, properties);
                }

                BaseMapFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope = new MaterialEditorUtility.FoldoutHeaderScope(TintColorFoldout.Value, "Tint Color"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawTintColorProperties(editor, properties);
                }

                TintColorFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope = new MaterialEditorUtility.FoldoutHeaderScope(FlowMapFoldout.Value, "Flow Map"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawFlowMapProperties(editor, properties);
                }

                FlowMapFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope =
                new MaterialEditorUtility.FoldoutHeaderScope(ColorCorrectionFoldout.Value, "Color Correction"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawColorCorrectionProperties(editor, properties);
                }

                ColorCorrectionFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope =
                new MaterialEditorUtility.FoldoutHeaderScope(AlphaTransitionFoldout.Value, "Alpha Transition"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawAlphaTransitionProperties(editor, properties);
                }

                AlphaTransitionFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope = new MaterialEditorUtility.FoldoutHeaderScope(EmissionFoldout.Value, "Emission"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawEmissionProperties(editor, properties);
                }

                EmissionFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope =
                new MaterialEditorUtility.FoldoutHeaderScope(TransparencyFoldout.Value, "Transparency"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawTransparencyProperties(editor, properties);
                }

                TransparencyFoldout.Value = foldoutScope.Foldout;
            }
        }

        protected override void MaterialChanged(Material material)
        {
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialKeywords(material);
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialBlendMode(material);
        }

        private void DrawRenderSettingsProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<RenderType>(editor, "Render Type", _renderTypeProp.Value);
            var renderType = (RenderType)_renderTypeProp.Value.floatValue;
            if (renderType == RenderType.Cutout)
            {
                editor.ShaderProperty(_cutoffProp.Value, "Cutoff");
            }
            else if (renderType == RenderType.Transparent)
            {
                MaterialEditorUtility.DrawEnumProperty<TransparentBlendMode>(editor, "Blend Mode",
                    _transparentBlendModeProp.Value);
            }

            MaterialEditorUtility.DrawEnumProperty<RenderFace>(editor, "Render Face", _cullProp.Value);
            MaterialEditorUtility.DrawIntRangeProperty(editor, "Render Priority", _queueOffsetProp.Value,
                RenderPriorityMin, RenderPriorityMax);
            MaterialEditorUtility.DrawEnumProperty<VertexAlphaMode>(editor, "Vertex Alpha Mode",
                _vertexAlphaModeProp.Value);
        }

        private void DrawBaseMapProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<BaseMapMode>(editor, "Mode", _baseMapModeProp.Value);
            var baseMapMode = (BaseMapMode)_baseMapModeProp.Value.floatValue;
            MaterialProperty baseMapProp;
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    baseMapProp = _baseMapProp.Value;
                    break;
                case BaseMapMode.FlipBook:
                    baseMapProp = _baseMap2DArrayProp.Value;
                    break;
                case BaseMapMode.FlipBookBlending:
                    baseMapProp = _baseMap3DProp.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                MaterialEditorUtility.DrawTexture(editor, baseMapProp, _baseMapOffsetXCoordProp.Value,
                    _baseMapOffsetYCoordProp.Value);

                if (changeCheckScope.changed)
                {
                    if (baseMapMode == BaseMapMode.FlipBook && _baseMap2DArrayProp.Value.textureValue != null)
                    {
                        var tex2DArray = (Texture2DArray)_baseMap2DArrayProp.Value.textureValue;
                        _baseMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                    }

                    if (baseMapMode == BaseMapMode.FlipBookBlending && _baseMap3DProp.Value.textureValue != null)
                    {
                        var tex3D = (Texture3D)_baseMap3DProp.Value.textureValue;
                        _baseMapSliceCountProp.Value.floatValue = tex3D.depth;
                    }
                }
            }

            MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Rotation",
                _baseMapRotationProp.Value, _baseMapRotationCoordProp.Value);
            using (new EditorGUI.IndentLevelScope())
            {
                MaterialEditorUtility.DrawVector2Property(editor, "Offset", _baseMapRotationOffsetsProp.Value);
            }

            MaterialEditorUtility.DrawToggleProperty(editor, "Mirror Sampling", _baseMapMirrorSamplingProp.Value);

            if (baseMapMode == BaseMapMode.FlipBook || baseMapMode == BaseMapMode.FlipBookBlending)
            {
                MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Flip-Book Progress",
                    _baseMapProgressProp.Value, _baseMapProgressCoordProp.Value);
            }
        }

        private void DrawTintColorProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<TintAreaMode>(editor, "Mode", _tintAreaModeProp.Value);
            var tintAreaMode = (TintAreaMode)_tintAreaModeProp.Value.floatValue;
            if (tintAreaMode == TintAreaMode.None)
            {
                return;
            }

            if (tintAreaMode == TintAreaMode.Rim)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Progress", _tintRimProgressProp.Value,
                        _tintRimSharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Sharpness", _tintRimSharpnessProp.Value,
                        _tintRimSharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawToggleProperty(editor, "Inverse", _inverseTintRimProp.Value);
                }
            }

            MaterialEditorUtility.DrawEnumProperty<TintColorMode>(editor, "Color Mode", _tintColorModeProp.Value);
            var tintColorMode = (TintColorMode)_tintColorModeProp.Value.floatValue;
            if (tintColorMode == TintColorMode.SingleColor)
            {
                editor.ShaderProperty(_baseColorProp.Value, "Color");
            }
            else if (tintColorMode == TintColorMode.Texture2D)
            {
                MaterialEditorUtility.DrawTexture(editor, _tintMapProp.Value, true);
            }
            else if (tintColorMode == TintColorMode.Texture3D)
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTexture(editor, _tintMap3DProp.Value, true);

                    if (changeCheckScope.changed && _tintMap3DProp.Value.textureValue != null)
                    {
                        var tex3D = (Texture3D)_tintMap3DProp.Value.textureValue;
                        _tintMapSliceCountProp.Value.floatValue = tex3D.depth;
                    }
                }

                MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Progress",
                    _tintMap3DProgressProp.Value, _tintMap3DProgressCoordProp.Value);
            }

            MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Blend Rate", _tintMapBlendRateProp.Value,
                _tintMapBlendRateCoordProp.Value);
        }

        private void DrawFlowMapProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawTexture(editor, _flowMapProp.Value, _flowMapOffsetXCoordProp.Value,
                _flowMapOffsetYCoordProp.Value);
            MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Intensity", _flowIntensityProp.Value,
                _flowIntensityCoordProp.Value);
            MaterialEditorUtility.DrawEnumFlagsProperty<FlowMapTarget>(editor, "Targets", _flowMapTargetProp.Value);
        }

        private void DrawColorCorrectionProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<ColorCorrectionMode>(editor, "Mode",
                _colorCorrectionModeProp.Value);
            var colorCorrectionMode = (ColorCorrectionMode)_colorCorrectionModeProp.Value.floatValue;
            if (colorCorrectionMode == ColorCorrectionMode.GradientMap)
            {
                MaterialEditorUtility.DrawTexture(editor, _gradientMapProp.Value, false);
            }
        }

        private void DrawAlphaTransitionProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<AlphaTransitionMode>(editor, "Mode",
                _alphaTransitionModeProp.Value);
            var mode = (AlphaTransitionMode)_alphaTransitionModeProp.Value.floatValue;
            if (mode != AlphaTransitionMode.None)
            {
                MaterialEditorUtility.DrawEnumProperty<AlphaTransitionMapMode>(editor, "Map Mode",
                    _alphaTransitionMapModeProp.Value);
                var alphaTransitionMapMode = (AlphaTransitionMapMode)_alphaTransitionMapModeProp.Value.floatValue;
                MaterialProperty alphaTransitionMapProp;
                switch (alphaTransitionMapMode)
                {
                    case AlphaTransitionMapMode.SingleTexture:
                        alphaTransitionMapProp = _alphaTransitionMapProp.Value;
                        break;
                    case AlphaTransitionMapMode.FlipBook:
                        alphaTransitionMapProp = _alphaTransitionMap2DArrayProp.Value;
                        break;
                    case AlphaTransitionMapMode.FlipBookBlending:
                        alphaTransitionMapProp = _alphaTransitionMap3DProp.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTexture(editor, alphaTransitionMapProp,
                        _alphaTransitionMapOffsetXCoordProp.Value, _alphaTransitionMapOffsetYCoordProp.Value);

                    if (changeCheckScope.changed)
                    {
                        if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBook
                            && _alphaTransitionMap2DArrayProp.Value.textureValue != null)
                        {
                            var tex2DArray = (Texture2DArray)_alphaTransitionMap2DArrayProp.Value.textureValue;
                            _alphaTransitionMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                        }

                        if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBookBlending
                            && _alphaTransitionMap3DProp.Value.textureValue != null)
                        {
                            var tex3D = (Texture3D)_alphaTransitionMap3DProp.Value.textureValue;
                            _alphaTransitionMapSliceCountProp.Value.floatValue = tex3D.depth;
                        }
                    }
                }

                if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBook
                    || alphaTransitionMapMode == AlphaTransitionMapMode.FlipBookBlending)
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Flip-Book Progress",
                        _alphaTransitionMapProgressProp.Value, _alphaTransitionMapProgressCoordProp.Value);
                }

                MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Transition Progress",
                    _alphaTransitionProgressProp.Value, _alphaTransitionProgressCoordProp.Value);
                if (mode == AlphaTransitionMode.Dissolve)
                {
                    editor.ShaderProperty(_dissolveSharpnessProp.Value, "Edge Sharpness");
                }
            }
        }

        private void DrawEmissionProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<EmissionAreaType>(editor, "Mode",
                _emissionAreaTypeProp.Value);
            var areaType = (EmissionAreaType)_emissionAreaTypeProp.Value.floatValue;
            if (areaType != EmissionAreaType.None)
            {
                if (areaType == EmissionAreaType.All)
                {
                    MaterialEditorUtility.DrawEnumProperty<EmissionColorTypeForAllArea>(editor, "Color Type",
                        _emissionColorTypeProp.Value);
                }
                else if (areaType == EmissionAreaType.ByTexture)
                {
                    MaterialEditorUtility.DrawEnumProperty<EmissionMapMode>(editor, "Map Mode",
                        _emissionMapModeProp.Value);
                    var emissionMapMode = (EmissionMapMode)_emissionMapModeProp.Value.floatValue;
                    MaterialProperty emissionMapProp;
                    switch (emissionMapMode)
                    {
                        case EmissionMapMode.SingleTexture:
                            emissionMapProp = _emissionMapProp.Value;
                            break;
                        case EmissionMapMode.FlipBook:
                            emissionMapProp = _emissionMap2DArrayProp.Value;
                            break;
                        case EmissionMapMode.FlipBookBlending:
                            emissionMapProp = _emissionMap3DProp.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        MaterialEditorUtility.DrawTexture(editor, emissionMapProp, true);

                        if (changeCheckScope.changed)
                        {
                            if (emissionMapMode == EmissionMapMode.FlipBook
                                && _emissionMap2DArrayProp.Value.textureValue != null)
                            {
                                var tex2DArray = (Texture2DArray)_emissionMap2DArrayProp.Value.textureValue;
                                _emissionMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                            }

                            if (emissionMapMode == EmissionMapMode.FlipBookBlending
                                && _emissionMap3DProp.Value.textureValue != null)
                            {
                                var tex3D = (Texture3D)_emissionMap3DProp.Value.textureValue;
                                _emissionMapSliceCountProp.Value.floatValue = tex3D.depth;
                            }
                        }
                    }

                    if (emissionMapMode == EmissionMapMode.FlipBook
                        || emissionMapMode == EmissionMapMode.FlipBookBlending)
                    {
                        MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Flip-Book Progress",
                            _emissionMapProgressProp.Value, _emissionMapProgressCoordProp.Value);
                    }

                    MaterialEditorUtility.DrawEnumProperty<EmissionColorType>(editor, "Color Type",
                        _emissionColorTypeProp.Value);
                }
                else if (areaType == EmissionAreaType.Edge)
                {
                    MaterialEditorUtility.DrawEnumProperty<EmissionColorType>(editor, "Color Type",
                        _emissionColorTypeProp.Value);
                }

                var colorType = (EmissionColorType)_emissionColorTypeProp.Value.floatValue;
                if (colorType == EmissionColorType.Color)
                {
                    editor.ShaderProperty(_emissionColorProp.Value, "Color");
                }
                else if (colorType == EmissionColorType.GradiantMap)
                {
                    MaterialEditorUtility.DrawTexture(editor, _emissionColorRampProp.Value, false);
                }

                if (areaType == EmissionAreaType.Edge)
                {
                    MaterialEditorUtility.DrawToggleProperty(editor, "Keep Edge Transparency",
                        _keepEdgeTransparencyProp.Value);
                }

                using (var ccs2 = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Intensity",
                        _emissionIntensityProp.Value, _emissionIntensityCoordProp.Value);
                    if (ccs2.changed)
                    {
                        _emissionIntensityProp.Value.floatValue =
                            Mathf.Max(0, _emissionIntensityProp.Value.floatValue);
                    }
                }
            }
        }

        private void DrawTransparencyProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawToggleProperty(editor, "Rim", _rimTransparencyEnabledProp.Value);
            if (_rimTransparencyEnabledProp.Value.floatValue > 0.5f)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Progress",
                        _rimTransparencyProgressProp.Value, _rimTransparencyProgressCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Sharpness",
                        _rimTransparencySharpnessProp.Value, _rimTransparencySharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawToggleProperty(editor, "Inverse", _inverseRimTransparencyProp.Value);
                }
            }

            MaterialEditorUtility.DrawToggleProperty(editor, "Luminance", _luminanceTransparencyEnabledProp.Value);
            if (_luminanceTransparencyEnabledProp.Value.floatValue > 0.5f)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Progress",
                        _luminanceTransparencyProgressProp.Value,
                        _luminanceTransparencyProgressCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Sharpness",
                        _luminanceTransparencySharpnessProp.Value,
                        _luminanceTransparencySharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawToggleProperty(editor, "Inverse", _inverseLuminanceTransparencyProp.Value);
                }
            }

            MaterialEditorUtility.DrawToggleProperty(editor, "Soft Particles", _softParticlesEnabledProp.Value);
            var softParticlesEnabled = _softParticlesEnabledProp.Value.floatValue >= 0.5f;
            if (softParticlesEnabled)
            {
                using (new EditorGUI.IndentLevelScope())
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    editor.ShaderProperty(_softParticlesIntensityProp.Value, "Intensity");
                    if (changeCheckScope.changed)
                    {
                        _softParticlesIntensityProp.Value.floatValue =
                            Mathf.Max(0, _softParticlesIntensityProp.Value.floatValue);
                    }
                }
            }

            MaterialEditorUtility.DrawToggleProperty(editor, "Depth Fade", _depthFadeEnabledProp.Value);
            var depthFadeEnabled = _depthFadeEnabledProp.Value.floatValue >= 0.5f;
            if (depthFadeEnabled)
            {
                using (new EditorGUI.IndentLevelScope())
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTwoFloatProperties("Distance", _depthFadeNearProp.Value, "Near",
                        _depthFadeFarProp.Value, "Far", editor);
                    editor.ShaderProperty(_depthFadeWidthProp.Value, "Width");
                }
            }
        }

        #region Foldout Properties

        private BoolEditorPrefsProperty AlphaTransitionFoldout { get; set; }
        private BoolEditorPrefsProperty BaseMapFoldout { get; set; }
        private BoolEditorPrefsProperty TintColorFoldout { get; set; }
        private BoolEditorPrefsProperty FlowMapFoldout { get; set; }
        private BoolEditorPrefsProperty ColorCorrectionFoldout { get; set; }
        private BoolEditorPrefsProperty EmissionFoldout { get; set; }
        private BoolEditorPrefsProperty RenderSettingsFoldout { get; set; }
        private BoolEditorPrefsProperty TransparencyFoldout { get; set; }

        #endregion

        #region Render Settings Material Properties

        private readonly Property _renderTypeProp = new Property(PropertyNames.RenderType);
        private readonly Property _cutoffProp = new Property(PropertyNames.Cutoff);
        private readonly Property _transparentBlendModeProp = new Property(PropertyNames.TransparentBlendMode);
        private readonly Property _cullProp = new Property(PropertyNames.Cull);
        private readonly Property _queueOffsetProp = new Property(PropertyNames.QueueOffset);
        private readonly Property _vertexAlphaModeProp = new Property(PropertyNames.VertexAlphaMode);
        private readonly Property _blendDstProp = new Property(PropertyNames.BlendDst);
        private readonly Property _blendSrcProp = new Property(PropertyNames.BlendSrc);
        private readonly Property _zWriteProp = new Property(PropertyNames.ZWrite);

        #endregion

        #region Base Map Material Properties

        private readonly Property _baseMapModeProp = new Property(PropertyNames.BaseMapMode);
        private readonly Property _baseMapProp = new Property(PropertyNames.BaseMap);
        private readonly Property _baseMap2DArrayProp = new Property(PropertyNames.BaseMap2DArray);
        private readonly Property _baseMap3DProp = new Property(PropertyNames.BaseMap3D);
        private readonly Property _baseMapProgressProp = new Property(PropertyNames.BaseMapProgress);
        private readonly Property _baseMapProgressCoordProp = new Property(PropertyNames.BaseMapProgressCoord);
        private readonly Property _baseMapSliceCountProp = new Property(PropertyNames.BaseMapSliceCount);
        private readonly Property _baseColorProp = new Property(PropertyNames.TintColor);
        private readonly Property _baseMapOffsetXCoordProp = new Property(PropertyNames.BaseMapOffsetXCoord);
        private readonly Property _baseMapOffsetYCoordProp = new Property(PropertyNames.BaseMapOffsetYCoord);
        private readonly Property _baseMapRotationProp = new Property(PropertyNames.BaseMapRotation);
        private readonly Property _baseMapRotationCoordProp = new Property(PropertyNames.BaseMapRotationCoord);
        private readonly Property _baseMapRotationOffsetsProp = new Property(PropertyNames.BaseMapRotationOffsets);
        private readonly Property _baseMapMirrorSamplingProp = new Property(PropertyNames.BaseMapMirrorSampling);

        #endregion

        #region Tint Color Material Properties

        private readonly Property _tintAreaModeProp = new Property(PropertyNames.TintAreaMode);
        private readonly Property _tintColorModeProp = new Property(PropertyNames.TintColorMode);
        private readonly Property _tintMapProp = new Property(PropertyNames.TintMap);
        private readonly Property _tintMap3DProp = new Property(PropertyNames.TintMap3D);
        private readonly Property _tintMap3DProgressProp = new Property(PropertyNames.TintMap3DProgress);
        private readonly Property _tintMap3DProgressCoordProp = new Property(PropertyNames.TintMap3DProgressCoord);
        private readonly Property _tintMapSliceCountProp = new Property(PropertyNames.TintMapSliceCount);
        private readonly Property _tintMapBlendRateProp = new Property(PropertyNames.TintMapBlendRate);
        private readonly Property _tintMapBlendRateCoordProp = new Property(PropertyNames.TintMapBlendRateCoord);
        private readonly Property _tintRimProgressProp = new Property(PropertyNames.TintRimProgress);
        private readonly Property _tintRimProgressCoordProp = new Property(PropertyNames.TintRimProgressCoord);
        private readonly Property _tintRimSharpnessProp = new Property(PropertyNames.TintRimSharpness);
        private readonly Property _tintRimSharpnessCoordProp = new Property(PropertyNames.TintRimSharpnessCoord);
        private readonly Property _inverseTintRimProp = new Property(PropertyNames.InverseTintRim);

        #endregion

        #region Flow Map Material Properties

        private readonly Property _flowMapProp = new Property(PropertyNames.FlowMap);
        private readonly Property _flowMapOffsetXCoordProp = new Property(PropertyNames.FlowMapOffsetXCoord);
        private readonly Property _flowMapOffsetYCoordProp = new Property(PropertyNames.FlowMapOffsetYCoord);
        private readonly Property _flowIntensityProp = new Property(PropertyNames.FlowIntensity);
        private readonly Property _flowIntensityCoordProp = new Property(PropertyNames.FlowIntensityCoord);
        private readonly Property _flowMapTargetProp = new Property(PropertyNames.FlowMapTarget);

        #endregion

        #region Color Correction Matrial Propreties

        private readonly Property _colorCorrectionModeProp = new Property(PropertyNames.ColorCorrectionMode);
        private readonly Property _gradientMapProp = new Property(PropertyNames.GradientMap);

        #endregion

        #region Alpha Transition Material Properties

        private readonly Property _alphaTransitionModeProp = new Property(PropertyNames.AlphaTransitionMode);
        private readonly Property _alphaTransitionMapModeProp = new Property(PropertyNames.AlphaTransitionMapMode);
        private readonly Property _alphaTransitionMapProp = new Property(PropertyNames.AlphaTransitionMap);

        private readonly Property _alphaTransitionMap2DArrayProp =
            new Property(PropertyNames.AlphaTransitionMap2DArray);

        private readonly Property _alphaTransitionMap3DProp = new Property(PropertyNames.AlphaTransitionMap3D);

        private readonly Property _alphaTransitionMapProgressProp =
            new Property(PropertyNames.AlphaTransitionMapProgress);

        private readonly Property _alphaTransitionMapProgressCoordProp =
            new Property(PropertyNames.AlphaTransitionMapProgressCoord);

        private readonly Property _alphaTransitionMapOffsetXCoordProp =
            new Property(PropertyNames.AlphaTransitionMapOffsetXCoord);

        private readonly Property _alphaTransitionMapOffsetYCoordProp =
            new Property(PropertyNames.AlphaTransitionMapOffsetYCoord);

        private readonly Property _alphaTransitionMapSliceCountProp =
            new Property(PropertyNames.AlphaTransitionMapSliceCount);

        private readonly Property _alphaTransitionProgressProp = new Property(PropertyNames.AlphaTransitionProgress);

        private readonly Property _alphaTransitionProgressCoordProp =
            new Property(PropertyNames.AlphaTransitionProgressCoord);

        private readonly Property _dissolveSharpnessProp = new Property(PropertyNames.DissolveSharpness);

        #endregion

        #region Emission Material Properties

        private readonly Property _emissionAreaTypeProp = new Property(PropertyNames.EmissionAreaType);
        private readonly Property _emissionMapModeProp = new Property(PropertyNames.EmissionMapMode);
        private readonly Property _emissionMapProp = new Property(PropertyNames.EmissionMap);
        private readonly Property _emissionMap2DArrayProp = new Property(PropertyNames.EmissionMap2DArray);
        private readonly Property _emissionMap3DProp = new Property(PropertyNames.EmissionMap3D);
        private readonly Property _emissionMapProgressProp = new Property(PropertyNames.EmissionMapProgress);
        private readonly Property _emissionMapProgressCoordProp = new Property(PropertyNames.EmissionMapProgressCoord);
        private readonly Property _emissionMapSliceCountProp = new Property(PropertyNames.EmissionMapSliceCount);
        private readonly Property _emissionColorTypeProp = new Property(PropertyNames.EmissionColorType);
        private readonly Property _emissionColorProp = new Property(PropertyNames.EmissionColor);
        private readonly Property _emissionColorRampProp = new Property(PropertyNames.EmissionColorRamp);
        private readonly Property _emissionIntensityProp = new Property(PropertyNames.EmissionIntensity);
        private readonly Property _emissionIntensityCoordProp = new Property(PropertyNames.EmissionIntensityCoord);
        private readonly Property _keepEdgeTransparencyProp = new Property(PropertyNames.KeepEdgeTransparency);

        #endregion

        #region Transparency Material Properties

        private readonly Property _rimTransparencyEnabledProp = new Property(PropertyNames.RimTransparencyEnabled);
        private readonly Property _rimTransparencyProgressProp = new Property(PropertyNames.RimTransparencyProgress);

        private readonly Property _rimTransparencyProgressCoordProp =
            new Property(PropertyNames.RimTransparencyProgressCoord);

        private readonly Property _rimTransparencySharpnessProp = new Property(PropertyNames.RimTransparencySharpness);

        private readonly Property _rimTransparencySharpnessCoordProp =
            new Property(PropertyNames.RimTransparencySharpnessCoord);

        private readonly Property _inverseRimTransparencyProp = new Property(PropertyNames.InverseRimTransparency);

        private readonly Property _luminanceTransparencyEnabledProp =
            new Property(PropertyNames.LuminanceTransparencyEnabled);

        private readonly Property _luminanceTransparencyProgressProp =
            new Property(PropertyNames.LuminanceTransparencyProgress);

        private readonly Property _luminanceTransparencyProgressCoordProp =
            new Property(PropertyNames.LuminanceTransparencyProgressCoord);

        private readonly Property _luminanceTransparencySharpnessProp =
            new Property(PropertyNames.LuminanceTransparencySharpness);

        private readonly Property _luminanceTransparencySharpnessCoordProp =
            new Property(PropertyNames.LuminanceTransparencySharpnessCoord);

        private readonly Property _inverseLuminanceTransparencyProp =
            new Property(PropertyNames.InverseLuminanceTransparency);

        private readonly Property _softParticlesEnabledProp = new Property(PropertyNames.SoftParticlesEnabled);
        private readonly Property _softParticlesIntensityProp = new Property(PropertyNames.SoftParticlesIntensity);
        private readonly Property _depthFadeEnabledProp = new Property(PropertyNames.DepthFadeEnabled);
        private readonly Property _depthFadeNearProp = new Property(PropertyNames.DepthFadeNear);
        private readonly Property _depthFadeFarProp = new Property(PropertyNames.DepthFadeFar);
        private readonly Property _depthFadeWidthProp = new Property(PropertyNames.DepthFadeWidth);

        #endregion
    }
}