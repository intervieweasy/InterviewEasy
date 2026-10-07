using InterviewEasy.Identity.Application.Auth;
using InterviewEasy.Identity.Application.Tenants;
using InterviewEasy.Identity.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewEasy.Identity.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
