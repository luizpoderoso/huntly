using FastEndpoints;
using Huntly.Application.Jobs.Commands.AddInterview;
using MediatR;

namespace Huntly.Api.Endpoints.Jobs.AddInterview;

public class AddInterviewEndpoint(IMediator mediator) : Endpoint<AddInterviewRequest>
{
    public override void Configure()
    {
        Post("{jobId:guid}/interviews");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(AddInterviewRequest req, CancellationToken ct)
    {
        var jobId = Route<Guid>("jobId");
        var command = new AddInterviewCommand(jobId, req.InterviewType, req.ScheduledAt, req.InterviewNotes);
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}