// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_2023_3_OR_NEWER
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public partial class DistortedUvBufferPass : ScriptableRenderPass
    {
        private static readonly string DistortedUvBufferTexName = "DistortedUvBuffer";
        private TextureHandle _distortedUvBufferTHdl;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("NOVA.DistortedUvBufferPass",
                       out var passData))
            {
                var resourcesData = frameData.Get<UniversalResourceData>();
                _distortedUvBufferTHdl = CreateRenderTargets(renderGraph, frameData);
                builder.SetRenderAttachment(_distortedUvBufferTHdl, 0);
                builder.SetRenderAttachmentDepth(resourcesData.activeDepthTexture);

                // TODO: ApplyDistortionPass側でGlobalTextureを設定する
                // builder.SetGlobalTextureAfterPass(_distortedUvBufferTHdl, TransparentTexturePropID);

                RendererListHandle renderList;
                {
                    var renderingData = frameData.Get<UniversalRenderingData>();
                    var cameraData = frameData.Get<UniversalCameraData>();
                    var lightData = frameData.Get<UniversalLightData>();

                    var param = InitRendererListParams(_shaderTagId, renderingData, cameraData, lightData,
                        SortingCriteria.CommonTransparent, _filteringSettings);
                    renderList = renderGraph.CreateRendererList(param);
                }
                builder.UseRendererList(renderList);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    context.cmd.DrawRendererList(data.RendererList);
                });
            }
        }

        private static TextureHandle CreateRenderTargets(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var desc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
            desc.depthBufferBits = 0;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.R8G8_UNorm, GraphicsFormatUsage.Render))
                desc.colorFormat = GraphicsFormat.R8G8_UNorm;
            return renderGraph.CreateTexture(desc);
        }

        private static RendererListParams InitRendererListParams(
            ShaderTagId shaderTagId,
            UniversalRenderingData renderingData,
            UniversalCameraData cameraData,
            UniversalLightData lightData,
            SortingCriteria sortingCriteria,
            FilteringSettings filteringSettings)
        {
            var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagId, renderingData, cameraData, lightData,
                sortingCriteria);
            return new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);
        }

        private class PassData
        {
            internal Material Material;
            internal RendererListHandle RendererList;
        }
    }
}
#endif
