using InterviewEasy.BuildingBlocks.Common.Results;
using InterviewEasy.Identity.Application.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewEasy.Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class TenantsController : ControllerBase
{
    private readonly ITenantService _service;

    public TenantsController(ITenantService service)
    {
        _service = service;
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
        var result = await _service.CreateAsync(request.Code, request.Name, ct: ct);
        if (result.IsFailure) return ToProblem(result.Error);
        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpGet("by-code/{code}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        var result = await _service.GetByCodeAsync(code, ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _service.ListAsync(ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
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
        var result = await _service.UpdateAsync(id, request.Name, ct: ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
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
        var result = await _service.SuspendAsync(id, request.Reason, ct: ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpPost("{id:guid}/activate")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        var result = await _service.ActivateAsync(id, ct: ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct: ct);
        return result.IsSuccess ? NoContent() : ToProblem(result.Error);
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
