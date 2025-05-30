// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Graphics;

namespace Tests.Runtime.Internal
{
    internal static class TestUtility
    {
        public static IEnumerator LoadScene(string scenePath)
        {
            var asyncOp = EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));
            // Wait for scene loading
            while (!asyncOp.isDone) yield return null;
            // Even when setting timeScale to 0, sometimes it goes to bind pose,
            // and sometimes the animation plays at frame 0, making the test unstable.
            // Therefore, disable all animators in the scene to prevent animation playback.
            var animators = Object.FindObjectsOfType<Animator>();
            foreach (var animator in animators) animator.enabled = false;

            // Wait for the scene to finish rendering once
            yield return new WaitForEndOfFrame();

            // Shader async compilation starts after first render, so wait for compilation to complete
            while (ShaderUtil.anythingCompiling) yield return null;
            
            // Wait for the scene to finish rendering once
            yield return new WaitForEndOfFrame();
            
        }
        /// <summary>
        ///     Captures the rendering result of the specified camera.
        ///     The capture process is based on the implementation of ImageAssert.AreEqual in the Test Framework.
        /// </summary>
        public static Texture2D CaptureActualImage(List<Camera> cameras, ImageComparisonSettings settings)
        {
            var width = settings.TargetWidth;
            var height = settings.TargetHeight;
            var samples = settings.TargetMSAASamples;
            var format = TextureFormat.ARGB32;
            Texture2D actualImage = null;
            var dummyRenderedFrameCount = 1;

            var defaultFormat = settings.UseHDR ? SystemInfo.GetGraphicsFormat(DefaultFormat.HDR) : SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            var desc = new RenderTextureDescriptor(width, height, defaultFormat, 24);
            desc.msaaSamples = samples;
            var rt = RenderTexture.GetTemporary(desc);
            Graphics.SetRenderTarget(rt);
            GL.Clear(true, true, Color.black);
            Graphics.SetRenderTarget(null);

            for (var i = 0; i < dummyRenderedFrameCount + 1; i++) // x frame delay + the last one is the one really tested ( ie 5 frames delay means 6 frames are rendered )
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
                        RenderTexture.active = rt;

                    actualImage.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    RenderTexture.active = null;

                    if (dummy != null)
                        RenderTexture.ReleaseTemporary(dummy);

                    actualImage.Apply();
                }
            }

            return actualImage;
        }
        public static string StripParametricTestCharacters(string name)
        {
            {
                var illegal = "\"";
                var found = name.IndexOf(illegal);
                while (found >= 0)
                {
                    name = name.Remove(found, 1);
                    found = name.IndexOf(illegal);
                }
            }
            {
                var illegal = ",";
                name = name.Replace(illegal, "-");
            }
            {
                var illegal = "(";
                name = name.Replace(illegal, "_");
            }
            {
                var illegal = ")";
                name = name.Replace(illegal, "_");
            }
            return name;
        }
        public static string GetExpectedImageRelativePath()
        {
            var expectedFile = TestContext.CurrentTestExecutionContext.CurrentTest.Name
                .Replace('(', '_')
                .Replace(')', '_')
                .Replace(',', '-')
                .Replace("\"", "");

            var dirName = Path.Combine("Assets/Tests/SuccessfulImages", TestUtils.GetCurrentTestResultsFolderPath());
            return Path.Combine(
                dirName,
                $"{expectedFile}.png");
        }
        public static Texture2D LoadImage(string filePath)
        {
            Texture2D expected = null;
            if (File.Exists(filePath))
            {
                var bytes = File.ReadAllBytes(Path.GetFullPath(filePath));
                expected = new Texture2D(Screen.width, Screen.height);
                expected.LoadImage(bytes);
            }
            else
            {
                // Create a dummy texture
                expected = new Texture2D(Screen.width, Screen.height);
                for (var x = 0; x < Screen.width; x++)
                for (var y = 0; y < Screen.height; y++)
                    expected.SetPixel(x, y, Color.black);
            }
            return expected;
        }
        public static Texture2D ExpectedImage()
        {
            return LoadImage(Path.GetFullPath(GetExpectedImageRelativePath()));
        }
    }
}
