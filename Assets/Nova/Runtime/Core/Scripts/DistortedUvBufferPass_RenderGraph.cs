// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_2023_3_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public partial class DistortedUvBufferPass : ScriptableRenderPass
    {
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("NOVA.DistortedUvBufferPass",
                       out var passData))
            {
                var resourcesData = frameData.Get<UniversalResourceData>();
                builder.SetRenderAttachment(resourcesData.activeColorTexture, 0, AccessFlags.Write);
                builder.SetRenderAttachmentDepth(resourcesData.activeDepthTexture, AccessFlags.Write);
                
                
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
