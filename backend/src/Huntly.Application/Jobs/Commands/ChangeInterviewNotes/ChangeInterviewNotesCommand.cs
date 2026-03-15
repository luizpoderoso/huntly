using MediatR;

namespace Huntly.Application.Jobs.Commands.ChangeInterviewNotes;

public record ChangeInterviewNotesCommand(
    Guid JobApplicationId,
    Guid NoteId,
    string NewInterviewNotes
) : IRequest;