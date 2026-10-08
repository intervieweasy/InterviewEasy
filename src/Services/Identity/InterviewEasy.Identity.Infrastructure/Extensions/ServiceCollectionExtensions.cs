using InterviewEasy.Identity.Infrastructure.Persistence;
using InterviewEasy.Identity.Infrastructure.Persistence.Interceptors;
using InterviewEasy.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using InterviewEasy.Identity.Infrastructure.Repositories;
using InterviewEasy.BuildingBlocks.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewEasy.Identity.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDb")
            ?? throw new InvalidOperationException(
                "Connection string 'IdentityDb' is not configured.");

        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "shared");
            });
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        // Use singleton so Argon2PasswordHasher shares its internal SemaphoreSlim
        // across all requests to throttle CPU-intensive hashing.
        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // Repositories & Unit of Work
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
