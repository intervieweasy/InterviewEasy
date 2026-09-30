# Spec: Question CRUD

**Service:** Question
**Phase:** 003-question
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

A **Question** is the core artifact of an interview. It represents a
problem the candidate must solve during a live session. Questions belong
to a course within a requirement (Phase 2). Each question:

- Has a problem statement (markdown)
- Targets a specific language (csharp, sql, python, etc.)
- Has difficulty (basic, medium, advanced)
- Has starter code, solution code
- Has multiple test cases (spec 002)
- May be imported in bulk (specs 003, 004)

This spec defines the Question entity, its CRUD operations, and its
relationship to courses.

---

## 2. Scope

### In Scope

- Question entity and invariants
- Question CRUD API
- Question-course relationship
- Difficulty levels
- Language targeting
- Topics and tags
- Starter code and solution code
- Per-language code fields
- Question soft-delete

### Out of Scope

- Test cases (Spec 003-question-002)
- Bulk import (Specs 003-question-003, 004)
- Solution verification (Spec 003-question-005)
- Search & filtering (Spec 003-question-006)
- Live interview question push (Phase 5)

---

## 3. User Stories

- **US-1:** As a **client recruiter**, I want to create a question
  manually, so that I can add specific problems to a course.

- **US-2:** As a **client recruiter**, I want to edit a question's
  statement, so that I can fix typos or clarify requirements.

- **US-3:** As an **interviewer**, I want to view a question's
  details, so that I know what to ask during the interview.

- **US-4:** As a **client admin**, I want to delete a question, so
  that outdated content doesn't clutter the course.

- **US-5:** As a **client recruiter**, I want questions tagged with
  topics, so that I can organize them by skill.

---

## 4. Functional Requirements

### 4.1 Question Entity

- **FR-1:** A `RequirementQuestion` entity SHALL exist with:
  - `Id` (Guid, PK)
  - `RequirementId` (Guid, FK)
  - `CourseId` (Guid, FK)
  - `ExternalId` (string?, client's own ID)
  - `Title` (string, 5–500 chars)
  - `Difficulty` (enum: Basic, Medium, Advanced)
  - `Language` (string, from supported set)
  - `Topics` (string array, 0–20 items)
  - `ProblemStatement` (JSONB — markdown + constraints + examples)
  - `FunctionSignature` (JSONB — per language)
  - `StarterCode` (JSONB — per language)
  - `SolutionCode` (JSONB — per language)
  - `TimeLimitMs` (int, default 2000)
  - `MemoryLimitMb` (int, default 256)
  - `EvaluationConfig` (JSONB)
  - `CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt`
  - `IsDeleted`, `DeletedAt`, `DeletedBy`

- **FR-2:** `Title` SHALL be trimmed, 5–500 characters.

- **FR-3:** `Difficulty` SHALL be one of: `basic`, `medium`, `advanced`.

- **FR-4:** `Language` SHALL be from the supported set: `csharp`,
  `dotnet`, `sql`, `python`, `java`, `javascript`, `typescript`,
  `go`, `text`.

- **FR-5:** A question's `Language` SHALL be a subset of its course's
  `Languages` (validated on create/update).

- **FR-6:** `Topics` SHALL be up to 20 unique, lowercase, trimmed
  strings, each 1–50 characters.

- **FR-7:** `ExternalId` SHALL be 1–100 chars if provided. Unique
  per requirement.

- **FR-8:** `TimeLimitMs` SHALL be 100–30000.

- **FR-9:** `MemoryLimitMb` SHALL be 16–2048.

### 4.2 Problem Statement Structure

- **FR-10:** `ProblemStatement` JSONB SHALL contain:
  ```json
  {
    "markdown": "# Problem\n\n...",
    "constraints": ["1 <= n <= 10^5", "..."],
    "examples": [
      {
        "input": "[3, 7, 2, 9, 1]",
        "output": "9",
        "explanation": "9 is the largest"
      }
    ]
  }