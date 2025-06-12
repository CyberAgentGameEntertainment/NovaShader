using Nova.Editor.Core.Scripts.Optimizer;
using Nova.Editor.Core.Scripts.Optimizer.Internal;
using UnityEditor;
using UnityEngine;

namespace Demo.Demo_OptimizedShader.Editor
{
    public class CustomBuild : MonoBehaviour
    {
        [MenuItem("Tools/Custom Build With Shader Optimize")]
        public static void Execute()
        {
            // Create optimized shaders
            OptimizedShaderGenerator.Generate("Assets/Demo/Demo_OptimizedShader/OptimizedShaders");
            
            // Create shader replacement settings
            var replaceSettings = new OptimizedShaderReplacer.Settings
            {
                OpaqueRequiredPasses = OptionalShaderPass.ShadowCaster,
                CutoutRequiredPasses = OptionalShaderPass.ShadowCaster,
                TransparentRequiredPasses = OptionalShaderPass.None,
                TargetFolderPath = "Assets/Demo/Demo_OptimizedShader/GeneratedMaterials",
            };
            // Replace uber shaders with optimized shaders
            OptimizedShaderReplacer.Replace(replaceSettings);

            // Configure build pipeline settings
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new string[] { "Assets/Demo/Demo_OptimizedShader/Demo_OptimizedShader.unity" },
                locationPathName = "Builds/Android/Demo.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development
            };

            // Execute build
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}