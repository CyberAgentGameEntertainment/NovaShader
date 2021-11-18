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
        private readonly ShaderTagId _shaderTagId = new ShaderTagId("DistortedUvBuffer"); // 上書きできるように
        private RenderTargetIdentifier _cameraColorTargetIdentifier;
        private FilteringSettings _filteringSettings;

        private RenderTargetIdentifier _renderTargetIdentifier;

        public DistortedUvBufferPass()
        {
            _filteringSettings = new FilteringSettings(_renderQueueRange);
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void Setup(RenderTargetIdentifier renderTargetIdentifier, RenderTargetIdentifier cameraColorTargetIdentifier)
        {
            _renderTargetIdentifier = renderTargetIdentifier;
            _cameraColorTargetIdentifier = cameraColorTargetIdentifier;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(ProfilerTag);
            cmd.Clear();
            cmd.SetRenderTarget(_renderTargetIdentifier, _cameraColorTargetIdentifier);
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