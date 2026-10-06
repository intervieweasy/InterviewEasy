using InterviewEasy.BuildingBlocks.Core.Entities;
using InterviewEasy.Identity.Domain.Enums;

namespace InterviewEasy.Identity.Domain.Entities;

public sealed class Role : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public RoleType Type { get; private set; }
    public bool IsSystem { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Role() { }

    public static Role Create(
        string name,
        string displayName,
        RoleType type,
        bool isSystem = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException(
                "Display name is required.", nameof(displayName));

        return new Role
        {
            Name = name.Trim().ToLowerInvariant(),
            DisplayName = displayName.Trim(),
            Type = type,
            IsSystem = isSystem,
            CreatedAt = DateTime.UtcNow
        };
    }
}
