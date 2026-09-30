# Spec: Solution Structure

**Service:** Foundation
**Phase:** 000-foundation
**Status:** Implemented
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

InterviewEasy is an enterprise interview platform built as a
**monorepo microservices architecture**. It will eventually contain
12+ backend services, 3 Backend-for-Frontend (BFF) layers, an API
gateway, and 4 frontend applications.

Without a consistent, enforced solution structure:

- New services would be scaffolded differently by different developers
- Dependency direction would erode (domain referencing infrastructure, etc.)
- Package versions would drift across services
- Tests would be scattered or missing
- Onboarding new team members would take weeks

This spec defines the **canonical structure** every service must follow,
the **naming conventions** used throughout the repo, and the
**dependency rules** that Clean Architecture demands.

This is the spec that makes every future spec predictable.

---

## 2. Scope

### In Scope

- Repository folder layout (root, `src/`, `tests/`, `specs/`, `docs/`)
- Project naming conventions for services
- 4-layer Clean Architecture per service
  - `{Service}.API`
  - `{Service}.Application`
  - `{Service}.Domain`
  - `{Service}.Infrastructure`
- Dependency direction rules between layers
- Central Package Management (CPM) via `Directory.Packages.props`
- Shared MSBuild properties via `Directory.Build.props`
- Test project mirroring (unit + integration per service)
- BFF and Gateway placement
- BuildingBlocks (shared libraries) placement

### Out of Scope

- Business logic of any specific service
- Database schema design (covered in Spec 002 — Database Strategy)
- Event bus design (covered in Spec 003 — Event Bus)
- Frontend folder structure (covered in Spec 012 — Frontends)
- Docker / Kubernetes configuration (covered in a later DevOps spec)
- CI/CD pipelines (covered in a later DevOps spec)

---

## 3. User Stories

- **US-1:** As a **backend developer**, I want every service to follow the
  same 4-layer structure, so that I can navigate any service in the repo
  without re-learning its layout.

- **US-2:** As a **tech lead**, I want dependency direction to be enforced
  structurally, so that Clean Architecture is not violated by accident.

- **US-3:** As a **DevOps engineer**, I want all services to be
  independently buildable and testable, so that CI can run per-service
  pipelines.

- **US-4:** As a **new team member**, I want to read one spec and
  understand the entire repo layout, so that I can contribute on day one.

- **US-5:** As a **QA engineer**, I want every service to have mirrored
  unit and integration test projects, so that I always know where tests live.

---

## 4. Functional Requirements

- **FR-1:** The repo root SHALL contain these folders:
  `src/`, `tests/`, `specs/`, `docs/`, `infrastructure/`, `scripts/`
  (last two may be added later; folder is reserved).

- **FR-2:** Every backend service SHALL live under `src/Services/{ServiceName}/`.

- **FR-3:** Every backend service SHALL have exactly 4 projects:
  - `InterviewEasy.{ServiceName}.API`
  - `InterviewEasy.{ServiceName}.Application`
  - `InterviewEasy.{ServiceName}.Domain`
  - `InterviewEasy.{ServiceName}.Infrastructure`

- **FR-4:** Every backend service SHALL have exactly 2 test projects under
  `tests/`:
  - `tests/UnitTests/InterviewEasy.{ServiceName}.UnitTests/`
  - `tests/IntegrationTests/InterviewEasy.{ServiceName}.IntegrationTests/`

- **FR-5:** Dependency direction SHALL be:
  - `Domain` → `BuildingBlocks.Core` only
  - `Application` → `Domain`, `BuildingBlocks.Core`, `BuildingBlocks.Common`
  - `Infrastructure` → `Application`, `BuildingBlocks.EventBus`, EF Core
  - `API` → `Application`, `Infrastructure`, `BuildingBlocks.Common`,
    `BuildingBlocks.Observability`
  - `Domain` SHALL NOT reference `Application`, `Infrastructure`, or `API`
  - `Application` SHALL NOT reference `Infrastructure` or `API`

- **FR-6:** BFF projects SHALL live under `src/Services/Bff/`
  and be named `InterviewEasy.Bff.{Consumer}` where consumer ∈
  {Admin, Customer, Candidate}.

- **FR-7:** The API Gateway SHALL live under `src/Gateways/InterviewEasy.Gateway/`.

- **FR-8:** Shared libraries SHALL live under `src/BuildingBlocks/`
  and be named `InterviewEasy.BuildingBlocks.{Concern}`.

