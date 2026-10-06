using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace InterviewEasy.BuildingBlocks.Observability.Metrics;

public static class MetricsExtensions
{
    /// <summary>
    /// Registers Prometheus metrics for the service.
    /// Exposes /metrics endpoint (scraped by Prometheus).
    /// </summary>
    public static IServiceCollection AddPlatformMetrics(this IServiceCollection services)
    {
        // No-op — metrics are configured via middleware.
        return services;
    }

    /// <summary>
    /// Starts the Prometheus metric server.
    /// Call from Program.cs after building the app.
    /// </summary>
    public static IApplicationBuilder UsePlatformMetrics(this IApplicationBuilder app)
    {
        app.UseHttpMetrics();
        app.UseMetricServer();   // exposes /metrics on the service's own port
        return app;
    }
}
