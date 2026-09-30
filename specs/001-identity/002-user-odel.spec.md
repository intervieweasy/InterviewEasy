
---

# 📄 Spec 001-identity/002 — User Model

Save as: `specs\001-identity\002-user-model.spec.md`

---

```markdown
# Spec: User Model

**Service:** Identity
**Phase:** 001-identity
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Users are the people who interact with InterviewEasy:
- **Client Admins / Recruiters** — create requirements, review interviews
- **Interviewers** — conduct interviews, submit feedback
- **Platform Admins** — manage tenants, oversee the platform

Candidates do NOT have user accounts — they access interviews via
magic links (see Spec 001-identity-003). This keeps the user model
focused on long-lived platform users.

This spec defines the User entity, its attributes, lifecycle, and
relationship to tenants and roles.

---

## 2. Scope

### In Scope

- User entity and invariants
- Email as identity key
- User status lifecycle (pending, active, suspended, deleted)
- User profile fields
- Password storage (hashed)
- User-tenant relationship
- Role assignment
- Email verification flag
- User CRUD operations

### Out of Scope

- Authentication (login, JWT issuance) — Spec 001-identity-003
- Authorization (roles, permissions) — Spec 001-identity-004
- Password reset — Spec 001-identity-006
- Profile pictures / avatar uploads (future spec)
- User preferences (language, timezone) — future spec

---

## 3. User Stories

- **US-1:** As a **client admin**, I want to invite users to my tenant,
  so that my team can access the platform.

- **US-2:** As a **user**, I want to update my profile, so that my
  name and contact info stay current.

- **US-3:** As a **client admin**, I want to deactivate a user, so
  that former employees cannot access the platform.

- **US-4:** As an **SRE**, I want to see all users in a tenant, so
  that I can audit access.

- **US-5:** As a **user**, I want my email verified, so that password
  reset and notifications work reliably.

---

## 4. Functional Requirements

### 4.1 User Entity

- **FR-1:** A `User` entity SHALL exist in the Identity domain with:
  - `Id` (Guid, PK)
  - `TenantId` (Guid, FK to tenant)
  - `Email` (string, unique per tenant, lowercase)
  - `FullName` (string, 2–200 chars)
  - `PasswordHash` (string, bcrypt or Argon2id)
  - `Status` (enum: Pending, Active, Suspended, Deleted)
  - `EmailVerified` (bool)
  - `EmailVerificationToken` (string?, hashed)
  - `EmailVerificationSentAt` (DateTime?)
  - `LastLoginAt` (DateTime?)
  - `FailedLoginCount` (int, default 0)
  - `LockedUntil` (DateTime?)
  - `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` (audit)

- **FR-2:** Email SHALL:
  - Be lowercase, trimmed
  - Match `^[^@\s]+@[^@\s]+\.[^@\s]+$`
  - Be unique per tenant (case-insensitive)

- **FR-3:** Full name SHALL be 2–200 characters after trim.

- **FR-4:** Passwords SHALL NEVER be stored in plaintext.

- **FR-5:** Password hashing SHALL use Argon2id with parameters:
  - Memory: 64 MB
  - Iterations: 3
  - Parallelism: 4
  (Or bcrypt with cost factor 12 as fallback.)

### 4.2 User Lifecycle

- **FR-6:** New users SHALL start in `Pending` status.

- **FR-7:** A `Pending` user SHALL become `Active` after email
  verification.

- **FR-8:** Only `Active` users SHALL authenticate successfully.

- **FR-9:** `Suspended` users SHALL NOT authenticate. Login returns a
  generic error to avoid leaking the reason.

- **FR-10:** `Deleted` users SHALL be soft-deleted — row retained for
  audit, email can be reused.

- **FR-11:** Failed login attempts SHALL increment `FailedLoginCount`.

- **FR-12:** After 5 consecutive failed logins, the account SHALL be
  locked for 15 minutes (`LockedUntil` set).

- **FR-13:** A successful login SHALL reset `FailedLoginCount` to 0
  and clear `LockedUntil`.

### 4.3 User Invitation

- **FR-14:** Client admins SHALL invite users via email.

- **FR-15:** An invited user SHALL start in `Pending` status with a
  verification token.

- **FR-16:** The invitation email SHALL contain a link to set the
  initial password.

- **FR-17:** Invitation tokens SHALL expire after 7 days.

- **FR-18:** The invitation flow SHALL be idempotent — resending
  generates a new token and invalidates the old one.

### 4.4 User Profile Updates

- **FR-19:** Users SHALL update their own `FullName` via
  `PUT /api/v1/users/me`.

- **FR-20:** Client admins SHALL update any user in their tenant via
  `PUT /api/v1/users/{id}`.

- **FR-21:** Email changes SHALL require re-verification. The old
  email SHALL be notified of the change.

- **FR-22:** Email SHALL NOT be directly editable — a separate "change
  email" flow with verification SHALL be used.

### 4.5 User Reads

- **FR-23:** `GET /api/v1/users/me` SHALL return the current user.

- **FR-24:** `GET /api/v1/users/{id}` SHALL return a user (client admin
  or super admin only).

- **FR-25:** `GET /api/v1/users` SHALL list users in the current
  tenant (paginated, client admin only).

- **FR-26:** User responses SHALL NOT include `PasswordHash`,
  `EmailVerificationToken`, or `FailedLoginCount`.

### 4.6 User Deletion

- **FR-27:** `DELETE /api/v1/users/{id}` SHALL soft-delete a user
  (client admin only).

- **FR-28:** Soft-deleted users SHALL NOT appear in listings.

- **FR-29:** A soft-deleted user's email SHALL be reusable by a new user.

- **FR-30:** Users SHALL NOT delete themselves — that's a "close my
  account" flow (deferred).

### 4.7 Tenant Isolation

- **FR-31:** Users SHALL belong to exactly one tenant.

- **FR-32:** Users SHALL NOT be able to query users from other tenants.

- **FR-33:** A super_admin MAY impersonate a tenant context to query
  users (audited).

---

## 5. Non-Functional Requirements

- **NFR-1:** User lookups by email SHALL complete in under 50ms (p95).

- **NFR-2:** Password hashing SHALL take 200–500ms (target — prevents
  brute force).

- **NFR-3:** The system SHALL support 100,000 users per tenant.

- **NFR-4:** Emails SHALL be unique per tenant — enforced by a unique index.

- **NFR-5:** User deletion SHALL retain audit history for at least
  2 years.

---

## 6. Domain Model

### 6.1 Entity: User

```csharp
public sealed class User : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public string PasswordHash { get; private set; }
    public UserStatus Status { get; private set; }
    public bool EmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationSentAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginCount { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    public static User Create(Guid tenantId, string email,
        string fullName, string passwordHash);

    public void VerifyEmail(Guid updatedBy);
    public void Rename(string newName, Guid updatedBy);
    public void RecordLogin();
    public void RecordFailedLogin();
    public void Suspend(Guid suspendedBy);
    public void Activate(Guid activatedBy);
    public void SoftDelete(Guid deletedBy);
}