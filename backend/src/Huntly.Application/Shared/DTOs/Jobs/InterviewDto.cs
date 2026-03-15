namespace Huntly.Application.Shared.DTOs.Jobs;

public record InterviewDto(
    Guid Id,
    string Type,
    DateTime ScheduledAt,
    string Outcome,
    string? Notes
);