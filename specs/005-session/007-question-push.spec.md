
---

# 📄 Spec 005-session/007 — Question Push

Save as: `specs\005-session\007-question-push.spec.md`

---

```markdown
# Spec: Question Push

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

During a live interview, the interviewer selects questions from the
requirement's question bank and **pushes** them to the candidate's
screen. The candidate then sees the problem statement, starter code,
and sample tests.

Question push is the mechanism that moves a question from the
authoring phase (Phase 3) into the live interview session (Phase 5).

This spec defines the push flow, question visibility (sample vs
hidden), transitions, and lifecycle within a session.

---

## 2. Scope

### In Scope

- Question push flow (interviewer → candidate)
- Question picker integration
- Sample vs hidden test case visibility
- Multi-question sessions (queue of questions)
- Current question state
- Question completion
- Question switching
- Starter code loading
- Audit

### Out of Scope

- Question CRUD (Phase 3)
- Question validation (Phase 3)
- Code execution (Phase 4)
- Editor control (Spec 005)

---

## 3. User Stories

- **US-1:** As an **interviewer**, I want to pick a question from the
  bank and push it, so that the candidate sees it.

- **US-2:** As a **candidate**, I want to see the problem statement
  and starter code immediately, so that I can start.

- **US-3:** As a **candidate**, I want to see sample test cases, so
  that I understand the expected behavior.

- **US-4:** As an **interviewer**, I want to mark a question as
  complete and push the next one.

- **US-5:** As a **client**, I want the list of questions asked in
  the session, so that I can review.

---

## 4. Functional Requirements

### 4.1 Question Picker

- **FR-1:** Interviewer SHALL see a question picker panel with:
  - Course filter
  - Language filter
  - Difficulty filter
  - Topics filter
  - Search box
  - Validation status filter

- **FR-2:** Only questions with `validation_status = passed` SHALL
  appear by default.

- **FR-3:** A toggle SHALL show unvalidated questions with a warning.

- **FR-4:** Only questions for the requirement's courses SHALL appear.

- **FR-5:** Question picker SHALL load in under 500ms.

### 4.2 Push Flow

- **FR-6:** Interviewer pushes a question via
  `POST /api/v1/sessions/{id}/questions/{questionId}/push`.

- **FR-7:** Push SHALL:
  1. Validate the question belongs to the requirement
  2. Validate the question is not already pushed in this session
  3. Load the question + sample test cases
  4. Initialize a new Yjs document for the question
  5. Set the question as current
  6. Publish `QuestionPushedEvent`
  7. Broadcast to candidate via SignalR

- **FR-8:** Push SHALL complete in under 500ms.

- **FR-9:** Only one question SHALL be "current" at a time.

- **FR-10:** Candidate SHALL see the question appear without page
  reload.

- **FR-11:** Candidate's editor SHALL load the starter code for the
  question's language.

### 4.3 Question Content

- **FR-12:** The pushed payload SHALL include:
  - Question ID
  - Title
  - Difficulty
  - Language
  - Problem statement (markdown + constraints + examples)
  - Function signature
  - Starter code
  - Sample test cases (visible to candidate)
  - Topics

- **FR-13:** Hidden test cases SHALL NOT be sent to the candidate.

- **FR-14:** Solution code SHALL NOT be sent to the candidate.

- **FR-15:** Interviewer SHALL see all test cases (including hidden).

### 4.4 Sample Test Cases

- **FR-16:** Sample test cases SHALL be visible to candidate.

- **FR-17:** Sample test cases SHALL be runnable by the candidate
  (via Sandbox).

- **FR-18:** Sample results SHALL be visible to both participants.

- **FR-19:** Sample test cases SHALL NOT contribute to the score.

### 4.5 Hidden Test Cases

- **FR-20:** Hidden test cases SHALL be visible to interviewer only.

- **FR-21:** Interviewer SHALL run hidden test cases against
  candidate's code (via Sandbox).

- **FR-22:** Results SHALL be visible to interviewer only initially.

- **FR-23:** Interviewer MAY share results with candidate (button).

### 4.6 Question Completion

- **FR-24:** Interviewer SHALL mark a question as completed via
  `POST /api/v1/sessions/{id}/questions/{questionId}/complete`.

- **FR-25:** Completion SHALL:
  - Mark question state as `completed`
  - Record duration
  - Persist final code snapshot
  - Publish `QuestionCompletedEvent`

- **FR-26:** After completion, the question's editor becomes
  read-only.

- **FR-27:** Interviewer MAY revisit a completed question (reopen)
  within the same session.

### 4.7 Question Queue

- **FR-28:** Sessions SHALL have a queue of questions:
  - `pending` — pushed but not started
  - `current` — currently active
  - `completed` — finished
  - `skipped` — pushed then removed

- **FR-29:** `GET /api/v1/sessions/{id}/questions` SHALL return
  the queue.

- **FR-30:** Interviewer SHALL reorder pending questions.

- **FR-31:** Interviewer SHALL remove pending questions.

- **FR-32:** Candidate SHALL see the queue count but not the titles
  of future questions.

### 4.8 Multi-Language Questions

- **FR-33:** Some questions support multiple languages
  (e.g., Python or JavaScript).

- **FR-34:** Interviewer SHALL pick the language at push time
  (default: question's primary language).

- **FR-35:** Language SHALL be changeable during the question
  (with editor reset confirmation).

### 4.9 Session Context

- **FR-36:** The current question SHALL be persisted in the session
  state.

- **FR-37:** On reconnection, the client SHALL restore the current
  question and editor state.

- **FR-38:** Multiple questions SHALL be pushed in a single session
  (up to 20).

- **FR-39:** Session duration SHALL adjust based on number of
  questions (warning after 90 min).

### 4.10 Audit

- **FR-40:** Every question push SHALL be logged:
  - Question ID
  - Pushed by
  - Pushed at
  - Language
  - Mode at push

- **FR-41:** Question completion SHALL be logged with:
  - Duration
  - Test results
  - Final score

- **FR-42:** Audit SHALL be retained for 1 year.

---

## 5. Non-Functional Requirements

- **NFR-1:** Push SHALL complete in under 500ms.

- **NFR-2:** Candidate SHALL see the question in under 1 second.

- **NFR-3:** Question picker SHALL load in under 500ms with 1000
  questions.

- **NFR-4:** The system SHALL support 20 questions per session.

- **NFR-5:** Push SHALL NOT block video or editor.

---

## 6. Domain Model

### 6.1 Entity: SessionQuestion

```csharp
public sealed class SessionQuestion
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid TenantId { get; private set; }
    public SessionQuestionState State { get; private set; }
    public string Language { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime? PushedAt { get; private set; }
    public Guid? PushedBy { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int? DurationSeconds { get; private set; }
    public double? Score { get; private set; }
    public Guid? FinalSnapshotId { get; private set; }
}