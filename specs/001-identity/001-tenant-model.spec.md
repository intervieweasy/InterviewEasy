# Spec: Tenant Model

**Service:** Identity
**Phase:** 001-identity
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

A **Tenant** in InterviewEasy represents a client organization (e.g.,
Acme Corp, Globex) that uses the platform to conduct interviews. Every
piece of data in the system — requirements, questions, interviews,
feedback — belongs to exactly one tenant.

Tenants are the root of the platform's multi-tenancy model
(Spec 002 — Database Strategy). Creating a tenant is the first action
that provisions a tenant's schema across all services.

This spec defines the Tenant entity, its lifecycle, the provisioning
process, and the events raised when a tenant is created, suspended,
or deleted.

---

## 2. Scope

### In Scope

- Tenant entity and its invariants
- Tenant status lifecycle (active, suspended, deleted)
- Tenant creation via API
- Tenant code and schema prefix generation
- Schema provisioning across services
- Tenant resolution context (used by all services)
- CRUD operations (create, read, update, suspend, delete)
- Events published on tenant changes

### Out of Scope

- User management (Spec 001-identity-002)
- Billing and subscription (future spec)
- Tenant-specific branding (future spec)
- Data export and deletion workflows (future spec)
- Tenant admin UI (Phase 12)

---

## 3. User Stories

- **US-1:** As an **InterviewEasy administrator**, I want to create a
  new tenant, so that a new client can start using the platform.

- **US-2:** As a **client administrator**, I want to view my tenant's
  details, so that I can verify our organization's information.

- **US-3:** As an **SRE**, I want to suspend a tenant temporarily, so
  that we can block access when a client breaches terms or fails to pay.

- **US-4:** As a **compliance officer**, I want to soft-delete a tenant,
  so that we retain audit history while removing access.

- **US-5:** As a **backend developer**, I want tenant provisioning to
  create all necessary schemas in one operation, so that no service is
  left without a schema after signup.

---

## 4. Functional Requirements

### 4.1 Tenant Entity

- **FR-1:** A `Tenant` entity SHALL exist in the Identity domain with:
  - `Id` (Guid, PK)
  - `Code` (string, lowercase, 3–50 chars, unique)
  - `Name` (string, 3–200 chars)
  - `SchemaPrefix` (string, `tenant_{code}`, unique)
  - `Status` (enum: Active, Suspended, Deleted)
  - `SuspendedAt` (DateTime?, UTC)
  - `SuspendedReason` (string?)
  - `DeletedAt` (DateTime?, UTC)
  - `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` (audit)

- **FR-2:** Tenant `Code` SHALL:
  - Be lowercase
  - Contain only letters, digits, and hyphens
  - Match the regex `^[a-z][a-z0-9-]{2,49}$`
  - Not start with a digit
  - Not be a reserved word (`shared`, `public`, `system`, `admin`)

- **FR-3:** Tenant `Code` SHALL be immutable after creation.

- **FR-4:** `SchemaPrefix` SHALL be computed as `tenant_{Code}` and
  SHALL be immutable.

- **FR-5:** Tenant `Name` SHALL be 3–200 characters and trimmed.

### 4.2 Tenant Lifecycle

- **FR-6:** A new tenant SHALL start in `Active` status.

- **FR-7:** A tenant in `Active` status SHALL accept all operations.

- **FR-8:** A tenant in `Suspended` status SHALL reject all authenticated
  requests (except for admin-level reactivation).

- **FR-9:** A tenant in `Deleted` status SHALL be soft-deleted —
  data retained, access denied.

- **FR-10:** Suspending a tenant SHALL require a reason (max 500 chars).

- **FR-11:** Reactivating a suspended tenant SHALL return it to `Active`.

- **FR-12:** Deleted tenants SHALL NOT be reactivatable via API — only
  by direct database manipulation (documented operation).

### 4.3 Tenant Creation

