using InterviewEasy.Identity.Domain.Entities;
using DomainUser = InterviewEasy.Identity.Domain.Entities.User;
using InterviewEasy.Identity.Infrastructure.Persistence;
using InterviewEasy.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IdentityDbContext _db;
    private readonly IPasswordHasher _hasher;

    public UsersController(IdentityDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public sealed record InviteUserRequest(
        Guid TenantId, string Email, string FullName, string InitialPassword);

    public sealed record UserResponse(
        Guid Id, Guid TenantId, string Email, string FullName,
        string Status, bool EmailVerified, DateTime CreatedAt);

    [HttpPost("invite")]
    [AllowAnonymous]
    public async Task<IActionResult> Invite([FromBody] InviteUserRequest request, CancellationToken ct)
    {
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, ct);
        if (tenant is null) return NotFound(new { error = "tenant_not_found" });

        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await _db.Users
            .AnyAsync(u => u.TenantId == request.TenantId && u.Email == email, ct);
        if (exists) return Conflict(new { error = "user_email_exists" });

        User user;
        try
        {
            var hash = _hasher.Hash(request.InitialPassword);
            user = DomainUser.Create(request.TenantId, email, request.FullName, hash);
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new { error = "validation_failed", message = ex.Message });
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, Map(user));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return NotFound();
        return Ok(Map(user));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListByTenant(
        [FromQuery] Guid tenantId,
        CancellationToken ct)
    {
        var users = await _db.Users
            .Where(u => u.TenantId == tenantId)
            .OrderByDescending(u => u.CreatedAt)
            .Take(200)
            .ToListAsync(ct);

        return Ok(users.Select(Map));
    }

    private static UserResponse Map(DomainUser u) => new(
        u.Id, u.TenantId, u.Email, u.FullName,
        u.Status.ToString().ToLowerInvariant(),
        u.EmailVerified, u.CreatedAt);
}
