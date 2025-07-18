# NOVA Shader Documentation Guidelines

<!-- This document defines guidelines for all NOVA Shader documentation -->

## Language and Format Requirements

- **MUST** Use clear, structured markdown with proper headings and formatting
- **MUST** Follow consistent naming conventions for files and sections
- **MUST** Write concise, technical documentation focused on current state only
- **MUST** Prioritize essential information and eliminate redundant content
- **MUST** Avoid excessive code examples and verbose explanations

## Document Format Standards

### Specification Documents
- **MUST** Follow the template defined in `documentation/FeatureSpecification_Template.md`
- **MUST** Use standardized section structure
- **MUST** Include implementation status tables
- **MUST** Provide technical specifications with code examples

### Excluded Elements
- **MUST NOT** include update history, dates, or author information
- **MUST NOT** include troubleshooting sections
- **MUST NOT** include implementation completion checklists
- **MUST NOT** include version tracking information
- **MUST NOT** include future extension possibilities or speculative content
- **MUST NOT** include verbose code examples when brief descriptions suffice
- **MUST NOT** include detailed migration guides beyond essential points

## File Naming Conventions

- Feature Specifications: `{FeatureName}_Specification.md`
- System Architecture: `{SystemName}_SystemArchitecture.md`
- Templates: `{DocumentType}_Template.md`

## Content Structure Requirements

- Use numbered sections (1., 2., 3., etc.)
- Include code blocks with proper syntax highlighting (only when essential)
- Use tables for structured data
- Use emoji indicators (✅/❌) for status information
- Keep content focused on current implementation state
- Consolidate related information to avoid duplication
- Replace verbose explanations with concise summaries
- Eliminate redundant sections and merge overlapping content

## Guideline Compliance

All NOVA Shader documentation **MUST** include the following compliance statement at the beginning:

```markdown
<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->
```

This ensures consistency and AI agent readability across all documentation files.

## Content Optimization Principles

### Information Priority
- **High Priority**: Core technical specifications, implementation status, essential constraints
- **Medium Priority**: Architecture overview, key design decisions, critical limitations
- **Low Priority**: Detailed code examples, verbose explanations, migration details

### Conciseness Guidelines
- Replace detailed code blocks with brief functional descriptions
- Consolidate multiple similar examples into single representative cases
- Convert step-by-step procedures into summary points
- Merge redundant sections that cover similar topics
- Eliminate sections that provide minimal technical value

### Content Reduction Strategies
- **Code Examples**: Include only essential examples, replace others with descriptions
- **Architecture Diagrams**: Use concise text descriptions instead of complex ASCII art
- **Implementation Details**: Focus on "what" and "why", minimize "how" unless critical
- **Validation Sections**: Summarize validation approaches rather than detailed procedures
- **Extension Guidelines**: Provide implementation requirements rather than step-by-step guides