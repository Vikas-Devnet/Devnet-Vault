using Application.Features.Account.Services;
using Application.Features.Common.Interfaces;
using Application.Features.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<IUtilitiesService, UtilitiesService>();

        return services;
    }
}
