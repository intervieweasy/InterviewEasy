using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InterviewEasy.BuildingBlocks.Common.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var (status, code, message, errors) = MapException(ex);

        if (status >= 500)
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
        else
            _logger.LogWarning(ex, "Handled domain exception: {Message}", ex.Message);

        var correlationId = context.Items["CorrelationId"]?.ToString();

        var problem = new
        {
            type = $"https://api.intervieweasy.com/errors/{code}",
            title = GetTitle(status),
            status,
            detail = message,
            instance = context.Request.Path.Value,
            traceId = correlationId,
            errors
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private static (int status, string code, string message, object? errors) MapException(Exception ex)
    {
        return ex switch
        {
            InterviewEasy.BuildingBlocks.Core.Exceptions.ValidationException ve => (422, "validation_failed", ve.Message, ve.Errors),
            InterviewEasy.BuildingBlocks.Core.Exceptions.NotFoundException nf => (404, "not_found", nf.Message, null),
            InterviewEasy.BuildingBlocks.Core.Exceptions.ConflictException cf => (409, "conflict", cf.Message, null),
            InterviewEasy.BuildingBlocks.Core.Exceptions.DomainException de => (400, "domain_error", de.Message, null),
            UnauthorizedAccessException => (401, "unauthorized", "Unauthorized.", null),
            ArgumentException ae => (422, "invalid_argument", ae.Message, null),
            _ => (500, "internal_error", "An unexpected error occurred.", null)
        };
    }

    private static string GetTitle(int status) => status switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        422 => "Validation Failed",
        500 => "Internal Server Error",
        _ => "Error"
    };
}