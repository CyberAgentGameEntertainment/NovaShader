// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using UnityEngine;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     GUI for a material assigned the ParticlesDistortion Shader.
    /// </summary>
    internal sealed class ParticlesDistortionGUI : ParticlesGUI
    {
        #region Render Settings Material Properties

        private readonly Property _cullProp = new Property(PropertyNames.Cull);

        #endregion

        protected override void SetupProperties(MaterialProperty[] properties)
        {
            // Render Settings
            _cullProp.Setup(properties);

            // Distortion
            _baseMapProp.Setup(properties);
            _baseMapOffsetXCoordProp.Setup(properties);
            _baseMapOffsetYCoordProp.Setup(properties);
            _baseMapRotationProp.Setup(properties);
            _baseMapRotationCoordProp.Setup(properties);
            _baseMapRotationOffsetsProp.Setup(properties);
            _baseMapMirrorSamplingProp.Setup(properties);
            _distortionIntensityProp.Setup(properties);
            _distortionIntensityCoordProp.Setup(properties);

            // Flow Map
            _flowMapProp.Setup(properties);
            _flowMapOffsetXCoordProp.Setup(properties);
            _flowMapOffsetYCoordProp.Setup(properties);
            _flowIntensityProp.Setup(properties);
            _flowIntensityCoordProp.Setup(properties);
            _flowMapTargetProp.Setup(properties);

            // Alpha Transition
            _alphaTransitionModeProp.Setup(properties);
            _alphaTransitionMapProp.Setup(properties);
            _alphaTransitionMapOffsetXCoordProp.Setup(properties);
            _alphaTransitionMapOffsetYCoordProp.Setup(properties);
            _alphaTransitionProgressProp.Setup(properties);
            _alphaTransitionProgressCoordProp.Setup(properties);
            _dissolveSharpnessProp.Setup(properties);

            // Transparency
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
            var distortionFoldoutKey = $"{prefsKeyPrefix}{nameof(DistortionFoldout)}";
            var flowMapFoldoutKey = $"{prefsKeyPrefix}{nameof(FlowMapFoldout)}";
            var alphaTransitionFoldoutKey = $"{prefsKeyPrefix}{nameof(AlphaTransitionFoldout)}";
            var transparencyFoldoutKey = $"{prefsKeyPrefix}{nameof(TransparencyFoldout)}";

            RenderSettingsFoldout = new BoolEditorPrefsProperty(renderSettingsFoldoutKey, true);
            DistortionFoldout = new BoolEditorPrefsProperty(distortionFoldoutKey, true);
            FlowMapFoldout = new BoolEditorPrefsProperty(flowMapFoldoutKey, true);
            AlphaTransitionFoldout = new BoolEditorPrefsProperty(alphaTransitionFoldoutKey, true);
            TransparencyFoldout = new BoolEditorPrefsProperty(transparencyFoldoutKey, true);
        }

        protected override void DrawGUI(MaterialEditor editor, MaterialProperty[] properties)
        {
            using (var foldoutScope =
                new MaterialEditorUtility.FoldoutHeaderScope(RenderSettingsFoldout.Value, "Render Settings"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawRenderSettingsProperties(editor, properties);
                }

                RenderSettingsFoldout.Value = foldoutScope.Foldout;
            }

            using (var foldoutScope =
                new MaterialEditorUtility.FoldoutHeaderScope(DistortionFoldout.Value, "Distortion"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawDistortionProperties(editor, properties);
                }

                DistortionFoldout.Value = foldoutScope.Foldout;
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
                new MaterialEditorUtility.FoldoutHeaderScope(AlphaTransitionFoldout.Value, "Alpha Transition"))
            {
                if (foldoutScope.Foldout)
                {
                    DrawAlphaTransitionProperties(editor, properties);
                }

                AlphaTransitionFoldout.Value = foldoutScope.Foldout;
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
            ParticlesDistortionMaterialPostProcessor.SetupMaterialKeywords(material);
        }

        private void DrawRenderSettingsProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<RenderFace>(editor, "Render Face", _cullProp.Value);
        }

        private void DrawDistortionProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawTexture(editor, _baseMapProp.Value, _baseMapOffsetXCoordProp.Value,
                _baseMapOffsetYCoordProp.Value);
            MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Intensity",
                _distortionIntensityProp.Value, _distortionIntensityCoordProp.Value);
            MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Rotation",
                _baseMapRotationProp.Value, _baseMapRotationCoordProp.Value);
            using (new EditorGUI.IndentLevelScope())
            {
                MaterialEditorUtility.DrawVector2Property(editor, "Offset", _baseMapRotationOffsetsProp.Value);
            }

            MaterialEditorUtility.DrawToggleProperty(editor, "Mirror Sampling", _baseMapMirrorSamplingProp.Value);
        }

        private void DrawFlowMapProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawTexture(editor, _flowMapProp.Value, _flowMapOffsetXCoordProp.Value,
                _flowMapOffsetYCoordProp.Value);
            MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Intensity", _flowIntensityProp.Value,
                _flowIntensityCoordProp.Value);
            MaterialEditorUtility.DrawEnumFlagsProperty<FlowMapTargetDistortion>(editor, "Targets", _flowMapTargetProp.Value);
        }

        private void DrawAlphaTransitionProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
            MaterialEditorUtility.DrawEnumProperty<AlphaTransitionMode>(editor, "Mode",
                _alphaTransitionModeProp.Value);
            var mode = (AlphaTransitionMode)_alphaTransitionModeProp.Value.floatValue;
            if (mode != AlphaTransitionMode.None)
            {
                MaterialEditorUtility.DrawTexture(editor, _alphaTransitionMapProp.Value,
                    _alphaTransitionMapOffsetXCoordProp.Value, _alphaTransitionMapOffsetYCoordProp.Value);
                MaterialEditorUtility.DrawPropertyAndCustomCoord(editor, "Progress",
                    _alphaTransitionProgressProp.Value, _alphaTransitionProgressCoordProp.Value);
                if (mode == AlphaTransitionMode.Dissolve)
                {
                    editor.ShaderProperty(_dissolveSharpnessProp.Value, "Edge Sharpness");
                }
            }
        }

        private void DrawTransparencyProperties(MaterialEditor editor, MaterialProperty[] properties)
        {
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

        private BoolEditorPrefsProperty RenderSettingsFoldout { get; set; }
        private BoolEditorPrefsProperty DistortionFoldout { get; set; }
        private BoolEditorPrefsProperty FlowMapFoldout { get; set; }
        private BoolEditorPrefsProperty AlphaTransitionFoldout { get; set; }
        private BoolEditorPrefsProperty TransparencyFoldout { get; set; }

        #endregion

        #region Distortion Material Properties

        private readonly Property _baseMapProp = new Property(PropertyNames.BaseMap);
        private readonly Property _baseMapOffsetXCoordProp = new Property(PropertyNames.BaseMapOffsetXCoord);
        private readonly Property _baseMapOffsetYCoordProp = new Property(PropertyNames.BaseMapOffsetYCoord);
        private readonly Property _baseMapRotationProp = new Property(PropertyNames.BaseMapRotation);
        private readonly Property _baseMapRotationCoordProp = new Property(PropertyNames.BaseMapRotationCoord);
        private readonly Property _baseMapRotationOffsetsProp = new Property(PropertyNames.BaseMapRotationOffsets);
        private readonly Property _baseMapMirrorSamplingProp = new Property(PropertyNames.BaseMapMirrorSampling);
        private readonly Property _distortionIntensityProp = new Property(PropertyNames.DistortionIntensity);
        private readonly Property _distortionIntensityCoordProp = new Property(PropertyNames.DistortionIntensityCoord);

        #endregion

        #region Flow Map Material Properties

        private readonly Property _flowMapProp = new Property(PropertyNames.FlowMap);
        private readonly Property _flowMapOffsetXCoordProp = new Property(PropertyNames.FlowMapOffsetXCoord);
        private readonly Property _flowMapOffsetYCoordProp = new Property(PropertyNames.FlowMapOffsetYCoord);
        private readonly Property _flowIntensityProp = new Property(PropertyNames.FlowIntensity);
        private readonly Property _flowIntensityCoordProp = new Property(PropertyNames.FlowIntensityCoord);
        private readonly Property _flowMapTargetProp = new Property(PropertyNames.FlowMapTarget);

        #endregion

        #region Alpha Transition Material Properties

        private readonly Property _alphaTransitionModeProp = new Property(PropertyNames.AlphaTransitionMode);
        private readonly Property _alphaTransitionMapProp = new Property(PropertyNames.AlphaTransitionMap);

        private readonly Property _alphaTransitionMapOffsetXCoordProp =
            new Property(PropertyNames.AlphaTransitionMapOffsetXCoord);

        private readonly Property _alphaTransitionMapOffsetYCoordProp =
            new Property(PropertyNames.AlphaTransitionMapOffsetYCoord);

        private readonly Property _alphaTransitionProgressProp = new Property(PropertyNames.AlphaTransitionProgress);

        private readonly Property _alphaTransitionProgressCoordProp =
            new Property(PropertyNames.AlphaTransitionProgressCoord);

        private readonly Property _dissolveSharpnessProp = new Property(PropertyNames.DissolveSharpness);

        #endregion

        #region Transparency Material Properties

        private readonly Property _softParticlesEnabledProp = new Property(PropertyNames.SoftParticlesEnabled);
        private readonly Property _softParticlesIntensityProp = new Property(PropertyNames.SoftParticlesIntensity);
        private readonly Property _depthFadeEnabledProp = new Property(PropertyNames.DepthFadeEnabled);
        private readonly Property _depthFadeNearProp = new Property(PropertyNames.DepthFadeNear);
        private readonly Property _depthFadeFarProp = new Property(PropertyNames.DepthFadeFar);
        private readonly Property _depthFadeWidthProp = new Property(PropertyNames.DepthFadeWidth);

        #endregion
    }
}