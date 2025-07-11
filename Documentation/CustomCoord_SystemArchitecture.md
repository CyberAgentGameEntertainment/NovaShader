# NOVA Shader Custom Coord System Technical Documentation

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->

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
10. [Troubleshooting](#troubleshooting)
11. [Extension Guidelines](#extension-guidelines)

---

## Overview

The NOVA Shader Custom Coord system provides a mechanism for delivering independent parameters per particle. It supports Unity's standard Particle System Custom1/Custom2 streams and the StableRandom stream added in Unity 2023.2, and can be used as animation parameters in over 30 features.

### Key Features

- **Type Safety**: Type-safe implementation based on C# enums
- **GPU Instancing Support**: High-performance instancing rendering
- **StableRandom Support**: Supports features available in Unity 2022.3+
- **Automatic Validation**: Automatic verification and correction of Vertex Streams settings
- **Comprehensive UI**: Intuitive editor interface

---

## System Architecture

### Overall Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  C# Editor Side │    │  HLSL Shader    │    │ Unity Particle  │
│                 │    │                 │    │ System          │
│ CustomCoord     │◄──►│ GET_CUSTOM_COORD│◄──►│ Vertex Streams  │
│ MaterialGUI     │    │ Macro           │    │ Custom1/2       │
│ ErrorHandler    │    │ SETUP_CUSTOM_   │    │ StableRandom    │
│                 │    │ COORD           │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Data Flow

1. **Particle System** → Custom1/Custom2/StableRandom Vertex Streams
2. **Vertex Shader** → Create float4 array with SETUP_CUSTOM_COORD
3. **Shader Functions** → Retrieve values with GET_CUSTOM_COORD
4. **Material Editor** → Setting management with CustomCoord enum

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
    Coord2X = 2,   Coord2Y = 12,   Coord2Z = 22,   Coord2W = 32,
    
    // StableRandom (Unity 2022.3+ StableRandom vertex stream)
    StableRandomX = 50
}
```

### UICustomCoord enum (`Assets/Nova/Editor/Core/Scripts/UICustomCoord.cs`)

```csharp
public enum UICustomCoord
{
    Unused = 0,
    Coord1X = 1,   Coord1Y = 11,
    Coord2X = 2,   Coord2Y = 12,
    // Note: UI Particles do not support Z/W components and StableRandom
}
```

### Encoding Rules

Custom Coord values use **decimal encoding**:

- **Lower digit (value % 10)**: Stream index (1=Custom1, 2=Custom2, 0=Unused)
- **Upper digit (value / 10)**: Component index (0=x, 1=y, 2=z, 3=w)
- **50**: Special value dedicated to StableRandom

Examples:
- `Coord1Y = 11` → Y component of Custom1 stream
- `Coord2Z = 22` → Z component of Custom2 stream
- `StableRandomX = 50` → StableRandom stream (X component)

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
#define GET_CUSTOM_COORD(propertyName) ( \
    ((uint)propertyName == 50) ? GET_STABLE_RANDOM_X() : \
    customCoords[(uint)propertyName % 10][(uint)propertyName / 10] \
)
```

**Analysis Process:**
1. **StableRandom Detection**: If value is 50, call StableRandom-specific function
2. **Index Decomposition**: `propertyName % 10` for stream, `/ 10` for component
3. **Array Access**: Retrieve value with `customCoords[stream][component]`

### StableRandom Support

#### During GPU Instancing
```hlsl
#define GET_STABLE_RANDOM_X() instanceData.stableRandom.x
```

#### During Normal Rendering (Fallback)
```hlsl
#define GET_STABLE_RANDOM_X() 0.5  // Fallback value
```

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
    float4 stableRandom;       // StableRandom stream values
};
```

### GPU Instancing Enablement Conditions

```hlsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) && !defined(SHADER_TARGET_SURFACE_ANALYSIS)
#define NOVA_PARTICLE_INSTANCING_ENABLED
#endif
```

### Data Transfer Method Comparison

| Rendering Method | Custom Coord Data Source | StableRandom Data Source |
|-----------------|---------------------------|---------------------------|
| GPU Instancing  | `instanceData.customCoord1/2` | `instanceData.stableRandom` |
| Normal Rendering | `input.customCoord1/2` (Vertex Streams) | `input.stableRandom` or fallback |

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

#### Generic Implementation (`Assets/Nova/Editor/Core/Scripts/ParticlesUberCommonGUI.cs`)

```csharp
// Texture + offset coordinate
MaterialEditorUtility.DrawTexture<TCustomCoord>(editor, label, 
    textureProp, offsetXCoordProp, offsetYCoordProp);