- **FR-9:** All NuGet package versions SHALL be defined **only** in
  `Directory.Packages.props` at the repo root. Individual `.csproj`
  files SHALL NOT contain `Version="..."` attributes on `PackageReference`.

- **FR-10:** All projects SHALL inherit shared MSBuild properties from
  `Directory.Build.props` at the repo root.

- **FR-11:** All projects SHALL target `net9.0` unless explicitly exempted.

- **FR-12:** Nullable reference types SHALL be enabled on all projects.

- **FR-13:** Warnings SHALL be treated as errors (`TreatWarningsAsErrors=true`).

- **FR-14:** Every spec file SHALL live under `specs/{phase}/{number}-{feature}.spec.md`.

- **FR-15:** Every ADR SHALL live under `docs/adr/{number}-{decision}.md`.

---

## 5. Non-Functional Requirements

- **NFR-1:** Clean `dotnet build` SHALL complete in under 90 seconds
  on a developer workstation.

- **NFR-2:** The solution SHALL have zero circular dependencies between
  projects at any layer.

- **NFR-3:** Solution structure SHALL support 20+ services without
  reorganization.

- **NFR-4:** Every project SHALL be independently buildable and
  deployable (`dotnet publish` per project).

- **NFR-5:** CI builds SHALL run per-service, not per-solution, to
  reduce build time.

- **NFR-6:** Adding a new service SHALL require no changes to existing
  services' project files.

---

## 6. Domain Model

**N/A** — This spec defines structure, not domain entities.

---

## 7. API Contracts

**N/A** — This spec defines structure, not endpoints.

---

## 8. Data Model

**N/A** — Database schema is covered in Spec 002 — Database Strategy.

---

## 9. Events

**N/A** — Event bus is covered in Spec 003 — Event Bus.

---

## 10. Acceptance Criteria

- [x] **AC-1:** `dotnet build` at repo root succeeds with **0 errors**.
- [x] **AC-2:** All 12 backend services exist under `src/Services/`.
- [x] **AC-3:** Each backend service has exactly 4 projects
  (`.API`, `.Application`, `.Domain`, `.Infrastructure`).
- [x] **AC-4:** Each backend service has 1 unit test project and 1
  integration test project.
- [x] **AC-5:** 3 BFF projects exist under `src/Services/Bff/`.
- [x] **AC-6:** The API Gateway project exists under `src/Gateways/`.
- [x] **AC-7:** 4 BuildingBlocks exist under `src/BuildingBlocks/`.
- [x] **AC-8:** `Directory.Packages.props` exists at repo root with
  `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`.
- [x] **AC-9:** No `.csproj` file contains a `Version` attribute on any
  `PackageReference`.
- [x] **AC-10:** `Directory.Build.props` exists at repo root with
  `net9.0`, `Nullable=enable`, `TreatWarningsAsErrors=true`.
- [x] **AC-11:** `specs/` folder with 13 phase subfolders exists.
- [x] **AC-12:** `specs/_templates/feature.spec.md` template exists.
- [x] **AC-13:** `specs/README.md` explaining the SDD loop exists.
- [x] **AC-14:** `docs/adr/` folder exists for Architecture Decision Records.
- [x] **AC-15:** `dotnet test` runs successfully against the solution.

**All acceptance criteria are satisfied** as of 2026-09-30.

---

## 11. Open Questions

- **Q1:** Should we migrate from `.sln` to the newer `.slnx` format?
  - Owner: Architecture
  - Status: Open — evaluate once .slnx reaches GA support in all IDEs.

- **Q2:** Should BFF projects have the same 4-layer structure as services?
  - Owner: Architecture
  - Status: Decided — No. BFFs are thin aggregation layers only; they
    have just an API project (no Domain, Application, or Infrastructure).

- **Q3:** When should a new service be split out of an existing one?
  - Owner: Architecture
  - Status: Open — revisit at Phase 5 (Session service).

- **Q4:** Should tests be colocated with services (e.g.,
  `src/Services/X/X.Tests/`) or centralized under `tests/`?
  - Owner: Architecture
  - Status: Decided — centralized under `tests/` (chosen for
    repo-wide test discovery and CI simplification).

---

## 12. References

- **ADR-0001:** Schema-per-tenant multi-tenancy (planned)
- **ADR-0002:** Yjs over Operational Transform for code sync (planned)
- **ADR-0003:** LiveKit as SFU for video (planned)
- **ADR-0004:** RabbitMQ via MassTransit for events (planned)
- **Spec 002:** Database Strategy
- **Spec 003:** Event Bus
- **Repository:** https://github.com/intervieweasy/InterviewEasy

---

**End of Spec**