// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    [Serializable]
    public sealed class ScreenSpaceDistortion : ScriptableRendererFeature
    {
        private const string DistortionLightMode = "DistortedUvBuffer";

        [SerializeField] private bool _applyToSceneView = true;
        [SerializeField] [HideInInspector] private Shader _applyDistortionShader;
        private ApplyDistortionPass _applyDistortionPass;

        private DistortedUvBufferPass _distortedUvBufferPass;

        public override void Create()
        {
            _applyDistortionShader = Shader.Find("Hidden/Nova/Particles/ApplyDistortion");
            if (_applyDistortionShader == null)
            {
                return;
            }

            _distortedUvBufferPass = new DistortedUvBufferPass(DistortionLightMode);
            _applyDistortionPass = new ApplyDistortionPass(_applyToSceneView, _applyDistortionShader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_applyDistortionShader == null)
            {
                return;
            }

            var cameraTargetDesciptor = renderingData.cameraData.cameraTargetDescriptor;
            var distortedUvBufferFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf)
                ? RenderTextureFormat.RGHalf
                : RenderTextureFormat.DefaultHDR;
            var distortedUvBuffer = RenderTexture.GetTemporary(cameraTargetDesciptor.width,
                cameraTargetDesciptor.height, 0, distortedUvBufferFormat);
            var distortedUvBufferIdentifier = new RenderTargetIdentifier(distortedUvBuffer);

            _distortedUvBufferPass.Setup(distortedUvBufferIdentifier, () => renderer.cameraDepthTarget);
            _applyDistortionPass.Setup(renderer.cameraColorTarget, distortedUvBufferIdentifier);
            renderer.EnqueuePass(_distortedUvBufferPass);
            renderer.EnqueuePass(_applyDistortionPass);
            RenderTexture.ReleaseTemporary(distortedUvBuffer);
        }
    }
}