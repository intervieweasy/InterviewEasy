using InterviewEasy.BuildingBlocks.Common.Auth;
using InterviewEasy.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext : DbContext
{
    public const string SharedSchema = "shared";

    private readonly ITenantContext? _tenantContext;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        ITenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Shared schema is the default. Tenant-scoped tables would
        // use _tenantContext.SchemaPrefix — applied when services
        // start having per-tenant tables.
        modelBuilder.HasDefaultSchema(SharedSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
