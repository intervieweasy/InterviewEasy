using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Common.Dtos;

namespace InterviewEasy.Identity.Application.Auth;

public interface IAuthService
{
    Task<Result<LoginResultDto>> LoginAsync(
        string tenantCode, string email, string password,
        CancellationToken ct = default);

    Task<Result<UserDto>> GetCurrentUserAsync(
        Guid userId, CancellationToken ct = default);
}
