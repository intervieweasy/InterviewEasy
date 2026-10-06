using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace InterviewEasy.BuildingBlocks.Common.Auth;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    private string? GetClaim(params string[] types)
    {
        var principal = Principal;
        if (principal is null) return null;

        foreach (var type in types)
        {
            var value = principal.FindFirst(type)?.Value;
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }

        return null;
    }

    public Guid? UserId =>
        Guid.TryParse(GetClaim(ClaimTypes.NameIdentifier, "sub"), out var id)
            ? (Guid?)id
            : null;

    public string? Email => GetClaim(ClaimTypes.Email, "email");

    public string? FullName => GetClaim("name", ClaimTypes.Name);

    public IReadOnlyList<string> Roles =>
        Principal?.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .Distinct()
            .ToList()
        ?? new List<string>();

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => Principal?.IsInRole(role) ?? false;
}