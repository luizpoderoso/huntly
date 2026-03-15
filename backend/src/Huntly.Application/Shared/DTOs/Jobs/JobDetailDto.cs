namespace Huntly.Application.Shared.DTOs.Jobs;

public record JobDetailDto(
    Guid Id,
    string CompanyName,
    string Position,
    string Status,
    string? JobUrl,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? SalaryCurrency,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyCollection<InterviewDto> Interviews,
    IReadOnlyCollection<NoteDto> Notes
);