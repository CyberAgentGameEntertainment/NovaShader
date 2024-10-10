// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;
using PropertyNames = Nova.Editor.Core.Scripts.MaterialPropertyNames;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     GUI for a material assigned the ParticlesUberUnlit Shader.
    /// </summary>
    internal class ParticlesUberUnlitGUIBase<TCustomCoord> : ParticlesGUI where TCustomCoord : Enum
    {
        private ParticlesUberCommonGUI<TCustomCoord> _commonGUI;

        private ParticlesUberCommonMaterialProperties _commonMaterialProperties;

        protected override void SetupProperties(MaterialProperty[] properties)
        {
            _commonMaterialProperties.Setup(properties);
        }

        protected override void Initialize(MaterialEditor editor, MaterialProperty[] properties)
        {
            _commonGUI = new ParticlesUberCommonGUI<TCustomCoord>(editor);
            _commonMaterialProperties = new ParticlesUberCommonMaterialProperties(properties);
        }

        protected override void DrawGUI(MaterialEditor editor, MaterialProperty[] properties)
        {
            _commonGUI.Setup(editor, _commonMaterialProperties);
            _commonGUI.DrawRenderSettingsProperties(null);
            _commonGUI.DrawVertexDeformationProperties();
            _commonGUI.DrawBaseMapProperties();
            _commonGUI.DrawTintColorProperties();
            _commonGUI.DrawFlowMapProperties();
            _commonGUI.DrawParallaxMapProperties();
            _commonGUI.DrawColorCorrectionProperties();
            _commonGUI.DrawAlphaTransitionProperties();
            _commonGUI.DrawEmissionProperties();
            _commonGUI.DrawTransparencyProperties();
            _commonGUI.DrawShadowCasterProperties();
            _commonGUI.DrawFixNowButton();
            _commonGUI.DrawErrorMessage();
        }

        protected override void MaterialChanged(Material material)
        {
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialKeywords(material);
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialBlendMode(material);
        }
    }
}
