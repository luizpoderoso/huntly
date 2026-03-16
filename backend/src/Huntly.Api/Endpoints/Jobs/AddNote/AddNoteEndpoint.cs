using FastEndpoints;
using Huntly.Application.Jobs.Commands.AddNote;
using MediatR;

namespace Huntly.Api.Endpoints.Jobs.AddNote;

public class AddNoteEndpoint(IMediator mediator) : Endpoint<AddNoteRequest>
{
    public override void Configure()
    {
        Post("{jobId:guid}/notes");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(AddNoteRequest req, CancellationToken ct)
    {
        var jobId = Route<Guid>("jobId");
        var command = new AddNoteCommand(jobId, req.NoteContent);
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}