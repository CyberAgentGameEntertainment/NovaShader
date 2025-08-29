// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nova.Editor.Core.Scripts.Optimizer;
using NUnit.Framework;
using TestHelper.Attributes;
using Tests.Runtime.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools.Graphics;
using Object = UnityEngine.Object;

namespace Tests.Runtime
{
    /// <summary>
    ///     Regression tests that run from Unity Test Runner.
    ///     Before running these tests, please read Documentation~/TestDucument.md
    ///     for required environment setup instructions.
    /// </summary>
    public class AverageTest
    {
        [TestCase("Test_Unlit", ExpectedResult = null)]
        [TestCase("Test_Lit", ExpectedResult = null)]
        [TestCase("Test_UIParticleUnlit", ExpectedResult = null)]
        [TestCase("Test_UIParticleLit", ExpectedResult = null)]
        [TestCase("Test_UIParticleMask", ExpectedResult = null)]
        [TestCase("Test_Distortion", ExpectedResult = null)]
        [TestCase("Test_Vertex_Deformation", ExpectedResult = null)]
        [GameViewResolution(1920, 1080, "Full HD")]
        [TimeScale(0.0f)]
        public IEnumerator Test(string scenePath)
        {
            yield return TestUtility.LoadScene($"Assets/Tests/Scenes/{scenePath}.unity");

            var settings = new ImageComparisonSettings
            {
                TargetWidth = Screen.width,
                TargetHeight = Screen.height,
                AverageCorrectnessThreshold = 0.005f,
                PerPixelCorrectnessThreshold = 0.005f,
                IncorrectPixelsThreshold = 0.1f
            };
            // Take a screenshot at this timing to match the scene rendering result
            var screenshotSrc = ScreenCapture.CaptureScreenshotAsTexture();
            var expected = TestUtility.ExpectedImage();
            // Recreate to match the successful image format
            var screenshot = new Texture2D(expected.width, expected.height, expected.format, false);
            screenshot.SetPixels(screenshotSrc.GetPixels());
            screenshot.Apply();
            // Image comparison using Flip
            ImageAssertExtensions.AreEqualWithFlip(screenshot, settings);

            Object.Destroy(screenshotSrc);
            Object.Destroy(screenshot);
            Object.Destroy(expected);
            yield return null;
        }

        [Test(ExpectedResult = null)]
        public IEnumerator TestOptimizedShader()
        {
            var settings = new ImageComparisonSettings
            {
                TargetWidth = Screen.width,
                TargetHeight = Screen.height,
                AverageCorrectnessThreshold = 0.0005f,
                PerPixelCorrectnessThreshold = 0.0005f
            };
            // Capture before shader replacement
            yield return TestUtility.LoadScene("Assets/Tests/Scenes/Test_OptimizedShader.unity");
            var expected = TestUtility.CaptureActualImage(new List<Camera> { Camera.main }, settings);

            // Generate and replace with optimized shaders
            OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
            var optimizedMaterialsPath = "Assets/Tests/Scenes/Materials/Optimized";
            // Get all materials in the Optimized folder
            var materials = AssetDatabase.FindAssets("t:Material", new[] { optimizedMaterialsPath })
                .Select(guid => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            // Create a temporary folder for material copies
            var tempFolderPath = "Assets/Tests/Scenes/Materials/Temp";
            if (!Directory.Exists(tempFolderPath)) Directory.CreateDirectory(tempFolderPath);

            // Copy each material to temp folder
            foreach (var material in materials)
            {
                var originalPath = AssetDatabase.GetAssetPath(material);
                var fileName = Path.GetFileName(originalPath);
                var tempPath = Path.Combine(tempFolderPath, fileName);
                AssetDatabase.CopyAsset(originalPath, tempPath);
            }

            // Replace test materials' shaders with optimized shaders
            OptimizedShaderReplacer.Replace(new OptimizedShaderReplacer.Settings
            {
                OpaqueRequiredPasses = OptionalShaderPass.DepthOnly | OptionalShaderPass.DepthNormals |
                                       OptionalShaderPass.ShadowCaster,
                CutoutRequiredPasses = OptionalShaderPass.ShadowCaster,
                TransparentRequiredPasses = OptionalShaderPass.None,
                TargetFolderPath = "Assets/Tests/Scenes/Materials/Optimized"
            });
            // Capture after replacement
            yield return TestUtility.LoadScene("Assets/Tests/Scenes/Test_OptimizedShader.unity");
            var actual = TestUtility.CaptureActualImage(new List<Camera> { Camera.main }, settings);
            ImageAssertExtensions.AreEqualWithFlip(expected, actual, settings);

            // Restore materials from temp folder and delete it
            // Move materials back to original location
            var tempMaterials = AssetDatabase.FindAssets("t:Material", new[] { tempFolderPath })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            foreach (var tempPath in tempMaterials)
            {
                var fileName = Path.GetFileName(tempPath);
                var originalPath = Path.Combine(optimizedMaterialsPath, fileName);
                var material = AssetDatabase.LoadAssetAtPath<Material>(tempPath);
                var originalMaterial = AssetDatabase.LoadAssetAtPath<Material>(originalPath);
                EditorUtility.CopySerialized(material, originalMaterial);
                AssetDatabase.SaveAssets();
            }

            // Delete temp folder
            AssetDatabase.DeleteAsset(tempFolderPath);
            AssetDatabase.Refresh();
            Object.Destroy(expected);
            Object.Destroy(actual);
            yield return null;
        }

        [MenuItem("Tools/NOVA Shader/Test/Copy AverageTest Result")]
        private static void CopyResult()
        {
            string[] platforms = { @"WindowsEditor/Direct3D11", @"OSXEditor_AppleSilicon/Metal" };
            foreach (var platform in platforms)
            {
                // Define source and destination paths
                var sourceDirectory = $"Assets/ActualImages/Linear/{platform}/None";
                var destinationDirectory = $"Assets/Tests/SuccessfulImages/Linear/{platform}/None";

                if (!Directory.Exists(sourceDirectory)) continue;
                // Create destination directory if it doesn't exist
                if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);

                // Get files from source directory and copy those that meet the conditions
                var files = Directory.EnumerateFiles(sourceDirectory, "*.png")
                    .Where(file => !file.EndsWith(".diff.png", StringComparison.OrdinalIgnoreCase)
                                   && !file.EndsWith(".expected.png", StringComparison.OrdinalIgnoreCase));

                foreach (var file in files)
                {
                    // Get filename
                    var fileName = Path.GetFileName(file);

                    // Define destination path
                    var destFile = Path.Combine(destinationDirectory, fileName);

                    // Copy file
                    File.Copy(file, destFile, true);
                }
            }
        }
    }
}
