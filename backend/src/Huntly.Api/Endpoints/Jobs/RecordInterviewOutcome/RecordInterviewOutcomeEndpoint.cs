using FastEndpoints;
using Huntly.Application.Jobs.Commands.RecordInterviewOutcome;
using MediatR;

namespace Huntly.Api.Endpoints.Jobs.RecordInterviewOutcome;

public class RecordInterviewOutcomeEndpoint(IMediator mediator) : Endpoint<RecordInterviewOutcomeRequest>
{
    public override void Configure()
    {
        Patch("{jobId:guid}/interviews/{interviewId:guid}/outcome");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(RecordInterviewOutcomeRequest req, CancellationToken ct)
    {
        var jobId = Query<Guid>("jobId");
        var interviewId = Query<Guid>("interviewId");
        var command = new RecordInterviewOutcomeCommand(jobId, interviewId, req.NewInterviewOutcome);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}