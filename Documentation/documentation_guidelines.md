# NOVA Shader Documentation Guidelines

<!-- This document defines guidelines for all NOVA Shader documentation -->

## Language and Format Requirements

- **MUST** Use clear, structured markdown with proper headings and formatting
- **MUST** Follow consistent naming conventions for files and sections
- **MUST** Write concise, technical documentation focused on current state only

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

## File Naming Conventions

- Feature Specifications: `{FeatureName}_Specification.md`
- System Architecture: `{SystemName}_SystemArchitecture.md`
- Templates: `{DocumentType}_Template.md`

## Content Structure Requirements

- Use numbered sections (1., 2., 3., etc.)
- Include code blocks with proper syntax highlighting
- Use tables for structured data
- Use emoji indicators (✅/❌) for status information
- Keep content focused on current implementation state

## Guideline Compliance

All NOVA Shader documentation **MUST** include the following compliance statement at the beginning:

```markdown
<!-- This document is written in accordance with @documentation/documentation_guidelines.md -->
```

This ensures consistency and AI agent readability across all documentation files.