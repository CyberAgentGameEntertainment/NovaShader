// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_2023_3_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public partial class ApplyDistortionPass : ScriptableRenderPass
    {
        private const string DistortedCameraColorTextureName = "DistortedCameraColorTexture";

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_material == null)
                return;
            if (!_applyToSceneView && frameData.Get<UniversalCameraData>().cameraType == CameraType.SceneView)
                return;

            if (!frameData.Contains<DistortionContextItem>())
            {
                Debug.LogError("[NOVA] Cannot execute ApplyDistortionPass. DistortedUvBufferPass must be executed.");
                return;
            }

            var resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle destTexture;
            {
                var desc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
                desc.name = DistortedCameraColorTextureName;
                destTexture = renderGraph.CreateTexture(desc);
            }

            using (var builder =
                   renderGraph.AddRasterRenderPass<PassData>("NOVA.ApplyDistortionPass", out var passData))
            {
                // DestTexture
                builder.SetRenderAttachment(destTexture, 0, AccessFlags.WriteAll);

                // MainTexture
                {
                    passData.MainTex = resourceData.activeColorTexture;
                    builder.UseTexture(passData.MainTex);
                    passData.MainTexPropertyId = _mainTexPropertyId;
                }

                // DistortedUvBuffer
                {
                    var contextItem = frameData.Get<DistortionContextItem>();
                    passData.DistortedUvBuffer = contextItem.DistortedUvTexture;
                    builder.UseTexture(passData.DistortedUvBuffer);
                    passData.DistortionBufferPropertyId = _distortionBufferPropertyId;
                }

                passData.Material = _material;

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                {
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
