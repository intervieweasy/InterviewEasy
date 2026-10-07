using InterviewEasy.Identity.Application.Common.Dtos;
using InterviewEasy.Identity.Domain.Entities;

namespace InterviewEasy.Identity.Application.Common.Mapping;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto(
            user.Id,
            user.TenantId,
            user.Email,
            user.FullName,
            user.Status.ToString().ToLowerInvariant(),
            user.EmailVerified,
            user.LastLoginAt,
            user.CreatedAt);
    }

    public static UserSummaryDto ToSummaryDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserSummaryDto(
            user.Id,
            user.Email,
            user.FullName,
            user.Status.ToString().ToLowerInvariant());
    }

    public static IEnumerable<UserDto> ToDtos(this IEnumerable<User> users)
        => users.Select(u => u.ToDto());
}
