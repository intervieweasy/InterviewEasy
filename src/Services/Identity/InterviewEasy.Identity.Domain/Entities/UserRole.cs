using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.Identity.Domain.Entities;

public sealed class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    private UserRole() { }

    public static UserRole Create(Guid userId, Guid roleId, Guid? assignedBy = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (roleId == Guid.Empty)
            throw new ArgumentException("RoleId is required.", nameof(roleId));

        return new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        };
    }
}
