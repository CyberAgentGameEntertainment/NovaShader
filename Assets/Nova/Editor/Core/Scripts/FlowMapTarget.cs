// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace Nova.Editor.Core.Scripts
{
    [Flags]
    public enum FlowMapTarget
    {
        None = 0,
        BaseMap = 1 << 0,
        TintMap = 1 << 1,
        AlphaTransitionMap = 1 << 2,
        EmissionMap = 1 << 3
    }

    [Flags]
    public enum FlowMapTargetDistortion
    {
        None = 0,
        BaseMap = 1 << 0,
        AlphaTransitionMap = 1 << 1,
    }
}