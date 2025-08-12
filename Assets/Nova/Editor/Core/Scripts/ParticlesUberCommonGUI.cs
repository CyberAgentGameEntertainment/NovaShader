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

                MaterialEditorUtility.DrawRandomRowSelection<TCustomCoord>(
                    _editor,
                    props.BaseMapRandomRowSelectionEnabledProp.Value,
                    props.BaseMapRowCountProp.Value,
                    props.BaseMapRandomRowCoordProp.Value,
                    props.BaseMapSliceCountProp.Value);
            }
            
            if (baseMapMaterialProp != null)
            {
                MaterialEditorUtility.DrawToggleProperty(_editor, "TriTone",
                    props.BaseMapTriToneProp.Value);
                if (props.BaseMapTriToneProp.Value.floatValue != 0.0f)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        _editor.ShaderProperty(props.BaseShadowColorProp.Value, "Shadow Color");
                        _editor.ShaderProperty(props.BaseMidColorProp.Value, "Midtones Color");
                        _editor.ShaderProperty(props.BaseHighlightColorProp.Value, "Highlights Color");

                        // 境界値の取得と制約適用
                        var shadow = Mathf.Clamp01(props.BaseMinValueProp.Value.floatValue);
                        var midtones = Mathf.Clamp01(props.BaseMidValueProp.Value.floatValue);
                        var highlights = Mathf.Clamp01(props.BaseMaxValueProp.Value.floatValue);

                        // 値の順序制約を適用
                        if (shadow > midtones) midtones = shadow;
                        if (midtones > highlights) highlights = midtones;
                        if (highlights < midtones) midtones = highlights;
                        if (midtones < shadow) shadow = midtones;

                        // UI表示（影→中間→ハイライトの順序で表示）
                        EditorGUILayout.Space(4);
                        EditorGUILayout.LabelField("Tone Boundaries", EditorStyles.boldLabel);
                        
                        using (var changeCheck = new EditorGUI.ChangeCheckScope())
                        {
                            shadow = EditorGUILayout.Slider("Shadow Boundary", shadow, 0.0f, Mathf.Min(midtones, highlights));
                            midtones = EditorGUILayout.Slider("Midtones Boundary", midtones, shadow, highlights);
                            highlights = EditorGUILayout.Slider("Highlights Boundary", highlights, Mathf.Max(shadow, midtones), 1.0f);
                            
                            EditorGUILayout.Space(4);
                            
                            // 視覚的な範囲表示（インデントを考慮）
                            var rect = EditorGUILayout.GetControlRect(false, 20);
                            // インデントを手動で適用
                            var indentOffset = EditorGUI.indentLevel * 15f; // Unity標準のインデント幅
                            rect.x += indentOffset;
                            rect.width -= indentOffset;
                            
                            var shadowWidth = shadow * rect.width;
                            var midtonesWidth = (midtones - shadow) * rect.width;
                            var highlightsWidth = (highlights - midtones) * rect.width;
                            var beyondHighlightsWidth = (1.0f - highlights) * rect.width;
                            
                            // 背景
                            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1.0f));
                            
                            // Shadow領域（0 → shadow）
                            var shadowRect = new Rect(rect.x, rect.y, shadowWidth, rect.height);
                            EditorGUI.DrawRect(shadowRect, new Color(0.05f, 0.05f, 0.05f, 1.0f)); // より暗く
                            
                            // Midtones領域（shadow → midtones）
                            var midtonesRect = new Rect(rect.x + shadowWidth, rect.y, midtonesWidth, rect.height);
                            EditorGUI.DrawRect(midtonesRect, new Color(0.35f, 0.35f, 0.35f, 1.0f)); // やや暗く調整
                            
                            // Highlights領域（midtones → highlights）
                            var highlightsRect = new Rect(rect.x + shadowWidth + midtonesWidth, rect.y, highlightsWidth, rect.height);
                            EditorGUI.DrawRect(highlightsRect, new Color(0.65f, 0.65f, 0.65f, 1.0f)); // 中間的な明るさに
                            
                            // Beyond highlights領域（highlights → 1.0）
                            var beyondRect = new Rect(rect.x + shadowWidth + midtonesWidth + highlightsWidth, rect.y, beyondHighlightsWidth, rect.height);
                            EditorGUI.DrawRect(beyondRect, new Color(0.95f, 0.95f, 0.95f, 1.0f)); // ほぼ白
                            
                            // 境界線
                            if (shadow > 0.001f)
                                EditorGUI.DrawRect(new Rect(rect.x + shadowWidth - 1, rect.y, 2, rect.height), Color.red);
                            if (midtones < 0.999f && midtones > shadow + 0.001f)
                                EditorGUI.DrawRect(new Rect(rect.x + shadowWidth + midtonesWidth - 1, rect.y, 2, rect.height), Color.yellow);
                            if (highlights < 0.999f && highlights > midtones + 0.001f)
                                EditorGUI.DrawRect(new Rect(rect.x + shadowWidth + midtonesWidth + highlightsWidth - 1, rect.y, 2, rect.height), Color.green);
                            
                            // 数値表示（境界値として右揃えで表示）
                            var shadowLabelStyle = new GUIStyle(EditorStyles.miniLabel) 
                            { 
                                alignment = TextAnchor.MiddleRight,
                                normal = { textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) }, // 暗い背景に白文字
                                padding = new RectOffset(0, 4, 0, 0) // 右側に少し余白
                            };
                            
                            var midtonesLabelStyle = new GUIStyle(EditorStyles.miniLabel) 
                            { 
                                alignment = TextAnchor.MiddleRight,
                                normal = { textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) }, // グレー背景に白文字
                                padding = new RectOffset(0, 4, 0, 0) // 右側に少し余白
                            };
                            
                            var highlightsLabelStyle = new GUIStyle(EditorStyles.miniLabel) 
                            { 
                                alignment = TextAnchor.MiddleRight,
                                normal = { textColor = new Color(0.0f, 0.0f, 0.0f, 1.0f) }, // 明るい背景に黒文字
                                padding = new RectOffset(0, 4, 0, 0) // 右側に少し余白
                            };
                            
                            // 各領域の境界値を右端に表示（各領域の上限値を示す）
                            if (shadowWidth > 35) GUI.Label(shadowRect, $"S:{shadow:F3}", shadowLabelStyle);
                            if (midtonesWidth > 35) GUI.Label(midtonesRect, $"M:{midtones:F3}", midtonesLabelStyle);
                            if (highlightsWidth > 35) GUI.Label(highlightsRect, $"H:{highlights:F3}", highlightsLabelStyle);
                            
                            if (changeCheck.changed)
                            {
                                // 制約を再適用（順序を保持）
                                shadow = Mathf.Clamp01(shadow);
                                midtones = Mathf.Clamp(midtones, shadow, 1.0f);
                                highlights = Mathf.Clamp(highlights, midtones, 1.0f);
                                
                                // プロパティに値を設定
                                props.BaseMinValueProp.Value.floatValue = shadow;
                                props.BaseMidValueProp.Value.floatValue = midtones;
                                props.BaseMaxValueProp.Value.floatValue = highlights;
                            }
                        }
                    }
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

                MaterialEditorUtility.DrawRandomRowSelection<TCustomCoord>(
                    _editor,
                    props.ParallaxMapRandomRowSelectionEnabledProp.Value,
                    props.ParallaxMapRowCountProp.Value,
                    props.ParallaxMapRandomRowCoordProp.Value,
                    props.ParallaxMapSliceCountProp.Value);
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

            }

            MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor, "Blend Rate",
                props.TintMapBlendRateProp.Value,
                props.TintMapBlendRateCoordProp.Value);

            // Random Row Selection (for both FlipBook and FlipBookBlending modes)
            if (tintColorMode == TintColorMode.FlipBook || tintColorMode == TintColorMode.FlipBookBlending)
            {
                MaterialEditorUtility.DrawRandomRowSelection<TCustomCoord>(
                    _editor,
                    props.TintMapRandomRowSelectionEnabledProp.Value,
                    props.TintMapRowCountProp.Value,
                    props.TintMapRandomRowCoordProp.Value,
                    props.TintMapSliceCountProp.Value);
            }
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

                    MaterialEditorUtility.DrawRandomRowSelection<TCustomCoord>(
                        _editor,
                        props.AlphaTransitionMapRandomRowSelectionEnabledProp.Value,
                        props.AlphaTransitionMapRowCountProp.Value,
                        props.AlphaTransitionMapRandomRowCoordProp.Value,
                        props.AlphaTransitionMapSliceCountProp.Value);
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
                        {
                            MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(_editor,
                                "Flip-Book Progress",
                                props.AlphaTransitionMapSecondTextureProgressProp.Value,
                                props.AlphaTransitionMapSecondTextureProgressCoordProp.Value);
                            
                            // Random Row Selection for Second Texture
                            MaterialEditorUtility.DrawRandomRowSelection<TCustomCoord>(
                                _editor,
                                props.AlphaTransitionMapSecondTextureRandomRowSelectionEnabledProp.Value,
                                props.AlphaTransitionMapSecondTextureRowCountProp.Value,
                                props.AlphaTransitionMapSecondTextureRandomRowCoordProp.Value,
                                props.AlphaTransitionMapSecondTextureSliceCountProp.Value);
                        }

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

                        MaterialEditorUtility.DrawRandomRowSelection<TCustomCoord>(
                            _editor,
                            props.EmissionMapRandomRowSelectionEnabledProp.Value,
                            props.EmissionMapRowCountProp.Value,
                            props.EmissionMapRandomRowCoordProp.Value,
                            props.EmissionMapSliceCountProp.Value);
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
