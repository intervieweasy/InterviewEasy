using InterviewEasy.BuildingBlocks.EventBus.Abstractions;
using InterviewEasy.BuildingBlocks.EventBus.RabbitMQ;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace InterviewEasy.BuildingBlocks.EventBus.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers MassTransit with RabbitMQ and the IEventBus abstraction.
    /// Call once per service in Program.cs.
    /// </summary>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        var options = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>() ?? new RabbitMqOptions();

        services.AddSingleton(options);
        services.AddScoped<IEventBus, RabbitMqEventBus>();

        services.AddMassTransit(x =>
        {
            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(options.Host, options.Port, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                cfg.UseMessageRetry(r => r.Exponential(
                    options.RetryCount,
                    TimeSpan.FromSeconds(options.RetryBaseSeconds),
                    TimeSpan.FromSeconds(options.RetryBaseSeconds * 10),
                    TimeSpan.FromSeconds(options.RetryBaseSeconds)));

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}