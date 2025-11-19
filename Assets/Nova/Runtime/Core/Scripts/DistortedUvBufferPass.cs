// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_2023_3_OR_NEWER
using UnityEngine.Experimental.Rendering;
#endif

namespace Nova.Runtime.Core.Scripts
{
    public partial class DistortedUvBufferPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(DistortedUvBufferPass);
        private readonly ProfilingSampler _profilingSampler = new(ProfilerTag);
        private readonly RenderQueueRange _renderQueueRange = RenderQueueRange.all;
        private readonly ShaderTagId _shaderTagId;
        private FilteringSettings _filteringSettings;

        private RTHandle _renderTargetRTHandle;

#if UNITY_2023_3_OR_NEWER
        private readonly GraphicsFormat _colorFormat;
#endif

        public DistortedUvBufferPass(string lightMode)
        {
            _filteringSettings = new FilteringSettings(_renderQueueRange);
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            _shaderTagId = new ShaderTagId(lightMode);

#if UNITY_2023_3_OR_NEWER
            _colorFormat = SystemInfo.IsFormatSupported(GraphicsFormat.R16G16_SFloat, GraphicsFormatUsage.Render)
                ? GraphicsFormat.R16G16_SFloat
                : GraphicsFormat.None;
#endif
        }

        public void Setup(RTHandle renderTargetRTHandle)
        {
            _renderTargetRTHandle = renderTargetRTHandle;
        }

#if UNITY_2023_3_OR_NEWER
        [Obsolete(DeprecationMessage.CompatibilityScriptingAPIObsolete, false)]
#endif
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var renderer = renderingData.cameraData.renderer;
            ConfigureTarget(_renderTargetRTHandle, renderer.cameraDepthTargetHandle);
            ConfigureClear(ClearFlag.Color, Color.gray);
        }

#if UNITY_2023_3_OR_NEWER
        [Obsolete(DeprecationMessage.CompatibilityScriptingAPIObsolete, false)]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
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
