using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(IdentityDbContext db, ILogger<UnitOfWork> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving identity database changes");
            throw;
        }
    }
}
