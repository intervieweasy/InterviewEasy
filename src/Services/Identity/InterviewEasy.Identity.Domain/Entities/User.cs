using InterviewEasy.BuildingBlocks.Core.Entities;
using InterviewEasy.Identity.Domain.Enums;

namespace InterviewEasy.Identity.Domain.Entities;

public sealed class User : AuditableEntity
{
    private const int MaxFailedLogins = 5;
    private const int LockoutMinutes = 15;

    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserStatus Status { get; private set; }
    public bool EmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationSentAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginCount { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    private User() { }

    public static User Create(
        Guid tenantId,
        string email,
        string fullName,
        string passwordHash,
        Guid? createdBy = null)
    {
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        var normalizedName = fullName?.Trim() ?? string.Empty;

        ValidateEmail(normalizedEmail);
        ValidateFullName(normalizedName);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException(
                "Password hash is required.", nameof(passwordHash));

        return new User
        {
            TenantId = tenantId,
            Email = normalizedEmail,
            FullName = normalizedName,
            PasswordHash = passwordHash,
            Status = UserStatus.Pending,
            EmailVerified = false,
            FailedLoginCount = 0,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };
    }

    public void VerifyEmail(string? verificationToken = null, Guid? updatedBy = null)
    {
        if (EmailVerified) return;

        if (!string.IsNullOrEmpty(EmailVerificationToken) &&
            EmailVerificationToken != verificationToken)
        {
            throw new InvalidOperationException("Invalid verification token.");
        }

        EmailVerified = true;
        Status = UserStatus.Active;
        EmailVerificationToken = null;
        EmailVerificationSentAt = null;
        Touch(updatedBy);
    }

    public void Rename(string newFullName, Guid? updatedBy = null)
    {
        var normalized = newFullName?.Trim() ?? string.Empty;
        ValidateFullName(normalized);
        FullName = normalized;
        Touch(updatedBy);
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginCount = 0;
        LockedUntil = null;
    }

    public void RecordFailedLogin()
    {
        FailedLoginCount++;
        if (FailedLoginCount >= MaxFailedLogins)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
        }
    }

    public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;

    public void Suspend(Guid? updatedBy = null)
    {
        if (Status == UserStatus.Deleted)
            throw new InvalidOperationException("Cannot suspend a deleted user.");
        Status = UserStatus.Suspended;
        Touch(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        if (Status == UserStatus.Deleted)
            throw new InvalidOperationException("Cannot activate a deleted user.");
        Status = UserStatus.Active;
        Touch(updatedBy);
    }

    public void SoftDelete(Guid? updatedBy = null)
    {
        Status = UserStatus.Deleted;
        Touch(updatedBy);
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (!System.Text.RegularExpressions.Regex.IsMatch(
            email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Email format is invalid.", nameof(email));
        if (email.Length > 255)
            throw new ArgumentException(
                "Email must be 255 characters or fewer.", nameof(email));
    }

    private static void ValidateFullName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Full name is required.", nameof(name));
        if (name.Length < 2 || name.Length > 200)
            throw new ArgumentException(
                "Full name must be 2–200 characters.", nameof(name));
    }
}
