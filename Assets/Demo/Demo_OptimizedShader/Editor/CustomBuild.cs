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
            // 最適化シェーダーの生成
            
            
            // シェーダー置き換え設定
            

            // マテリアルのシェーダーの置き換えを実行
            

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