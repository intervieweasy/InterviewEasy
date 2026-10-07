using InterviewEasy.BuildingBlocks.Common.Abstractions;
using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Common.Dtos;
using InterviewEasy.Identity.Application.Common.Mapping;
using InterviewEasy.Identity.Domain.Enums;
using InterviewEasy.Identity.Infrastructure.Repositories;
using InterviewEasy.Identity.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITenantRepository _tenants;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository users,
        ITenantRepository tenants,
        IPasswordHasher hasher,
        IJwtTokenService jwt,
        IUnitOfWork uow,
        ILogger<AuthService> logger)
    {
        _users = users;
        _tenants = tenants;
        _hasher = hasher;
        _jwt = jwt;
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<LoginResultDto>> LoginAsync(
        string tenantCode, string email, string password,
        CancellationToken ct = default)
    {
        var tenant = await _tenants.GetByCodeAsync(tenantCode, ct);
        if (tenant is null)
            return Result.Failure<LoginResultDto>(
                Error.Unauthorized("invalid_credentials",
                    "Invalid credentials."));

        if (tenant.Status == TenantStatus.Suspended)
            return Result.Failure<LoginResultDto>(
                Error.Forbidden("tenant_suspended",
                    "Your organization's account is suspended."));

        var user = await _users.GetByEmailAsync(tenant.Id, email, ct);
        if (user is null)
            return Result.Failure<LoginResultDto>(
                Error.Unauthorized("invalid_credentials",
                    "Invalid credentials."));

        if (user.IsLocked)
            return Result.Failure<LoginResultDto>(
                Error.Unauthorized("account_locked",
                    "Account is locked. Try again later."));

        if (user.Status != UserStatus.Active)
            return Result.Failure<LoginResultDto>(
                Error.Unauthorized("account_not_active",
                    "Account is not active."));

        if (!_hasher.Verify(password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _uow.SaveChangesAsync(ct);

            _logger.LogWarning(
                "Failed login for {Email} in tenant {TenantCode}",
                email, tenantCode);

            return Result.Failure<LoginResultDto>(
                Error.Unauthorized("invalid_credentials",
                    "Invalid credentials."));
        }

        user.RecordLogin();
        await _uow.SaveChangesAsync(ct);

        var roles = new[] { "client_admin" };
        var accessToken = _jwt.IssueAccessToken(
            user.Id, tenant.Id, tenant.Code,
            user.Email, user.FullName, roles);

        _logger.LogInformation(
            "User {UserId} logged in to tenant {TenantCode}",
            user.Id, tenantCode);

        return Result.Success(new LoginResultDto(
            accessToken,
            "Bearer",
            15 * 60,
            user.ToUserInfoDto(tenant, roles)));
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(
        Guid userId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        return user is null
            ? Result.Failure<UserDto>(
                Error.NotFound("user_not_found", $"User {userId} was not found."))
            : Result.Success(user.ToDto());
    }
}
