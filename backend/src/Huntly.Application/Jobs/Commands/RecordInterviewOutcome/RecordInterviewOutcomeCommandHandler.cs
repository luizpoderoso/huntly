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
        
        var interview = job.Interviews.FirstOrDefault(x => x.Id == command.InterviewId);
        
        if (interview is null)
            throw new NotFoundException("Interview not found.");
        
        interview.RecordOutcome(command.NewInterviewOutcome);

        await atomicWork.CommitAsync(ct);
    }
}