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
        [Test]
        public void TestParticlesUberLit()
        {
            var material = new Material(Shader.Find("Nova/Particles/UberLit"));
            TestUnlitParams(material);
            Object.DestroyImmediate(material);
        }

        [Test]
        public void TestParticlesUberUnlit()
        {
            var material = new Material(Shader.Find("Nova/Particles/UberUnlit"));
            TestUnlitParams(material);
            Object.DestroyImmediate(material);
        }

        private static void TestUnlitParams(Material material)
        {
            SetupUnlitParams(material);
            UnusedReferencesRemover.RemoveUnusedReferences(material);
            CheckUnlitParams(material);
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
    }
}
