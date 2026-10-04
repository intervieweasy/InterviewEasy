namespace InterviewEasy.BuildingBlocks.Core.Exceptions;

/// <summary>
/// Thrown when input validation fails. Maps to HTTP 422.
/// </summary>
public sealed class ValidationException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string message)
        : base("validation_failed", message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("validation_failed", "One or more validation errors occurred.")
    {
        Errors = errors;
    }
}