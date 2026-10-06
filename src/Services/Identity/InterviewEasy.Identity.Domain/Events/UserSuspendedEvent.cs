using InterviewEasy.BuildingBlocks.EventBus.Abstractions;

namespace InterviewEasy.Identity.Domain.Events;

public sealed record UserSuspendedEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public Guid TenantId { get; init; }
    public Guid SuspendedBy { get; init; }
    public DateTime SuspendedAt { get; init; }
}
