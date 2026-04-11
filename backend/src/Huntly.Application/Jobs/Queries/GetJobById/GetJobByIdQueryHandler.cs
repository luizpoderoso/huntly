using Huntly.Application.Shared.DTOs.Jobs;
using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Repositories;
using Mediator;

namespace Huntly.Application.Jobs.Queries.GetJobById;

public class GetJobByIdQueryHandler(
    IJobApplicationRepository repository,
    IUserContext userContext) : IRequestHandler<GetJobByIdQuery, JobDetailDto>
{
    public async ValueTask<JobDetailDto> Handle(GetJobByIdQuery query, CancellationToken ct)
    {
        var job = await repository.GetByIdAsync(query.JobId, ct);
        
        if (job is null)
            throw new NotFoundException("Job application not found.");

        if (job.UserId != userContext.UserId)
            throw new NotFoundException("Job application not found.");
        
        return new JobDetailDto(
            job.Id,
            job.CompanyName.Value,
            job.Position.Value,
            job.Status.ToString(),
            job.JobUrl?.Value,
            job.SalaryRange?.Min,
            job.SalaryRange?.Max,
            job.SalaryRange?.Currency,
            job.CreatedAt,
            job.UpdatedAt,
            job.Interviews.Select(i => new InterviewDto(
                i.Id,
                i.Type.ToString(),
                i.ScheduledAt,
                i.Outcome.ToString(),
                i.Notes
            )).ToList().AsReadOnly(),
            job.Notes.Select(n => new NoteDto(
                n.Id,
                n.Content,
                n.CreatedAt
            )).ToList().AsReadOnly()
        );
    }
}