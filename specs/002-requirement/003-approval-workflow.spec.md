
---

# 📄 Spec 002-requirement/003 — Approval Workflow

Save as: `specs\002-requirement\003-approval-workflow.spec.md`

---

```markdown
# Spec: Approval Workflow

**Service:** Requirement
**Phase:** 002-requirement
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Clients create requirements in `Draft` status. Before a requirement
goes live (interviews can be scheduled), it must be **approved by an
InterviewEasy admin**. This protects quality:

- Questions are validated against test cases (Phase 3)
- Weights sum to 100 across courses
- Test cases are verified against solutions
- Content is not offensive or broken

This spec defines the approval workflow: submit for approval by the
client, review by the admin, and either approve or reject with feedback.

---

## 2. Scope

### In Scope

- Submit-for-approval transition
- Admin review queue
- Approve action
- Reject action with reason
- Validation rules run at submission
- Approval audit trail
- Events on state transitions

### Out of Scope

- Question validation (Phase 3)
- Test case verification (Phase 4 — Sandbox)
- Notifications (Phase 8)
- Bulk approval (future)

---

## 3. User Stories

- **US-1:** As a **client recruiter**, I want to submit a requirement
  for approval, so that it can go live.

- **US-2:** As an **InterviewEasy admin**, I want to review submitted
  requirements, so that I can ensure quality.

- **US-3:** As an **admin**, I want to reject a requirement with a
  reason, so that the client can fix issues and resubmit.

- **US-4:** As a **client recruiter**, I want to see why a requirement
  was rejected, so that I can fix and resubmit.

- **US-5:** As an **SRE**, I want every state change audited, so that
  I can trace who approved what and when.

---

## 4. Functional Requirements

### 4.1 Submission

- **FR-1:** `POST /api/v1/requirements/{id}/submit` SHALL transition
  a Draft requirement to `PendingApproval`.

- **FR-2:** Submission SHALL run validation:
  - At least 1 course exists
  - Sum of course weights = 100
  - Each course has at least 1 question
  - All questions have at least 1 sample test case
  - Title and description are non-empty

- **FR-3:** If validation fails, return 422 with a list of errors.

- **FR-4:** On success, the requirement's status changes to
  `PendingApproval`.

- **FR-5:** Submission SHALL be allowed only by a `client_admin` or
  `recruiter` of the tenant.

- **FR-6:** Only Draft requirements SHALL be submittable.

- **FR-7:** Submitting SHALL record `SubmittedBy` and `SubmittedAt`.

### 4.2 Review Queue

- **FR-8:** Admins SHALL see a paginated list of `PendingApproval`
  requirements across all tenants via
  `GET /api/v1/admin/requirements/pending`.

- **FR-9:** The list SHALL include: requirement ID, tenant, title,
  submitted at, submitted by, number of courses, number of questions.

- **FR-10:** The list SHALL support filters: tenant code, submitted
  date range.

- **FR-11:** Admin endpoints SHALL be under `/api/v1/admin/*` and
  require `super_admin` role.

### 4.3 Approve

- **FR-12:** `POST /api/v1/admin/requirements/{id}/approve` SHALL
  transition a PendingApproval requirement to `Active`.

- **FR-13:** Approval SHALL record `ApprovedBy` and `ApprovedAt`.

- **FR-14:** On approval, `RequirementApprovedEvent` SHALL be
  published.

- **FR-15:** Approved requirements SHALL become editable only by
  admins (read-only for client except description).

- **FR-16:** Approval SHALL fail with 409 if status is not
  PendingApproval.

### 4.4 Reject

- **FR-17:** `POST /api/v1/admin/requirements/{id}/reject` SHALL
  transition a PendingApproval requirement back to `Draft`.

- **FR-18:** Rejection SHALL require a reason (10–1000 chars).

- **FR-19:** Rejection SHALL record `RejectedBy`, `RejectedAt`,
  `RejectionReason` in a history table (FR-24).

- **FR-20:** On rejection, the requirement's `Status` becomes `Draft`
  again, but the current `RejectionReason` SHALL be visible to the
  client.

- **FR-21:** `RequirementRejectedEvent` SHALL be published with the
  reason.

- **FR-22:** The client SHALL be notified (Phase 8).

### 4.5 Re-submission

- **FR-23:** After rejection, the client can edit the Draft and
  resubmit — the workflow restarts.

- **FR-24:** Every submission SHALL be recorded in the requirement's
  history with timestamp and result.

### 4.6 Audit

- **FR-25:** Every status transition SHALL be recorded in a
  `requirement_audit_log` table:
  - Requirement ID
  - Tenant ID
  - From status
  - To status
  - Actor (user ID)
  - Timestamp
  - Reason (if applicable)

- **FR-26:** Audit log SHALL be immutable — no updates or deletes.

- **FR-27:** Admins SHALL view the audit log via
  `GET /api/v1/admin/requirements/{id}/audit`.

### 4.7 Validation

- **FR-28:** Validation SHALL run server-side at submit time — the
  client UI also validates but the server is authoritative.

- **FR-29:** Validation errors SHALL be grouped by section:
  - `courses` — course-related errors
  - `questions` — question-related errors
  - `weights` — weight sum error
  - `general` — other errors

- **FR-30:** Each error SHALL include a code, message, and optional
  field path.

---

## 5. Non-Functional Requirements

- **NFR-1:** Submit SHALL respond in under 500ms (p95) — includes
  validation.

- **NFR-2:** Approve/reject SHALL respond in under 200ms (p95).

- **NFR-3:** The audit log SHALL retain records indefinitely.

- **NFR-4:** Admin review list SHALL respond in under 300ms (p95)
  for up to 1,000 pending requirements.

- **NFR-5:** Concurrent approve/reject on the same requirement SHALL
  result in one success and one 409.

---

## 6. Domain Model

### 6.1 Entity: RequirementAuditLog

```csharp
public sealed class RequirementAuditLog
{
    public Guid Id { get; private set; }
    public Guid RequirementId { get; private set; }
    public Guid TenantId { get; private set; }
    public RequirementStatus FromStatus { get; private set; }
    public RequirementStatus ToStatus { get; private set; }
    public Guid ActorId { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string? Reason { get; private set; }
}