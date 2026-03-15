using Huntly.Application.Shared.DTOs.Jobs;
using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using MediatR;

namespace Huntly.Application.Jobs.Commands.AddNote;

public class AddNoteCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext)
    : IRequestHandler<AddNoteCommand, NoteDto>
{
    public async Task<NoteDto> Handle(AddNoteCommand command, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(command.JobApplicationId, ct);

        if (job == null || job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");

        var note = job.AddNote(command.NoteContent);

        await atomicWork.CommitAsync(ct);

        return new NoteDto(
            note.Id,
            note.Content,
            note.CreatedAt
        );
    }
}