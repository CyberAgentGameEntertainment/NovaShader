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

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();
            cmd.SetRenderTarget(_renderTargetIdentifier, _getCameraDepthTargetIdentifier.Invoke());
            cmd.ClearRenderTarget(false, true, Color.grey);

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