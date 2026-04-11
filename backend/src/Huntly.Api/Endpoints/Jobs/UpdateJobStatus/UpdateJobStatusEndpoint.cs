using FastEndpoints;
using Huntly.Application.Jobs.Commands.UpdateJobStatus;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.UpdateJobStatus;

public class UpdateJobStatusEndpoint(IMediator mediator) : Endpoint<UpdateJobStatusRequest>
{
    public override void Configure()
    {
        Patch("/{id:guid}/status");
        Group<JobsGroup>();
    }
    
    public override async Task HandleAsync(UpdateJobStatusRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var command = new UpdateJobStatusCommand(id, req.NewStatus);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}