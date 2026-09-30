# Spec: Event Bus

**Service:** Foundation
**Phase:** 000-foundation
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

InterviewEasy is built as **12+ microservices**, each owning its own
database schema (see Spec 002 — Database Strategy). Services need to
communicate without sharing databases or making synchronous
service-to-service HTTP calls for state changes.

Example flows that require asynchronous messaging:

- When a new tenant is created (Identity service), the Question service
  must provision its sub-schema, and Notification must send a welcome email.
- When a candidate submits code (Session service), the Sandbox service
  must execute it and publish results back.
- When an interview is completed (Session service), Analytics and
  Recording must react.
- When feedback is submitted (Feedback service), the client dashboard
  must update and possibly trigger a hiring decision event.

Synchronous HTTP calls between services create tight coupling, cascading
failures, and blocking user requests. Instead, we use an **event bus**:
services publish **integration events** to a message broker, and other
services subscribe. Publishers don't know who consumes their events.

This spec defines the **event contract, naming conventions, retry
strategy, dead-letter handling, and outbox pattern** that every service
must follow when publishing or consuming events.

This is the third and final foundational spec before Phase 1 begins.

---

## 2. Scope

### In Scope

- Choice of message broker (RabbitMQ) and client library (MassTransit)
- The `IIntegrationEvent` contract
- Event naming conventions
- Exchange and queue naming conventions
- Publish and subscribe patterns
- Retry policy (exponential backoff)
- Dead-letter queue (DLQ) handling
- Transactional outbox pattern for guaranteed delivery
- Consumer idempotency requirements
- Event schema versioning strategy
- Event size limits
- Observability (trace IDs, correlation IDs)

### Out of Scope

- Domain events (in-process, within a single service) — these are
  C# events handled by MediatR inside one service, not published to the bus
- Event sourcing as a persistence pattern (we use traditional CRUD + events)
- Kafka or other streaming platforms (RabbitMQ is the choice)
- gRPC-based service-to-service calls (used only for Sandbox execution)
- Saga / process manager orchestration (future spec if needed)
- Event replay tools (future admin feature)

---

## 3. User Stories

- **US-1:** As a **backend developer**, I want a standard way to publish
  domain changes as integration events, so that other services can react
  without me knowing who consumes them.

- **US-2:** As a **backend developer**, I want a standard way to subscribe
  to integration events, so that my service reacts reliably even if the
  publisher was temporarily down.

- **US-3:** As an **SRE**, I want failed event deliveries to be retried
  with backoff and eventually moved to a DLQ, so that transient failures
  don't lose data.

- **US-4:** As an **SRE**, I want every published event to carry a
  correlation ID and trace ID, so that I can follow a request through
  multiple services in logs.

- **US-5:** As a **compliance officer**, I want guaranteed event delivery
  even when the database transaction and the message broker are not
  atomic, so that no business event is lost.

- **US-6:** As a **tech lead**, I want consumer logic to be idempotent by
  requirement, so that duplicate deliveries (which RabbitMQ allows) don't
  cause duplicate side effects.

---

## 4. Functional Requirements

### 4.1 Event Contract

- **FR-1:** Every integration event SHALL implement
  `IIntegrationEvent` from `InterviewEasy.BuildingBlocks.EventBus.Abstractions`.

- **FR-2:** `IIntegrationEvent` SHALL expose:
  - `Guid Id` — unique event ID (used for idempotency)
  - `DateTime OccurredOn` — UTC timestamp when the event was raised
  - `string CorrelationId` — request correlation ID
  - `string? CausationId` — the ID of the event/command that caused this one

- **FR-3:** Events SHALL be immutable, serializable records (C# `record`
  or `record class`).

- **FR-4:** Events SHALL be defined in the `Application` layer of the
  publishing service (under `Events/` or `IntegrationEvents/`).

- **FR-5:** Events SHALL be versioned. Breaking changes require a new
  event (e.g., `RequirementCreatedV2Event`), not mutating the existing one.

### 4.2 Naming Conventions

- **FR-6:** Event class names SHALL follow
  `{Entity}{PastTenseVerb}Event`:
  - `TenantCreatedEvent`
  - `RequirementApprovedEvent`
  - `InterviewCompletedEvent`
  - `FeedbackSubmittedEvent`

- **FR-7:** Event names SHALL NOT include the service name (the exchange
  namespace already scopes it).

- **FR-8:** Exchange names SHALL follow
  `intervieweasy.{service}.{event-kebab}`:
  - `intervieweasy.identity.tenant-created`
  - `intervieweasy.requirement.requirement-approved`
  - `intervieweasy.session.interview-completed`

- **FR-9:** Queue names SHALL follow `{consumer-service}.{event-kebab}`:
  - `notification.tenant-created` (Notification service consuming TenantCreatedEvent)
  - `analytics.interview-completed`
  - `recording.interview-completed`

- **FR-10:** Every consumer service SHALL have its own queue per event
  it consumes. Publishers SHALL NOT know consumer queue names.

### 4.3 Publish Pattern

- **FR-11:** Services SHALL publish events via `IEventBus.PublishAsync<T>()`.

- **FR-12:** Events SHALL be published **after** the database transaction
  commits, using the **transactional outbox pattern** (see FR-15).

- **FR-13:** Publishers SHALL NOT await consumer processing. Publishing
  is fire-and-forget from the publisher's perspective.

- **FR-14:** If a publish fails after retries, the event SHALL remain in
  the outbox table and be retried by a background worker.

### 4.4 Transactional Outbox

- **FR-15:** Every service that publishes events SHALL have an
  `outbox_messages` table in its schema.

- **FR-16:** Publishing an event SHALL write to `outbox_messages`
  **within the same EF Core transaction** as the aggregate change.

