# NOVA Shader Custom Coord System Technical Documentation

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->
<!-- EDITING REMINDER: Before editing, read CLAUDE.md Documentation Editing Protocol -->
<!-- PROHIBITED: Troubleshooting sections, Checklists, Version history, Debug steps -->
<!-- FOCUS: Current implementation state, Technical specifications with code examples -->

## Table of Contents

1. [Overview](#overview)
2. [System Architecture](#system-architecture)
3. [Custom Coord Types and Definitions](#custom-coord-types-and-definitions)
4. [HLSL Implementation Details](#hlsl-implementation-details)
5. [GPU Instancing Support](#gpu-instancing-support)
6. [Editor Integration](#editor-integration)
7. [Feature-Specific Custom Coord Usage](#feature-specific-custom-coord-usage)
8. [Validation and Error Handling](#validation-and-error-handling)
9. [Performance Considerations](#performance-considerations)
10. [Extension Guidelines](#extension-guidelines)

---

## Overview

The NOVA Shader Custom Coord system provides a mechanism for delivering independent parameters per particle. It supports Unity's standard Particle System Custom1/Custom2 streams and can be used as animation parameters in over 30 features.

### Key Features

- **Type Safety**: Type-safe implementation based on C# enums
- **GPU Instancing Support**: High-performance instancing rendering
- **Flexible Integration**: Compatible with Unity's Custom Data system for random values
- **Automatic Validation**: Automatic verification and correction of Vertex Streams settings
- **Comprehensive UI**: Intuitive editor interface

---

## System Architecture

Custom Coord system connects C# editor, HLSL shaders, and Unity Particle System:
- **C# Editor Side**: CustomCoord enum, MaterialGUI, ErrorHandler
- **HLSL Shader**: GET_CUSTOM_COORD and SETUP_CUSTOM_COORD macros
- **Unity Particle System**: Vertex Streams (Custom1/2) and Custom Data

---

## Custom Coord Types and Definitions

### CustomCoord enum (`Assets/Nova/Editor/Core/Scripts/CustomCoord.cs`)

```csharp
public enum CustomCoord
{
    Unused = 0,                    // Unused
    
    // Coord1 (CustomCoord1 vertex stream)
    Coord1X = 1,   Coord1Y = 11,   Coord1Z = 21,   Coord1W = 31,
    
    // Coord2 (CustomCoord2 vertex stream)  
    Coord2X = 2,   Coord2Y = 12,   Coord2Z = 22,   Coord2W = 32
}
```

### UICustomCoord enum (`Assets/Nova/Editor/Core/Scripts/UICustomCoord.cs`)

```csharp
public enum UICustomCoord
{
    Unused = 0,
    Coord1X = 1,   Coord1Y = 11,
    Coord2X = 2,   Coord2Y = 12,
    // Note: UI Particles do not support Z/W components
}
```

### UIParticles Limitations

UIParticles have specific limitations due to Unity UI system constraints. For detailed information about UIParticles constraints and Custom Coord support, see @documentation/UIParticles_Limitations.md.

### Encoding Rules

Custom Coord values use **decimal encoding**:

- **Lower digit (value % 10)**: Stream index (1=Custom1, 2=Custom2, 0=Unused)
- **Upper digit (value / 10)**: Component index (0=x, 1=y, 2=z, 3=w)

Examples:
- `Coord1Y = 11` → Y component of Custom1 stream
- `Coord2Z = 22` → Z component of Custom2 stream

---

## HLSL Implementation Details

### Custom Coord Array Construction

#### SETUP_CUSTOM_COORD Macro (`Assets/Nova/Runtime/Core/Shaders/Particles.hlsl`)

**When GPU Instancing is enabled:**
```hlsl
#define SETUP_CUSTOM_COORD(input) float4 customCoords[] = \
{ \
    float4(0.0, 0.0, 0.0, 0.0),  // [0] Unused \
    instanceData.customCoord1,    // [1] Custom1 \
    instanceData.customCoord2     // [2] Custom2 \
};
```

**During normal rendering:**
```hlsl
#define SETUP_CUSTOM_COORD(input) float4 customCoords[] = \
{ \
    float4(0.0, 0.0, 0.0, 0.0),  // [0] Unused \
    input.customCoord1,           // [1] Custom1 \
    input.customCoord2            // [2] Custom2 \
};
```

### Custom Coord Value Retrieval

#### GET_CUSTOM_COORD Macro

```hlsl
#define GET_CUSTOM_COORD(propertyName) customCoords[(uint)propertyName % 10][(uint)propertyName / 10]
```

Decomposes index using `propertyName % 10` for stream and `/ 10` for component.

---

## GPU Instancing Support

### ParticleInstanceData Structure (`Assets/Nova/Runtime/Core/Shaders/ParticlesInstancing.hlsl`)

```hlsl
struct DefaultParticleInstanceData
{
    float3x4 transform;        // Transform matrix (position, rotation, scale)
    uint color;                // Packed color information
    float4 customCoord1;       // Custom1 stream values
    float4 customCoord2;       // Custom2 stream values
};
```

### GPU Instancing Enablement Conditions

```hlsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) && !defined(SHADER_TARGET_SURFACE_ANALYSIS)
#define NOVA_PARTICLE_INSTANCING_ENABLED
#endif
```

### Data Transfer Method Comparison

| Rendering Method | Custom Coord Data Source | Random Value Source |
|-----------------|---------------------------|------------------------|
| GPU Instancing  | `instanceData.customCoord1/2` | Unity Custom Data → Custom Coord |
| Normal Rendering | `input.customCoord1/2` (Vertex Streams) | Unity Custom Data → Custom Coord |

---

## Editor Integration

### Material Property Definitions (`Assets/Nova/Editor/Core/Scripts/MaterialPropertyNames.cs`)

Property name patterns that use Custom Coord:

```csharp
// Offset-related
public const string BaseMapOffsetXCoord = "_BaseMapOffsetXCoord";
public const string BaseMapOffsetYCoord = "_BaseMapOffsetYCoord";

// Progress-related  
public const string BaseMapProgressCoord = "_BaseMapProgressCoord";
public const string AlphaTransitionProgressCoord = "_AlphaTransitionProgressCoord";

// Intensity-related
public const string FlowIntensityCoord = "_FlowIntensityCoord";
public const string EmissionIntensityCoord = "_EmissionIntensityCoord";
```

### GUI Display Patterns

Editor provides generic patterns for texture offset coordinates and property value with coordinate selection through `MaterialEditorUtility` helper methods.

---

## Feature-Specific Custom Coord Usage

### Base Map Features

| Property | Custom Coord Usage | Notes |
|-----------|------------------|------|
| `_BaseMapOffsetXCoord` | Texture UV offset X | Texture scrolling, etc. |
| `_BaseMapOffsetYCoord` | Texture UV offset Y | Texture scrolling, etc. |
| `_BaseMapRotationCoord` | Texture rotation angle | 0-1 → 0-360 degrees |
| `_BaseMapProgressCoord` | FlipBook animation progress | 0-1 for frame selection |
| `_BaseMapRandomRowCoord` | Random value for Random Row Selection | **Any Custom Coord channel** |

### Tint Color Features

| Property | Custom Coord Usage |
|-----------|------------------|
| `_TintMapOffsetXCoord` | Tint map offset |
| `_TintMapOffsetYCoord` | Tint map offset |
| `_TintRimProgressCoord` | Rim light progress |
| `_TintRimSharpnessCoord` | Rim light boundary value |
| `_TintLuminanceProgressCoord` | Luminance-based color adjustment |
| `_TintLuminanceSharpnessCoord` | Luminance boundary value |

### Other Features

- **Flow Map**: `_FlowMapOffsetXCoord/YCoord`, `_FlowIntensityCoord`
- **Parallax Map**: `_ParallaxMapOffsetXCoord/YCoord`, `_ParallaxMapProgressCoord`
- **Alpha Transition**: `_AlphaTransitionMapOffsetXCoord/YCoord`, `_AlphaTransitionProgressCoord`
- **Emission**: `_EmissionMapOffsetXCoord/YCoord`, `_EmissionIntensityCoord`
- **Transparency**: `_RimTransparencyProgressCoord`, `_LuminanceTransparencyProgressCoord`
- **Vertex Deformation**: `_VertexDeformationMapOffsetXCoord/YCoord`, `_VertexDeformationIntensityCoord`

**Total**: Over 30 properties support Custom Coord

---

## Validation and Error Handling

### Custom Coord Usage Detection (`Assets/Nova/Editor/Core/Scripts/RendererErrorHandler.cs`)

```csharp
private static bool IsCustomCoordUsed(ParticlesGUI.Property prop)
{
    return (CustomCoord)prop.Value.floatValue != CustomCoord.Unused;
}
```

### Feature-Specific Usage Detection

Individually check Custom Coord usage status for each feature:

```csharp
private static bool IsCustomCoordUsedInBaseMap(ParticlesUberCommonMaterialProperties props)
{
    // Check Base Map-related Custom Coord usage
    var isCustomCoordUsed = IsCustomCoordUsed(props.BaseMapOffsetXCoordProp)
                        || IsCustomCoordUsed(props.BaseMapOffsetYCoordProp)
                        || IsCustomCoordUsed(props.BaseMapRotationCoordProp);
    
    // Check Progress usage in FlipBook mode
    var baseMapMode = (BaseMapMode)props.BaseMapModeProp.Value.floatValue;
    isCustomCoordUsed |= (baseMapMode == BaseMapMode.FlipBook || 
                         baseMapMode == BaseMapMode.FlipBookBlending) &&
                         IsCustomCoordUsed(props.BaseMapProgressCoordProp);
    
    // Check Random Row Selection
    if ((baseMapMode == BaseMapMode.FlipBook || baseMapMode == BaseMapMode.FlipBookBlending) &&
        props.BaseMapRandomRowSelectionEnabledProp.Value.floatValue > 0.5f)
    {
        var randomCoord = (CustomCoord)props.BaseMapRandomRowCoordProp.Value.floatValue;
        isCustomCoordUsed |= (randomCoord != CustomCoord.Unused);
    }
    
    return isCustomCoordUsed;
}
```

### Automatic Vertex Streams Configuration

System automatically configures required Vertex Streams based on material settings. When GPU Instancing is disabled and Custom Coord is used, Custom1XYZW and Custom2XYZW streams are automatically added. Editor provides "Fix Now" button for one-click correction.

---

## Performance Considerations

### Benefits of Using GPU Instancing

1. **Data Transfer Efficiency**: No Vertex Streams required, bulk transfer via StructuredBuffer
2. **Rendering Efficiency**: Significant reduction in Draw Call count
3. **Memory Usage**: Reduced Vertex Buffer usage

### Optimization Strategies

- **GPU Instancing**: Eliminates Vertex Streams overhead (recommended)
- **Shader Keywords**: Reduces conditional branching with compile-time optimization
- **Custom Coord**: Flexible channel selection with minimal performance impact

---

## Extension Guidelines

### Adding New Custom Coord-Compatible Features

#### 1. Material Property Definition

```csharp
// MaterialPropertyNames.cs
public const string NewFeatureIntensityCoord = "_NewFeatureIntensityCoord";
```

#### 2. Material Properties Class Extension

```csharp
// ParticlesUberCommonMaterialProperties.cs
public ParticlesGUI.Property NewFeatureIntensityCoordProp { get; } = 
    new(PropertyNames.NewFeatureIntensityCoord);

public override void Setup(MaterialProperty[] properties)
{
    // ... existing setup ...
    NewFeatureIntensityCoordProp.Setup(properties);
}
```

#### 3. Shader Property Definition

```hlsl
// ParticlesUber.hlsl
DECLARE_CUSTOM_COORD(_NewFeatureIntensityCoord);
```

#### 4. GUI Implementation

```csharp
// ParticlesUberCommonGUI.cs
MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(
    _editor, "New Feature Intensity",
    props.NewFeatureIntensityProp.Value, 
    props.NewFeatureIntensityCoordProp.Value);
```

#### 5. Add Validation

```csharp
// RendererErrorHandler.cs
private static bool IsCustomCoordUsedInNewFeature(ParticlesUberCommonMaterialProperties props)
{
    return IsCustomCoordUsed(props.NewFeatureIntensityCoordProp);
}

// Add to IsCustomCoordUsed() method
isCustomCoordUsed |= IsCustomCoordUsedInNewFeature(commonMaterialProperties);
```

#### 6. HLSL Usage

```hlsl
// Usage in shader
float intensity = GET_CUSTOM_COORD(_NewFeatureIntensityCoord);
```

### Implementation Guidelines

**Naming Convention**: `_FeatureNameCoordType` (e.g., `_BaseMapProgressCoord`)

**Key Requirements**:
- Use decimal encoding for Custom Coord values
- Implement type safety with generic `<TCustomCoord>`
- Add validation for each feature
- Provide automatic error correction

---

## Summary

The NOVA Shader Custom Coord system provides:
- Unified parameter delivery across 30+ features
- Type-safe implementation with C# enums
- GPU Instancing optimization
- Automatic validation and error correction
- Easy extensibility for new features

**Key Constraints**: UIParticles support only .xy components. Always use `shader_feature_local` for new features.