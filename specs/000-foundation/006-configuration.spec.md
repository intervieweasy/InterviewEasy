
---

# 📄 Spec 006 — Configuration & Secrets

Save as: `specs\000-foundation\006-configuration.spec.md`

---

```markdown
# Spec: Configuration & Secrets

**Service:** Foundation
**Phase:** 000-foundation
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

InterviewEasy runs in three environments (development, staging, production),
each with its own database, message broker, secrets, and feature flags.
Configuration mistakes are a common source of outages — a wrong connection
string, a leaked API key, or a missing environment variable.

This spec defines how configuration and secrets are **structured, loaded,
validated, and rotated** across all services. It ensures that:

- No secrets ever land in source control
- Missing or invalid configuration fails fast at startup
- Every service uses the same configuration patterns
- Production secrets are managed in a vault, not files

---

## 2. Scope

### In Scope

- Configuration file structure (`appsettings.json` per environment)
- Environment variable overrides
- Azure Key Vault / AWS Secrets Manager integration
- Strongly-typed configuration with `IOptions<T>`
- Startup validation (fail fast)
- Secret rotation strategy
- Per-environment configuration
- Feature flags
- Configuration for local development (Docker Compose)
- What NOT to put in configuration

### Out of Scope

- Tenant-specific runtime configuration (that's data, not configuration)
- Feature flag evaluation logic (separate spec if needed)
- Deployment pipeline configuration (DevOps spec)
- Kubernetes ConfigMaps and Secrets (DevOps concern)
- Certificate management (future security spec)

---

## 3. User Stories

- **US-1:** As a **backend developer**, I want strongly-typed configuration,
  so that I get compile-time safety and IntelliSense for config values.

- **US-2:** As a **DevOps engineer**, I want secrets loaded from Key Vault,
  so that no credential ever lands in source control.

- **US-3:** As an **SRE**, I want the service to fail fast on invalid
  configuration, so that misconfigured pods don't run in a broken state.

- **US-4:** As a **compliance officer**, I want an audit trail of secret
  access, so that we can prove who accessed what and when.

- **US-5:** As a **developer**, I want a working local dev setup with
  Docker Compose, so that I can run the stack without any cloud
  dependencies.

---

## 4. Functional Requirements

### 4.1 Configuration Sources (in precedence order)

- **FR-1:** Configuration SHALL be loaded from the following sources,
  with later sources overriding earlier ones:
  1. `appsettings.json` — base defaults
  2. `appsettings.{Environment}.json` — per-environment overrides
  3. Environment variables
  4. Azure Key Vault / AWS Secrets Manager (production only)
  5. Command-line arguments (for local debugging)

- **FR-2:** Environment variables SHALL use the `__` (double underscore)
  separator for nesting:
  - `ConnectionStrings__Default`
  - `RabbitMQ__Host`
  - `Jwt__SigningKey`

- **FR-3:** Key Vault SHALL be the source of truth for secrets in
  staging and production.

- **FR-4:** The service SHALL NOT start if required secrets cannot be
  loaded.

### 4.2 Strongly-Typed Configuration

- **FR-5:** Every configuration section SHALL have a strongly-typed
  options class implementing the pattern:
  ```csharp
  public sealed class JwtOptions
  {
      public const string SectionName = "Jwt";
      public required string Issuer { get; init; }
      public required string Audience { get; init; }
      public required string SigningKey { get; init; }
      public int AccessTokenMinutes { get; init; } = 15;
      public int RefreshTokenDays { get; init; } = 7;
  }