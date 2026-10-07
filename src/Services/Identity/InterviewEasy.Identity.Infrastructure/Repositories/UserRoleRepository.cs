using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public sealed class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(IdentityDbContext db) : base(db) { }

    public async Task<IReadOnlyList<UserRole>> ListByUserAsync(Guid userId, CancellationToken ct = default)
    {
        return await Set.Where(ur => ur.UserId == userId).ToListAsync(ct);
    }
}
