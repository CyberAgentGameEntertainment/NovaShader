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
            // Create optimized shaders
            OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
            
            // Create shader replacement settings
            var replaceSettings = new OptimizedShaderReplacer.Settings
            {
                OpaqueRequiredPasses = OptionalShaderPass.ShadowCaster,
                CutoutRequiredPasses = OptionalShaderPass.ShadowCaster,
                TransparentRequiredPasses = OptionalShaderPass.None
            };
            // Replace uber shaders with optimized shaders
            OptimizedShaderReplacer.Replace(replaceSettings);

            // Configure build pipeline settings
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new string[] { "Assets/Demo/Demo00/Demo00.unity" },
                locationPathName = "Builds/Android/OptimizedShaderDemo.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development
            };

            // Execute build
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
        [MenuItem("Tools/NOVA Shader/Sample/Generate Optimized Shader")]
        public static void GenerateOptimizedShader()
        {
            OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
        }
    }
}
