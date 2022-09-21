// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     Perform processing after properties of the material with the PerticlesUberLit shader have been changed.
    /// </summary>
    public static class ParticlesUberLitMaterialPostProcessor
    {
        private static readonly int ReceiveShadowsEnabledId =
            Shader.PropertyToID(MaterialPropertyNames.LitReceiveShadows);

        private static readonly int SpecularHighlightsEnabledId =
            Shader.PropertyToID(MaterialPropertyNames.SpecularHighlights);

        private static readonly int NormalMapId = Shader.PropertyToID(MaterialPropertyNames.NormalMap);
        private static readonly int NormalMap2DArrayId = Shader.PropertyToID(MaterialPropertyNames.NormalMap2DArray);
        private static readonly int NormalMap3DId = Shader.PropertyToID(MaterialPropertyNames.NormalMap3D);
        private static readonly int MetallicMapId = Shader.PropertyToID(MaterialPropertyNames.MetallicMap);

        private static readonly int MetallicMap2DArrayId =
            Shader.PropertyToID(MaterialPropertyNames.MetallicMap2DArray);

        private static readonly int MetallicMap3DId = Shader.PropertyToID(MaterialPropertyNames.MetallicMap3D);
        private static readonly int SmoothnessMapId = Shader.PropertyToID(MaterialPropertyNames.SmoothnessMap);

        private static readonly int SmoothnessMap2DArrayId =
            Shader.PropertyToID(MaterialPropertyNames.SmoothnessMap2DArray);

        private static readonly int SmoothnessMap3DId = Shader.PropertyToID(MaterialPropertyNames.SmoothnessMap3D);
        private static readonly int SpecularMapId = Shader.PropertyToID(MaterialPropertyNames.SpecularMap);

        private static readonly int SpecularMap2DArrayId =
            Shader.PropertyToID(MaterialPropertyNames.SpecularMap2DArray);

        private static readonly int SpecularMap3DId = Shader.PropertyToID(MaterialPropertyNames.SpecularMap3D);
        private static readonly int BaseMapModeId = Shader.PropertyToID(MaterialPropertyNames.BaseMapMode);

        private static readonly int EnvironmentReflectionsEnabledId =
            Shader.PropertyToID(MaterialPropertyNames.EnvironmentReflections);

        private static readonly int WorkflowModeId = Shader.PropertyToID(MaterialPropertyNames.LitWorkflowMode);

        private static bool HasSurfaceMap(Material material, BaseMapMode baseMapMode, int map2DId, int map2DArrayId,
            int map3DID)
        {
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    return material.GetTexture(map2DId) != null;
                case BaseMapMode.FlipBook:
                    return material.GetTexture(map2DArrayId) != null;
                case BaseMapMode.FlipBookBlending:
                    return material.GetTexture(map3DID) != null;
            }

            return false;
        }

        public static void SetupMaterialKeywords(Material material)
        {
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialKeywords(material);

            var receiveShadowsEnabled = material.GetFloat(ReceiveShadowsEnabledId) > 0.5f;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.ReceiveShadowsEnabled, receiveShadowsEnabled);

            var specularHighlightsEnabled = material.GetFloat(SpecularHighlightsEnabledId) > 0.5f;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.SpecularHightlightsEnabled,
                specularHighlightsEnabled);

            var environmentReflectionsEnabled = material.GetFloat(EnvironmentReflectionsEnabledId) > 0.5f;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.EnvironmentReflectionsEnabled,
                environmentReflectionsEnabled);

            var specularSetup = (LitWorkflowMode)material.GetFloat(WorkflowModeId) == LitWorkflowMode.Specular;
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.SpecularSetup, specularSetup);

            var baseMapMode = (BaseMapMode)material.GetFloat(BaseMapModeId);
            var hasNormalMap = HasSurfaceMap(material, baseMapMode, NormalMapId, NormalMap2DArrayId, NormalMap3DId);
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.NormalMapEnabled, hasNormalMap);

            var hasMetallicMap =
                HasSurfaceMap(material, baseMapMode, MetallicMapId, MetallicMap2DArrayId, MetallicMap3DId);
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.MetallicMapEnabled, hasMetallicMap);

            var hasSmoothnessMap = HasSurfaceMap(material, baseMapMode, SmoothnessMapId, SmoothnessMap2DArrayId,
                SmoothnessMap3DId);
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.SmoothnessMapEnabled, hasSmoothnessMap);

            var hasSpecularMap =
                HasSurfaceMap(material, baseMapMode, SpecularMapId, SpecularMap2DArrayId, SpecularMap3DId);
            MaterialEditorUtility.SetKeyword(material, ShaderKeywords.SpecularMapEnabled, hasSpecularMap);
        }

        public static void SetupMaterialBlendMode(Material material)
        {
            ParticlesUberUnlitMaterialPostProcessor.SetupMaterialBlendMode(material);
        }
    }
}