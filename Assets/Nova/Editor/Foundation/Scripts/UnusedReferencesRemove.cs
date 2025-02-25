// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using Nova.Editor.Core.Scripts;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Foundation.Scripts
{
    internal static class UnusedReferencesRemove
    {
        [MenuItem("Tools/NOVA Shader/RemoveUnusedReferences")]
        private static void RemoveUnusedReferences()
        {
            Debug.Log("[NOVA] Start remove unused references.");
            foreach (var obj in Selection.objects)
                if (obj is Material material)
                {
                    RemoveUnusedReferences(material);
                    AssetDatabase.SaveAssetIfDirty(material);
                }

            AssetDatabase.Refresh();
            Debug.Log("[NOVA] Finished remove unused references.");
        }

        private static void RemoveUnusedReferences(Material material)
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
            var isChanged = false;

            // Same Unlit
            {
                isChanged |= FixBaseMap(material);
                isChanged |= FixTintColor(material);
                isChanged |= FixParallaxMap(material);
                isChanged |= FixColorCorrection(material);
                isChanged |= FixAlphaTransition(material);
                isChanged |= FixEmission(material);
            }

            // Lit Specific
            {
                isChanged |= FixSurfaceMap(material);
            }

            if (isChanged)
                EditorUtility.SetDirty(material);
        }

        private static void RemoveUnusedReferencesFromParticlesUberUnlit(Material material)
        {
            var isChanged = false;

            isChanged |= FixBaseMap(material);
            isChanged |= FixTintColor(material);
            isChanged |= FixParallaxMap(material);
            isChanged |= FixColorCorrection(material);
            isChanged |= FixAlphaTransition(material);
            isChanged |= FixEmission(material);

            if (isChanged)
                EditorUtility.SetDirty(material);
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberLit(Material material)
        {
            RemoveUnusedReferencesFromParticlesUberLit(material);
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberUnlit(Material material)
        {
            RemoveUnusedReferencesFromParticlesUberUnlit(material);
        }

        private static bool FixBaseMap(Material material)
        {
            var isChanged = false;
            var mode = (BaseMapMode)material.GetFloat(MaterialPropertyNames.BaseMapMode);
            switch (mode)
            {
                case BaseMapMode.SingleTexture:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.BaseMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.BaseMap3D);
                    break;
                case BaseMapMode.FlipBook:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.BaseMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.BaseMap3D);
                    break;
                case BaseMapMode.FlipBookBlending:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.BaseMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.BaseMap2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isChanged;
        }

        private static bool FixTintColor(Material material)
        {
            var isChanged = false;
            var mode = (TintColorMode)material.GetFloat(MaterialPropertyNames.TintColorMode);
            switch (mode)
            {
                case TintColorMode.SingleColor:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.TintMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.TintMap3D);
                    break;
                case TintColorMode.Texture2D:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.TintMap3D);
                    break;
                case TintColorMode.Texture3D:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.TintMap);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isChanged;
        }

        private static bool FixParallaxMap(Material material)
        {
            var isChanged = false;
            var mode = (ParallaxMapMode)material.GetFloat(MaterialPropertyNames.ParallaxMapMode);
            switch (mode)
            {
                case ParallaxMapMode.SingleTexture:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.ParallaxMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.ParallaxMap3D);
                    break;
                case ParallaxMapMode.FlipBook:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.ParallaxMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.ParallaxMap3D);
                    break;
                case ParallaxMapMode.FlipBookBlending:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.ParallaxMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.ParallaxMap2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isChanged;
        }

        private static bool FixColorCorrection(Material material)
        {
            var isChanged = false;
            var mode = (ColorCorrectionMode)material.GetFloat(MaterialPropertyNames.ColorCorrectionMode);
            switch (mode)
            {
                case ColorCorrectionMode.None:
                case ColorCorrectionMode.Greyscale:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.GradientMap);
                    break;
                case ColorCorrectionMode.GradientMap:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isChanged;
        }

        private static bool FixAlphaTransition(Material material)
        {
            var isChanged = false;
            var mode = (AlphaTransitionMapMode)material.GetFloat(MaterialPropertyNames.AlphaTransitionMapMode);
            // 1st Texture
            {
                switch (mode)
                {
                    case AlphaTransitionMapMode.SingleTexture:
                        isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap2DArray);
                        isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap3D);
                        break;
                    case AlphaTransitionMapMode.FlipBook:
                        isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap);
                        isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap3D);
                        break;
                    case AlphaTransitionMapMode.FlipBookBlending:
                        isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap);
                        isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMap2DArray);
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
                    isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture3D);
                }
                else
                {
                    switch (mode)
                    {
                        case AlphaTransitionMapMode.SingleTexture:
                            isChanged |= ClearTexture(material,
                                MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray);
                            isChanged |= ClearTexture(material,
                                MaterialPropertyNames.AlphaTransitionMapSecondTexture3D);
                            break;
                        case AlphaTransitionMapMode.FlipBook:
                            isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture);
                            isChanged |= ClearTexture(material,
                                MaterialPropertyNames.AlphaTransitionMapSecondTexture3D);
                            break;
                        case AlphaTransitionMapMode.FlipBookBlending:
                            isChanged |= ClearTexture(material, MaterialPropertyNames.AlphaTransitionMapSecondTexture);
                            isChanged |= ClearTexture(material,
                                MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            return isChanged;
        }

        private static bool FixEmission(Material material)
        {
            var isChanged = false;
            var areaMode = (EmissionAreaType)material.GetFloat(MaterialPropertyNames.EmissionAreaType);

            // EmissionMap
            {
                if (areaMode != EmissionAreaType.ByTexture)
                {
                    isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap3D);
                }
                else
                {
                    var mode = (EmissionMapMode)material.GetFloat(MaterialPropertyNames.EmissionMapMode);
                    switch (mode)
                    {
                        case EmissionMapMode.SingleTexture:
                            isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap2DArray);
                            isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap3D);
                            break;
                        case EmissionMapMode.FlipBook:
                            isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap);
                            isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap3D);
                            break;
                        case EmissionMapMode.FlipBookBlending:
                            isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap);
                            isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionMap2DArray);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            // GradientMap
            {
                if (areaMode != EmissionAreaType.ByTexture && areaMode != EmissionAreaType.Edge)
                    isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionColorRamp);

                var colorType = (EmissionColorType)material.GetFloat(MaterialPropertyNames.EmissionColorType);
                if (colorType != EmissionColorType.GradiantMap)
                    isChanged |= ClearTexture(material, MaterialPropertyNames.EmissionColorRamp);
            }
            return isChanged;
        }

        private static bool FixSurfaceMap(Material material)
        {
            var isChanged = false;
            var baseMapMode = (BaseMapMode)material.GetFloat(MaterialPropertyNames.BaseMapMode);
            switch (baseMapMode)
            {
                case BaseMapMode.SingleTexture:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.NormalMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.NormalMap3D);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap3D);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap3D);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SmoothnessMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SmoothnessMap3D);
                    break;
                case BaseMapMode.FlipBook:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.NormalMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.NormalMap3D);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap3D);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap3D);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SmoothnessMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SmoothnessMap3D);
                    break;
                case BaseMapMode.FlipBookBlending:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.NormalMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.NormalMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SmoothnessMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SmoothnessMap2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var workFlowMode = (LitWorkflowMode)material.GetFloat(MaterialPropertyNames.LitWorkflowMode);
            switch (workFlowMode)
            {
                case LitWorkflowMode.Specular:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.MetallicMap3D);
                    break;
                case LitWorkflowMode.Metallic:
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap2DArray);
                    isChanged |= ClearTexture(material, MaterialPropertyNames.SpecularMap3D);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isChanged;
        }

        private static bool ClearTexture(Material material, string propertyName)
        {
            if (material.GetTexture(propertyName) == null)
                return false;
            material.SetTexture(propertyName, null);
            Debug.Log($"[NOVA] {material.name}: Removed unused texture from property: {propertyName}");
            return true;
        }

        private static class ShaderNames
        {
            public const string ParticlesUberLit = "Nova/Particles/UberLit";
            public const string ParticlesUberUnlit = "Nova/Particles/UberUnlit";
            public const string UIParticlesUberLit = "Nova/UIParticles/UberLit";
            public const string UIParticlesUberUnlit = "Nova/UIParticles/UberUnlit";
        }
    }
}
