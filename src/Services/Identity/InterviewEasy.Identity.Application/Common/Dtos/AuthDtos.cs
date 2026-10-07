namespace InterviewEasy.Identity.Application.Common.Dtos;

public sealed record LoginResultDto(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UserInfoDto User);

public sealed record UserInfoDto(
    Guid Id,
    string Email,
    string FullName,
    Guid TenantId,
    string TenantCode,
    string[] Roles);
