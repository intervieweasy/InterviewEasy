using InterviewEasy.BuildingBlocks.EventBus.Abstractions;

namespace InterviewEasy.Identity.Domain.Events;

public sealed record UserInvitedEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public Guid TenantId { get; init; }
    public string Email { get; init; } = string.Empty;
    public Guid InvitedBy { get; init; }
    public DateTime InvitedAt { get; init; }
}
