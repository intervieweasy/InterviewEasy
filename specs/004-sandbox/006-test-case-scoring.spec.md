
---

# 📄 Spec 004-sandbox/006 — Test Case Scoring

Save as: `specs\004-sandbox\006-test-case-scoring.spec.md`

---

```markdown
# Spec: Test Case Scoring

**Service:** Sandbox
**Phase:** 004-sandbox
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

After executing candidate code against test cases, the sandbox computes
a score. Different questions need different scoring:

- **All-or-nothing** — a candidate passes only if all test cases pass.
- **Partial credit** — score = percentage of test cases passed.
- **Weighted** — test cases carry different points; score = sum of
  passed points.

The scoring mode is defined per question in the `evaluation_config`
(Spec 003-question-001). This spec defines how scoring is computed,
how partial credit is presented, and how results feed back into the
Session service (Phase 5).

---

## 2. Scope

### In Scope

- Scoring modes (all-or-nothing, partial, weighted)
- Point allocation per test case
- Score aggregation
- Result categorization (passed, failed, timeout, error)
- Score rounding and representation
- Result publishing to Session
- Handling of infrastructure errors

### Out of Scope

- Solution verification (question validation) — different from
  candidate scoring
- Feedback scoring (Phase 6)
- Interview overall scoring (Phase 6)

---

## 3. User Stories

- **US-1:** As a **candidate**, I want partial credit for partially
  correct solutions.

- **US-2:** As an **interviewer**, I want to see which test cases
  failed and why.

- **US-3:** As a **client recruiter**, I want scoring to reflect the
  question's intent (weighted or all-or-nothing).

- **US-4:** As a **client admin**, I want scores to be reproducible.

- **US-5:** As a **developer**, I want a clear API for computing scores
  from test results.

---

## 4. Functional Requirements

### 4.1 Scoring Modes

- **FR-1:** The question's `evaluation_config.scoringMode` SHALL be one
  of:
  - `all_or_nothing` — score = 100 if all pass, else 0
  - `partial` — score = (passed / total) * 100
  - `weighted` — score = (sum of passed points / total points) * 100

- **FR-2:** If not specified, default to `partial`.

- **FR-3:** Sample test cases (visible to candidate) SHALL NOT count
  toward score by default.

- **FR-4:** Hidden and performance test cases SHALL count.

- **FR-5:** Question config MAY set `includeSampleInScore: true` to
  count sample test cases (rare).

### 4.2 Point Allocation

- **FR-6:** Each hidden test case SHALL have `points` (1–100).

- **FR-7:** Sum of points across hidden test cases SHALL define the
  total score budget.

- **FR-8:** In `weighted` mode, score = `(sum of passed points / total
  points) * 100`.

- **FR-9:** In `partial` mode, points are ignored — all test cases
  weighted equally.

- **FR-10:** In `all_or_nothing` mode, points are irrelevant.

### 4.3 Result Categorization

- **FR-11:** Each test case SHALL have one of:
  - `passed` — output matched
  - `failed` — output mismatched
  - `timeout` — exceeded time limit
  - `error` — runtime error (exception)
  - `skipped` — not executed (e.g., earlier compile failure)

- **FR-12:** Timeout and error SHALL count as failures for scoring.

- **FR-13:** `skipped` SHALL NOT count as pass or fail.

### 4.4 Score Aggregation

- **FR-14:** The overall score SHALL be a decimal 0–100.

- **FR-15:** Score SHALL be rounded to 2 decimal places.

- **FR-16:** If all test cases are `skipped` (compile error), score
  SHALL be 0 with reason `not_executed`.

- **FR-17:** If infrastructure error occurred, score SHALL NOT be
  reported; return `infrastructure_error` instead.

### 4.5 Output Comparison

- **FR-18:** Output comparison SHALL be exact by default.

- **FR-19:** Question config MAY allow:
  - `ignoreOrder` — sort arrays before comparison
  - `floatTolerance` — absolute tolerance for float comparison
  - `ignoreWhitespace` — trim strings
  - `caseInsensitive` — string comparison

- **FR-20:** Comparison rules SHALL be in
  `evaluation_config.comparison`.

- **FR-21:** JSON comparison SHALL be structural — key order doesn't
  matter.

- **FR-22:** Number comparison SHALL handle integers vs. floats
  (e.g., `9` and `9.0` are equal).

### 4.6 Expected Exceptions

- **FR-23:** A test case MAY expect an exception:
  ```json
  { "expectedOutput": { "exception": "ArgumentNullException" } }