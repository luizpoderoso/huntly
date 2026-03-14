namespace Huntly.Application.Jobs.Queries.DTOs;

public record InterviewDto(
    Guid Id,
    string Type,
    DateTime ScheduledAt,
    string Outcome,
    string? Notes
);