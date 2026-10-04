using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.BuildingBlocks.Core.Abstractions;

/// <summary>
/// Generic repository interface for aggregate roots.
/// Implementations live in each service's Infrastructure layer.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}