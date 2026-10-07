using InterviewEasy.Identity.Domain.Entities;
using InterviewEasy.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class TenantsController : ControllerBase
{
    private readonly IdentityDbContext _db;

    public TenantsController(IdentityDbContext db)
    {
        _db = db;
    }

    public sealed record CreateTenantRequest(string Code, string Name);
    public sealed record TenantResponse(
        Guid Id, string Code, string Name, string SchemaPrefix,
        string Status, DateTime CreatedAt, DateTime UpdatedAt);

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "code_and_name_required" });

        var exists = await _db.Tenants
            .AnyAsync(t => t.Code == request.Code.ToLowerInvariant(), ct);

        if (exists)
            return Conflict(new { error = "tenant_code_exists" });

        Tenant tenant;
        try
        {
            tenant = Tenant.Create(request.Code, request.Name);
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new { error = "validation_failed", message = ex.Message });
        }

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, Map(tenant));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tenant is null) return NotFound();
        return Ok(Map(tenant));
    }

    [HttpGet("by-code/{code}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        var normalized = code.Trim().ToLowerInvariant();
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Code == normalized, ct);
        if (tenant is null) return NotFound();
        return Ok(Map(tenant));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var tenants = await _db.Tenants
            .OrderByDescending(t => t.CreatedAt)
            .Take(100)
            .ToListAsync(ct);

        return Ok(tenants.Select(Map));
    }

    public sealed record UpdateTenantRequest(string Name);

    [HttpPut("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tenant is null) return NotFound();

        try
        {
            tenant.Rename(request.Name);
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new { error = "validation_failed", message = ex.Message });
        }

        await _db.SaveChangesAsync(ct);
        return Ok(Map(tenant));
    }

    public sealed record SuspendTenantRequest(string Reason);

    [HttpPost("{id:guid}/suspend")]
    [AllowAnonymous]
    public async Task<IActionResult> Suspend(
        Guid id,
        [FromBody] SuspendTenantRequest request,
        CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tenant is null) return NotFound();

        try
        {
            tenant.Suspend(request.Reason);
        }
        catch (Exception ex)
        {
            return UnprocessableEntity(new { error = "invalid_state", message = ex.Message });
        }

        await _db.SaveChangesAsync(ct);
        return Ok(Map(tenant));
    }

    [HttpPost("{id:guid}/activate")]
    [AllowAnonymous]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tenant is null) return NotFound();

        try
        {
            tenant.Activate();
        }
        catch (Exception ex)
        {
            return UnprocessableEntity(new { error = "invalid_state", message = ex.Message });
        }

        await _db.SaveChangesAsync(ct);
        return Ok(Map(tenant));
    }

    [HttpDelete("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tenant is null) return NotFound();

        tenant.SoftDelete();
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static TenantResponse Map(Tenant t) => new(
        t.Id, t.Code, t.Name, t.SchemaPrefix,
        t.Status.ToString().ToLowerInvariant(),
        t.CreatedAt, t.UpdatedAt);
}
