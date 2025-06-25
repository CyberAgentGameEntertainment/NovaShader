using Nova.Editor.Core.Scripts.Optimizer;
using Nova.Editor.Core.Scripts.Optimizer.Internal;
using UnityEditor;
using UnityEngine;

namespace Demo.Demo_OptimizedShader.Editor
{
    public class Workshop_CustomBuild : MonoBehaviour
    {
        [MenuItem("Tools/Custom Build With Shader Optimize")]
        public static void Execute()
        {
            // step-3 ビルド時に最適化シェーダーを自動生成/置き換え
            
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
        [MenuItem("Tools/Generate Optimized Shader")]
        public static void GenerateOptimizedShader()
        {
            // step-1 最適化シェーダーを生成する
        }
        [MenuItem("Tools/Replace Optimized Shader")]
        public static void ReplaceOptimizedShader()
        {
            // step-2 最適化シェーダーに置き換える
        }
    }
}
