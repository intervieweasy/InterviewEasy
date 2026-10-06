using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace InterviewEasy.BuildingBlocks.Observability.Tracing;

public static class TracingExtensions
{
    /// <summary>
    /// Registers OpenTelemetry tracing with OTLP exporter.
    /// Auto-instruments ASP.NET Core, HttpClient, EF Core, and MassTransit.
    /// </summary>
    public static IServiceCollection AddPlatformTracing(
        this IServiceCollection services,
        string serviceName,
        string otlpEndpoint)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(serviceName)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                        ?? "Development"
                }))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.RecordException = true;
                        o.Filter = ctx =>
                            !ctx.Request.Path.StartsWithSegments("/health") &&
                            !ctx.Request.Path.StartsWithSegments("/metrics");
                    })
                    .AddHttpClientInstrumentation(o => o.RecordException = true)
                    .AddEntityFrameworkCoreInstrumentation(o =>
                        o.SetDbStatementForText = false)
                    .AddSource("MassTransit")
                    .AddSource(ActivitySourceNames.Identity)
                    .AddSource(ActivitySourceNames.Requirement)
                    .AddSource(ActivitySourceNames.Question)
                    .AddSource(ActivitySourceNames.Scheduling)
                    .AddSource(ActivitySourceNames.Session)
                    .AddSource(ActivitySourceNames.Feedback)
                    .AddSource(ActivitySourceNames.Sandbox)
                    .AddSource(ActivitySourceNames.Proctoring)
                    .AddSource(ActivitySourceNames.Notification)
                    .AddSource(ActivitySourceNames.Recording)
                    .AddSource(ActivitySourceNames.Analytics)
                    .AddSource(ActivitySourceNames.Common)
                    .AddSource(ActivitySourceNames.EventBus)
                    .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
            });

        return services;
    }
}
