using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InterviewEasy.Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class TenantsController : ControllerBase
{
    private readonly ITenantService _service;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(ITenantService service, ILogger<TenantsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public sealed record CreateTenantRequest(string Code, string Name);
    public sealed record UpdateTenantRequest(string Name);
    public sealed record SuspendTenantRequest(string Reason);

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.CreateAsync(request.Code, request.Name, ct: ct);
            if (result.IsFailure) return ToProblem(result.Error);
            return StatusCode(StatusCodes.Status201Created, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while creating tenant {TenantCode}", request.Code);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_create_failed", "An unexpected error occurred while creating the tenant."), 500));
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
            _logger.LogError(ex, "Unhandled error while retrieving tenant {TenantId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_get_failed", "An unexpected error occurred while retrieving the tenant."), 500));
        }
    }

    [HttpGet("by-code/{code}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetByCodeAsync(code, ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while retrieving tenant by code {TenantCode}", code);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_get_by_code_failed", "An unexpected error occurred while retrieving the tenant."), 500));
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        try
        {
            var result = await _service.ListAsync(ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while listing tenants");
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_list_failed", "An unexpected error occurred while listing tenants."), 500));
        }
    }

    [HttpPut("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request.Name, ct: ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while updating tenant {TenantId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_update_failed", "An unexpected error occurred while updating the tenant."), 500));
        }
    }

    [HttpPost("{id:guid}/suspend")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Suspend(
        Guid id,
        [FromBody] SuspendTenantRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.SuspendAsync(id, request.Reason, ct: ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while suspending tenant {TenantId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_suspend_failed", "An unexpected error occurred while suspending the tenant."), 500));
        }
    }

    [HttpPost("{id:guid}/activate")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.ActivateAsync(id, ct: ct);
            return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while activating tenant {TenantId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_activate_failed", "An unexpected error occurred while activating the tenant."), 500));
        }
    }

    [HttpDelete("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.DeleteAsync(id, ct: ct);
            return result.IsSuccess ? NoContent() : ToProblem(result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while deleting tenant {TenantId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(Error.Unexpected("tenant_delete_failed", "An unexpected error occurred while deleting the tenant."), 500));
        }
    }

    // --- Mapping Error to HTTP ---

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
