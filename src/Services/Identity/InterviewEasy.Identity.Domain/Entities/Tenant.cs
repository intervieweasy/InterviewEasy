using InterviewEasy.BuildingBlocks.Core.Entities;
using InterviewEasy.Identity.Domain.Enums;

namespace InterviewEasy.Identity.Domain.Entities;

public sealed class Tenant : AuditableEntity
{
    private static readonly string[] ReservedCodes =
        { "shared", "public", "system", "admin", "identity" };

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string SchemaPrefix { get; private set; } = default!;
    public TenantStatus Status { get; private set; }
    public DateTime? SuspendedAt { get; private set; }
    public string? SuspendedReason { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Tenant() { }

    public static Tenant Create(string code, string name, Guid? createdBy = null)
    {
        var normalizedCode = code?.Trim().ToLowerInvariant() ?? string.Empty;
        var normalizedName = name?.Trim() ?? string.Empty;

        ValidateCode(normalizedCode);
        ValidateName(normalizedName);

        return new Tenant
        {
            Code = normalizedCode,
            Name = normalizedName,
            SchemaPrefix = $"tenant_{normalizedCode}",
            Status = TenantStatus.Active,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };
    }

    public void Rename(string newName, Guid? updatedBy = null)
    {
        var normalized = newName?.Trim() ?? string.Empty;
        ValidateName(normalized);
        Name = normalized;
        Touch(updatedBy);
    }

    public void Suspend(string reason, Guid? updatedBy = null)
    {
        if (Status == TenantStatus.Suspended) return;
        if (Status == TenantStatus.Deleted)
            throw new InvalidOperationException("Cannot suspend a deleted tenant.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500)
            throw new ArgumentException(
                "Suspension reason must be 1–500 characters.", nameof(reason));

        Status = TenantStatus.Suspended;
        SuspendedAt = DateTime.UtcNow;
        SuspendedReason = reason.Trim();
        Touch(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        if (Status == TenantStatus.Deleted)
            throw new InvalidOperationException("Cannot activate a deleted tenant.");

        Status = TenantStatus.Active;
        SuspendedAt = null;
        SuspendedReason = null;
        Touch(updatedBy);
    }

    public void SoftDelete(Guid? updatedBy = null)
    {
        Status = TenantStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
        Touch(updatedBy);
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Tenant code is required.", nameof(code));
        if (code.Length < 3 || code.Length > 50)
            throw new ArgumentException(
                "Tenant code must be 3–50 characters.", nameof(code));
        if (!System.Text.RegularExpressions.Regex.IsMatch(code, "^[a-z][a-z0-9-]*$"))
            throw new ArgumentException(
                "Tenant code must start with a letter and contain only lowercase letters, digits, and hyphens.",
                nameof(code));
        if (ReservedCodes.Contains(code))
            throw new ArgumentException(
                $"Tenant code '{code}' is reserved.", nameof(code));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name is required.", nameof(name));
        if (name.Length < 3 || name.Length > 200)
            throw new ArgumentException(
                "Tenant name must be 3–200 characters.", nameof(name));
    }
}
