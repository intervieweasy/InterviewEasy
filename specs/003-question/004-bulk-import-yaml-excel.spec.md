
---

# 📄 Spec 003-question/004 — Bulk Import (YAML & Excel)

Save as: `specs\003-question\004-bulk-import-yaml-excel.spec.md`

---

```markdown
# Spec: Bulk Import — YAML & Excel

**Service:** Question
**Phase:** 003-question
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Some clients prefer authoring questions in YAML (git-friendly, better
for code review) or Excel (non-technical authors). This spec extends
the JSON import (Spec 003-question-003) to support YAML and Excel.

All formats converge to the same internal bundle representation and
use the same import pipeline.

---

## 2. Scope

### In Scope

- YAML import format
- Excel (.xlsx) import format with multiple sheets
- Format auto-detection
- Conversion to canonical JSON model
- Same validation + reporting pipeline as JSON

### Out of Scope

- CSV import (Excel covers most use cases — CSV can be deferred)
- Markdown-only import
- Custom formats (e.g., XML)

---

## 3. User Stories

- **US-1:** As a **developer-client**, I want to upload a YAML file,
  so that questions can live in git and be code-reviewed.

- **US-2:** As an **HR recruiter (non-technical)**, I want to upload
  an Excel file with questions, so that I don't need JSON knowledge.

- **US-3:** As a **client**, I want the format to be detected
  automatically, so that I don't have to specify it.

- **US-4:** As a **client**, I want Excel errors to reference cell
  coordinates (e.g., A5), so that I can fix them.

---

## 4. Functional Requirements

### 4.1 Format Detection

- **FR-1:** The import endpoint SHALL detect format from:
  1. File extension (`.json`, `.yaml`/`.yml`, `.xlsx`)
  2. Content-Type header
  3. Content sniffing (if extension is ambiguous)

- **FR-2:** If format cannot be detected, return 415.

- **FR-3:** Format SHALL be stored on the job for reporting.

### 4.2 YAML Format

- **FR-4:** YAML SHALL mirror the JSON schema (Spec 003-question-003).

- **FR-5:** YAML SHALL be parsed with `YamlDotNet`.

- **FR-6:** YAML anchors, aliases, and multi-line strings SHALL be
  supported.

- **FR-7:** YAML SHALL be UTF-8 encoded.

- **FR-8:** YAML comments SHALL be ignored.

- **FR-9:** YAML SHALL NOT allow arbitrary object construction
  (security — disable type tags).

- **FR-10:** After parsing, YAML SHALL be converted to the canonical
  bundle model (same as JSON).

### 4.3 Excel Format

- **FR-11:** Excel import SHALL use the `.xlsx` format (not `.xls`).

- **FR-12:** The workbook SHALL have these sheets:
  - `Course` — one row describing the course
  - `Questions` — one row per question
  - `TestCases` — one row per test case, referencing question by
    external_id
  - `Topics` — optional, one row per topic mapping to questions

- **FR-13:** The `Questions` sheet SHALL have these columns:
  - `external_id`
  - `title`
  - `difficulty`
  - `language`
  - `topics` (comma-separated)
  - `problem_statement` (markdown, multi-line in cell)
  - `constraints` (newline-separated)
  - `examples` (JSON string in cell)
  - `function_signature` (JSON string in cell)
  - `starter_code` (multi-line code in cell)
  - `solution_code` (multi-line code in cell)
  - `time_limit_ms`
  - `memory_limit_mb`

- **FR-14:** The `TestCases` sheet SHALL have these columns:
  - `question_external_id`
  - `external_id`
  - `type` (sample | hidden | performance)
  - `name`
  - `input_data` (JSON string)
  - `expected_output` (JSON string)
  - `points`
  - `explanation`

- **FR-15:** Excel parsing SHALL use the `ClosedXML` library (MIT
  licensed, .NET-native).

- **FR-16:** Column headers SHALL be case-insensitive and trimmed.

- **FR-17:** Missing required columns SHALL return 422 with the
  column name.

- **FR-18:** Row-level errors SHALL report as `Questions!A5` style
  cell references.

- **FR-19:** Multi-line cells SHALL preserve line breaks.

- **FR-20:** Excel formulas SHALL be evaluated to their cached values
  (no live formula evaluation).

### 4.4 Template Download

- **FR-21:** `GET /api/v1/import/templates/json` SHALL return a
  sample JSON bundle.

- **FR-22:** `GET /api/v1/import/templates/yaml` SHALL return a
  sample YAML bundle.

- **FR-23:** `GET /api/v1/import/templates/xlsx` SHALL return a
  pre-formatted Excel template with column headers and one example row.

- **FR-24:** Templates SHALL include inline comments explaining each
  field.

### 4.5 Same Pipeline

- **FR-25:** After parsing, all formats SHALL use the same validation
  pipeline as JSON (Spec 003-question-003).

- **FR-26:** Reports SHALL indicate source format but use the same
  error structure.

- **FR-27:** Idempotency (file hash) SHALL work across formats.

### 4.6 Error Reporting

- **FR-28:** YAML errors SHALL include line and column references
  from the YAML parser.

- **FR-29:** Excel errors SHALL include sheet name + cell reference
  (e.g., `Questions!B5`).

- **FR-30:** Error messages SHALL be actionable (e.g., "Column
  'difficulty' must be one of: basic, medium, advanced").

---

## 5. Non-Functional Requirements

- **NFR-1:** YAML parsing SHALL NOT be vulnerable to billion laughs
  or arbitrary type attacks.

- **NFR-2:** Excel parsing SHALL NOT execute macros.

- **NFR-3:** 500-question YAML file SHALL import in under 60 seconds.

- **NFR-4:** 500-question Excel file SHALL import in under 90 seconds.

- **NFR-5:** Template downloads SHALL respond in under 500ms.

- **NFR-6:** YAML and Excel parsing SHALL use memory under 200 MB for
  a 50 MB file.

---

## 6. Domain Model

### 6.1 Importer Abstraction

```csharp
public interface IQuestionBundleImporter
{
    string[] SupportedExtensions { get; }
    string Format { get; }
    Task<QuestionBundle> ParseAsync(Stream stream,
        CancellationToken ct);
}

public sealed class JsonQuestionBundleImporter : IQuestionBundleImporter { }
public sealed class YamlQuestionBundleImporter : IQuestionBundleImporter { }
public sealed class ExcelQuestionBundleImporter : IQuestionBundleImporter { }