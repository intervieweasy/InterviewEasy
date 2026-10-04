namespace InterviewEasy.BuildingBlocks.Core.Entities;

/// <summary>
/// Base class for entities that track creation and modification metadata.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    /// <summary>
    /// Updates the audit metadata. Called by SaveChanges interceptor or
    /// explicitly by the domain when the entity is modified.
    /// </summary>
    protected void Touch(Guid? actor = null)
    {
        UpdatedAt = DateTime.UtcNow;
        if (actor.HasValue) UpdatedBy = actor;
    }
}