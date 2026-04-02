// --------------------------------------------------------------
// Copyright 2026 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tests.Runtime.Internal
{
    /// <summary>
    ///     Represents the platform and architecture of the test environment.
    /// </summary>
    internal class TestPlatform
    {
        private readonly Architecture architecture;
        private readonly RuntimePlatform platform;

        public Architecture Arch => architecture;
        public RuntimePlatform Platform => platform;

        public TestPlatform(RuntimePlatform platform, Architecture architecture)
        {
            this.platform = platform;
            this.architecture = architecture;
        }

        public static TestPlatform GetCurrent()
        {
            var currentPlatform = Application.platform;
            var currentArchitecture = RuntimeInformation.OSArchitecture;

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            // Apple M1 processor is ARM64 but does not appear as such in
            // RuntimeInformation.OSArchitecture on some testing environments.
            if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                    SystemInfo.processorType, "Apple M", CompareOptions.IgnoreCase) >= 0)
                currentArchitecture = Architecture.Arm64;
#endif

            return new TestPlatform(currentPlatform, currentArchitecture);
        }

        public override string ToString() => platform.ToUniqueString(architecture);
    }
}
