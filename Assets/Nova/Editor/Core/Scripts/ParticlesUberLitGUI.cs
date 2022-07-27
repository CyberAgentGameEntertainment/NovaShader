// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     GUI for a material assigned the ParticlesUberUnlit Shader.
    /// </summary>
    internal sealed class ParticlesUberLitGUI : ParticlesGUI
    {
        private readonly ParticlesUberCommonGUI _commonGUI = new ParticlesUberCommonGUI();

        private ParticlesUberCommonMaterialProperties _commonMaterialProperties;
        private MaterialEditor _editor;

        protected override void SetupProperties(MaterialProperty[] properties)
        {
            // common properties
            _commonMaterialProperties?.Setup(properties);
            // Render Settings( for lit )
            _litWorkflowModeProp.Setup(properties);
            _litReceiveShadowsProp.Setup(properties);
            _specularHighlightsProp.Setup(properties);
            _environmentReflectionsProp.Setup(properties);

            // Sruface Map
            _normalMapProp.Setup(properties);
            _normalMap2DArrayProp.Setup(properties);
            _normalMap3DProp.Setup(properties);
            _normalMapBumpScaleProp.Setup(properties);
            _specularMapProp.Setup(properties);
            _specularMap2DArrayProp.Setup(properties);
            _specularMap3DProp.Setup(properties);
            _specularProp.Setup(properties);

            _metallicMapProp.Setup(properties);
            _metallicMap2DArrayProp.Setup(properties);
            _metallicMap3DProp.Setup(properties);
            _metallicProp.Setup(properties);
            _metallicMapChannelsXProp.Setup(properties);
            _smoothnessMapProp.Setup(properties);
            _smoothnessMap2DArrayProp.Setup(properties);
            _smoothnessMap3DProp.Setup(properties);
            _smoothnessProp.Setup(properties);
            _smoothnessMapChannelsXProp.Setup(properties);
        }


        protected override void Initialize(MaterialEditor editor, MaterialProperty[] properties)
        {
            // common properties
            _commonMaterialProperties = new ParticlesUberCommonMaterialProperties(editor, properties);

            // Lit Settings
            var prefsKeyPrefix = $"{GetType().Namespace}.{GetType().Name}.";
            var litSettingsFoldoutKey = $"{prefsKeyPrefix}{nameof(LitSettingsFoldout)}";
            var surfaceMapsFoldoutKey = $"{prefsKeyPrefix}{nameof(SurfaceMapsFoldout)}";


            LitSettingsFoldout = new BoolEditorPrefsProperty(litSettingsFoldoutKey, true);
            SurfaceMapsFoldout = new BoolEditorPrefsProperty(surfaceMapsFoldoutKey, true);
        }

        protected override void DrawGUI(MaterialEditor editor, MaterialProperty[] properties)
        {
            _editor = editor;
            _commonGUI.Setup(editor, _commonMaterialProperties);
            // Render Settings
            _commonGUI.DrawRenderSettingsProperties(InternalRenderSettingsProperties);
            // Base Map
            _commonGUI.DrawBaseMapProperties();
            // Surface
            _commonGUI.DrawProperties(SurfaceMapsFoldout, "Surface Maps",
                InternalDrawSurfaceMapsProperties); // Tint Color
            // Tint Color
            _commonGUI.DrawTintColorProperties();
            // Flow Map
            _commonGUI.DrawFlowMapProperties();
            // Color Correction
            _commonGUI.DrawColorCorrectionProperties();
            // Alpha Transition
            _commonGUI.DrawAlphaTransitionProperties();
            // Emission
            _commonGUI.DrawEmissionProperties();
            // Transparency
            _commonGUI.DrawTransparencyProperties();
        }

        private void InternalDrawSurfaceMapsTexturePropertiesCore(string label, Property map2DProp,
            Property map2DArrayProp,
            Property map3DProp, Property normalizedValueProp,
            Property channelsXProperty)
        {
            var mapHeight = EditorGUIUtility.singleLineHeight * 2;
            var mapWidth = mapHeight;
            var props = _commonMaterialProperties;
            // The surface maps mode is decided by baseMapMode.
            var baseMapMode = (BaseMapMode)props.BaseMapModeProp.Value.floatValue;
            MaterialProperty normalMaterialProp;
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    normalMaterialProp = map2DProp.Value;
                    break;
                case BaseMapMode.FlipBook:
                    normalMaterialProp = map2DArrayProp.Value;
                    break;
                case BaseMapMode.FlipBookBlending:
                    normalMaterialProp = map3DProp.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Texture
            Type textureType;
            switch (normalMaterialProp.textureDimension)
            {
                case TextureDimension.Unknown:
                case TextureDimension.None:
                case TextureDimension.Any:
                    textureType = typeof(Texture);
                    break;
                case TextureDimension.Tex2D:
                case TextureDimension.Cube:
                    textureType = typeof(Texture2D);
                    break;
                case TextureDimension.Tex3D:
                    textureType = typeof(Texture3D);
                    break;
                case TextureDimension.Tex2DArray:
                case TextureDimension.CubeArray:
                    textureType = typeof(Texture2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var fullRect = EditorGUILayout.GetControlRect(false, mapHeight);
            var textureRect = fullRect;
            textureRect.width = mapHeight;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                var texture = (Texture)EditorGUI.ObjectField(textureRect, normalMaterialProp.textureValue,
                    textureType,
                    false);
                if (changeCheckScope.changed)
                {
                    _editor.RegisterPropertyChangeUndo(normalMaterialProp.name);
                    normalMaterialProp.textureValue = texture;
                }
            }

            var offsetXFromTextureRectLeft = 8;
            var offsetXFromTextureRectRight = textureRect.width + offsetXFromTextureRectLeft;
            var offsetYFromTextureRectTop = 0.0f;
            if (channelsXProperty == null)
                // If normalizedValueProp is null, num of property is 1.
                // Therefore, the coordinates of the properties are aligned to the center.  
                offsetYFromTextureRectTop = textureRect.height / 2 - EditorGUIUtility.singleLineHeight / 2;

            var propertyRect = EditorGUI.IndentedRect(fullRect);

            propertyRect.y += offsetYFromTextureRectTop;
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            if (normalizedValueProp != null)
            {
                _editor.ShaderProperty(propertyRect, normalizedValueProp.Value, label, 3);
            }
            else
            {
                var rect = propertyRect;
                rect.x += offsetXFromTextureRectRight;
                GUI.Label(rect, label);
            }

            propertyRect.xMin += offsetXFromTextureRectRight;
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            if (channelsXProperty != null)
            {
                var xPropertyLabelRect = propertyRect;
                GUI.Label(xPropertyLabelRect, "Channels");

                var xPropertyXOffset = EditorGUIUtility.labelWidth - mapWidth - offsetXFromTextureRectLeft;
                var xPropertyRect = propertyRect;
                xPropertyRect.xMin += xPropertyXOffset;
                GUI.Label(xPropertyRect, "X");
                xPropertyRect.xMin += GUI.skin.label.fontSize;
                MaterialEditorUtility.DrawEnumContentsProperty<ColorChannels>(_editor, xPropertyRect,
                    channelsXProperty.Value);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        private void InternalDrawSurfaceMapsProperties()
        {
            InternalDrawSurfaceMapsTexturePropertiesCore(
                "Normal Map",
                _normalMapProp,
                _normalMap2DArrayProp,
                _normalMap3DProp,
                _normalMapBumpScaleProp,
                null);

            var mode = (LitWorkflowMode)_litWorkflowModeProp.Value.floatValue;
            if (mode == LitWorkflowMode.Specular)
                InternalDrawSurfaceMapsTexturePropertiesCore(
                    "Specular",
                    _specularMapProp,
                    _specularMap2DArrayProp,
                    _specularMap3DProp,
                    _specularProp,
                    null);
            else
                InternalDrawSurfaceMapsTexturePropertiesCore(
                    "Metallic",
                    _metallicMapProp,
                    _metallicMap2DArrayProp,
                    _metallicMap3DProp,
                    _metallicProp,
                    _metallicMapChannelsXProp);

            InternalDrawSurfaceMapsTexturePropertiesCore(
                "Smoothness",
                _smoothnessMapProp,
                _smoothnessMap2DArrayProp,
                _smoothnessMap3DProp,
                _smoothnessProp,
                _smoothnessMapChannelsXProp);
        }

        private void InternalRenderSettingsProperties()
        {
            _commonGUI.DrawRenderSettingsPropertiesCore();
            MaterialEditorUtility.DrawEnumProperty<LitWorkflowMode>(_editor, "Work Flow Mode",
                _litWorkflowModeProp.Value);
            MaterialEditorUtility.DrawToggleProperty(_editor, "Receive Shadows", _litReceiveShadowsProp.Value);
            MaterialEditorUtility.DrawToggleProperty(_editor, "Specular Highlights", _specularHighlightsProp.Value);
            MaterialEditorUtility.DrawToggleProperty(_editor, "Environment Reflections",
                _environmentReflectionsProp.Value);
        }

        protected override void MaterialChanged(Material material)
        {
            ParticlesUberLitMaterialPostProcessor.SetupMaterialKeywords(material);
            ParticlesUberLitMaterialPostProcessor.SetupMaterialBlendMode(material);
        }

        #region Foldout Properties

        private BoolEditorPrefsProperty LitSettingsFoldout { get; set; }
        private BoolEditorPrefsProperty SurfaceMapsFoldout { get; set; }

        #endregion

        #region Lit Settings Material Properties

        private readonly Property _litWorkflowModeProp = new Property(PropertyNames.LitWorkflowMode);
        private readonly Property _litReceiveShadowsProp = new Property(PropertyNames.LitReceiveShadows);

        #endregion

        #region Render Settings Properties

        // Specular Highlights
        private readonly Property _specularHighlightsProp = new Property(PropertyNames.SpecularHighlights);

        // Environment Reflections
        private readonly Property _environmentReflectionsProp = new Property(PropertyNames.EnvironmentReflections);

        #endregion

        #region Surface Maps Properties

        // normalMap
        private readonly Property _normalMapProp = new Property(PropertyNames.NormalMap);
        private readonly Property _normalMap2DArrayProp = new Property(PropertyNames.NormalMap2DArray);
        private readonly Property _normalMap3DProp = new Property(PropertyNames.NormalMap3D);
        private readonly Property _normalMapBumpScaleProp = new Property(PropertyNames.NormalMapBumpScale);

        // specularMap
        private readonly Property _specularMapProp = new Property(PropertyNames.SpecularMap);
        private readonly Property _specularMap2DArrayProp = new Property(PropertyNames.SpecularMap2DArray);
        private readonly Property _specularMap3DProp = new Property(PropertyNames.SpecularMap3D);
        private readonly Property _specularProp = new Property(PropertyNames.Specular);
        
        // metallicMap
        private readonly Property _metallicMapProp = new Property(PropertyNames.MetallicMap);
        private readonly Property _metallicMap2DArrayProp = new Property(PropertyNames.MetallicMap2DArray);
        private readonly Property _metallicMap3DProp = new Property(PropertyNames.MetallicMap3D);
        private readonly Property _metallicProp = new Property(PropertyNames.Metallic);
        private readonly Property _metallicMapChannelsXProp = new Property(PropertyNames.MetallicMapChannelsX);

        // smoothnessMap
        private readonly Property _smoothnessMapProp = new Property(PropertyNames.SmoothnessMap);
        private readonly Property _smoothnessMap2DArrayProp = new Property(PropertyNames.SmoothnessMap2DArray);
        private readonly Property _smoothnessMap3DProp = new Property(PropertyNames.SmoothnessMap3D);
        private readonly Property _smoothnessProp = new Property(PropertyNames.Smoothness);
        private readonly Property _smoothnessMapChannelsXProp = new Property(PropertyNames.SmoothnessMapChannelsX);

        #endregion
    }
}