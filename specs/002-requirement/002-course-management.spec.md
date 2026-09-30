
---

# 📄 Spec 002-requirement/002 — Course Management

Save as: `specs\002-requirement\002-course-management.spec.md`

---

```markdown
# Spec: Course Management

**Service:** Requirement
**Phase:** 002-requirement
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

A **Course** is a logical grouping of interview questions within a
Requirement. Examples:
- C# & .NET
- SQL & Databases
- System Design
- DevOps
- Frontend

Each course has:
- A code (e.g., `CSHARP_DOTNET`)
- A name (e.g., `C# & .NET`)
- Supported languages
- A weight (contributes to overall scoring)

Courses enable:
- Structured question organization
- Per-course scoring in feedback
- Flexible interview design (choose 3 of 5 courses)

This spec defines course creation, update, and management within a
requirement.

---

## 2. Scope

### In Scope

- Course entity and invariants
- Course CRUD operations
- Course weightage
- Language support per course
- Ordering of courses within a requirement
- Course soft-delete

### Out of Scope

- Questions within a course (Phase 3)
- Test cases (Phase 3)
- Course validation against questions (Phase 3)
- Standard/template courses (future)

---

## 3. User Stories

- **US-1:** As a **client recruiter**, I want to add courses to a
  requirement, so that I can organize questions by topic.

- **US-2:** As a **client recruiter**, I want to weight courses, so
  that I can reflect what matters most for the role.

- **US-3:** As an **interviewer**, I want to see courses grouped, so
  that I can pick questions efficiently during the interview.

- **US-4:** As a **client admin**, I want to reorder courses, so that
  the interview flow makes sense.

- **US-5:** As a **client recruiter**, I want to remove a course, so
  that I can adjust to a role's changing needs.

---

## 4. Functional Requirements

### 4.1 Course Entity

- **FR-1:** A `RequirementCourse` entity SHALL exist with:
  - `Id` (Guid, PK)
  - `RequirementId` (Guid, FK)
  - `Code` (string, 2–50 chars, uppercase, unique per requirement)
  - `Name` (string, 2–200 chars)
  - `Description` (string, 0–1000 chars)
  - `Languages` (string array, 1–10 items)
  - `Weight` (int, 0–100)
  - `DisplayOrder` (int)
  - `CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt`
  - `IsDeleted`, `DeletedAt`, `DeletedBy`

- **FR-2:** `Code` SHALL:
  - Be uppercase letters, digits, and underscores only
  - Match `^[A-Z][A-Z0-9_]{1,49}$`
  - Not start with a digit
  - Be unique within the requirement (case-insensitive)

- **FR-3:** `Code` SHALL be immutable after creation.

- **FR-4:** `Name` SHALL be trimmed, 2–200 characters.

- **FR-5:** `Languages` SHALL be a non-empty array of unique,
  lowercase language codes from the supported set:
  `csharp`, `dotnet`, `sql`, `python`, `java`, `javascript`,
  `typescript`, `go`, `text`.

- **FR-6:** `Weight` SHALL be 0–100. Sum of weights across all
  courses in a requirement SHALL equal 100 when the requirement is
  submitted for approval (validation happens in Spec 003).

- **FR-7:** `DisplayOrder` SHALL be an integer ≥ 0, unique per
  requirement.

### 4.2 Course CRUD

- **FR-8:** `POST /api/v1/requirements/{reqId}/courses` SHALL create
  a course.

- **FR-9:** `GET /api/v1/requirements/{reqId}/courses` SHALL list
  courses for a requirement, ordered by `DisplayOrder`.

- **FR-10:** `GET /api/v1/requirements/{reqId}/courses/{id}` SHALL
  return a single course.

- **FR-11:** `PUT /api/v1/requirements/{reqId}/courses/{id}` SHALL
  update name, description, languages, weight, and order.

- **FR-12:** `DELETE /api/v1/requirements/{reqId}/courses/{id}` SHALL
  soft-delete the course.

- **FR-13:** All mutating endpoints SHALL require the parent
  requirement to be in `Draft` status.

### 4.3 Language Support

- **FR-14:** A course's `Languages` SHALL define which languages are
  available for questions in that course.

- **FR-15:** When adding a question to a course (Phase 3), the
  question's language SHALL be a subset of the course's languages.

- **FR-16:** Languages SHALL be ordered per the global list:
  csharp, dotnet, sql, python, java, javascript, typescript, go, text.

### 4.4 Weight Rules

- **FR-17:** Course `Weight` SHALL be 0–100 inclusive.

- **FR-18:** Weight SHALL NOT exceed 100 in total per requirement.

- **FR-19:** Weight sum validation SHALL be enforced at requirement
  submission (Spec 003), not on individual course updates.

- **FR-20:** A course with weight 0 is allowed — it contributes
  questions but no score.

### 4.5 Ordering

- **FR-21:** `DisplayOrder` SHALL be set to the highest existing +1
  when creating a course (unless specified).

- **FR-22:** `POST /api/v1/requirements/{reqId}/courses/reorder`
  SHALL accept a list of course IDs in the new order and update all
  `DisplayOrder` values atomically.

- **FR-23:** Reordering SHALL NOT affect course contents.

### 4.6 Deletion

- **FR-24:** Soft-deleting a course SHALL soft-delete its questions
  too (Phase 3 cascade).

- **FR-25:** Deleted courses SHALL NOT appear in listings.

- **FR-26:** A requirement in `Draft` with courses SHALL be
  modifiable; in `Active`, courses are read-only.

### 4.7 Tenant Isolation

- **FR-27:** All course queries SHALL be scoped to the current tenant
  and the specified requirement.

- **FR-28:** Cross-tenant access SHALL return 404.

---

## 5. Non-Functional Requirements

- **NFR-1:** Course creation SHALL respond in under 200ms (p95).

- **NFR-2:** Course listing SHALL respond in under 100ms (p95).

- **NFR-3:** Reordering SHALL be atomic — no partial updates.

- **NFR-4:** The system SHALL support up to 50 courses per requirement.

- **NFR-5:** The system SHALL support up to 10,000 courses per tenant.

---

## 6. Domain Model

### 6.1 Entity: RequirementCourse

```csharp
public sealed class RequirementCourse : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid RequirementId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyList<string> Languages { get; private set; }
    public int Weight { get; private set; }
    public int DisplayOrder { get; private set; }

    public static RequirementCourse Create(
        Guid requirementId, string code, string name,
        string description, IList<string> languages,
        int weight, int displayOrder, Guid createdBy);

    public void Update(string name, string description,
        IList<string> languages, int weight,
        Guid updatedBy);

    public void Reorder(int newDisplayOrder, Guid updatedBy);
    public void SoftDelete(Guid deletedBy);
}