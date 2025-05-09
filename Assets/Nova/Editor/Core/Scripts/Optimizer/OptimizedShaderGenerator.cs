// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

// #define NOVA_USE_ASSET_POSTPROCESSOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Core.Scripts.Optimizer
{
    public static class OptimizedShaderGenerator
    #if NOVA_USE_ASSET_POSTPROCESSOR
        : AssetPostprocessor
    #endif
    {
        /// <summary>
        /// UberLit and UberUnlit shaders are optimized and saved in the specified output folder.
        /// </summary>
        /// <param name="outputFolderPath"></param>
        public static void Generate(string outputFolderPath)
        {
            // Find UberLit and UberUnlit shaders
            var uberShaderPaths = new[]
                {
                    Shader.Find("Nova/Particles/UberLit"),
                    Shader.Find("Nova/Particles/UberUnlit")
                }
                .Where(shader => shader != null)
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();
            // Get relative path from output directory to shader directory
            var uberShaderFolderPath = Path.GetDirectoryName(uberShaderPaths[0]);
            string relativePath;
            if (uberShaderFolderPath.Contains("Packages"))
            {
                // パッケージとしてインポートされているのでそのまま使う
                relativePath = uberShaderFolderPath;
            }
            else
            {
                // Assetsフォルダにインポートされている
                var outputFullPath = Path.GetFullPath(outputFolderPath);
                var shaderFullPath = Path.GetFullPath(uberShaderFolderPath);
                relativePath = Path.GetRelativePath(outputFullPath, shaderFullPath);
            }
            foreach (var assetPath in uberShaderPaths) Generate(outputFolderPath, assetPath, relativePath);
        }
        /// <summary>
        /// TODO: Experimental Function
        /// This feature will be enabled when NOVA_USE_ASSET_POSTPROCESSOR is activated.
        /// </summary>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                var outputDir = Path.GetDirectoryName(assetPath);
                outputDir = Path.Combine(outputDir, "OptimizedShaders");
                Generate(outputDir, assetPath, "../");
            }
        }

        private static void Generate(string outputFolderPath, string assetPath, string additionalIncludePath)
        {
            var source = File.ReadAllText(assetPath);

            if (!IsTargetShader(assetPath, source)) return;
            
            var requiredLightModes = new[]
            {
                OptionalShaderPass.None,
                OptionalShaderPass.DepthOnly,
                OptionalShaderPass.DepthOnly | OptionalShaderPass.DepthNormals,
                OptionalShaderPass.DepthOnly | OptionalShaderPass.ShadowCaster,
                OptionalShaderPass.DepthOnly | OptionalShaderPass.DepthNormals |
                OptionalShaderPass.ShadowCaster,
                OptionalShaderPass.DepthNormals,
                OptionalShaderPass.DepthNormals | OptionalShaderPass.ShadowCaster,
                OptionalShaderPass.ShadowCaster
            };

            var UsedPassNames = new[]
            {
                "None",
                "DepthOnly",
                "DepthOnly DepthNormals",
                "DepthOnly ShadowCaster",
                "DepthOnly DepthNormals ShadowCaster",
                "DepthNormals",
                "DepthNormals ShadowCaster",
                "ShadowCaster"
            };
            var renderTypeNames = new[]
            {
                "Opaque",
                "Transparent",
                "Cutout"
            };
            for (var renderType = 0; renderType < (int)RenderType.Num; renderType++)
            for (var lightMode = 0; lightMode < requiredLightModes.Length; lightMode++)
            {
                var postFixName = $"(Optimized {renderTypeNames[renderType]} {UsedPassNames[lightMode]})";
                if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);
                var optimizedPath = Path.Combine($"{outputFolderPath}/", Path.GetFileNameWithoutExtension(assetPath)
                                                                  + $"{postFixName}.shader");
                var optimizedSource = "";
                if (source.Contains("Nova/Particles/UberUnlit"))
                    optimizedSource = source.Replace("Nova/Particles/UberUnlit",
                        $"Hidden/Nova/Particles/UberUnlit{postFixName}");
                else if (source.Contains("Nova/Particles/UberLit"))
                    optimizedSource = source.Replace("Nova/Particles/UberLit",
                        $"Hidden/Nova/Particles/UberLit{postFixName}");


                optimizedSource =
                    OptimizeShader(optimizedSource, requiredLightModes[lightMode], (RenderType)renderType);
                // Since the output folder is OptimizedShaders, adjust the include paths accordingly
                optimizedSource = AdditionalIncludePath(optimizedSource, additionalIncludePath);
                File.WriteAllText(optimizedPath, optimizedSource);
                AssetDatabase.ImportAsset(optimizedPath);
            }
        }

        private static string AdditionalIncludePath(string shaderCode, string additionalIncludePath)
        {
            var regex = new Regex(@"#include\s+""([^""]+)""", RegexOptions.Multiline);
            return regex.Replace(shaderCode, match =>
            {
                var path = match.Groups[1].Value;
                return $"#include \"{additionalIncludePath}/{path}\"";
            });
        }

        private static bool IsTargetShader(string path, string shaderText)
        {
            var regex = new Regex(
                @"^\s*Shader\s+""Nova/Particles/(:?UberUnlit|UberLit)""\s*",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return regex.IsMatch(shaderText);
        }

        private static string OptimizeShader(string shaderCode, OptionalShaderPass requiredLightMode,
            RenderType renderType)
        {
            foreach (var passName in TargetLightModes)
                shaderCode = OptimizeShaderPass(
                    shaderCode, passName, requiredLightMode, renderType);
            return shaderCode;
        }

        private static int CountChar(string line, char c)
        {
            var count = 0;
            foreach (var ch in line)
                if (ch == c)
                    count++;
            return count;
        }

        private static string OptimizeShaderPass(string shaderCode, string passName,
            OptionalShaderPass requiredLightMode, RenderType renderType)
        {
            var result = "";
            var lines = new List<string>(shaderCode.Split('\n'));
            var lineCount = lines.Count;
            var i = 0;

            while (i < lineCount)
            {
                var line = lines[i];

                if (!PassStartRegex.IsMatch(line.Trim()))
                {
                    i++;
                    continue;
                }

                // Found Pass start -> Looking for next {
                var startLine = i;

                // Scan forward to find the first {
                while (i < lineCount && !lines[i].Contains("{")) i++;

                if (i >= lineCount)
                    break;

                var passBodyStart = i;
                var braceDepth = 0;
                var passEnd = -1;

                for (var j = passBodyStart; j < lineCount; j++)
                {
                    braceDepth += CountChar(lines[j], '{');
                    braceDepth -= CountChar(lines[j], '}');

                    if (braceDepth == 0)
                    {
                        passEnd = j;
                        break;
                    }
                }

                if (passEnd == -1)
                {
                    Debug.LogWarning("[OptimizeShaderPasses] Failed to parse Pass block: unmatched braces.");
                    break;
                }

                // Check LightMode inside this Pass
                string lightMode = null;
                for (var k = startLine; k <= passEnd; k++)
                {
                    var modeMatch = LightModeRegex.Match(lines[k]);
                    if (modeMatch.Success)
                    {
                        lightMode = modeMatch.Groups["mode"].Value;
                        break;
                    }
                }

                var enablePass = true;
                // Disable DepthOnly, ShadowCaster, and DepthNormals passes if they are not required
                if (lightMode == "DepthOnly")
                    enablePass = (requiredLightMode & OptionalShaderPass.DepthOnly) != 0;
                else if (lightMode == "ShadowCaster")
                    enablePass = (requiredLightMode & OptionalShaderPass.ShadowCaster) != 0;
                else if (lightMode == "DepthNormals")
                    enablePass = (requiredLightMode & OptionalShaderPass.DepthNormals) != 0;
                if (enablePass)
                {
                    // Remove Unused Keywords
                    if (!string.IsNullOrEmpty(lightMode)
                        && Array.Exists(TargetLightModes, x => x.Equals(lightMode))
                        && renderType == (int)RenderType.Opaque)
                        for (var k = startLine; k <= passEnd; k++)
                        {
                            var lineCache = lines[k];
                            if (!ShaderFeaturePragmaRegex.IsMatch(lineCache)) continue;
                            if (!ShaderKeywordRegex.IsMatch(lineCache)) continue;
                            lines[k] = "";
                        }
                }
                else
                {
                    for (var k = startLine; k <= passEnd; k++) lines[k] = "";
                }

                i = passEnd + 1;
            }

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                result += line + "\n";
            }

            return result;
        }

        private static readonly string[] TargetLightModes =
        {
            "DepthOnly",
            "ShadowCaster",
            "DepthNormals"
        };

        private static readonly Regex ShaderFeaturePragmaRegex = new(
            @"^\s*#pragma\s+shader_feature",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        private static readonly Regex ShaderKeywordRegex = new(
            @"(?:" +
            @"_VERTEX_ALPHA_AS_TRANSITION_PROGRESS\b|" +
            @"_ALPHAMODULATE_ENABLED\b|" +
            @"_ALPHATEST_ENABLED\b|" +
            @"_BASE_MAP_MODE_2D\b|" +
            @"_BASE_MAP_MODE_2D_ARRAY\b|" +
            @"_BASE_MAP_MODE_3D\b|" +
            @"_BASE_MAP_ROTATION_ENABLED\b|" +
            @"_BASE_SAMPLER_STATE_POINT_MIRROR\b|" +
            @"_BASE_SAMPLER_STATE_LINEAR_MIRROR\b|" +
            @"_BASE_SAMPLER_STATE_TRILINEAR_MIRROR\b|" +
            @"_TINT_AREA_ALL\b|" +
            @"_TINT_AREA_RIM\b|" +
            @"_TINT_COLOR_ENABLED\b|" +
            @"_TINT_MAP_ENABLED\b|" +
            @"_TINT_MAP_3D_ENABLED\b|" +
            @"_FLOW_MAP_ENABLED\b|" +
            @"_FLOW_MAP_TARGET_BASE\b|" +
            @"_FLOW_MAP_TARGET_TINT\b|" +
            @"_FLOW_MAP_TARGET_EMISSION\b|" +
            @"_FLOW_MAP_TARGET_ALPHA_TRANSITION\b|" +
            @"_PARALLAX_MAP_TARGET_BASE\b|" +
            @"_PARALLAX_MAP_TARGET_TINT\b|" +
            @"_PARALLAX_MAP_TARGET_EMISSION\b|" +
            @"_PARALLAX_MAP_MODE_2D\b|" +
            @"_PARALLAX_MAP_MODE_2D_ARRAY\b|" +
            @"_PARALLAX_MAP_MODE_3D\b|" +
            @"_GREYSCALE_ENABLED\b|" +
            @"_GRADIENT_MAP_ENABLED\b|" +
            @"_FADE_TRANSITION_ENABLED\b|" +
            @"_DISSOLVE_TRANSITION_ENABLED\b|" +
            @"_ALPHA_TRANSITION_MAP_MODE_2D\b|" +
            @"_ALPHA_TRANSITION_MAP_MODE_2D_ARRAY\b|" +
            @"_ALPHA_TRANSITION_MAP_MODE_3D\b|" +
            @"_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE\b|" +
            @"_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY\b|" +
            @"_EMISSION_AREA_ALL\b|" +
            @"_EMISSION_AREA_MAP\b|" +
            @"_EMISSION_AREA_ALPHA\b|" +
            @"_EMISSION_MAP_MODE_2D\b|" +
            @"_EMISSION_MAP_MODE_2D_ARRAY\b|" +
            @"_EMISSION_MAP_MODE_3D\b|" +
            @"_EMISSION_COLOR_COLOR\b|" +
            @"_EMISSION_COLOR_BASECOLOR\b|" +
            @"_EMISSION_COLOR_MAP\b|" +
            @"_TRANSPARENCY_BY_LUMINANCE\b|" +
            @"_TRANSPARENCY_BY_RIM\b|" +
            @"_SOFT_PARTICLES_ENABLED\b|" +
            @"_DEPTH_FADE_ENABLED\b" +
            @")\b.",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex PassStartRegex = new(
            @"^\s*Pass\s*(//.*)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex LightModeRegex = new(
            @"[""']?LightMode[""']?\s*=\s*[""']?(?<mode>\w+)[""']?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    }
}
