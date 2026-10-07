using InterviewEasy.Identity.Application.Common.Dtos;
using InterviewEasy.Identity.Domain.Entities;

namespace InterviewEasy.Identity.Application.Common.Mapping;

public static class AuthMappingExtensions
{
    public static UserInfoDto ToUserInfoDto(
        this User user,
        Tenant tenant,
        IEnumerable<string> roles)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(tenant);

        return new UserInfoDto(
            user.Id,
            user.Email,
            user.FullName,
            tenant.Id,
            tenant.Code,
            roles.ToArray());
    }
}
