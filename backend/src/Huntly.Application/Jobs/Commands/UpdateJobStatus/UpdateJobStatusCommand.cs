using Huntly.Core.Jobs.Enums;
using MediatR;

namespace Huntly.Application.Jobs.Commands.UpdateJobStatus;

public record UpdateJobStatusCommand(
    Guid JobId,
    ApplicationStatus NewStatus
) : IRequest;