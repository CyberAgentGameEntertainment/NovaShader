# NOVA Shader TEXCOORD Usage Strategy

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->
<!-- EDITING REMINDER: Before editing, read CLAUDE.md Documentation Editing Protocol -->
<!-- PROHIBITED: Troubleshooting sections, Checklists, Version history, Debug steps -->
<!-- FOCUS: Current implementation state, Technical specifications with code examples -->

## 1. Overview

This document describes NOVA Shader's TEXCOORD usage strategy, particularly for the Random Row Selection feature implementation. The strategy demonstrates how TEXCOORD slots are optimized per render pass while maintaining consistency in vertex input.

## 2. Design Principles

### 2.1 Input Consistency
All shaders use the same TEXCOORD index for vertex input to ensure consistent Vertex Streams configuration:
```hlsl
// Consistent across all shaders
float stableRandomX : TEXCOORD15;
```

### 2.2 Varyings Optimization
Each render pass optimizes TEXCOORD usage based on available slots:
```hlsl
// Pass-specific optimization
Forward Pass:     float stableRandomX : TEXCOORD14;
ShadowCaster:     float stableRandomX : TEXCOORD8;
DepthNormals:     float stableRandomX : TEXCOORD10;
```

## 3. Technical Implementation

### 3.1 TEXCOORD Slot Allocation

| Component | Input (Attributes) | Forward Pass | ShadowCaster | DepthNormals | Notes |
|-----------|-------------------|--------------|--------------|--------------|-------|
| customCoord1 | TEXCOORD1 | TEXCOORD0 | TEXCOORD0 | TEXCOORD0 | Standard allocation |
| customCoord2 | TEXCOORD2 | TEXCOORD1 | TEXCOORD1 | TEXCOORD1 | Standard allocation |
| baseMapUV | - | TEXCOORD2 | TEXCOORD2 | TEXCOORD2 | Calculated in vertex |
| flowTransitionUVs | - | TEXCOORD3 | TEXCOORD3 | TEXCOORD3 | Conditional usage |
| tintEmissionUV | - | TEXCOORD4 | TEXCOORD4 | TEXCOORD4 | Conditional usage |
| transitionProgress | - | TEXCOORD5 | TEXCOORD5 | TEXCOORD5 | Progress values |
| viewDirWS | - | TEXCOORD6 | - | TEXCOORD6 | Forward/DepthNormals only |
| normalWS | - | TEXCOORD7 | - | - | Forward only |
| projectedPosition | - | TEXCOORD8 | - | TEXCOORD7 | Position varies |
| viewDirTS | - | TEXCOORD9 | - | - | Parallax mapping |
| parallaxMapUV | - | TEXCOORD10 | - | - | Parallax mapping |
| flowTransition2nd | - | TEXCOORD11 | TEXCOORD6 | TEXCOORD8 | Second texture |
| transition2nd | - | TEXCOORD12 | TEXCOORD7 | TEXCOORD9 | Second texture |
| mask | - | TEXCOORD13 | - | - | UI clipping |
| stableRandomX | TEXCOORD3 | TEXCOORD14 | TEXCOORD8 | TEXCOORD10 | Random Row Selection |
| probeOcclusion | - | TEXCOORD14 | - | - | APV (Lit only) |

### 3.2 Conflict Resolution

#### TEXCOORD14 Usage in ParticlesUberLit
```hlsl
// Both use TEXCOORD14 but never simultaneously
#ifdef USE_APV_PROBE_OCCLUSION
    float4 probeOcclusion : TEXCOORD14;  // APV + Debug mode only
#endif

#if !defined(NOVA_PARTICLE_INSTANCING_ENABLED) && defined(_BASE_MAP_RANDOM_ROW_SELECTION_ENABLED)
    float stableRandomX : TEXCOORD14;    // Non-instanced Random Row Selection
#endif
```

**Why no conflict occurs:**
1. `probeOcclusion`: Requires APV enabled AND Debug Display mode
2. `stableRandomX`: Requires GPU Instancing disabled AND Random Row Selection enabled
3. Debug Display mode is development-only, not used in production
4. When using Random Row Selection in production, GPU Instancing is typically enabled

## 4. Implementation Guidelines

### 4.1 Adding New Features

When adding features that require TEXCOORD slots:

1. **Input Struct**: Use the highest available TEXCOORD index for consistency
2. **Varyings Struct**: Optimize per-pass based on available slots
3. **Document Usage**: Update this table with new allocations

### 4.2 TEXCOORD Limits

Unity supports up to 16 TEXCOORD semantics (TEXCOORD0-15). Current usage:
- **Input**: Uses up to TEXCOORD15
- **Forward Pass**: Uses up to TEXCOORD14
- **Other Passes**: Optimized usage based on requirements

## 5. Random Row Selection Specific Notes

### 5.1 StableRandom.x Allocation

The Random Row Selection feature uses StableRandom.x with the following strategy:

```hlsl
// Input: Always TEXCOORD3 for mobile compatibility
float stableRandomX : TEXCOORD3;

// Varyings: Optimized per pass
Forward:      TEXCOORD14  // Shared with probeOcclusion (exclusive conditions)
ShadowCaster: TEXCOORD8   // Reuses second texture blend slot when not needed
DepthNormals: TEXCOORD10  // Reuses parallax slot (not used in this pass)
```

### 5.2 GPU Instancing Optimization

When GPU Instancing is enabled, StableRandom.x is accessed directly from instance data, eliminating TEXCOORD usage entirely:

```hlsl
#ifdef NOVA_PARTICLE_INSTANCING_ENABLED
    #define GET_STABLE_RANDOM_X() instanceData.stableRandom.x
#endif
```

## 6. Performance Considerations

### 6.1 Optimization Strategy

1. **Minimize TEXCOORD Usage**: Each pass uses only required slots
2. **Conditional Compilation**: Features not used don't allocate slots
3. **GPU Instancing Priority**: Eliminates most TEXCOORD requirements

### 6.2 Best Practices

- Enable GPU Instancing when possible to reduce TEXCOORD pressure
- Use conditional compilation to exclude unused features
- Reuse TEXCOORD slots across exclusive features

## 7. Validation and Testing

### 7.1 Shader Compilation

All shader variants compile successfully with current TEXCOORD allocation:
- No conflicts in any shader configuration
- All features work correctly with their assigned slots

### 7.2 Runtime Validation

The Vertex Streams system automatically configures required streams based on material settings, ensuring correct data flow.

## 8. Future Considerations

### 8.1 Available Slots

Currently available for future features:
- Forward Pass: None (TEXCOORD14 is the last available)
- ShadowCaster: TEXCOORD9-15 (simplified pass has more available)
- DepthNormals: TEXCOORD11-15 (simplified pass has more available)

### 8.2 Expansion Strategy

If more TEXCOORD slots are needed:
1. Review conditional features for optimization opportunities
2. Consider pass-specific feature exclusion
3. Utilize GPU Instancing to reduce slot requirements