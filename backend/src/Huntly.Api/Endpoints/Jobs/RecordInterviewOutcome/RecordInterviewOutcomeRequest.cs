using Huntly.Core.Jobs.Enums;

namespace Huntly.Api.Endpoints.Jobs.RecordInterviewOutcome;

public record RecordInterviewOutcomeRequest(
    InterviewOutcome NewInterviewOutcome
);