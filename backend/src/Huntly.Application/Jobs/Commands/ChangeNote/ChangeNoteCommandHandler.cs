using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Commands.ChangeNote;

public class ChangeNoteCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext) 
    : IRequestHandler<ChangeNoteCommand>
{
    public async Task Handle(ChangeNoteCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);
        
        if (job == null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        var note = job.Notes.FirstOrDefault(x => x.Id == command.NoteId);
        
        if (note == null)
            throw new NotFoundException("Note not found.");
        
        note.ChangeContent(command.NewNoteContent);

        await atomicWork.CommitAsync(ct);
    }
}