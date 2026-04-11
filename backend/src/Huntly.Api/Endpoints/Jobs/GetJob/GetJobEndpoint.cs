using FastEndpoints;
using Huntly.Application.Jobs.Queries.GetJobById;
using Huntly.Application.Shared.DTOs.Jobs;
using Mediator;

namespace Huntly.Api.Endpoints.Jobs.GetJob;

public class GetJobEndpoint(IMediator mediator) : EndpointWithoutRequest<JobDetailDto>
{
    public override void Configure()
    {
        Get("/{id:guid}");
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var query = new GetJobByIdQuery(id);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result, ct);
    }
}