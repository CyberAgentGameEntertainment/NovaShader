// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public sealed class ApplyDistortionPass : ScriptableRenderPass
    {
        private const string RenderPassName = nameof(ApplyDistortionPass);
        private const string ProfilingSamplerName = "SrcToDest";

        private readonly bool _applyToSceneView;
        private readonly int _distortionBufferPropertyId = Shader.PropertyToID("_ScreenSpaceUvTexture");
        private readonly int _mainTexPropertyId = Shader.PropertyToID("_MainTex");
        private readonly Material _material;
        private readonly ProfilingSampler _profilingSampler;

        private RenderTargetIdentifier _cameraColorTarget;
        private RenderTargetIdentifier _distortedUvBufferIdentifier;
        private RenderTargetHandle _tempRenderTargetHandle;

        public ApplyDistortionPass(bool applyToSceneView, Shader shader)
        {
            _applyToSceneView = applyToSceneView;
            _profilingSampler = new ProfilingSampler(ProfilingSamplerName);
            _tempRenderTargetHandle.Init("_TempRT");
            _material = CoreUtils.CreateEngineMaterial(shader);
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void Setup(RenderTargetIdentifier cameraColorTarget, RenderTargetIdentifier distortedUvBufferIdentifier)
        {
            _cameraColorTarget = cameraColorTarget;
            _distortedUvBufferIdentifier = distortedUvBufferIdentifier;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
            {
                return;
            }

            if (!_applyToSceneView && renderingData.cameraData.cameraType == CameraType.SceneView)
            {
                return;
            }

            var cmd = CommandBufferPool.Get(RenderPassName);
            cmd.Clear();

            var source = _cameraColorTarget;
            var tempTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            tempTargetDescriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(_tempRenderTargetHandle.id, tempTargetDescriptor);

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cmd.SetGlobalTexture(_mainTexPropertyId, source);
                cmd.SetGlobalTexture(_distortionBufferPropertyId, _distortedUvBufferIdentifier);
                Blit(cmd, source, _tempRenderTargetHandle.Identifier(), _material);
            }

            Blit(cmd, _tempRenderTargetHandle.Identifier(), source);
            cmd.ReleaseTemporaryRT(_tempRenderTargetHandle.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}