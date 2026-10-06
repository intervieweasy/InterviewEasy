using InterviewEasy.BuildingBlocks.EventBus.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.BuildingBlocks.EventBus.RabbitMQ;

internal sealed class RabbitMqEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RabbitMqEventBus> _logger;

    public RabbitMqEventBus(IPublishEndpoint publishEndpoint, ILogger<RabbitMqEventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public Task PublishAsync<T>(T @event, CancellationToken ct = default)
        where T : class, IIntegrationEvent
        => PublishInternalAsync(@event, @event.CorrelationId, ct);

    public Task PublishAsync<T>(T @event, string correlationId, CancellationToken ct = default)
        where T : class, IIntegrationEvent
        => PublishInternalAsync(@event, correlationId, ct);

    private async Task PublishInternalAsync<T>(T @event, string correlationId, CancellationToken ct) where T : class, IIntegrationEvent
    {
        _logger.LogInformation(
            "Publishing event {EventType} with id {EventId} (correlation {CorrelationId})",
            typeof(T).Name, @event.Id, correlationId);

        await _publishEndpoint.Publish<T>(
            @event,
            Pipe.Execute<PublishContext<T>>(context =>
            {
                context.CorrelationId = Guid.TryParse(correlationId, out var g) ? g : Guid.NewGuid();
                context.Headers.Set("x-event-id", @event.Id.ToString());
                context.Headers.Set("x-event-type", typeof(T).FullName ?? typeof(T).Name);
                context.Headers.Set("x-correlation-id", correlationId);
            }),
            ct);

        _logger.LogInformation(
            "Published event {EventType} with id {EventId}",
            typeof(T).Name, @event.Id);
    }
}