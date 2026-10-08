using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IdentityDbContext db, ILogger<UserRepository> logger)
        : base(db, logger) { }

    public async Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default)
    {
        try
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await Set.FirstOrDefaultAsync(
                u => u.TenantId == tenantId && u.Email == normalized, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving user by email {Email} for tenant {TenantId}", email, tenantId);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken ct = default)
    {
        try
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await Set.AnyAsync(u => u.TenantId == tenantId && u.Email == normalized, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking email existence for {Email} in tenant {TenantId}", email, tenantId);
            throw;
        }
    }

    public async Task<IReadOnlyList<User>> ListByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            return await Set
                .Where(u => u.TenantId == tenantId)
                .OrderByDescending(u => u.CreatedAt)
                .Take(200)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing users for tenant {TenantId}", tenantId);
            throw;
        }
    }
}
