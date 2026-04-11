using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using Mediator;

namespace Huntly.Application.Jobs.Commands.DeleteJob;

public class DeleteJobCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext) 
    : IRequestHandler<DeleteJobCommand, Unit>
{
    public async ValueTask<Unit> Handle(DeleteJobCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobId, ct);

        if (job is null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        repository.Remove(job);
        await atomicWork.CommitAsync(ct);
        
        return Unit.Value;
    }
}