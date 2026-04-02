// --------------------------------------------------------------
// Copyright 2026 CyberAgent, Inc.
// --------------------------------------------------------------

namespace Tests.Runtime.Internal
{
    /// <summary>
    ///     Settings for image comparison in graphics regression tests.
    /// </summary>
    internal class ImageComparisonSettings
    {
        public int TargetWidth { get; set; }
        public int TargetHeight { get; set; }
        public int TargetMSAASamples { get; set; } = 1;
        public bool UseHDR { get; set; } = false;
        public float AverageCorrectnessThreshold { get; set; }
        public float PerPixelCorrectnessThreshold { get; set; }
        public float IncorrectPixelsThreshold { get; set; }
    }
}
