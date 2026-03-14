using FluentValidation;

namespace Huntly.Application.Jobs.Commands.CreateJob;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position is required.")
            .MaximumLength(200);

        RuleFor(x => x.JobUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                         && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .When(x => !string.IsNullOrEmpty(x.JobUrl))
            .WithMessage("Job URL must be a valid HTTP/HTTPS URL.");

        RuleFor(x => x.SalaryMin)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum salary cannot be negative.")
            .When(x => x.SalaryMin.HasValue);

        RuleFor(x => x.SalaryMax)
            .GreaterThanOrEqualTo(x => x.SalaryMin!.Value)
            .WithMessage("Maximum salary cannot be less than minimum.")
            .When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue);
        
        RuleFor(x => x.SalaryCurrency)
            .MaximumLength(3).WithMessage("Currency code cannot exceed 3 characters.")
            .Matches("^[A-Z]{3}$").WithMessage("Currency code must be a 3-letter ISO code (e.g. USD, BRL).")
            .When(x => x.SalaryCurrency is not null);
    }
}