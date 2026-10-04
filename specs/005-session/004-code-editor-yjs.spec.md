# Spec: Code Editor (Yjs)

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

The interview's code editor is a **collaborative, real-time** editing
surface where candidate and interviewer see the same code. The editor
uses **Monaco** (the engine behind VS Code) for UI and **Yjs** (a CRDT
library) for synchronization.

Yjs guarantees conflict-free merging of concurrent edits without a
central lock. Each participant's cursor, selection, and awareness are
visible to others.

This spec defines the Yjs document structure, Monaco integration,
awareness protocol, persistence, language switching, and snapshot
management.

---

## 2. Scope

### In Scope

- Monaco editor UI
- Yjs document sync via y-websocket
- Text synchronization (CRDT)
- Awareness (cursors, selections, presence)
- Language mode (syntax highlighting)
- Multi-file support (v1: one file per question)
- Snapshot creation and restore
- Persistence to PostgreSQL
- Undo/redo (Yjs UndoManager)

### Out of Scope

- Editor control (Spec 005)
- Test execution (Phase 4)
- Chat (Spec 006)
- Question push (Spec 007)

---

## 3. User Stories

- **US-1:** As a **candidate**, I want to write code, so that I can
  solve the question.

- **US-2:** As an **interviewer**, I want to see the candidate's code
  in real time, so that I can help.

- **US-3:** As an **interviewer**, I want to highlight lines and leave
  inline comments, so that I can guide the candidate.

- **US-4:** As a **user**, I want to see where the other person's
  cursor is, so that I don't type over them.

- **US-5:** As a **user** who lost connection, I want my edits to
  survive, so that I don't lose work.

---

## 4. Functional Requirements

### 4.1 Document Model

- **FR-1:** Each session SHALL have ONE Yjs document per question
  pushed in the session.

- **FR-2:** Document name SHALL be `session:{sessionId}:question:{questionId}`.

- **FR-3:** The Yjs document SHALL contain:
  - `code` — Y.Text (the source code)
  - `language` — Y.Map (current language)
  - `meta` — Y.Map (questionId, difficulty, title)
  - `snapshots` — Y.Array (named snapshots)

- **FR-4:** Only one file per question — multi-file deferred.

- **FR-5:** Initial content SHALL be the question's starter code for
  the chosen language.

### 4.2 Synchronization

- **FR-6:** Yjs sync SHALL use **y-websocket** protocol over WSS.

- **FR-7:** The y-websocket server SHALL be **Hocuspocus** (self-hosted)
  or equivalent.

- **FR-8:** Server SHALL authenticate connections via signed JWT
  (same token as session).

- **FR-9:** Server SHALL authorize participants only for their own
  session's document.

- **FR-10:** Sync SHALL be sub-100ms for typical edits.

- **FR-11:** Server SHALL persist documents to PostgreSQL every 30
  seconds OR every 100 updates (whichever first).

- **FR-12:** Server SHALL persist on session end (final snapshot).

- **FR-13:** Server SHALL retain documents for 30 days after session
  end.

### 4.3 Monaco Integration

- **FR-14:** Editor SHALL use **Monaco Editor** (v0.45+).

- **FR-15:** Yjs SHALL bind to Monaco via **y-monaco** provider.

- **FR-16:** Editor SHALL support:
  - Syntax highlighting per language
  - Line numbers
  - Auto-indent
  - Bracket matching
  - Find/replace
  - Multi-cursor
  - Undo/redo

- **FR-17:** Editor SHALL be read-only when session is Ended.

- **FR-18:** Editor SHALL be read-only for shadow interviewers.

- **FR-19:** Editor SHALL be editable by candidate in default mode.

- **FR-20:** Editor SHALL be editable by interviewer only in
  Takeover mode (Spec 005).

### 4.4 Awareness Protocol

- **FR-21:** Every participant SHALL publish awareness state:
  - `userId`
  - `fullName`
  - `role`
  - `cursor` (line, column)
  - `selection` (range)
  - `color` (assigned per participant)

- **FR-22:** Cursors of other participants SHALL be rendered in
  Monaco with the participant's color.

