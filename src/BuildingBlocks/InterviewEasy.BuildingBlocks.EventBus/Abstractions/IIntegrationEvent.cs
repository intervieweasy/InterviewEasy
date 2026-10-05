namespace InterviewEasy.BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// Base contract for all integration events published on the event bus.
/// Every event carries its own identity, timing, and correlation metadata
/// so consumers can deduplicate, trace, and order messages.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>Unique event ID — used for idempotency checks.</summary>
    Guid Id { get; }

    /// <summary>UTC timestamp when the event was raised.</summary>
    DateTime OccurredOn { get; }

    /// <summary>Correlation ID from the originating HTTP request or command.</summary>
    string CorrelationId { get; }

    /// <summary>Optional ID of the event or command that caused this one.</summary>
    string? CausationId { get; }
}