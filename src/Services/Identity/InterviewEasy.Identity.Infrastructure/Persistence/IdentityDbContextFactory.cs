using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InterviewEasy.Identity.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by `dotnet ef migrations` CLI.
/// Not used at runtime.
/// </summary>
public sealed class IdentityDbContextFactory
    : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("IDENTITY_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=intervieweasy_dev;" +
               "Username=intervieweasy;Password=dev_password";

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable(
                    "__ef_migrations_history", "shared");
            })
            .Options;

        return new IdentityDbContext(options);
    }
}
