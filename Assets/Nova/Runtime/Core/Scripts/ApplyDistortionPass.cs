// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public partial class ApplyDistortionPass : ScriptableRenderPass
    {
        private const string RenderPassName = nameof(ApplyDistortionPass);
        private readonly bool _applyToSceneView;
        private readonly int _distortionBufferPropertyId = Shader.PropertyToID("_ScreenSpaceUvTexture");
        private readonly int _mainTexPropertyId = Shader.PropertyToID("_MainTex");
        private readonly Material _material;
        private readonly ProfilingSampler _renderPassProfilingSampler;

        private RenderTargetIdentifier _distortedUvBufferIdentifier;

        public ApplyDistortionPass(bool applyToSceneView, Shader shader)
        {
            _applyToSceneView = applyToSceneView;
            _renderPassProfilingSampler = new ProfilingSampler(RenderPassName);
            _material = CoreUtils.CreateEngineMaterial(shader);
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void Setup(RenderTargetIdentifier distortedUvBufferIdentifier)
        {
            _distortedUvBufferIdentifier = distortedUvBufferIdentifier;
        }

#if UNITY_2023_3_OR_NEWER
        [Obsolete(DeprecationMessage.CompatibilityScriptingAPIObsolete, false)]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null) return;

            if (!_applyToSceneView && renderingData.cameraData.cameraType == CameraType.SceneView) return;

            var source = renderingData.cameraData.renderer.cameraColorTargetHandle.nameID;

            var cmd = CommandBufferPool.Get();
            cmd.Clear();
            using (new ProfilingScope(cmd, _renderPassProfilingSampler))
            {
                cmd.SetGlobalTexture(_mainTexPropertyId, source);
                cmd.SetGlobalTexture(_distortionBufferPropertyId, _distortedUvBufferIdentifier);
                Blit(cmd, ref renderingData, _material);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
