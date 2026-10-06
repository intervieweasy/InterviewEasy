using InterviewEasy.BuildingBlocks.Observability.HealthChecks;
using InterviewEasy.BuildingBlocks.Observability.Metrics;
using Microsoft.AspNetCore.Builder;

namespace InterviewEasy.BuildingBlocks.Observability.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Registers health check and metrics middleware.
    /// Call from Program.cs after building the app.
    /// </summary>
    public static WebApplication UseObservability(this WebApplication app)
    {
        app.UsePlatformHealthChecks();
        app.UsePlatformMetrics();
        return app;
    }
}
