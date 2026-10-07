using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.Identity.Domain.Entities;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken ct = default);
    Task<IReadOnlyList<User>> ListByTenantAsync(Guid tenantId, CancellationToken ct = default);
}
