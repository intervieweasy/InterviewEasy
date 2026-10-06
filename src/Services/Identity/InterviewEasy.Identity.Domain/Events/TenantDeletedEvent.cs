using InterviewEasy.BuildingBlocks.EventBus.Abstractions;

namespace InterviewEasy.Identity.Domain.Events;

public sealed record TenantDeletedEvent : IntegrationEvent
{
    public Guid TenantId { get; init; }
    public DateTime DeletedAt { get; init; }
}
