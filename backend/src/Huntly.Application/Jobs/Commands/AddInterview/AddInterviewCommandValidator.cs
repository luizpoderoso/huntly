using FluentValidation;

namespace Huntly.Application.Jobs.Commands.AddInterview;

public class AddInterviewCommandValidator : AbstractValidator<AddInterviewCommand>
{
    public AddInterviewCommandValidator()
    {
        RuleFor(x => x.JobApplicationId)
            .NotEmpty().WithMessage("Job application ID is required.");
        
        RuleFor(x => x.InterviewType)
            .IsInEnum().WithMessage("Interview type is required.");
        
        RuleFor(x => x.ScheduledAt)
            .NotEmpty().WithMessage("Interview schedule is required.")
            .GreaterThan(_ => DateTime.UtcNow).WithMessage("Interview must be scheduled in the future.");;
    }
}