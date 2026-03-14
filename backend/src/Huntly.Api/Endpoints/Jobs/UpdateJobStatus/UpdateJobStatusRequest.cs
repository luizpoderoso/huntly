using Huntly.Core.Jobs.Enums;

namespace Huntly.Api.Endpoints.Jobs.UpdateJobStatus;

public record UpdateJobStatusRequest(ApplicationStatus NewStatus);