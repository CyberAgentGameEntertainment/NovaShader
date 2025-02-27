// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public partial class ApplyDistortionPass : ScriptableRenderPass
    {
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddUnsafePass<PassData>("NOVA.ApplyDistortionPass",
                       out var passData))
            {
                // builder.SetRenderFunc(static (PassData data, UnsafeGraphContext context) =>
                // {
                //     var cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                //     cmd.SetGlobalTexture(_mainTexPropertyId, source);
                //     cmd.SetGlobalTexture(_distortionBufferPropertyId, _distortedUvBufferIdentifier);
                //     Blitter.BlitCameraTexture(cmd, cocTex, cocTex,
                //         RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material,
                //         (int)Pass.CreateCocTexture);
                // });
            }
        }

        private class PassData
        {
        }
    }
}
#endif
