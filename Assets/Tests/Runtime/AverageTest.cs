// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
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
            var asyncOp = EditorSceneManager.LoadSceneAsyncInPlayMode(
                $"Assets/Tests/Scenes/{scenePath}.unity",
                new LoadSceneParameters(LoadSceneMode.Single));
            // シーンの読み込み待ち
            while (!asyncOp.isDone) yield return null;
            // タイムスケールを0に指定しても、バインドポーズになるときもあれば、
            // 0フレームのアニメーションが再生されてしまうことがあり、テストが不安定だった。
            // そこでシーンに含まれているアニメーターを無効にしてアニメーションが再生されないようにする。
#if UNITY_2023_3_OR_NEWER
            var animators = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
#else
            var animators = Object.FindObjectsOfType<Animator>();
#endif

            foreach (var animator in animators) animator.enabled = false;

            // シーンのレンダリングが一回終わるまで待つ
            yield return new WaitForEndOfFrame();

            // 一回描画するとシェーダーの非同期コンパイルが走るので、コンパイルが終わるのを待つ
            while (ShaderUtil.anythingCompiling) yield return null;

            // さらにシーンのレンダリングが一回終わるまで待ってスクリーンショットを撮る
            yield return new WaitForEndOfFrame();
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
