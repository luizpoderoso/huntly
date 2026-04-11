using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using Mediator;

namespace Huntly.Application.Jobs.Commands.DeleteNote;

public class DeleteNoteCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext)
    : IRequestHandler<DeleteNoteCommand>
{
    public async ValueTask<Unit> Handle(DeleteNoteCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job == null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        var removed = job.RemoveNote(command.NoteId);
        
        if (!removed)
            throw new NotFoundException("Note not found.");

        await atomicWork.CommitAsync(ct);
        
        return Unit.Value;
    }
}