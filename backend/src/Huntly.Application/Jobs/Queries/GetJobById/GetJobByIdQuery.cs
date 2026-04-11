using Huntly.Application.Shared.DTOs.Jobs;
using Mediator;

namespace Huntly.Application.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(Guid JobId) : IRequest<JobDetailDto>;