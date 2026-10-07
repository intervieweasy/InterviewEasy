using InterviewEasy.BuildingBlocks.Common.Auth;
using InterviewEasy.BuildingBlocks.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InterviewEasy.Identity.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser? _currentUser;

    public AuditableEntityInterceptor(ICurrentUser? currentUser = null)
    {
        _currentUser = currentUser;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;
        var actor = _currentUser?.UserId;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(AuditableEntity.CreatedAt)).CurrentValue = now;
                    entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = now;
                    if (actor.HasValue)
                    {
                        entry.Property(nameof(AuditableEntity.CreatedBy)).CurrentValue = actor;
                        entry.Property(nameof(AuditableEntity.UpdatedBy)).CurrentValue = actor;
                    }
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = now;
                    if (actor.HasValue)
                    {
                        entry.Property(nameof(AuditableEntity.UpdatedBy)).CurrentValue = actor;
                    }
                    break;
            }
        }
    }
}
