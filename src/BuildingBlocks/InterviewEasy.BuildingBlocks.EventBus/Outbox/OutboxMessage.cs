using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.BuildingBlocks.EventBus.Outbox;

/// <summary>
/// Persistence record for the transactional outbox pattern.
/// Written in the same DB transaction as the aggregate change,
/// published asynchronously by the outbox worker.
/// </summary>
public sealed class OutboxMessage : BaseEntity
{
    public string EventType { get; private set; } = default!;
    public string EventData { get; private set; } = default!;
    public DateTime OccurredOn { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }

    private OutboxMessage() { }

    public static OutboxMessage Create(
        string eventType,
        string eventData,
        DateTime occurredOn)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType is required.", nameof(eventType));
        if (string.IsNullOrWhiteSpace(eventData))
            throw new ArgumentException("EventData is required.", nameof(eventData));

        return new OutboxMessage
        {
            EventType = eventType,
            EventData = eventData,
            OccurredOn = occurredOn,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkPublished()
    {
        PublishedAt = DateTime.UtcNow;
        LastError = null;
    }

    public void RecordFailure(string error)
    {
        RetryCount++;
        LastError = error?.Length > 2000 ? error[..2000] : error;
    }
}