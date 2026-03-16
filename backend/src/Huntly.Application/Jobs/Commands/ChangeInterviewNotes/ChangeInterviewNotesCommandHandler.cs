using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Commands.ChangeInterviewNotes;

public class ChangeInterviewNotesCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext)
    : IRequestHandler<ChangeInterviewNotesCommand>
{
    public async Task Handle(ChangeInterviewNotesCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job == null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        var updated = job.ChangeInterviewNotes(command.InterviewId, command.NewInterviewNotes);
        
        if (!updated)
            throw new NotFoundException("Interview not found.");

        await atomicWork.CommitAsync(ct);
    }
}