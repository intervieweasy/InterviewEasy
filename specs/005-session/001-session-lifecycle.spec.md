# Spec: Session Lifecycle

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

A **Session** (also called an *interview session*) is the live,
real-time interaction between an interviewer and a candidate. It is
where the interview actually happens — video, audio, code editor,
questions, screen share, chat, and code execution all converge in
one session.

Sessions are ephemeral: they start when both parties join, end when
the interviewer closes them, and produce a recorded artifact. The
Session service is the **orchestrator** of the five real-time
channels (video, screen, editor, chat, execution).

This spec defines the session lifecycle: creation, joining, active
state, ending, and cleanup.

---

## 2. Scope

### In Scope

- Session entity and lifecycle states
- Session creation (triggered by scheduling → interview)
- Join flow (interviewer + candidate)
- Reconnect handling
- Session end (normal, forced, abandoned)
- Session state management
- Timeout and abandonment rules
- Session events

### Out of Scope

- Video/SignalR channels (Specs 002-006)
- Recording pipeline (Spec 008)
- Individual channel mechanics

---

## 3. User Stories

- **US-1:** As an **interviewer**, I want to start a session at the
  scheduled time, so that the interview begins.

- **US-2:** As a **candidate**, I want to join the session via a
  secure link, so that I can be interviewed.

- **US-3:** As an **interviewer**, I want to end the session, so that
  the recording is finalized and feedback can be submitted.

- **US-4:** As a **user** who lost connection, I want to reconnect to
  the same session, so that I don't lose work.

- **US-5:** As an **SRE**, I want abandoned sessions cleaned up, so
  that resources aren't wasted.

---

## 4. Functional Requirements

### 4.1 Session Entity

- **FR-1:** A `Session` entity SHALL exist with:
  - `Id` (Guid, PK)
  - `InterviewId` (Guid, FK to Scheduled Interview)
  - `TenantId` (Guid)
  - `RequirementId` (Guid)
  - `InterviewerId` (Guid)
  - `CandidateId` (Guid) — a lightweight candidate record
  - `Status` (enum — see §6.2)
  - `ScheduledStartAt` (DateTime)
  - `ActualStartAt` (DateTime?)
  - `EndedAt` (DateTime?)
  - `EndedBy` (Guid?)
  - `EndReason` (enum: Normal, ForceClosed, Abandoned, Timeout)
  - `DurationSeconds` (int?)
  - `RecordingId` (Guid?)
  - `SignalRConnectionMap` (JSONB — participant → connection IDs)
  - `LiveKitRoomName` (string)
  - `Metadata` (JSONB)
  - `CreatedAt`, `UpdatedAt`

- **FR-2:** `LiveKitRoomName` SHALL be generated deterministically
  as `session_{sessionId}`.

### 4.2 Session States

- **FR-3:** The session lifecycle SHALL have these states:
  - `Scheduled` — created, not yet started
  - `WaitingForParticipants` — one party joined, waiting for the other
  - `Active` — both parties joined
  - `Ending` — end initiated, waiting for cleanup
  - `Ended` — completed and cleaned up
  - `Abandoned` — never started within the timeout window
  - `Cancelled` — cancelled before start

- **FR-4:** Transitions:
  - Scheduled → WaitingForParticipants (first participant joins)
  - WaitingForParticipants → Active (second participant joins)
  - Active → Ending (either participant triggers end, or timeout)
  - Ending → Ended (cleanup complete)
  - Scheduled → Abandoned (no one joined within 15 min of scheduled time)
  - Scheduled/WaitingForParticipants → Cancelled (interviewer cancels)

### 4.3 Session Creation

- **FR-5:** A session SHALL be created automatically when an interview
  is scheduled (Phase 5, but triggered by Scheduling).

- **FR-6:** The session SHALL be created with `Status = Scheduled`.

- **FR-7:** LiveKit room SHALL be created lazily on first join.

- **FR-8:** Session ID SHALL be returned in the interview scheduling
  response.

### 4.4 Join Flow

- **FR-9:** Interviewer SHALL join via
  `POST /api/v1/sessions/{id}/join` with JWT auth.

- **FR-10:** Candidate SHALL join via a secure signed link:
  `/sessions/join?token={signed-token}`.

- **FR-11:** The signed token SHALL contain:
  - Session ID
  - Candidate ID
  - Expiry (24 hours after scheduled end)
  - HMAC signature

- **FR-12:** First join SHALL transition `Scheduled → WaitingForParticipants`.

- **FR-13:** Second join SHALL transition
  `WaitingForParticipants → Active`.

- **FR-14:** Join SHALL return:
  - LiveKit access token (per participant)
  - Session state
  - Participant list
  - Channel tokens (SignalR groups)

- **FR-15:** Duplicate join by same user SHALL be idempotent (return
  existing tokens, unless reconnection).

- **FR-16:** Join SHALL be rejected if:
  - Session is `Ended`, `Cancelled`, or `Abandoned`
  - Current time > scheduled end + 30 min (grace period)
  - Candidate's user is suspended (interviewer side only)

### 4.5 Active Session

- **FR-17:** While `Active`, the session SHALL support:
  - Video/audio (Spec 002)
  - Screen share (Spec 003)
  - Collaborative editor (Spec 004)
  - Editor control changes (Spec 005)
  - Chat (Spec 006)
  - Question push (Spec 007)
  - Recording (Spec 008)

