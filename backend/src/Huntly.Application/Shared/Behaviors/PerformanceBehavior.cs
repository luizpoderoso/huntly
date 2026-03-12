using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huntly.Application.Shared.Behaviors;

// Performance Behavior warns when a request takes more than a specified time to happen.
public class PerformanceBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next(ct);

        stopwatch.Stop();

        // 500ms is an arbitrary value and, naturally, will be changed after tests. Maybe it will be different
        // in each environment or depending on request type (Query or Command). 
        if (stopwatch.ElapsedMilliseconds > 500) 
            _logger.LogWarning(
                "Slow request detected: {RequestName} took {ElapsedMilliseconds}ms",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds);

        return response;
    }
}