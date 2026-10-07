using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Common.Dtos;

namespace InterviewEasy.Identity.Application.Tenants;

public interface ITenantService
{
    Task<Result<TenantDto>> CreateAsync(
        string code, string name, Guid? createdBy = null,
        CancellationToken ct = default);

    Task<Result<TenantDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<TenantDto>> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Result<IReadOnlyList<TenantDto>>> ListAsync(CancellationToken ct = default);
    Task<Result<TenantDto>> UpdateAsync(Guid id, string newName, Guid? updatedBy = null, CancellationToken ct = default);
    Task<Result<TenantDto>> SuspendAsync(Guid id, string reason, Guid? updatedBy = null, CancellationToken ct = default);
    Task<Result<TenantDto>> ActivateAsync(Guid id, Guid? updatedBy = null, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, Guid? deletedBy = null, CancellationToken ct = default);
}
