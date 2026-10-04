namespace InterviewEasy.BuildingBlocks.Core.Abstractions;

/// <summary>
/// Unit of Work pattern — commits all changes made in a transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}