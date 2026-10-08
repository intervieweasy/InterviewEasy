using System.Linq.Expressions;
using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.BuildingBlocks.Core.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly IdentityDbContext Db;
    protected readonly DbSet<T> Set;
    protected readonly ILogger<GenericRepository<T>> Logger;

    public GenericRepository(IdentityDbContext db, ILogger<GenericRepository<T>> logger)
    {
        Db = db;
        Set = db.Set<T>();
        Logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            return await Set.FirstOrDefaultAsync(e => e.Id == id, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving {EntityType} by id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        try
        {
            return await Set.FirstOrDefaultAsync(predicate, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error querying {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        try
        {
            var query = predicate is null ? Set : Set.Where(predicate);
            return await query.ToListAsync(ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        try
        {
            return await Set.AnyAsync(predicate, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking existence for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        try
        {
            return predicate is null ? await Set.CountAsync(ct) : await Set.CountAsync(predicate, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            await Set.AddAsync(entity, ct);
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            Set.Update(entity);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            Set.Remove(entity);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual IQueryable<T> Query() => Set.AsQueryable();
}
