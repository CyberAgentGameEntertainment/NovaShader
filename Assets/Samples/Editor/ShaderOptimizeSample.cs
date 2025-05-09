using Nova.Editor.Core.Scripts.Optimizer;
using Nova.Editor.Core.Scripts.Optimizer.Internal;
using UnityEditor;
using UnityEngine;

namespace Samples.Editor
{
    public class ShaderOptimizeSample : MonoBehaviour
    {
        [MenuItem("Tools/NOVA Shader/Sample/Custom Build With Shader Optimize")]
        public static void CustomBuild()
        {
            // シェーダーの最適化を実行
            var optimizeParameters = new ShaderOptimizer.Parameters
            {
                OpaqueRequiredPasses = OptionalShaderPass.ShadowCaster,
                CutoutRequiredPasses = OptionalShaderPass.ShadowCaster,
                TransparentRequiredPasses = OptionalShaderPass.None,
                OutputPath = "Assets/OptimizedShaders"
            };
            ShaderOptimizer.Execute(optimizeParameters);

            // ビルドパイプラインの設定
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new string[] { "Assets/Demo/Demo00/Demo00.unity" },
                locationPathName = "Builds/Android/OptimizedShaderDemo.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development
            };

            // ビルドの実行
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
        [MenuItem("Tools/NOVA Shader/Sample/Generate Optimized Shader")]
        public static void GenerateOptimizedShader()
        {
            OptimizedShaderGenerator.Execute("Assets/OptimizedShaders");
        }
    }
}
