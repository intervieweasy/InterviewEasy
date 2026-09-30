
---

# 📄 Spec 004-sandbox/007 — Job Queue

Save as: `specs\004-sandbox\007-job-queue.spec.md`

---

```markdown
# Spec: Job Queue

**Service:** Sandbox
**Phase:** 004-sandbox
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

The Sandbox service handles high-concurrency execution requests from
multiple sources:
- Question validation (bulk, low priority)
- Live interview submissions (single, high priority)

Requests must be queued, prioritized, and executed with concurrency
limits per language, per tenant, and globally. Queue backpressure
protects the platform from overload.

This spec defines the job queue architecture, prioritization, retries,
concurrency limits, and backpressure.

---

## 2. Scope

### In Scope

- Queue implementation (Redis-backed)
- Job priorities
- Per-language concurrency caps
- Per-tenant concurrency caps
- Global concurrency cap
- Backpressure and rate limiting
- Retries and dead-letter handling
- Job cancellation
- Job timeout (queue level)
- Job lifecycle tracking

### Out of Scope

- Container orchestration (Spec 004-sandbox-001)
- Individual language executors (Specs 004-sandbox-002 to 005)
- Observability (Spec 004-sandbox-008)

---

## 3. User Stories

- **US-1:** As an **interviewer**, I want my submissions prioritized
  over validation jobs, so that live interviews stay smooth.

- **US-2:** As an **SRE**, I want concurrency limits enforced, so
  that the platform doesn't overload.

- **US-3:** As a **developer**, I want a clean queue API, so that
  adding a language doesn't require queue changes.

- **US-4:** As a **client admin**, I want my validation jobs to
  finish reliably, even if queued.

- **US-5:** As a **compliance officer**, I want job lifecycles
  auditable, so that I can prove results are deterministic.

---

## 4. Functional Requirements

### 4.1 Queue Implementation

- **FR-1:** The queue SHALL be backed by **Redis** using a
  priority queue pattern.

- **FR-2:** Redis SHALL be a **separate instance** from the Redis
  used by other services (isolation).

- **FR-3:** The queue SHALL support **at-least-once delivery** with
  idempotent consumers (jobs are deterministic).

- **FR-4:** Jobs SHALL be persisted to Redis with a **TTL of 1 hour**.

- **FR-5:** Job payloads SHALL be under 1 MB.

### 4.2 Job Priorities

- **FR-6:** Jobs SHALL have three priority levels:
  - `High` — live interview submissions
  - `Normal` — interactive validation (single question)
  - `Low` — bulk import validation

- **FR-7:** Priority SHALL be encoded as separate Redis sorted sets.

- **FR-8:** Workers SHALL drain in order: High, Normal, Low.

- **FR-9:** A High job SHALL NOT wait behind more than 10 Normal jobs.

- **FR-10:** Age-based promotion: jobs older than 5 minutes in Normal
  are promoted to High; Low jobs older than 15 minutes are promoted
  to Normal.

### 4.3 Concurrency Limits

- **FR-11:** Three concurrency caps SHALL be enforced:
  - Global: 50 concurrent executions (default)
  - Per-language: 20 concurrent executions
  - Per-tenant: 10 concurrent executions

- **FR-12:** Limits SHALL be configurable per environment.

- **FR-13:** Enqueueing SHALL fail with 429 when the queue depth
  exceeds `maxQueueDepth` (default 1000).

- **FR-14:** Requests SHALL wait up to `maxQueueWaitSeconds`
  (default 30) before being rejected.

### 4.4 Enqueue Flow

- **FR-15:** The HTTP/gRPC endpoint SHALL validate the request,
  assign a job ID, and enqueue.

- **FR-16:** Enqueue SHALL be O(log n) — uses Redis ZADD.

- **FR-17:** If enqueue succeeds but the queue is full, return 429
  immediately (don't wait).

- **FR-18:** Enqueue SHALL be atomic — either the job is queued or
  the request fails.

### 4.5 Worker Flow

- **FR-19:** Workers SHALL poll Redis at a configurable interval
  (default 100ms).

- **FR-20:** Each worker SHALL pick up the highest-priority,
  oldest job that doesn't violate concurrency caps.

- **FR-21:** Worker SHALL mark the job as `processing` in Redis
  with a heartbeat TTL (30s).

- **FR-22:** Worker SHALL execute the job via the appropriate
  language executor.

- **FR-23:** Worker SHALL publish the result via the completion
  channel (SignalR or callback).

- **FR-24:** Worker SHALL acknowledge the job (remove from queue)
  after successful completion.

- **FR-25:** If the worker crashes, the job's heartbeat expires and
  the job is re-queued.

### 4.6 Retries

- **FR-26:** Failed jobs (infrastructure errors) SHALL be retried
  up to 3 times.

- **FR-27:** Retries SHALL use exponential backoff (5s, 30s, 120s).

- **FR-28:** After 3 retries, jobs SHALL move to a dead-letter queue
  (Redis list `sandbox:dlq`).

- **FR-29:** DLQ entries SHALL be inspected via admin API.

- **FR-30:** Business logic failures (compile errors, test failures)
  SHALL NOT be retried — they're not errors from the queue's
  perspective.

### 4.7 Cancellation

- **FR-31:** Callers SHALL cancel a queued job via
  `DELETE /api/v1/sandbox/jobs/{jobId}`.

- **FR-32:** If the job is `queued`, it SHALL be removed from the
  queue.

- **FR-33:** If the job is `processing`, the container SHALL be
  killed, and the job marked `cancelled`.

- **FR-34:** Cancellation SHALL be idempotent.

### 4.8 Job Lifecycle

- **FR-35:** A job SHALL have these states:
  - `queued` — in Redis, waiting for a worker
  - `processing` — picked up by a worker
  - `completed` — finished with a result
  - `failed` — infrastructure error after retries
  - `cancelled` — cancelled by the caller
  - `timed_out` — exceeded max queue wait

- **FR-36:** Lifecycle transitions SHALL be recorded in Redis (and
  optionally in Application Insights).

- **FR-37:** Job state SHALL be queryable via
  `GET /api/v1/sandbox/jobs/{jobId}` for up to 1 hour after
  completion.

### 4.9 Backpressure

- **FR-38:** When queue depth > 80% of max, new Normal and Low jobs
  SHALL be rejected with 429.

- **FR-39:** When queue depth = 100%, all new jobs SHALL be rejected
  with 429 (except High from the same tenant).

- **FR-40:** Per-tenant rate limit: 10 submissions/second.

- **FR-41:** Global rate limit: 100 submissions/second.

- **FR-42:** Rate limit responses SHALL include `Retry-After` header.

### 4.10 Fairness

- **FR-43:** Fair scheduling SHALL prevent one tenant from starving
  others.

- **FR-44:** Round-robin across tenants within a priority level.

- **FR-45:** A tenant cannot exceed per-tenant concurrency even if
  global capacity is available.

### 4.11 Health Checks

- **FR-46:** Sandbox SHALL expose queue health:
  - Queue depth per priority
  - Concurrency usage
  - Worker count
  - DLQ depth

- **FR-47:** Health check endpoint SHALL fail if Redis is unreachable.

---

## 5. Non-Functional Requirements

- **NFR-1:** Enqueue SHALL complete in under 20ms (p95).

- **NFR-2:** Worker SHALL pick up a job within 200ms of enqueue at
  low load.

- **NFR-3:** Global throughput SHALL sustain 100 jobs/second.

- **NFR-4:** Queue depth SHALL be monitored with alerts at > 500.

- **NFR-5:** Job state SHALL survive worker restarts.

- **NFR-6:** DLQ depth SHALL trigger alerts at > 100.

---

## 6. Domain Model

### 6.1 Entities

```csharp
public sealed class SandboxJob
{
    public Guid Id { get; init; }
    public string Language { get; init; }
    public string TenantId { get; init; }
    public JobPriority Priority { get; init; }
    public JobState State { get; set; }
    public string Payload { get; init; }
    public DateTime QueuedAt { get; init; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RetryCount { get; set; }
    public string? LastError { get; set; }
}

public enum JobPriority { Low = 0, Normal = 1, High = 2 }

public enum JobState
{
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled,
    TimedOut
}