// Property value + coordinate selection
MaterialEditorUtility.DrawPropertyAndCustomCoord<TCustomCoord>(editor, 
    label, valueProp, coordProp);
```

#### Random Row Selection Dedicated GUI

```csharp
// Enable Random Row Selection
MaterialEditorUtility.DrawToggleProperty(editor, "Random Row Selection",
    props.BaseMapRandomRowSelectionEnabledProp.Value);

// Custom Coord selection (displays StableRandom.x recommendation)
var coord = (TCustomCoord)Enum.ToObject(typeof(TCustomCoord), 
    Convert.ToInt32(props.BaseMapRandomRowCoordProp.Value.floatValue));
var newCoord = (TCustomCoord)EditorGUILayout.EnumPopup(coord);
```

---

## Feature-Specific Custom Coord Usage

### Base Map Features

| Property | Custom Coord Usage | Notes |
|-----------|------------------|------|
| `_BaseMapOffsetXCoord` | Texture UV offset X | Texture scrolling, etc. |
| `_BaseMapOffsetYCoord` | Texture UV offset Y | Texture scrolling, etc. |
| `_BaseMapRotationCoord` | Texture rotation angle | 0-1 → 0-360 degrees |
| `_BaseMapProgressCoord` | FlipBook animation progress | 0-1 for frame selection |
| `_BaseMapRandomRowCoord` | Random value for Random Row Selection | **StableRandom.x recommended** |

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

#### Configuration Logic

```csharp
internal static void SetupCorrectVertexStreams(Material material,
    ParticleSystemRenderer renderer, ParticlesUberCommonMaterialProperties commonMaterialProperties)
{
    var correctVertexStreams = new List<ParticleSystemVertexStream>
    {
        ParticleSystemVertexStream.Position,
        ParticleSystemVertexStream.Normal,
        ParticleSystemVertexStream.Tangent,
        ParticleSystemVertexStream.Color,
        ParticleSystemVertexStream.UV,
        ParticleSystemVertexStream.UV2
    };

    // GPU Instancing check
    bool hasGpuInstancing = material.IsKeywordEnabled("UNITY_PROCEDURAL_INSTANCING_ENABLED");
    
    if (!hasGpuInstancing)
    {
        // Regular Custom Coord usage check
        bool isRegularCustomCoordUsed = IsCustomCoordUsedExcludingRandomRow(commonMaterialProperties);
        
        // Random Row Selection usage check
        bool isRandomRowSelectionEnabled = /* ... */;
        
        if (isRegularCustomCoordUsed)
        {
            correctVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
            correctVertexStreams.Add(ParticleSystemVertexStream.Custom2XYZW);
        }
        else if (isRandomRowSelectionEnabled)
        {
            // Only StableRandom.x needed
            correctVertexStreams.Add(ParticleSystemVertexStream.StableRandomX);
        }
    }
    
    // Vertex Streams configuration
    renderer.SetActiveVertexStreams(correctVertexStreams);
}
```

### Error Detection and Correction

#### "Fix Now" Button Functionality

```csharp
if (RendererErrorHandler.TryFindIncorrectVertexStreams(material, renderer, 
    commonMaterialProperties, out var correctVertexStreams))
{
    if (EditorGUILayout.Button("Fix Now"))
    {
        RendererErrorHandler.SetupCorrectVertexStreams(material, renderer, commonMaterialProperties);
    }
}
```

---

## Performance Considerations

### Benefits of Using GPU Instancing

1. **Data Transfer Efficiency**: No Vertex Streams required, bulk transfer via StructuredBuffer
2. **Rendering Efficiency**: Significant reduction in Draw Call count
3. **Memory Usage**: Reduced Vertex Buffer usage

### StableRandom Usage Optimization

#### Recommended Settings for Random Row Selection Feature

- **Use StableRandom.x**: Utilizes dedicated optimization path
- **GPU Instancing**: StableRandom automatically available
- **Non-GPU Instancing**: Add minimal required Vertex Stream (StableRandomX only)

#### Performance Comparison

| Setting | Vertex Streams | GPU Load | Notes |
|------|---------------|---------|------|
| StableRandom.x (GPU Instancing) | None | Minimal | **Recommended setting** |
| StableRandom.x (Normal) | StableRandomX | Low | Lightweight |
| Custom1/Custom2 (Normal) | Custom1XYZW, Custom2XYZW | Medium | High versatility |

### Shader Variant Optimization

#### Reducing Conditional Branching Using Keywords

```hlsl
#ifdef _BASE_MAP_RANDOM_ROW_SELECTION_ENABLED
    // Code only when Random Row Selection is enabled
    if (_BaseMapRandomRowSelectionEnabled > 0.5 && _BaseMapRowCount > 1.0) {
        float randomValue = GET_CUSTOM_COORD(_BaseMapRandomRowCoord);
        // ...
    }
