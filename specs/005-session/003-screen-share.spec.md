
---

# 📄 Spec 005-session/003 — Screen Share

Save as: `specs\005-session\003-screen-share.spec.md`

---

```markdown
# Spec: Screen Share

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

During interviews, participants often share their screen:
- **Interviewers** — to show a diagram, Jira ticket, or system design
- **Candidates** — to show local IDE, design docs, or browser

Screen sharing uses the browser's `getDisplayMedia()` API and is
implemented as a LiveKit video track on a separate publication. The
session layout auto-switches when screen share starts.

This spec defines screen share initiation, signaling, layout,
concurrency rules, and recording interaction.

---

## 2. Scope

### In Scope

- Screen share initiation (interviewer, candidate)
- Signaling via SignalR
- LiveKit track publication
- Layout switching
- Concurrent sharing rules (one at a time)
- Screen share consent
- Screen share audit
- Recording integration
- Watermarking

### Out of Scope

- Full screen vs window vs tab distinction (browser-level)
- Audio sharing (system audio) — optional
- Multi-screen share

---

## 3. User Stories

- **US-1:** As an **interviewer**, I want to share my screen, so that
  I can show diagrams or tickets.

- **US-2:** As a **candidate**, I want to share my screen, so that I
  can show my IDE or design docs.

- **US-3:** As an **interviewer**, I want the layout to switch to
  screen-first, so that the shared content is prominent.

- **US-4:** As a **client**, I want screen sharing to be audited, so
  that I know when it happened.

- **US-5:** As a **security officer**, I want consent before screen
  share is recorded.

---

## 4. Functional Requirements

### 4.1 Initiation

- **FR-1:** Interviewer or candidate SHALL initiate screen share via
  a UI button.

- **FR-2:** Shadow interviewers SHALL NOT initiate screen share.

- **FR-3:** Initiation SHALL request consent via the browser's
  native picker.

- **FR-4:** Client SHALL call `navigator.mediaDevices.getDisplayMedia()`
  to obtain the stream.

- **FR-5:** On success, the client SHALL publish the track to LiveKit
  with `source: ScreenShare`.

- **FR-6:** On cancel, no track is published.

### 4.2 Signaling

- **FR-7:** Start SHALL signal via SignalR:
  `screen_share.started` with `{ sessionId, participantId, streamId }`.

- **FR-8:** Stop SHALL signal via SignalR:
  `screen_share.stopped` with `{ sessionId, participantId }`.

- **FR-9:** SignalR SHALL broadcast to `session:{id}` group.

- **FR-10:** Clients SHALL update layout on receiving the signal.

- **FR-11:** If no signal is received (crash), LiveKit track
  subscription events SHALL trigger layout update within 5s.

### 4.3 LiveKit Track

- **FR-12:** Screen share SHALL be published as a LiveKit video
  track with `source: ScreenShare`.

- **FR-13:** Screen share SHALL publish at native resolution with
  simulcast disabled (single layer).

- **FR-14:** Target bitrate SHALL be 2 Mbps.

- **FR-15:** Frame rate SHALL target 15 fps (screen content doesn't
  need 30).

- **FR-16:** System audio MAY be included if the browser provides it.

### 4.4 Concurrency

- **FR-17:** Only ONE participant SHALL share screen at a time.

- **FR-18:** If participant B starts while A is sharing:
  - Request SHALL be rejected with 409 (client shows message)
  - OR auto-stop A's share (configurable — default: reject)

- **FR-19:** Auto-stop MAY be enabled by the interviewer overriding.

- **FR-20:** Interviewer SHALL be able to stop the candidate's screen
  share (moderation).

### 4.5 Layout Switching

- **FR-21:** When screen share starts, the layout SHALL switch to
  screen-first:
  - Shared content: large
  - Cameras: small thumbnails

- **FR-22:** When screen share stops, layout SHALL revert to
  camera-first.

- **FR-23:** Layout preferences SHALL be stored per user per
  session (not persisted).

- **FR-24:** Candidate SHALL see the same layout as interviewer
  (synchronized).

### 4.6 Consent

- **FR-25:** Before recording starts, screen share participants
  SHALL consent to recording via an in-app dialog.

- **FR-26:** Consent SHALL be logged with:
  - Session ID
  - Participant ID
  - Consent timestamp
  - IP address
  - User agent

- **FR-27:** If consent is declined, screen share SHALL be blocked
  from recording (video only).

- **FR-28:** Consent SHALL be captured once per session per
  participant.

### 4.7 Watermarking

- **FR-29:** Screen share SHALL be watermarked with:
  - Candidate's name (for candidate shares)
  - Timestamp
  - Session ID

- **FR-30:** Watermark SHALL be applied client-side (CSS overlay)
  AND server-side (during recording composite).

- **FR-31:** Watermark SHALL be semi-transparent, top-right corner.

### 4.8 Audit

- **FR-32:** Every screen share start/stop SHALL be recorded:
  - Session ID
  - Participant ID
  - Started at
  - Ended at
  - Duration
  - Share type (screen / window / tab — if browser reports it)

- **FR-33:** Audit records SHALL be retained for 1 year.

- **FR-34:** Audit SHALL be viewable in the admin panel.

### 4.9 Recording Integration

- **FR-35:** Screen share SHALL be included in the composite
  recording (Spec 008).

- **FR-36:** Screen share layout SHALL be screen-first in the
  recording when active.

- **FR-37:** Recording SHALL switch layout when screen share
  starts/stops — with a 500ms transition.

### 4.10 Failure Handling

- **FR-38:** If screen share track fails to publish, UI SHALL show
  an error and log the event.

- **FR-39:** If the browser stops share (user clicks "Stop sharing"
  in browser UI), the client SHALL detect `track.onended` and
  clean up.

- **FR-40:** Network failure during screen share SHALL drop the
  track silently and notify the participant.

### 4.11 Performance

- **FR-41:** Screen share SHALL be capped at 1080p max.

- **FR-42:** Screen share SHALL fall back to lower resolution on
  poor networks (adaptive).

- **FR-43:** Screen share SHALL NOT block camera publishing (both
  simultaneously).

---

## 5. Non-Functional Requirements

- **NFR-1:** Screen share start SHALL complete in under 2 seconds
  (from click to visible).

- **NFR-2:** Screen share latency SHALL be under 200ms.

- **NFR-3:** Screen share stop SHALL propagate to all participants
  in under 500ms.

- **NFR-4:** Layout switch SHALL be smooth (no black frames).

- **NFR-5:** Screen share SHALL consume ≤ 2 Mbps uplink.

---

## 6. Domain Model

### 6.1 Value Objects

```csharp
public sealed record ScreenShareState(
    Guid SessionId,
    Guid ParticipantId,
    string StreamId,
    DateTime StartedAt,
    DateTime? EndedAt,
    string ShareType);