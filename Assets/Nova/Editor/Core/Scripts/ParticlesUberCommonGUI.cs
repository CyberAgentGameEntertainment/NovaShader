// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     Common GUI for ParticleUberUnlit, ParticleUberLit classes.
    /// </summary>
    internal class ParticlesUberCommonGUI<TCustomCoord> where TCustomCoord : Enum
    {
        public ParticlesUberCommonGUI(MaterialEditor editor)
        {
            var material = editor.target as Material;
            _renderersUsingThisMaterial = RendererErrorHandler.FindAllRenderersWithMaterial(material);
        }

        public void Setup(MaterialEditor editor, ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            _editor = editor;
            _commonMaterialProperties = commonMaterialProperties;
            RendererErrorHandler.SetupCorrectVertexStreams(_editor.target as Material, out _correctVertexStreams,
                out _correctVertexStreamsInstanced);
        }

        public void DrawRenderSettingsProperties(Action drawPropertiesFunc)
        {
            DrawProperties(_commonMaterialProperties.RenderSettingsFoldout, "Render Settings",
                drawPropertiesFunc ?? DrawRenderSettingsPropertiesCore);
        }

        public void DrawBaseMapProperties()
        {
            DrawProperties(_commonMaterialProperties.BaseMapFoldout,
                "Base Map", InternalDrawBaseMapProperties);
        }

        public void DrawParallaxMapProperties()
        {
            DrawProperties(_commonMaterialProperties.ParallaxMapFoldout,
                "Parallax Map", InternalDrawParallaxMapsProperties);
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

        public void DrawVertexDeformationProperties()
        {
            DrawProperties(_commonMaterialProperties.VertexDeformationFoldout,
                "Vertex Deformation", InternalDrawVertexDeformationMapProperties);
        }

        public void DrawShadowCasterProperties()
        {
            DrawProperties(_commonMaterialProperties.ShadowCasterFoldout,
                "Shadow Caster", InternalDrawShadowCasterProperties);
        }

        public void DrawFixNowButton()
        {
            if (!RendererErrorHandler.CheckError(_renderersUsingThisMaterial, _editor.target as Material,
                    _correctVertexStreams,
                    _correctVertexStreamsInstanced))
                return;

            EditorGUILayout.HelpBox(
                "Some particle System Renderers are using this material with incorrect Vertex Streams or Trail Vertex Streams.\n" +
                "Recommend that you press the Fix Now button to correct the error."
                , MessageType.Error, true);
            if (GUILayout.Button(StreamApplyToAllSystemsText, EditorStyles.miniButton,
                    GUILayout.ExpandWidth(true)))
            {
                Undo.RecordObjects(
                    _renderersUsingThisMaterial.Where(r => r != null).ToArray(),
                    "Apply custom vertex streams from material");
                RendererErrorHandler.FixError(_renderersUsingThisMaterial, _editor.target as Material,
                    _correctVertexStreams,
                    _correctVertexStreamsInstanced);
            }
        }

        public void DrawErrorMessage()
        {
            if (string.IsNullOrEmpty(_errorMessage)) return;
            EditorGUILayout.HelpBox(
                _errorMessage, MessageType.Error, true);
            _errorMessage = "";
        }

        public void DrawProperties(BoolEditorPrefsProperty foldout, string categoryName, Action internalDrawFunction)
        {
            using var foldoutScope = new MaterialEditorUtility.FoldoutHeaderScope(foldout.Value, categoryName);
            if (foldoutScope.Foldout) internalDrawFunction();

            foldout.Value = foldoutScope.Foldout;
        }

        public void DrawRenderSettingsPropertiesCore()
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
            MaterialEditorUtility.DrawEnumProperty<ZWriteOverride>(_editor, "ZWrite",
                props.ZWriteOverrideProp.Value);
            MaterialEditorUtility.DrawEnumProperty<ZTest>(_editor, "ZTest", props.ZTestProp.Value);
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
                MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, baseMapMaterialProp,
                    props.BaseMapOffsetXCoordProp.Value,
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

            MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Rotation",
                props.BaseMapRotationProp.Value, props.BaseMapRotationCoordProp.Value);
            using (new EditorGUI.IndentLevelScope())
            {
                MaterialEditorUtility.DrawVector2Property(_editor, "Offset", props.BaseMapRotationOffsetsProp.Value);
            }

            MaterialEditorUtility.DrawToggleProperty(_editor, "Mirror Sampling",
                props.BaseMapMirrorSamplingProp.Value);

            if (baseMapMode == BaseMapMode.FlipBook || baseMapMode == BaseMapMode.FlipBookBlending)
            {
                MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Flip-Book Progress",
                    props.BaseMapProgressProp.Value, props.BaseMapProgressCoordProp.Value);

                MaterialEditorUtility.DrawToggleProperty(_editor, "Random Row Selection",
                    props.BaseMapRandomRowSelectionEnabledProp.Value);

                var randomRowEnabled = props.BaseMapRandomRowSelectionEnabledProp.Value.floatValue > 0.5f;

                if (randomRowEnabled)
                    using (new EditorGUI.IndentLevelScope())
                    {
                        _editor.FloatProperty(props.BaseMapRowCountProp.Value, "Row Count");

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel("Random Coord");
                            var coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord),
                                Convert.ToInt32(props.BaseMapRandomRowCoordProp.Value.floatValue));
                            if (!Enum.IsDefined(typeof(TCustomCoord), coord))
                                coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord), 0);

                            using (var ccs = new EditorGUI.ChangeCheckScope())
                            {
                                EditorGUI.showMixedValue = props.BaseMapRandomRowCoordProp.Value.hasMixedValue;
                                coord = (TCustomCoord)EditorGUILayout.EnumPopup(coord);
                                EditorGUI.showMixedValue = false;

                                if (ccs.changed)
                                {
                                    _editor.RegisterPropertyChangeUndo(props.BaseMapRandomRowCoordProp.Value.name);
                                    props.BaseMapRandomRowCoordProp.Value.floatValue = Convert.ToInt32(coord);
                                }
                            }
                        }

                        // Validation and help message
                        var sliceCount = props.BaseMapSliceCountProp.Value.floatValue;
                        var rowCount = props.BaseMapRowCountProp.Value.floatValue;

                        if (rowCount <= 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Row Count must be greater than 0. Setting to 1 will disable random row selection.",
                                MessageType.Warning);
                        }
                        else if (rowCount > sliceCount && sliceCount > 0)
                        {
                            EditorGUILayout.HelpBox(
                                $"Row Count ({rowCount}) cannot be greater than Slice Count ({sliceCount}). Reduce Row Count or increase Slice Count.",
                                MessageType.Error);
                        }
                        else if (sliceCount > 0)
                        {
                            // Convert to integers for accurate division check
                            var sliceCountInt = Mathf.FloorToInt(sliceCount);
                            var rowCountInt = Mathf.FloorToInt(rowCount);

                            if (sliceCountInt % rowCountInt != 0)
                            {
                                var framesPerRow = sliceCountInt / rowCountInt;
                                var unusedSlices = sliceCountInt - rowCountInt * framesPerRow;
                                EditorGUILayout.HelpBox(
                                    $"Row Count ({rowCountInt}) does not divide Slice Count ({sliceCountInt}) evenly. Each row will have {framesPerRow} frames, with {unusedSlices} unused slices.",
                                    MessageType.Warning);
                            }
                        }

                        EditorGUILayout.HelpBox(
                            "Setup:\n" +
                            "• Row Count: Set to number of rows in your texture (e.g., 4×4 texture = 4 rows)\n" +
                            "• Random Coord: Select a Custom Coord channel for random values\n" +
                            "  - Configure Particle System's Custom Data as Random Between Two Constants (0 to Row Count)",
                            MessageType.Info);
                    }
            }
        }

        private void InternalDrawParallaxMapsProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawEnumProperty<ParallaxMapMode>(_editor, "TextureMode",
                props.ParallaxMapModeProp.Value);
            var parallaxMapMode = (ParallaxMapMode)props.ParallaxMapModeProp.Value.floatValue;
            MaterialProperty textureProp;
            switch (parallaxMapMode)
            {
                case ParallaxMapMode.SingleTexture:
                    textureProp = props.ParallaxMapProp.Value;
                    break;
                case ParallaxMapMode.FlipBook:
                    textureProp = props.ParallaxMap2DArrayProp.Value;
                    break;
                case ParallaxMapMode.FlipBookBlending:
                    textureProp = props.ParallaxMap3DProp.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                MaterialEditorUtility.DrawTexture<TCustomCoord>(
                    _editor,
                    textureProp,
                    props.ParallaxMapOffsetXCoordProp.Value,
                    props.ParallaxMapOffsetYCoordProp.Value,
                    props.ParallaxMapChannel.Value,
                    null
                );

                if (changeCheckScope.changed)
                {
                    if (parallaxMapMode == ParallaxMapMode.FlipBook &&
                        props.ParallaxMap2DArrayProp.Value.textureValue != null)
                    {
                        var tex2DArray = (Texture2DArray)props.ParallaxMap2DArrayProp.Value.textureValue;
                        props.ParallaxMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                    }

                    if (parallaxMapMode == ParallaxMapMode.FlipBookBlending &&
                        props.ParallaxMap3DProp.Value.textureValue != null)
                    {
                        var tex3D = (Texture3D)props.ParallaxMap3DProp.Value.textureValue;
                        props.ParallaxMapSliceCountProp.Value.floatValue = tex3D.depth;
                    }
                }
            }

            if (parallaxMapMode > ParallaxMapMode.SingleTexture)
            {
                MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(
                    _editor,
                    "Flip-Book Progress",
                    props.ParallaxMapProgressProp.Value,
                    props.ParallaxMapProgressCoordProp.Value);

                MaterialEditorUtility.DrawToggleProperty(_editor, "Random Row Selection",
                    props.ParallaxMapRandomRowSelectionEnabledProp.Value);

                var randomRowEnabled = props.ParallaxMapRandomRowSelectionEnabledProp.Value.floatValue > 0.5f;

                if (randomRowEnabled)
                    using (new EditorGUI.IndentLevelScope())
                    {
                        _editor.FloatProperty(props.ParallaxMapRowCountProp.Value, "Row Count");

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel("Random Coord");
                            var coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord),
                                Convert.ToInt32(props.ParallaxMapRandomRowCoordProp.Value.floatValue));
                            if (!Enum.IsDefined(typeof(TCustomCoord), coord))
                                coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord), 0);

                            using (var ccs = new EditorGUI.ChangeCheckScope())
                            {
                                EditorGUI.showMixedValue = props.ParallaxMapRandomRowCoordProp.Value.hasMixedValue;
                                coord = (TCustomCoord)EditorGUILayout.EnumPopup(coord);
                                EditorGUI.showMixedValue = false;

                                if (ccs.changed)
                                {
                                    _editor.RegisterPropertyChangeUndo(props.ParallaxMapRandomRowCoordProp.Value.name);
                                    props.ParallaxMapRandomRowCoordProp.Value.floatValue = Convert.ToInt32(coord);
                                }
                            }
                        }

                        // Validation and help message
                        var sliceCount = props.ParallaxMapSliceCountProp.Value.floatValue;
                        var rowCount = props.ParallaxMapRowCountProp.Value.floatValue;

                        if (rowCount <= 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Row Count must be greater than 0. Setting to 1 will disable random row selection.",
                                MessageType.Warning);
                        }
                        else if (rowCount > sliceCount && sliceCount > 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Row Count cannot exceed Slice Count. Adjust Row Count accordingly.",
                                MessageType.Warning);
                        }

                        // Auto-fix
                        if (props.ParallaxMapRowCountProp.Value.floatValue < 1)
                        {
                            props.ParallaxMapRowCountProp.Value.floatValue = 1;
                        }
                    }
            }

            MaterialEditorUtility.DrawFloatRangeProperty(
                _editor,
                "Strength",
                props.ParallaxStrengthProp.Value,
                props.ParallaxStrengthProp.Value.rangeLimits.x,
                props.ParallaxStrengthProp.Value.rangeLimits.y);

            MaterialEditorUtility.DrawEnumFlagsProperty<ParallaxMapTarget>(
                _editor,
                "Target",
                props.ParallaxMapTargetProp.Value);
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
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(
                        _editor, "Progress", props.TintRimProgressProp.Value,
                        props.TintRimProgressCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Sharpness",
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
            else if (tintColorMode == TintColorMode.SingleTexture)
            {
                MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, props.TintMapProp.Value,
                    props.TintMapOffsetXCoordProp.Value, props.TintMapOffsetYCoordProp.Value,
                    null, null);
            }
            else if (tintColorMode == TintColorMode.FlipBookBlending)
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, props.TintMap3DProp.Value,
                        props.TintMapOffsetXCoordProp.Value, props.TintMapOffsetYCoordProp.Value,
                        null, null);

                    if (changeCheckScope.changed && props.TintMap3DProp.Value.textureValue != null)
                    {
                        var tex3D = (Texture3D)props.TintMap3DProp.Value.textureValue;
                        props.TintMapSliceCountProp.Value.floatValue = tex3D.depth;
                    }
                }

                MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Flip-Book Progress",
                    props.TintMap3DProgressProp.Value, props.TintMap3DProgressCoordProp.Value);

                MaterialEditorUtility.DrawToggleProperty(_editor, "Random Row Selection",
                    props.TintMapRandomRowSelectionEnabledProp.Value);

                var randomRowEnabled = props.TintMapRandomRowSelectionEnabledProp.Value.floatValue > 0.5f;

                if (randomRowEnabled)
                    using (new EditorGUI.IndentLevelScope())
                    {
                        _editor.FloatProperty(props.TintMapRowCountProp.Value, "Row Count");

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel("Random Coord");
                            var coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord),
                                Convert.ToInt32(props.TintMapRandomRowCoordProp.Value.floatValue));
                            if (!Enum.IsDefined(typeof(TCustomCoord), coord))
                                coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord), 0);

                            using (var ccs = new EditorGUI.ChangeCheckScope())
                            {
                                EditorGUI.showMixedValue = props.TintMapRandomRowCoordProp.Value.hasMixedValue;
                                coord = (TCustomCoord)EditorGUILayout.EnumPopup(coord);
                                EditorGUI.showMixedValue = false;

                                if (ccs.changed)
                                {
                                    _editor.RegisterPropertyChangeUndo(props.TintMapRandomRowCoordProp.Value.name);
                                    props.TintMapRandomRowCoordProp.Value.floatValue = Convert.ToInt32(coord);
                                }
                            }
                        }

                        // Validation and help message
                        var sliceCount = props.TintMapSliceCountProp.Value.floatValue;
                        var rowCount = props.TintMapRowCountProp.Value.floatValue;

                        if (rowCount <= 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Row Count must be greater than 0. Setting to 1 will disable random row selection.",
                                MessageType.Warning);
                        }
                        else if (rowCount > sliceCount && sliceCount > 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Row Count cannot exceed Slice Count. Adjust Row Count accordingly.",
                                MessageType.Warning);
                        }

                        // Auto-fix
                        if (props.TintMapRowCountProp.Value.floatValue < 1)
                        {
                            props.TintMapRowCountProp.Value.floatValue = 1;
                        }
                    }
            }
            else if (tintColorMode == TintColorMode.FlipBook)
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, props.TintMap2DArrayProp.Value,
                        props.TintMapOffsetXCoordProp.Value, props.TintMapOffsetYCoordProp.Value,
                        null, null);

                    if (changeCheckScope.changed && props.TintMap2DArrayProp.Value.textureValue != null)
                    {
                        var tex2DArray = (Texture2DArray)props.TintMap2DArrayProp.Value.textureValue;
                        props.TintMapSliceCountProp.Value.floatValue = tex2DArray.depth;
                    }
                }

                MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Flip-Book Progress",
                    props.TintMapProgressProp.Value, props.TintMapProgressCoordProp.Value);

                MaterialEditorUtility.DrawToggleProperty(_editor, "Random Row Selection",
                    props.TintMapRandomRowSelectionEnabledProp.Value);

                var randomRowEnabled = props.TintMapRandomRowSelectionEnabledProp.Value.floatValue > 0.5f;

                if (randomRowEnabled)
                    using (new EditorGUI.IndentLevelScope())
                    {
                        _editor.FloatProperty(props.TintMapRowCountProp.Value, "Row Count");

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel("Random Coord");
                            var coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord),
                                Convert.ToInt32(props.TintMapRandomRowCoordProp.Value.floatValue));
                            if (!Enum.IsDefined(typeof(TCustomCoord), coord))
                                coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord), 0);

                            using (var ccs = new EditorGUI.ChangeCheckScope())
                            {
                                EditorGUI.showMixedValue = props.TintMapRandomRowCoordProp.Value.hasMixedValue;
                                coord = (TCustomCoord)EditorGUILayout.EnumPopup(coord);
                                EditorGUI.showMixedValue = false;

                                if (ccs.changed)
                                {
                                    _editor.RegisterPropertyChangeUndo(props.TintMapRandomRowCoordProp.Value.name);
                                    props.TintMapRandomRowCoordProp.Value.floatValue = Convert.ToInt32(coord);
                                }
                            }
                        }

                        // Validation and help message
                        var sliceCount = props.TintMapSliceCountProp.Value.floatValue;
                        var rowCount = props.TintMapRowCountProp.Value.floatValue;

                        if (rowCount <= 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Row Count must be greater than 0. Setting to 1 will disable random row selection.",
                                MessageType.Warning);
                        }
                        else if (rowCount > sliceCount && sliceCount > 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Row Count cannot exceed Slice Count. Adjust Row Count accordingly.",
                                MessageType.Warning);
                        }

                        // Auto-fix
                        if (props.TintMapRowCountProp.Value.floatValue < 1)
                        {
                            props.TintMapRowCountProp.Value.floatValue = 1;
                        }
                    }
            }

            MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Blend Rate",
                props.TintMapBlendRateProp.Value,
                props.TintMapBlendRateCoordProp.Value);
        }

        private void InternalDrawFlowMapProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, props.FlowMapProp.Value,
                props.FlowMapOffsetXCoordProp.Value,
                props.FlowMapOffsetYCoordProp.Value,
                props.FlowMapChannelsXProp.Value, props.FlowMapChannelsYProp.Value);
            MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(
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
                MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, props.GradientMapProp.Value, false);
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
                MaterialProperty alphaTransitionMapSecondTextureProp;
                switch (alphaTransitionMapMode)
                {
                    case AlphaTransitionMapMode.SingleTexture:
                        alphaTransitionMapProp = props.AlphaTransitionMapProp.Value;
                        alphaTransitionMapSecondTextureProp = props.AlphaTransitionMapSecondTextureProp.Value;
                        break;
                    case AlphaTransitionMapMode.FlipBook:
                        alphaTransitionMapProp = props.AlphaTransitionMap2DArrayProp.Value;
                        alphaTransitionMapSecondTextureProp = props.AlphaTransitionMapSecondTexture2DArrayProp.Value;
                        break;
                    case AlphaTransitionMapMode.FlipBookBlending:
                        alphaTransitionMapProp = props.AlphaTransitionMap3DProp.Value;
                        alphaTransitionMapSecondTextureProp = props.AlphaTransitionMapSecondTexture3DProp.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, alphaTransitionMapProp,
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
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Flip-Book Progress",
                        props.AlphaTransitionMapProgressProp.Value, props.AlphaTransitionMapProgressCoordProp.Value);

                    MaterialEditorUtility.DrawToggleProperty(_editor, "Random Row Selection",
                        props.AlphaTransitionMapRandomRowSelectionEnabledProp.Value);

                    var randomRowEnabled = props.AlphaTransitionMapRandomRowSelectionEnabledProp.Value.floatValue > 0.5f;

                    if (randomRowEnabled)
                        using (new EditorGUI.IndentLevelScope())
                        {
                            _editor.FloatProperty(props.AlphaTransitionMapRowCountProp.Value, "Row Count");

                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.PrefixLabel("Random Coord");
                                var coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord),
                                    Convert.ToInt32(props.AlphaTransitionMapRandomRowCoordProp.Value.floatValue));
                                if (!Enum.IsDefined(typeof(TCustomCoord), coord))
                                    coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord), 0);

                                using (var ccs = new EditorGUI.ChangeCheckScope())
                                {
                                    EditorGUI.showMixedValue = props.AlphaTransitionMapRandomRowCoordProp.Value.hasMixedValue;
                                    coord = (TCustomCoord)EditorGUILayout.EnumPopup(coord);
                                    EditorGUI.showMixedValue = false;

                                    if (ccs.changed)
                                    {
                                        _editor.RegisterPropertyChangeUndo(props.AlphaTransitionMapRandomRowCoordProp.Value.name);
                                        props.AlphaTransitionMapRandomRowCoordProp.Value.floatValue = Convert.ToInt32(coord);
                                    }
                                }
                            }

                            // Validation and help message
                            var sliceCount = props.AlphaTransitionMapSliceCountProp.Value.floatValue;
                            var rowCount = props.AlphaTransitionMapRowCountProp.Value.floatValue;

                            if (rowCount <= 0)
                            {
                                EditorGUILayout.HelpBox(
                                    "Row Count must be greater than 0. Setting to 1 will disable random row selection.",
                                    MessageType.Warning);
                            }
                            else if (rowCount > sliceCount && sliceCount > 0)
                            {
                                EditorGUILayout.HelpBox(
                                    "Row Count cannot exceed Slice Count. Adjust Row Count accordingly.",
                                    MessageType.Warning);
                            }

                            // Auto-fix
                            if (props.AlphaTransitionMapRowCountProp.Value.floatValue < 1)
                            {
                                props.AlphaTransitionMapRowCountProp.Value.floatValue = 1;
                            }
                        }
                }

                MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Transition Progress",
                    props.AlphaTransitionProgressProp.Value, props.AlphaTransitionProgressCoordProp.Value);
                if (mode == AlphaTransitionMode.Dissolve)
                    _editor.ShaderProperty(props.DissolveSharpnessProp.Value, "Edge Sharpness");

                // 2nd Texture
                {
                    MaterialEditorUtility.DrawEnumProperty<AlphaTransitionBlendMode>(_editor, "2nd Texture Blend Mode",
                        props.AlphaTransitionSecondTextureBlendModeProp.Value);
                    var alphaTransitionSecondTextureBlendMode =
                        (AlphaTransitionBlendMode)props.AlphaTransitionSecondTextureBlendModeProp.Value.floatValue;
                    if (alphaTransitionSecondTextureBlendMode != AlphaTransitionBlendMode.None)
                    {
                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor,
                                alphaTransitionMapSecondTextureProp,
                                props.AlphaTransitionMapSecondTextureOffsetXCoordProp.Value,
                                props.AlphaTransitionMapSecondTextureOffsetYCoordProp.Value,
                                props.AlphaTransitionMapSecondTextureChannelsXProp.Value, null);

                            if (changeCheckScope.changed)
                            {
                                if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBook
                                    && props.AlphaTransitionMapSecondTexture2DArrayProp.Value.textureValue != null)
                                {
                                    var tex2DArray = (Texture2DArray)props.AlphaTransitionMapSecondTexture2DArrayProp
                                        .Value.textureValue;
                                    props.AlphaTransitionMapSecondTextureSliceCountProp.Value.floatValue =
                                        tex2DArray.depth;
                                }

                                if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBookBlending
                                    && props.AlphaTransitionMapSecondTexture3DProp.Value.textureValue != null)
                                {
                                    var tex3D = (Texture3D)props.AlphaTransitionMapSecondTexture3DProp.Value
                                        .textureValue;
                                    props.AlphaTransitionMapSecondTextureSliceCountProp.Value.floatValue = tex3D.depth;
                                }
                            }
                        }

                        if (alphaTransitionMapMode == AlphaTransitionMapMode.FlipBook
                            || alphaTransitionMapMode == AlphaTransitionMapMode.FlipBookBlending)
                            MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor,
                                "Flip-Book Progress",
                                props.AlphaTransitionMapSecondTextureProgressProp.Value,
                                props.AlphaTransitionMapSecondTextureProgressCoordProp.Value);

                        MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Transition Progress",
                            props.AlphaTransitionProgressSecondTextureProp.Value,
                            props.AlphaTransitionProgressCoordSecondTextureProp.Value);

                        if (mode == AlphaTransitionMode.Dissolve)
                            _editor.ShaderProperty(props.DissolveSharpnessSecondTextureProp.Value, "Edge Sharpness");
                    }
                }
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
                        MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, emissionMapProp,
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
                    {
                        MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Flip-Book Progress",
                            props.EmissionMapProgressProp.Value, props.EmissionMapProgressCoordProp.Value);

                        MaterialEditorUtility.DrawToggleProperty(_editor, "Random Row Selection",
                            props.EmissionMapRandomRowSelectionEnabledProp.Value);

                        var randomRowEnabled = props.EmissionMapRandomRowSelectionEnabledProp.Value.floatValue > 0.5f;

                        if (randomRowEnabled)
                            using (new EditorGUI.IndentLevelScope())
                            {
                                _editor.FloatProperty(props.EmissionMapRowCountProp.Value, "Row Count");

                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.PrefixLabel("Random Coord");
                                    var coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord),
                                        Convert.ToInt32(props.EmissionMapRandomRowCoordProp.Value.floatValue));
                                    if (!Enum.IsDefined(typeof(TCustomCoord), coord))
                                        coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord), 0);

                                    using (var ccs = new EditorGUI.ChangeCheckScope())
                                    {
                                        EditorGUI.showMixedValue = props.EmissionMapRandomRowCoordProp.Value.hasMixedValue;
                                        coord = (TCustomCoord)EditorGUILayout.EnumPopup(coord);
                                        EditorGUI.showMixedValue = false;

                                        if (ccs.changed)
                                        {
                                            _editor.RegisterPropertyChangeUndo(props.EmissionMapRandomRowCoordProp.Value.name);
                                            props.EmissionMapRandomRowCoordProp.Value.floatValue = Convert.ToInt32(coord);
                                        }
                                    }
                                }

                                // Validation and help message
                                var sliceCount = props.EmissionMapSliceCountProp.Value.floatValue;
                                var rowCount = props.EmissionMapRowCountProp.Value.floatValue;

                                if (rowCount <= 0)
                                {
                                    EditorGUILayout.HelpBox(
                                        "Row Count must be greater than 0. Setting to 1 will disable random row selection.",
                                        MessageType.Warning);
                                }
                                else if (rowCount > sliceCount && sliceCount > 0)
                                {
                                    EditorGUILayout.HelpBox(
                                        "Row Count cannot exceed Slice Count. Adjust Row Count accordingly.",
                                        MessageType.Warning);
                                }

                                // Auto-fix
                                if (props.EmissionMapRowCountProp.Value.floatValue < 1)
                                {
                                    props.EmissionMapRowCountProp.Value.floatValue = 1;
                                }
                            }
                    }

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
                    MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, props.EmissionColorRampProp.Value, false);

                if (areaType == EmissionAreaType.Edge)
                    MaterialEditorUtility.DrawToggleProperty(_editor, "Keep Edge Transparency",
                        props.KeepEdgeTransparencyProp.Value);

                using (var ccs2 = new EditorGUI.ChangeCheckScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Intensity",
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
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Progress",
                        props.RimTransparencyProgressProp.Value, props.RimTransparencyProgressCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Sharpness",
                        props.RimTransparencySharpnessProp.Value, props.RimTransparencySharpnessCoordProp.Value);
                    MaterialEditorUtility.DrawToggleProperty(_editor, "Inverse",
                        props.InverseRimTransparencyProp.Value);
                }

            MaterialEditorUtility.DrawToggleProperty(_editor, "Luminance",
                props.LuminanceTransparencyEnabledProp.Value);
            if (props.LuminanceTransparencyEnabledProp.Value.floatValue > 0.5f)
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Progress",
                        props.LuminanceTransparencyProgressProp.Value,
                        props.LuminanceTransparencyProgressCoordProp.Value);
                    MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Sharpness",
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

        private void InternalDrawVertexDeformationMapProperties()
        {
            var props = _commonMaterialProperties;

            MaterialEditorUtility.DrawTexture<TCustomCoord>(_editor, props.VertexDeformationMapProp.Value,
                props.VertexDeformationMapOffsetXCoordProp.Value,
                props.VertexDeformationMapOffsetYCoordProp.Value,
                props.VertexDeformationMapChannelProp.Value, null);
            MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(
                _editor,
                "Intensity",
                props.VertexDeformationIntensityProp.Value,
                props.VertexDeformationIntensityCoordProp.Value);
            MaterialEditorUtility.DrawFloatRangeProperty(_editor, "Base Value",
                props.VertexDeformationBaseValueProp.Value, 0,
                1);
        }

        private void InternalDrawShadowCasterProperties()
        {
            var props = _commonMaterialProperties;
            MaterialEditorUtility.DrawToggleProperty(_editor, "Enable", props.ShadowCasterEnabledProp.Value);
            if (props.ShadowCasterEnabledProp.Value.floatValue < 0.5f)
                return;

            MaterialEditorUtility.DrawToggleProperty(_editor, "Apply Vertex Deformation",
                props.ShadowCasterApplyVertexDeformationProp.Value);

            MaterialEditorUtility.DrawToggleProperty(_editor, "Alpha Test Enable",
                props.ShadowCasterAlphaTestEnabledProp.Value);
            if (props.ShadowCasterAlphaTestEnabledProp.Value.floatValue < 0.5f)
                return;

            EditorGUI.indentLevel++;
            MaterialEditorUtility.DrawFloatRangeProperty(_editor, "Cutoff", props.ShadowCasterAlphaCutoffProp.Value, 0,
                1);
            EditorGUI.indentLevel--;

            EditorGUI.LabelField(EditorGUILayout.GetControlRect(), "Alpha Affected By");
            EditorGUI.indentLevel++;
            MaterialEditorUtility.DrawToggleProperty(_editor, "Tint Color",
                props.ShadowCasterAlphaAffectedByTintColorProp.Value);
            MaterialEditorUtility.DrawToggleProperty(_editor, "Flow Map",
                props.ShadowCasterAlphaAffectedByFlowMapProp.Value);
            MaterialEditorUtility.DrawToggleProperty(_editor, "Alpha Transition Map",
                props.ShadowCasterAlphaAffectedByAlphaTransitionMapProp.Value);
            MaterialEditorUtility.DrawToggleProperty(_editor, "Transparency Luminance",
                props.ShadowCasterAlphaAffectedByTransparencyLuminanceProp.Value);
            EditorGUI.indentLevel--;
        }

        #region private variable

        private string _errorMessage = "";
        private const int RenderPriorityMax = 50;
        private const int RenderPriorityMin = -RenderPriorityMax;
        private MaterialEditor _editor;
        private ParticlesUberCommonMaterialProperties _commonMaterialProperties;

        private static readonly GUIContent StreamApplyToAllSystemsText = new("Fix Now",
            "Apply the vertex stream layout to all Particle Systems using this material");

        private readonly List<ParticleSystemRenderer> _renderersUsingThisMaterial;

        private List<ParticleSystemVertexStream> _correctVertexStreams = new();

        private List<ParticleSystemVertexStream> _correctVertexStreamsInstanced = new();

        # endregion
    }
}
