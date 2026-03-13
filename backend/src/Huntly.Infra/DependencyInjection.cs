using Huntly.Application.Auth.Interfaces;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Auth.Entities;
using Huntly.Core.Jobs.Repositories;
using Huntly.Infra.Persistence;
using Huntly.Infra.Persistence.Context;
using Huntly.Infra.Persistence.Repositories.Jobs;
using Huntly.Infra.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Huntly.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention());

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
        services.AddScoped<IAtomicWork, AtomicWork>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}