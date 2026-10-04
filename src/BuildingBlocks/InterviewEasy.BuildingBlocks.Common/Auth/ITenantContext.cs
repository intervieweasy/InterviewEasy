namespace InterviewEasy.BuildingBlocks.Common.Auth;

public interface ITenantContext
{
    Guid? TenantId { get; }
    string? TenantCode { get; }
    string? SchemaPrefix { get; }
    bool IsResolved { get; }
}