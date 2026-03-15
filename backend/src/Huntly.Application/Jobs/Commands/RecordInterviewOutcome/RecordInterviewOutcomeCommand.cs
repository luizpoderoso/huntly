using Huntly.Core.Jobs.Enums;
using MediatR;

namespace Huntly.Application.Jobs.Commands.RecordInterviewOutcome;

public record RecordInterviewOutcomeCommand(
    Guid JobApplicationId,
    Guid InterviewId,
    InterviewOutcome NewInterviewOutcome
) : IRequest;