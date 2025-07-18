# NOVA Shader Project CLAUDE.md

<!-- This file contains project-specific instructions for Claude Code -->

## Basic Principles

- **MUST** Prioritize @reference format for instruction documentation
- **MUST** Keep this file concise and split details into dedicated files

## Project Overview

NOVA Shader is a high-performance particle shader system.

### Version Requirements
- **MUST** Minimum supported Unity version: 2022.3 LTS

### Design Decisions
- **MUST** Unity's Texture Sheet Animation is deprecated in NOVA
- **MUST** Use Flip Book and Flip Book Blending as replacements for Texture Sheet Animation functionality
- **MUST** TintColorMode enum ordering differs from BaseMapMode due to backward compatibility (FlipBookBlending at index 2, FlipBook at index 3)

## Documentation Guidelines

- **MUST** All CLAUDE.md files and .md files in the Documentation folder related to NOVA must be written in English and in a format that is easy for AI agents to understand
- **MUST** Follow @documentation/documentation_guidelines.md
- **MUST** Adhere to the following specification and design document formats:
  - Feature Specifications: @documentation/FeatureSpecification_Template.md

### Documentation Editing Protocol

**CRITICAL**: Before editing any .md file in the Documentation folder, you **MUST** follow this protocol:

#### Step 1: Pre-Edit Verification
- **MUST** Read @documentation/documentation_guidelines.md completely
- **MUST** Identify what content is prohibited:
  - ❌ Troubleshooting sections
  - ❌ Implementation completion checklists  
  - ❌ Update history, dates, or author information
  - ❌ Version tracking information
  - ❌ Debug steps or procedural guides

#### Step 2: Content Planning
- **MUST** Plan edits to focus on current implementation state only
- **MUST** Ensure technical specifications include code examples
- **MUST** Use standardized section structure with numbered sections
- **MUST** Include implementation status tables where appropriate

#### Step 3: Edit Execution
- **MUST** Add only content that enhances technical specification
- **MUST** Use ✅/❌ emoji indicators for status information
- **MUST** Structure content with proper markdown formatting

#### Step 4: Post-Edit Compliance Check
- **MUST** Verify no prohibited content was added
- **MUST** Confirm focus remains on current implementation state
- **MUST** Check that technical specifications are complete

### Prohibited Content Enforcement

If you find yourself wanting to add any of these, **STOP** and restructure as technical specifications:
- "Troubleshooting" → "Technical Specifications"
- "Common Issues" → "Implementation Details"  
- "Debug Steps" → "Configuration Requirements"
- "Test Checklists" → "Implementation Status Tables"

## Existing Specifications and Design Documents

- Random Row Selection Feature: @documentation/RandomRowSelection_Specification.md
- Custom Coord System: @documentation/CustomCoord_SystemArchitecture.md
- UIParticles Limitations: @documentation/UIParticles_Limitations.md
- TEXCOORD Usage Strategy: @documentation/TEXCOORD_Usage_Strategy.md

## Recent Development History

### Tint Color FlipBook Feature Implementation (PR #153)
**Status**: Completed and ready for review

**Overview**: Added FlipBook (Texture2DArray) functionality to Tint Color to achieve feature parity with Base Color while maintaining full backward compatibility.

**Pull Request Guidelines**:
- **MUST** Write PR titles in English and PR descriptions/commit messages in Japanese