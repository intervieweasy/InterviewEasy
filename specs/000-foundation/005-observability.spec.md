# Spec: Observability & Logging

**Service:** Foundation
**Phase:** 000-foundation
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

InterviewEasy is a distributed microservices platform. When something
goes wrong — a slow API, a failed event, a stuck interview — engineers
must be able to trace the issue across multiple services, databases,
and message brokers within minutes.

Without a shared observability strategy:

- Logs are in different formats across services — impossible to grep
- No correlation IDs — cannot follow a request across services
- No metrics — cannot tell if the platform is healthy
- No traces — cannot see where time is spent in a request

This spec defines the **logging, metrics, tracing, and health check
conventions** that every service must follow. It is the foundation of
production readiness.

---

## 2. Scope

### In Scope

- Structured logging with Serilog
- Log levels and what to log at each level
- Correlation ID propagation across HTTP and messaging
- OpenTelemetry traces for HTTP, EF Core, and MassTransit
- Prometheus metrics via OpenTelemetry
- Health check endpoints (liveness, readiness, startup)
- Log retention and shipping to central storage
- Sensitive data redaction rules
- Alerting thresholds and SLOs

### Out of Scope

- Grafana dashboard design (infra repo, separate)
- Alerting routing (PagerDuty, Opsgenie — DevOps concern)
- Log analytics queries (implementation detail)
- APM vendor selection (we use OSS stack)
- Distributed tracing UI (Jaeger/Tempo — infra concern)

---

## 3. User Stories

- **US-1:** As an **SRE**, I want every request to carry a correlation ID,
  so that I can trace it across services in the logs.

- **US-2:** As a **developer**, I want structured JSON logs, so that I
  can filter and search them easily.

- **US-3:** As an **SRE**, I want Prometheus metrics from every service,
  so that I can build dashboards and alerts.

- **US-4:** As a **developer**, I want OpenTelemetry traces, so that I
  can see where time is spent in a request.

- **US-5:** As an **SRE**, I want health check endpoints on every service,
  so that Kubernetes can restart unhealthy pods.

- **US-6:** As a **compliance officer**, I want sensitive data redacted
  from logs, so that we don't leak PII or secrets.

---

## 4. Functional Requirements

### 4.1 Logging Framework

- **FR-1:** Serilog SHALL be the logging framework for all .NET services.

- **FR-2:** Serilog SHALL be configured via the shared
  `BuildingBlocks.Observability` library.

- **FR-3:** Logs SHALL be emitted as structured JSON in non-development
  environments.

- **FR-4:** Logs SHALL be human-readable console output in development.

- **FR-5:** Every log entry SHALL include:
  - `timestamp` (UTC, ISO 8601)
  - `level` (Verbose, Debug, Information, Warning, Error, Fatal)
  - `message` (structured template)
  - `service` (e.g., `identity-api`)
  - `environment` (dev, staging, prod)
  - `version` (build version)
  - `correlationId`
  - `traceId` and `spanId`
  - `userId` (when authenticated)
  - `tenantId` (when resolved)

### 4.2 Log Levels

- **FR-6:** Log levels SHALL be used as follows:
  - **Verbose** — Detailed debugging, disabled in production
  - **Debug** — Diagnostic info, enabled in staging only
  - **Information** — Normal operations (request started/completed,
    event published/consumed)
  - **Warning** — Recoverable issues (retry, validation failure,
    deprecated API use)
  - **Error** — Operation failed (unhandled exception, consumer failure,
    DB error)
  - **Fatal** — Application cannot continue (startup failure)

- **FR-7:** The minimum level SHALL be configurable per environment:
  - Development: `Debug`
  - Staging: `Information`
  - Production: `Information` (with `Warning` for noisy frameworks)

### 4.3 Correlation ID

- **FR-8:** Every HTTP request SHALL have a correlation ID:
  - If `X-Correlation-Id` header is present, use it
  - Otherwise, generate a new UUIDv4

- **FR-9:** Correlation ID SHALL be added to the response as
  `X-Correlation-Id` header.

- **FR-10:** Correlation ID SHALL flow through:
  - HTTP → command handlers
  - Command handlers → domain events
  - Domain events → integration events (as `CorrelationId` field)
  - Integration events → consumer handlers
  - Consumer handlers → HTTP calls to other services

- **FR-11:** Every log entry SHALL include the correlation ID.

- **FR-12:** When a service makes an outbound HTTP call, it SHALL
  propagate `X-Correlation-Id`.

### 4.4 OpenTelemetry Traces

- **FR-13:** OpenTelemetry SHALL be the tracing standard.

- **FR-14:** Every service SHALL emit traces to the OpenTelemetry
  Collector via OTLP.

- **FR-15:** The following SHALL be automatically instrumented:
  - ASP.NET Core (HTTP requests)
  - HttpClient (outbound HTTP)
  - EF Core (database queries)
  - MassTransit (message publish/consume)
  - Redis (cache operations)

- **FR-16:** Custom spans SHALL be created for:
  - Command handling (MediatR)
  - Query handling (MediatR)
  - Domain operations with significant logic

- **FR-17:** Span attributes SHALL include:
  - `service.name`
  - `service.version`
  - `http.method`, `http.route`, `http.status_code`
  - `db.system`, `db.statement` (redacted for sensitive queries)
  - `messaging.system`, `messaging.destination`
  - `tenant.id`, `user.id`
  - `error=true` on failures

- **FR-18:** Sensitive data (passwords, tokens, PII) SHALL NOT appear
  in span attributes.

### 4.5 Prometheus Metrics

- **FR-19:** Metrics SHALL be exposed at `/metrics` on every service.

