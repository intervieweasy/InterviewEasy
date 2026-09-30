
---

# 📄 Spec 003-question/002 — Test Case Management

Save as: `specs\003-question\002-test-case-management.spec.md`

---

```markdown
# Spec: Test Case Management

**Service:** Question
**Phase:** 003-question
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

A **Test Case** validates a candidate's solution. Every question has:

- **Sample test cases** — visible to candidates during the interview,
  used to demonstrate expected behavior
- **Hidden test cases** — visible only to the interviewer, used for
  scoring

Test cases enable automated grading. The Sandbox service (Phase 4)
executes candidate code against test cases and returns pass/fail
results.

This spec defines test case entity, storage, validation, and CRUD.

---

## 2. Scope

### In Scope

- Test case entity and invariants
- Sample vs hidden test cases
- Input/expected output formats
- Per-language test case input (rare — most are language-agnostic)
- Test case points (for weighted scoring)
- Test case CRUD
- Test case ordering
- Test case verification (via Sandbox — Phase 4)

### Out of Scope

- Sandbox execution (Phase 4)
- Scoring logic (Phase 4)
- Performance test cases (Phase 4)
- Random/generated test cases (future)

---

## 3. User Stories

- **US-1:** As a **client recruiter**, I want to add sample test
  cases, so that candidates see examples.

- **US-2:** As a **client recruiter**, I want to add hidden test
  cases, so that scoring isn't gameable.

- **US-3:** As an **interviewer**, I want to see all test cases, so
  that I understand what the candidate must pass.

- **US-4:** As a **client admin**, I want to mark a test case as
  performance-sensitive, so that it enforces time limits.

- **US-5:** As a **client recruiter**, I want to reorder test cases,
  so that they're presented logically.

---

## 4. Functional Requirements

### 4.1 Test Case Entity

- **FR-1:** A `TestCase` entity SHALL exist with:
  - `Id` (Guid, PK)
  - `QuestionId` (Guid, FK)
  - `ExternalId` (string?, client's own ID)
  - `Type` (enum: Sample, Hidden, Performance)
  - `Name` (string, 1–200 chars)
  - `InputData` (JSONB — structured input)
  - `InputGenerator` (JSONB, optional — for generated inputs)
  - `ExpectedOutput` (JSONB)
  - `VisibleToCandidate` (bool)
  - `Points` (int, default 10)
  - `Explanation` (string?, 0–500 chars)
  - `DisplayOrder` (int)
  - `IsPerformanceTest` (bool, default false)
  - `CreatedAt`, `UpdatedAt`

- **FR-2:** `Type` SHALL be one of: `sample`, `hidden`, `performance`.

- **FR-3:** `VisibleToCandidate` SHALL be `true` for Sample, `false`
  for Hidden and Performance.

- **FR-4:** Sample test cases SHALL always be visible to candidates.

- **FR-5:** Hidden test cases SHALL never be visible to candidates.

- **FR-6:** Performance test cases SHALL have `IsPerformanceTest = true`
  and stricter time limits.

### 4.2 Input Data

- **FR-7:** `InputData` SHALL be a JSON object with parameter names
  matching the function signature.

  Example for `int FindMax(int[] arr)`:
  ```json
  { "arr": [3, 7, 2, 9, 1] }