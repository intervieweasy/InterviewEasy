
---

# 📄 Spec 004-sandbox/008 — Observability

Save as: `specs\004-sandbox\008-observability.spec.md`

---

```markdown
# Spec: Sandbox Observability

**Service:** Sandbox
**Phase:** 004-sandbox
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

The Sandbox service executes untrusted code at scale. Observability is
critical for:
- Debugging failed executions
- Detecting resource abuse
- Tracking throughput and latency
- Alerting on infrastructure issues
- Auditing security incidents

This spec defines the Sandbox-specific logs, metrics, traces, and
alerts. It extends the platform-wide observability spec
(Spec 000-foundation-005) with sandbox-specific requirements.

---

## 2. Scope

### In Scope

- Structured logs for executions
- Prometheus metrics for sandbox
- OpenTelemetry traces for execution
- Security event logging
- Alerting thresholds
- Dashboard specifications
- Audit trails
- PII redaction for code

### Out of Scope

- Platform-wide logging (Spec 000-foundation-005)
- Container logs (not persisted)
- User-facing error UI (frontend spec)

---

## 3. User Stories

- **US-1:** As an **SRE**, I want per-language execution metrics, so
  that I can detect a slow runtime.

- **US-2:** As a **developer**, I want traces showing which phase of
  execution was slow (compile vs. run).

- **US-3:** As a **security officer**, I want to detect suspicious
  code patterns (e.g., attempts to escape the sandbox).

- **US-4:** As a **support engineer**, I want to find the logs for a
  specific submission by correlation ID.

- **US-5:** As a **manager**, I want to see daily execution volume and
  failure rate.

---

## 4. Functional Requirements

### 4.1 Structured Logs

- **FR-1:** Every execution SHALL emit a log entry with:
  - `timestamp` (UTC)
  - `level`
  - `service` = `sandbox-api`
  - `correlationId`
  - `traceId`
  - `spanId`
  - `jobId`
  - `tenantId`
  - `language`
  - `priority`
  - `event` (`started` | `completed` | `failed` | `cancelled`)

- **FR-2:** Completion logs SHALL include:
  - `durationMs` (total)
  - `compileDurationMs` (if compiled)
  - `executionDurationMs`
  - `testCaseCount`
  - `passedCount`
  - `failedCount`
  - `status`

- **FR-3:** Failure logs SHALL include:
  - `errorCode`
  - `errorMessage` (redacted)
  - `retryCount`

- **FR-4:** User code SHALL NEVER be logged in plaintext — only a
  hash (SHA-256) for correlation.

- **FR-5:** Test case inputs and expected outputs SHALL NOT be logged
  for hidden test cases.

- **FR-6:** Logs SHALL use structured templates:
  - ✅ `logger.LogInformation("Execution {JobId} for {Language} completed in {DurationMs}ms", ...)`
  - ❌ String concatenation

### 4.2 Prometheus Metrics

- **FR-7:** Metrics SHALL be exposed at `/metrics`.

- **FR-8:** Required metrics:
  - `sandbox_executions_total{language, status}` (counter)
  - `sandbox_execution_duration_seconds{language, phase}` (histogram)
  - `sandbox_test_cases_total{language, outcome}` (counter)
  - `sandbox_queue_depth{priority}` (gauge)
  - `sandbox_concurrency{language, scope}` (gauge)
  - `sandbox_workers_active` (gauge)
  - `sandbox_dlq_depth` (gauge)
  - `sandbox_container_start_duration_seconds{language}` (histogram)
  - `sandbox_compile_duration_seconds{language}` (histogram)
  - `sandbox_timeout_total{language}` (counter)
  - `sandbox_memory_peak_bytes{language}` (histogram)
  - `sandbox_output_size_bytes{language}` (histogram)
  - `sandbox_security_violations_total{type}` (counter)

- **FR-9:** All metrics SHALL include a `service="sandbox"` label.

- **FR-10:** `tenantId` SHALL NOT be a metric label (high cardinality) —
  use logs for tenant-specific analysis.

- **FR-11:** Metrics SHALL be exported via OpenTelemetry to Prometheus
  or directly scraped.

### 4.3 Traces

- **FR-12:** Every execution SHALL produce a trace with spans:
  - `sandbox.execute` (root)
    - `sandbox.enqueue`
    - `sandbox.dequeue`
    - `sandbox.container.start`
    - `sandbox.compile` (if compiled)
    - `sandbox.execute`
    - `sandbox.collect_results`

- **FR-13:** Span attributes SHALL include:
  - `sandbox.language`
  - `sandbox.job_id`
  - `sandbox.priority`
  - `sandbox.test_case_count`
  - `sandbox.status`
  - `sandbox.duration_ms`

- **FR-14:** Traces SHALL correlate with the caller's trace (Session
  or Question service).

- **FR-15:** User code SHALL NOT appear in span attributes.

- **FR-16:** Sampling rate SHALL be:
  - Development: 100%
  - Staging: 100%
  - Production: 10% for successes, 100% for failures

### 4.4 Security Event Logging

- **FR-17:** The following SHALL be logged as security events:
  - Attempts to import blocked modules
  - Attempts to access blocked files
  - Attempts to open sockets
  - Attempts to spawn processes
  - Repeated timeouts (> 5 in 10 min for one user)
  - Excessive memory allocation attempts
  - Compile errors mentioning dangerous patterns

- **FR-18:** Security events SHALL include:
  - `type` (e.g., `blocked_import`)
  - `evidence` (truncated snippet — no full code)
  - `userId`, `tenantId`, `jobId`

- **FR-19:** Security events SHALL increment
  `sandbox_security_violations_total` with a `type` label.

- **FR-20:** Repeated violations from one user SHALL trigger an alert.

### 4.5 Alerts

- **FR-21:** Alerts SHALL fire when:
  - `sandbox_dlq_depth > 100` for 5 minutes
  - `sandbox_queue_depth{priority="high"} > 100` for 2 minutes
  - `sandbox_executions_total{status="infrastructure_error"} > 10/min`
  - `sandbox_container_start_duration_seconds` p95 > 10s for 5 min
  - `sandbox_security_violations_total` rate > 100/min
  - `sandbox_workers_active == 0` for 1 minute
  - `sandbox_concurrency{scope="global"} >= limit` for 5 min

- **FR-22:** Alerts SHALL route to PagerDuty (Sev 1/2) or Slack
  (Sev 3).

- **FR-23:** Alert thresholds SHALL be configurable.

### 4.6 Dashboards

- **FR-24:** A Grafana dashboard SHALL be provided with panels:
  - Executions per second (by language)
  - Success / failure rate
  - Execution duration p50/p95/p99 (by language)
  - Queue depth (by priority)
  - Concurrency usage (global + per language)
  - DLQ depth
  - Worker count
  - Container start duration
  - Security violations
  - Top tenants by volume (bar chart)
  - Error breakdown

- **FR-25:** Dashboard SHALL be versioned in the infra repository.

- **FR-26:** Dashboard URL SHALL be linked from the admin app.

### 4.7 Audit Trail

- **FR-27:** Every job submission SHALL be audited:
  - Job ID
  - Tenant ID
  - User ID (if authenticated)
  - Language
  - Priority
  - Correlation ID
  - Submitted At
  - Code Hash (SHA-256)
  - Test case count

- **FR-28:** Every job result SHALL be audited:
  - Job ID
  - Status
  - Score (if applicable)
  - Duration
  - Completed At

- **FR-29:** Audit logs SHALL be retained for 90 days.

- **FR-30:** Audit logs SHALL be queryable by job ID, tenant ID,
  and correlation ID.

### 4.8 Correlation with Callers

- **FR-31:** Session service SHALL pass its correlation ID in the
  sandbox request.

- **FR-32:** Question service (validation) SHALL pass its correlation
  ID.

- **FR-33:** Sandbox logs SHALL include the correlation ID.

- **FR-34:** Following a correlation ID in logs SHALL show the full
  flow across services.

### 4.9 Performance Logging

- **FR-35:** For slow executions (> 3 seconds), the sandbox SHALL
  emit a warning log with:
  - Job ID
  - Language
  - Duration
  - Test case count
  - Reason for slowness (compile, execution, container start)

- **FR-36:** Slow executions SHALL be sampled for further analysis.

### 4.10 Cost Tracking

- **FR-37:** Metrics SHALL track:
  - CPU seconds consumed per tenant
  - Memory-seconds per tenant
  - Executions per tenant per day

- **FR-38:** These metrics SHALL feed the billing system (future).

- **FR-39:** Cost metrics SHALL be aggregated daily.

---

## 5. Non-Functional Requirements

- **NFR-1:** Logging overhead SHALL be under 1% of execution time.

- **NFR-2:** Metrics collection SHALL add under 2ms per execution.

- **NFR-3:** Trace sampling SHALL NOT impact execution latency.

- **NFR-4:** Logs SHALL be shipped within 5 seconds of emission.

- **NFR-5:** Dashboard data SHALL be up to date within 15 seconds.

- **NFR-6:** Metric cardinality SHALL stay under 10,000 series.

---

## 6. Domain Model

### 6.1 Log Event Schemas

**Execution Started:**
```json
{
  "event": "started",
  "jobId": "...",
  "tenantId": "...",
  "language": "csharp",
  "priority": "high",
  "testCaseCount": 10,
  "correlationId": "...",
  "timestamp": "..."
}