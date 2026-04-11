using FastEndpoints;
using Huntly.Application.Jobs.Commands.DeleteInterview;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.DeleteInterview;

public class DeleteInterviewEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("{jobId:guid}/interviews/{interviewId:guid}");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var jobId = Route<Guid>("jobId");
        var interviewId = Route<Guid>("interviewId");
        var command = new DeleteInterviewCommand(jobId, interviewId);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}