using Huntly.Core.Jobs.Enums;

namespace Huntly.Api.Endpoints.Jobs.AddInterview;

public record AddInterviewRequest(
    InterviewType InterviewType,
    DateTime ScheduledAt,
    string? InterviewNotes
);