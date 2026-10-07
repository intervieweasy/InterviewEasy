namespace InterviewEasy.Identity.Application.Common.Dtos;

public sealed record TenantDto(
    Guid Id,
    string Code,
    string Name,
    string SchemaPrefix,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record TenantSummaryDto(
    Guid Id,
    string Code,
    string Name,
    string Status);
