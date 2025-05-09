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
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Graphics;
using Object = UnityEngine.Object;

namespace Tests.Runtime
{
    /// <summary>
    ///     アベレージテストを行うクラス
    ///     Jzazbz色空間でのピクセルの差分の平均値を使ってテストを行います。
    /// </summary>
    public class AverageTest
    {
        private IEnumerator LoadScene(string scenePath)
        {
            var asyncOp = EditorSceneManager.LoadSceneAsyncInPlayMode(
                $"Assets/Tests/Scenes/{scenePath}.unity",
                new LoadSceneParameters(LoadSceneMode.Single));
            // シーンの読み込み待ち
            while (!asyncOp.isDone) yield return null;
            // タイムスケールを0に指定しても、バインドポーズになるときもあれば、
            // 0フレームのアニメーションが再生されてしまうことがあり、テストが不安定だった。
            // そこでシーンに含まれているアニメーターを無効にしてアニメーションが再生されないようにする。
            var animators = Object.FindObjectsOfType<Animator>();
            foreach (var animator in animators) animator.enabled = false;

            // シーンのレンダリングが一回終わるまで待つ
            yield return new WaitForEndOfFrame();

            // 一回描画するとシェーダーの非同期コンパイルが走るので、コンパイルが終わるのを待つ
            while (ShaderUtil.anythingCompiling) yield return null;

            // シーンのレンダリングが一回終わるまで待つ
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        ///     指定されたカメラの描画結果をキャプチャーします。
        ///     キャプチャーの処理はTest FrameworkのImageAssert.AreEqualの実装を参考にしています。
        /// </summary>
        private static Texture2D CaptureActualImage(List<Camera> cameras, ImageComparisonSettings settings)
        {
            var width = settings.TargetWidth;
            var height = settings.TargetHeight;
            var samples = settings.TargetMSAASamples;
            var format = TextureFormat.ARGB32;
            Texture2D actualImage = null;
            var dummyRenderedFrameCount = 1;

            var defaultFormat = settings.UseHDR
                ? SystemInfo.GetGraphicsFormat(DefaultFormat.HDR)
                : SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            var desc = new RenderTextureDescriptor(width, height, defaultFormat, 24);
            desc.msaaSamples = samples;
            var rt = RenderTexture.GetTemporary(desc);
            Graphics.SetRenderTarget(rt);
            GL.Clear(true, true, Color.black);
            Graphics.SetRenderTarget(null);

            for (var i = 0;
                 i < dummyRenderedFrameCount + 1;
                 i++) // x frame delay + the last one is the one really tested ( ie 5 frames delay means 6 frames are rendered )
            {
                foreach (var camera in cameras)
                {
                    if (camera == null)
                        continue;
                    camera.targetTexture = rt;
                    camera.Render();
                    camera.targetTexture = null;
                }

                // only proceed the test on the last rendered frame
                if (dummyRenderedFrameCount == i)
                {
                    actualImage = new Texture2D(width, height, format, false);
                    RenderTexture dummy = null;

                    if (settings.UseHDR)
                    {
                        desc.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
                        dummy = RenderTexture.GetTemporary(desc);
                        Graphics.Blit(rt, dummy);
                    }
                    else
                    {
                        RenderTexture.active = rt;
                    }

                    actualImage.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    RenderTexture.active = null;

                    if (dummy != null)
                        RenderTexture.ReleaseTemporary(dummy);

                    actualImage.Apply();
                }
            }

            return actualImage;
        }

        [TestCase("Test_Unlit", ExpectedResult = null)]
        [TestCase("Test_Lit", ExpectedResult = null)]
        [TestCase("Test_UIParticleUnlit", ExpectedResult = null)]
        [TestCase("Test_UIParticleLit", ExpectedResult = null)]
        [TestCase("Test_UIParticleMask", ExpectedResult = null)]
        [TestCase("Test_Distortion", ExpectedResult = null)]
        [TestCase("Test_Vertex_Deformation", ExpectedResult = null)]
        [GameViewResolution(1920, 1080, "Full HD")]
        public IEnumerator Test(string scenePath)
        {
            yield return LoadScene(scenePath);

            var screenshotSrc = ScreenCapture.CaptureScreenshotAsTexture();

            var settings = new ImageComparisonSettings
            {
                TargetWidth = Screen.width,
                TargetHeight = Screen.height,
                AverageCorrectnessThreshold = 0.005f,
                PerPixelCorrectnessThreshold = 0.005f
            };
            var expected = ExpectedImage();

            // 成功イメージのフォーマットに合わせて再作成する。
            var screenshot = new Texture2D(expected.width, expected.height, expected.format, false);
            screenshot.SetPixels(screenshotSrc.GetPixels());
            screenshot.Apply();

            ImageAssert.AreEqual(expected, screenshot, settings);
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
            yield return LoadScene("Test_OptimizedShader");
            var expected = CaptureActualImage(new List<Camera> { Camera.main }, settings);
            
            // 最適化シェーダーを作成して差し替える
            OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
            var optimizedMaterialsPath = "Assets/Tests/Scenes/Materials/Optimized";
            // Get all materials in the Optimized folder
            var materials = AssetDatabase.FindAssets("t:Material", new[] { optimizedMaterialsPath} )
                .Select(guid => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            // Store original shader and render queue settings for each material
            var originalSettings = materials.Select(material => new
            {
                Material = material,
                Shader = material.shader,
                RenderQueue = material.renderQueue
            }).ToList();
            // テスト用のマテリアルのシェーダーを最適化シェーダーに置き換える
            OptimizedShaderReplacer.Replace(new OptimizedShaderReplacer.Settings
            {
                OpaqueRequiredPasses = OptionalShaderPass.DepthOnly | OptionalShaderPass.DepthNormals | OptionalShaderPass.ShadowCaster,
                CutoutRequiredPasses = OptionalShaderPass.ShadowCaster,
                TransparentRequiredPasses = OptionalShaderPass.None,
                TargetFolderPath = "Assets/Tests/Scenes/Materials/Optimized",
            });
            // 差し替え後でキャプチャする
            yield return LoadScene("Test_OptimizedShader");
            var actual = CaptureActualImage(new List<Camera> { Camera.main }, settings);
            ImageAssert.AreEqual(expected, actual, settings);

            // Restore original shader and render queue settings for each material
            foreach (var setting in originalSettings)
            {
                setting.Material.shader = setting.Shader;
                setting.Material.renderQueue = setting.RenderQueue;
            }
            
            yield return null;
        }

        private Texture2D ExpectedImage()
        {
            Texture2D expected = null;
            var expectedFile = TestContext.CurrentTestExecutionContext.CurrentTest.Name
                .Replace('(', '_')
                .Replace(')', '_')
                .Replace("\"", "");

            var dirName = Path.Combine("Assets/Tests/SuccessfulImages", TestUtils.GetCurrentTestResultsFolderPath());
            var expectedPath = Path.GetFullPath(Path.Combine(
                dirName,
                $"{expectedFile}.png"));
            if (File.Exists(expectedPath))
            {
                var bytes = File.ReadAllBytes(Path.GetFullPath(expectedPath));
                expected = new Texture2D(Screen.width, Screen.height);
                expected.LoadImage(bytes);
            }
            else
            {
                // ダミーのテクスチャを作成
                expected = new Texture2D(Screen.width, Screen.height);
                for (var x = 0; x < Screen.width; x++)
                for (var y = 0; y < Screen.height; y++)
                    expected.SetPixel(x, y, Color.black);
            }

            return expected;
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
