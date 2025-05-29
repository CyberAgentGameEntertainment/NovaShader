// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------
//
// This file uses the flip tool from NVIDIA CORPORATION & AFFILIATES
// which is licensed under the BSD 3-Clause License.
// See Assets/Tests/LICENSE.md for the full license text.
// SPDX-License-Identifier: BSD-3-Clause
// --------------------------------------------------------------

using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

namespace Tests.Runtime.Internal
{
    /// <summary>
    ///     Extension methods for ImageAssert
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
            // Save test image, reference image, and heatmap
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
            catch (Exception)
            {
                OnTestFailed(actualImage, expectedImage, TestUtility.LoadImage(Path.GetFullPath(heatmapFilePath)));
                throw;
            }
            finally
            {
                // Delete temporary files
                if (File.Exists(actualPath))
                    File.Delete(actualPath);
                if(File.Exists(actualPath))
                    File.Delete(actualPath);
                if (File.Exists(heatmapFilePath))
                    File.Delete(heatmapFilePath);
            }
            
            
        }

        /// <summary>
        ///     Method to perform Flip image comparison
        /// </summary>
        /// <param name="actualPath">File path of the actual image</param>
        /// <param name="cameras"></param>
        /// <param name="settings">Comparison settings</param>
        public static void AreEqualWithFlip(Texture2D actualImage, ImageComparisonSettings settings)
        {
            // Capture test image and save as temporary file
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
            catch (Exception)
            {
                OnTestFailed(actualImage, TestUtility.LoadImage(expectedPath), 
                    TestUtility.LoadImage(Path.GetFullPath(heatmapFilePath)));
                throw;
            }
            finally
            {
                // Delete temporary files
                if (File.Exists(actualPath))
                    File.Delete(actualPath);
                // Delete heatmap
                if (File.Exists(heatmapFilePath))
                    File.Delete(heatmapFilePath);
            }
        }
    }
}
