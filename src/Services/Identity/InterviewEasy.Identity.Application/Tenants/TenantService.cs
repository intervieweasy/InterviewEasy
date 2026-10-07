using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Common.Dtos;
using InterviewEasy.Identity.Application.Common.Mapping;
using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Application.Tenants;

public sealed class TenantService : ITenantService
{
    private readonly ITenantRepository _tenants;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<TenantService> _logger;

    public TenantService(
        ITenantRepository tenants,
        IUnitOfWork uow,
        ILogger<TenantService> logger)
    {
        _tenants = tenants;
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<TenantDto>> CreateAsync(
        string code, string name, Guid? createdBy = null,
        CancellationToken ct = default)
    {
        if (await _tenants.CodeExistsAsync(code, ct))
            return Result.Failure<TenantDto>(
                Error.Conflict("tenant_code_exists",
                    $"Tenant code '{code}' is already taken."));

        Tenant tenant;
        try
        {
            tenant = Tenant.Create(code, name, createdBy);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TenantDto>(
                Error.Validation("validation_failed", ex.Message));
        }

        await _tenants.AddAsync(tenant, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Tenant {TenantCode} created with id {TenantId}",
            tenant.Code, tenant.Id);

        return Result.Success(tenant.ToDto());
    }

    public async Task<Result<TenantDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByIdAsync(id, ct);
        return tenant is null
            ? Result.Failure<TenantDto>(
                Error.NotFound("tenant_not_found", $"Tenant {id} was not found."))
            : Result.Success(tenant.ToDto());
    }

    public async Task<Result<TenantDto>> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByCodeAsync(code, ct);
        return tenant is null
            ? Result.Failure<TenantDto>(
                Error.NotFound("tenant_not_found", $"Tenant '{code}' was not found."))
            : Result.Success(tenant.ToDto());
    }

    public async Task<Result<IReadOnlyList<TenantDto>>> ListAsync(CancellationToken ct = default)
    {
        var tenants = await _tenants.ListAsync(null, ct);
        return Result.Success<IReadOnlyList<TenantDto>>(tenants.ToDtos().ToList());
    }

    public async Task<Result<TenantDto>> UpdateAsync(
        Guid id, string newName, Guid? updatedBy = null,
        CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result.Failure<TenantDto>(
                Error.NotFound("tenant_not_found", $"Tenant {id} was not found."));

        try
        {
            tenant.Rename(newName, updatedBy);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TenantDto>(
                Error.Validation("validation_failed", ex.Message));
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success(tenant.ToDto());
    }

    public async Task<Result<TenantDto>> SuspendAsync(
        Guid id, string reason, Guid? updatedBy = null,
        CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result.Failure<TenantDto>(
                Error.NotFound("tenant_not_found", $"Tenant {id} was not found."));

        try
        {
            tenant.Suspend(reason, updatedBy);
        }
        catch (Exception ex)
        {
            return Result.Failure<TenantDto>(
                Error.Validation("invalid_state", ex.Message));
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success(tenant.ToDto());
    }

    public async Task<Result<TenantDto>> ActivateAsync(
        Guid id, Guid? updatedBy = null,
        CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result.Failure<TenantDto>(
                Error.NotFound("tenant_not_found", $"Tenant {id} was not found."));

        try
        {
            tenant.Activate(updatedBy);
        }
        catch (Exception ex)
        {
            return Result.Failure<TenantDto>(
                Error.Validation("invalid_state", ex.Message));
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success(tenant.ToDto());
    }

    public async Task<Result> DeleteAsync(
        Guid id, Guid? deletedBy = null,
        CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result.Failure(
                Error.NotFound("tenant_not_found", $"Tenant {id} was not found."));

        tenant.SoftDelete(deletedBy);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
