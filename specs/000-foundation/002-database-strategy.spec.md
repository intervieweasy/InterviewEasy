# Spec: Database Strategy

**Service:** Foundation
**Phase:** 000-foundation
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

InterviewEasy is a **multi-tenant SaaS platform**. Each client (e.g.,
Acme Corp, Globex, Initech) is a tenant whose data must be logically
isolated from other tenants. A single PostgreSQL database cannot simply
mix all tenants' rows in the same tables without careful isolation
mechanisms — one bug in a query filter could leak data across clients.

Additionally, because InterviewEasy is built as **12+ microservices**,
each service owns its data. The database strategy must answer:

- How is tenant isolation enforced?
- How does each service own its tables without stepping on others?
- How do migrations work across N tenants?
- How are connection strings managed safely?

This spec defines the **database architecture, tenancy model, EF Core
conventions, migration strategy, and naming standards** that every
service must follow.

This is the second foundational spec — it enables every future
service to persist data safely and consistently.

---

## 2. Scope

### In Scope

- Multi-tenancy model (schema-per-tenant vs shared schema vs db-per-tenant)
- Tenant resolution strategy
- PostgreSQL naming conventions (schemas, tables, columns, indexes, constraints)
- EF Core conventions (DbContext design, entity configuration, migrations)
- Connection string management per environment
- Migration strategy across tenants
- Per-tenant backup and restore strategy
- JSONB usage for dynamic fields (feedback templates, question content)
- Indexing strategy for tenant-scoped queries
- Soft delete pattern
- Audit columns (created_at, updated_at, created_by, updated_by)

### Out of Scope

- Event bus schema (covered in Spec 003 — Event Bus)
- Read models and materialized views for analytics (Phase 10)
- Redis caching strategy (separate spec)
- Cross-service data joins (services communicate via events, not DB joins)
- Sharding beyond PostgreSQL (if scale demands it — future spec)
- Column-level encryption (future security spec)

---

## 3. User Stories

- **US-1:** As a **client of InterviewEasy**, I want my data to be
  physically or logically isolated from other clients, so that no other
  client can ever see my interview data.

- **US-2:** As a **backend developer**, I want a single database strategy
  used across all services, so that I don't have to make per-service
  decisions about tenancy.

- **US-3:** As a **DBA**, I want migrations to run predictably across
  all tenants, so that schema drift never occurs.

- **US-4:** As a **DevOps engineer**, I want connection strings managed
  per environment without secrets in source control, so that we stay
  compliant.

- **US-5:** As a **compliance officer**, I want per-tenant backup and
  export capability, so that we can satisfy GDPR "right to access" and
  "right to be forgotten" requests.

- **US-6:** As an **SRE**, I want clear indexing and soft-delete
  conventions, so that performance is predictable as data grows.

---

## 4. Functional Requirements

### 4.1 Tenancy Model

- **FR-1:** The platform SHALL use **schema-per-tenant** as the default
  multi-tenancy model.

- **FR-2:** A **shared schema** (`shared`) SHALL hold cross-tenant data:
  tenants registry, feature flags, system configuration.

- **FR-3:** Each tenant SHALL have its own PostgreSQL schema named
  `tenant_{tenant_id}` where `tenant_id` is a sanitized lowercase
  identifier (e.g., `tenant_acme`).

- **FR-4:** Every table inside a tenant schema SHALL belong to exactly
  one service's bounded context.

- **FR-5:** When a tenant is created, the system SHALL provision its
  schema and run all migrations for all services that own data in that
  schema (or per-service sub-schemas — see FR-6).

- **FR-6:** To keep service data physically separate within a tenant,
  each service SHALL use a **sub-schema** named
  `tenant_{tenant_id}_{service}` (e.g., `tenant_acme_question`,
  `tenant_acme_session`).

- **FR-7:** Services SHALL NOT query tables owned by another service.

### 4.2 Tenant Resolution

- **FR-8:** The tenant SHALL be resolved per HTTP request from one of:
  - JWT claim `tenant_id` (for authenticated users)
  - Subdomain (e.g., `acme.intervieweasy.com`) — fallback
  - Signed candidate invite token (for candidate app)

- **FR-9:** Tenant resolution middleware SHALL populate
  `HttpContext.Items["TenantId"]` and `HttpContext.Items["TenantSchema"]`.

- **FR-10:** If no tenant can be resolved for a request that requires
  one, the API SHALL return `400 Bad Request` with error code
  `TENANT_NOT_RESOLVED`.

### 4.3 EF Core Conventions

- **FR-11:** Each service SHALL have exactly one `DbContext` per bounded
  context.

- **FR-12:** All DbContexts SHALL set `search_path` at connection-open
  time based on the resolved tenant schema.

- **FR-13:** Entity configurations SHALL use `IEntityTypeConfiguration<T>`
  classes, NOT inline `OnModelCreating` for each entity.

- **FR-14:** All tables SHALL have:
  - `id UUID PRIMARY KEY DEFAULT gen_random_uuid()`
  - `created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()`
  - `updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()`
  - `created_by UUID NULL`
  - `updated_by UUID NULL`

- **FR-15:** Soft-deletable tables SHALL additionally have:
  - `is_deleted BOOLEAN NOT NULL DEFAULT FALSE`
  - `deleted_at TIMESTAMPTZ NULL`
  - `deleted_by UUID NULL`

