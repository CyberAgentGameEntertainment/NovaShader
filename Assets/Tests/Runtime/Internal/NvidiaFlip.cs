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
    ///     Class providing image comparison functionality using NVIDIA's Flip tool
    /// </summary>
    internal static class NvidiaFlip
    {
        class ParseException : Exception
        {
            public ParseException(string message)
                : base(message){}
        }
        /// <summary>
        ///     Execute image comparison using Flip
        /// </summary>
        /// <param name="referenceImagePath"></param>
        /// <param name="testImagePath"></param>
        /// <returns>Execution results from flip</returns>
        public static Result Execute(string referenceImagePath, string testImagePath)
        {
            Result result = null;
            // Flip executable (full path may be required depending on environment)
            // string command = "/Library/Frameworks/Python.framework/Versions/3.13/bin/flip";  // Replace with "./flip" or "C:/Tools/flip.exe" for example
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            #if UNITY_EDITOR_WIN
            var command = $"{projectRoot}/Assets/Tests/bin/Windows/flip"; // Replace with "./flip" or "C:/Tools/flip.exe" for example
            #elif UNITY_EDITOR_OSX
            var command = $"{projectRoot}/Assets/Tests/bin/Mac/flip";  // Replace with "./flip" or "C:/Tools/flip.exe" for example
            #else
            var command = $"{projectRoot}/Assets/Tests/bin/flip"
            #endif
            // Image file paths - when placed in the "Assets/StreamingAssets/" folder of Unity project

            referenceImagePath = Path.Combine(projectRoot, referenceImagePath);
            testImagePath = Path.Combine(projectRoot, testImagePath);
            // Command line arguments
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
                Debug.LogWarning($"flip result parse error: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to run flip command. Please read Documentation~/TestDocuments.md for required environment setup instructions.");
                Debug.LogError($"Error running flip command: {e.Message}");
                throw;
            }
            return result;
        }
        private static Result ParseFlipOutput(string output)
        {
            var result = new Result();

            // Match each line with regex (key: value format)
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
        /// Results from Flip execution
        /// </summary>
        public class Result
        {
            public float firstQuartile; // First weighted quartile. Indicates that the bottom 25% of the values in the difference map are below this value (weighted).
            public float max; // The largest difference value in the difference map.
            public float mean; // Weighted average value of the difference map.
            public float min; // The smallest difference value in the difference map.
            public float thirdQuartile; // Third weighted quartile. Indicates that the bottom 75% of the values in the difference map are below this value (weighted).
            public float weightedMedian; // Weighted median. The middle value when pixel-by-pixel difference values in the difference map are arranged in ascending order.
        }
    }
}
