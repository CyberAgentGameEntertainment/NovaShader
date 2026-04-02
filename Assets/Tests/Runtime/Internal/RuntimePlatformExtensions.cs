// --------------------------------------------------------------
// Copyright 2026 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Runtime.InteropServices;
using UnityEngine;

namespace Tests.Runtime.Internal
{
    /// <summary>
    ///     Extension methods for the RuntimePlatform enum.
    /// </summary>
    internal static class RuntimePlatformExtensions
    {
        /// <summary>
        ///     Converts the RuntimePlatform to a unique string value.
        ///     Required to generate backward compatible unique string values
        ///     for duplicated RuntimePlatform enum values.
        /// </summary>
        public static string ToUniqueString(this RuntimePlatform platform, Architecture architecture)
        {
            var result = platform switch
            {
                RuntimePlatform.WSAPlayerX86 => "MetroPlayerX86",
                RuntimePlatform.WSAPlayerX64 => "MetroPlayerX64",
                RuntimePlatform.WSAPlayerARM => "MetroPlayerARM",
                _ => platform.ToString(),
            };

            if (architecture is Architecture.Arm64)
            {
                switch (platform)
                {
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.OSXEditor:
                        result += "_AppleSilicon";
                        break;
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        result += "_ARM64";
                        break;
                }
            }

            return result;
        }
    }
}
