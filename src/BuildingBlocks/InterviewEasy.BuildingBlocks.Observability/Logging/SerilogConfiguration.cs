using Microsoft.Extensions.Configuration;
using Serilog;

namespace InterviewEasy.BuildingBlocks.Observability.Logging;

public static class SerilogConfiguration
{
    /// <summary>
    /// Configures Serilog for the service.
    /// Dev: human-readable console. Prod: JSON to console (shipped by platform).
    /// </summary>
    public static LoggerConfiguration ConfigureSerilog(
        this LoggerConfiguration logger,
        IConfiguration configuration,
        string serviceName,
        string environment)
    {
        logger
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", serviceName)
            .Enrich.WithProperty("environment", environment)
            .Enrich.WithProperty("version",
                typeof(SerilogConfiguration).Assembly.GetName().Version?.ToString()
                ?? "unknown");

        if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            logger.WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] " +
                "({service}) {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            logger.WriteTo.Console(new Serilog.Formatting.Compact.CompactJsonFormatter());
        }

        return logger;
    }
}
