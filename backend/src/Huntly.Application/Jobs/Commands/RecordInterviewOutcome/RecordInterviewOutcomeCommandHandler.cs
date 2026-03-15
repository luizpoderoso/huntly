using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Commands.RecordInterviewOutcome;

public class RecordInterviewOutcomeCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext)
    : IRequestHandler<RecordInterviewOutcomeCommand>
{
    public async Task Handle(RecordInterviewOutcomeCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job is null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");

        var updated = job.RecordInterviewOutcome(command.InterviewId, command.NewInterviewOutcome);
        
        if (!updated)
            throw new NotFoundException("Interview not found.");

        await atomicWork.CommitAsync(ct);
    }
}