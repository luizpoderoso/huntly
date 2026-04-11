using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using Mediator;

namespace Huntly.Application.Jobs.Commands.UpdateJobStatus;

public class UpdateJobStatusCommandHandler( 
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext) 
    : IRequestHandler<UpdateJobStatusCommand>
{
    public async ValueTask<Unit> Handle(UpdateJobStatusCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobId, ct);

        if (job is null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");

        job.UpdateStatus(command.NewStatus);

        await atomicWork.CommitAsync(ct);
        
        return Unit.Value;
    }
}