using InterviewEasy.BuildingBlocks.Common.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewEasy.BuildingBlocks.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ITenantContext, TenantContext>();
        return services;
    }
}