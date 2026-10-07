using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.Identity.Domain.Entities;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<IReadOnlyList<UserRole>> ListByUserAsync(Guid userId, CancellationToken ct = default);
}
