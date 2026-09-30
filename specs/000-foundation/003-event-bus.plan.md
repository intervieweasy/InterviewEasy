# Plan: Event Bus

**Spec:** 003-event-bus.spec.md
**Phase:** 000-foundation
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## Overview

This plan breaks `003-event-bus.spec.md` into concrete, ordered tasks.
Each task produces testable code. Tasks are completed in order — later
tasks depend on earlier ones.

**Estimated effort:** 3–5 days for a senior .NET developer.

---

## Task 1 — Create Event Contracts

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Abstractions/IIntegrationEvent.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Abstractions/IntegrationEvent.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Abstractions/IEventBus.cs`

**Content:**

- `IIntegrationEvent` with `Id`, `OccurredOn`, `CorrelationId`, `CausationId`
- Abstract `IntegrationEvent` record implementing the interface
- `IEventBus` with `PublishAsync<T>(T @event, CancellationToken)` and
  `PublishAsync<T>(T @event, string correlationId, CancellationToken)`

**Tests:**
- `tests/UnitTests/InterviewEasy.BuildingBlocks.EventBus.UnitTests/`
  - `IntegrationEvent_GeneratesNewId_OnConstruction`
  - `IntegrationEvent_DefaultsToUtcNow`

**Depends on:** Nothing

**Acceptance:**
- `dotnet build` succeeds
- Unit tests pass

---

## Task 2 — Create Outbox and ProcessedEvents Entities

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Outbox/OutboxMessage.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Outbox/IOutboxStore.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Idempotency/ProcessedEvent.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Idempotency/IProcessedEventStore.cs`

**Content:**

- `OutboxMessage` entity with fields: `Id`, `EventType`, `EventData`,
  `OccurredOn`, `CreatedAt`, `PublishedAt`, `RetryCount`, `LastError`
- `ProcessedEvent` entity with fields: `EventId`, `EventType`, `ProcessedAt`
- Interfaces for store access — implementations live in
  `BuildingBlocks.EventBus.RabbitMQ` or per-service infrastructure

**Tests:**
- `OutboxMessage_RecordsRetryCount`
- `ProcessedEvent_UniquenessByEventId`

**Depends on:** Task 1

**Acceptance:**
- Entities compile
- Tests pass

---

## Task 3 — Implement MassTransit RabbitMQ Event Bus

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/RabbitMQ/RabbitMqEventBus.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/RabbitMQ/RabbitMqOptions.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Extensions/ServiceCollectionExtensions.cs`

**Content:**

- `RabbitMqEventBus : IEventBus` wrapping `IBus` from MassTransit
- `RabbitMqOptions` with `Host`, `Port`, `Username`, `Password`, `VirtualHost`
- `AddEventBus(configuration)` extension registering MassTransit with
  RabbitMQ transport

**Tests:**
- Integration test: boot RabbitMQ via Testcontainers, publish a message,
  assert it lands in the exchange

**Depends on:** Task 1

**Acceptance:**
- Publishing a message to a real RabbitMQ container works
- Test uses Testcontainers

---

## Task 4 — Implement Outbox Interceptor

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Outbox/OutboxSaveChangesInterceptor.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Outbox/OutboxExtensions.cs`

**Content:**

- EF Core `SaveChangesInterceptor` that intercepts entity changes,
  scans for pending `IntegrationEvent` instances, and writes them to
  `outbox_messages` in the same transaction
- Extension method `AddOutbox<TContext>()` registering the interceptor

**Tests:**
- Integration test: save a change with an event, assert `outbox_messages`
  has the row

**Depends on:** Task 2

**Acceptance:**
- Interceptor writes to outbox within the transaction
- If transaction rolls back, no outbox row remains

---

## Task 5 — Implement Outbox Publisher Worker

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Outbox/OutboxPublisherWorker.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Outbox/OutboxPublisherOptions.cs`

**Content:**

- `BackgroundService` that polls `outbox_messages` every 5 seconds
  (configurable), publishes unpublished rows via `IEventBus`, marks
  `published_at` on success, increments `retry_count` on failure
- Configurable batch size (default 100)

**Tests:**
- Integration test: insert 3 rows manually, start worker, assert all
  published and `published_at` set

**Depends on:** Task 3, Task 4

**Acceptance:**
- Worker drains outbox within 10 seconds
- Retry count increments on failure

---

## Task 6 — Implement Consumer Idempotency

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Idempotency/IdempotentConsumerFilter.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Idempotency/IdempotencyExtensions.cs`

