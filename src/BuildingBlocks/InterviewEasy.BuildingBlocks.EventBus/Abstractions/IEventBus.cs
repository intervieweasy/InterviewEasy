namespace InterviewEasy.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default)
        where T : class, IIntegrationEvent;

    Task PublishAsync<T>(T @event, string correlationId, CancellationToken ct = default)
        where T : class, IIntegrationEvent;
}