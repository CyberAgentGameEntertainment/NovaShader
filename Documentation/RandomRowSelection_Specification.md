# NOVA Shader Random Row Selection Feature Design Document

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->
<!-- EDITING REMINDER: Before editing, read CLAUDE.md Documentation Editing Protocol -->
<!-- PROHIBITED: Troubleshooting sections, Checklists, Version history, Debug steps -->
<!-- FOCUS: Current implementation state, Technical specifications with code examples -->

## 1. Overview

The Random Row Selection feature is an implementation of Unity Particle System's Texture Sheet Animation Row Mode > Random functionality for NOVA shaders. This feature allows random selection of one row from a texture sheet when generating animations for each particle.

## 2. Current Implementation Status

### 2.1 Supported Shaders

| Shader | Status | Notes |
|----------|---------|------|
| ParticlesUberUnlit | ✅ Implemented | Full support with any Custom Coord |
| ParticlesUberLit | ✅ Implemented | Full support with any Custom Coord |
| UIParticlesUberUnlit | ✅ Implemented | Supports Custom1.x/y, Custom2.x/y |
| UIParticlesUberLit | ✅ Implemented | Supports Custom1.x/y, Custom2.x/y |
| ParticlesDistortion | ❌ Not supported | Excluded as FlipBook feature itself is not implemented |

### 2.2 Implementation Components

#### Shader Properties
- `_BaseMapRandomRowSelectionEnabled`: Enable/disable flag for Random Row Selection feature (Float: 0.0 or 1.0)
- `_BaseMapRandomRowCoord`: Custom Coord for obtaining random values (Float: CustomCoord index)
  - Default: 0.0 (Unused) - consistent across all shader variants
- `_BaseMapRowCount`: Number of rows in texture sheet (Float: 1.0 or greater)

#### Shader Keywords
- `_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED`: Keyword defined when feature is enabled
  - **Optimization**: Uses `shader_feature_local_vertex` for vertex-only access
  - **Performance**: Reduces compilation time and memory usage

#### HLSL Functions (Particles.hlsl)
- `FlipBookProgressWithRandomRow()`: Random row selection function for standard FlipBook
- `FlipBookBlendingProgressWithRandomRow()`: Random row selection function for FlipBook Blending

## 3. Custom Coord Design

### 3.1 Custom Coord Integration

Random Row Selection integrates with NOVA's Custom Coord system (see @documentation/CustomCoord_SystemArchitecture.md for detailed information).

### 3.2 Random Value Configuration

Random Row Selection can use any Custom Coord channel:
- Configure Unity Particle System's Custom Data
- Use "Random Between Two Constants" mode
- Set range from 0 to Row Count (e.g., 0-4 for 4 rows)
- Assign to desired Custom Coord channel

**Note**: UIParticles support this feature using Custom1.x/y and Custom2.x/y components only.

## 4. Operation Principle

### 4.1 Row Selection Algorithm

```hlsl
half framesPerRow = sliceCount / rowCount;
// randomValue is expected to be in range [0, rowCount)
// Unity Custom Data "Random Between Two Constants" should be set to 0 to rowCount
half selectedRow = clamp(floor(randomValue), 0.0, rowCount - 1.0);
half frameProgress = FlipBookProgress(progress, framesPerRow);
return selectedRow * framesPerRow + frameProgress;
```

1. Calculate frames per row (`framesPerRow = total frames / row count`)
2. Select row from random value in range [0, rowCount)
3. Use `clamp(floor(randomValue), 0.0, rowCount - 1.0)` to ensure valid row index
4. Calculate animation progress within selected row
5. Return final frame index

### 4.2 Editor UI Integration

Editor provides enable/disable toggle, Custom Coord channel selection, and Row Count setting with automatic error correction.

## 5. Usage

### 5.1 Basic Setup

