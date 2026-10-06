using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace InterviewEasy.BuildingBlocks.Observability.Logging;

/// <summary>
/// Enriches log events with correlation ID and tenant ID
/// pulled from the current HttpContext.
/// </summary>
public sealed class HttpContextEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _accessor;

    public HttpContextEnricher(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var ctx = _accessor.HttpContext;
        if (ctx is null) return;

        if (ctx.Items.TryGetValue("CorrelationId", out var corr) && corr is string corrStr)
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("CorrelationId", corrStr));
        }

        if (ctx.Items.TryGetValue("TenantId", out var tenant) && tenant is Guid tenantId)
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("TenantId", tenantId));
        }

        if (ctx.User?.Identity?.IsAuthenticated == true)
        {
            var sub = ctx.User.FindFirst("sub")?.Value
                   ?? ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(sub))
            {
                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty("UserId", sub));
            }
        }
    }
}
