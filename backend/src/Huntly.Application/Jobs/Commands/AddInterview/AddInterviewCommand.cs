using Huntly.Core.Jobs.Enums;
using MediatR;

namespace Huntly.Application.Jobs.Commands.AddInterview;

public record AddInterviewCommand(
    Guid JobApplicationId,
    InterviewType InterviewType,
    DateTime ScheduledAt,
    string? InterviewNotes
) : IRequest<Guid>;