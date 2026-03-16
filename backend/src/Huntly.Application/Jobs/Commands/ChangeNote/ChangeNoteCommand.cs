using MediatR;

namespace Huntly.Application.Jobs.Commands.ChangeNote;

public record ChangeNoteCommand(
    Guid JobApplicationId,
    Guid NoteId,
    string NewNoteContent
) : IRequest;