- **FR-13:** Creating a tenant SHALL:
  1. Validate the code and name
  2. Check for duplicate code
  3. Insert a row in `shared.tenants`
  4. Provision schemas across all services (async)
  5. Publish `TenantCreatedEvent`
  6. Return the new tenant with 201 Created

- **FR-14:** Schema provisioning SHALL create:
  - `tenant_{code}` (root schema — reserved for future use)
  - `tenant_{code}_identity`
  - `tenant_{code}_requirement`
  - `tenant_{code}_question`
  - `tenant_{code}_scheduling`
  - `tenant_{code}_session`
  - `tenant_{code}_feedback`
  - `tenant_{code}_proctoring`
  - `tenant_{code}_sandbox`
  - `tenant_{code}_notification`
  - `tenant_{code}_recording`
  - `tenant_{code}_analytics`

- **FR-15:** Each sub-schema SHALL be provisioned with the tables
  needed by that service (via EF Core migrations).

- **FR-16:** Provisioning failures SHALL be logged and the tenant
  marked as `ProvisioningFailed` (new status) for retry.

- **FR-17:** Provisioning SHALL complete in under 30 seconds (NFR from
  Spec 002).

### 4.4 Tenant Reads

- **FR-18:** `GET /api/v1/tenants/{id}` SHALL return tenant details
  for a valid ID (authenticated + authorized).

- **FR-19:** `GET /api/v1/tenants/by-code/{code}` SHALL allow lookup
  by code (used during login).

- **FR-20:** `GET /api/v1/tenants` SHALL return a paginated list
  (super_admin only).

- **FR-21:** Tenant reads SHALL NOT return suspended/deleted tenants
  to non-admin users.

### 4.5 Tenant Updates

- **FR-22:** `PUT /api/v1/tenants/{id}` SHALL update `Name` only.
  (Code and SchemaPrefix are immutable.)

- **FR-23:** `POST /api/v1/tenants/{id}/suspend` SHALL suspend a tenant.

- **FR-24:** `POST /api/v1/tenants/{id}/activate` SHALL reactivate a
  suspended tenant.

- **FR-25:** `DELETE /api/v1/tenants/{id}` SHALL soft-delete a tenant.

### 4.6 Tenant Context

- **FR-26:** The Identity service SHALL expose an `ITenantContext`
  interface providing the current tenant ID during a request.

- **FR-27:** Tenant context SHALL be resolved from the JWT `tenant_id`
  claim OR from the `X-Tenant-Code` header (for unauthenticated flows
  like login).

- **FR-28:** All services SHALL share the `ITenantContext` abstraction
  (defined in BuildingBlocks.Common).

### 4.7 Tenant Provisioning Events

- **FR-29:** On successful provisioning, the Identity service SHALL
  publish `TenantProvisioned` event.

- **FR-30:** On provisioning failure, the Identity service SHALL
  publish `TenantProvisioningFailed` event for alerting.

---

## 5. Non-Functional Requirements

- **NFR-1:** Tenant creation SHALL complete (API response) in under
  500ms. Provisioning runs asynchronously.

- **NFR-2:** Tenant reads SHALL complete in under 100ms (p95).

- **NFR-3:** The system SHALL support at least 10,000 tenants.

- **NFR-4:** Tenant codes SHALL be unique across all tenants —
  enforced by a unique index.

- **NFR-5:** All tenant operations SHALL be audited (who, what, when).

---

## 6. Domain Model

### 6.1 Entity: Tenant

```csharp
public sealed class Tenant : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string SchemaPrefix { get; private set; }
    public TenantStatus Status { get; private set; }
    public DateTime? SuspendedAt { get; private set; }
    public string? SuspendedReason { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Factory method
    public static Tenant Create(string code, string name, Guid createdBy);

    // Behavior
    public void Rename(string newName, Guid updatedBy);
    public void Suspend(string reason, Guid suspendedBy);
    public void Activate(Guid activatedBy);
    public void SoftDelete(Guid deletedBy);
}