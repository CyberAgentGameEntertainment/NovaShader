// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tests.Runtime.Internal
{
    /// <summary>
    ///     NvidiaのFlipを利用する画像比較機能を提供するクラス
    /// </summary>
    internal static class NvidiaFlip
    {
        class ParseException : Exception
        {
            public ParseException(string message)
                : base(message){}
        }
        /// <summary>
        ///     Flipによる画像比較を実行する
        /// </summary>
        /// <param name="referenceImagePath"></param>
        /// <param name="testImagePath"></param>
        /// <returns>flipによる実行結果</returns>
        public static Result Execute(string referenceImagePath, string testImagePath)
        {
            Result result = null;
            // flip 実行ファイル（環境によってはフルパスを指定する必要があります）
            // string command = "/Library/Frameworks/Python.framework/Versions/3.13/bin/flip";  // 例えば "./flip" または "C:/Tools/flip.exe" に置き換える
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            #if UNITY_EDITOR_WIN
            var command = $"{projectRoot}/Assets/Tests/bin/Windows/flip"; // 例えば "./flip" または "C:/Tools/flip.exe" に置き換える
            #elif UNITY_EDITOR_OSX
            var command = $"{projectRoot}/Assets/Tests/bin/Mac/flip";  // 例えば "./flip" または "C:/Tools/flip.exe" に置き換える
            #else
            var command = $"{projectRoot}/Assets/Tests/bin/flip"
            #endif
            // 画像ファイルのパス - Unityプロジェクト内の "Assets/StreamingAssets/" フォルダに置いた場合

            referenceImagePath = Path.Combine(projectRoot, referenceImagePath);
            testImagePath = Path.Combine(projectRoot, testImagePath);
            // コマンドライン引数
            var args = $"-r \"{referenceImagePath}\" -t \"{testImagePath}\"";

            var psi = new ProcessStartInfo();
            psi.FileName = command;
            psi.Arguments = args;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;

            try
            {
                using var process = Process.Start(psi);
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                result = ParseFlipOutput(output);
                if (result.mean < 0)
                {
                    throw new ParseException("Parse Error");
                }
                if (!string.IsNullOrEmpty(error)) Debug.LogError("Flip Error: " + error);
            }
            catch (ParseException e)
            {
                Debug.LogWarning("flip result parse error: ");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError("Error running flip command: " + e.Message);
                throw;
            }
            return result;
        }
        private static Result ParseFlipOutput(string output)
        {
            var result = new Result();

            // 各行を正規表現でマッチ（キー: 値 の形式）
            result.mean = ParseKey(output, @"Mean:\s*([-+]?[0-9]*\.?[0-9]+)");
            result.weightedMedian = ParseKey(output, @"Weighted median:\s*([-+]?[0-9]*\.?[0-9]+)");
            result.firstQuartile = ParseKey(output, @"1st weighted quartile:\s*([-+]?[0-9]*\.?[0-9]+)");
            result.thirdQuartile = ParseKey(output, @"3rd weighted quartile:\s*([-+]?[0-9]*\.?[0-9]+)");
            result.min = ParseKey(output, @"Min:\s*([-+]?[0-9]*\.?[0-9]+)");
            result.max = ParseKey(output, @"Max:\s*([-+]?[0-9]*\.?[0-9]+)");

            return result;
        }
        private static float ParseKey(string input, string pattern)
        {
            var match = Regex.Match(input, pattern);
            if (match.Success && float.TryParse(match.Groups[1].Value, out var value))
                return value;
            Debug.LogWarning("Could not parse pattern: " + pattern);
            return -1f;
        }
        /// <summary>
        /// Flipの実行結果
        /// </summary>
        public class Result
        {
            public float firstQuartile; // 第1重み付き四分位数。差分マップの値の下位25%がこの値以下であることを示します。（重み付き）
            public float max;　// 差分マップにおける最も大きい差分値。
            public float mean; // 差分マップの重み付き平均値。
            public float min; // 差分マップにおける最も小さい差分値。
            public float thirdQuartile; // 第3重み付き四分位数。 差分マップの値の下位75%がこの値以下であることを示します（重み付き）
            public float weightedMedian; // // 重み付き中央値。差分マップにおけるピクセルごとの差分値を小さい順に並べたときの中央に位置する値。
        }
    }
}
