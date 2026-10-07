namespace InterviewEasy.Identity.Application.Common.Dtos;

public sealed record UserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string FullName,
    string Status,
    bool EmailVerified,
    DateTime? LastLoginAt,
    DateTime CreatedAt);

public sealed record UserSummaryDto(
    Guid Id,
    string Email,
    string FullName,
    string Status);
