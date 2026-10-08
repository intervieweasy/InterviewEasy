using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService service, ILogger<AuthController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public sealed record LoginRequest(string TenantCode, string Email, string Password);

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.LoginAsync(
                request.TenantCode, request.Email, request.Password, ct);

            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error during login for {Email} in tenant {TenantCode}", request.Email, request.TenantCode);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("login_failed", "An unexpected error occurred while signing in."), 500));
        }
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        try
        {
            var sub = User.FindFirst("sub")?.Value
                   ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(sub, out var userId))
                return Unauthorized();

            var result = await _service.GetCurrentUserAsync(userId, ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while getting current user");
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("current_user_failed", "An unexpected error occurred while loading the current user."), 500));
        }
    }

    private IActionResult ToProblem(Error error) => error.Type switch
    {
        ErrorType.Validation => UnprocessableEntity(Problem(error, 422)),
        ErrorType.NotFound => NotFound(Problem(error, 404)),
        ErrorType.Conflict => Conflict(Problem(error, 409)),
        ErrorType.Unauthorized => Unauthorized(Problem(error, 401)),
        ErrorType.Forbidden => StatusCode(403, Problem(error, 403)),
        _ => StatusCode(500, Problem(error, 500))
    };

    private static object Problem(Error error, int status) => new
    {
        type = $"https://api.intervieweasy.com/errors/{error.Code}",
        title = error.Code,
        status,
        detail = error.Message
    };
}
