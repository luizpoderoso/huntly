using FluentValidation;

namespace Huntly.Application.Jobs.Commands.DeleteInterview;

public class DeleteInterviewCommandValidator : AbstractValidator<DeleteInterviewCommand>
{
    public DeleteInterviewCommandValidator()
    {
        RuleFor(x => x.JobApplicationId)
            .NotEmpty().WithMessage("Job application ID is required.");
        
        RuleFor(x => x.InterviewId)
            .NotEmpty().WithMessage("Interview ID is required.");
    }
}