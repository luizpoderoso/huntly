using FluentValidation;

namespace Huntly.Application.Jobs.Commands.UpdateJobStatus;

public class UpdateJobStatusCommandValidator : AbstractValidator<UpdateJobStatusCommand>
{
    public UpdateJobStatusCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Job ID is required.");
        
        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Job status is invalid.");
    }
}