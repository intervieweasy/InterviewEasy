namespace InterviewEasy.BuildingBlocks.EventBus.Outbox;

/// <summary>
/// Access layer for the outbox table. Implemented per-service
/// against the service's own DbContext.
/// </summary>
public interface IOutboxStore
{
    Task<IReadOnlyList<OutboxMessage>> GetUnpublishedAsync(
        int batchSize, CancellationToken ct = default);

    Task MarkPublishedAsync(Guid messageId, CancellationToken ct = default);

    Task RecordFailureAsync(Guid messageId, string error,
        CancellationToken ct = default);
}