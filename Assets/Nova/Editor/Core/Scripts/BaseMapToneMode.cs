// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    /// Defines the tone mapping mode for the base map.
    /// </summary>
    public enum BaseMapToneMode
    {
        /// <summary>
        /// No tone mapping applied.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Three-tone mapping (Highlights, Midtones, Shadows).
        /// </summary>
        Tritone = 1,
        
        /// <summary>
        /// Five-tone mapping (Highlights, Brights, Midtones, Darktones, Shadows).
        /// </summary>
        Pentone = 2
    }
}