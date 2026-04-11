using FastEndpoints;
using Huntly.Application.Jobs.Commands.DeleteNote;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.DeleteNote;

public class DeleteNoteEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("{jobId:guid}/notes/{noteId:guid}");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var jobId = Route<Guid>("jobId");
        var noteId = Route<Guid>("noteId");
        var command = new DeleteNoteCommand(jobId, noteId);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}