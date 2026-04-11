using FastEndpoints;
using Huntly.Application.Jobs.Commands.DeleteJob;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.DeleteJob;

public class DeleteJobEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/{id:guid}");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var command = new DeleteJobCommand(id);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}