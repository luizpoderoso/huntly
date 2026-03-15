using MediatR;

namespace Huntly.Application.Jobs.Commands.AddNote;

public record AddNoteCommand(
    Guid JobApplicationId,
    string NoteContent
) : IRequest<Guid>;