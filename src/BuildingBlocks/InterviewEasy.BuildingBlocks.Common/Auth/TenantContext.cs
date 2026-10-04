using Microsoft.AspNetCore.Http;

namespace InterviewEasy.BuildingBlocks.Common.Auth;

public sealed class TenantContext : ITenantContext
{
    private const string TenantIdKey = "TenantId";
    private const string TenantCodeKey = "TenantCode";
    private const string SchemaPrefixKey = "TenantSchema";

    private readonly IHttpContextAccessor _accessor;

    public TenantContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private IDictionary<object, object?>? Items => _accessor.HttpContext?.Items;

    public Guid? TenantId
    {
        get
        {
            var raw = Items?.TryGetValue(TenantIdKey, out var v) == true ? v : null;
            return raw is Guid g ? g : null;
        }
    }

    public string? TenantCode =>
        Items?.TryGetValue(TenantCodeKey, out var v) == true ? v as string : null;

    public string? SchemaPrefix =>
        Items?.TryGetValue(SchemaPrefixKey, out var v) == true ? v as string : null;

    public bool IsResolved => TenantId.HasValue && !string.IsNullOrEmpty(SchemaPrefix);
}