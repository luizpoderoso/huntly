using MediatR;

namespace Huntly.Application.Jobs.Commands.ChangeInterviewNotes;

public record ChangeInterviewNotesCommand(
    Guid JobApplicationId,
    Guid InterviewId,
    string NewInterviewNotes
) : IRequest;