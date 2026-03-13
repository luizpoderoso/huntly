using Huntly.Application.Jobs.Queries.DTOs;
using MediatR;

namespace Huntly.Application.Jobs.Queries.GetAllJobs;

public record GetAllJobsQuery : IRequest<IReadOnlyCollection<JobSummaryDto>>;