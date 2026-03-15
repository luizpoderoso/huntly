namespace Huntly.Application.Shared.DTOs.Jobs;

public record JobSummaryDto(
    Guid Id,
    string CompanyName,
    string Position,
    string Status,
    string? JobUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);