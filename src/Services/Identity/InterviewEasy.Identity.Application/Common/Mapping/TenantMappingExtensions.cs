using InterviewEasy.Identity.Application.Common.Dtos;
using InterviewEasy.Identity.Domain.Entities;

namespace InterviewEasy.Identity.Application.Common.Mapping;

public static class TenantMappingExtensions
{
    public static TenantDto ToDto(this Tenant tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        return new TenantDto(
            tenant.Id,
            tenant.Code,
            tenant.Name,
            tenant.SchemaPrefix,
            tenant.Status.ToString().ToLowerInvariant(),
            tenant.CreatedAt,
            tenant.UpdatedAt);
    }

    public static TenantSummaryDto ToSummaryDto(this Tenant tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        return new TenantSummaryDto(
            tenant.Id,
            tenant.Code,
            tenant.Name,
            tenant.Status.ToString().ToLowerInvariant());
    }

    public static IEnumerable<TenantDto> ToDtos(this IEnumerable<Tenant> tenants)
        => tenants.Select(t => t.ToDto());
}
