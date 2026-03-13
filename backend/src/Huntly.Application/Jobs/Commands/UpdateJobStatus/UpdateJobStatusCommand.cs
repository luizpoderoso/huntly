using Huntly.Core.Job.Enums;
using MediatR;

namespace Huntly.Application.Jobs.Commands.UpdateJobStatus;

public record UpdateJobStatusCommand(
    Guid JobId,
    ApplicationStatus NewStatus
) : IRequest;