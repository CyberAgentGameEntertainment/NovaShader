// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using Nova.Editor.Core.Scripts;
using Nova.Editor.Foundation.Scripts;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor
{
    public class UnusedReferencesRemoverTest
    {
        [TestCase("Nova/Particles/UberUnlit")]
        [TestCase("Nova/Particles/UberLit")]
        [TestCase("Nova/UIParticles/UberLit")]
        [TestCase("Nova/UIParticles/UberUnlit")]
        public void TestParticles(string shaderName)
        {
            var material = new Material(Shader.Find(shaderName));
            switch (shaderName)
            {
                case UnusedReferencesRemover.ShaderNames.ParticlesUberLit:
                case UnusedReferencesRemover.ShaderNames.UIParticlesUberLit:
                    TestUnlitParams(material);
                    TestLitParams(material);
                    break;
                case UnusedReferencesRemover.ShaderNames.ParticlesUberUnlit:
                case UnusedReferencesRemover.ShaderNames.UIParticlesUberUnlit:
                    TestUnlitParams(material);
                    break;
                default:
                    Debug.LogWarning($"[NOVA] Testing: {material.name} is not a target shader.");
                    break;
            }

            Object.DestroyImmediate(material);
        }

        private static void TestUnlitParams(Material material)
        {
            SetupUnlitParams(material);
            UnusedReferencesRemover.RemoveUnusedReferences(material);
            CheckUnlitParams(material);
        }

        private static void TestLitParams(Material material)
        {
            SetupLitParams(material);
            UnusedReferencesRemover.RemoveUnusedReferences(material);
            CheckLitParams(material);
        }

        private static void SetupUnlitParams(Material material)
        {
            // BaseMap
            {
                material.SetTexture(MaterialPropertyNames.BaseMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.BaseMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetFloat(MaterialPropertyNames.BaseMapMode, (float)BaseMapMode.SingleTexture);
            }

            // TintColor
            {
                material.SetFloat(MaterialPropertyNames.TintAreaMode, (float)TintAreaMode.All);
                material.SetTexture(MaterialPropertyNames.TintMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.TintMap3D,
                    new Texture3D(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetFloat(MaterialPropertyNames.TintColorMode, (float)TintColorMode.SingleColor);
            }

            // ParallaxMap
            {
                material.SetTexture(MaterialPropertyNames.ParallaxMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.ParallaxMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetFloat(MaterialPropertyNames.ParallaxMapMode, (float)ParallaxMapMode.SingleTexture);
            }

            // ColorCorrection
            {
                material.SetTexture(MaterialPropertyNames.GradientMap, new Texture2D(1, 1));
                material.SetFloat(MaterialPropertyNames.ColorCorrectionMode, (float)ColorCorrectionMode.None);
            }

            // AlphaTransition
            {
                material.SetFloat(MaterialPropertyNames.AlphaTransitionMode,
                    (float)AlphaTransitionMode.Fade);
                material.SetTexture(MaterialPropertyNames.AlphaTransitionMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.AlphaTransitionMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetFloat(MaterialPropertyNames.AlphaTransitionMapMode,
                    (float)AlphaTransitionMapMode.SingleTexture);
                material.SetFloat(MaterialPropertyNames.AlphaTransitionSecondTextureBlendMode,
                    (float)AlphaTransitionBlendMode.None);
            }

            // Emission
            {
                material.SetTexture(MaterialPropertyNames.EmissionMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.EmissionMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetFloat(MaterialPropertyNames.EmissionAreaType, (float)EmissionAreaType.ByTexture);
                material.SetFloat(MaterialPropertyNames.EmissionMapMode, (float)EmissionMapMode.SingleTexture);
            }
        }

        private static void SetupLitParams(Material material)
        {
            // SurfaceMap
            {
                material.SetTexture(MaterialPropertyNames.NormalMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.NormalMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetTexture(MaterialPropertyNames.NormalMap3D,
                    new Texture3D(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetTexture(MaterialPropertyNames.SpecularMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.SpecularMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetTexture(MaterialPropertyNames.SpecularMap3D,
                    new Texture3D(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetTexture(MaterialPropertyNames.MetallicMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.MetallicMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetTexture(MaterialPropertyNames.MetallicMap3D,
                    new Texture3D(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetTexture(MaterialPropertyNames.SmoothnessMap, new Texture2D(1, 1));
                material.SetTexture(MaterialPropertyNames.SmoothnessMap2DArray,
                    new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetTexture(MaterialPropertyNames.SmoothnessMap3D,
                    new Texture3D(1, 1, 1, TextureFormat.RGBA32, false));
                material.SetFloat(MaterialPropertyNames.BaseMapMode, (float)BaseMapMode.SingleTexture);
                material.SetFloat(MaterialPropertyNames.LitWorkflowMode, (float)LitWorkflowMode.Specular);
            }
        }

        private static void CheckUnlitParams(Material material)
        {
            // BaseMap
            {
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.BaseMap2DArray));
                Assert.IsNotNull(material.GetTexture(MaterialPropertyNames.BaseMap));
            }

            // TintColor
            {
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.TintMap3D));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.TintMap));
            }

            // ParallaxMap
            {
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.ParallaxMap2DArray));
                Assert.IsNotNull(material.GetTexture(MaterialPropertyNames.ParallaxMap));
            }

            // ColorCorrection
            {
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.GradientMap));
            }

            // AlphaTransition
            {
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.AlphaTransitionMap2DArray));
                Assert.IsNotNull(material.GetTexture(MaterialPropertyNames.AlphaTransitionMap));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.AlphaTransitionMapSecondTexture));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.AlphaTransitionMapSecondTexture2DArray));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.AlphaTransitionMapSecondTexture3D));
            }

            // Emission
            {
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.EmissionMap2DArray));
                Assert.IsNotNull(material.GetTexture(MaterialPropertyNames.EmissionMap));
            }
        }

        private static void CheckLitParams(Material material)
        {
            // SurfaceMap
            {
                Assert.IsNotNull(material.GetTexture(MaterialPropertyNames.NormalMap));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.NormalMap2DArray));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.NormalMap3D));
                Assert.IsNotNull(material.GetTexture(MaterialPropertyNames.SpecularMap));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.SpecularMap2DArray));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.SpecularMap3D));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.MetallicMap));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.MetallicMap2DArray));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.MetallicMap3D));
                Assert.IsNotNull(material.GetTexture(MaterialPropertyNames.SmoothnessMap));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.SmoothnessMap2DArray));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.SmoothnessMap3D));
                Assert.AreEqual((float)BaseMapMode.SingleTexture, material.GetFloat(MaterialPropertyNames.BaseMapMode));
                Assert.AreEqual((float)LitWorkflowMode.Specular,
                    material.GetFloat(MaterialPropertyNames.LitWorkflowMode));
            }
        }
    }
}
