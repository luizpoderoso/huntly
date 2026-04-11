using Huntly.Application.Shared.DTOs.Jobs;
using Mediator;

namespace Huntly.Application.Jobs.Commands.AddNote;

public record AddNoteCommand(
    Guid JobApplicationId,
    string NoteContent
) : IRequest<NoteDto>;