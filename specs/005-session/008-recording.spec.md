
---

# 📄 Spec 005-session/008 — Recording

Save as: `specs\005-session\008-recording.spec.md`

---

```markdown
# Spec: Session Recording

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Every session is recorded for compliance, review, and quality. The
recording includes composite video (interviewer + candidate + screen
share if active) plus audio. It is stored in object storage and
streamed via CDN.

Recording is handled by **LiveKit Egress** — LiveKit's built-in
recording service. Egress composites all tracks into a single MP4.

This spec defines recording start/stop, composition, storage, and
playback.

---

## 2. Scope

### In Scope

- Recording start (automatic on session Active)
- Recording stop (on session end)
- Composite layout selection
- Audio mixing
- Egress lifecycle
- Storage (S3/Blob) and CDN
- Playback (HLS)
- Watermarking
- Retention
- Failure handling

### Out of Scope

- LiveKit video track management (Spec 002)
- Screen share track (Spec 003)
- Recording UI in the app (frontend spec)

---

## 3. User Stories

- **US-1:** As a **client**, I want every interview recorded, so that
  I can review candidate performance later.

- **US-2:** As a **candidate**, I want to know I'm being recorded, so
  that I consent.

- **US-3:** As an **interviewer**, I want to jump to specific moments
  in the recording, so that I can reference them.

- **US-4:** As a **compliance officer**, I want recordings retained
  per policy, so that we meet regulatory requirements.

- **US-5:** As an **SRE**, I want to monitor recording failures, so
  that no interview is lost.

---

## 4. Functional Requirements

### 4.1 Recording Lifecycle

- **FR-1:** Recording SHALL start automatically when session
  transitions to `Active`.

- **FR-2:** Recording SHALL stop when session transitions to `Ending`.

- **FR-3:** Recording SHALL be finalized (composite ready) within
  2 minutes of session end.

- **FR-4:** Recording SHALL be optional — client config SHALL allow
  disabling recording.

- **FR-5:** Candidate SHALL consent to recording before joining.

- **FR-6:** Recording consent SHALL be stored with the session.

### 4.2 LiveKit Egress

- **FR-7:** LiveKit Egress SHALL be used for recording.

- **FR-8:** Egress SHALL use the **Composite** mode (single MP4).

- **FR-9:** Egress SHALL include:
  - Interviewer video
  - Candidate video
  - Audio (mixed)
  - Screen share (when active)

- **FR-10:** Egress SHALL output:
  - Format: MP4 (H.264 + AAC)
  - Resolution: 1080p
  - Bitrate: 4000 kbps video, 128 kbps audio
  - Frame rate: 30 fps

- **FR-11:** Egress SHALL support mid-session layout changes (screen
  share start/stop).

- **FR-12:** Egress SHALL be started via LiveKit API with the room
  name.

- **FR-13:** Egress SHALL write to a temporary storage location
  (S3 multipart upload).

### 4.3 Composite Layout

- **FR-14:** Default layout SHALL be **side-by-side**:
  - Left: Interviewer
  - Right: Candidate
  - Bottom: shared content (screen or editor, if active)

- **FR-15:** Layout SHALL switch to **screen-first** when screen
  share is active:
  - Center: Screen share
  - Bottom: interviewers as thumbnails

- **FR-16:** Layout transitions SHALL be smooth (500ms fade).

- **FR-17:** Watermark SHALL be applied:
  - Session ID (top-left)
  - Candidate name (top-right)
  - Timestamp (bottom-right)
  - "REC" indicator (top-right)

- **FR-18:** Layout SHALL be configurable per tenant (future).

### 4.4 Storage

- **FR-19:** Recordings SHALL be stored in S3 (or Azure Blob).

- **FR-20:** Storage path SHALL be:
  `recordings/{tenantId}/{yyyy}/{MM}/{sessionId}.mp4`

- **FR-21:** After upload, the MP4 SHALL be transcoded to HLS
  (adaptive bitrate) for streaming.

- **FR-22:** HLS segments SHALL be stored alongside the MP4.

- **FR-23:** CDN (CloudFront/Azure CDN) SHALL serve the HLS stream.

- **FR-24:** MP4 original SHALL be retained for download.

- **FR-25:** Recording files SHALL be encrypted at rest (S3 SSE-KMS).

- **FR-26:** Signed URLs SHALL be used for playback (expires in 1 hour).

- **FR-27:** Access SHALL be restricted to tenant users.

### 4.5 Playback

- **FR-28:** Recording SHALL be playable in the client's browser.

- **FR-29:** Playback SHALL use HLS.js (or native HLS on Safari).

- **FR-30:** Playback SHALL support:
  - Play/Pause
  - Seek
  - Playback speed (0.5x–2x)
  - Volume control
  - Full-screen
  - Download (MP4)

- **FR-31:** Recording duration SHALL match session duration.

- **FR-32:** Playback SHALL start within 3 seconds on broadband.

### 4.6 Chapter Markers

- **FR-33:** The recording SHALL include chapter markers for:
  - Session start
  - Question pushed
  - Code submission
  - Screen share start/stop
  - Solution revealed
  - Session end

- **FR-34:** Chapters SHALL be stored in a sidecar JSON file.

- **FR-35:** Playback UI SHALL show chapters as a timeline.

### 4.7 Retention

- **FR-36:** Retention policy SHALL be per tenant:
  - Default: 90 days
  - Enterprise: configurable (1–365 days)

- **FR-37:** A retention job SHALL delete expired recordings.

- **FR-38:** Deletion SHALL be hard (permanent) after a grace period
  of 7 days.

- **FR-39:** Client SHALL receive a deletion report monthly.

### 4.8 Failure Handling

- **FR-40:** If Egress fails to start:
  - Retry 3 times within 30 seconds
  - Log the failure
  - Notify admin if all retries fail

- **FR-41:** If Egress fails mid-session:
  - Attempt restart
  - If unsuccessful, record is marked `partial`

- **FR-42:** Partial recordings SHALL still be uploaded and playable.

- **FR-43:** Recording failures SHALL be visible to SRE via alerts.

### 4.9 Consent and Privacy

- **FR-44:** Consent SHALL be captured before joining:
  - Checkbox on the pre-join screen
  - Timestamp + IP + user agent logged

- **FR-45:** If a participant declines consent, the session SHALL NOT
  start (interviewer notified).

- **FR-46:** Recordings SHALL NOT be shared across tenants.

- **FR-47:** GDPR export/delete SHALL work on recordings.

### 4.10 Download

- **FR-48:** Client SHALL download the MP4 via signed URL.

- **FR-49:** Download SHALL be rate-limited (10/hour per user).

- **FR-50:** Download SHALL be audited (who, when).

---

## 5. Non-Functional Requirements

- **NFR-1:** Recording start SHALL begin within 5 seconds of session
  active.

- **NFR-2:** Composite SHALL be finalized within 2 minutes of session
  end.

- **NFR-3:** Recording quality SHALL be 1080p @ 30 fps (or lower for
  bandwidth-constrained sessions).

- **NFR-4:** Playback SHALL start within 3 seconds on broadband.

- **NFR-5:** Storage cost SHALL be optimized (cold storage after 30 days).

- **NFR-6:** The system SHALL support 100 concurrent recordings per
  LiveKit cluster.

- **NFR-7:** Recording SHALL NOT degrade live session quality.

---

## 6. Domain Model

### 6.1 Entity: Recording

```csharp
public sealed class Recording
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid TenantId { get; private set; }
    public RecordingStatus Status { get; private set; }
    public string? EgressId { get; private set; }
    public string? StoragePath { get; private set; }
    public string? HlsManifestPath { get; private set; }
    public int? DurationSeconds { get; private set; }
    public long? SizeBytes { get; private set; }
    public string? ThumbnailPath { get; private set; }
    public JsonDocument? Chapters { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? FinalizedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? FailureReason { get; private set; }

    public static Recording Create(Guid sessionId, Guid tenantId);
    public void MarkStarted(string egressId);
    public void MarkCompleted(string storagePath, int durationSeconds,
        long sizeBytes);
    public void MarkFailed(string reason);
}