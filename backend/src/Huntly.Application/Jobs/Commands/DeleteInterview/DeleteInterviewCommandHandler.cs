using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Commands.DeleteInterview;

public class DeleteInterviewCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext) 
    : IRequestHandler<DeleteInterviewCommand>
{
    public async Task Handle(DeleteInterviewCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job == null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        var removed = job.RemoveInterview(command.InterviewId);
        
        if (!removed)
            throw new NotFoundException("Interview not found.");

        await atomicWork.CommitAsync(ct);
    }
}