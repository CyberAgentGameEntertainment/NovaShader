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

            // 選択されたオブジェクトを取得
            var selectedObjects = Selection.objects;

            foreach (var obj in selectedObjects)
                if (obj is Material material)
                    RemoveUnusedReferences(material);

            // アセットデータベースを保存
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NOVA] Finished remove unused references.");
        }

        private static bool MaterialPropertyIsUnused(Material material, string propertyName)
        {
            // この関数はプロジェクトに応じて拡張する必要があります。
            // 現在のシェーダコード、カスタムロジックなどを使ってプロパティの使用状況を判定するべきです。
            // ここでは単純にテクスチャが割り当てられているかを確認します。
            var texture = material.GetTexture(propertyName);
            return texture == null;
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
            FixBaseMap(material);
            FixTintColor(material);
            FixParallaxMap(material);
            FixColorCorrection(material);
            FixAlphaTransition(material);
            FixEmission(material);

            // マテリアルを保存する
            EditorUtility.SetDirty(material);
        }

        private static void RemoveUnusedReferencesFromParticlesUberUnlit(Material material)
        {
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberLit(Material material)
        {
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberUnlit(Material material)
        {
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
            var mode = (TintColorMode)material.GetFloat(MaterialPropertyNames.TintColorMode);
            switch (mode)
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
            var mode = (AlphaTransitionMapMode)material.GetFloat(MaterialPropertyNames.AlphaTransitionMapMode);
            // 1st Texture
            {
                switch (mode)
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
                    switch (mode)
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
            if (areaMode != EmissionAreaType.ByTexture)
            {
                ClearTexture(material, MaterialPropertyNames.EmissionMap);
                ClearTexture(material, MaterialPropertyNames.EmissionMap2DArray);
                ClearTexture(material, MaterialPropertyNames.EmissionMap3D);
                return;
            }

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

            // TODO GradientMap
        }


        private static void ClearTexture(Material material, string propertyName)
        {
            material.SetTexture(propertyName, null);
            Debug.Log($"[NOVA] {material.name}: Removed unused texture from property: {propertyName}");
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
