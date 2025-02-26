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
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.TintMap));
                Assert.IsNull(material.GetTexture(MaterialPropertyNames.TintMap3D));
            }
        }
    }
}
