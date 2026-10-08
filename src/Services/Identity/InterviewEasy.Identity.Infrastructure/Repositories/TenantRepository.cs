using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public sealed class TenantRepository : GenericRepository<Tenant>, ITenantRepository
{
    public TenantRepository(IdentityDbContext db, ILogger<TenantRepository> logger)
        : base(db, logger) { }

    public async Task<Tenant?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        try
        {
            var normalized = code.Trim().ToLowerInvariant();
            return await Set.FirstOrDefaultAsync(t => t.Code == normalized, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving tenant by code {TenantCode}", code);
            throw;
        }
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
    {
        try
        {
            var normalized = code.Trim().ToLowerInvariant();
            return await Set.AnyAsync(t => t.Code == normalized, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking tenant code existence for {TenantCode}", code);
            throw;
        }
    }
}
