// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nova.Runtime.Core.Scripts
{
    public sealed class ApplyDistortionPass : ScriptableRenderPass
    {
        private const string RenderPassName = nameof(ApplyDistortionPass);
        private const string SrcToDestProfilingSamplerName = "SrcToDest";

        private readonly bool _applyToSceneView;
        private readonly int _distortionBufferPropertyId = Shader.PropertyToID("_ScreenSpaceUvTexture");
        private readonly int _mainTexPropertyId = Shader.PropertyToID("_MainTex");
        private readonly Material _material;
        private readonly ProfilingSampler _srcToDestProfilingSampler;
        private readonly ProfilingSampler _renderPassProfilingSampler;
        
        private ScriptableRenderer _renderer;
        private RenderTargetIdentifier _distortedUvBufferIdentifier;
        private RenderTargetHandle _tempRenderTargetHandle;

        public ApplyDistortionPass(bool applyToSceneView, Shader shader)
        {
            _applyToSceneView = applyToSceneView;
            _srcToDestProfilingSampler = new ProfilingSampler(SrcToDestProfilingSamplerName);
            _renderPassProfilingSampler = new ProfilingSampler(RenderPassName);
            _tempRenderTargetHandle.Init("_TempRT");
            _material = CoreUtils.CreateEngineMaterial(shader);
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void Setup(ScriptableRenderer renderer, RenderTargetIdentifier distortedUvBufferIdentifier)
        {
            _renderer = renderer;
            _distortedUvBufferIdentifier = distortedUvBufferIdentifier;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
            {
                return;
            }

            if (!_applyToSceneView && renderingData.cameraData.cameraType == CameraType.SceneView)
            {
                return;
            }
            
        #if UNITY_2022_1_OR_NEWER
            // todo:後で詳しくGUI周りを調査する
            // マテリアルGUI描画時、なぜかここがNullでエラーが出てしまうので、とりあえずNullチェックを入れる
            // ランタイム実行に影響がない
            if (renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null)
            {
                return;
            }
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle.nameID;
        #else
            var source = _renderer.cameraColorTarget;
        #endif

            var cmd = CommandBufferPool.Get();
            cmd.Clear();
            using (new ProfilingScope(cmd, _renderPassProfilingSampler))
            {
                var tempTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                tempTargetDescriptor.depthBufferBits = 0;
                cmd.GetTemporaryRT(_tempRenderTargetHandle.id, tempTargetDescriptor);

                using (new ProfilingScope(cmd, _srcToDestProfilingSampler))
                {
                    cmd.SetGlobalTexture(_mainTexPropertyId, source);
                    cmd.SetGlobalTexture(_distortionBufferPropertyId, _distortedUvBufferIdentifier);
                    Blit(cmd, source, _tempRenderTargetHandle.Identifier(), _material);
                }

                Blit(cmd, _tempRenderTargetHandle.Identifier(), source);
                cmd.ReleaseTemporaryRT(_tempRenderTargetHandle.id);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}