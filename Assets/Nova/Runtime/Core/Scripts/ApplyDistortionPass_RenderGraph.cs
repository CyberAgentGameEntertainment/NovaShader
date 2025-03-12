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
    public partial class ApplyDistortionPass : ScriptableRenderPass
    {
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle destTexture;
            {
                var desc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
                destTexture = renderGraph.CreateTexture(desc);
            }

            using (var builder =
                   renderGraph.AddRasterRenderPass<PassData>("NOVA.ApplyDistortionPass", out var passData))
            {
                // DestTexture
                builder.SetRenderAttachment(destTexture, 0);

                // MainTexture
                {
                    passData.MainTex = resourceData.activeColorTexture;
                    builder.UseTexture(passData.MainTex);
                    passData.MainTexPropertyId = _mainTexPropertyId;
                }

                // DistortedUvBuffer
                {
                    var desc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
                    desc.depthBufferBits = 0;
                    if (SystemInfo.IsFormatSupported(GraphicsFormat.R8G8_UNorm, GraphicsFormatUsage.Render))
                        desc.colorFormat = GraphicsFormat.R8G8_UNorm;
                    passData.DistortedUvBuffer = renderGraph.CreateTexture(desc);
                    builder.UseTexture(passData.DistortedUvBuffer);
                    passData.DistortionBufferPropertyId = _distortionBufferPropertyId;
                }

                passData.Material = _material;

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                {
                    var cmd = context.cmd;
                    data.Material.SetTexture(data.MainTexPropertyId, data.MainTex);
                    data.Material.SetTexture(data.DistortionBufferPropertyId, data.DistortedUvBuffer);
                    Blitter.BlitTexture(context.cmd, data.MainTex, Vector2.one, data.Material, 0);
                });
            }

            resourceData.cameraColor = destTexture;
        }

        private class PassData
        {
            internal TextureHandle DistortedUvBuffer;
            internal int DistortionBufferPropertyId;
            internal TextureHandle MainTex;
            internal int MainTexPropertyId;
            internal Material Material;
        }
    }
}
#endif
