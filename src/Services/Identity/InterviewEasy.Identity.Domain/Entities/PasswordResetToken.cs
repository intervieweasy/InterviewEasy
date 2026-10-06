using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.Identity.Domain.Entities;

public sealed class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public string? RequestIp { get; private set; }
    public string? RequestUserAgent { get; private set; }

    private PasswordResetToken() { }

    public static PasswordResetToken Create(
        Guid userId,
        Guid tenantId,
        string tokenHash,
        TimeSpan lifetime,
        string? requestIp = null,
        string? userAgent = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("TokenHash is required.", nameof(tokenHash));

        return new PasswordResetToken
        {
            UserId = userId,
            TenantId = tenantId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.Add(lifetime),
            CreatedAt = DateTime.UtcNow,
            RequestIp = requestIp,
            RequestUserAgent = userAgent
        };
    }

    public bool IsActive => UsedAt is null && ExpiresAt > DateTime.UtcNow;

    public void MarkUsed()
    {
        UsedAt = DateTime.UtcNow;
    }
}
