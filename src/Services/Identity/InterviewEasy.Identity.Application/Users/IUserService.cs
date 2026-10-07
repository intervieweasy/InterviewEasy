using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Common.Dtos;

namespace InterviewEasy.Identity.Application.Users;

public interface IUserService
{
    Task<Result<UserDto>> InviteAsync(
        Guid tenantId, string email, string fullName,
        string initialPassword, Guid? invitedBy = null,
        CancellationToken ct = default);

    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<UserDto>>> ListByTenantAsync(Guid tenantId, CancellationToken ct = default);
}
