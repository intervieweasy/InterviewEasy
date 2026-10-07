namespace InterviewEasy.Identity.Infrastructure.Services;

public interface IJwtTokenService
{
    string IssueAccessToken(
        Guid userId,
        Guid tenantId,
        string tenantCode,
        string email,
        string fullName,
        IEnumerable<string> roles);

    string GenerateRefreshToken();
    string HashRefreshToken(string rawToken);
}
