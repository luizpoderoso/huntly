using MediatR;

namespace Huntly.Application.Jobs.Commands.DeleteInterview;

public record DeleteInterviewCommand(
    Guid JobApplicationId,
    Guid InterviewId
) : IRequest;