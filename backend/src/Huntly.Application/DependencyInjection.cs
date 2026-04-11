using System.Reflection;
using FluentValidation;
using Huntly.Application.Shared.Behaviors;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Huntly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.NotificationPublisherType = typeof(ForeachAwaitPublisher);
        });
        
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}