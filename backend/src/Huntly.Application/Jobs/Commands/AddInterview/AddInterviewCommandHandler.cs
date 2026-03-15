using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Commands.AddInterview;

public class AddInterviewCommandHandler(
    IJobApplicationRepository repository,
    IUserContext userContext,
    IAtomicWork atomicWork)
    : IRequestHandler<AddInterviewCommand, Guid>
{
    public async Task<Guid> Handle(AddInterviewCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job is null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        var interview = job.AddInterview(command.InterviewType, command.ScheduledAt, command.InterviewNotes);

        await atomicWork.CommitAsync(ct);

        return interview.Id;
    }
}