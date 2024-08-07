// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public sealed class DistortedUvBufferPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(DistortedUvBufferPass);
        private readonly ProfilingSampler _profilingSampler = new(ProfilerTag);
        private readonly RenderQueueRange _renderQueueRange = RenderQueueRange.all;
        private readonly ShaderTagId _shaderTagId;
        private FilteringSettings _filteringSettings;

#if UNITY_2022_1_OR_NEWER
        private RTHandle _renderTargetRTHandle;
#else
        private RenderTargetIdentifier _renderTargetIdentifier;
#endif

        public DistortedUvBufferPass(string lightMode)
        {
            _filteringSettings = new FilteringSettings(_renderQueueRange);
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            _shaderTagId = new ShaderTagId(lightMode);
        }

#if UNITY_2022_1_OR_NEWER
        public void Setup(RTHandle renderTargetRTHandle)
        {
            _renderTargetRTHandle = renderTargetRTHandle;
        }
#else
        public void Setup(RenderTargetIdentifier renderTargetIdentifier)
        {
            _renderTargetIdentifier = renderTargetIdentifier;
        }
#endif

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var renderer = renderingData.cameraData.renderer;
#if UNITY_2022_1_OR_NEWER
            ConfigureTarget(_renderTargetRTHandle, renderer.cameraDepthTargetHandle);
#else
            ConfigureTarget(_renderTargetIdentifier, renderer.cameraDepthTarget);
#endif
            ConfigureClear(ClearFlag.Color, Color.gray);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            { context.ExecuteCommandBuffer(cmd);
              cmd.Clear();

              var drawingSettings =
                  CreateDrawingSettings(_shaderTagId, ref renderingData, SortingCriteria.CommonTransparent);

#if UNITY_2023_1_OR_NEWER
                var param = new RendererListParams(renderingData.cullResults, drawingSettings, _filteringSettings);
                var renderList = context.CreateRendererList(ref param);
                cmd.DrawRendererList(renderList);
#else
              context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
#endif
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
