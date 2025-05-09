// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace Nova.Editor.Core.Scripts.Optimizer
{   
    [Flags]
    public enum OptionalShaderPass
    {
        None = 0,
        DepthOnly = 1,
        DepthNormals = 1 << 1,
        ShadowCaster = 1 << 2
    }
}