#endif
```

---

## Troubleshooting

### Common Problems and Solutions

#### 1. Cannot retrieve Custom Coord values

**Symptoms**: GET_CUSTOM_COORD always returns 0

**Causes and Solutions**:
- Vertex Streams not configured when GPU Instancing is disabled
- Enable Custom1XYZW/Custom2XYZW in ParticleSystemRenderer
- Use editor "Fix Now" button for automatic correction

#### 2. StableRandom not working

**Symptoms**: StableRandom.x selected but doesn't produce random values

**Causes and Solutions**:
- Unity versions before 2022.3: StableRandom not supported, change to Custom Coord
- Vertex Streams configuration: Add StableRandomX stream for Non-GPU Instancing
- Fallback: Shader may be using fixed 0.5 value

#### 3. Random Row Selection not working

**Symptoms**: Random Row Selection enabled but rows not randomly selected

**Causes and Solutions**:
- Missing pragma declaration: Add `#pragma shader_feature_local _BASE_MAP_RANDOM_ROW_SELECTION_ENABLED`
- Base Map Mode: Set to FlipBook or FlipBook Blending
- Row Count: Set to 2 or higher

#### 4. Shader compile error "invalid subscript 'stableRandomX'"

**Symptoms**: Error accessing stableRandomX in Fragment shader

**Causes and Solutions**:
- Varyings struct missing stableRandomX field
- No transfer process from Vertex shader to Fragment shader
- Duplicate TEXCOORD index

**Resolution Steps**:
1. Add stableRandomX field to Varyings struct
2. Transfer value from input to output in Vertex shader
3. Use TEXCOORD index that doesn't conflict with other features

#### 5. Cannot select Z or W in UI Particles

**Symptoms**: Coord1Z/Coord1W etc. cannot be selected in UI shaders

**Cause**: UI Particles use UICustomCoord, supporting only XY
**Solution**: This is a specification limitation. Use normal Particle shaders when ZW is required

### Debug Macros

```hlsl
// Debug: Visualize Custom Coord values
#define DEBUG_CUSTOM_COORD(coord) return float4(GET_CUSTOM_COORD(coord).xxx, 1.0);

// Usage example: In Fragment Shader
// DEBUG_CUSTOM_COORD(_BaseMapProgressCoord);
```

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

### Adding New Custom Coord Types

#### For adding special streams other than StableRandom:

1. **CustomCoord enum extension**: Define new values in the 50+ range
2. **GET_CUSTOM_COORD extension**: Add special handling for new values
3. **GPU Instancing extension**: Add new field to DefaultParticleInstanceData
4. **Vertex Streams extension**: Configure new stream in RendererErrorHandler

### Coding Conventions

#### Naming Rules

- **Properties**: `_FeatureNameCoordType` (e.g., `_BaseMapProgressCoord`)
- **GUI Display**: Use English names directly
- **Custom Coord Values**: Decimal encoding following existing patterns

#### Implementation Patterns

- **Generic Support**: Ensure type safety with `<TCustomCoord>`
- **Validation**: Implement dedicated check functions for each feature
- **Error Handling**: Provide automatic correction functionality

---

## Conclusion

The NOVA Shader Custom Coord system achieves high flexibility and maintainability through the following design principles:

### Excellent Design Points

1. **Consistency**: Unified Custom Coord support across 30+ features
2. **Type Safety**: Type-safe implementation using C# enums
3. **Performance**: GPU Instancing and StableRandom optimization
4. **Usability**: Intuitive UI design and automatic validation
5. **Extensibility**: Easy addition of new features through generic design

### Considerations for Future Development

- **UI Particles Limitations**: Z/W components and StableRandom not supported
- **Unity Version**: StableRandom available in Unity 2022.3 and later
- **Pragma Declarations**: Don't forget to add `#pragma shader_feature_local` for new features
- **Validation**: Maintain automatic Vertex Streams configuration when using Custom Coord

This system enables artists to efficiently create complex particle effects while allowing developers to maintain a highly maintainable codebase.