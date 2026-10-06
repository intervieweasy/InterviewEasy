namespace InterviewEasy.BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// Convenience base record for integration events.
/// Provides sensible defaults for <see cref="IIntegrationEvent"/> members.
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string CorrelationId { get; init; } = string.Empty;
    public string? CausationId { get; init; }
}