- **FR-20:** Metrics SHALL follow Prometheus naming conventions:
  - Snake_case
  - Suffix `_total` for counters
  - Suffix `_seconds` for durations
  - Suffix `_bytes` for sizes
  - Unit before name for histograms (e.g., `http_request_duration_seconds`)

- **FR-21:** The following standard metrics SHALL be exported:
  - `http_requests_total{method, route, status}`
  - `http_request_duration_seconds{method, route, status}`
  - `http_requests_in_flight{method, route}`
  - `db_query_duration_seconds{operation, table}`
  - `events_published_total{event_type}`
  - `events_consumed_total{event_type, outcome}`
  - `outbox_pending_total{service}`
  - `cache_hits_total{cache}`
  - `cache_misses_total{cache}`
  - `background_job_duration_seconds{job_name}`
  - `background_job_failures_total{job_name}`

- **FR-22:** Every metric SHALL include a `service` label.

- **FR-23:** Every metric SHALL include a `tenant_id` label ONLY when
  cardinality is bounded (< 10,000 tenants) — otherwise aggregated.

### 4.6 Health Checks

- **FR-24:** Every service SHALL expose three health check endpoints:
  - `GET /health/live` — liveness (process is alive)
  - `GET /health/ready` — readiness (all dependencies healthy)
  - `GET /health/startup` — startup (initialization complete)

- **FR-25:** Liveness SHALL always return `200 OK` if the process is
  running, regardless of dependencies.

- **FR-26:** Readiness SHALL check:
  - PostgreSQL reachable
  - RabbitMQ reachable (if used)
  - Redis reachable (if used)
  - Migrations applied

- **FR-27:** Readiness SHALL return `503 Service Unavailable` if any
  critical dependency fails.

- **FR-28:** Startup SHALL return `200 OK` once the service is
  initialized (config loaded, migrations ready, DI built).

- **FR-29:** Health check responses SHALL include JSON with dependency
  status and duration.

### 4.7 Log Shipping

- **FR-30:** In production, logs SHALL be shipped to central storage.
  - Preferred: Loki (Grafana stack)
  - Alternative: Elasticsearch (ELK stack)

- **FR-31:** Log shipping SHALL be non-blocking — the service SHALL NOT
  fail if the log sink is unreachable.

- **FR-32:** Logs SHALL be buffered locally and retried on failure.

- **FR-33:** Log retention SHALL be:
  - Development: local file only
  - Staging: 7 days
  - Production: 90 days

- **FR-34:** Logs SHALL be indexed by `service`, `level`, `tenantId`,
  and `correlationId`.

### 4.8 Sensitive Data Redaction

- **FR-35:** The following fields SHALL be redacted in logs and traces:
  - Passwords
  - JWT tokens (access and refresh)
  - API keys
  - Database connection strings
  - Credit card numbers
  - National ID numbers
  - Email addresses (hashed, not raw)

- **FR-36:** Redaction SHALL be applied via Serilog enrichers/destructuring
  policies — not ad-hoc in code.

- **FR-37:** Custom sensitive fields SHALL be marked with a
  `[SensitiveData]` attribute on DTOs/entities for automatic redaction.

- **FR-38:** Sensitive fields SHALL still be usable in production code
  — redaction happens at log time only.

### 4.9 Service Level Objectives (SLOs)

- **FR-39:** Each service SHALL define SLOs in a `slo.yaml` file:
  - Availability (e.g., 99.9%)
  - Latency p95 (e.g., < 200ms)
  - Error rate (e.g., < 0.1%)

- **FR-40:** SLO burn rate alerts SHALL fire when error budget is
  consumed too fast.

- **FR-41:** SLOs SHALL be reviewed quarterly and adjusted.

### 4.10 Logging Conventions in Code

- **FR-42:** Logging SHALL use structured templates, not string
  concatenation:
  - ✅ `logger.LogInformation("Tenant {TenantId} created", tenantId)`
  - ❌ `logger.LogInformation($"Tenant {tenantId} created")`

- **FR-43:** Logging SHALL use `ILogger<T>` injected via DI, not static
  loggers.

- **FR-44:** Exceptions SHALL be logged with the exception object:
  - ✅ `logger.LogError(ex, "Failed to create tenant {TenantId}", tenantId)`
  - ❌ `logger.LogError("Failed: " + ex.Message)`

- **FR-45:** Log statements SHALL be at the correct level — no `Information`
  for debug details, no `Error` for expected validation failures.

- **FR-46:** Repeated log messages in a loop SHALL be avoided — aggregate
  and log once.

---

## 5. Non-Functional Requirements

- **NFR-1:** Logging overhead SHALL be under 2% of request latency.

- **NFR-2:** Log shipping SHALL NOT block request processing.

- **NFR-3:** Metrics collection SHALL add under 1ms per request.

- **NFR-4:** Traces SHALL sample at 10% in production, 100% in staging
  (configurable per environment).

- **NFR-5:** Health checks SHALL respond in under 200ms (p95).

- **NFR-6:** Log volume SHALL be under 5 GB/day across all services
  in production (baseline).

- **NFR-7:** Distributed traces SHALL be complete end-to-end — no
  broken trace chains.

- **NFR-8:** OpenTelemetry Collector SHALL buffer up to 1 hour of trace
  data during backend outages.

---

## 6. Domain Model

**N/A** — This spec defines observability conventions, not domain entities.

---

## 7. API Contracts

### 7.1 Health Check Endpoints

**`GET /health/live`**

Response 200:
```json
{
  "status": "Healthy",
  "timestamp": "2026-09-30T10:00:00Z"
}