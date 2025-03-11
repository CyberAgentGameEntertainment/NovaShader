// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using Nova.Editor.Core.Scripts;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nova.Editor.Foundation.Scripts
{
    public static class UnusedReferencesRemover
    {
        [MenuItem("Tools/NOVA Shader/RemoveUnusedReferences")]
        private static void RemoveUnusedReferences()
        {
            Debug.Log("[NOVA] Start remove unused references.");
            foreach (var obj in Selection.objects)
                if (obj is Material material)
                {
                    Undo.RecordObject(material, "[NOVA] Remove Unused References");
                    RefreshMaterial(material);
                    RemoveUnusedReferences(material);
                    EditorUtility.SetDirty(material);
                }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NOVA] Finished remove unused references.");
        }

        public static void RemoveUnusedReferences(Material material)
        {
            var shader = material.shader;
            var shaderName = shader.name;

            switch (shaderName)
            {
                case ShaderNames.ParticlesUberLit:
                    RemoveUnusedReferencesFromParticlesUberLit(material);
                    break;
                case ShaderNames.ParticlesUberUnlit:
                    RemoveUnusedReferencesFromParticlesUberUnlit(material);
                    break;
                case ShaderNames.UIParticlesUberLit:
                    RemoveUnusedReferencesFromUIParticlesUberLit(material);
                    break;
                case ShaderNames.UIParticlesUberUnlit:
                    RemoveUnusedReferencesFromUIParticlesUberUnlit(material);
                    break;
                default:
                    Debug.LogWarning($"[NOVA] {material.name} is not a target shader.");
                    break;
            }
        }

        private static void RemoveUnusedReferencesFromParticlesUberLit(Material material)
        {
            // Same Unlit
            {
                FixBaseMap(material);
                FixTintColor(material);
                FixParallaxMap(material);
                FixColorCorrection(material);
                FixAlphaTransition(material);
                FixEmission(material);
            }

            // Lit Specific
            {
                FixSurfaceMap(material);
            }
        }

        private static void RemoveUnusedReferencesFromParticlesUberUnlit(Material material)
        {
            FixBaseMap(material);
            FixTintColor(material);
            FixParallaxMap(material);
            FixColorCorrection(material);
            FixAlphaTransition(material);
            FixEmission(material);
        }

        private static void RefreshMaterial(Material material)
        {
            if (material == null)
                return;
            var newMaterial = new Material(material.shader);
            // Remove Another Shader's Properties
            newMaterial.CopyMatchingPropertiesFromMaterial(material);
            material.CopyPropertiesFromMaterial(newMaterial);
            Object.DestroyImmediate(newMaterial);
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberLit(Material material)
        {
            RemoveUnusedReferencesFromParticlesUberLit(material);
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberUnlit(Material material)
        {
            RemoveUnusedReferencesFromParticlesUberUnlit(material);
        }

        private static void FixBaseMap(Material material)
        {
            var mode = (BaseMapMode)material.GetFloat(MaterialPropertyNames.BaseMapMode);
            switch (mode)
            {
                case BaseMapMode.SingleTexture:
                    ClearTexture(material, MaterialPropertyNames.BaseMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.BaseMap3D);
                    break;
                case BaseMapMode.FlipBook:
                    ClearTexture(material, MaterialPropertyNames.BaseMap);
                    ClearTexture(material, MaterialPropertyNames.BaseMap3D);
                    break;
                case BaseMapMode.FlipBookBlending:
                    ClearTexture(material, MaterialPropertyNames.BaseMap);
                    ClearTexture(material, MaterialPropertyNames.BaseMap2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void FixTintColor(Material material)
        {
            var areaMode = (TintAreaMode)material.GetFloat(MaterialPropertyNames.TintAreaMode);
            if (areaMode == TintAreaMode.None)
            {
                ClearTexture(material, MaterialPropertyNames.TintMap);
                ClearTexture(material, MaterialPropertyNames.TintMap3D);
                return;
            }

            var colorMode = (TintColorMode)material.GetFloat(MaterialPropertyNames.TintColorMode);
            switch (colorMode)
            {
                case TintColorMode.SingleColor:
                    ClearTexture(material, MaterialPropertyNames.TintMap);
                    ClearTexture(material, MaterialPropertyNames.TintMap3D);
                    break;
                case TintColorMode.Texture2D:
                    ClearTexture(material, MaterialPropertyNames.TintMap3D);
                    break;
                case TintColorMode.Texture3D:
                    ClearTexture(material, MaterialPropertyNames.TintMap);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void FixParallaxMap(Material material)
        {
            var mode = (ParallaxMapMode)material.GetFloat(MaterialPropertyNames.ParallaxMapMode);
            switch (mode)
            {
                case ParallaxMapMode.SingleTexture:
                    ClearTexture(material, MaterialPropertyNames.ParallaxMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.ParallaxMap3D);
                    break;
                case ParallaxMapMode.FlipBook:
                    ClearTexture(material, MaterialPropertyNames.ParallaxMap);
                    ClearTexture(material, MaterialPropertyNames.ParallaxMap3D);
                    break;
                case ParallaxMapMode.FlipBookBlending:
                    ClearTexture(material, MaterialPropertyNames.ParallaxMap);
                    ClearTexture(material, MaterialPropertyNames.ParallaxMap2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void FixColorCorrection(Material material)
        {
            var mode = (ColorCorrectionMode)material.GetFloat(MaterialPropertyNames.ColorCorrectionMode);
            switch (mode)
            {
                case ColorCorrectionMode.None:
                case ColorCorrectionMode.Greyscale:
                    ClearTexture(material, MaterialPropertyNames.GradientMap);
                    break;
                case ColorCorrectionMode.GradientMap:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void FixAlphaTransition(Material material)
        {
            var mode = (AlphaTransitionMode)material.GetFloat(MaterialPropertyNames.AlphaTransitionMode);
            if (mode == AlphaTransitionMode.None)
            {
                ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap);
                ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap2DArray);
                ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap3D);
                ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture);
                ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray);
                ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture3D);
                return;
            }

            var mapMode = (AlphaTransitionMapMode)material.GetFloat(MaterialPropertyNames.AlphaTransitionMapMode);
            // 1st Texture
            {
                switch (mapMode)
                {
                    case AlphaTransitionMapMode.SingleTexture:
                        ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap2DArray);
                        ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap3D);
                        break;
                    case AlphaTransitionMapMode.FlipBook:
                        ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap);
                        ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap3D);
                        break;
                    case AlphaTransitionMapMode.FlipBookBlending:
                        ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap);
                        ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap2DArray);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // 2nd Texture
            {
                var blendMode = (AlphaTransitionBlendMode)material.GetFloat(MaterialPropertyNames
                    .AlphaTransitionSecondTextureBlendMode);
                if (blendMode == AlphaTransitionBlendMode.None)
                {
                    ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture);
                    ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray);
                    ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture3D);
                }
                else
                {
                    switch (mapMode)
                    {
                        case AlphaTransitionMapMode.SingleTexture:
                            ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray);
                            ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture3D);
                            break;
                        case AlphaTransitionMapMode.FlipBook:
                            ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture);
                            ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture3D);
                            break;
                        case AlphaTransitionMapMode.FlipBookBlending:
                            ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture);
                            ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private static void FixEmission(Material material)
        {
            var areaMode = (EmissionAreaType)material.GetFloat(MaterialPropertyNames.EmissionAreaType);

            // EmissionMap
            {
                if (areaMode != EmissionAreaType.ByTexture)
                {
                    ClearTexture(material, MaterialPropertyNames.EmissionMap);
                    ClearTexture(material, MaterialPropertyNames.EmissionMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.EmissionMap3D);
                }
                else
                {
                    var mode = (EmissionMapMode)material.GetFloat(MaterialPropertyNames.EmissionMapMode);
                    switch (mode)
                    {
                        case EmissionMapMode.SingleTexture:
                            ClearTexture(material, MaterialPropertyNames.EmissionMap2DArray);
                            ClearTexture(material, MaterialPropertyNames.EmissionMap3D);
                            break;
                        case EmissionMapMode.FlipBook:
                            ClearTexture(material, MaterialPropertyNames.EmissionMap);
                            ClearTexture(material, MaterialPropertyNames.EmissionMap3D);
                            break;
                        case EmissionMapMode.FlipBookBlending:
                            ClearTexture(material, MaterialPropertyNames.EmissionMap);
                            ClearTexture(material, MaterialPropertyNames.EmissionMap2DArray);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            // GradientMap
            {
                if (areaMode != EmissionAreaType.ByTexture && areaMode != EmissionAreaType.Edge)
                    ClearTexture(material, MaterialPropertyNames.EmissionColorRamp);

                var colorType = (EmissionColorType)material.GetFloat(MaterialPropertyNames.EmissionColorType);
                if (colorType != EmissionColorType.GradiantMap)
                    ClearTexture(material, MaterialPropertyNames.EmissionColorRamp);
            }
        }

        private static void FixSurfaceMap(Material material)
        {
            var baseMapMode = (BaseMapMode)material.GetFloat(MaterialPropertyNames.BaseMapMode);
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    ClearTexture(material, MaterialPropertyNames.NormalMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.NormalMap3D);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap3D);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap3D);
                    ClearTexture(material, MaterialPropertyNames.SmoothnessMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.SmoothnessMap3D);
                    break;
                case BaseMapMode.FlipBook:
                    ClearTexture(material, MaterialPropertyNames.NormalMap);
                    ClearTexture(material, MaterialPropertyNames.NormalMap3D);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap3D);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap3D);
                    ClearTexture(material, MaterialPropertyNames.SmoothnessMap);
                    ClearTexture(material, MaterialPropertyNames.SmoothnessMap3D);
                    break;
                case BaseMapMode.FlipBookBlending:
                    ClearTexture(material, MaterialPropertyNames.NormalMap);
                    ClearTexture(material, MaterialPropertyNames.NormalMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.SmoothnessMap);
                    ClearTexture(material, MaterialPropertyNames.SmoothnessMap2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var workFlowMode = (LitWorkflowMode)material.GetFloat(MaterialPropertyNames.LitWorkflowMode);
            switch (workFlowMode)
            {
                case LitWorkflowMode.Specular:
                    ClearTexture(material, MaterialPropertyNames.MetallicMap);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.MetallicMap3D);
                    break;
                case LitWorkflowMode.Metallic:
                    ClearTexture(material, MaterialPropertyNames.SpecularMap);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap2DArray);
                    ClearTexture(material, MaterialPropertyNames.SpecularMap3D);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ClearTexture(Material material, string propertyName)
        {
            var id = Shader.PropertyToID(propertyName);
            if (material.GetTexture(id) == null)
                return;
            material.SetTexture(id, null);
            Debug.Log($"[NOVA] {material.name}: Removed unused texture from property: {propertyName}");
        }

        public static class ShaderNames
        {
            public const string ParticlesUberLit = "Nova/Particles/UberLit";
            public const string ParticlesUberUnlit = "Nova/Particles/UberUnlit";
            public const string UIParticlesUberLit = "Nova/UIParticles/UberLit";
            public const string UIParticlesUberUnlit = "Nova/UIParticles/UberUnlit";
        }
    }
}
