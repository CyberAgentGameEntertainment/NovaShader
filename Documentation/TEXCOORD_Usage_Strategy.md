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

Custom Coord data is transferred using TRANSFER_CUSTOM_COORD macro, which automatically selects between instance data (GPU Instancing) or vertex input (normal rendering). This ensures consistent TEXCOORD indices across all shader variants.

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

### 5.1 Custom Coord Integration

Custom Coord uses TEXCOORD1/2 for input consistency. GPU Instancing eliminates Vertex Stream requirements by accessing data directly from instance buffer. For details, see @documentation/CustomCoord_SystemArchitecture.md.

## 6. Performance Considerations

### 6.1 Optimization Guidelines

- **TEXCOORD Efficiency**: Each pass uses only required slots through conditional compilation
- **GPU Instancing**: Preferred for eliminating Vertex Stream overhead
- **Shader Keywords**: Use `shader_feature_local_vertex/fragment` for faster compilation and reduced memory usage
- **Custom Coord**: Unified system allows multiple features to share data structure

## 7. Validation

All shader variants compile successfully with current TEXCOORD allocation. The Vertex Streams system automatically configures required streams based on material settings, adding Custom1XYZW/Custom2XYZW streams when Custom Coord is used.

## 8. Future Considerations

### 8.1 Available Slots

Currently available for future features:
- Forward Pass: None (TEXCOORD14 is the last available)
- ShadowCaster: TEXCOORD9-15 (simplified pass has more available)
- DepthNormals: TEXCOORD11-15 (simplified pass has more available)

### 8.2 Expansion Strategy

When additional TEXCOORD slots are needed: utilize GPU Instancing, leverage Custom Coord system for animated parameters, implement data packing, or exclude features in specific passes.