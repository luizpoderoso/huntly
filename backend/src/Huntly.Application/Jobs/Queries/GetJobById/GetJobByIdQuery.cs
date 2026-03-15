using Huntly.Application.Shared.DTOs.Jobs;
using MediatR;

namespace Huntly.Application.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(Guid JobId) : IRequest<JobDetailDto>;