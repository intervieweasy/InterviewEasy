namespace InterviewEasy.BuildingBlocks.EventBus.Idempotency;

/// <summary>
/// Persistence record of a consumed event ID.
/// Used by consumers to skip duplicate deliveries.
/// </summary>
public sealed class ProcessedEvent
{
    public Guid EventId { get; private set; }
    public string EventType { get; private set; } = default!;
    public DateTime ProcessedAt { get; private set; }

    private ProcessedEvent() { }

    public static ProcessedEvent Create(Guid eventId, string eventType)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("EventId is required.", nameof(eventId));
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType is required.", nameof(eventType));

        return new ProcessedEvent
        {
            EventId = eventId,
            EventType = eventType,
            ProcessedAt = DateTime.UtcNow
        };
    }
}