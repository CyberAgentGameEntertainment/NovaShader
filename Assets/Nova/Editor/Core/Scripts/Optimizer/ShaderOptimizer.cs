// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Linq;
using Nova.Editor.Core.Scripts.Optimizer.Internal;
using UnityEngine;

namespace Nova.Editor.Core.Scripts.Optimizer
{
    /// <summary>
    /// Optimize shaders
    /// </summary>
    public static class ShaderOptimizer
    {
        private static readonly int RenderType = Shader.PropertyToID("_RenderType");
        
        /// <summary>
        /// Parameters for shader optimization
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Required shader passes for Opaque render type
            /// </summary>
            public OptionalShaderPass OpaqueRequiredPasses { get; set; }

            /// <summary>
            /// Required shader passes for Transparent render type
            /// </summary>
            public OptionalShaderPass TransparentRequiredPasses { get; set; }

            /// <summary>
            /// Required shader passes for Cutoff render type
            /// </summary>
            public OptionalShaderPass CutoutRequiredPasses { get; set; }
            /// <summary>
            /// Output path for optimized shaders
            /// </summary>
            public string OutputPath { get; set; }
            /// <summary>
            /// Whether to generate optimized shaders
            /// </summary>
            /// <remarks>
            /// If you want to use the previously created optimized shaders, set this to false.
            /// </remarks>
            public bool GenerateOptimizedShader { get; set; } = true;
        }
        public static void GenerateOptimizedShader(string outputPath)
        {
            // Generate optimized shaders
            OptimizedShaderGenerator.Execute(outputPath);
        }
        /// <summary>
        /// Uberシェーダーを最適化されたシェーダーに差し替えます。
        /// </summary>
        /// <param name="parameters">
        ///     Parameters for shader optimization
        ///     <see cref="Parameters"/>
        /// </param>
        public static void ReplaceOptimizedShader(Parameters parameters)
        {
            if (parameters.GenerateOptimizedShader)
            {
                OptimizedShaderGenerator.Execute(parameters.OutputPath);
            }

            // Find all materials in the project
            // and filter them to only include those using the UberLit or UberUnlit shaders
            var materials = UnityEditor.AssetDatabase.FindAssets("t:Material")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Material>)
                .Where(material => material != null && 
                    (material.shader.name == "Nova/Particles/UberLit" || 
                     material.shader.name == "Nova/Particles/UberUnlit"))
                .ToList();
            foreach (var material in materials)
            {
                var renderType = (RenderType)material.GetFloat(RenderType);
                OptionalShaderPass requiredPasses;
                if (renderType == Scripts.RenderType.Opaque) // Opaque
                {
                    requiredPasses = parameters.OpaqueRequiredPasses;
                }
                else if (renderType == Scripts.RenderType.Transparent) // Transparent
                {
                    requiredPasses = parameters.TransparentRequiredPasses;
                }
                else if (renderType == Scripts.RenderType.Cutout) // Cutoff
                {
                    requiredPasses = parameters.CutoutRequiredPasses;
                }
                else
                {
                    // Unknown render type, skip this material
                    continue;
                }
                // Get the optimized shader name based on the material's shader name, render type and required passes
                var optimizedShaderName = OptimizedShaderUtillity.GetOptimizedShaderName(material.shader.name, renderType, requiredPasses);
                
                // Find the optimized shader by name and assign it to the material
                var optimizedShader = Shader.Find(optimizedShaderName);
                if (optimizedShader != null)
                {
                    var renderQueue = material.renderQueue;
                    material.shader = optimizedShader;
                    material.renderQueue = renderQueue;
                }
                else 
                {
                    Debug.LogWarning($"Could not find optimized shader: {optimizedShaderName}");
                }
            }
        }
    }
}
