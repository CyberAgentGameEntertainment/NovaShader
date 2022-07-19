// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     Perform processing after properties of the material with the PerticlesUberLit shader have been changed.
    /// </summary>
    public static class ParticlesUberLitMaterialPostProcessor
    {
        private static readonly int ReceiveShadowsEnabledId =Shader.PropertyToID(MaterialPropertyNames.LitReceiveShadows);
        private static readonly int SpecularHighlightsEnabledId = Shader.PropertyToID(MaterialPropertyNames.SpecularHighlights);
        private static readonly int NormalMapId = Shader.PropertyToID(MaterialPropertyNames.NormalMap);
        private static readonly int MetallicMapId = Shader.PropertyToID(MaterialPropertyNames.MetallicMap);
        private static readonly int SmoothnessMapId = Shader.PropertyToID(MaterialPropertyNames.SmoothnessMap);
        private static readonly int EnvironmentReflectionsEnabledId = Shader.PropertyToID(MaterialPropertyNames.EnvironmentReflections);
        private static readonly int WorkflowModeId = Shader.PropertyToID(MaterialPropertyNames.LitWorkflowMode);
        public static void SetupMaterialKeywords(Material material)
        {
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialKeywords(material);
            
            var receiveShadowsEnabled = material.GetFloat(ReceiveShadowsEnabledId) > 0.5f;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.ReceiveShadowsEnabled, receiveShadowsEnabled);
            
            var specularHighlightsEnabled = material.GetFloat(SpecularHighlightsEnabledId) > 0.5f;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.SpecularHightlightsEnabled, specularHighlightsEnabled);

            var environmentReflectionsEnabled = material.GetFloat(EnvironmentReflectionsEnabledId) > 0.5f;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.EnvironmentReflectionsEnabled, environmentReflectionsEnabled);

            var specularSetup = (LitWorkflowMode)material.GetFloat(WorkflowModeId) == LitWorkflowMode.Specular;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.SpecularSetup, specularSetup);
            
            var hasNormalMap = material.GetTexture(NormalMapId) != null;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.NormalMapEnabled, hasNormalMap);
            
            var hasMetallicMap = material.GetTexture(MetallicMapId) != null;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.MetallicMapEnabled, hasMetallicMap);
            
            var hasSmoothnessMap = material.GetTexture(SmoothnessMapId) != null;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.SmoothnessMapEnabled, hasSmoothnessMap);
        }

        public static void SetupMaterialBlendMode(Material material)
        {
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialBlendMode(material);
        }
    }
}