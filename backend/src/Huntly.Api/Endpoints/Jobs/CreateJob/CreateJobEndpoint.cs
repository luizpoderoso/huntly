using FastEndpoints;
using Huntly.Application.Jobs.Commands.CreateJob;
using MediatR;

namespace Huntly.Api.Endpoints.Jobs.CreateJob;

public class CreateJobEndpoint(IMediator mediator)
    : Endpoint<CreateJobRequest, Guid>
{
    public override void Configure()
    {
        Post("/");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(CreateJobRequest req, CancellationToken ct)
    {
        var command = new CreateJobCommand(
            req.CompanyName,
            req.Position,
            req.JobUrl,
            req.SalaryMin,
            req.SalaryMax,
            req.SalaryCurrency);

        var jobId = await mediator.Send(command, ct);
        await Send.OkAsync(jobId, ct);
    }
}