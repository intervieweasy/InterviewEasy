# Spec: Sandbox Overview

**Service:** Sandbox
**Phase:** 004-sandbox
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

The Sandbox service is the isolated execution layer of InterviewEasy.
It runs untrusted candidate code against predefined test cases and
returns pass/fail results. It is invoked from two contexts:

1. **Question validation** (Phase 3) — the reference solution runs
   against test cases to verify the question is solvable.
2. **Live interview sessions** (Phase 5) — the candidate's code runs
   against sample + hidden test cases during a live interview.

Because the code is untrusted, the sandbox must isolate execution
strictly: no network, no host filesystem, CPU/memory/time limits,
and process cleanup. A single vulnerability exposes the entire
platform.

This spec defines the sandbox architecture, isolation model, supported
languages, resource limits, and security guarantees.

---

## 2. Scope

### In Scope

- Sandbox architecture and isolation model
- Supported languages and runtimes
- Resource limits (CPU, memory, disk, time)
- Network policy
- File system policy
- Security guarantees
- Invocation contexts (validation vs live interview)
- Communication protocol (gRPC + SignalR streaming)
- Container lifecycle
- Concurrency model
- Failure modes

### Out of Scope

- Individual language executors (Specs 004-sandbox-002 through 005)
- Test case scoring (Spec 004-sandbox-006)
- Job queue (Spec 004-sandbox-007)
- Observability details (Spec 004-sandbox-008)
- Recording or playback

---

## 3. User Stories

- **US-1:** As a **client**, I want my questions' solutions validated
  by the sandbox, so that I know candidates can solve them.

- **US-2:** As an **interviewer**, I want the candidate's code to run
  in under 5 seconds, so that the interview flow stays smooth.

- **US-3:** As a **security officer**, I want untrusted code isolated
  from the platform, so that a malicious submission cannot breach
  other tenants or infrastructure.

- **US-4:** As an **SRE**, I want sandbox jobs to be traced and
  monitored, so that I can debug failures.

- **US-5:** As a **developer**, I want a clean interface (gRPC) to
  submit code and receive results, so that I don't need to know the
  internals.

---

## 4. Functional Requirements

### 4.1 Architecture

- **FR-1:** The Sandbox service SHALL be implemented as a standalone
  ASP.NET Core service that exposes:
  - A **gRPC endpoint** for synchronous execution requests
  - A **SignalR stream** for async result delivery to clients
  - An **HTTP fallback** endpoint for non-gRPC callers

- **FR-2:** Execution SHALL run inside **Docker containers** with
  strict isolation (see §4.3).

- **FR-3:** The Sandbox service SHALL orchestrate containers via the
  Docker API (Spec 004-sandbox-007).

- **FR-4:** A single sandbox instance SHALL support **concurrent
  execution** of multiple jobs, subject to resource limits.

- **FR-5:** Jobs SHALL be processed by a job queue (Spec 004-sandbox-007)
  with a maximum concurrency cap per language.

### 4.2 Supported Languages

- **FR-6:** The Sandbox SHALL support these languages initially:
  - C# (.NET 9, Roslyn)
  - SQL (PostgreSQL 16 sandbox)
  - Python 3.12
  - JavaScript / TypeScript (Node.js 20)
  - Java 21 (future — Phase 4b)
  - Go 1.22 (future — Phase 4b)

- **FR-7:** Each language SHALL have a dedicated container image
  built and versioned by the platform.

- **FR-8:** Container images SHALL be:
  - Based on official minimal images (alpine or slim variants)
  - Pinned to specific versions
  - Signed and scanned for CVEs on build

### 4.3 Isolation Model

- **FR-9:** Each execution SHALL run in a **fresh container** with:
  - No network access (`--network none`)
  - Read-only root filesystem (`--read-only`)
  - Writable tmpfs for scratch (`--tmpfs /tmp:size=64m`)
  - Non-root user (`--user 1000:1000`)
  - No privileged capabilities (`--cap-drop ALL`)
  - No new privileges (`--security-opt no-new-privileges`)
  - PID limit (`--pids-limit 64`)
  - CPU limit (`--cpus 1`)
  - Memory limit (`--memory 256m`, `--memory-swap 256m`)
  - Execution timeout (killed after N seconds)

- **FR-10:** Containers SHALL be destroyed after execution — no reuse.

- **FR-11:** Containers SHALL NOT mount the host Docker socket or any
  host paths.

- **FR-12:** Seccomp profile SHALL be applied to restrict syscalls to
  the minimum required.

- **FR-13:** The sandbox SHALL run on dedicated nodes (Kubernetes
  taints/tolerations) separate from the API nodes.

### 4.4 Resource Limits

- **FR-14:** Per-execution limits (configurable per question):
  - **Wall clock time:** 2 seconds (default), up to 30 seconds
  - **CPU time:** 1 second
  - **Memory:** 256 MB (default), up to 2048 MB
  - **Disk I/O:** 64 MB tmpfs
  - **Output size:** 1 MB stdout/stderr
  - **Compilation time:** 10 seconds (for compiled languages)

- **FR-15:** Exceeding limits SHALL terminate the container and return
  a specific failure reason (timeout, memory, output_too_large).

- **FR-16:** Overall job timeout SHALL be enforced (60 seconds max per
  job).

### 4.5 Network Policy

- **FR-17:** Containers SHALL have **zero network access** — no DNS,
  no HTTP, no socket connections outside the container.

- **FR-18:** Exception: SQL containers MAY have a scoped connection to
  a dedicated PostgreSQL instance (see Spec 004-sandbox-003).

- **FR-19:** All other containers SHALL be blocked from network at the
  kernel level (`--network none`).

### 4.6 File System Policy

- **FR-20:** The container root filesystem SHALL be read-only.

- **FR-21:** Writable areas SHALL be limited to:
  - `/tmp` (tmpfs, 64 MB)
  - `/workspace` (tmpfs, 64 MB) — for compilation artifacts

- **FR-22:** No access to host paths, environment secrets, or other
  containers.

### 4.7 Invocation Contexts

- **FR-23:** Two invocation contexts SHALL be supported:
  - **Validation** (question-authoring): trusted solution code
  - **Evaluation** (live interview): untrusted candidate code

- **FR-24:** Both contexts SHALL use the same isolation model —
  no trust assumptions.

- **FR-25:** Validation requests SHALL be prioritized lower than
  evaluation requests in the queue.

- **FR-26:** Evaluation requests SHALL be prioritized higher
  (interview latency matters).

### 4.8 Communication Protocol

- **FR-27:** The Sandbox SHALL expose a gRPC service:
  ```protobuf
  service SandboxExecution {
    rpc Execute(ExecuteRequest) returns (ExecuteResponse);
    rpc ExecuteStream(ExecuteRequest) returns (stream ExecuteProgress);
  }

  message ExecuteRequest {
    string language = 1;
    string code = 2;
    repeated TestCaseInput test_cases = 3;
    ExecutionLimits limits = 4;
    string correlation_id = 5;
  }

  message ExecuteResponse {
    ExecutionStatus status = 1;
    repeated TestCaseResult results = 2;
    int32 total_duration_ms = 3;
    string error = 4;
  }