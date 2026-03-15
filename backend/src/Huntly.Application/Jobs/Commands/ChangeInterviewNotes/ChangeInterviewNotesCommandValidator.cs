using FluentValidation;

namespace Huntly.Application.Jobs.Commands.ChangeInterviewNotes;

public class ChangeInterviewNotesCommandValidator : AbstractValidator<ChangeInterviewNotesCommand>
{
    public ChangeInterviewNotesCommandValidator()
    {
        RuleFor(x => x.JobApplicationId)
            .NotEmpty().WithMessage("Job Application ID is required.");
        
        RuleFor(x => x.NoteId)
            .NotEmpty().WithMessage("Note ID is required.");
    }
}