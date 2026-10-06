namespace InterviewEasy.BuildingBlocks.EventBus.Idempotency;

public interface IProcessedEventStore
{
    Task<bool> HasBeenProcessedAsync(Guid eventId,
        CancellationToken ct = default);

    Task MarkAsProcessedAsync(Guid eventId, string eventType,
        CancellationToken ct = default);
}