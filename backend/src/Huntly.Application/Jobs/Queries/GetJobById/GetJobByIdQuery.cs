using Huntly.Application.Jobs.Queries.DTOs;
using MediatR;

namespace Huntly.Application.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(Guid JobId) : IRequest<JobDetailDto>;