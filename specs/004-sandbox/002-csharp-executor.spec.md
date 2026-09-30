
---

# 📄 Spec 004-sandbox/002 — C# Executor

Save as: `specs\004-sandbox\002-csharp-executor.spec.md`

---

```markdown
# Spec: C# Executor

**Service:** Sandbox
**Phase:** 004-sandbox
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

The C# executor runs candidate or reference C# code inside an isolated
container. It supports the standard interview format: the candidate
implements a method or class, and the executor invokes it with test
inputs, capturing the return value or exception.

This spec defines the C# execution container, compilation process,
entry point discovery, input/output marshaling, and error reporting.

---

## 2. Scope

### In Scope

- .NET 9 runtime container
- Roslyn-based compilation
- Entry point discovery (method vs. class)
- Test runner harness
- Input marshaling (JSON → method arguments)
- Output marshaling (return value → JSON)
- Exception handling
- Compilation error reporting
- Supported .NET features and namespaces

### Out of Scope

- Other languages (specs 003–005)
- Test case scoring (spec 006)
- Job queue (spec 007)
- Observability (spec 008)

---

## 3. User Stories

- **US-1:** As a **candidate**, I want my C# code to compile and run,
  so that I can solve the question.

- **US-2:** As an **interviewer**, I want compile errors to be shown
  clearly, so that I can help the candidate.

- **US-3:** As a **client recruiter**, I want to see the return value
  of the reference solution, so that I know the test cases are valid.

- **US-4:** As a **security officer**, I want user code isolated from
  the runtime and host, so that malicious code cannot breach the
  platform.

- **US-5:** As a **developer**, I want a clean wrapper around Roslyn,
  so that I don't have to hand-roll compilation.

---

## 4. Functional Requirements

### 4.1 Container

- **FR-1:** The C# container SHALL be based on
  `mcr.microsoft.com/dotnet/runtime:9.0-alpine` or equivalent
  minimal image.

- **FR-2:** The container SHALL include:
  - .NET 9 runtime
  - Minimal entry point harness (no SDK at runtime)
  - Compiled harness assembly pinned to the container

- **FR-3:** The container SHALL be under 200 MB.

- **FR-4:** The container SHALL NOT include the .NET SDK (compilation
  happens at the orchestrator, not in the container).

### 4.2 Compilation

- **FR-5:** Compilation SHALL happen in a **compile container**
  (separate from the execute container) using the .NET SDK.

- **FR-6:** Compilation SHALL use **Roslyn** (Microsoft.CodeAnalysis.CSharp).

- **FR-7:** Compilation SHALL produce a single in-memory assembly
  loaded into the execute container.

- **FR-8:** Compilation SHALL be limited to 10 seconds wall clock.

- **FR-9:** Compilation SHALL reference these assemblies:
  - `System.Runtime`
  - `System.Collections`
  - `System.Linq`
  - `System.Text`
  - `System.Text.Json`
  - `System.Console`

- **FR-10:** Disallowed references SHALL include:
  - `System.IO` — file system access
  - `System.Net` — network access
  - `System.Reflection.Emit` — dynamic code generation
  - `System.Diagnostics.Process` — process spawning
  - `Microsoft.Win32` — registry

- **FR-11:** Compilation errors SHALL be returned with:
  - Error code (e.g., `CS0103`)
  - Line and column
  - Message

### 4.3 Entry Point Discovery

- **FR-12:** The executor SHALL discover the entry point based on the
  question's `FunctionSignature`:
  - Class name (e.g., `Solution`)
  - Method name (e.g., `FindMax`)
  - Return type
  - Parameters

- **FR-13:** Discovery SHALL use reflection on the compiled assembly.

- **FR-14:** If the method is not found, return
  `entry_point_not_found` with a clear error.

- **FR-15:** Both static and instance methods SHALL be supported.

### 4.4 Input Marshaling

- **FR-16:** Test case input SHALL be a JSON object with parameter
  names matching the method signature.

  Example:
  ```json
  { "arr": [3, 7, 2, 9, 1], "target": 9 }