- **FR-18:** Active session SHALL be monitored for:
  - Inactivity (no video/audio/editor events for 10 min)
  - Participant disconnection (all disconnected for 5 min)

- **FR-19:** Long sessions (> scheduled duration + 30 min) SHALL
  trigger a warning to the interviewer.

### 4.6 Reconnection

- **FR-20:** A participant reconnecting SHALL reuse their existing
  participant ID.

- **FR-21:** Reconnection SHALL restore:
  - LiveKit token
  - SignalR group memberships
  - Editor state (via Yjs)
  - Chat history (last 100 messages)
  - Current question

- **FR-22:** Reconnection within 5 min SHALL NOT change session
  state.

- **FR-23:** Reconnection after 5 min of full disconnect SHALL
  transition session to `Ending` if all participants are gone.

### 4.7 Ending a Session

- **FR-24:** Either party SHALL end the session via
  `POST /api/v1/sessions/{id}/end`.

- **FR-25:** Ending SHALL:
  1. Transition to `Ending`
  2. Stop recording
  3. Freeze editor state (Yjs snapshot)
  4. Notify all participants via SignalR
  5. Wait for final code submissions to complete
  6. Transition to `Ended`
  7. Publish `SessionEndedEvent`

- **FR-26:** Cleanup SHALL complete within 30 seconds.

- **FR-27:** Ended session SHALL be read-only.

- **FR-28:** Only the interviewer SHALL be able to end normally;
  candidate ending SHALL notify the interviewer but not close.

### 4.8 Abandonment

- **FR-29:** Session SHALL be marked `Abandoned` if:
  - No participant joined within 15 min of scheduled start
  - OR all participants disconnected for > 30 min during `Active`

- **FR-30:** Abandoned sessions SHALL publish
  `SessionAbandonedEvent` for analytics.

- **FR-31:** A background job SHALL run every 5 minutes to detect
  abandoned sessions.

### 4.9 Session State (Live)

- **FR-32:** Session state SHALL be cached in Redis:
  - Key: `session:{id}:state`
  - TTL: 24 hours after end

- **FR-33:** State SHALL include:
  - Status
  - Participants (interviewer, candidate, shadow)
  - Current question ID
  - Editor mode
  - Recording status
  - Start time

- **FR-34:** State changes SHALL publish to SignalR group
  `session:{id}`.

### 4.10 Session Events (SignalR)

- **FR-35:** Participants SHALL subscribe to SignalR group
  `session:{id}` on join.

- **FR-36:** SignalR events SHALL include:
  - `session.state.changed`
  - `session.participant.joined`
  - `session.participant.left`
  - `session.ended`
  - `session.endedByOther`
  - `session.reconnecting`

- **FR-37:** Missed events SHALL be replayable via
  `GET /api/v1/sessions/{id}/events?since={timestamp}`.

### 4.11 Multi-Party Support

- **FR-38:** Sessions SHALL support additional participants:
  - Shadow interviewers (observers)
  - Panel members joining mid-session

- **FR-39:** Shadow interviewers SHALL join via the panel API and
  have read-only access to all channels.

- **FR-40:** Maximum participants per session SHALL be 6.

### 4.12 Cancellation

- **FR-41:** Interviewer SHALL cancel a `Scheduled` session via
  `POST /api/v1/sessions/{id}/cancel`.

- **FR-42:** Cancellation SHALL publish `SessionCancelledEvent`.

- **FR-43:** Cancellation SHALL notify the candidate via email
  (Phase 8).

---

## 5. Non-Functional Requirements

- **NFR-1:** Session creation SHALL complete in under 200ms.

- **NFR-2:** Join SHALL complete in under 500ms (including LiveKit
  token issuance).

- **NFR-3:** State changes SHALL propagate to participants within
  500ms.

- **NFR-4:** Reconnection SHALL restore all state within 3 seconds.

- **NFR-5:** Session state SHALL survive API restarts (Redis-backed).

- **NFR-6:** The system SHALL support 10,000 concurrent active
  sessions.

- **NFR-7:** The system SHALL support 1,000 new sessions per minute.

---

## 6. Domain Model

### 6.1 Entity: Session

```csharp
public sealed class Session : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid InterviewId { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid RequirementId { get; private set; }
    public Guid InterviewerId { get; private set; }
    public Guid CandidateId { get; private set; }
    public SessionStatus Status { get; private set; }
    public DateTime ScheduledStartAt { get; private set; }
    public DateTime? ActualStartAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public Guid? EndedBy { get; private set; }
    public SessionEndReason? EndReason { get; private set; }
    public int? DurationSeconds { get; private set; }
    public Guid? RecordingId { get; private set; }
    public string LiveKitRoomName { get; private set; }
    public JsonDocument? SignalRConnectionMap { get; private set; }
    public JsonDocument? Metadata { get; private set; }

    public static Session Create(Guid interviewId, Guid tenantId,
        Guid requirementId, Guid interviewerId, Guid candidateId,
        DateTime scheduledStartAt);

    public void ParticipantJoined(Guid participantId);
    public void ParticipantLeft(Guid participantId);
    public void End(Guid endedBy, SessionEndReason reason);
    public void Abandon();
    public void Cancel(Guid cancelledBy);
}