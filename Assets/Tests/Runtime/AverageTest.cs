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
                AverageCorrectnessThreshold = 0.05f,
                PerPixelCorrectnessThreshold = 0.005f,
                IncorrectPixelsThreshold = 0.1f
            };
            // このタイミングでスクリーンショットを撮ればシーンの描画結果と一致するらしい
            var screenshotSrc = ScreenCapture.CaptureScreenshotAsTexture();
            var expected = TestUtility.ExpectedImage();
            // 成功イメージのフォーマットに合わせて再作成する。
            var screenshot = new Texture2D(expected.width, expected.height, expected.format, false);
            screenshot.SetPixels(screenshotSrc.GetPixels());
            screenshot.Apply();
            // Flipを使った画像比較
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
            // シェーダー差し替え前でキャプチャする
            yield return TestUtility.LoadScene($"Assets/Tests/Scenes/Test_OptimizedShader.unity");
            var expected = TestUtility.CaptureActualImage(new List<Camera> { Camera.main }, settings);
            
            // 最適化シェーダーを作成して差し替える
            OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
            var optimizedMaterialsPath = "Assets/Tests/Scenes/Materials/Optimized";
            // Get all materials in the Optimized folder
            var materials = AssetDatabase.FindAssets("t:Material", new[] { optimizedMaterialsPath} )
                .Select(guid => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            // Create a temporary folder for material copies
            var tempFolderPath = "Assets/Tests/Scenes/Materials/Temp";
            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }

            // Copy each material to temp folder
            foreach (var material in materials)
            {
                var originalPath = AssetDatabase.GetAssetPath(material);
                var fileName = Path.GetFileName(originalPath);
                var tempPath = Path.Combine(tempFolderPath, fileName);
                AssetDatabase.CopyAsset(originalPath, tempPath);
            }
         
            // テスト用のマテリアルのシェーダーを最適化シェーダーに置き換える
            OptimizedShaderReplacer.Replace(new OptimizedShaderReplacer.Settings
            {
                OpaqueRequiredPasses = OptionalShaderPass.DepthOnly | OptionalShaderPass.DepthNormals | OptionalShaderPass.ShadowCaster,
                CutoutRequiredPasses = OptionalShaderPass.ShadowCaster,
                TransparentRequiredPasses = OptionalShaderPass.None,
                TargetFolderPath = "Assets/Tests/Scenes/Materials/Optimized",
            });
            // 差し替え後でキャプチャする
            yield return TestUtility.LoadScene($"Assets/Tests/Scenes/Test_OptimizedShader.unity");
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
                // コピー元とコピー先のパスを定義
                var sourceDirectory = $"Assets/ActualImages/Linear/{platform}/None";
                var destinationDirectory = $"Assets/Tests/SuccessfulImages/Linear/{platform}/None";

                if (!Directory.Exists(sourceDirectory)) continue;
                // コピー先のディレクトリが存在しない場合は作成
                if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);

                // コピー元ディレクトリからファイルを取得し、条件に合うものをコピー
                var files = Directory.EnumerateFiles(sourceDirectory, "*.png")
                    .Where(file => !file.EndsWith(".diff.png", StringComparison.OrdinalIgnoreCase)
                                   && !file.EndsWith(".expected.png", StringComparison.OrdinalIgnoreCase));

                foreach (var file in files)
                {
                    // ファイル名を取得
                    var fileName = Path.GetFileName(file);

                    // コピー先のパスを定義
                    var destFile = Path.Combine(destinationDirectory, fileName);

                    // ファイルをコピー
                    File.Copy(file, destFile, true);
                }
            }
        }
    }
}
