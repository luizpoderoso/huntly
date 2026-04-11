using FastEndpoints;
using Huntly.Application.Jobs.Queries.GetAllJobs;
using Huntly.Application.Shared.DTOs.Jobs;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.GetJobs;

public class GetJobsEndpoint(IMediator mediator) : EndpointWithoutRequest<IReadOnlyCollection<JobSummaryDto>>
{
    public override void Configure()
    {
        Get("/");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetAllJobsQuery();
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result, ct);
    }
}