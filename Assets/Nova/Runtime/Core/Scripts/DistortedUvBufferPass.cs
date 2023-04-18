// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public sealed class DistortedUvBufferPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(DistortedUvBufferPass);
        private readonly ProfilingSampler _profilingSampler = new ProfilingSampler(ProfilerTag);
        private readonly RenderQueueRange _renderQueueRange = RenderQueueRange.all;
        private readonly ShaderTagId _shaderTagId;
        private Func<RenderTargetIdentifier> _getCameraDepthTargetIdentifier;
        private FilteringSettings _filteringSettings;

        private RenderTargetIdentifier _renderTargetIdentifier;

        public DistortedUvBufferPass(string lightMode)
        {
            _filteringSettings = new FilteringSettings(_renderQueueRange);
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            _shaderTagId = new ShaderTagId(lightMode);
        }

        public void Setup(RenderTargetIdentifier renderTargetIdentifier,
            Func<RenderTargetIdentifier> getCameraDepthTargetIdentifier)
        {
            _renderTargetIdentifier = renderTargetIdentifier;
            _getCameraDepthTargetIdentifier = getCameraDepthTargetIdentifier;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(_renderTargetIdentifier, _getCameraDepthTargetIdentifier.Invoke());
            ConfigureClear(ClearFlag.Color, Color.gray);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);

                var drawingSettings =
                    CreateDrawingSettings(_shaderTagId, ref renderingData, SortingCriteria.CommonTransparent);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
            }
        }
    }
}