- **FR-16:** All queries SHALL filter out soft-deleted rows by default
  via EF Core global query filters.

- **FR-17:** `updated_at` SHALL be auto-updated via EF Core
  `SaveChangesInterceptor`.

- **FR-18:** JSONB columns SHALL be used for dynamic structures
  (feedback template schemas, question content, metadata blobs).

- **FR-19:** EF Core migrations SHALL be stored **per service** under
  `src/Services/{Service}/InterviewEasy.{Service}.Infrastructure/Migrations/`.

### 4.4 Naming Conventions

- **FR-20:** Schemas SHALL use `snake_case`:
  `tenant_acme`, `shared`.

- **FR-21:** Tables SHALL use `snake_case` **plural**:
  `requirements`, `feedback_templates`, `test_cases`.

- **FR-22:** Columns SHALL use `snake_case` **singular**:
  `created_at`, `tenant_id`, `requirement_id`.

- **FR-23:** Foreign keys SHALL be named `{referenced_table_singular}_id`:
  `requirement_id`, `course_id`.

- **FR-24:** Primary keys SHALL be named `id` on every table (no
  `{table}_id` prefix).

- **FR-25:** Indexes SHALL be named `idx_{table}_{columns}`:
  `idx_requirements_tenant_status`.

- **FR-26:** Unique constraints SHALL be named
  `uq_{table}_{columns}`: `uq_requirements_tenant_title`.

- **FR-27:** Foreign key constraints SHALL be named
  `fk_{table}_{referenced_table}`: `fk_questions_requirements`.

- **FR-28:** Check constraints SHALL be named
  `ck_{table}_{rule}`: `ck_requirements_status`.

### 4.5 Migrations

- **FR-29:** Migrations SHALL be generated per service via
  `dotnet ef migrations add {Name}`.

- **FR-30:** Migrations SHALL be applied via a migration runner at
  application startup OR a dedicated CLI command — never manually in
  production.

- **FR-31:** The migration runner SHALL iterate over all active tenants
  and apply migrations to each tenant's schema.

- **FR-32:** Failed migrations for one tenant SHALL NOT block other
  tenants; failures SHALL be logged and the tenant marked for retry.

- **FR-33:** A `schema_migrations` table SHALL exist **per tenant
  sub-schema** to track applied migrations for that service.

- **FR-34:** Migrations SHALL be **idempotent** and safe to re-run.

- **FR-35:** Destructive migrations (column drops, table drops) SHALL
  require a two-phase rollout:
  1. Deploy code that stops using the column
  2. Deploy migration that drops it after N days

### 4.6 Connection Strings & Secrets

- **FR-36:** Connection strings SHALL be stored in environment
  configuration, NOT in `appsettings.json` for non-dev environments.

- **FR-37:** In development, connection strings MAY be in
  `appsettings.Development.json` (gitignored) or in
  `docker-compose.yml` environment variables.

- **FR-38:** In staging and production, connection strings SHALL be
  loaded from Azure Key Vault (preferred) or AWS Secrets Manager.

- **FR-39:** Connection strings SHALL NEVER be logged, even at DEBUG level.

- **FR-40:** Each environment (dev, staging, prod) SHALL use a
  **separate PostgreSQL instance**. Cross-environment data sharing is
  forbidden.

### 4.7 Backup & Restore

- **FR-41:** The system SHALL support per-tenant `pg_dump` exports for
  compliance use cases.

- **FR-42:** Daily full backups SHALL run at the cluster level (managed
  by infrastructure, not application code).

- **FR-43:** Point-in-time recovery (PITR) SHALL be enabled via WAL
  archiving in production.

- **FR-44:** Restore drills SHALL be documented and run quarterly.

### 4.8 Multi-Schema Access

- **FR-45:** Services SHALL use a **single database connection** with
  `search_path` set per request. They SHALL NOT open one connection per
  tenant.

- **FR-46:** Connection pooling SHALL use PgBouncer in transaction mode
  when scaling beyond 100 tenants.

---

## 5. Non-Functional Requirements

- **NFR-1:** A single query on a tenant-scoped table SHALL complete in
  under 50ms (p95) with 1M rows.

- **NFR-2:** Creating a new tenant schema SHALL complete in under 30
  seconds.

- **NFR-3:** Running migrations across 500 tenants SHALL complete in
  under 15 minutes.

- **NFR-4:** The database SHALL support 1,000+ tenants on a single
  PostgreSQL cluster (schema-per-tenant scales to thousands).

- **NFR-5:** Per-tenant backup export SHALL complete in under 5 minutes
  for tenants with under 100K rows.

- **NFR-6:** Connection pool exhaustion SHALL NOT occur under 10,000
  concurrent requests.

- **NFR-7:** Zero data leakage between tenants — enforced via
  integration tests that attempt cross-tenant queries.

---

## 6. Domain Model

This spec does not define domain entities. It defines the **persistence
layer conventions** every entity must follow.

**Shared conventions for every entity:**

- Primary key: `Guid Id` mapped to `id UUID`
- Audit fields: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`
- Soft delete: `IsDeleted`, `DeletedAt`, `DeletedBy` (optional per entity)
- Tenant scoping: implicit via schema (not a column)

**Example: `Requirement` entity (illustrative)**

```csharp
public class Requirement : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    // ... domain methods
}