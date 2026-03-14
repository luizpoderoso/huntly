namespace Huntly.Api.Endpoints.Jobs.CreateJob;

public record CreateJobRequest(
    string CompanyName,
    string Position,
    string? JobUrl,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? SalaryCurrency
);