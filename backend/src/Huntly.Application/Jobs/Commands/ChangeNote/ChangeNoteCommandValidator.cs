using FluentValidation;

namespace Huntly.Application.Jobs.Commands.ChangeNote;

public class ChangeNoteCommandValidator : AbstractValidator<ChangeNoteCommand>
{
    public ChangeNoteCommandValidator()
    {
        RuleFor(x => x.JobApplicationId)
            .NotEmpty().WithMessage("Job application ID is required.");
        
        RuleFor(x => x.NoteId)
            .NotEmpty().WithMessage("Note ID is required.");
        
        RuleFor(x => x.NewNoteContent)
            .NotEmpty().WithMessage("Note content is required.")
            .MaximumLength(2000).WithMessage("Note content must not exceed 2000 characters.");
    }
}