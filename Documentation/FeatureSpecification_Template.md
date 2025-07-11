# NOVA Shader Feature Specification Template

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->

## Feature Specification Document Format - Standard Template

### 📝 Basic Structure

```markdown
# NOVA Shader {Feature Name} Feature Specification

<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->

## 1. Overview
- Purpose and background of the feature
- Relationship with Unity standard features
- Value proposition

## 2. Implementation Status
### 2.1 Supported Shaders
| Shader | Status | Notes |
|--------|--------|-------|
| ParticlesUberUnlit | ✅/❌ | Details |

### 2.2 Implementation Components
#### Shader Properties
- Property names, types, and purposes

#### Shader Keywords  
- Keyword names and conditions

#### HLSL Functions
- Function names and roles

## 3. Technical Specifications
### 3.1 Algorithm
```hlsl
// Code example
```

### 3.2 Parameter Specifications
| Parameter | Type | Range | Description |

## 4. Usage
### 4.1 Basic Setup
### 4.2 Editor Settings
### 4.3 Performance Settings

## 5. Limitations and Notes
```

## Usage Instructions

1. Copy the template structure above
2. Replace `{Feature Name}` with the actual feature name
3. Fill in each section with feature-specific content
4. Use ✅ for implemented features and ❌ for not implemented
5. Include code examples in the Technical Specifications section
6. Document all limitations and important notes

## Excluded Elements

The following elements should NOT be included in feature specifications:
- Implementation completion checklist
- Troubleshooting sections
- Update history, dates, author information
- Version information

This template ensures consistency across all NOVA Shader feature documentation.