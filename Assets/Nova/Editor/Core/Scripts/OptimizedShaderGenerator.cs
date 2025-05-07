// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Core.Scripts
{
    public class OptimizedShaderGenerator : AssetPostprocessor
    {
        // 対象Pass名一覧（case-insensitive）
        // 対象とする LightMode タグ値
        private static readonly string[] TargetLightModes =
        {
            "DepthOnly",
            "ShadowCaster",
            "DepthNormals"
        };

        // 除去対象の #pragma（正規表現）
        private static readonly Regex PragmaRegex = new Regex(
            @"^\s*#\spragma\s+shader_feature_local(?:_vertex|_fragment)?\s+(?:" +
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
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        // "Pass {" のパターン（改行を含んでいてもマッチする）
        private static readonly Regex PassStartRegex = new Regex(
            @"^\s*Pass\s*(//.*)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex LightModeRegex = new Regex(
            @"[""']?LightMode[""']?\s*=\s*[""']?(?<mode>\w+)[""']?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                if (!assetPath.EndsWith(".shader")) continue;

                var fullPath = Application.dataPath + assetPath.Substring("Assets".Length);
                var source = File.ReadAllText(fullPath);

                if (!IsTargetShader(assetPath, source)) continue;

                var original = source;
                var optimized = RemoveTargetKeywordsFromPasses(source);

                if (original != optimized)
                {
                    File.WriteAllText(fullPath, optimized);
                    Debug.Log($"Optimized shader keywords for: {assetPath}");
                    // シェーダーファイル名に(Optimized)を追加
                    var optimizedPath = fullPath.Replace(".shader", "(Optimized).shader");
                    File.WriteAllText(optimizedPath, optimized);
                    AssetDatabase.ImportAsset(optimizedPath);
                }
            }
        }

        // 対象ファイルパターン
        private static bool IsTargetShader(string path, string shaderText)
        {
            return shaderText.Contains("Nova/Particles/UberUnlit") || shaderText.Contains("Nova/Particles/UberLit");
        }

        private static string RemoveTargetKeywordsFromPasses(string shaderCode)
        {
            foreach (var passName in TargetLightModes) shaderCode = OptimizeShaderPass(shaderCode, passName);

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

        private static string OptimizeShaderPass(string shaderCode, string passName)
        {
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

                // Pass開始を見つけた → 次の { 探す
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

                if (!string.IsNullOrEmpty(lightMode) && Array.Exists(TargetLightModes, x => x.Equals(lightMode)))
                    // Debug.Log($"[OptimizeShaderPasses] Cleaning pass (LightMode = {lightMode})");
                    for (var k = startLine; k <= passEnd; k++)
                        if (PragmaRegex.IsMatch(lines[k]))
                        {
                            Debug.Log($"[OptimizeShaderPasses] Removed: {lines[k].Trim()} (in {lightMode})");
                            lines[k] = ""; // 削除対象を空行に
                        }

                i = passEnd + 1;
            }

            return string.Join("\n", lines);
        }
    }
}
