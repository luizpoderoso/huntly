using Mediator;

namespace Huntly.Application.Jobs.Commands.DeleteNote;

public record DeleteNoteCommand(
    Guid JobApplicationId,
    Guid NoteId
) : IRequest;