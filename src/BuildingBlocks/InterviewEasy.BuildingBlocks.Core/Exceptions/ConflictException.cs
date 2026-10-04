namespace InterviewEasy.BuildingBlocks.Core.Exceptions;

/// <summary>
/// Thrown when an operation conflicts with existing state. Maps to HTTP 409.
/// </summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string message)
        : base("conflict", message)
    {
    }
}