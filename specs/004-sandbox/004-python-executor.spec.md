
---

# 📄 Spec 004-sandbox/003 — SQL Executor

Save as: `specs\004-sandbox\003-sql-executor.spec.md`

---

```markdown
# Spec: SQL Executor

**Service:** Sandbox
**Phase:** 004-sandbox
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

SQL questions test a candidate's ability to write queries. The
candidate receives a schema and sample data, writes a SELECT or DML
query, and the sandbox runs it against a temporary PostgreSQL
database. The result set is compared to the expected output.

Unlike code execution (which is generic), SQL execution needs:
- A per-job database with the schema + seed data
- Query parsing to prevent destructive operations
- Result set comparison (order-insensitive by default)

This spec defines the SQL execution container, database provisioning,
query safety, and result comparison.

---

## 2. Scope

### In Scope

- PostgreSQL 16 sandbox per job
- Schema + seed data provisioning
- Query parsing and safety checks
- Read-only enforcement (SELECT only by default)
- DML support (INSERT/UPDATE/DELETE) when question allows
- Result set comparison
- Query timeout and row limit
- Error reporting

### Out of Scope

- Other database engines (MySQL, SQL Server) — future
- Query plan analysis
- Performance benchmarking

---

## 3. User Stories

- **US-1:** As a **candidate**, I want to write a SELECT query and see
  if it matches the expected output.

- **US-2:** As an **interviewer**, I want to see the candidate's query
  results, so that I can help debug.

- **US-3:** As a **client recruiter**, I want to validate a SQL question
  with reference data, so that I know the question is solvable.

- **US-4:** As a **security officer**, I want to prevent destructive
  queries (DROP, TRUNCATE, etc.), so that the sandbox is safe.

- **US-5:** As a **developer**, I want schema + data to be provisioned
  per job, so that jobs don't interfere.

---

## 4. Functional Requirements

### 4.1 Container & Database

- **FR-1:** SQL execution SHALL run inside a container based on
  `postgres:16-alpine`.

- **FR-2:** Each execution SHALL create a **fresh database** inside
  the container.

- **FR-3:** The container SHALL be destroyed after execution.

- **FR-4:** The container SHALL NOT have network access except
  loopback.

- **FR-5:** The PostgreSQL data directory SHALL be in tmpfs (no disk).

- **FR-6:** The container SHALL start PostgreSQL with:
  - `fsync=off` (no durability needed)
  - `synchronous_commit=off`
  - `max_connections=10`
  - `shared_buffers=64MB`

- **FR-7:** Startup SHALL complete in under 3 seconds.

### 4.2 Schema + Data Provisioning

- **FR-8:** Each question's test case SHALL include:
  - `schemaSql` — CREATE TABLE statements
  - `seedSql` — INSERT statements
  - `query` — the candidate's SQL (or reference)
  - `expectedOutput` — the expected result set

- **FR-9:** `schemaSql` SHALL be executed in a transaction before
  running the candidate's query.

- **FR-10:** `seedSql` SHALL run inside the same transaction.

- **FR-11:** Any error during provisioning SHALL fail the test case
  with `provisioning_error`.

- **FR-12:** DDL and D