- **FR-23:** Cursor labels SHALL show the participant's name on hover.

- **FR-24:** Participant color SHALL be deterministic
  (hash of userId → palette).

- **FR-25:** Awareness state SHALL be removed when participant
  disconnects.

### 4.5 Language Switching

- **FR-26:** Interviewer SHALL switch the language via
  `POST /api/v1/sessions/{id}/questions/{qid}/language`.

- **FR-27:** Language switch SHALL update:
  - Yjs `language` field
  - Monaco syntax highlighting
  - Starter code (if the code is unchanged from the previous starter)

- **FR-28:** Language switch SHALL NOT erase candidate's code — it
  prompts the interviewer to confirm.

- **FR-29:** Supported languages SHALL match the question's languages.

### 4.6 Snapshots

- **FR-30:** Any participant MAY create a named snapshot via
  `POST /api/v1/sessions/{id}/questions/{qid}/snapshots`.

- **FR-31:** Snapshot SHALL include:
  - Full code content
  - Language
  - Created by
  - Created at
  - Label (optional)

- **FR-32:** Snapshots SHALL be persisted to PostgreSQL.

- **FR-33:** Interviewer SHALL restore a snapshot via
  `POST /api/v1/sessions/{id}/questions/{qid}/snapshots/{snapshotId}/restore`.

- **FR-34:** Restore SHALL replace the current Yjs text with the
  snapshot's content.

- **FR-35:** Restore SHALL be undoable (kept in Yjs UndoManager).

### 4.7 Undo/Redo

- **FR-36:** Undo/Redo SHALL be handled by Yjs UndoManager, scoped
  per participant.

- **FR-37:** Each participant SHALL only undo their own changes
  by default.

- **FR-38:** Full document undo/redo SHALL be available to the
  interviewer (rare).

### 4.8 Read-Only Modes

- **FR-39:** Editor modes SHALL include:
  - `CandidateOnly` — candidate editable, interviewer read-only
  - `Collaborative` — both editable
  - `InterviewerTakeover` — interviewer editable, candidate read-only
  - `ReadOnly` — both read-only (session ended)

- **FR-40:** Read-only state SHALL be enforced client-side AND
  server-side (reject edits via Yjs updates from unauthorized
  participants).

### 4.9 Persistence

- **FR-41:** Yjs document binary SHALL be stored as `BYTEA` in
  PostgreSQL.

- **FR-42:** On server restart, documents SHALL be restored from
  PostgreSQL to Yjs server memory on demand.

- **FR-43:** Final snapshot SHALL be persisted on session end.

- **FR-44:** Snapshots SHALL be retained for 90 days after session end.

### 4.10 Performance

- **FR-45:** Edits SHALL propagate to all participants within 100ms.

- **FR-46:** Yjs document size SHALL stay under 1 MB for typical
  sessions (5000 lines of code).

- **FR-47:** Initial document load SHALL complete in under 500ms.

- **FR-48:** Snapshots SHALL be stored compressed (gzip).

---

## 5. Non-Functional Requirements

- **NFR-1:** Sync latency SHALL be under 100ms (p95) on good network.

- **NFR-2:** Yjs server SHALL support 10,000 concurrent documents.

- **NFR-3:** Persistence writes SHALL NOT block sync (async).

- **NFR-4:** Documents SHALL survive server restart without data loss
  (last 30 seconds of edits may be lost).

- **NFR-5:** Memory usage per document SHALL be under 5 MB.

- **NFR-6:** Monaco editor SHALL render in under 1 second on modern
  browsers.

---

## 6. Domain Model

### 6.1 Value Objects

```csharp
public sealed record EditorSnapshot(
    Guid Id,
    Guid SessionId,
    Guid QuestionId,
    string Content,
    string Language,
    Guid CreatedBy,
    DateTime CreatedAt,
    string? Label);

public sealed record EditorAwareness(
    Guid UserId,
    string FullName,
    string Role,
    Position Cursor,
    Range Selection,
    string Color);

public sealed record Position(int Line, int Column);
public sealed record Range(Position Start, Position End);