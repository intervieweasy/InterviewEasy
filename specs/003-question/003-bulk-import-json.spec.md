
---

# 📄 Spec 003-question/003 — Bulk Import (JSON)

Save as: `specs\003-question\003-bulk-import-json.spec.md`

---

```markdown
# Spec: Bulk Import — JSON

**Service:** Question
**Phase:** 003-question
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Clients often have hundreds or thousands of questions prepared offline.
Manually entering each one via the API is impractical. **Bulk import**
lets clients upload a JSON file containing an entire course's questions
+ test cases in one operation.

This spec defines the JSON import format, validation pipeline, error
reporting, and idempotency rules. YAML and Excel imports are covered
in Spec 003-question-004.

---

## 2. Scope

### In Scope

- JSON bundle format
- Import API endpoint
- File upload limits
- Validation rules
- Import job tracking
- Partial success handling
- Duplicate detection
- Error reporting per question
- Idempotency via file hash

### Out of Scope

- YAML / Excel imports (Spec 003-question-004)
- Question verification (Spec 003-question-005)
- Bulk export
- Bulk deletion

---

## 3. User Stories

- **US-1:** As a **client recruiter**, I want to upload a JSON file
  with 200 questions, so that I can prepare a course quickly.

- **US-2:** As a **client recruiter**, I want to see which questions
  failed validation, so that I can fix them and re-upload.

- **US-3:** As a **client admin**, I want the import to be idempotent,
  so that a retry doesn't duplicate questions.

- **US-4:** As a **client recruiter**, I want large imports to run in
  the background, so that I don't wait for the browser.

- **US-5:** As an **SRE**, I want import jobs to be tracked, so that I
  can debug failures.

---

## 4. Functional Requirements

### 4.1 JSON Bundle Format

- **FR-1:** A JSON bundle SHALL have the structure:
  ```json
  {
    "bundleVersion": "1.0",
    "course": {
      "code": "CSHARP_DOTNET",
      "name": "C# & .NET",
      "description": "...",
      "languages": ["csharp", "dotnet"]
    },
    "questions": [ ... ]
  }