- **FR-17:** A background worker (`OutboxPublisher`) SHALL read
  unpublished rows from `outbox_messages` and publish them to RabbitMQ.

- **FR-18:** After successful publish, the worker SHALL mark the row as
  published (`published_at` timestamp set).

- **FR-19:** The outbox worker SHALL run every 5 seconds (configurable).

- **FR-20:** Outbox rows SHALL be retained for 30 days for audit, then
  purged by a cleanup job.

- **FR-21:** The `outbox_messages` table SHALL have the structure:
  - `id UUID PRIMARY KEY`
  - `event_type VARCHAR(500) NOT NULL` (assembly-qualified type)
  - `event_data JSONB NOT NULL` (serialized event)
  - `occurred_on TIMESTAMPTZ NOT NULL`
  - `created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()`
  - `published_at TIMESTAMPTZ NULL`
  - `retry_count INT NOT NULL DEFAULT 0`
  - `last_error TEXT NULL`

### 4.5 Subscribe Pattern

- **FR-22:** Services SHALL subscribe to events via
  `IConsumer<TEvent>` implementations registered in DI.

- **FR-23:** Every consumer SHALL be idempotent. Processing the same
  event twice SHALL produce the same result as processing it once.

- **FR-24:** Consumers SHALL track processed event IDs in a
  `processed_events` table to prevent duplicate processing.

- **FR-25:** The `processed_events` table SHALL have:
  - `event_id UUID PRIMARY KEY`
  - `event_type VARCHAR(500) NOT NULL`
  - `processed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()`

- **FR-26:** A consumer SHALL check `processed_events` at the start;
  if the event ID exists, it SHALL return without side effects.

- **FR-27:** Consumers SHALL wrap their logic and the `processed_events`
  insert in a single database transaction.

### 4.6 Retry and Error Handling

- **FR-28:** Failed message processing SHALL retry 3 times with
  exponential backoff:
  - Attempt 1: immediate
  - Attempt 2: after 5 seconds
  - Attempt 3: after 30 seconds
  - After 3 failures: move to dead-letter queue

- **FR-29:** Retry delays SHALL be configurable per environment.

- **FR-30:** Messages in the DLQ SHALL be visible in RabbitMQ management UI
  and alertable via metrics.

- **FR-31:** A DLQ message SHALL be manually re-playable from the admin UI
  (future feature — not in this spec's implementation scope).

- **FR-32:** Consumers SHALL log every failure with the event ID,
  correlation ID, exception, and retry attempt.

### 4.7 Observability

- **FR-33:** Every event SHALL propagate `CorrelationId` from the
  originating HTTP request or command.

- **FR-34:** Publishing and consuming SHALL be traceable via
  OpenTelemetry spans. The publisher's span SHALL be the parent of the
  consumer's span.

- **FR-35:** Metrics SHALL be exposed for:
  - `events_published_total{event_type}`
  - `events_consumed_total{event_type, consumer_service}`
  - `events_failed_total{event_type, consumer_service}`
  - `events_dlq_total{event_type}`
  - `outbox_pending_total{service}`

### 4.8 Event Size and Serialization

- **FR-36:** Events SHALL be serialized as JSON (System.Text.Json).

- **FR-37:** Event payloads SHALL NOT exceed 256 KB. Larger payloads
  SHALL store data externally (S3) and pass a reference URL.

- **FR-38:** Events SHALL NOT include sensitive data (passwords, tokens,
  PII) in cleartext. Sensitive fields SHALL be redacted or referenced
  by ID.

- **FR-39:** Events SHALL NOT include entire entity graphs — only the
  minimal data consumers need. Consumers fetch additional data via APIs
  if required.

### 4.9 Configuration

- **FR-40:** RabbitMQ connection settings SHALL come from configuration:
  - `RabbitMQ:Host`
  - `RabbitMQ:Port`
  - `RabbitMQ:Username`
  - `RabbitMQ:Password` (from Key Vault in prod)
  - `RabbitMQ:VirtualHost`

- **FR-41:** Connection settings SHALL be registered via
  `AddEventBus(configuration)` extension method.

- **FR-42:** MassTransit SHALL be configured with:
  - Automatic endpoint naming (from FR-9)
  - Retry policy (from FR-28)
  - Error queue per consumer
  - Message scheduler for delayed retries

---

## 5. Non-Functional Requirements

- **NFR-1:** Publish latency (from `PublishAsync` call to message in
  RabbitMQ) SHALL be under 100ms (p95).

- **NFR-2:** End-to-end latency (publish to consumer processing) SHALL be
  under 500ms (p95) under normal load.

- **NFR-3:** The system SHALL guarantee **at-least-once delivery**.
  Exactly-once is achieved via idempotent consumers, not the broker.

- **NFR-4:** Event ordering SHALL be guaranteed **per aggregate root**
  (events for the same entity are delivered in order). Cross-aggregate
  ordering is NOT guaranteed.

- **NFR-5:** The outbox worker SHALL drain a backlog of 10,000 messages
  in under 60 seconds.

- **NFR-6:** RabbitMQ SHALL be deployed with mirrored queues for HA in
  production (quorum queues).

- **NFR-7:** The system SHALL sustain 1,000 events/second publish rate
  per service instance.

- **NFR-8:** Zero message loss on service restart — outbox guarantees
  events survive process crashes.

- **NFR-9:** The DLQ SHALL be monitored and alertable; DLQ depth > 100
  SHALL trigger a PagerDuty alert.

---

## 6. Domain Model

This spec does not define domain entities. It defines **integration
event contracts** shared across services.

**Base interface (in BuildingBlocks.EventBus):**

```csharp
public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    string CorrelationId { get; }
    string? CausationId { get; }
}