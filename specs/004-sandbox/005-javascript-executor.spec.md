
---

# 📄 Spec 004-sandbox/005 — JavaScript/TypeScript Executor

Save as: `specs\004-sandbox\005-javascript-executor.spec.md`

---

```markdown
# Spec: JavaScript / TypeScript Executor

**Service:** Sandbox
**Phase:** 004-sandbox
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

JavaScript and TypeScript are common for full-stack and frontend
interview questions. The JS/TS executor runs candidate code inside a
Node.js container, evaluates it against test cases, and returns
results.

TypeScript adds a compile step (tsc or esbuild) before execution.

This spec defines the Node.js container, TS compilation, entry point
discovery, input/output marshaling, module restrictions, and error
handling.

---

## 2. Scope

### In Scope

- Node.js 20 runtime container
- JavaScript execution (native)
- TypeScript compilation via esbuild
- Entry point discovery (function)
- Input marshaling (JSON → args)
- Output marshaling (return → JSON)
- Module whitelist
- Blocked globals
- Exception handling

### Out of Scope

- Browser JavaScript (jsdom)
- ES modules vs CommonJS debate (support both)
- Deno / Bun
- React/Vue component testing

---

## 3. User Stories

- **US-1:** As a **candidate**, I want to write a JS or TS function
  and see if it passes the tests.

- **US-2:** As an **interviewer**, I want to see runtime errors
  clearly, so that I can help the candidate debug.

- **US-3:** As a **client recruiter**, I want to validate JS/TS
  questions with reference solutions.

- **US-4:** As a **security officer**, I want to block file system
  and network access from JS/TS code.

- **US-5:** As a **developer**, I want the same API regardless of
  whether the user submits JS or TS.

---

## 4. Functional Requirements

### 4.1 Container

- **FR-1:** The Node container SHALL be based on
  `node:20-alpine`.

- **FR-2:** The container SHALL include:
  - Node.js 20
  - The harness (`runner.mjs`)
  - `esbuild` binary for TS compilation (in the same container)

- **FR-3:** The container SHALL be under 150 MB.

- **FR-4:** The container SHALL run as a non-root user.

### 4.2 JavaScript Execution

- **FR-5:** JS code SHALL be executed natively by Node.js.

- **FR-6:** The harness SHALL import the user's code via dynamic
  `import()` from `/tmp/solution.mjs`.

- **FR-7:** The harness SHALL support both ESM (`export`) and CJS
  (`module.exports`) — the harness detects and adapts.

- **FR-8:** Top-level `await` SHALL be supported (Node 20 native).

### 4.3 TypeScript Compilation

- **FR-9:** TS code SHALL be compiled by `esbuild` before execution.

- **FR-10:** Compilation SHALL target ES2022 and Node 20.

- **FR-11:** Compilation SHALL produce a single ESM file.

- **FR-12:** Type errors SHALL NOT block execution — only syntax
  errors do (TypeScript's `transpileOnly` mode).

- **FR-13:** Compile errors SHALL return `compile_error` with line +
  column.

### 4.4 Entry Point Discovery

- **FR-14:** The question SHALL specify the exported function name.

  Example:
  ```typescript
  export function findMax(arr: number[]): number {
    return Math.max(...arr);
  }