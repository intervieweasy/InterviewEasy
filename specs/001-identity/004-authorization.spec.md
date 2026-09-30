
---

# 📄 Spec 001-identity/004 — Authorization

Save as: `specs\001-identity\004-authorization.spec.md`

---

```markdown
# Spec: Authorization

**Service:** Identity
**Phase:** 001-identity
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Authentication answers "who are you?" Authorization answers "what can
you do?" InterviewEasy has multiple user roles with distinct
permissions:

- **Super Admin** — platform operator (InterviewEasy team)
- **Client Admin** — manages a tenant's users, requirements, panel
- **Recruiter** — creates requirements and schedules interviews
- **Interviewer** — conducts interviews, submits feedback
- **Viewer** — read-only access to interviews and feedback

Authorization is enforced **per endpoint** via policy-based checks and
**per resource** via ownership/tenant checks.

This spec defines the roles, permissions, policy patterns, and how
authorization decisions are made.

---

## 2. Scope

### In Scope

- Role definitions
- Permission definitions
- Role-to-permission mapping
- Policy-based authorization in ASP.NET Core
- Resource-level authorization (ownership, tenancy)
- Permission resolution (from JWT claims or DB)
- Admin impersonation (super_admin acts on behalf of a tenant)
- Audit of authorization denials

### Out of Scope

- Authentication (Spec 001-identity-003)
- User-role assignment API (covered in User model spec — extension)
- Permission delegation (future)
- Attribute-based access control (ABAC) — future if needed
- Field-level authorization (future)

---

## 3. User Stories

- **US-1:** As a **client admin**, I want to restrict which users can
  approve requirements, so that only trusted staff control hiring.

- **US-2:** As an **interviewer**, I want to see only interviews I'm
  assigned to, so that I don't accidentally see other panels' data.

- **US-3:** As a **super admin**, I want to impersonate a tenant
  context, so that I can debug customer issues.

- **US-4:** As a **security officer**, I want every denied
  authorization attempt logged, so that I can detect probing.

- **US-5:** As a **developer**, I want clear permission strings, so
  that I can enforce them consistently.

---

## 4. Functional Requirements

### 4.1 Roles

- **FR-1:** The system SHALL define the following system roles:
  - `super_admin` — full platform access
  - `client_admin` — full tenant access
  - `recruiter` — create/manage requirements, schedule interviews
  - `interviewer` — conduct interviews, submit feedback
  - `viewer` — read-only access

- **FR-2:** Users SHALL have one or more roles.

- **FR-3:** Roles SHALL be assigned per user per tenant.

- **FR-4:** A user in one tenant SHALL NOT have roles in another
  tenant (except super_admin).

- **FR-5:** System roles SHALL be seeded at tenant creation.

- **FR-6:** Custom roles MAY be added per tenant in future versions
  (out of scope for v1).

### 4.2 Permissions

- **FR-7:** Permissions SHALL follow the format `{resource}:{action}`.

- **FR-8:** Standard permissions SHALL include:

  | Resource | Actions |
  |----------|---------|
  | `tenants` | `read`, `create`, `update`, `suspend`, `delete` |
  | `users` | `read`, `invite`, `update`, `suspend`, `delete` |
  | `requirements` | `read`, `create`, `update`, `approve`, `delete` |
  | `courses` | `read`, `create`, `update`, `delete` |
  | `questions` | `read`, `create`, `update`, `delete`, `import` |
  | `interviews` | `read`, `schedule`, `conduct`, `cancel` |
  | `feedback` | `read`, `submit` |
  | `recordings` | `read`, `delete` |
  | `analytics` | `read`, `export` |
  | `audit` | `read` |

- **FR-9:** Permissions SHALL be global strings, not tenant-scoped.

- **FR-10:** Permission checks SHALL be case-sensitive.

### 4.3 Role-Permission Mapping

- **FR-11:** `super_admin` SHALL have all permissions (wildcard `*`).

- **FR-12:** `client_admin` SHALL have all permissions except
  `tenants:*` and `audit:*` (scoped to their tenant).

- **FR-13:** `recruiter` SHALL have:
  - `requirements:*`
  - `courses:*`
  - `questions:read`
  - `interviews:read`, `interviews:schedule`
  - `feedback:read`
  - `analytics:read`

- **FR-14:** `interviewer` SHALL have:
  - `requirements:read`
  - `questions:read`
  - `interviews:read`, `interviews:conduct`
  - `feedback:read`, `feedback:submit`

- **FR-15:** `viewer` SHALL have:
  - `*:read` for the tenant's resources (read-only)

### 4.4 Policy-Based Authorization

- **FR-16:** Authorization SHALL use ASP.NET Core policy-based
  authorization.

- **FR-17:** Every endpoint SHALL declare a policy via
  `[Authorize(Policy = "...")]`.

- **FR-18:** Policies SHALL be registered in DI at startup.

- **FR-19:** A policy SHALL require one or more permissions:
  ```csharp
  options.AddPolicy("requirements:create",
      p => p.RequireClaim("permissions", "requirements:create"));