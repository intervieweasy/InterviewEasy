using InterviewEasy.BuildingBlocks.EventBus.Abstractions;

namespace InterviewEasy.Identity.Domain.Events;

public sealed record UserRegisteredEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public Guid TenantId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime RegisteredAt { get; init; }
}
