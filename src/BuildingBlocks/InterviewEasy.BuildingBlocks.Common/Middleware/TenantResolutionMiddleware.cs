using Microsoft.AspNetCore.Http;

namespace InterviewEasy.BuildingBlocks.Common.Middleware;

public sealed class TenantResolutionMiddleware
{
    private const string TenantIdClaim = "tenant_id";
    private const string TenantCodeClaim = "tenant_code";
    private const string TenantIdHeader = "X-Tenant-Id";

    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Prefer JWT claim
        var tenantIdClaim = context.User?.FindFirst(TenantIdClaim)?.Value;
        var tenantCodeClaim = context.User?.FindFirst(TenantCodeClaim)?.Value;

        // 2. Fallback to header
        var tenantIdHeader = context.Request.Headers[TenantIdHeader].FirstOrDefault();

        var tenantIdRaw = tenantIdClaim ?? tenantIdHeader;
        var tenantCode = tenantCodeClaim;

        if (Guid.TryParse(tenantIdRaw, out var tenantId))
        {
            context.Items["TenantId"] = tenantId;
        }

        if (!string.IsNullOrEmpty(tenantCode))
        {
            context.Items["TenantCode"] = tenantCode;
            context.Items["TenantSchema"] = $"tenant_{tenantCode}";
        }

        await _next(context);
    }
}