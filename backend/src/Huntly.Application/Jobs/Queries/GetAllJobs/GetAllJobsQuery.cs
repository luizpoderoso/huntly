using Huntly.Application.Shared.DTOs.Jobs;
using Mediator;

namespace Huntly.Application.Jobs.Queries.GetAllJobs;

public record GetAllJobsQuery : IRequest<IReadOnlyCollection<JobSummaryDto>>;