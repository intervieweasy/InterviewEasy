
---

# 📄 Spec 005-session/006 — Chat

Save as: `specs\005-session\006-chat.spec.md`

---

```markdown
# Spec: Chat

**Service:** Session
**Phase:** 005-session
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Chat is the text channel of the session. It serves two purposes:

1. **Free-form conversation** — interviewers and candidates type
   short messages (clarifications, links, small hints)
2. **System timeline** — every structured event in the session is
   logged as a chat message (question pushed, test run, mode change,
   screen share)

This makes chat the **audit timeline** of the interview — a single
scrollable record.

Chat uses **SignalR** (not LiveKit data channel) for reliability and
persistence.

---

## 2. Scope

### In Scope

- Chat message entity
- Message types (text, code, system)
- SignalR hub for real-time delivery
- Message history and pagination
- Read receipts and typing indicators
- Message retention
- System event logging
- File/link sharing (link preview)
- Mentions and notifications

### Out of Scope

- Direct messages between participants (all chat is session-wide)
- Reactions (emoji)
- Threading (v1 = flat)
- Voice/video messages

---

## 3. User Stories

- **US-1:** As an **interviewer**, I want to type a hint to the
  candidate, so that I can guide without editing.

- **US-2:** As a **candidate**, I want to ask for clarification, so
  that I understand the problem.

- **US-3:** As an **interviewer**, I want to see a timeline of what
  happened, so that I can review after the interview.

- **US-4:** As a **user**, I want my message to appear immediately,
  so that chat feels responsive.

- **US-5:** As a **client**, I want the chat transcript saved with
  the session, so that I can review the interaction.

---

## 4. Functional Requirements

### 4.1 Chat Message Entity

- **FR-1:** A `ChatMessage` entity SHALL exist with:
  - `Id` (Guid, PK)
  - `SessionId` (Guid)
  - `TenantId` (Guid)
  - `SenderId` (Guid?)
  - `SenderRole` (string?)
  - `SenderName` (string)
  - `MessageType` (enum: Text, Code, System, Attachment, Link)
  - `Content` (string)
  - `Language` (string?, for code)
  - `Metadata` (JSONB)
  - `CreatedAt` (DateTime)
  - `EditedAt` (DateTime?)
  - `IsDeleted` (bool)

- **FR-2:** `MessageType` SHALL be one of: `text`, `code`, `system`,
  `attachment`, `link`.

- **FR-3:** System messages SHALL have `SenderId = null` and
  `SenderRole = "system"`.

- **FR-4:** Content SHALL be 1–5000 characters for text messages.

- **FR-5:** Code messages SHALL have a language specified.

### 4.2 Message Types

- **FR-6:** **Text** — plain message from a participant.

- **FR-7:** **Code** — code snippet with language (rendered with
  syntax highlighting).

- **FR-8:** **System** — automatically generated for session events:
  - Question pushed
  - Language changed
  - Mode changed
  - Test run completed
  - Screen share started/stopped
  - Solution revealed
  - Recording started/stopped
  - Session ended

- **FR-9:** **Attachment** — file reference (not implemented in v1;
  reserved).

- **FR-10:** **Link** — URL with auto-generated preview (deferred to
  Phase 5b).

### 4.3 Real-Time Delivery

- **FR-11:** Chat SHALL use SignalR for delivery.

- **FR-12:** SignalR hub SHALL be `/hubs/session`.

- **FR-13:** Messages SHALL be broadcast to `session:{id}` group.

- **FR-14:** Message delivery SHALL be under 100ms (p95).

- **FR-15:** Messages SHALL be persisted to PostgreSQL before
  broadcast (delivery guarantee).

- **FR-16:** Offline participants SHALL receive missed messages on
  reconnect via history API.

### 4.4 Message History

- **FR-17:** Clients SHALL fetch history via
  `GET /api/v1/sessions/{id}/chat?before={messageId}&limit=50`.

- **FR-18:** History SHALL support pagination (cursor-based).

- **FR-19:** History SHALL default to 50 messages per page.

- **FR-20:** History SHALL be ordered chronologically (oldest first).

- **FR-21:** History SHALL be cached in Redis for fast access.

- **FR-22:** History SHALL be retained for 90 days after session end.

### 4.5 Read Receipts

- **FR-23:** Each participant SHALL have a `lastReadMessageId`
  tracked.

- **FR-24:** Read receipts SHALL be sent via SignalR when the client
  scrolls to bottom.

- **FR-25:** Other participants SHALL see "Read by X" indicator.

- **FR-26:** Read receipts SHALL NOT be persisted to PostgreSQL —
  ephemeral in Redis.

### 4.6 Typing Indicators

- **FR-27:** Clients SHALL send `typing.started` on keystroke and
  `typing.stopped` after 2 seconds of inactivity.

- **FR-28:** Typing state SHALL be broadcast via SignalR.

- **FR-29:** Typing indicators SHALL auto-expire after 3 seconds if
  no update.

- **FR-30:** Typing indicators SHALL be ephemeral (not persisted).

### 4.7 System Event Logging

- **FR-31:** The following events SHALL create system messages:
  - Session started
  - Question pushed
  - Question completed
  - Language changed
  - Editor mode changed
  - Code executed (with pass/fail summary)
  - Screen share started/stopped
  - Solution revealed/hidden
  - Recording started/stopped
  - Participant joined/left
  - Session ended

- **FR-32:** System messages SHALL be created server-side by the
  Session service.

- **FR-33:** System messages SHALL include structured metadata for
  frontend rendering.

### 4.8 Mentions

- **FR-34:** Users SHALL mention others using `@name`.

- **FR-35:** Mentioned users SHALL receive a highlight in their chat UI.

- **FR-36:** Mentions SHALL be validated against active participants.

- **FR-37:** Invalid mentions SHALL render as plain text.

### 4.9 Message Editing

- **FR-38:** Users SHALL edit their own messages within 5 minutes.

- **FR-39:** Editing SHALL set `EditedAt`.

- **FR-40:** Edited messages SHALL show "(edited)" indicator.

- **FR-41:** Edit history SHALL NOT be retained (last version only).

### 4.10 Message Deletion

- **FR-42:** Users SHALL delete their own messages within 5 minutes.

- **FR-43:** Deletion SHALL be soft (`IsDeleted = true`).

- **FR-44:** Deleted messages SHALL show "This message was deleted".

- **FR-45:** Interviewer SHALL delete any participant's message
  (moderation).

### 4.11 Retention and Access

- **FR-46:** Chat messages SHALL be retained for 90 days after
  session end.

- **FR-47:** Chat transcripts SHALL be included in the post-interview
  report.

- **FR-48:** Client admins SHALL view chat transcripts via the
  interview detail page.

- **FR-49:** Chat messages SHALL NOT be included in recordings
  (separate artifact).

### 4.12 Rate Limiting

- **FR-50:** Chat SHALL be rate-limited: 30 messages/minute per
  participant.

- **FR-51:** Exceeding SHALL return 429 with `Retry-After`.

- **FR-52:** System messages SHALL NOT count toward rate limit.

---

## 5. Non-Functional Requirements

- **NFR-1:** Message delivery SHALL be under 100ms (p95).

- **NFR-2:** History fetch SHALL be under 200ms (p95) for 50 messages.

- **NFR-3:** The system SHALL support 100 messages/minute per session.

- **NFR-4:** Message storage SHALL be compressed at rest.

- **NFR-5:** Chat SHALL NOT block video or editor performance.

- **NFR-6:** Read receipts and typing indicators SHALL NOT be
  persisted.

---

## 6. Domain Model

### 6.1 Entity: ChatMessage

```csharp
public sealed class ChatMessage
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid? SenderId { get; private set; }
    public string? SenderRole { get; private set; }
    public string SenderName { get; private set; }
    public ChatMessageType MessageType { get; private set; }
    public string Content { get; private set; }
    public string? Language { get; private set; }
    public JsonDocument? Metadata { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public static ChatMessage CreateText(Guid sessionId, Guid tenantId,
        Guid senderId, string senderRole, string senderName,
        string content);

    public static ChatMessage CreateCode(Guid sessionId, Guid tenantId,
        Guid senderId, string senderRole, string senderName,
        string content, string language);

    public static ChatMessage CreateSystem(Guid sessionId, Guid tenantId,
        string content, JsonDocument metadata);

    public void Edit(string newContent);
    public void SoftDelete(Guid deletedBy);
}