1. Set material's Base Map Mode to FlipBook or FlipBook Blending
2. Enable Random Row Selection
3. Set Row Count to the number of rows in texture sheet
4. Select a Custom Coord channel for random values
5. Configure Particle System's Custom Data:
   - Set mode to "Random Between Two Constants"
   - Range: 0 to Row Count (e.g., 0-4 for 4 rows)
   - Assign to the selected Custom Coord channel

### 5.2 Vertex Streams Setup

When not using GPU Instancing, ensure proper Vertex Streams:
1. Open Particle System > Renderer > Custom Vertex Streams
2. Add Custom1.xyzw and/or Custom2.xyzw as needed
3. Use editor's "Fix Now" button for automatic configuration

## 6. Implementation Details

### 6.1 Keyword Management

Keywords are set in ParticlesUberUnlitMaterialPostProcessor.cs under the following conditions:
```csharp
// Random Row Selection enabled for all shader types including UIParticles
var randomRowSelectionEnabled = (baseMapMode == BaseMapMode.FlipBook || 
                                baseMapMode == BaseMapMode.FlipBookBlending) &&
                                material.GetFloat(BaseMapRandomRowSelectionEnabledId) > 0.5f;
```

### 6.2 Performance Considerations

- Random value calculation is executed only once in vertex shader
- No additional texture sampling required
- Full compatibility with GPU Instancing
- Flexible configuration with any Custom Coord channel

## 7. Limitations

1. Only available in FlipBook or FlipBook Blending modes
2. Row Count must be an integer value of 1 or greater
3. Texture sheet structure must be uniform rows × columns
4. **UIParticles limitation**: Only Custom1.x/y and Custom2.x/y components available (Z/W components not supported). For detailed UIParticles constraints, see @documentation/UIParticles_Limitations.md

## 8. Shader Optimization System Integration

### 8.1 NOVA Shader Optimization Support

Random Row Selection is **fully integrated** with NOVA's shader optimization system:

- **OptimizedShaderGenerator**: Preserves Random Row Selection functionality in optimized shaders
- **Keyword Protection**: `_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED` is excluded from optimization removal
- **24 Shader Variants**: All optimized shader variants (Opaque/Transparent/Cutout × 8 passes) include Random Row Selection support
- **Performance Benefits**: Maintains feature while eliminating unused shader passes

Uses `shader_feature_local_vertex` for vertex-only access, improving compilation speed and memory usage.

## 9. Implementation Status

Random Row Selection feature is **production-ready**. The feature uses the standard Custom Coord system with the following implementation:

```hlsl
#ifdef _BASE_MAP_RANDOM_ROW_SELECTION_ENABLED
if (_BaseMapRandomRowSelectionEnabled > 0.5 && _BaseMapRowCount > 1.0) {
    float randomValue = GET_CUSTOM_COORD(_BaseMapRandomRowCoord);
    progress = FlipBookProgressWithRandomRow(progress, _BaseMapSliceCount, _BaseMapRowCount, randomValue);
}
#endif
```

## 10. Technical Specifications

### 10.1 TEXCOORD Resource Allocation

The implementation uses optimized TEXCOORD allocation to avoid conflicts. For detailed TEXCOORD usage strategy, see @documentation/TEXCOORD_Usage_Strategy.md.

Random Row Selection now uses standard Custom Coord system, avoiding TEXCOORD conflicts entirely. For comprehensive Custom Coord system architecture, see @documentation/CustomCoord_SystemArchitecture.md.

### 10.2 Random Value Input Methods

#### Unity Particle System Custom Data Configuration
```csharp
// Example: Configure Random Between Two Constants
ParticleSystem.CustomDataModule customData = particleSystem.customData;
customData.enabled = true;
customData.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
customData.SetVectorComponentCount(ParticleSystemCustomData.Custom1, 1);

// Set Random Between Two Constants (0 to rowCount)
var minMaxCurve = new ParticleSystem.MinMaxCurve(0f, rowCount);
customData.SetVector(ParticleSystemCustomData.Custom1, 0, minMaxCurve);
```