using FastEndpoints;
using Huntly.Application.Jobs.Commands.CreateJob;
using Huntly.Application.Shared.DTOs.Jobs;
using MediatR;

namespace Huntly.Api.Endpoints.Jobs.CreateJob;

public class CreateJobEndpoint(IMediator mediator)
    : Endpoint<CreateJobRequest, JobSummaryDto>
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

        var jobSummaryDto = await mediator.Send(command, ct);
        await Send.OkAsync(jobSummaryDto, ct);
    }
}