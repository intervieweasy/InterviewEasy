using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.Identity.Domain.Entities;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public interface ITenantRepository : IGenericRepository<Tenant>
{
    Task<Tenant?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}
