using System.Linq.Expressions;
using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.BuildingBlocks.Core.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly IdentityDbContext Db;
    protected readonly DbSet<T> Set;

    public GenericRepository(IdentityDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(e => e.Id == id, ct);

    public virtual Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(predicate, ct);

    public virtual async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        var query = predicate is null ? Set : Set.Where(predicate);
        return await query.ToListAsync(ct);
    }

    public virtual Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
        => Set.AnyAsync(predicate, ct);

    public virtual Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
        => predicate is null ? Set.CountAsync(ct) : Set.CountAsync(predicate, ct);

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await Set.AddAsync(entity, ct);
        return entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Set.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        Set.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual IQueryable<T> Query() => Set.AsQueryable();
}
