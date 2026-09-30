
---

# 📄 Spec 002-requirement/004 — Panel Assignment

Save as: `specs\002-requirement\004-panel-assignment.spec.md`

---

```markdown
# Spec: Panel Assignment

**Service:** Requirement
**Phase:** 002-requirement
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

A **panel** is the set of interviewers assigned to a requirement.
When scheduling interviews for a candidate, the client selects one or
more panel members from the assigned panel.

Panels enable:
- Restricting which interviewers can conduct interviews for a requirement
- Fair distribution of interviews across interviewers
- Tracking panel member availability and workload

This spec defines how panel members are assigned to a requirement,
their roles, and their permissions.

---

## 2. Scope

### In Scope

- Panel member entity
- Add/remove panel members
- Panel member roles (lead, member, shadow)
- Panel member availability (soft, informational)
- Panel member workload tracking
- Panel restrictions for scheduling (feeds Phase 5)

### Out of Scope

- Interview scheduling (Phase 5)
- Interviewer calendars (future)
- Panel notification (Phase 8)
- Interviewer availability management UI (Phase 12)

---

## 3. User Stories

- **US-1:** As a **client recruiter**, I want to assign interviewers
  to a requirement, so that only the right people can conduct its
  interviews.

- **US-2:** As a **client recruiter**, I want to mark one interviewer
  as the panel lead, so that there's an owner for the panel.

- **US-3:** As an **interviewer**, I want to see requirements where
  I'm on the panel, so that I can prepare.

- **US-4:** As a **client admin**, I want to see how many interviews
  each interviewer has done, so that I can balance the load.

- **US-5:** As a **client recruiter**, I want to remove a panel member
  without disrupting scheduled interviews, so that I can adjust the
  panel over time.

---

## 4. Functional Requirements

### 4.1 Panel Member Entity

- **FR-1:** A `PanelMember` entity SHALL exist with:
  - `Id` (Guid, PK)
  - `RequirementId` (Guid, FK)
  - `UserId` (Guid, FK to Identity user)
  - `Role` (enum: Lead, Member, Shadow)
  - `AddedBy` (Guid)
  - `AddedAt` (DateTime)
  - `RemovedBy` (Guid?, nullable)
  - `RemovedAt` (DateTime?, nullable)
  - `IsActive` (bool)

- **FR-2:** Panel member is a **soft relationship** — removing sets
  `IsActive=false`, doesn't delete the row.

- **FR-3:** A user can be on the panel of multiple requirements.

- **FR-4:** A user can appear only ONCE per requirement (uniqueness
  on `requirementId + userId` where `IsActive=true`).

### 4.2 Panel Roles

- **FR-5:** Panel member roles SHALL be:
  - `Lead` — owns the panel, can add/remove other members
  - `Member` — can conduct interviews
  - `Shadow` — observes only, cannot conduct

- **FR-6:** Each requirement SHALL have **exactly one** Active `Lead`.

- **FR-7:** Only a `client_admin` or `recruiter` can assign a `Lead`.

- **FR-8:** Changing the Lead SHALL transfer the role from the
  previous Lead to the new one.

- **FR-9:** A panel SHALL have between 1 and 20 Active members.

### 4.3 Panel Assignment

- **FR-10:** `POST /api/v1/requirements/{reqId}/panel` SHALL add a
  panel member.

- **FR-11:** Adding SHALL require:
  - The user exists in the same tenant
  - The user has `interviewer`, `recruiter`, or `client_admin` role
  - The user is not already an Active panel member

- **FR-12:** Adding a Lead when one exists SHALL either:
  - Fail with 409 (`A Lead already exists`), OR
  - Demote the existing Lead to Member (via a `promoteLead` flag)

- **FR-13:** Panel membership SHALL only be editable while the
  requirement is in `Draft` status.

- **FR-14:** Once a requirement is `Active`, panel membership SHALL
  be locked.

### 4.4 Removing Panel Members

- **FR-15:** `DELETE /api/v1/requirements/{reqId}/panel/{userId}`
  SHALL set `IsActive=false` for that member.

- **FR-16:** Removing the Lead SHALL require promoting another member
  first (or fail with 409).

- **FR-17:** Removing a member SHALL NOT affect past interviews.

- **FR-18:** A removed member SHALL NOT be re-addable without an
  explicit re-add (preserves history).

### 4.5 Panel Reads

- **FR-19:** `GET /api/v1/requirements/{reqId}/panel` SHALL return
  the Active panel members.

- **FR-20:** Response SHALL include:
  - User ID
  - Full name
  - Email
  - Role (Lead, Member, Shadow)
  - Added at
  - Interview count (for the requirement)
  - Upcoming interviews count

- **FR-21:** `GET /api/v1/requirements/{reqId}/panel/history` SHALL
  return all panel members (active + removed) with timestamps.

### 4.6 Panel Workload

- **FR-22:** Each panel member SHALL have:
  - `InterviewCount` — interviews conducted for this requirement
  - `UpcomingInterviewCount` — scheduled but not yet conducted
  - `LastInterviewAt`

- **FR-23:** Workload SHALL be updated when interviews are scheduled
  or completed (via events from Phase 5).

- **FR-24:** Panel list SHALL sort by `InterviewCount ASC` when
  `?balanceWorkload=true`.

### 4.7 Panel Restrictions for Scheduling

- **FR-25:** When scheduling an interview (Phase 5), only Active
  panel members SHALL be selectable.

- **FR-26:** The panel Lead SHALL be automatically added to any
  interview for the requirement (configurable).

- **FR-27:** Shadow panel members SHALL NOT be selectable as the
  primary interviewer but MAY be added as observers.

### 4.8 Tenant Isolation

- **FR-28:** All panel operations SHALL be scoped to the current
  tenant and the specified requirement.

- **FR-29:** Cross-tenant user assignment SHALL return 422
  (validation: user not in tenant).

---

## 5. Non-Functional Requirements

- **NFR-1:** Panel member addition SHALL respond in under 200ms (p95).

- **NFR-2:** Panel listing SHALL respond in under 150ms (p95) with
  workload data.

- **NFR-3:** Panel changes SHALL be audited.

- **NFR-4:** The system SHALL support up to 20 panel members per
  requirement and 1,000 requirements per interviewer.

---

## 6. Domain Model

### 6.1 Entity: PanelMember

```csharp
public sealed class PanelMember : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid RequirementId { get; private set; }
    public Guid UserId { get; private set; }
    public PanelRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime AddedAt { get; private set; }
    public Guid AddedBy { get; private set; }
    public DateTime? RemovedAt { get; private set; }
    public Guid? RemovedBy { get; private set; }

    // Workload (denormalized)
    public int InterviewCount { get; private set; }
    public int UpcomingInterviewCount { get; private set; }
    public DateTime? LastInterviewAt { get; private set; }

    public static PanelMember Create(Guid requirementId,
        Guid userId, PanelRole role, Guid addedBy);

    public void PromoteToLead(Guid promotedBy);
    public void DemoteToMember(Guid updatedBy);
    public void Deactivate(Guid removedBy);
    public void IncrementInterviewCount();
    public void DecrementUpcomingCount();
}