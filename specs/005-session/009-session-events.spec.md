
---

# 📄 Spec 005-session/009 — Session Events

Save as: `specs\005-session\009-session-events.spec.md`

---

```markdown
# Spec: Session Events (SignalR)

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Every real-time action in a session — participant joins, question
pushed, mode changes, screen share starts, chat messages, test
results — is broadcast to all participants via **SignalR**.

SignalR is the **control plane** of the session, distinct from LiveKit
(media plane) and Yjs (editor plane). It handles low-frequency,
high-reliability events with ordering guarantees.

This spec defines the SignalR hub, event contracts, groups, ordering,
delivery guarantees, reconnection, and backpressure.

---

## 2. Scope

### In Scope

- SignalR hub architecture
- Session group membership
- Event contracts (all session events)
- Event ordering guarantees
- Delivery guarantees (at-least-once)
- Reconnection and missed event recovery
- Backpressure and rate limiting
- Authentication and authorization
- Multi-region SignalR

### Out of Scope

- Chat messages (Spec 006)
- Editor sync (Spec 004)
- Video tracks (Spec 002)
- Screen share signaling details (Spec 003)

---

## 3. User Stories

- **US-1:** As a **participant**, I want session events to reach me in
  order, so that UI doesn't flicker.

- **US-2:** As a **participant** who lost connection, I want missed
  events replayed, so that my UI catches up.

- **US-3:** As an **SRE**, I want SignalR metrics, so that I can react
  to connection issues.

- **US-4:** As a **developer**, I want a clear event contract, so
  that adding a new event doesn't break clients.

- **US-5:** As a **security officer**, I want SignalR auth verified,
  so that only session participants receive events.

---

## 4. Functional Requirements

### 4.1 Hub Architecture

- **FR-1:** The Session service SHALL expose a SignalR hub at
  `/hubs/session`.

- **FR-2:** The hub SHALL use **Azure SignalR Service** in production
  (no sticky sessions).

- **FR-3:** In development, the hub SHALL use in-process SignalR.

- **FR-4:** The hub SHALL be authenticated via JWT (same as API).

- **FR-5:** The hub SHALL be authorized per session (user must be a
  participant).

- **FR-6:** The hub SHALL support up to 6 participants per session
  (matching session limit).

### 4.2 Groups

- **FR-7:** Each session SHALL have a SignalR group named
  `session:{sessionId}`.

- **FR-8:** Every participant SHALL join the group on connect.

- **FR-9:** Participants SHALL leave the group on disconnect.

- **FR-10:** Some events SHALL be sent to subsets:
  - `session:{sessionId}:interviewers` — interviewers + shadows only
  - `session:{sessionId}:candidate` — candidate only

- **FR-11:** Sensitive events (hidden test results, solution reveal)
  SHALL be sent to interviewer group only.

### 4.3 Event Contracts

- **FR-12:** Events SHALL follow a standard envelope:
  ```json
  {
    "eventId": "guid",
    "eventType": "string",
    "sessionId": "guid",
    "occurredAt": "ISO8601",
    "actorId": "guid",
    "payload": { ... }
  }