using FastEndpoints;
using Huntly.Application.Seed;
using MediatR;

namespace Huntly.Api.Endpoints.Seed;

public class SeedEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/seed");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new SeedCommand();
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}