using Huntly.Application.Auth.Interfaces;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Auth.Entities;
using Huntly.Core.Jobs.Repositories;
using Huntly.Infra.Persistence;
using Huntly.Infra.Persistence.Context;
using Huntly.Infra.Persistence.Repositories.Jobs;
using Huntly.Infra.Security;
using Huntly.Infra.Security.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Huntly.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. DB
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention());

        // 2. Identity
        // 2.1 Identity Password Hasher
        services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher>();
        // 2.2 Identity Core
        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>();

        // 3. Options
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<AuthOptions>(configuration.GetSection("Auth"));

        // 4. Repositories
        services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
        services.AddScoped<IAtomicWork, AtomicWork>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}