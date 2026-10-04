namespace InterviewEasy.BuildingBlocks.Core.Exceptions;

/// <summary>
/// Thrown when an entity is not found. Maps to HTTP 404.
/// </summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string resource, object key)
        : base("not_found", $"{resource} with id '{key}' was not found.")
    {
    }

    public NotFoundException(string message)
        : base("not_found", message)
    {
    }
}