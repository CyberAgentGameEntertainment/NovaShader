// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace Nova.Editor.Core.Scripts
{
    [Flags]
    public enum ParallaxMapTarget
    {
        None = 0,
        BaseMap = 1 << 0,
        TintMap = 1 << 1,
        EmissionMap = 1 << 2
    }

    public enum ParallaxMapMode
    {
        SingleTexture,
        FlipBook,
        FlipBookBlending,
    }
}
