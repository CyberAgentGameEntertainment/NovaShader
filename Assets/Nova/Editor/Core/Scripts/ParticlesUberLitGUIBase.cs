// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
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
    internal class ParticlesUberLitGUIBase<TCustomCoord> : ParticlesGUI where TCustomCoord : Enum
    {
        private ParticlesUberCommonGUI<TCustomCoord> _commonGUI;
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
            _commonGUI = new ParticlesUberCommonGUI<TCustomCoord>(editor);
            _commonMaterialProperties = new ParticlesUberCommonMaterialProperties(properties);
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
            // Vertex Deformation
            _commonGUI.DrawVertexDeformationProperties();
            // Base Map
            _commonGUI.DrawBaseMapProperties();
            // Surface
            _commonGUI.DrawProperties(SurfaceMapsFoldout, "Surface Maps",
                InternalDrawSurfaceMapsProperties); // Tint Color
            // Tint Color
            _commonGUI.DrawTintColorProperties();
            // Flow Map
            _commonGUI.DrawFlowMapProperties();
            // Parallax Map
            _commonGUI.DrawParallaxMapProperties();
            // Color Correction
            _commonGUI.DrawColorCorrectionProperties();
            // Alpha Transition
            _commonGUI.DrawAlphaTransitionProperties();
            // Emission
            _commonGUI.DrawEmissionProperties();
            // Transparency
            _commonGUI.DrawTransparencyProperties();
            // ShadowCaster
            _commonGUI.DrawShadowCasterProperties();
            // FixNow
            _commonGUI.DrawFixNowButton();
            // Error Message
            _commonGUI.DrawErrorMessage();
        }

        private void InternalDrawSurfaceMapsTexturePropertiesCore(string label, Property map2DProp,
            Property map2DArrayProp,
            Property map3DProp, Property normalizedValueProp,
            Property channelsXProperty)
        {
            var props = _commonMaterialProperties;
            // The surface maps mode is decided by baseMapMode.
            var baseMapMode = (BaseMapMode)props.BaseMapModeProp.Value.floatValue;
            MaterialProperty textureProp;
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    textureProp = map2DProp.Value;
                    break;
                case BaseMapMode.FlipBook:
                    textureProp = map2DArrayProp.Value;
                    break;
                case BaseMapMode.FlipBookBlending:
                    textureProp = map3DProp.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MaterialEditorUtility.DrawSmallTexture(
                _editor,
                label,
                textureProp,
                channelsXProperty?.Value,
                normalizedValueProp?.Value
            );
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

        private readonly Property _litWorkflowModeProp = new(PropertyNames.LitWorkflowMode);
        private readonly Property _litReceiveShadowsProp = new(PropertyNames.LitReceiveShadows);

        #endregion

        #region Render Settings Properties

        // Specular Highlights
        private readonly Property _specularHighlightsProp = new(PropertyNames.SpecularHighlights);

        // Environment Reflections
        private readonly Property _environmentReflectionsProp = new(PropertyNames.EnvironmentReflections);

        #endregion

        #region Surface Maps Properties

        // normalMap
        private readonly Property _normalMapProp = new(PropertyNames.NormalMap);
        private readonly Property _normalMap2DArrayProp = new(PropertyNames.NormalMap2DArray);
        private readonly Property _normalMap3DProp = new(PropertyNames.NormalMap3D);
        private readonly Property _normalMapBumpScaleProp = new(PropertyNames.NormalMapBumpScale);

        // specularMap
        private readonly Property _specularMapProp = new(PropertyNames.SpecularMap);
        private readonly Property _specularMap2DArrayProp = new(PropertyNames.SpecularMap2DArray);
        private readonly Property _specularMap3DProp = new(PropertyNames.SpecularMap3D);
        private readonly Property _specularProp = new(PropertyNames.Specular);

        // metallicMap
        private readonly Property _metallicMapProp = new(PropertyNames.MetallicMap);
        private readonly Property _metallicMap2DArrayProp = new(PropertyNames.MetallicMap2DArray);
        private readonly Property _metallicMap3DProp = new(PropertyNames.MetallicMap3D);
        private readonly Property _metallicProp = new(PropertyNames.Metallic);
        private readonly Property _metallicMapChannelsXProp = new(PropertyNames.MetallicMapChannelsX);

        // smoothnessMap
        private readonly Property _smoothnessMapProp = new(PropertyNames.SmoothnessMap);
        private readonly Property _smoothnessMap2DArrayProp = new(PropertyNames.SmoothnessMap2DArray);
        private readonly Property _smoothnessMap3DProp = new(PropertyNames.SmoothnessMap3D);
        private readonly Property _smoothnessProp = new(PropertyNames.Smoothness);
        private readonly Property _smoothnessMapChannelsXProp = new(PropertyNames.SmoothnessMapChannelsX);

        #endregion
    }
}
