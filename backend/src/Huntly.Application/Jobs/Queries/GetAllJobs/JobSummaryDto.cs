namespace Huntly.Application.Jobs.Queries.GetAllJobs;

public record JobSummaryDto(
    Guid Id,
    string CompanyName,
    string Position,
    string Status,
    string? JobUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);