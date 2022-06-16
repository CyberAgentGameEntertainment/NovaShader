// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

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
        private readonly ParticlesUberCommonGUI _commonGUI = new ParticlesUberCommonGUI();

        private readonly ParticlesUberCommonMaterialProperties _commonMaterialProperties =
            new ParticlesUberCommonMaterialProperties();

        protected override void SetupProperties(MaterialProperty[] properties)
        {
            _commonMaterialProperties.Setup(properties);
        }

        protected override void Initialize(MaterialEditor editor, MaterialProperty[] properties)
        {
            _commonMaterialProperties.Initialize(editor, properties);
        }

        protected override void DrawGUI(MaterialEditor editor, MaterialProperty[] properties)
        {
            _commonGUI.Setup(editor, _commonMaterialProperties);
            _commonGUI.DrawRenderSettingsProperties();
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
    }
}