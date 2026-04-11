using Mediator;

namespace Huntly.Application.Jobs.Commands.DeleteJob;

public record DeleteJobCommand(Guid JobId) : IRequest;