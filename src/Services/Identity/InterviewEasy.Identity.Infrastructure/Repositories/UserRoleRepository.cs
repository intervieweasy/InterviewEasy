using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public sealed class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(IdentityDbContext db, ILogger<UserRoleRepository> logger)
        : base(db, logger) { }

    public async Task<IReadOnlyList<UserRole>> ListByUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            return await Set.Where(ur => ur.UserId == userId).ToListAsync(ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing user roles for user {UserId}", userId);
            throw;
        }
    }
}
