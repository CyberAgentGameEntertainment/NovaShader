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
        private const string DistortedUvBufferTexName = "DistortedUvBuffer";

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("NOVA.DistortedUvBufferPass",
                       out var passData))
            {
                var distortedUvTexture = CreateRenderTarget(renderGraph, frameData);
                // Insert data to be passed to ApplyDistortionPass.
                {
                    var contextItem = frameData.GetOrCreate<DistortionContextItem>();
                    contextItem.DistortedUvTexture = distortedUvTexture;
                }
                builder.SetRenderAttachment(distortedUvTexture, 0);

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
                passData.RendererList = renderList;

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                {
                    context.cmd.DrawRendererList(data.RendererList);
                });
            }
        }

        private static TextureHandle CreateRenderTarget(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var desc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
            desc.name = DistortedUvBufferTexName;
            desc.depthBufferBits = 0;
            desc.clearColor = Color.gray;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16_SFloat, GraphicsFormatUsage.Render))
                desc.colorFormat = GraphicsFormat.R16G16_SFloat;
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
            internal RendererListHandle RendererList;
        }
    }
}
#endif
