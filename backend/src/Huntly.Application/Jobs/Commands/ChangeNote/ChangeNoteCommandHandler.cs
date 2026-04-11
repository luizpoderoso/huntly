using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using Mediator;

namespace Huntly.Application.Jobs.Commands.ChangeNote;

public class ChangeNoteCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext) 
    : IRequestHandler<ChangeNoteCommand, Unit>
{
    public async ValueTask<Unit> Handle(ChangeNoteCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job == null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        var updated = job.ChangeNoteContent(command.NoteId, command.NewNoteContent);

        if (!updated)
            throw new NotFoundException("Note not found.");

        await atomicWork.CommitAsync(ct);
        
        return Unit.Value;
    }
}