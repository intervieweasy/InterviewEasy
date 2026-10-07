using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public sealed class TenantRepository : GenericRepository<Tenant>, ITenantRepository
{
    public TenantRepository(IdentityDbContext db) : base(db) { }

    public Task<Tenant?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var normalized = code.Trim().ToLowerInvariant();
        return Set.FirstOrDefaultAsync(t => t.Code == normalized, ct);
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
    {
        var normalized = code.Trim().ToLowerInvariant();
        return Set.AnyAsync(t => t.Code == normalized, ct);
    }
}
