// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Linq;
using UnityEngine;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     Replaces uber shaders with optimized shaders
    /// </summary>
    public static class ReplaceOptimizedShader
    {
        private static readonly int RenderType = Shader.PropertyToID("_RenderType");

        public class Settings
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

        }
        public static void Execute(Settings settings)
        {
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
                    requiredPasses = settings.OpaqueRequiredPasses;
                }
                else if (renderType == Scripts.RenderType.Transparent) // Transparent
                {
                    requiredPasses = settings.TransparentRequiredPasses;
                }
                else if (renderType == Scripts.RenderType.Cutout) // Cutoff
                {
                    requiredPasses = settings.CutoutRequiredPasses;
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
                    material.shader = optimizedShader;
                }
                else 
                {
                    Debug.LogWarning($"Could not find optimized shader: {optimizedShaderName}");
                }
            }
        }
    }
}
