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

## Documentation Guidelines

- **MUST** All CLAUDE.md files and .md files in the Documentation folder related to NOVA must be written in English and in a format that is easy for AI agents to understand
- **MUST** Follow @documentation/documentation_guidelines.md
- **MUST** Adhere to the following specification and design document formats:
  - Feature Specifications: @documentation/FeatureSpecification_Template.md

## Existing Specifications and Design Documents

- Random Row Selection Feature: @documentation/RandomRowSelection_Specification.md
- Custom Coord System: @documentation/CustomCoord_SystemArchitecture.md