using InterviewEasy.BuildingBlocks.EventBus.Abstractions;

namespace InterviewEasy.Identity.Domain.Events;

public sealed record TenantSuspendedEvent : IntegrationEvent
{
    public Guid TenantId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime SuspendedAt { get; init; }
}
