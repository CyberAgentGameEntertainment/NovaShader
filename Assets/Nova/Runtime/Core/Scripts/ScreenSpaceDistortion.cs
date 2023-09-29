// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    [Serializable]
    public sealed class ScreenSpaceDistortion : ScriptableRendererFeature
    {
        private const string DistortionLightMode = "DistortedUvBuffer";

        [SerializeField] private bool _applyToSceneView = true;
        [SerializeField] [HideInInspector] private Shader _applyDistortionShader;
        private ApplyDistortionPass _applyDistortionPass;

        private DistortedUvBufferPass _distortedUvBufferPass;
        
    #if UNITY_2022_1_OR_NEWER
        private RTHandle _distortedUvBufferRTHandle;
    #endif

        public override void Create()
        {
            _applyDistortionShader = Shader.Find("Hidden/Nova/Particles/ApplyDistortion");
            if (_applyDistortionShader == null) return;

            _distortedUvBufferPass = new DistortedUvBufferPass(DistortionLightMode);
            _applyDistortionPass = new ApplyDistortionPass(_applyToSceneView, _applyDistortionShader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_applyDistortionShader == null 
                || renderingData.cameraData.cameraType == CameraType.Reflection
                || renderingData.cameraData.cameraType == CameraType.Preview)
                return;

            var distortedUvBufferFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf)
                ? RenderTextureFormat.RGHalf
                : RenderTextureFormat.DefaultHDR;
            
        #if UNITY_2022_1_OR_NEWER
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.colorFormat = distortedUvBufferFormat;
            RenderingUtils.ReAllocateIfNeeded(ref _distortedUvBufferRTHandle, desc);
            _distortedUvBufferPass.Setup(_distortedUvBufferRTHandle);
            _applyDistortionPass.Setup(_distortedUvBufferRTHandle);
        #else
            var cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            var distortedUvBuffer = RenderTexture.GetTemporary(cameraTargetDescriptor.width,
                cameraTargetDescriptor.height, 0, distortedUvBufferFormat, RenderTextureReadWrite.Default,
                cameraTargetDescriptor.msaaSamples);
            var distortedUvBufferIdentifier = new RenderTargetIdentifier(distortedUvBuffer);
            _distortedUvBufferPass.Setup(distortedUvBufferIdentifier);
            _applyDistortionPass.Setup(distortedUvBufferIdentifier);
        #endif
            renderer.EnqueuePass(_distortedUvBufferPass);
            renderer.EnqueuePass(_applyDistortionPass);
        #if !UNITY_2022_1_OR_NEWER
            RenderTexture.ReleaseTemporary(distortedUvBuffer);
        #endif
        }
    }
}