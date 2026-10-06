using InterviewEasy.BuildingBlocks.Observability.HealthChecks;
using InterviewEasy.BuildingBlocks.Observability.Logging;
using InterviewEasy.BuildingBlocks.Observability.Metrics;
using InterviewEasy.BuildingBlocks.Observability.Tracing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace InterviewEasy.BuildingBlocks.Observability.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Wires up Serilog, OpenTelemetry tracing, Prometheus metrics,
    /// and health checks in a single call.
    /// </summary>
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string? connectionString = null)
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        var otlpEndpoint = configuration["OpenTelemetry:Endpoint"]
            ?? "http://localhost:4317";

        // Logging — Serilog
        Log.Logger = new LoggerConfiguration()
            .ConfigureSerilog(configuration, serviceName, environment)
            .CreateLogger();
        services.AddSerilog(Log.Logger, dispose: true);

        // Tracing — OpenTelemetry
        services.AddPlatformTracing(serviceName, otlpEndpoint);

        // Metrics — Prometheus
        services.AddPlatformMetrics();

        // Health Checks
        services.AddPlatformHealthChecks(connectionString);

        return services;
    }
}
