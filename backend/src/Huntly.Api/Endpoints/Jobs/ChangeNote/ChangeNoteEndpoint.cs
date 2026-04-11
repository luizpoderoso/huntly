using FastEndpoints;
using Huntly.Application.Jobs.Commands.ChangeNote;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.ChangeNote;

public class ChangeNoteEndpoint(IMediator mediator) : Endpoint<ChangeNoteRequest>
{
    public override void Configure()
    {
        Patch("{jobId:guid}/notes/{noteId:guid}");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(ChangeNoteRequest req, CancellationToken ct)
    {
        var jobId = Route<Guid>("jobId");
        var noteId = Route<Guid>("noteId");
        var command = new ChangeNoteCommand(jobId, noteId, req.NewNoteContent);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}