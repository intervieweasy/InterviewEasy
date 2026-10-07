using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Common.Dtos;
using InterviewEasy.Identity.Application.Common.Mapping;
using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Repositories;
using InterviewEasy.Identity.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Application.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly ITenantRepository _tenants;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository users,
        ITenantRepository tenants,
        IUnitOfWork uow,
        IPasswordHasher hasher,
        ILogger<UserService> logger)
    {
        _users = users;
        _tenants = tenants;
        _uow = uow;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task<Result<UserDto>> InviteAsync(
        Guid tenantId, string email, string fullName,
        string initialPassword, Guid? invitedBy = null,
        CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByIdAsync(tenantId, ct);
        if (tenant is null)
            return Result.Failure<UserDto>(
                Error.NotFound("tenant_not_found",
                    $"Tenant {tenantId} was not found."));

        if (await _users.EmailExistsAsync(tenantId, email, ct))
            return Result.Failure<UserDto>(
                Error.Conflict("user_email_exists",
                    $"User with email '{email}' already exists in this tenant."));

        User user;
        try
        {
            var hash = _hasher.Hash(initialPassword);
            user = User.Create(tenantId, email, fullName, hash, invitedBy);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<UserDto>(
                Error.Validation("validation_failed", ex.Message));
        }

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "User {Email} invited to tenant {TenantId}",
            user.Email, tenantId);

        return Result.Success(user.ToDto());
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(id, ct);
        return user is null
            ? Result.Failure<UserDto>(
                Error.NotFound("user_not_found", $"User {id} was not found."))
            : Result.Success(user.ToDto());
    }

    public async Task<Result<IReadOnlyList<UserDto>>> ListByTenantAsync(
        Guid tenantId, CancellationToken ct = default)
    {
        var users = await _users.ListByTenantAsync(tenantId, ct);
        return Result.Success<IReadOnlyList<UserDto>>(users.ToDtos().ToList());
    }
}
