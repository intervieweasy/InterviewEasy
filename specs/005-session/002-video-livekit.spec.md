
---

# 📄 Spec 005-session/002 — Video (LiveKit)

Save as: `specs\005-session\002-video-livekit.spec.md`

---

```markdown
# Spec: Video (LiveKit)

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

InterviewEasy's live interviews require real-time audio and video
between interviewer, candidate, and shadow observers. We use **LiveKit**
as the SFU (Selective Forwarding Unit) — an open-source WebRTC media
server that handles track forwarding, simulcast, and recording.

LiveKit handles the hard parts of WebRTC:
- Signaling
- ICE / STUN / TURN
- Simulcast (multiple quality levels)
- Adaptive bitrate
- Track subscription control

This spec defines how InterviewEasy integrates LiveKit: room creation,
token issuance, participant roles, track policies, and network
fallback.

---

## 2. Scope

### In Scope

- LiveKit room creation and lifecycle
- Access token issuance per participant
- Participant roles (interviewer, candidate, shadow)
- Track publication (camera, mic, screen share)
- Track subscription policies
- Quality settings and simulcast
- Network fallback (TURN)
- Audio/video device switching
- Bandwidth and CPU adaptation

### Out of Scope

- Screen share signaling details (Spec 003)
- Recording (Spec 008)
- SignalR events (Spec 009)

---

## 3. User Stories

- **US-1:** As a **candidate**, I want to see the interviewer's video
  and hear their audio clearly.

- **US-2:** As an **interviewer**, I want to toggle my camera and mic,
  so that I control my presence.

- **US-3:** As a **shadow interviewer**, I want to observe without
  publishing my camera, so that I don't distract.

- **US-4:** As a **user** on a slow network, I want the video to
  degrade gracefully, so that audio stays clear.

- **US-5:** As an **SRE**, I want to monitor LiveKit health, so that
  I can react to outages.

---

## 4. Functional Requirements

### 4.1 LiveKit Room

- **FR-1:** Each session SHALL have one LiveKit room.

- **FR-2:** Room name SHALL be `session_{sessionId}`.

- **FR-3:** Room SHALL be created **lazily** on the first join.

- **FR-4:** Room SHALL be deleted when session ends and recording
  is finalized.

- **FR-5:** Room SHALL have these settings:
  - `empty_timeout`: 300 seconds (5 min)
  - `departure_timeout`: 60 seconds
  - `max_participants`: 6
  - `audio_preset`: voice-optimized
  - `video_preset`: 720p default

### 4.2 Access Tokens

- **FR-6:** Every participant SHALL receive a LiveKit JWT on join.

- **FR-7:** The JWT SHALL include:
  - `sub` — participant identity (`{role}_{userId}`)
  - `iss` — LiveKit API key
  - `exp` — expiry (session end + 1 hour)
  - `video.roomJoin` — true
  - `video.room` — `session_{sessionId}`
  - `video.canPublish` — true (interviewer, candidate); false (shadow)
  - `video.canSubscribe` — true
  - `video.canPublishData` — true
  - `metadata` — JSON with role, name

- **FR-8:** Token expiry SHALL be extended by refreshing before
  session end (client refreshes every 30 min).

- **FR-9:** Token refresh endpoint SHALL be provided
  (`/sessions/{id}/refresh-token`).

- **FR-10:** Revoked tokens SHALL be rejected by LiveKit.

### 4.3 Participant Roles

- **FR-11:** Three roles SHALL be supported:

  | Role | Publish | Subscribe | Screen Share | Data |
  |------|---------|-----------|--------------|------|
  | Interviewer | Camera + Mic | All | Yes | Yes |
  | Candidate | Camera + Mic | All | Yes | Yes |
  | Shadow | None | All | No | No |

- **FR-12:** Shadow interviewers SHALL NOT publish any tracks.

- **FR-13:** Shadow interviewers SHALL receive all tracks from
  interviewer and candidate.

- **FR-14:** Role SHALL be encoded in participant metadata.

### 4.4 Track Publication

- **FR-15:** Interviewer and candidate SHALL publish:
  - Microphone track (audio)
  - Camera track (video, simulcast)

- **FR-16:** Simulcast SHALL publish three layers:
  - Low: 320x180 @ 15fps, 150 kbps
  - Medium: 640x360 @ 24fps, 500 kbps
  - High: 1280x720 @ 30fps, 1500 kbps

- **FR-17:** Publications SHALL include metadata:
  - `participantRole`
  - `fullName`
  - `userId` or `candidateId`

- **FR-18:** Participants SHALL be able to mute/unmute mic
  independently.

- **FR-19:** Participants SHALL be able to toggle camera independently.

- **FR-20:** Muted state SHALL be visible to all participants.

### 4.5 Track Subscription

- **FR-21:** LiveKit SHALL auto-subscribe participants to all
  available tracks by default.

- **FR-22:** Candidates SHALL NOT subscribe to shadow interviewers'
  tracks (shadows publish none).

- **FR-23:** Subscription quality SHALL adapt based on bandwidth:
  - < 500 kbps: low layer
  - 500-1500 kbps: medium layer
  - > 1500 kbps: high layer

- **FR-24:** Audio SHALL always be subscribed (never degraded).

### 4.6 Network Fallback

- **FR-25:** LiveKit SHALL provide TURN servers for restricted
  networks.

- **FR-26:** ICE servers SHALL include:
  - LiveKit's built-in STUN
  - Configured TURN (for strict firewalls)

- **FR-27:** Connection failures SHALL trigger a UI prompt
  ("Check your network") within 10 seconds.

- **FR-28:** Fallback to audio-only SHALL be automatic when video
  cannot be sustained.

### 4.7 Device Management

- **FR-29:** Users SHALL be able to select camera, mic, and speaker
  from the pre-join check screen.

- **FR-30:** Device selection SHALL persist per session (not across
  sessions initially).

- **FR-31:** Device switching mid-session SHALL NOT interrupt audio.

- **FR-32:** Device hot-swap (e.g., unplugging headphones) SHALL
  trigger re-selection prompt.

### 4.8 Quality Adaptation

- **FR-33:** LiveKit's adaptive bitrate SHALL adjust published
  quality based on receiver feedback.

- **FR-34:** Publishers SHALL be notified when their connection is
  poor (UI indicator).

- **FR-35:** If uplink < 300 kbps, video SHALL drop to low layer
  automatically.

- **FR-36:** CPU-bound devices SHALL reduce resolution rather than
  drop frames.

### 4.9 Data Channels

- **FR-37:** A LiveKit data channel SHALL be used for ephemeral
  signals (reactions, typing indicators).

- **FR-38:** Chat messages SHALL NOT use the LiveKit data channel —
  they use SignalR (Spec 006).

- **FR-39:** Data channel messages SHALL be reliable (ordered).

### 4.10 Recording Integration

- **FR-40:** LiveKit Egress SHALL be started when session becomes
  `Active`.

- **FR-41:** Egress SHALL record:
  - All participants' camera + audio
  - Screen share (if active)
  - Composite layout (Spec 008)

- **FR-42:** Egress SHALL stop when session ends.

### 4.11 Health & Monitoring

- **FR-43:** LiveKit server health SHALL be monitored:
  - CPU, memory, bandwidth
  - Room count
  - Participant count
  - Egress count

- **FR-44:** LiveKit server SHALL run in HA mode (2+ nodes)
  in production.

- **FR-45:** If LiveKit is unavailable, join SHALL return 503 with
  a clear error.

---

## 5. Non-Functional Requirements

- **NFR-1:** Video latency SHALL be under 150ms (glass-to-glass).

- **NFR-2:** Audio latency SHALL be under 100ms.

- **NFR-3:** Audio SHALL remain clear even at 200 kbps uplink.

- **NFR-4:** LiveKit SHALL support 10,000 concurrent participants
  per cluster.

- **NFR-5:** Room creation SHALL complete in under 200ms.

- **NFR-6:** Token issuance SHALL complete in under 50ms.

- **NFR-7:** Join to first video frame SHALL complete in under 3
  seconds.

- **NFR-8:** Reconnection SHALL restore media in under 2 seconds.

---

## 6. Domain Model

### 6.1 Value Objects

```csharp
public sealed record LiveKitToken(
    string Token,
    DateTime ExpiresAt,
    string RoomName,
    string Identity);

public sealed record LiveKitRoomConfig(
    string Name,
    int EmptyTimeoutSeconds,
    int MaxParticipants,
    string AudioPreset,
    string VideoPreset);

public sealed record ParticipantMetadata(
    string Role,
    string FullName,
    Guid? UserId,
    Guid? CandidateId);