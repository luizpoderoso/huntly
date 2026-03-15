using Huntly.Application.Shared.DTOs.Jobs;
using MediatR;

namespace Huntly.Application.Jobs.Queries.GetAllJobs;

public record GetAllJobsQuery : IRequest<IReadOnlyCollection<JobSummaryDto>>;