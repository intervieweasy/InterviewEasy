using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InterviewEasy.BuildingBlocks.Observability.HealthChecks;

public static class HealthCheckExtensions
{
    /// <summary>
    /// Registers the three standard health check endpoints:
    /// /health/live, /health/ready, /health/startup.
    /// </summary>
    public static IServiceCollection AddPlatformHealthChecks(
        this IServiceCollection services,
        string? connectionString = null)
    {
        var builder = services.AddHealthChecks();

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            builder.AddNpgSql(
                connectionString: connectionString,
                name: "postgres",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "ready" });
        }

        return services;
    }

    public static IApplicationBuilder UsePlatformHealthChecks(
        this IApplicationBuilder app)
    {
        // Liveness — always 200 if the process is running.
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        // Readiness — all critical dependencies must be healthy.
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        // Startup — full check (same as ready for now).
        app.UseHealthChecks("/health/startup", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        return app;
    }
}
