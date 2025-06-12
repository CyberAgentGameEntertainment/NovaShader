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
            // step-1 最適化シェーダーの生成
            
            
            // step-2 シェーダー置き換え設定
            

            // step-3 マテリアルのシェーダーの置き換えを実行
            

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