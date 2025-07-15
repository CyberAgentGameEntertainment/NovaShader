# NOVA Shader TEXCOORD Usage Strategy

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->
<!-- EDITING REMINDER: Before editing, read CLAUDE.md Documentation Editing Protocol -->
<!-- PROHIBITED: Troubleshooting sections, Checklists, Version history, Debug steps -->
<!-- FOCUS: Current implementation state, Technical specifications with code examples -->

## 1. Overview

This document describes NOVA Shader's TEXCOORD usage strategy and allocation patterns. The strategy demonstrates how TEXCOORD slots are optimized per render pass while maintaining consistency in vertex input for Custom Coord features.

## 2. Design Principles

### 2.1 Input Consistency
All shaders use consistent TEXCOORD indices for vertex input to ensure uniform Vertex Streams configuration:
```hlsl
// Consistent across all shaders
float4 customCoord1 : TEXCOORD1;
float4 customCoord2 : TEXCOORD2;
```

### 2.2 Varyings Optimization
Each render pass optimizes TEXCOORD usage based on available slots and feature requirements. Custom Coord data is transferred efficiently using the TRANSFER_CUSTOM_COORD macro system.

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
| probeOcclusion | - | TEXCOORD14 | - | - | APV (Lit only) |

### 3.2 Custom Coord Integration

#### Custom Coord Transfer Strategy
```hlsl
// Vertex shader: Transfer Custom Coord data to fragment
#ifdef NOVA_PARTICLE_INSTANCING_ENABLED
    TRANSFER_CUSTOM_COORD(input, output) output.customCoord1 = instanceData.customCoord1;
    output.customCoord2 = instanceData.customCoord2;
#else
    TRANSFER_CUSTOM_COORD(input, output) output.customCoord1 = input.customCoord1;
    output.customCoord2 = input.customCoord2;
#endif
```

**Key Design Benefits:**
1. **Consistent Interface**: Same TEXCOORD indices across all shader variants
2. **GPU Instancing Optimization**: Automatic data source selection
3. **Feature Independence**: Custom Coord transfer independent of specific features
4. **Scalability**: Support for multiple features using Custom Coord system

## 4. Implementation Guidelines

### 4.1 Adding New Features

When adding features that require TEXCOORD slots:

1. **Input Struct**: Use the highest available TEXCOORD index for consistency
2. **Varyings Struct**: Optimize per-pass based on available slots
3. **Document Usage**: Update this table with new allocations

### 4.2 TEXCOORD Limits

Unity supports up to 16 TEXCOORD semantics (TEXCOORD0-15). Current usage:
- **Input**: Uses up to TEXCOORD2 for Custom Coord (customCoord1, customCoord2)
- **Forward Pass**: Uses up to TEXCOORD14 for various features
- **Other Passes**: Optimized usage based on requirements

## 5. Custom Coord Usage Notes

### 5.1 Custom Coord Allocation Strategy

The Custom Coord system provides a flexible foundation for feature implementation. For comprehensive Custom Coord system architecture, see @documentation/CustomCoord_SystemArchitecture.md.

```hlsl
// Input: Consistent allocation for Custom Coord data
float4 customCoord1 : TEXCOORD1;  // Custom1.xyzw
float4 customCoord2 : TEXCOORD2;  // Custom2.xyzw

// Usage: Access via GET_CUSTOM_COORD macro
float randomValue = GET_CUSTOM_COORD(_BaseMapRandomRowCoord);
```

### 5.2 GPU Instancing Optimization

When GPU Instancing is enabled, Custom Coord data is accessed directly from instance data, eliminating Vertex Stream requirements:

```hlsl
#ifdef NOVA_PARTICLE_INSTANCING_ENABLED
    // Direct access from instance data
    float4 customCoords[] = { float4(0,0,0,0), instanceData.customCoord1, instanceData.customCoord2 };
#endif
```

## 6. Performance Considerations

### 6.1 Optimization Strategy

1. **Minimize TEXCOORD Usage**: Each pass uses only required slots
2. **Conditional Compilation**: Features not used don't allocate slots
3. **GPU Instancing Priority**: Eliminates Vertex Stream requirements for Custom Coord
4. **Unified Custom Coord System**: Multiple features share same underlying data structure

### 6.2 Best Practices

- Enable GPU Instancing when possible to eliminate Vertex Stream overhead
- Use conditional compilation to exclude unused features
- Leverage Custom Coord system for new feature development
- Maintain consistent TEXCOORD allocation patterns across shader variants

## 7. Validation and Testing

### 7.1 Shader Compilation

All shader variants compile successfully with current TEXCOORD allocation:
- No conflicts in any shader configuration
- All features work correctly with their assigned slots

### 7.2 Runtime Validation

The Vertex Streams system automatically configures required streams based on material settings, ensuring correct data flow. Custom Coord usage is detected automatically and appropriate Custom1XYZW/Custom2XYZW streams are added when needed.

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
4. Leverage Custom Coord system for new random/animated parameter features
5. Implement feature-specific data packing to maximize efficiency