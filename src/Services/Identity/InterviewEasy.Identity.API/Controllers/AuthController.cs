using InterviewEasy.Identity.Domain.Enums;
using InterviewEasy.Identity.Infrastructure.Persistence;
using InterviewEasy.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IdentityDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthController(
        IdentityDbContext db,
        IPasswordHasher hasher,
        IJwtTokenService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    public sealed record LoginRequest(string TenantCode, string Email, string Password);

    public sealed record LoginResponse(
        string AccessToken, string TokenType, int ExpiresIn,
        UserInfo User);

    public sealed record UserInfo(
        Guid Id, string Email, string FullName,
        Guid TenantId, string TenantCode, string[] Roles);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var tenantCode = request.TenantCode.Trim().ToLowerInvariant();
        var email = request.Email.Trim().ToLowerInvariant();

        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Code == tenantCode, ct);
        if (tenant is null) return Unauthorized(new { error = "invalid_credentials" });

        if (tenant.Status == TenantStatus.Suspended)
            return StatusCode(403, new { error = "tenant_suspended" });

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenant.Id && u.Email == email, ct);
        if (user is null) return Unauthorized(new { error = "invalid_credentials" });

        if (user.IsLocked)
            return Unauthorized(new { error = "account_locked" });

        if (user.Status != UserStatus.Active)
            return Unauthorized(new { error = "account_not_active" });

        if (!_hasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _db.SaveChangesAsync(ct);
            return Unauthorized(new { error = "invalid_credentials" });
        }

        user.RecordLogin();
        await _db.SaveChangesAsync(ct);

        var roles = new[] { "client_admin" }; // wired to actual roles later
        var accessToken = _jwt.IssueAccessToken(
            user.Id, tenant.Id, tenant.Code,
            user.Email, user.FullName, roles);

        return Ok(new LoginResponse(
            accessToken, "Bearer", 15 * 60,
            new UserInfo(user.Id, user.Email, user.FullName,
                tenant.Id, tenant.Code, roles)));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var sub = User.FindFirst("sub")?.Value
               ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var userId))
            return Unauthorized();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Status,
            user.EmailVerified,
            user.CreatedAt,
            user.LastLoginAt
        });
    }
}
