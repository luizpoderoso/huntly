using Huntly.Application.Shared.DTOs.Jobs;
using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using Mediator;

namespace Huntly.Application.Jobs.Commands.AddInterview;

public class AddInterviewCommandHandler(
    IJobApplicationRepository repository,
    IUserContext userContext,
    IAtomicWork atomicWork)
    : IRequestHandler<AddInterviewCommand, InterviewDto>
{
    public async ValueTask<InterviewDto> Handle(AddInterviewCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job is null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        var interview = job.AddInterview(command.InterviewType, command.ScheduledAt, command.InterviewNotes);
        await repository.AddInterviewAsync(interview, ct);

        await atomicWork.CommitAsync(ct);

        return new InterviewDto(
            interview.Id,
            interview.Type.ToString(),
            interview.ScheduledAt,
            interview.Outcome.ToString(),
            interview.Notes
        );
    }
}