**Content:**

- MassTransit consume filter that checks `processed_events` before
  invoking the consumer; if the event ID exists, short-circuits and
  returns success without invoking the consumer
- Extension method `UseIdempotentConsumers()` to register the filter

**Tests:**
- Integration test: publish same event twice, assert consumer executed once

**Depends on:** Task 2

**Acceptance:**
- Duplicate events don't cause duplicate side effects

---

## Task 7 — Configure Retry and Dead-Letter Policy

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/RabbitMQ/RetryConfiguration.cs`
- Update `ServiceCollectionExtensions.AddEventBus()` to apply retry policy

**Content:**

- Exponential backoff: immediate, 5s, 30s
- Error queue naming: `{queue}_error` (MassTransit default)
- Dead-letter handling via MassTransit's built-in error transport

**Tests:**
- Integration test: consumer that throws always → assert message lands
  in `_error` queue after 3 attempts

**Depends on:** Task 3

**Acceptance:**
- Failed messages move to error queue after retries
- RabbitMQ management UI shows the DLQ

---

## Task 8 — Add Observability

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Observability/EventBusMetrics.cs`
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Observability/TracingExtensions.cs`
- Update `ServiceCollectionExtensions.AddEventBus()` to register
  OpenTelemetry instrumentation

**Content:**

- Counters: `events_published_total`, `events_consumed_total`,
  `events_failed_total`, `outbox_pending_total`
- OpenTelemetry spans on publish and consume
- Correlation ID propagation

**Tests:**
- Unit test: metrics increment on publish
- Integration test: trace spans span publisher and consumer

**Depends on:** Task 3

**Acceptance:**
- `/metrics` exposes the counters
- Traces show parent-child span relationship

---

## Task 9 — Wire Up Consumer Registration Extensions

**Files:**
- `src/BuildingBlocks/InterviewEasy.BuildingBlocks.EventBus/Extensions/ConsumerRegistrationExtensions.cs`

**Content:**

- Extension method `AddConsumer<TEvent, TConsumer>()` for clean
  registration in each service's `Program.cs`
- Automatic endpoint naming per convention (FR-9)

**Tests:**
- Unit test: endpoint name matches `{consumer}.{event-kebab}`

**Depends on:** Task 3, Task 6, Task 7

**Acceptance:**
- Consumer registered via the extension receives events

---

## Task 10 — Full End-to-End Integration Test

**Files:**
- `tests/IntegrationTests/InterviewEasy.BuildingBlocks.EventBus.IntegrationTests/EndToEndTests.cs`

**Content:**

- Boot Postgres + RabbitMQ via Testcontainers
- Publisher service writes an aggregate + event via outbox
- Worker publishes
- Consumer service receives, checks idempotency, processes
- Assert: event handled exactly once, outbox empty, processed_events has row

**Tests:** This IS the test.

**Depends on:** Tasks 1–9

**Acceptance:**
- Full flow works in CI
- Test runs in under 30 seconds

---

## Task 11 — Documentation Update

**Files:**
- `docs/guides/event-bus-guide.md`
- `docs/adr/0004-rabbitmq-via-masstransit.md`
- `docs/adr/0005-outbox-pattern.md`

**Content:**

- How to publish an event from a service
- How to consume an event in a service
- Naming conventions cheat sheet
- ADRs documenting the choice of RabbitMQ, MassTransit, and the outbox pattern

**Depends on:** Tasks 1–10

**Acceptance:**
- A developer unfamiliar with the codebase can follow the guide and
  publish their first event within 30 minutes

---

## Task 12 — Update Spec 003 Status to `Implemented`

**Files:**
- `specs/000-foundation/003-event-bus.spec.md`

**Content:**

- Change `Status: Draft` → `Status: Implemented`
- Check all 15 acceptance criteria boxes

**Depends on:** All previous tasks

**Acceptance:**
- All ACs verified
- Spec marked `Implemented`

---

## Task Ordering (Dependency Graph)
