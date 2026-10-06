using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.Identity.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedBy { get; private set; }
    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime? LastUsedAt { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(
        Guid userId,
        Guid tenantId,
        string tokenHash,
        TimeSpan lifetime,
        string? userAgent = null,
        string? ipAddress = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("TokenHash is required.", nameof(tokenHash));

        return new RefreshToken
        {
            UserId = userId,
            TenantId = tenantId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.Add(lifetime),
            CreatedAt = DateTime.UtcNow,
            UserAgent = userAgent,
            IpAddress = ipAddress
        };
    }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    public void Revoke(Guid? replacedBy = null)
    {
        if (RevokedAt.HasValue) return;
        RevokedAt = DateTime.UtcNow;
        ReplacedBy = replacedBy;
    }

    public void MarkUsed()
    {
        LastUsedAt = DateTime.UtcNow;
    }
}
