
---

# 📄 Spec 003-question/006 — Question Search & Filter

Save as: `specs\003-question\006-question-search.spec.md`

---

```markdown
# Spec: Question Search & Filter

**Service:** Question
**Phase:** 003-question
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Interviewers need to find questions quickly during a live interview.
Clients need to find questions to edit. Admins need to search across
tenants. A powerful search + filter is essential.

Questions have structured facets:
- Course
- Language
- Difficulty
- Topics
- Validation status
- Usage count (how many times asked)

Plus full-text search over the problem statement and title.

This spec defines search and filter semantics.

---

## 2. Scope

### In Scope

- Full-text search
- Faceted filtering (course, language, difficulty, topics, status)
- Sorting options
- Pagination
- Cross-requirement search
- Saved filters (search presets) — future
- Search performance

### Out of Scope

- Semantic/AI search
- Cross-tenant search (admin-only, simple)
- Elasticsearch migration (v1 uses PostgreSQL only)

---

## 3. User Stories

- **US-1:** As an **interviewer**, I want to filter questions by
  course + difficulty, so that I can pick the right one.

- **US-2:** As an **interviewer**, I want to search questions by
  title, so that I can find a specific one.

- **US-3:** As a **client recruiter**, I want to find all advanced
  C# questions, so that I can review them.

- **US-4:** As a **client admin**, I want to see how many times a
  question has been used, so that I can retire stale ones.

- **US-5:** As an **interviewer**, I want to sort by fewest times
  used, so that I can vary the interview.

---

## 4. Functional Requirements

### 4.1 Search Endpoint

- **FR-1:** `GET /api/v1/questions/search` SHALL search across all
  questions in the current tenant (or a specified requirement).

- **FR-2:** Query parameters SHALL include:
  - `q` (free-text query)
  - `requirementId`
  - `courseId`
  - `courseCode`
  - `language`
  - `difficulty`
  - `topics` (comma-separated, OR match by default)
  - `topicsMatch` (any | all — default `any`)
  - `validationStatus` (passed | failed | pending)
  - `minPoints` / `maxPoints`
  - `minUses` / `maxUses`
  - `hasSampleTests` (bool)
  - `hasHiddenTests` (bool)
  - `onlyUnvalidated` (bool)
  - `sortBy` (relevance | createdAt | updatedAt | difficulty | uses | title)
  - `sortDir` (asc | desc)
  - `pageNumber` (default 1)
  - `pageSize` (default 20, max 100)

- **FR-3:** All parameters SHALL combine with AND logic, except
  `topics` which uses OR (or AND if `topicsMatch=all`).

### 4.2 Full-Text Search

- **FR-4:** Free-text search (`q`) SHALL search:
  - Title (weight: high)
  - Problem statement markdown (weight: medium)
  - Topics (weight: high)
  - External ID (weight: high, exact match)

- **FR-5:** PostgreSQL full-text search (`tsvector` + `tsquery`)
  SHALL be used.

- **FR-6:** Search SHALL support:
  - Prefix matching (`find*` matches "find", "finding")
  - Phrase matching (`"find maximum"`)
  - Boolean operators (AND, OR, NOT)

- **FR-7:** Search results SHALL include a `rank` field (relevance
  score) when `q` is present.

- **FR-8:** Minimum query length SHALL be 2 characters.

- **FR-9:** Search SHALL be case-insensitive.

### 4.3 Filters

- **FR-10:** `requirementId` filter SHALL scope to one requirement.

- **FR-11:** `courseId` OR `courseCode` SHALL scope to one course
  (mutually exclusive).

- **FR-12:** `language` SHALL accept comma-separated values (OR).

- **FR-13:** `difficulty` SHALL accept comma-separated values (OR).

- **FR-14:** `topics` SHALL accept comma-separated values:
  - `topicsMatch=any` (default) — OR match
  - `topicsMatch=all` — AND match

- **FR-15:** `validationStatus` SHALL accept: passed, failed, pending.

- **FR-16:** `minPoints` / `maxPoints` SHALL filter by sum of test
  case points.

- **FR-17:** `minUses` / `maxUses` SHALL filter by count of
  interviews the question has been used in.

- **FR-18:** `hasSampleTests` / `hasHiddenTests` SHALL filter to
  questions that have or lack those test types.

### 4.4 Sorting

- **FR-19:** `sortBy=relevance` SHALL only be allowed when `q` is
  provided.

- **FR-20:** Default sort SHALL be `relevance` if `q` present, else
  `updatedAt DESC`.

- **FR-21:** `uses` sort SHALL use the count from the analytics
  read model.

- **FR-22:** `difficulty` sort SHALL follow the natural order:
  basic < medium < advanced.

### 4.5 Facets

- **FR-23:** The search endpoint SHALL return facet counts for:
  - Courses
  - Languages
  - Difficulties
  - Topics (top 20)

- **FR-24:** Facet counts SHALL reflect the current filter set,
  excluding the facet's own dimension.

- **FR-25:** Response SHALL include:
  ```json
  {
    "facets": {
      "courses": [
        { "id": "...", "name": "C# & .NET", "count": 45 }
      ],
      "languages": [
        { "code": "csharp", "count": 80 },
        { "code": "sql", "count": 25 }
      ],
      "difficulties": [
        { "level": "basic", "count": 40 },
        { "level": "medium", "count": 65 },
        { "level": "advanced", "count": 20 }
      ],
      "topics": [
        { "name": "arrays", "count": 30 },
        { "name": "linq", "count": 22 }
      ]
    }
  }