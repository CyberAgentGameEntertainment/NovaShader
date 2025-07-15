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

### 2.3 StableRandom Limitations

| Stream | Standard Particles | UIParticles | Reason |
|--------|-------------------|-------------|--------|
| StableRandom.x | ✅ Available | ❌ Not available | Unity UI system does not support StableRandom streams |
| StableRandom.y | ✅ Available | ❌ Not available | Unity UI system does not support StableRandom streams |
| StableRandom.z | ✅ Available | ❌ Not available | Unity UI system does not support StableRandom streams |
| StableRandom.w | ✅ Available | ❌ Not available | Unity UI system does not support StableRandom streams |

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

UIParticles use a dedicated enum with restricted options:

```csharp
public enum UICustomCoord
{
    Unused = 0,
    Coord1X = 1,   // Custom1.x
    Coord1Y = 11,  // Custom1.y
    Coord2X = 2,   // Custom2.x
    Coord2Y = 12,  // Custom2.y
    // Note: Z/W components and StableRandom are not supported
}
```

## 5. Editor Integration

### 5.1 Automatic Feature Detection

The editor automatically detects UIParticles and applies appropriate limitations:

```csharp
bool isUIParticles = material.shader.name.Contains("UIParticles");
bool isRandomRowSelectionEnabled = !isUIParticles && /* other conditions */;
```

### 5.2 GUI Adaptations

- **Available Features**: Random Row Selection is now available for UIParticles using Custom Coord
- **Custom Coord Selection**: Limited to .xy components (Custom1.x/y, Custom2.x/y)
- **Setup Guidance**: Editor provides guidance for Custom Data configuration

## 6. Alternative Solutions

### 6.1 Manual Random Row Selection

For UIParticles requiring random row selection:

**Setup**:
1. Use Custom1.x for random values (0.0-1.0)
2. Pre-calculate random values in particle system or script
3. Configure Custom Vertex Streams: Custom1XYZW

**Implementation**:
```hlsl
// Manual random row selection using Custom1.x
half randomValue = input.customCoord1.x;
half rowCount = _BaseMapRowCount;
half framesPerRow = _BaseMapSliceCount / rowCount;
uint selectedRow = min(floor(randomValue * rowCount), rowCount - 1);
half frameProgress = FlipBookProgress(progress, framesPerRow);
half result = selectedRow * framesPerRow + frameProgress;
```

### 6.2 Scripted Random Value Generation

```csharp
// Example: Pre-calculate random values for Custom1.x
ParticleSystem.CustomDataModule customData = particleSystem.customData;
customData.enabled = true;
customData.mode = ParticleSystemCustomDataMode.Custom;

// Set random values for Custom1.x
AnimationCurve randomCurve = new AnimationCurve();
for (int i = 0; i < 10; i++)
{
    float time = i / 9.0f;
    float randomValue = UnityEngine.Random.Range(0.0f, 1.0f);
    randomCurve.AddKey(time, randomValue);
}
customData.vectorComponentCount = 1;
customData.SetVector(ParticleSystemCustomData.Custom1, ParticleSystemMinMaxCurve.Custom1X, randomCurve);
```

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

### 8.1 From Standard Particles to UIParticles

When converting materials from standard Particles to UIParticles:

1. **Review Custom Coord Usage**: Ensure only .xy components are used
2. **Configure Random Row Selection**: Use Custom1.x/y or Custom2.x/y for random values
3. **Update Vertex Streams**: Remove Z/W components and StableRandom streams  
4. **Test Functionality**: Verify all features work correctly

### 8.2 From UIParticles to Standard Particles

When converting from UIParticles to standard Particles:

1. **Enable Advanced Features**: Random Row Selection and Z/W components become available
2. **Update Vertex Streams**: Add necessary streams for new features
3. **Review Performance**: Standard Particles may have different performance characteristics

## 9. Technical Architecture

### 9.1 Shader Variants

UIParticles use separate shader variants to enforce limitations:

- **UIParticlesUberUnlit.shader**: UI-optimized unlit variant
- **UIParticlesUberLit.shader**: UI-optimized lit variant
- **Automatic Keyword Management**: Incompatible keywords are automatically disabled

### 9.2 Editor Integration

```csharp
// Automatic feature detection and restriction
private static bool IsUIParticlesShader(Material material)
{
    return material.shader.name.Contains("UIParticles");
}

// Automatic disabling of incompatible features
if (IsUIParticlesShader(material))
{
    material.SetFloat("_BaseMapRandomRowSelectionEnabled", 0.0f);
    // Show warning message
}
```

## 10. Summary

UIParticles provide UI-optimized particle rendering with specific limitations due to Unity UI system constraints. Most features including Random Row Selection are available using the Custom Coord system (.xy components). UIParticles offer better performance and compatibility for UI-based particle effects.

### Key Limitations Summary

- **Custom Coord**: Only .xy components available (but sufficient for Random Row Selection)
- **StableRandom**: Not supported (use Custom Data instead)
- **Random Row Selection**: ✅ Supported via Custom Coord system
- **Advanced Features**: Limited to .xy components due to Unity UI constraints

### Recommended Usage

- **UI Effects**: Use UIParticles for UI-integrated particle effects
- **Advanced Effects**: Use standard Particles for complex particle systems
- **Performance-Critical**: UIParticles offer better performance for simple effects