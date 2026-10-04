namespace InterviewEasy.BuildingBlocks.Core.Exceptions;

/// <summary>
/// Base class for all domain-specific exceptions.
/// Maps to HTTP 400 or 422 in the API layer.
/// </summary>
public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public DomainException(string code, string message, Exception inner)
        : base(message, inner)
    {
        Code = code;
    }
}