using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IdentityDbContext db) : base(db) { }

    public Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return Set.FirstOrDefaultAsync(
            u => u.TenantId == tenantId && u.Email == normalized, ct);
    }

    public Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return Set.AnyAsync(u => u.TenantId == tenantId && u.Email == normalized, ct);
    }

    public async Task<IReadOnlyList<User>> ListByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await Set
            .Where(u => u.TenantId == tenantId)
            .OrderByDescending(u => u.CreatedAt)
            .Take(200)
            .ToListAsync(ct);
    }
}
