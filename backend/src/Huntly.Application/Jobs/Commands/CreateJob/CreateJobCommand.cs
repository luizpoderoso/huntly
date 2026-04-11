using Huntly.Application.Shared.DTOs.Jobs;
using Mediator;

namespace Huntly.Application.Jobs.Commands.CreateJob;

public record CreateJobCommand(
    string CompanyName,
    string Position,
    string? JobUrl,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? SalaryCurrency = "USD"
) : IRequest<JobSummaryDto>;