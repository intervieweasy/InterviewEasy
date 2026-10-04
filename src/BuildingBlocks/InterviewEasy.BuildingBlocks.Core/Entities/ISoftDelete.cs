namespace InterviewEasy.BuildingBlocks.Core.Entities;

/// <summary>
/// Marker interface for entities that support soft deletion.
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    Guid? DeletedBy { get; }
}