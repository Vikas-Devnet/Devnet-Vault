using Application.Features.Account.Interfaces;
using Application.Features.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Cache;
using Infrastructure.Email;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.GetConnectionString("Redis");
            options.InstanceName = "AppInstance_";
        });

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Security
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddSingleton<IEmailQueue, EmailQueue>();
        services.AddSingleton<EmailService>();

        services.AddHostedService(provider => (EmailQueue)provider.GetRequiredService<IEmailQueue>());

        return services;
    }
}
