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
| ParticlesUberUnlit | ✅ Implemented | Full support |
| ParticlesUberLit | ✅ Implemented | Full support |
| UIParticlesUberUnlit | ❌ Not supported | Unity UI system limitations (see @documentation/UIParticles_Limitations.md) |
| UIParticlesUberLit | ❌ Not supported | Unity UI system limitations (see @documentation/UIParticles_Limitations.md) |
| ParticlesDistortion | ❌ Not supported | Excluded as FlipBook feature itself is not implemented |

### 2.2 Implementation Components

#### Shader Properties
- `_BaseMapRandomRowSelectionEnabled`: Enable/disable flag for Random Row Selection feature (Float: 0.0 or 1.0)
- `_BaseMapRandomRowCoord`: Custom Coord for obtaining random values (Float: CustomCoord index)
- `_BaseMapRowCount`: Number of rows in texture sheet (Float: 1.0 or greater)

#### Shader Keywords
- `_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED`: Keyword defined when feature is enabled

#### HLSL Functions (Particles.hlsl)
- `FlipBookProgressWithRandomRow()`: Random row selection function for standard FlipBook
- `FlipBookBlendingProgressWithRandomRow()`: Random row selection function for FlipBook Blending

## 3. Custom Coord Design

### 3.1 Custom Coord Types

The following Custom Coords are available in NOVA shader:

#### Standard Custom Coord
- Coord1.x (1), Coord1.y (11), Coord1.z (21), Coord1.w (31)
- Coord2.x (2), Coord2.y (12), Coord2.z (22), Coord2.w (32)

### 3.2 Random Value Configuration

Random Row Selection can use any Custom Coord channel:
- Configure Unity Particle System's Custom Data
- Use "Random Between Two Constants" mode
- Set range from 0 to Row Count (e.g., 0-4 for 4 rows)
- Assign to desired Custom Coord channel

## 4. Operation Principle

### 4.1 Row Selection Algorithm

```hlsl
half framesPerRow = sliceCount / rowCount;
// Handle both normalized (0-1) and row index (0-rowCount) random values
uint selectedRow = randomValue < 1.0 ? 
    min(floor(randomValue * rowCount), rowCount - 1) : 
    min(floor(randomValue), rowCount - 1);
half frameProgress = FlipBookProgress(progress, framesPerRow);
return selectedRow * framesPerRow + frameProgress;
```

1. Calculate frames per row (`framesPerRow = total frames / row count`)
2. Select row from random value:
   - If value < 1.0: treat as normalized (0-1) range
   - If value >= 1.0: treat as row index (0-rowCount)
3. Calculate animation progress within selected row
4. Return final frame index

### 4.2 Editor UI Integration

ParticlesUberCommonGUI.cs provides the following functionality:
- Random Row Selection enable/disable toggle
- Custom Coord channel selection for random values
- Row Count setting
- Setting error detection and automatic correction functionality

## 5. Usage

### 5.1 Basic Setup

1. Set material's Base Map Mode to FlipBook or FlipBook Blending
2. Enable Random Row Selection
3. Select a Custom Coord channel for random values
4. Set Row Count to the number of rows in texture sheet
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
// UIParticles exclusion for Unity UI system compatibility
bool isUIParticles = material.shader.name.Contains("UIParticles");
var randomRowSelectionEnabled = !isUIParticles && (baseMapMode == BaseMapMode.FlipBook || 
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
4. **Not supported in UIParticles** due to Unity UI system limitations

## 8. Shader Optimization System Integration

### 8.1 NOVA Shader Optimization Support

Random Row Selection is **fully integrated** with NOVA's shader optimization system:

- **OptimizedShaderGenerator**: Preserves Random Row Selection functionality in optimized shaders
- **Keyword Protection**: `_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED` is excluded from optimization removal
- **24 Shader Variants**: All optimized shader variants (Opaque/Transparent/Cutout × 8 passes) include Random Row Selection support
- **Performance Benefits**: Maintains feature while eliminating unused shader passes

### 8.2 Shader Variant Impact

| Shader | Total Features | Random Row Selection Impact | Notes |
|--------|---------------|-----------------------------|-------|
| ParticlesUberUnlit | 166 shader_feature pragmas | +6 variants (6 passes) | Minimal impact with shader_feature_local |
| ParticlesUberLit | 175 shader_feature pragmas | +6 variants (6 passes) | Minimal impact with shader_feature_local |
| UIParticles | N/A | No impact | Feature excluded |

## 9. Future Extension Possibilities

The current implementation is complete and provides equivalent functionality to Unity's standard Row Mode > Random feature.
Future extension ideas:
- Column-based random selection functionality
- Weighted random selection
- Animation speed randomization

## 9. Implementation Status

### 9.1 Completed Implementation

✅ **Shader Keyword pragma Declaration Added**

pragma declaration for `_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED` has been added to the following shader files:

- ✅ ParticlesUberUnlit.shader
- ✅ ParticlesUberLit.shader
- ❌ UIParticlesUberUnlit.shader (removed due to Unity UI system limitations)
- ❌ UIParticlesUberLit.shader (removed due to Unity UI system limitations)

Implementation location:
```hlsl
// Base Map
#pragma shader_feature_local _BASE_MAP_MODE_2D _BASE_MAP_MODE_2D_ARRAY _BASE_MAP_MODE_3D
#pragma shader_feature_local _BASE_MAP_RANDOM_ROW_SELECTION_ENABLED  // ✅ Added
#pragma shader_feature_local_vertex _BASE_MAP_ROTATION_ENABLED
```

✅ **StableRandom Transfer Between Vertex-Fragment**

For Non-GPU Instancing, StableRandom needs to be transferred from vertex shader to fragment shader:

**Addition to Varyings struct:**
```hlsl
#if !defined(NOVA_PARTICLE_INSTANCING_ENABLED) && defined(_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED)
float stableRandomX : TEXCOORD[N];  // StableRandom.x for Fragment Shader
#endif
```

**Transfer in Vertex Shader:**
```hlsl
// Transfer StableRandom.x for Random Row Selection
#if !defined(NOVA_PARTICLE_INSTANCING_ENABLED) && defined(_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED)
output.stableRandomX = input.stableRandomX;
#endif
```

### 9.2 Implementation Completion Report

Random Row Selection feature is **production-ready with comprehensive robustness**. All components are in place:

- ✅ HLSL function implementation (`FlipBookProgressWithRandomRow` etc.)
- ✅ Material property definition with optimal defaults
- ✅ Editor GUI implementation with Custom Coord channel selection
- ✅ Shader keyword management
- ✅ pragma declaration addition
- ✅ Custom Coord channel selection for random values
- ✅ Vertex Streams automatic setup
- ✅ Vertex-Fragment data transfer
- ✅ TEXCOORD channel optimization
- ✅ Shader compile error resolution
- ✅ GUI optimization for user-friendly experience
- ✅ **Robust edge case handling for production deployment**

## 10. Technical Specifications

### 10.1 TEXCOORD Resource Allocation

The implementation uses optimized TEXCOORD allocation to avoid conflicts. For detailed TEXCOORD usage strategy, see @documentation/TEXCOORD_Usage_Strategy.md.

Random Row Selection now uses standard Custom Coord system, avoiding TEXCOORD conflicts entirely. The implementation relies on Unity's Custom Data system for random value input.

### 10.2 Mathematical Implementation

The implementation handles both normalized (0-1) and row index (0-rowCount) random values automatically, providing flexibility in how random values are configured.

### 10.3 Random Value Input Methods

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