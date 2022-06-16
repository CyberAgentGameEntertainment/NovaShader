// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using Nova.Editor.Foundation.Scripts;
using UnityEditor;
using UnityEngine;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     GUI for a material assigned the ParticlesUberUnlit Shader.
    /// </summary>
    internal sealed class ParticlesUberLitGUI : ParticlesGUI
    {
        protected override void SetupProperties(MaterialProperty[] properties)
        {
            // common properties
            _commonMaterialProperties.Setup(properties);
            // Lit Settings
            _litWorkflowModeProp.Setup(properties);
        }

        protected override void Initialize(MaterialEditor editor, MaterialProperty[] properties)
        {
            // common properties
            _commonMaterialProperties.Initialize(editor, properties);

            // Lit Settings
            var prefsKeyPrefix = $"{GetType().Namespace}.{GetType().Name}.";
            var litSettingsFoldoutKey = $"{prefsKeyPrefix}{nameof(LitSettingsFoldout)}";
            LitSettingsFoldout = new BoolEditorPrefsProperty(litSettingsFoldoutKey, true);
        }

        protected override void DrawGUI(MaterialEditor editor, MaterialProperty[] properties)
        {
            _commonGUI.BeginDraw(editor, _commonMaterialProperties);

            _commonGUI.DrawRenderSettingsProperties();

            // todo for lit GUI
            using (var foldoutScope =
                   new MaterialEditorUtility.FoldoutHeaderScope(LitSettingsFoldout.Value, "Lit Settings"))
            {
                if (foldoutScope.Foldout)
                {
                }

                LitSettingsFoldout.Value = foldoutScope.Foldout;
            }

            _commonGUI.DrawBaseMapProperties();
            _commonGUI.DrawTintColorProperties();
            _commonGUI.DrawFlowMapProperties();
            _commonGUI.DrawColorCorrectionProperties();
            _commonGUI.DrawAlphaTransitionProperties();
            _commonGUI.DrawEmissionProperties();
            _commonGUI.DrawTransparencyProperties();
        }

        protected override void MaterialChanged(Material material)
        {
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialKeywords(material);
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialBlendMode(material);
        }

        private readonly ParticlesUberCommonGUI _commonGUI = new ParticlesUberCommonGUI();

        private readonly ParticlesUberCommonMaterialProperties _commonMaterialProperties =
            new ParticlesUberCommonMaterialProperties();

        #region Foldout Properties

        private BoolEditorPrefsProperty LitSettingsFoldout { get; set; }

        #endregion

        #region Lit Settings Material Properties

        private readonly Property _litWorkflowModeProp = new Property(PropertyNames.LitWorkflowMode);

        #endregion
    }
}