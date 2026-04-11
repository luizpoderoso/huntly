using FastEndpoints;
using Huntly.Application.Jobs.Commands.ChangeInterviewNotes;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.ChangeInterviewNotes;

public class ChangeInterviewNotesEndpoint(IMediator mediator) : Endpoint<ChangeInterviewNotesRequest>
{
    public override void Configure()
    {
        Patch("{jobId:guid}/interviews/{interviewId:guid}/notes");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(ChangeInterviewNotesRequest req, CancellationToken ct)
    {
        var jobId = Route<Guid>("jobId");
        var interviewId = Route<Guid>("interviewId");
        var command = new ChangeInterviewNotesCommand(jobId, interviewId, req.NewInterviewNotes);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}