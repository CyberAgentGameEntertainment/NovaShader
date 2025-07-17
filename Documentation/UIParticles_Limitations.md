# NOVA Shader UIParticles Limitations Specification

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->

## 1. Overview

UIParticles shaders in NOVA are specialized versions of standard Particle shaders designed for Unity UI system integration. Due to Unity UI system architectural constraints, UIParticles have specific limitations that differ from standard Particle shaders.

## 2. Unity UI System Constraints

### 2.1 Vertex Data Structure Limitations

Unity UI system uses UIVertex structure which has fundamental constraints:

```csharp
// Unity UI system's UIVertex structure
struct UIVertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector4 tangent;
    public Color32 color;
    public Vector4 uv0;    // ✅ Available (.xy components)
    public Vector4 uv1;    // ✅ Available (.xy components)
    public Vector4 uv2;    // ❌ Limited to .xy components
    public Vector4 uv3;    // ❌ Limited to .xy components
}
```

### 2.2 Custom Coord Limitations

| Component | Standard Particles | UIParticles | Notes |
|-----------|-------------------|-------------|-------|
| Custom1.x | ✅ Available | ✅ Available | Mapped to uv2.x |
| Custom1.y | ✅ Available | ✅ Available | Mapped to uv2.y |
| Custom1.z | ✅ Available | ❌ Not available | Unity UI system constraint |
| Custom1.w | ✅ Available | ❌ Not available | Unity UI system constraint |
| Custom2.x | ✅ Available | ✅ Available | Mapped to uv3.x |
| Custom2.y | ✅ Available | ✅ Available | Mapped to uv3.y |
| Custom2.z | ✅ Available | ❌ Not available | Unity UI system constraint |
| Custom2.w | ✅ Available | ❌ Not available | Unity UI system constraint |


## 3. Feature Limitations

### 3.1 Random Row Selection

| Feature | Standard Particles | UIParticles | Notes |
|---------|-------------------|-------------|-------|
| Random Row Selection | ✅ Supported | ✅ Supported | Uses Custom Coord system (Custom1.x/y, Custom2.x/y) |
| Custom Data Integration | ✅ Available | ✅ Available | Random Between Two Constants with Custom Coord |

**Implementation**: Random Row Selection now uses Unity's Custom Data system with Custom Coord, making it available for both Standard Particles and UIParticles.

### 3.2 Advanced Custom Coord Features

Features requiring Z/W components are not supported:

- **3D Texture Sampling**: Limited to 2D texture coordinates
- **Quaternion Operations**: Cannot use full 4-component data
- **Advanced Deformation**: Limited to 2D deformation parameters

## 4. UICustomCoord Enum

UIParticles use a dedicated enum with restricted options. For detailed Custom Coord system architecture and enum definitions, see @documentation/CustomCoord_SystemArchitecture.md.

**Key limitation**: UICustomCoord only supports .xy components (Custom1.x/y, Custom2.x/y) due to Unity UI system constraints.

## 5. Editor Integration

### 5.1 Automatic Feature Detection

The editor automatically detects UIParticles and applies appropriate limitations for Custom Coord components:

```csharp
bool isUIParticles = material.shader.name.Contains("UIParticles");
// UIParticles automatically restrict Custom Coord to .xy components only
```

### 5.2 GUI Adaptations

Editor limits Custom Coord selection to .xy components (Custom1.x/y, Custom2.x/y) for UIParticles, with automatic guidance for configuration.

## 6. Implementation Notes

For Random Row Selection implementation details, see @documentation/RandomRowSelection_Specification.md.

**UIParticles constraint**: Only Custom1.x/y and Custom2.x/y components are available.

## 7. Performance Considerations

### 7.1 UIParticles Optimization

- **Reduced Vertex Data**: Only .xy components reduce memory usage
- **Simplified Shaders**: Fewer features result in faster compilation
- **UI Batching**: Better compatibility with Unity UI batching system

### 7.2 Feature Impact

| Feature | Performance Impact | UIParticles Support |
|---------|-------------------|-------------------|
| Basic FlipBook | Minimal | ✅ Supported |
| FlipBook Blending | Low | ✅ Supported |
| Random Row Selection | Medium | ✅ Supported (Custom Coord) |
| Complex Custom Coord | High | ❌ Limited support |

## 8. Migration Guidelines

**To UIParticles**: Ensure only .xy components are used, configure Random Row Selection with Custom1.x/y or Custom2.x/y.

**From UIParticles**: Z/W components become available for advanced features.

## 9. Technical Architecture

UIParticles use separate shader variants (UIParticlesUberUnlit/Lit.shader) with automatic keyword management. For editor integration details, see @documentation/CustomCoord_SystemArchitecture.md.

## 10. Summary

UIParticles provide UI-optimized particle rendering with specific limitations due to Unity UI system constraints. Most features including Random Row Selection are available using the Custom Coord system (.xy components). UIParticles offer better performance and compatibility for UI-based particle effects.

**Key Limitations**: Only .xy components available for Custom Coord, sufficient for Random Row Selection and most UI effects.

**Usage Recommendation**: Use UIParticles for UI-integrated effects, standard Particles for complex 3D effects requiring Z/W components.