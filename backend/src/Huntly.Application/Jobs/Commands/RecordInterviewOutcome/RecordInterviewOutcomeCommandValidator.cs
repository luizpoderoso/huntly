using FluentValidation;
using Huntly.Core.Jobs.Enums;

namespace Huntly.Application.Jobs.Commands.RecordInterviewOutcome;

public class RecordInterviewOutcomeCommandValidator : AbstractValidator<RecordInterviewOutcomeCommand>
{
    public RecordInterviewOutcomeCommandValidator()
    {
        RuleFor(x => x.JobApplicationId)
            .NotEmpty().WithMessage("Job application ID is required.");
        
        RuleFor(x => x.InterviewId)
            .NotEmpty().WithMessage("Interview ID is required.");
        
        RuleFor(x => x.NewInterviewOutcome)
            .IsInEnum().WithMessage("Interview outcome is invalid.")
            .NotEqual(InterviewOutcome.Pending).WithMessage("Cannot set outcome to Pending.");;
    }
}