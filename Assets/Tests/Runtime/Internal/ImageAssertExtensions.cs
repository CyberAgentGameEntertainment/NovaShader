    // --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

namespace Tests.Runtime.Internal
{
    /// <summary>
    ///     ImageAssertの拡張メソッド
    /// </summary>
    internal static class ImageAssertExtensions
    {
        
        private static void OnTestFailed(Texture2D actualImage, Texture2D expectedImage, Texture2D heatmapImage)
        {
            var actualImagePath = "Assets/ActualImages";
            var currentTestResultsFolderPath = TestUtils.GetCurrentTestResultsFolderPath();
            var dirName = Path.Combine(actualImagePath, currentTestResultsFolderPath);

            var imageName = TestContext.CurrentContext.Test.MethodName != null ? TestContext.CurrentContext.Test.Name : "NoName";
            var imageMessage = new ImageMessage
            {
                PathName = dirName,
                ImageName = TestUtility.StripParametricTestCharacters(imageName)
            };
            // テスト画像、リファレンス画像、ヒートマップを保存
            imageMessage.ActualImage = actualImage.EncodeToPNG();
            imageMessage.DiffImage = heatmapImage.EncodeToPNG();
            imageMessage.ExpectedImage = expectedImage.EncodeToPNG();
            var importSettings = new ImageHandler.TextureImporterSettings();
            ImageHandler.instance.SaveImage(imageMessage, false, importSettings);
        }
        public static void AreEqualWithFlip(Texture2D actualImage, Texture2D expectedImage, ImageComparisonSettings settings)
        {
            var actualImageBytes = actualImage.EncodeToPNG();
            File.WriteAllBytes("Assets/Tests/actual.png", actualImageBytes);
            var actualPath = "Assets/Tests/actual.png";
            var expectedImageBytes = expectedImage.EncodeToPNG();
            File.WriteAllBytes("Assets/Tests/expected.png", expectedImageBytes);
            var expectedPath = "Assets/Tests/expected.png";
            var heatmapFilePath =
                $"flip.{Path.GetFileNameWithoutExtension(expectedPath)}.actual.67ppd.ldr.png";
            
            try
            {
                var result = NvidiaFlip.Execute(expectedPath, actualPath);
                Assert.Less(result.mean, settings.AverageCorrectnessThreshold);
            }
            catch (Exception e)
            {
                OnTestFailed(actualImage, expectedImage, TestUtility.LoadImage(Path.GetFullPath(heatmapFilePath)));
                throw;
            }
            finally
            {
                // 一時ファイルを削除
                if (File.Exists(actualPath))
                    File.Delete(actualPath);
                if(File.Exists(actualPath))
                    File.Delete(actualPath);
                if (File.Exists(heatmapFilePath))
                    File.Delete(heatmapFilePath);
            }
            
            
        }

        /// <summary>
        ///     Flip画像比較を行うメソッド
        /// </summary>
        /// <param name="actualPath">実際の画像のファイルパス</param>
        /// <param name="cameras"></param>
        /// <param name="settings">比較設定</param>
        public static void AreEqualWithFlip(Texture2D actualImage, ImageComparisonSettings settings)
        {
            // テストイメージをキャプチャして一時ファイルとして保存する
            var bytes = actualImage.EncodeToPNG();
            File.WriteAllBytes("Assets/Tests/actual.png", bytes);
            var actualPath = "Assets/Tests/actual.png";
            var expectedPath = TestUtility.GetExpectedImageRelativePath();
            var heatmapFilePath =
                $"flip.{Path.GetFileNameWithoutExtension(expectedPath)}.actual.67ppd.ldr.png";
            try
            {
                var result = NvidiaFlip.Execute(expectedPath, actualPath);
                Assert.Less(result.mean, settings.AverageCorrectnessThreshold);
            }
            catch (Exception e)
            {
                OnTestFailed(actualImage, TestUtility.LoadImage(expectedPath), 
                    TestUtility.LoadImage(Path.GetFullPath(heatmapFilePath)));
                throw;
            }
            finally
            {
                // 一時ファイルを削除
                if (File.Exists(actualPath))
                    File.Delete(actualPath);
                // ヒートマップを削除
                if (File.Exists(heatmapFilePath))
                    File.Delete(heatmapFilePath);
            }
        }
    }
}
