using Huntly.Core.Jobs.Enums;
using Mediator;

namespace Huntly.Application.Jobs.Commands.RecordInterviewOutcome;

public record RecordInterviewOutcomeCommand(
    Guid JobApplicationId,
    Guid InterviewId,
    InterviewOutcome NewInterviewOutcome
) : IRequest;