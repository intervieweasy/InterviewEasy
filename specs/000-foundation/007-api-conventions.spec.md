
---

# 📄 Spec 007 — API Conventions

Save as: `specs\000-foundation\007-api-conventions.spec.md`

---

```markdown
# Spec: API Conventions

**Service:** Foundation
**Phase:** 000-foundation
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

InterviewEasy exposes REST APIs across 12+ services. Without shared
conventions, each service would invent its own URL structure, response
format, error codes, pagination, and versioning — creating an
inconsistent experience for frontend teams and integration partners.

This spec defines the **REST API conventions** that every service must
follow. It ensures that a frontend developer who consumes one service's
API already knows how to consume every other service's API.

---

## 2. Scope

### In Scope

- URL structure and naming
- HTTP method semantics
- HTTP status codes
- Request and response formats
- Error response format (Problem Details)
- Validation error format
- Pagination, sorting, filtering
- API versioning strategy
- Authentication and authorization headers
- Idempotency for POST/PUT
- Rate limiting headers
- OpenAPI (Swagger) documentation
- Content negotiation (JSON only)
- Time format and timezone handling
- Batch operations

### Out of Scope

- GraphQL (not used)
- gRPC (used only internally for Sandbox — separate spec)
- WebSocket / SignalR (Session service — separate spec)
- File upload endpoints (specific to Question import — its own spec)
- Streaming endpoints (recording playback — separate spec)

---

## 3. User Stories

- **US-1:** As a **frontend developer**, I want consistent URL patterns,
  so that I can guess endpoints without reading docs.

- **US-2:** As a **frontend developer**, I want standardized error
  responses, so that I can display validation errors uniformly.

- **US-3:** As an **integration partner**, I want versioned APIs, so
  that breaking changes don't disrupt my integration.

- **US-4:** As a **backend developer**, I want clear conventions, so
  that I don't have to make per-endpoint decisions.

- **US-5:** As an **SRE**, I want rate limiting and idempotency built
  into the API layer, so that clients cannot accidentally overload us.

---

## 4. Functional Requirements

### 4.1 URL Structure

- **FR-1:** All APIs SHALL be prefixed with `/api/v{version}`:
  - `/api/v1/tenants`
  - `/api/v1/requirements/{id}/courses`

- **FR-2:** Resource names SHALL be **plural nouns** in kebab-case:
  - ✅ `/api/v1/requirements`
  - ✅ `/api/v1/feedback-templates`
  - ❌ `/api/v1/requirement`
  - ❌ `/api/v1/FeedbackTemplates`

- **FR-3:** URLs SHALL NOT contain verbs — HTTP methods convey the
  action:
  - ✅ `POST /api/v1/tenants`
  - ❌ `POST /api/v1/tenants/create`

- **FR-4:** Sub-resources SHALL be nested under their parent:
  - `/api/v1/requirements/{requirementId}/courses`
  - `/api/v1/courses/{courseId}/questions`

- **FR-5:** Nesting SHALL NOT exceed two levels:
  - ✅ `/requirements/{id}/courses/{id}/questions`
  - ❌ `/requirements/{id}/courses/{id}/questions/{id}/test-cases`

- **FR-6:** Actions that don't map to CRUD SHALL use a sub-resource
  with a verb (kebab-case):
  - `POST /api/v1/requirements/{id}/approve`
  - `POST /api/v1/interviews/{id}/end`
  - `POST /api/v1/questions/{id}/verify`

- **FR-7:** Query parameters SHALL be camelCase:
  - `?pageSize=20&pageNumber=1&sortBy=createdAt&sortDir=desc`

### 4.2 HTTP Methods

- **FR-8:** The following method semantics SHALL be followed:
  - **GET** — Read, safe, idempotent, no side effects
  - **POST** — Create, or trigger non-CRUD actions
  - **PUT** — Full replace, idempotent
  - **PATCH** — Partial update (JSON Merge Patch or JSON Patch)
  - **DELETE** — Remove, idempotent

- **FR-9:** GET requests SHALL NOT have a request body.

- **FR-10:** DELETE requests SHALL be idempotent — repeated calls
  return the same success response.

- **FR-11:** PATCH SHALL use JSON Merge Patch (`application/merge-patch+json`).

### 4.3 HTTP Status Codes

- **FR-12:** The following status codes SHALL be used:

  | Code | Meaning | When |
  |------|---------|------|
  | 200 | OK | Successful GET, PUT, PATCH |
  | 201 | Created | Successful POST that creates a resource |
  | 202 | Accepted | Async operation started |
  | 204 | No Content | Successful DELETE, or POST with no body |
  | 400 | Bad Request | Malformed request, missing required fields |
  | 401 | Unauthorized | No token or invalid token |
  | 403 | Forbidden | Valid token, insufficient permissions |
  | 404 | Not Found | Resource does not exist |
  | 405 | Method Not Allowed | HTTP method not supported for this route |
  | 409 | Conflict | Duplicate resource, version mismatch |
  | 412 | Precondition Failed | Concurrency (ETag/If-Match) failed |
  | 422 | Unprocessable Entity | Validation failure (semantic) |
  | 429 | Too Many Requests | Rate limit exceeded |
  | 500 | Internal Server Error | Unhandled exception |
  | 502 | Bad Gateway | Downstream service failed |
  | 503 | Service Unavailable | Overloaded, maintenance |
  | 504 | Gateway Timeout | Downstream timed out |

- **FR-13:** 400 vs 422 distinction:
  - 400 — Malformed request (invalid JSON, wrong field type)
  - 422 — Well-formed but semantically invalid (e.g., end date before start)

- **FR-14:** 201 responses SHALL include a `Location` header with the
  new resource's URL.

- **FR-15:** 202 responses SHALL include a status URL where the client
  can poll for completion.

### 4.4 Response Format

- **FR-16:** All responses SHALL be JSON with
  `Content-Type: application/json`.

- **FR-17:** Response bodies SHALL use camelCase for property names.

- **FR-18:** Single-resource GET responses SHALL return the resource
  directly (not wrapped in `{ "data": ... }`).

- **FR-19:** Collection GET responses SHALL follow this structure:
  ```json
  {
    "items": [ ... ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 152,
    "totalPages": 8,
    "hasPreviousPage": false,
    "hasNextPage": true
  }