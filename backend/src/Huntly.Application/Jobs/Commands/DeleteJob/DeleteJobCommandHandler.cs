using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Job.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Commands.DeleteJob;

public class DeleteJobCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext) 
    : IRequestHandler<DeleteJobCommand>
{
    public async Task Handle(DeleteJobCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobId, ct);

        if (job is null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        await repository.DeleteAsync(job, ct);
        await atomicWork.CommitAsync(ct);
    }
}