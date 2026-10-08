using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService service, ILogger<UsersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public sealed record InviteUserRequest(Guid TenantId, string Email, string FullName, string InitialPassword);

    [HttpPost("invite")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Invite([FromBody] InviteUserRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.InviteAsync(
                request.TenantId, request.Email, request.FullName,
                request.InitialPassword, ct: ct);

            if (result.IsFailure) return ToProblem(result.Error);

            return CreatedAtAction(
                nameof(GetById), new { id = result.Value.Id }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while inviting user {Email} for tenant {TenantId}", request.Email, request.TenantId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("user_invite_failed", "An unexpected error occurred while inviting the user."), 500));
        }
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while retrieving user {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("user_get_failed", "An unexpected error occurred while retrieving the user."), 500));
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListByTenant([FromQuery] Guid tenantId, CancellationToken ct)
    {
        try
        {
            var result = await _service.ListByTenantAsync(tenantId, ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while listing users for tenant {TenantId}", tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("user_list_failed", "An unexpected error occurred while listing users."), 500));
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
