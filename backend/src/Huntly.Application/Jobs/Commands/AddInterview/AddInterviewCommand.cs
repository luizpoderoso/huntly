using Huntly.Application.Shared.DTOs.Jobs;
using Huntly.Core.Jobs.Enums;
using Mediator;

namespace Huntly.Application.Jobs.Commands.AddInterview;

public record AddInterviewCommand(
    Guid JobApplicationId,
    InterviewType InterviewType,
    DateTime ScheduledAt,
    string? InterviewNotes
) : IRequest<InterviewDto>;