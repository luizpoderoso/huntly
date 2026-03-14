using Huntly.Application.Jobs.Queries.DTOs;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Queries.GetAllJobs;

public class GetAllJobsQueryHandler(
    IJobApplicationRepository repository,
    IUserContext userContext)
    : IRequestHandler<GetAllJobsQuery, IReadOnlyCollection<JobSummaryDto>>
{
    public async Task<IReadOnlyCollection<JobSummaryDto>> Handle(GetAllJobsQuery query, CancellationToken ct)
    {
        var jobs = await repository.GetAllByUserIdAsync(userContext.UserId, ct);

        return jobs.Select(j => new JobSummaryDto(
            j.Id,
            j.CompanyName.Value,
            j.Position.Value,
            j.Status.ToString(),
            j.JobUrl?.Value,
            j.CreatedAt,
            j.UpdatedAt
            )).ToList().AsReadOnly();
    }
}