using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.Identity.Domain.Entities;

public sealed class Permission : BaseEntity
{
    public string Code { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Permission() { }

    public static Permission Create(string code, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Permission code is required.", nameof(code));
        if (!System.Text.RegularExpressions.Regex.IsMatch(
            code, @"^[a-z]+:[a-z\-]+$"))
            throw new ArgumentException(
                "Permission code must match 'resource:action' format.",
                nameof(code));

        return new Permission
        {
            Code = code.Trim().ToLowerInvariant(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
