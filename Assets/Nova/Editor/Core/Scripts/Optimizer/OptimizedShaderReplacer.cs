// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Linq;
using Nova.Editor.Core.Scripts.Optimizer.Internal;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Core.Scripts.Optimizer
{
    /// <summary>
    ///     Class for replacing Uber shader with optimized shader.
    /// </summary>
    public static class OptimizedShaderReplacer
    {
        private static readonly int RenderType = Shader.PropertyToID("_RenderType");

        /// <summary>
        ///     Replace Uber shader with optimized shader.
        /// </summary>
        /// <param name="settings">
        ///     Parameters for shader replacement.
        ///     <see cref="Settings" />
        /// </param>
        public static void Replace(Settings settings)
        {
            // Find all materials in the project
            // and filter them to only include those using the UberLit or UberUnlit shaders
            var materialPaths = string.IsNullOrEmpty(settings.TargetFolderPath)
                ? AssetDatabase.FindAssets("t:Material")
                : AssetDatabase.FindAssets("t:Material", new[] { settings.TargetFolderPath });

            var materials = materialPaths
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Material>)
                .Where(material => material != null &&
                                   (material.shader.name == "Nova/Particles/UberLit" ||
                                    material.shader.name == "Nova/Particles/UberUnlit"))
                .ToList();
            
            foreach (var material in materials)
            {
                var renderType = (RenderType)material.GetFloat(RenderType);
                OptionalShaderPass requiredPasses;
                if (renderType == Scripts.RenderType.Opaque) // Opaque
                    requiredPasses = settings.OpaqueRequiredPasses;
                else if (renderType == Scripts.RenderType.Transparent) // Transparent
                    requiredPasses = settings.TransparentRequiredPasses;
                else if (renderType == Scripts.RenderType.Cutout) // Cutoff
                    requiredPasses = settings.CutoutRequiredPasses;
                else
                    // Unknown render type, skip this material
                    continue;
                // Get the optimized shader name based on the material's shader name, render type and required passes
                var optimizedShaderName =
                    OptimizedShaderUtillity.GetOptimizedShaderName(material.shader.name, renderType, requiredPasses);

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
                    Debug.LogWarning($"Please run OptimizedShaderGenerator.Generate() to create optimized shaders.");
                }
            }
        }

        /// <summary>
        ///     Parameters for shader replacement.
        /// </summary>
        public class Settings
        {
            /// <summary>
            ///     Required shader passes for Opaque render type
            /// </summary>
            public OptionalShaderPass OpaqueRequiredPasses { get; set; }

            /// <summary>
            ///     Required shader passes for Transparent render type
            /// </summary>
            public OptionalShaderPass TransparentRequiredPasses { get; set; }

            /// <summary>
            ///     Required shader passes for Cutoff render type
            /// </summary>
            public OptionalShaderPass CutoutRequiredPasses { get; set; }

            /// <summary>
            ///     Folder containing materials to be replaced. If not specified, all materials will be targeted.
            /// </summary>
            public string TargetFolderPath { get; set; }
        }
    }
}
